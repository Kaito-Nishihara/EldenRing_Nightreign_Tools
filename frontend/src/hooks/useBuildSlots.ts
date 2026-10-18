"use client";

import { useCallback, useMemo, useState } from "react";
import { calcBuildTotals, emptySlots, type BuildSlot } from "@/lib/buildCalc";
import type { RelicEffect } from "@/types";

/** 枠を指す組。列と、列の中の位置 */
type SlotRef = Pick<BuildSlot, "kind" | "index">;

function isSlot(slot: BuildSlot, ref: SlotRef): boolean {
  return slot.kind === ref.kind && slot.index === ref.index;
}

/**
 * ビルドシミュレーターの 6 つの枠の状態と、効果の合計(ビルドシミュレーター 3.1、3.2)。
 * 合計は枠が変わるたびにブラウザで計算し直し、API を呼ばない(決定の記録 0006)。
 */
export function useBuildSlots(effectMap: ReadonlyMap<number, RelicEffect>) {
  const [slots, setSlots] = useState<BuildSlot[]>(emptySlots);

  /** 枠の効果を選ぶ。ランクは効果の初めのランク(rankMin)にする。null は枠を空ける */
  const chooseEffect = useCallback(
    (ref: SlotRef, effectId: number | null) => {
      const rank = effectId === null ? 0 : (effectMap.get(effectId)?.rankMin ?? 0);
      setSlots((previous) => previous.map((slot) => (isSlot(slot, ref) ? { ...slot, effectId, rank } : slot)));
    },
    [effectMap],
  );

  const chooseRank = useCallback((ref: SlotRef, rank: number) => {
    setSlots((previous) => previous.map((slot) => (isSlot(slot, ref) ? { ...slot, rank } : slot)));
  }, []);

  /** 「すべて外す」。保存していないので確かめの枠は出さない(3.1) */
  const clearAll = useCallback(() => setSlots(emptySlots()), []);

  const totals = useMemo(() => calcBuildTotals(slots, effectMap), [slots, effectMap]);

  return { slots, totals, chooseEffect, chooseRank, clearAll };
}
