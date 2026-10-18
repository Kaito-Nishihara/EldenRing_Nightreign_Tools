"use client";

import SegmentedControl from "@/components/ui/SegmentedControl";
import { formatRankValue, hasRanks, selectedRank, slotLabel, type BuildSlot } from "@/lib/buildCalc";
import type { RelicEffectOptionGroup } from "@/hooks/useRelicEffects";
import type { RelicEffect } from "@/types";

const NUMERALS = ["I", "II", "III"];

const KIND_TITLE: Record<BuildSlot["kind"], string> = { normal: "通常効果", deep: "深層効果" };

const CALC_LABEL: Record<RelicEffect["calcType"], string> = {
  multiply: "掛け算",
  percent_add: "割合の足し算",
  flat_add: "足し算",
  special: "特殊",
};

export const DEMERIT_MESSAGE = "この段からデメリットが付く";

interface BuildEffectSlotProps {
  slot: BuildSlot;
  /** 選択の欄に出す効果。通常効果の枠なら深層でない効果だけ */
  options: RelicEffectOptionGroup[];
  /** 選んでいる効果。選んでいなければ undefined */
  effect: RelicEffect | undefined;
  onEffectChange: (effectId: number | null) => void;
  onRankChange: (rank: number) => void;
}

/** 枠の下の説明。例: 攻撃力 ／ 掛け算 ×1.05 */
function description(effect: RelicEffect, rank: number): string {
  if (effect.calcType === "special") {
    return [effect.categoryNameJa, CALC_LABEL.special, effect.conditionJa].filter(Boolean).join(" ／ ");
  }
  const value = selectedRank(effect, rank);
  return `${effect.categoryNameJa} ／ ${CALC_LABEL[effect.calcType]} ${value ? formatRankValue(value.value, value.unit) : ""}`.trim();
}

/**
 * 効果の枠 1 つ(ビルドシミュレーター 3.1)。
 * 効果の選択の欄と、ランクのある効果ではランクのボタン、深層効果ではデメリットの知らせを出す。
 */
export default function BuildEffectSlot({ slot, options, effect, onEffectChange, onRankChange }: BuildEffectSlotProps) {
  const numeral = NUMERALS[slot.index];
  const rank = effect ? selectedRank(effect, slot.rank) : undefined;
  const showDemerit = slot.kind === "deep" && rank?.hasDemerit === true;
  const isDemeritEffect = effect?.effectSign === "negative";

  return (
    <div
      className={`flex flex-col gap-2 border p-3 ${
        slot.kind === "deep" ? "border-tide-soft bg-linear-to-b from-tide-soft/40 to-transparent" : "border-line bg-night"
      }`}
    >
      <span aria-hidden className="font-display text-xs tracking-display text-ash-soft">
        {numeral}
      </span>
      <select
        aria-label={`${KIND_TITLE[slot.kind]} ${numeral}`}
        value={slot.effectId ?? ""}
        onChange={(e) => onEffectChange(e.target.value ? Number(e.target.value) : null)}
        className="min-h-11 w-full border border-line-strong bg-night px-3 text-base text-ash transition-colors duration-200 ease-out hover:border-azure"
      >
        <option value="">(空き)</option>
        {options.map((group) => (
          <optgroup key={group.categoryId} label={group.nameJa}>
            {group.effects.map((option) => (
              <option key={option.id} value={option.id}>
                {option.nameJa}
              </option>
            ))}
          </optgroup>
        ))}
      </select>

      {effect && hasRanks(effect) && (
        <div className="flex flex-wrap items-center gap-2">
          <span className="text-xs text-ash-soft">ランク</span>
          <SegmentedControl
            ariaLabel={`${slotLabel(slot)} のランク`}
            options={effect.ranks.map((r) => ({ value: String(r.rank), label: String(r.rank), display: true }))}
            value={String(slot.rank)}
            onChange={(value) => onRankChange(Number(value))}
          />
          {showDemerit && <span className="ml-auto text-xs text-ember">◆ {DEMERIT_MESSAGE}</span>}
        </div>
      )}

      <span className={`text-xs ${isDemeritEffect ? "text-ember" : "text-ash-soft"}`}>
        {effect ? description(effect, slot.rank) : "効果を選んでください"}
      </span>
    </div>
  );
}
