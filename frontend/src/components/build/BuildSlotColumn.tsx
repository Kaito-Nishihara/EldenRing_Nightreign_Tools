"use client";

import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import BuildEffectSlot from "@/components/build/BuildEffectSlot";
import type { BuildSlot } from "@/lib/buildCalc";
import type { RelicEffectOptionGroup } from "@/hooks/useRelicEffects";
import type { RelicEffect } from "@/types";

const HEADINGS: Record<BuildSlot["kind"], { eyebrow: string; title: string }> = {
  normal: { eyebrow: "Relic Effects", title: "通常効果" },
  deep: { eyebrow: "Depths of Night", title: "深層効果" },
};

interface BuildSlotColumnProps {
  kind: BuildSlot["kind"];
  /** この列の 3 つの枠 */
  slots: BuildSlot[];
  options: RelicEffectOptionGroup[];
  effectMap: ReadonlyMap<number, RelicEffect>;
  onEffectChange: (slot: BuildSlot, effectId: number | null) => void;
  onRankChange: (slot: BuildSlot, rank: number) => void;
}

/** 通常効果か深層効果の列。3 つの枠を縦に並べる(ビルドシミュレーター 3.1) */
export default function BuildSlotColumn({ kind, slots, options, effectMap, onEffectChange, onRankChange }: BuildSlotColumnProps) {
  const headingId = `build-${kind}-title`;
  return (
    <Panel ornate aria-labelledby={headingId}>
      <SectionHeading id={headingId} {...HEADINGS[kind]} />
      <div className="flex flex-col gap-3">
        {slots.map((slot) => (
          <BuildEffectSlot
            key={slot.index}
            slot={slot}
            options={options}
            effect={slot.effectId === null ? undefined : effectMap.get(slot.effectId)}
            onEffectChange={(effectId) => onEffectChange(slot, effectId)}
            onRankChange={(rank) => onRankChange(slot, rank)}
          />
        ))}
      </div>
    </Panel>
  );
}
