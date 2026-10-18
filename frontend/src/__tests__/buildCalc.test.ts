import { describe, it, expect } from "vitest";
import type { RelicEffect, RelicEffectRank } from "@/types";
import {
  calcBuildTotals,
  describeEntries,
  emptySlots,
  formatRankValue,
  formatTotal,
  slotLabel,
  type BuildSlot,
} from "@/lib/buildCalc";

/** 分類の並び順。旧版の V11 のとおり */
const CATEGORY_ORDER: Record<string, [string, number]> = {
  stat: ["能力値", 0],
  attack: ["攻撃力", 1],
  recovery: ["回復", 3],
  other: ["その他", 7],
};

function effect(id: number, overrides: Partial<RelicEffect> & { ranks: RelicEffectRank[] }): RelicEffect {
  const categoryId = overrides.categoryId ?? "attack";
  const [categoryNameJa, categorySortOrder] = CATEGORY_ORDER[categoryId];
  return {
    id,
    categoryId,
    categoryNameJa,
    categorySortOrder,
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
    ...overrides,
  };
}

function rank(value: number | null, unit: RelicEffectRank["unit"], r = 0, hasDemerit = false): RelicEffectRank {
  return { rank: r, value, unit, hasDemerit };
}

const MELEE_105 = effect(1, { nameJa: "近接攻撃力上昇", attribute: "melee", ranks: [rank(1.05, "multiplier")] });
const MELEE_115 = effect(2, { nameJa: "近接攻撃力上昇(別)", attribute: "melee", ranks: [rank(1.15, "multiplier")] });
const SKILL_115 = effect(3, { nameJa: "戦技攻撃力上昇", attribute: "skill", ranks: [rank(1.15, "multiplier")] });
const PHYSICAL = effect(4, {
  nameJa: "物理攻撃力上昇+0〜2",
  calcType: "percent_add",
  attribute: "physical",
  rankMax: 2,
  ranks: [rank(4, "percent", 0), rank(5, "percent", 1), rank(6, "percent", 2)],
});
const PHYSICAL_DEEP = effect(5, {
  nameJa: "物理攻撃力上昇+0〜4（深層）",
  isDeep: true,
  calcType: "percent_add",
  attribute: "physical",
  rankMax: 4,
  ranks: [rank(4, "percent", 0), rank(5, "percent", 1), rank(6, "percent", 2), rank(10.5, "percent", 3, true), rank(12, "percent", 4, true)],
});
const HP_REGEN = effect(6, { categoryId: "recovery", nameJa: "HP持続回復", calcType: "flat_add", ranks: [rank(2, "per_sec")] });
const REGAIN = effect(7, {
  categoryId: "recovery",
  nameJa: "リゲイン有効化",
  calcType: "special",
  conditionJa: "ダメージの後",
  ranks: [rank(null, "none")],
});
const RUNE_DOWN = effect(8, {
  categoryId: "other",
  nameJa: "取得ルーン減少",
  isDeep: true,
  effectSign: "negative",
  calcType: "multiply",
  ranks: [rank(0.9, "multiplier")],
});
const STRENGTH = effect(9, {
  categoryId: "stat",
  nameJa: "筋力+1〜3",
  calcType: "flat_add",
  attribute: "strength",
  rankMin: 1,
  rankMax: 3,
  ranks: [rank(1, "flat", 1), rank(2, "flat", 2), rank(3, "flat", 3)],
});

const EFFECTS = new Map(
  [MELEE_105, MELEE_115, SKILL_115, PHYSICAL, PHYSICAL_DEEP, HP_REGEN, REGAIN, RUNE_DOWN, STRENGTH].map((e) => [e.id, e]),
);

/** 枠に効果を入れる。picks は [列, 位置, 効果, ランク] の並び */
function slots(...picks: [BuildSlot["kind"], number, RelicEffect, number?][]): BuildSlot[] {
  const result = emptySlots();
  for (const [kind, index, picked, r] of picks) {
    const slot = result.find((s) => s.kind === kind && s.index === index)!;
    slot.effectId = picked.id;
    slot.rank = r ?? picked.rankMin;
  }
  return result;
}

function rows(picked: BuildSlot[]) {
  return calcBuildTotals(picked, EFFECTS).flatMap((c) => c.rows);
}

