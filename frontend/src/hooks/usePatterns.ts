"use client";

import { useState, useCallback, useRef } from "react";
import type { PatternResponse } from "@/types";
import { getPatterns } from "@/lib/api";

/** パターン検索フックの戻り値 */
interface UsePatternsReturn {
  /** 検索結果 */
  data: PatternResponse | null;
  /** ローディング状態 */
  loading: boolean;
  /** エラー情報(画面に出してよい文言) */
  error: string | null;
  /** パターンを検索する */
  search: (bossId: string, spawnPointId: number, jihenId?: string) => Promise<void>;
  /** 検索結果をクリアする */
  clear: () => void;
}

/**
 * パターン検索(パターン検索 3.1)。
 * 条件を素早く変えたとき、前の検索の応答が後から届いても画面に出さない(AC-PAT-14)。
 * 検索のたびに番号を振り、最後に始めた検索の応答だけを受け取る。
 */
export function usePatterns(): UsePatternsReturn {
  const [data, setData] = useState<PatternResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const latestRequest = useRef(0);

  const search = useCallback(async (bossId: string, spawnPointId: number, jihenId?: string) => {
    const request = ++latestRequest.current;
    setLoading(true);
    setError(null);

    try {
      const response = await getPatterns(bossId, spawnPointId, jihenId);
      if (request !== latestRequest.current) return;
      setData(response);
    } catch (err) {
      if (request !== latestRequest.current) return;
      setError(err instanceof Error ? err.message : "パターンの取得に失敗しました");
      setData(null);
    } finally {
      if (request === latestRequest.current) {
        setLoading(false);
      }
    }
  }, []);

  /** 検索結果をクリアする。走っている検索の応答も捨てる */
  const clear = useCallback(() => {
    latestRequest.current++;
    setData(null);
    setError(null);
    setLoading(false);
  }, []);

  return { data, loading, error, search, clear };
}
