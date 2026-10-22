import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup, within, act } from "@testing-library/react";
import { clearAdminKey, saveAdminKey } from "@/lib/adminKey";
import type { AdminPlaceKind } from "@/types";

// 管理用の API は差し替える
const adminListPlaces = vi.fn();
const adminUpdatePlace = vi.fn();
const adminCreatePlace = vi.fn();
vi.mock("@/lib/api", () => ({
  adminListPlaces: (...args: unknown[]) => adminListPlaces(...args),
  adminCreatePlace: (...args: unknown[]) => adminCreatePlace(...args),
  adminUpdatePlace: (...args: unknown[]) => adminUpdatePlace(...args),
}));
vi.mock("@/hooks/useMasterData", () => ({
  useMasterData: () => ({ bosses: [], jihenList: [], spawnPoints: [], loading: false, error: null }),
}));
// マップは Konva で描くので、jsdom では枠だけに差し替える
vi.mock("@/components/map/MapCanvas", () => ({ default: () => <div data-testid="map" /> }));

import AdminPage from "@/app/admin/page";

const spawn = { id: 9, nameJa: "南の教会", x: 500, y: 900, isHidden: false };

beforeEach(() => {
  adminListPlaces.mockReset().mockImplementation((kind: AdminPlaceKind) => Promise.resolve(kind === "spawn-points" ? [spawn] : []));
  adminUpdatePlace.mockReset();
  adminCreatePlace.mockReset();
  sessionStorage.clear();
});

afterEach(() => {
  cleanup();
});

describe("マスタの管理の画面(マスタの管理 3 章)", () => {
  it("AC-ADM-15: 鍵のない状態で開くと、鍵の入力の欄だけが出て、マップが出ない", () => {
    render(<AdminPage />);

    expect(screen.getByLabelText("管理用の鍵", { selector: "input" })).toBeTruthy();
    expect(screen.queryByTestId("map")).toBeNull();
    expect(adminListPlaces).not.toHaveBeenCalled();
  });

  it("AC-ADM-08: 「非表示にする」を押すと確かめの枠が出て、確かめると一覧の行に「非表示」の印が付く", async () => {
    saveAdminKey("correct-admin-key-0001");
    adminUpdatePlace.mockResolvedValue({ ...spawn, isHidden: true });
    render(<AdminPage />);

    fireEvent.click(await screen.findByRole("button", { name: "南の教会" }));
    fireEvent.click(screen.getByRole("button", { name: "非表示にする" }));
    const dialog = screen.getByRole("dialog", { name: "開始地点を非表示にしますか" });
    fireEvent.click(within(dialog).getByRole("button", { name: "非表示にする" }));

    expect((await screen.findByRole("button", { name: /南の教会/, current: true })).textContent).toContain("非表示");
    expect(screen.queryByRole("dialog")).toBeNull();
    expect(screen.getByRole("button", { name: "表示に戻す" })).toBeTruthy();
    expect(adminUpdatePlace).toHaveBeenCalledWith("spawn-points", 9, { nameJa: "南の教会", x: 500, y: 900, isHidden: true }, "correct-admin-key-0001");
  });

  it("AC-ADM-29: 拠点候補地で種類に「強敵」を選んで保存すると、kind を field_boss で送り、一覧の行に「強敵」と出る", async () => {
    saveAdminKey("correct-admin-key-0001");
    adminCreatePlace.mockResolvedValue({ id: 1001, mapId: "normal", kind: "field_boss", nameJa: null, x: 400, y: 300, isHidden: false });
    render(<AdminPage />);

    fireEvent.click(await screen.findByRole("button", { name: "拠点候補地" }));
    fireEvent.change(screen.getByLabelText("種類"), { target: { value: "field_boss" } });
    fireEvent.change(screen.getByLabelText("X"), { target: { value: "400" } });
    fireEvent.change(screen.getByLabelText("Y"), { target: { value: "300" } });
    fireEvent.click(screen.getByRole("button", { name: "保存する" }));

    expect(await screen.findByRole("button", { name: "1001 番" })).toBeTruthy();
    expect(screen.getByRole("cell", { name: "強敵" })).toBeTruthy();
    expect(adminCreatePlace).toHaveBeenCalledWith(
      "base-points",
      { mapId: "normal", kind: "field_boss", nameJa: null, x: 400, y: 300, isHidden: undefined },
      "correct-admin-key-0001"
    );
  });

  it("確かめの枠で「やめる」を押すと、送らずに閉じる", async () => {
    saveAdminKey("correct-admin-key-0001");
    render(<AdminPage />);

    fireEvent.click(await screen.findByRole("button", { name: "南の教会" }));
    fireEvent.click(screen.getByRole("button", { name: "非表示にする" }));
    fireEvent.click(screen.getByRole("button", { name: "やめる" }));

    expect(screen.queryByRole("dialog")).toBeNull();
    expect(adminUpdatePlace).not.toHaveBeenCalled();
  });

  it("鍵を消して入力の欄に戻っても、入れかけた名前と座標を残す(マスタの管理 3.1)", async () => {
    saveAdminKey("correct-admin-key-0001");
    render(<AdminPage />);
    fireEvent.click(await screen.findByRole("button", { name: "南の教会" }));
    fireEvent.change(screen.getByLabelText("名前"), { target: { value: "南の教会(直し)" } });

    act(() => clearAdminKey());
    expect(screen.queryByTestId("map")).toBeNull();
    act(() => saveAdminKey("correct-admin-key-0001"));

    expect(((await screen.findByLabelText("名前")) as HTMLInputElement).value).toBe("南の教会(直し)");
    expect((screen.getByLabelText("X") as HTMLInputElement).value).toBe("500.00");
  });
});