describe("効果の枠(ビルドシミュレーター 3.1)", () => {
  it("空の状態は通常 3 枠と深層 3 枠で、どれも効果を選んでいない", () => {
    const empty = emptySlots();
    expect(empty.map(slotLabel)).toEqual(["通常 I", "通常 II", "通常 III", "深層 I", "深層 II", "深層 III"]);
    expect(empty.every((s) => s.effectId === null)).toBe(true);
  });

  it("何も選んでいなければ合計は空", () => {
    expect(calcBuildTotals(emptySlots(), EFFECTS)).toEqual([]);
  });
});

describe("効果の合計(ビルドシミュレーター 3.2)", () => {
  it("AC-BLD-02: multiply の 1.05 と 1.15 を同じ属性で選ぶと、合計が ×1.21 と出る", () => {
    const [row] = rows(slots(["normal", 0, MELEE_105], ["normal", 1, MELEE_115]));
    expect(formatTotal(row)).toBe("×1.21");
  });

  it("AC-BLD-03: percent_add の 4 と 6 を同じ属性で選ぶと、合計が +10% と出る", () => {
    const [row] = rows(slots(["normal", 0, PHYSICAL, 0], ["normal", 1, PHYSICAL, 2]));
    expect(formatTotal(row)).toBe("+10%");
  });

  it("AC-BLD-04: flat_add の per_sec の 2 を選ぶと、合計が +2/秒 と出る", () => {
    const [row] = rows(slots(["normal", 0, HP_REGEN]));
    expect(formatTotal(row)).toBe("+2/秒");
  });

  it("AC-BLD-05: special の効果を選ぶと、合計の数ではなく効果の名前と発動の条件が出る", () => {
    const [row] = rows(slots(["normal", 0, REGAIN]));
    expect(row.total).toBeNull();
    expect(formatTotal(row)).toBe("");
    expect(row.nameJa).toBe("リゲイン有効化");
    expect(row.conditionJa).toBe("ダメージの後");
  });

  it("AC-BLD-06: 属性の違う 2 つの multiply は、別の行に出る", () => {
    const result = rows(slots(["normal", 0, MELEE_105], ["normal", 1, SKILL_115]));
    expect(result.map(formatTotal)).toEqual(["×1.05", "×1.15"]);
  });

  it("AC-BLD-09: 取得ルーン減少(×0.9)を選ぶと、合計が負の色の行になる", () => {
    const [row] = rows(slots(["deep", 0, RUNE_DOWN]));
    expect(formatTotal(row)).toBe("×0.90");
    expect(row.isNegative).toBe(true);
  });

  it("通常効果と深層効果でも、計算の種類と属性が同じなら 1 行にまとめ、名前からランクの幅と深層を除く", () => {
    const result = rows(slots(["normal", 0, PHYSICAL, 0], ["deep", 0, PHYSICAL_DEEP, 3]));
    expect(result).toHaveLength(1);
    expect(result[0].nameJa).toBe("物理攻撃力上昇");
    expect(formatTotal(result[0])).toBe("+14.5%");
    expect(describeEntries(result[0].entries)).toBe("通常 I のランク 0、深層 I のランク 3");
  });

  it("ランクのない効果の内訳は、枠と値を出す", () => {
    const [row] = rows(slots(["normal", 0, MELEE_105], ["normal", 2, MELEE_115]));
    expect(describeEntries(row.entries)).toBe("通常 I の ×1.05、通常 III の ×1.15");
  });

  it("分類は分類の並び順で区切る", () => {
    const result = calcBuildTotals(slots(["deep", 0, RUNE_DOWN], ["normal", 0, MELEE_105], ["normal", 1, STRENGTH, 3]), EFFECTS);
    expect(result.map((c) => c.nameJa)).toEqual(["能力値", "攻撃力", "その他"]);
    expect(formatTotal(result[0].rows[0])).toBe("+3");
  });

  it("負の足し算は - を付けて出す", () => {
    expect(formatTotal({ calcType: "percent_add", unit: "percent", total: -35 })).toBe("-35%");
    expect(formatTotal({ calcType: "flat_add", unit: "flat", total: -80 })).toBe("-80");
    expect(formatTotal({ calcType: "flat_add", unit: "per_sec", total: -2 })).toBe("-2/秒");
  });
});

describe("ランクの値の表示", () => {
  it("単位ごとに記号を付け、数のない効果は空文字", () => {
    expect(formatRankValue(1.05, "multiplier")).toBe("×1.05");
    expect(formatRankValue(10.5, "percent")).toBe("+10.5%");
    expect(formatRankValue(-3, "flat")).toBe("-3");
    expect(formatRankValue(2, "per_sec")).toBe("+2/秒");
    expect(formatRankValue(null, "none")).toBe("");
  });
});
