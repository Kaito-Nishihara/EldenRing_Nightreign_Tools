import { describe, it, expect, vi, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup } from "@testing-library/react";
import SiteHeader from "@/components/layout/SiteHeader";
import Button from "@/components/ui/Button";
import SegmentedControl from "@/components/ui/SegmentedControl";
import PatternChip from "@/components/ui/PatternChip";
import { ErrorNotice } from "@/components/ui/StatusViews";

// ヘッダーは今のパスで入口の状態を変えるので、パスを差し替えられるようにする
const pathname = { current: "/" };
vi.mock("next/navigation", () => ({ usePathname: () => pathname.current }));

afterEach(() => {
  cleanup();
});

describe("SiteHeader(画面の共通仕様 3.6)", () => {
  it("3 つの入口を出し、今いる画面の入口にだけ aria-current を付ける", () => {
    pathname.current = "/record";
    render(<SiteHeader />);

    const links = screen.getAllByRole("link").filter((link) => link.closest("nav"));
    expect(links.map((link) => link.textContent)).toEqual(["パターン", "記録する", "ビルド"]);
    expect(screen.getByRole("link", { name: "記録する" }).getAttribute("aria-current")).toBe("page");
    expect(screen.getByRole("link", { name: "パターン" }).getAttribute("aria-current")).toBeNull();
  });

  it("AC-NAV-05: 管理の画面への入口を出さない。管理の画面では入口を出さず ADMIN の印を出す", () => {
    pathname.current = "/";
    render(<SiteHeader />);
    expect(screen.queryByRole("link", { name: /管理/ })).toBeNull();
    cleanup();

    pathname.current = "/admin";
    render(<SiteHeader />);
    expect(screen.queryByRole("navigation")).toBeNull();
    expect(screen.getByText("ADMIN")).toBeTruthy();
  });
});

describe("共通の部品(画面の共通仕様 3.7)", () => {
  it("ボタンは既定で type=button になり、フォームを勝手に送らない", () => {
    render(<Button>押す</Button>);

    expect(screen.getByRole("button", { name: "押す" }).getAttribute("type")).toBe("button");
  });

  it("切り替えは選んだものに aria-pressed を付け、押すと値を知らせる", () => {
    const onChange = vi.fn();
    render(
      <SegmentedControl
        ariaLabel="日"
        value="1"
        onChange={onChange}
        options={[
          { value: "1", label: "DAY I", display: true },
          { value: "2", label: "DAY II", display: true },
        ]}
      />,
    );

    expect(screen.getByRole("button", { name: "DAY I" }).getAttribute("aria-pressed")).toBe("true");
    expect(screen.getByRole("button", { name: "DAY II" }).getAttribute("aria-pressed")).toBe("false");
    fireEvent.click(screen.getByRole("button", { name: "DAY II" }));
    expect(onChange).toHaveBeenCalledWith("2");
  });

  it("AC-UI-08: パターンの丸は、色だけでなく番号も出す", () => {
    render(<PatternChip patternIndex={3} />);

    expect(screen.getByLabelText("パターン 3").textContent).toBe("3");
  });

  it("AC-UI-09: エラーの知らせは role=alert で文言を出し、「もう一度読み込む」で読み直せる", () => {
    const onRetry = vi.fn();
    render(<ErrorNotice message="サーバーに接続できません。時間をおいてやり直してください" onRetry={onRetry} />);

    expect(screen.getByRole("alert").textContent).toContain("サーバーに接続できません。時間をおいてやり直してください");
    fireEvent.click(screen.getByRole("button", { name: "もう一度読み込む" }));
    expect(onRetry).toHaveBeenCalledOnce();
  });
});
