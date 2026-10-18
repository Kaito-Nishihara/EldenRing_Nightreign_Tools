"use client";

import { useEffect, useMemo, useState } from "react";
import { getRelicEffects } from "@/lib/api";
import type { RelicEffect } from "@/types";

/** 選択の欄の 1 つの分類と、その中の効果 */
export interface RelicEffectOptionGroup {
  categoryId: string;
  nameJa: string;
  effects: RelicEffect[];
}

interface RelicEffectsState {
  effects: RelicEffect[];
  loading: boolean;
  /** 取得に失敗したときの文言(画面の共通仕様 3.10)。成功していれば null */
  error: string | null;
}

/** 効果を分類ごとにまとめ、分類の並び順、効果の識別子の順に並べる(ビルドシミュレーター 3.1) */
function groupByCategory(effects: RelicEffect[]): RelicEffectOptionGroup[] {
  const groups = new Map<string, RelicEffectOptionGroup & { sortOrder: number }>();
  for (const effect of [...effects].sort((a, b) => a.id - b.id)) {
    const group = groups.get(effect.categoryId) ?? {
      categoryId: effect.categoryId,
      nameJa: effect.categoryNameJa,
      sortOrder: effect.categorySortOrder,
      effects: [],
    };
    group.effects.push(effect);
    groups.set(effect.categoryId, group);
  }
  return [...groups.values()]
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map(({ categoryId, nameJa, effects: grouped }) => ({ categoryId, nameJa, effects: grouped }));
}

/**
 * 遺物の効果のマスタを、画面を開いたときに 1 回だけ取る(ビルドシミュレーター 6 章)。
 * 効果を選び直しても取り直さない(AC-BLD-10)。retry を呼んだときだけ取り直す。
 */
export function useRelicEffects() {
  const [state, setState] = useState<RelicEffectsState>({ effects: [], loading: true, error: null });
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    let cancelled = false;
    setState((previous) => ({ ...previous, loading: true, error: null }));

    getRelicEffects()
      .then((effects) => {
        if (!cancelled) setState({ effects, loading: false, error: null });
      })
      .catch((error: unknown) => {
        if (!cancelled) {
          const message = error instanceof Error ? error.message : "遺物の効果を取得できませんでした";
          setState((previous) => ({ ...previous, loading: false, error: message }));
        }
      });

    return () => {
      cancelled = true;
    };
  }, [attempt]);

  const effectMap = useMemo(() => new Map(state.effects.map((e) => [e.id, e])), [state.effects]);

  // 通常効果の枠には深層でない効果、深層効果の枠には深層効果だけを出す(3.1、AC-BLD-08)
  const normalOptions = useMemo(() => groupByCategory(state.effects.filter((e) => !e.isDeep)), [state.effects]);
  const deepOptions = useMemo(() => groupByCategory(state.effects.filter((e) => e.isDeep)), [state.effects]);

  return { ...state, effectMap, normalOptions, deepOptions, retry: () => setAttempt((n) => n + 1) };
}
