import { describe, it, expect, vi, afterEach, beforeEach } from "vitest";
import { render, screen, fireEvent, cleanup, renderHook, act, waitFor, within } from "@testing-library/react";
import type { RelicEffect, RelicEffectRank } from "@/types";

// API は差し替える
const getRelicEffects = vi.fn();
vi.mock("@/lib/api", () => ({ getRelicEffects: (...args: unknown[]) => getRelicEffects(...args) }));

import BuildEffectSlot, { DEMERIT_MESSAGE } from "@/components/build/BuildEffectSlot";
import BuildTotalsPanel, { TOTALS_EMPTY_MESSAGE } from "@/components/build/BuildTotalsPanel";
import { useRelicEffects } from "@/hooks/useRelicEffects";
import { useBuildSlots } from "@/hooks/useBuildSlots";
import { calcBuildTotals, emptySlots, type BuildSlot } from "@/lib/buildCalc";

function rank(r: number, value: number | null, unit: RelicEffectRank["unit"], hasDemerit = false): RelicEffectRank {
  return { rank: r, value, unit, hasDemerit };
}

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
    ranks: [rank(0, 1.05, "multiplier")],
    ...overrides,
  };
}

const MELEE = effect(18, { nameJa: "近接攻撃力上昇", attribute: "melee" });
const HP = effect(1, {
  categoryId: "stat",
  categoryNameJa: "能力値",
  categorySortOrder: 0,
  nameJa: "最大HP上昇",
  calcType: "flat_add",
  ranks: [rank(0, 100, "flat")],
});
const PHYSICAL_DEEP = effect(57, {
  nameJa: "物理攻撃力上昇+0〜4（深層）",
  isDeep: true,
  calcType: "percent_add",
  attribute: "physical",
  rankMax: 4,
  ranks: [rank(0, 4, "percent"), rank(1, 5, "percent"), rank(2, 6, "percent"), rank(3, 10.5, "percent", true), rank(4, 12, "percent", true)],
});
const RUNE_DOWN = effect(70, {
  categoryId: "other",
  categoryNameJa: "その他",
  categorySortOrder: 7,
  nameJa: "取得ルーン減少",
  isDeep: true,
  effectSign: "negative",
  ranks: [rank(0, 0.9, "multiplier")],
});
const ALL = [RUNE_DOWN, MELEE, PHYSICAL_DEEP, HP];

beforeEach(() => {
  getRelicEffects.mockReset();
});

afterEach(() => {
  cleanup();
});

function deepSlot(index: number, picked: RelicEffect | null, r = 0): BuildSlot {
  return { kind: "deep", index, effectId: picked?.id ?? null, rank: r };
}

describe("useRelicEffects(ビルドシミュレーター 6 章)", () => {
  it("AC-BLD-08: 通常効果の枠の選択肢に深層効果が出ず、分類は分類の並び順に並ぶ", async () => {
    getRelicEffects.mockResolvedValue(ALL);
    const { result } = renderHook(() => useRelicEffects());

    await waitFor(() => expect(result.current.loading).toBe(false));

    const normalIds = result.current.normalOptions.flatMap((g) => g.effects.map((e) => e.id));
    expect(normalIds).toEqual([HP.id, MELEE.id]);
    expect(result.current.normalOptions.map((g) => g.nameJa)).toEqual(["能力値", "攻撃力"]);
    expect(result.current.deepOptions.flatMap((g) => g.effects.map((e) => e.id))).toEqual([PHYSICAL_DEEP.id, RUNE_DOWN.id]);
  });

  it("AC-BLD-10: 効果を選び直しても /api/v1/relic-effects が再び呼ばれない", async () => {
    getRelicEffects.mockResolvedValue(ALL);
    const { result } = renderHook(() => {
      const effects = useRelicEffects();
      return { effects, slots: useBuildSlots(effects.effectMap) };
    });
    await waitFor(() => expect(result.current.effects.loading).toBe(false));

    act(() => result.current.slots.chooseEffect({ kind: "normal", index: 0 }, MELEE.id));
    act(() => result.current.slots.chooseEffect({ kind: "deep", index: 0 }, PHYSICAL_DEEP.id));
    act(() => result.current.slots.chooseRank({ kind: "deep", index: 0 }, 3));

    expect(result.current.slots.totals).toHaveLength(1);
    expect(getRelicEffects).toHaveBeenCalledTimes(1);
  });

  it("取得に失敗したら文言を返し、retry で取り直す", async () => {
    getRelicEffects.mockRejectedValueOnce(new Error("サーバーに接続できません。時間をおいてやり直してください"));
    getRelicEffects.mockResolvedValueOnce(ALL);
    const { result } = renderHook(() => useRelicEffects());

    await waitFor(() => expect(result.current.error).toBe("サーバーに接続できません。時間をおいてやり直してください"));
    act(() => result.current.retry());

    await waitFor(() => expect(result.current.effects).toHaveLength(4));
    expect(result.current.error).toBeNull();
  });
});

