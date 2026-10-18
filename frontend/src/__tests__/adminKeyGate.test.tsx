import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup, waitFor, act } from "@testing-library/react";
import { ApiError } from "@/lib/apiError";
import { ADMIN_KEY_STORAGE_KEY, saveAdminKey } from "@/lib/adminKey";

// 管理用の API は差し替える
const adminListPlaces = vi.fn();
vi.mock("@/lib/api", () => ({ adminListPlaces: (...args: unknown[]) => adminListPlaces(...args) }));

const pathname = { current: "/admin" };
vi.mock("next/navigation", () => ({ usePathname: () => pathname.current }));

import AdminKeyGate from "@/components/admin/AdminKeyGate";
import SiteHeader from "@/components/layout/SiteHeader";

beforeEach(() => {
  adminListPlaces.mockReset();
  sessionStorage.clear();
});

afterEach(() => {
  cleanup();
});

function enterKey(key: string) {
  fireEvent.change(screen.getByLabelText("管理用の鍵", { selector: "input" }), { target: { value: key } });
  fireEvent.click(screen.getByRole("button", { name: "入る" }));
}

describe("AdminKeyGate(マスタの管理 3.1)", () => {
  it("鍵の入力の欄は type=password で、空のうちは「入る」を押せない", () => {
    render(<AdminKeyGate />);

    expect(screen.getByLabelText("管理用の鍵", { selector: "input" }).getAttribute("type")).toBe("password");
    expect((screen.getByRole("button", { name: "入る" }) as HTMLButtonElement).disabled).toBe(true);
  });

  it("AC-ADM-16: 誤った鍵で「入る」を押すと「管理用の鍵が違います」と出て、鍵を置かない", async () => {
    adminListPlaces.mockRejectedValue(new ApiError(401, "この操作は許可されていません"));
    render(<AdminKeyGate />);

    enterKey("wrong-key-0000000");

    expect((await screen.findByRole("alert")).textContent).toContain("管理用の鍵が違います");
    expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBeNull();
    expect(screen.getByLabelText("管理用の鍵", { selector: "input" })).toBeTruthy();
  });

  it("合う鍵で「入る」を押すと、鍵を付けて開始地点の一覧を呼び、鍵を sessionStorage に置く", async () => {
    adminListPlaces.mockResolvedValue([]);
    render(<AdminKeyGate />);

    enterKey("correct-admin-key-0001");

    await waitFor(() => expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBe("correct-admin-key-0001"));
    expect(adminListPlaces).toHaveBeenCalledWith("spawn-points", "correct-admin-key-0001");
  });

  it("回数の制限にかかったときは、Gateway の文言をそのまま出す", async () => {
    adminListPlaces.mockRejectedValue(new ApiError(429, "しばらく時間をおいてやり直してください"));
    render(<AdminKeyGate />);

    enterKey("correct-admin-key-0001");

    expect((await screen.findByRole("alert")).textContent).toContain("しばらく時間をおいてやり直してください");
  });
});

describe("SiteHeader の「鍵を消す」(マスタの管理 3.1)", () => {
  it("鍵がないときは「鍵を消す」を出さない", () => {
    render(<SiteHeader />);

    expect(screen.queryByRole("button", { name: "鍵を消す" })).toBeNull();
  });

  it("AC-ADM-17: 「鍵を消す」を押すと sessionStorage から鍵が消え、ボタンも消える", () => {
    saveAdminKey("correct-admin-key-0001");
    render(<SiteHeader />);

    fireEvent.click(screen.getByRole("button", { name: "鍵を消す" }));

    expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBeNull();
    expect(screen.queryByRole("button", { name: "鍵を消す" })).toBeNull();
  });

  it("管理の画面の外では、鍵を置いていても「鍵を消す」を出さない", () => {
    pathname.current = "/";
    act(() => saveAdminKey("correct-admin-key-0001"));
    render(<SiteHeader />);

    expect(screen.queryByRole("button", { name: "鍵を消す" })).toBeNull();
    pathname.current = "/admin";
  });
});
