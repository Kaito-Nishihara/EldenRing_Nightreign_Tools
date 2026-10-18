import type { RelicEffect, RelicEffectRank } from "@/types";

// ビルドシミュレーターの枠と、効果の合計(ビルドシミュレーター 3.1、3.2)。
// 計算はブラウザだけで行う(決定の記録 0006)

/** 通常効果の列か、深層効果の列か */
export type BuildSlotKind = "normal" | "deep";

/** 1 つの効果の枠。効果を選んでいなければ effectId は null */
export interface BuildSlot {
  kind: BuildSlotKind;
  /** 列の中での位置。0 から 2 */
  index: number;
  effectId: number | null;
  rank: number;
}

/** 列ごとの枠の数(3.1) */
export const SLOTS_PER_KIND = 3;

const SLOT_NUMERALS = ["I", "II", "III"];

const KIND_LABEL: Record<BuildSlotKind, string> = { normal: "通常", deep: "深層" };

/** 枠の名前。例: 通常 I、深層 III */
export function slotLabel(slot: Pick<BuildSlot, "kind" | "index">): string {
  return `${KIND_LABEL[slot.kind]} ${SLOT_NUMERALS[slot.index]}`;
}

/** 6 枠をすべて空にした状態。「すべて外す」でもこれに戻す */
export function emptySlots(): BuildSlot[] {
  return (["normal", "deep"] as const).flatMap((kind) =>
    Array.from({ length: SLOTS_PER_KIND }, (_, index) => ({ kind, index, effectId: null, rank: 0 })),
  );
}

/** 効果にランクの選択があるか(3.1)。rankMax が 0 より大きいものだけ */
export function hasRanks(effect: RelicEffect): boolean {
  return effect.rankMax > 0;
}

/** 枠で選んだランクの値。ランクのない効果は、ランク 0 の 1 件を使う */
export function selectedRank(effect: RelicEffect, rank: number): RelicEffectRank | undefined {
  return effect.ranks.find((r) => r.rank === (hasRanks(effect) ? rank : effect.rankMin));
}

/** 合計の 1 行に合わさった、1 つの枠の効果 */
export interface BuildTotalEntry {
  slotLabel: string;
  effectId: number;
  nameJa: string;
  /** ランクのない効果は null */
  rank: number | null;
  value: number | null;
  unit: RelicEffectRank["unit"];
}

/** 合計の 1 行。special の行は合計を持たない */
export interface BuildTotalRow {
  key: string;
  nameJa: string;
  calcType: RelicEffect["calcType"];
  unit: RelicEffectRank["unit"];
  /** special は null */
  total: number | null;
  /** 1 より小さい掛け算と、負の足し算。ember の色で出す(3.2) */
  isNegative: boolean;
  /** special の行で出す発動の条件 */
  conditionJa: string | null;
  entries: BuildTotalEntry[];
}

/** 分類ごとの区切り */
export interface BuildTotalCategory {
  categoryId: string;
  nameJa: string;
  rows: BuildTotalRow[];
}

/**
 * まとめる鍵(3.2)。属性があれば計算の種類と属性、なければ計算の種類と効果の識別子。
 * special は合計しないので、属性があっても効果ごとに分ける
 */
function totalKey(effect: RelicEffect): string {
  if (effect.calcType !== "special" && effect.attribute) {
    return `${effect.calcType}:${effect.attribute}`;
  }
  return `${effect.calcType}:effect:${effect.id}`;
}

/** 属性でまとめた行の名前。ランクの幅と「(深層)」を除く。例: 物理攻撃力上昇+0〜4(深層) → 物理攻撃力上昇 */
function rowName(effect: RelicEffect): string {
  if (effect.calcType === "special" || !effect.attribute) return effect.nameJa;
  return effect.nameJa.replace(/[+＋]\d+〜\d+/, "").replace(/[（(]深層[）)]/, "").trim();
}

function combine(calcType: RelicEffect["calcType"], values: number[]): number | null {
  if (calcType === "special") return null;
  if (calcType === "multiply") return values.reduce((total, v) => total * v, 1);
  return values.reduce((total, v) => total + v, 0);
}

function isNegativeTotal(calcType: RelicEffect["calcType"], total: number | null): boolean {
  if (total === null) return false;
  return calcType === "multiply" ? total < 1 : total < 0;
}

