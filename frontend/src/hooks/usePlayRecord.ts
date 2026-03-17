"use client";

import { useState, useCallback } from "react";
import type { PlayRecordRequest } from "@/types";
import { createPlayRecord } from "@/lib/api";

/** プレイ記録作成結果 */
interface PlayRecordResult {
  id: number;
  createdAt: string;
}

/** プレイ記録管理フックの戻り値 */
interface UsePlayRecordReturn {
  /** 作成結果 */
  result: PlayRecordResult | null;
  /** 送信中の状態 */
  submitting: boolean;
  /** エラー情報 */
  error: string | null;
  /** プレイ記録を送信する */
  submit: (request: PlayRecordRequest) => Promise<boolean>;
  /** 状態をリセットする */
  reset: () => void;
}

/**
 * プレイ記録管理カスタムフック
 * プレイ記録の作成と状態管理を行う
 */
export function usePlayRecord(): UsePlayRecordReturn {
  const [result, setResult] = useState<PlayRecordResult | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /** プレイ記録を送信する */
  const submit = useCallback(
    async (request: PlayRecordRequest): Promise<boolean> => {
      setSubmitting(true);
      setError(null);

      try {
        const response = await createPlayRecord(request);
        setResult(response);
        return true;
      } catch (err) {
        const message =
          err instanceof Error
            ? err.message
            : "プレイ記録の保存に失敗しました";
        setError(message);
        return false;
      } finally {
        setSubmitting(false);
      }
    },
    []
  );

  /** 状態をリセットする */
  const reset = useCallback(() => {
    setResult(null);
    setError(null);
  }, []);

  return { result, submitting, error, submit, reset };
}