describe("useBuildSlots(ビルドシミュレーター 3.1)", () => {
  it("効果を選ぶとランクは rankMin になり、すべて外すと 6 枠が空になる", () => {
    const map = new Map(ALL.map((e) => [e.id, e]));
    const { result } = renderHook(() => useBuildSlots(map));

    act(() => result.current.chooseEffect({ kind: "deep", index: 1 }, PHYSICAL_DEEP.id));
    expect(result.current.slots.find((s) => s.kind === "deep" && s.index === 1)).toMatchObject({ effectId: 57, rank: 0 });

    act(() => result.current.clearAll());
    expect(result.current.slots).toEqual(emptySlots());
  });
});

describe("効果の枠(ビルドシミュレーター 3.1)", () => {
  const options = [{ categoryId: "attack", nameJa: "攻撃力", effects: [PHYSICAL_DEEP] }];

  it("AC-BLD-07: 深層効果「物理攻撃力上昇+0〜4(深層)」でランク 3 を選ぶと、デメリットの知らせが出る", () => {
    render(<BuildEffectSlot slot={deepSlot(0, PHYSICAL_DEEP, 3)} options={options} effect={PHYSICAL_DEEP} onEffectChange={vi.fn()} onRankChange={vi.fn()} />);

    expect(screen.getByText(`◆ ${DEMERIT_MESSAGE}`).className).toContain("text-ember");
    expect(screen.getByRole("button", { name: "3" }).getAttribute("aria-pressed")).toBe("true");
  });

  it("ランク 2 ではデメリットの知らせが出ず、ランクのボタンを押すとそのランクを伝える", () => {
    const onRankChange = vi.fn();
    render(<BuildEffectSlot slot={deepSlot(0, PHYSICAL_DEEP, 2)} options={options} effect={PHYSICAL_DEEP} onEffectChange={vi.fn()} onRankChange={onRankChange} />);

    expect(screen.queryByText(`◆ ${DEMERIT_MESSAGE}`)).toBeNull();
    fireEvent.click(screen.getByRole("button", { name: "4" }));
    expect(onRankChange).toHaveBeenCalledWith(4);
  });

  it("デメリットの効果では、枠の下の説明を ember の色にする", () => {
    render(<BuildEffectSlot slot={deepSlot(1, RUNE_DOWN)} options={options} effect={RUNE_DOWN} onEffectChange={vi.fn()} onRankChange={vi.fn()} />);

    expect(screen.getByText("その他 ／ 掛け算 ×0.9").className).toContain("text-ember");
    expect(screen.queryByRole("group", { name: /ランク/ })).toBeNull();
  });

  it("空きを選ぶと null を伝える", () => {
    const onEffectChange = vi.fn();
    render(<BuildEffectSlot slot={deepSlot(0, PHYSICAL_DEEP)} options={options} effect={PHYSICAL_DEEP} onEffectChange={onEffectChange} onRankChange={vi.fn()} />);

    fireEvent.change(screen.getByRole("combobox", { name: "深層効果 I" }), { target: { value: "" } });
    expect(onEffectChange).toHaveBeenCalledWith(null);
  });
});

describe("効果の合計(ビルドシミュレーター 3.2、3.3)", () => {
  it("何も選んでいなければ「効果を選ぶと、ここに合計が出ます」", () => {
    render(<BuildTotalsPanel totals={[]} />);

    expect(screen.getByText(TOTALS_EMPTY_MESSAGE)).toBeTruthy();
  });

  it("AC-BLD-09: 取得ルーン減少(×0.9)を選ぶと、合計が ember の色で出る", () => {
    const totals = calcBuildTotals([deepSlot(1, RUNE_DOWN)], new Map([[RUNE_DOWN.id, RUNE_DOWN]]));
    render(<BuildTotalsPanel totals={totals} />);

    const section = screen.getByRole("region", { name: "その他" });
    expect(within(section).getByText("×0.90").className).toContain("text-ember");
    expect(within(section).getByText("深層 II の ×0.9")).toBeTruthy();
  });
});
