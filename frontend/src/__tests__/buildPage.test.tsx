import { describe, it, expect, vi, afterEach, beforeEach } from "vitest";
import { render, screen, fireEvent, cleanup, within } from "@testing-library/react";
import type { RelicEffect } from "@/types";

// API は差し替える
const getRelicEffects = vi.fn();
vi.mock("@/lib/api", () => ({ getRelicEffects: (...args: unknown[]) => getRelicEffects(...args) }));

import BuildPage from "@/app/build/page";
import { TOTALS_EMPTY_MESSAGE } from "@/components/build/BuildTotalsPanel";

function effect(id: number, overrides: Partial<RelicEffect>): RelicEffect {
  return {
    id,
    categoryId: "attack",
    categoryNameJa: "攻撃力",
    categorySortOrder: 1,
    nameJa: `効果 ${id}`,
    isDeep: false,
    effectSign: "positive",
    calcType: "multiply",
    attribute: null,
    stackable: "yes",
    rankMin: 0,
    rankMax: 0,
    conditionJa: null,
    notesJa: null,
    ranks: [{ rank: 0, value: 1.05, unit: "multiplier", hasDemerit: false }],
    ...overrides,
  };
}

const MELEE = effect(18, { nameJa: "近接攻撃力上昇", attribute: "melee" });
const RUNE_DOWN = effect(70, {
  categoryId: "other",
  categoryNameJa: "その他",
  categorySortOrder: 7,
  nameJa: "取得ルーン減少",
  isDeep: true,
  effectSign: "negative",
  ranks: [{ rank: 0, value: 0.9, unit: "multiplier", hasDemerit: false }],
});

beforeEach(() => {
  getRelicEffects.mockReset();
});

afterEach(() => {
  cleanup();
});

describe("ビルドシミュレーターの画面(ビルドシミュレーター 3 章)", () => {
  it("読み込み中は 6 枠分の骨組みを出し、読み終えると 6 つの選択の欄と空の合計を出す", async () => {
    getRelicEffects.mockResolvedValue([MELEE, RUNE_DOWN]);
    render(<BuildPage />);

    expect(screen.getAllByLabelText("読み込み中")).toHaveLength(2);
    expect(await screen.findAllByRole("combobox")).toHaveLength(6);
    expect(screen.getByText(TOTALS_EMPTY_MESSAGE)).toBeTruthy();
  });

  it("効果を選ぶと合計が出て、すべて外すと 6 枠が空になり、API は 1 回だけ呼ばれる", async () => {
    getRelicEffects.mockResolvedValue([MELEE, RUNE_DOWN]);
    render(<BuildPage />);

    fireEvent.change(await screen.findByRole("combobox", { name: "通常効果 I" }), { target: { value: "18" } });
    fireEvent.change(screen.getByRole("combobox", { name: "深層効果 II" }), { target: { value: "70" } });

    expect(within(screen.getByRole("region", { name: "攻撃力" })).getByText("×1.05")).toBeTruthy();
    expect(within(screen.getByRole("region", { name: "その他" })).getByText("×0.90").className).toContain("text-ember");

    fireEvent.click(screen.getByRole("button", { name: "すべて外す" }));

    expect(screen.getAllByRole("combobox").every((select) => (select as HTMLSelectElement).value === "")).toBe(true);
    expect(screen.getByText(TOTALS_EMPTY_MESSAGE)).toBeTruthy();
    expect(getRelicEffects).toHaveBeenCalledTimes(1);
  });

  it("AC-BLD-08: 通常効果の枠の選択の欄に、深層効果が出ない", async () => {
    getRelicEffects.mockResolvedValue([MELEE, RUNE_DOWN]);
    render(<BuildPage />);

    const normal = await screen.findByRole("combobox", { name: "通常効果 I" });
    const deep = screen.getByRole("combobox", { name: "深層効果 I" });

    expect(within(normal).queryByRole("option", { name: "取得ルーン減少" })).toBeNull();
    expect(within(deep).getByRole("option", { name: "取得ルーン減少" })).toBeTruthy();
  });

  it("取得に失敗したら文言と「もう一度読み込む」を出し、押すと取り直す", async () => {
    getRelicEffects.mockRejectedValueOnce(new Error("サーバーで問題が起きました。時間をおいてやり直してください"));
    getRelicEffects.mockResolvedValueOnce([MELEE]);
    render(<BuildPage />);

    expect((await screen.findByRole("alert")).textContent).toContain("サーバーで問題が起きました");
    fireEvent.click(screen.getByRole("button", { name: "もう一度読み込む" }));

    expect(await screen.findAllByRole("combobox")).toHaveLength(6);
    expect(getRelicEffects).toHaveBeenCalledTimes(2);
  });
});
