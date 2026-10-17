import type { RelicEffect, BuildRelicSlot, BuildResultGroup } from "@/types";

/**
 * ビルドシミュレーター計算ロジック
 *
 * calc_type ごとの合算方法:
 *   multiply    … 全値の積 (例: 1.05 × 1.15 = 1.2075)
 *   percent_add … %ポイントを加算 (例: 4 + 6 = +10%)
 *   flat_add    … 固定値を加算 (例: 100 + 100 = +200)
 *   special     … 個別表示のみ（合算なし）
 */

interface EffectContribution {
  effectId: number;
  nameJa: string;
  categoryId: string;
  categoryNameJa: string;
  calcType: RelicEffect["calcType"];
  attribute: string | null;
  value: number;
  unit: string;
}

/** スロット一覧から全効果の寄与リストを展開する */
function collectContributions(
  slots: BuildRelicSlot[],
  effectMap: Map<number, RelicEffect>
): EffectContribution[] {
  const contributions: EffectContribution[] = [];

  for (const slot of slots) {
    for (const entry of slot.effects) {
      if (entry.effectId === null) continue;
      const effect = effectMap.get(entry.effectId);
      if (!effect) continue;

      const rankData = effect.ranks.find((r) => r.rank === entry.rank);
      if (!rankData || rankData.unit === "none") continue;

      contributions.push({
        effectId: effect.id,
        nameJa: effect.nameJa,
        categoryId: effect.categoryId,
        categoryNameJa: effect.categoryNameJa,
        calcType: effect.calcType,
        attribute: effect.attribute,
        value: rankData.value,
        unit: rankData.unit,
      });
    }
  }

  return contributions;
}

/** グループキーを生成する（attribute優先、なければeffectId） */
function groupKey(c: EffectContribution): string {
  if (c.calcType === "special") return `special_${c.effectId}`;
  if (c.attribute) return `${c.calcType}_${c.attribute}`;
  return `${c.calcType}_eff_${c.effectId}`;
}

/** multiply 値群を積算して合計倍率を返す */
function combineMultiply(values: number[]): number {
  return values.reduce((acc, v) => acc * v, 1);
}

/** percent_add / flat_add 値群を合算する */
function combineSum(values: number[]): number {
  return values.reduce((acc, v) => acc + v, 0);
}

/**
 * ビルドの計算結果を返す
 * @param slots - 遺物スロット一覧
 * @param effectMap - id → RelicEffect マップ
 * @returns カテゴリ・属性ごとに合算した結果グループ
 */
export function calcBuildResult(
  slots: BuildRelicSlot[],
  effectMap: Map<number, RelicEffect>
): BuildResultGroup[] {
  const contributions = collectContributions(slots, effectMap);

  const groupMap = new Map<string, EffectContribution[]>();
  for (const c of contributions) {
    const key = groupKey(c);
    if (!groupMap.has(key)) groupMap.set(key, []);
    groupMap.get(key)!.push(c);
  }

  const results: BuildResultGroup[] = [];

  for (const [key, items] of groupMap) {
    const first = items[0];
    const values = items.map((i) => i.value);

    let combined: number;
    switch (first.calcType) {
      case "multiply":
        combined = combineMultiply(values);
        break;
      case "percent_add":
      case "flat_add":
      case "special":
        combined = combineSum(values);
        break;
      default:
        combined = combineSum(values);
    }

    results.push({
      key,
      nameJa: first.attribute
        ? `${first.nameJa.replace(/\+\d+〜\d+$/, "").trim()}`
        : first.nameJa,
      categoryId: first.categoryId,
      categoryNameJa: first.categoryNameJa,
      calcType: first.calcType,
      attribute: first.attribute,
      unit: first.unit,
      combined,
      entries: items.map((i) => ({
        effectId: i.effectId,
        nameJa: i.nameJa,
        value: i.value,
      })),
    });
  }

  // カテゴリ→属性→名前でソート
  results.sort((a, b) => {
    if (a.categoryId !== b.categoryId)
      return a.categoryId.localeCompare(b.categoryId);
    if (a.attribute !== b.attribute)
      return (a.attribute ?? "").localeCompare(b.attribute ?? "");
    return a.nameJa.localeCompare(b.nameJa);
  });

  return results;
}

/** 合算値を表示文字列に変換する */
export function formatCombined(group: BuildResultGroup): string {
  const { calcType, combined, unit } = group;

  if (calcType === "multiply") {
    return `×${combined.toFixed(4).replace(/\.?0+$/, "")}`;
  }
  if (unit === "percent") {
    return `+${combined.toFixed(1).replace(/\.0$/, "")}%`;
  }
  if (unit === "per_sec") {
    return `+${combined.toFixed(2).replace(/\.?0+$/, "")}/秒`;
  }
  return `+${combined}`;
}
