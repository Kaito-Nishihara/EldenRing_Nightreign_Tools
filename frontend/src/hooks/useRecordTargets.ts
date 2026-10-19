"use client";

import { useEffect, useState } from "react";
import { getNightBosses, getSecondShrinkPoints, getShrinkZones } from "@/lib/api";
import type { NightBoss, SecondShrinkPoint, ShrinkZone } from "@/types";

interface RecordTargets {
  /** 第 1 次のスロットに入れる収縮候補地 */
  shrinkZones: ShrinkZone[];
  /** 第 2 次のスロットに入れる第二収縮地点 */
  secondShrinkPoints: SecondShrinkPoint[];
  /** 夜ボスの選択の欄に並べる夜ボス */
  nightBosses: NightBoss[];
  loading: boolean;
  /** 取得に失敗したときの文言(画面の共通仕様 3.10)。成功していれば null */
  error: string | null;
}

const EMPTY: RecordTargets = { shrinkZones: [], secondShrinkPoints: [], nightBosses: [], loading: true, error: null };

/**
 * プレイ記録の画面で収縮先と夜ボスに使うマスタを取る(プレイ記録 4 章の「画面が使う取得の API」)。
 * retry を呼ぶと取り直す。
 */
export function useRecordTargets() {
  const [state, setState] = useState<RecordTargets>(EMPTY);
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    let cancelled = false;
    setState((previous) => ({ ...previous, loading: true, error: null }));

    Promise.all([getShrinkZones(), getSecondShrinkPoints(), getNightBosses()])
      .then(([shrinkZones, secondShrinkPoints, nightBosses]) => {
        if (!cancelled) {
          setState({ shrinkZones, secondShrinkPoints, nightBosses, loading: false, error: null });
        }
      })
      .catch((error: unknown) => {
        if (!cancelled) {
          const message = error instanceof Error ? error.message : "マスタを取得できませんでした";
          setState((previous) => ({ ...previous, loading: false, error: message }));
        }
      });

    return () => {
      cancelled = true;
    };
  }, [attempt]);

  return { ...state, retry: () => setAttempt((n) => n + 1) };
}
