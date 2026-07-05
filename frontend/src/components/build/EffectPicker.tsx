"use client";

import type { RelicEffect, BuildEffectEntry } from "@/types";

interface EffectPickerProps {
  entry: BuildEffectEntry;
  effectsByCategory: Map<string, { nameJa: string; effects: RelicEffect[] }>;
  effectMap: Map<number, RelicEffect>;
  onEffectChange: (effectId: number | null) => void;
  onRankChange: (rank: number) => void;
}

/**
 * 単一の効果 + ランク選択コンポーネント
 */
export default function EffectPicker({
  entry,
  effectsByCategory,
  effectMap,
  onEffectChange,
  onRankChange,
}: EffectPickerProps) {
  const selectedEffect =
    entry.effectId !== null ? effectMap.get(entry.effectId) : null;

  const hasRank =
    selectedEffect !== undefined &&
    selectedEffect !== null &&
    selectedEffect.rankMin < selectedEffect.rankMax;

  const rankOptions = selectedEffect
    ? Array.from(
        { length: selectedEffect.rankMax - selectedEffect.rankMin + 1 },
        (_, i) => selectedEffect.rankMin + i
      )
    : [];

  return (
    <div className="flex items-center gap-2">
      {/* 効果セレクト */}
      <select
        className="flex-1 bg-gray-800 border border-gray-600 rounded px-2 py-1.5 text-sm text-gray-100 focus:outline-none focus:border-amber-500"
        value={entry.effectId ?? ""}
        onChange={(e) =>
          onEffectChange(e.target.value ? Number(e.target.value) : null)
        }
      >
        <option value="">-- 効果を選択 --</option>
        {[...effectsByCategory.entries()].map(([catId, { nameJa, effects }]) => (
          <optgroup key={catId} label={nameJa}>
            {effects.map((ef) => (
              <option key={ef.id} value={ef.id}>
                {ef.nameJa}
                {ef.stackable === "no" ? " ※重複不可" : ""}
                {ef.stackable === "conditional" ? " ※条件付き" : ""}
              </option>
            ))}
          </optgroup>
        ))}
      </select>

      {/* ランクセレクト（ランクあり効果のみ） */}
      {hasRank && (
        <select
          className="w-28 bg-gray-800 border border-gray-600 rounded px-2 py-1.5 text-sm text-gray-100 focus:outline-none focus:border-amber-500"
          value={entry.rank}
          onChange={(e) => onRankChange(Number(e.target.value))}
        >
          {rankOptions.map((r) => {
            const rv = selectedEffect!.ranks.find((rk) => rk.rank === r);
            const label =
              rv && rv.unit !== "none"
                ? rv.unit === "multiplier"
                  ? `Rank${r}: ×${rv.value}`
                  : rv.unit === "percent"
                  ? `Rank${r}: +${rv.value}%`
                  : `Rank${r}: +${rv.value}`
                : `Rank${r}`;
            return (
              <option key={r} value={r}>
                {label}
              </option>
            );
          })}
        </select>
      )}

      {/* 発動条件ありインジケーター */}
      {selectedEffect?.conditionJa && (
        <span
          className="text-xs text-yellow-400 shrink-0"
          title={selectedEffect.conditionJa}
        >
          ⚡
        </span>
      )}
    </div>
  );
}