/**
 * 選んだ効果を、計算の種類と属性の組でまとめて合計する(3.2)。
 * 分類は分類の並び順に、分類の中の行は、合わさった効果の識別子の小さい順に並べる
 */
export function calcBuildTotals(slots: BuildSlot[], effects: ReadonlyMap<number, RelicEffect>): BuildTotalCategory[] {
  const groups = new Map<string, { effect: RelicEffect; entries: BuildTotalEntry[] }>();

  for (const slot of slots) {
    if (slot.effectId === null) continue;
    const effect = effects.get(slot.effectId);
    const rank = effect && selectedRank(effect, slot.rank);
    if (!effect || !rank) continue;

    const key = totalKey(effect);
    const group = groups.get(key) ?? { effect, entries: [] };
    group.entries.push({
      slotLabel: slotLabel(slot),
      effectId: effect.id,
      nameJa: effect.nameJa,
      rank: hasRanks(effect) ? rank.rank : null,
      value: rank.value,
      unit: rank.unit,
    });
    groups.set(key, group);
  }

  const categories = new Map<string, BuildTotalCategory & { sortOrder: number }>();
  const ordered = [...groups.entries()].sort(([, a], [, b]) => minEffectId(a.entries) - minEffectId(b.entries));
  for (const [key, { effect, entries }] of ordered) {
    const total = combine(effect.calcType, entries.flatMap((e) => (e.value === null ? [] : [e.value])));
    const category = categories.get(effect.categoryId) ?? {
      categoryId: effect.categoryId,
      nameJa: effect.categoryNameJa,
      sortOrder: effect.categorySortOrder,
      rows: [],
    };
    category.rows.push({
      key,
      nameJa: rowName(effect),
      calcType: effect.calcType,
      unit: entries[0].unit,
      total,
      isNegative: isNegativeTotal(effect.calcType, total),
      conditionJa: effect.conditionJa,
      entries,
    });
    categories.set(effect.categoryId, category);
  }

  return [...categories.values()]
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map(({ categoryId, nameJa, rows }) => ({ categoryId, nameJa, rows }));
}

function minEffectId(entries: BuildTotalEntry[]): number {
  return Math.min(...entries.map((e) => e.effectId));
}

/** 足し算の符号。負の値は「-」、それ以外は「+」 */
function signed(value: number, digits: string): string {
  return `${value < 0 ? "-" : "+"}${digits}`;
}

/** 小数 1 桁にし、0 で終わる小数を省く。例: 10.0 → 10、10.5 → 10.5 */
function oneDecimal(value: number): string {
  return Math.abs(value).toFixed(1).replace(/\.0$/, "");
}

/**
 * 合計の表示(3.2)。
 * multiply は × と小数 2 桁、percent_add は + と小数 1 桁と %、flat_add は + と整数(per_sec なら /秒)。special は空文字
 */
export function formatTotal(row: Pick<BuildTotalRow, "calcType" | "unit" | "total">): string {
  const { calcType, unit, total } = row;
  if (total === null || calcType === "special") return "";
  if (calcType === "multiply") return `×${total.toFixed(2)}`;
  if (calcType === "percent_add") return `${signed(total, oneDecimal(total))}%`;
  return `${signed(total, String(Math.round(Math.abs(total))))}${unit === "per_sec" ? "/秒" : ""}`;
}

/** 1 つのランクの値の表示。枠の説明と合計の内訳に使う。例: ×1.05、+4%、-80、+2/秒。数がなければ空文字 */
export function formatRankValue(value: number | null, unit: RelicEffectRank["unit"]): string {
  if (value === null) return "";
  switch (unit) {
    case "multiplier":
      return `×${value}`;
    case "percent":
      return `${signed(value, oneDecimal(value))}%`;
    case "per_sec":
      return `${signed(value, String(Math.abs(value)))}/秒`;
    case "flat":
      return signed(value, String(Math.abs(value)));
    default:
      return "";
  }
}

/** 合計の行の内訳(3.2)。例: 通常 I の ×1.05、深層 I のランク 3 */
export function describeEntries(entries: BuildTotalEntry[]): string {
  return entries
    .map((e) => {
      if (e.rank !== null) return `${e.slotLabel} のランク ${e.rank}`;
      const value = formatRankValue(e.value, e.unit);
      return value ? `${e.slotLabel} の ${value}` : e.slotLabel;
    })
    .join("、");
}
