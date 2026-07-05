"use client";

import { useState, useEffect, useMemo, useCallback } from "react";
import { getRelicEffects } from "@/lib/api";
import { calcBuildResult } from "@/lib/buildCalc";
import type {
  RelicEffect,
  BuildRelicSlot,
  BuildEffectEntry,
  BuildResultGroup,
} from "@/types";

/** 通常効果・深層効果それぞれ固定3枠 */
const EFFECTS_PER_SECTION = 3;

/** 空の効果エントリを生成する */
function emptyEntry(): BuildEffectEntry {
  return { id: crypto.randomUUID(), effectId: null, rank: 0 };
}

/** 固定3枠の初期スロットを生成する */
function initSlots(): BuildRelicSlot[] {
  return [
    {
      id: 1,
      isDeep: false,
      effects: Array.from({ length: EFFECTS_PER_SECTION }, emptyEntry),
    },
    {
      id: 2,
      isDeep: true,
      effects: Array.from({ length: EFFECTS_PER_SECTION }, emptyEntry),
    },
  ];
}

export function useBuildSimulator() {
  const [effects, setEffects] = useState<RelicEffect[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [slots, setSlots] = useState<BuildRelicSlot[]>(initSlots);

  useEffect(() => {
    getRelicEffects()
      .then(setEffects)
      .catch(() => setError("遺物効果データの取得に失敗しました"))
      .finally(() => setLoading(false));
  }, []);

  /** id → RelicEffect マップ */
  const effectMap = useMemo(
    () => new Map(effects.map((e) => [e.id, e])),
    [effects]
  );

  /** 通常効果をカテゴリ別にグループ化 */
  const normalEffectsByCategory = useMemo(() => {
    const map = new Map<string, { nameJa: string; effects: RelicEffect[] }>();
    for (const e of effects.filter((e) => !e.isDeep)) {
      if (!map.has(e.categoryId)) {
        map.set(e.categoryId, { nameJa: e.categoryNameJa, effects: [] });
      }
      map.get(e.categoryId)!.effects.push(e);
    }
    return map;
  }, [effects]);

  /** 深層効果をカテゴリ別にグループ化 */
  const deepEffectsByCategory = useMemo(() => {
    const map = new Map<string, { nameJa: string; effects: RelicEffect[] }>();
    for (const e of effects.filter((e) => e.isDeep)) {
      if (!map.has(e.categoryId)) {
        map.set(e.categoryId, { nameJa: e.categoryNameJa, effects: [] });
      }
      map.get(e.categoryId)!.effects.push(e);
    }
    return map;
  }, [effects]);

  /** 計算結果 */
  const resultGroups: BuildResultGroup[] = useMemo(
    () => calcBuildResult(slots, effectMap),
    [slots, effectMap]
  );

  /** 効果エントリを変更する */
  const updateEffect = useCallback(
    (slotId: number, entryId: string, effectId: number | null) => {
      const effect = effectId !== null ? effectMap.get(effectId) : null;
      const defaultRank = effect ? effect.rankMin : 0;
      setSlots((prev) =>
        prev.map((slot) =>
          slot.id !== slotId
            ? slot
            : {
                ...slot,
                effects: slot.effects.map((ent) =>
                  ent.id !== entryId
                    ? ent
                    : { ...ent, effectId, rank: defaultRank }
                ),
              }
        )
      );
    },
    [effectMap]
  );

  /** ランクを変更する */
  const updateRank = useCallback(
    (slotId: number, entryId: string, rank: number) => {
      setSlots((prev) =>
        prev.map((slot) =>
          slot.id !== slotId
            ? slot
            : {
                ...slot,
                effects: slot.effects.map((ent) =>
                  ent.id !== entryId ? ent : { ...ent, rank }
                ),
              }
        )
      );
    },
    []
  );

  /** 全スロットをリセットする */
  const resetAll = useCallback(() => setSlots(initSlots()), []);

  return {
    loading,
    error,
    slots,
    effectMap,
    normalEffectsByCategory,
    deepEffectsByCategory,
    resultGroups,
    updateEffect,
    updateRank,
    resetAll,
    EFFECTS_PER_SECTION,
  };
}
