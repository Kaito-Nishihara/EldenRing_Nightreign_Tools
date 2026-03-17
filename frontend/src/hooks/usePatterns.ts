"use client";

import { useState, useCallback } from "react";
import type { PatternResponse } from "@/types";
import { getPatterns } from "@/lib/api";

/** パターン検索フックの戻り値 */
interface UsePatternsReturn {
  /** 検索結果 */
  data: PatternResponse | null;
  /** ローディング状態 */
  loading: boolean;
  /** エラー情報 */
  error: string | null;
  /** パターンを検索する */
  search: (
    bossId: string,
    spawnPointId: number,
    jihenId?: string
  ) => Promise<void>;
  /** 検索結果をクリアする */
  clear: () => void;
}

/**
 * パターン検索カスタムフック
 * APIからパターンデータを取得し、状態管理を行う
 */
export function usePatterns(): UsePatternsReturn {
  const [data, setData] = useState<PatternResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /** パターンを検索する */
  const search = useCallback(
    async (bossId: string, spawnPointId: number, jihenId?: string) => {
      setLoading(true);
      setError(null);

      try {
        const response = await getPatterns(bossId, spawnPointId, jihenId);
        setData(response);
      } catch (err) {
        const message =
          err instanceof Error
            ? err.message
            : "パターンの取得に失敗しました";
        setError(message);
        setData(null);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  /** 検索結果をクリアする */
  const clear = useCallback(() => {
    setData(null);
    setError(null);
  }, []);

  return { data, loading, error, search, clear };
}
