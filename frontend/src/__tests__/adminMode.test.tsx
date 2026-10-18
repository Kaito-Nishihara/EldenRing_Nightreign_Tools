import { describe, it, expect, vi, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup } from "@testing-library/react";
import AdminModePanel from "@/components/admin/AdminModePanel";
import ConfirmDialog from "@/components/ui/ConfirmDialog";
import { HIDE_DESCRIPTION } from "@/components/admin/AdminPlaceForm";

afterEach(() => {
  cleanup();
});

describe("AdminModePanel(マスタの管理 3.2)", () => {
  it("4 つの種類のボタンを出し、選んだものに aria-pressed を付ける", () => {
    const onKindChange = vi.fn();
    render(<AdminModePanel kind="shrink-zones" mapId="normal" jihenList={[]} onKindChange={onKindChange} onMapChange={vi.fn()} />);

    const buttons = screen.getAllByRole("button");
    fireEvent.click(screen.getByRole("button", { name: "拠点候補地" }));

    expect(buttons.map((b) => b.textContent)).toEqual(["開始地点", "収縮候補地", "第二収縮地点", "拠点候補地"]);
    expect(screen.getByRole("button", { name: "収縮候補地" }).getAttribute("aria-pressed")).toBe("true");
    expect(onKindChange).toHaveBeenCalledWith("base-points");
  });
});

describe("ConfirmDialog", () => {
  it("確かめると onConfirm を、やめるか Esc で onCancel を呼ぶ", () => {
    const onConfirm = vi.fn();
    const onCancel = vi.fn();
    render(<ConfirmDialog title="非表示にしますか" message={HIDE_DESCRIPTION} confirmLabel="非表示にする" onConfirm={onConfirm} onCancel={onCancel} />);

    expect(screen.getByRole("dialog", { name: "非表示にしますか" })).toBeTruthy();
    expect(document.activeElement?.textContent).toBe("やめる");
    fireEvent.click(screen.getByRole("button", { name: "非表示にする" }));
    fireEvent.keyDown(window, { key: "Escape" });

    expect(onConfirm).toHaveBeenCalledTimes(1);
    expect(onCancel).toHaveBeenCalledTimes(1);
  });
});
