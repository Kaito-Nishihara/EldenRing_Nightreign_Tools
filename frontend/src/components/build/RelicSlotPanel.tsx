"use client";

import type { RelicEffect, BuildRelicSlot } from "@/types";
import EffectPicker from "./EffectPicker";

interface RelicSlotPanelProps {
  slot: BuildRelicSlot;
  effectsByCategory: Map<string, { nameJa: string; effects: RelicEffect[] }>;
  effectMap: Map<number, RelicEffect>;
  onEffectChange: (entryId: string, effectId: number | null) => void;
  onRankChange: (entryId: string, rank: number) => void;
}

/**
 * 遺物効果セクションパネル（通常3枠 / 深層3枠）
 */
export default function RelicSlotPanel({
  slot,
  effectsByCategory,
  effectMap,
  onEffectChange,
  onRankChange,
}: RelicSlotPanelProps) {
  const label = slot.isDeep ? "深層効果" : "通常効果";
  const accentColor = slot.isDeep
    ? "text-purple-400 border-purple-800"
    : "text-amber-400 border-amber-800";

  return (
    <div className={`bg-gray-900 border rounded-lg p-4 space-y-3 ${slot.isDeep ? "border-purple-900/60" : "border-gray-700"}`}>
      {/* セクションヘッダー */}
      <div className={`flex items-center gap-2 pb-2 border-b ${accentColor}`}>
        <span className="font-semibold text-sm">{label}</span>
        <span className="text-gray-500 text-xs">（3枠固定）</span>
      </div>

      {/* 効果ピッカー3枠 */}
      <div className="space-y-2">
        {slot.effects.map((entry, idx) => (
          <div key={entry.id} className="flex items-center gap-2">
            <span className="text-gray-600 text-xs w-4 shrink-0">{idx + 1}</span>
            <div className="flex-1">
              <EffectPicker
                entry={entry}
                effectsByCategory={effectsByCategory}
                effectMap={effectMap}
                onEffectChange={(effectId) => onEffectChange(entry.id, effectId)}
                onRankChange={(rank) => onRankChange(entry.id, rank)}
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
