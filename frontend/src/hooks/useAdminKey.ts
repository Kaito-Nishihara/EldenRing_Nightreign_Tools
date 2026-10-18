"use client";

import { useCallback, useState, useSyncExternalStore } from "react";
import { adminListPlaces } from "@/lib/api";
import { ApiError } from "@/lib/apiError";
import { ADMIN_KEY_INVALID_MESSAGE, readAdminKey, saveAdminKey, subscribeAdminKey } from "@/lib/adminKey";

/** サーバーで描くときは sessionStorage を読めないので、まだ分からないことを undefined で表す */
const readOnServer = () => undefined;

/**
 * 置いてある管理用の鍵(マスタの管理 3.1)。
 * null は鍵がないこと、undefined はまだ読んでいないことを表す。読む前は何も出さず、鍵の入力の欄が一瞬出るのを防ぐ。
 */
export function useAdminKey(): string | null | undefined {
  return useSyncExternalStore(subscribeAdminKey, readAdminKey, readOnServer);
}

/** 管理用の API が 401 を返したか。鍵がないか合わない */
export function isUnauthorized(error: unknown): boolean {
  return error instanceof ApiError && error.status === 401;
}

/**
 * 鍵の入力(マスタの管理 3.1)。
 * 入れた鍵で開始地点の一覧を呼び、200 なら鍵を置く。401 なら「管理用の鍵が違います」を出し、鍵は置かない。
 */
export function useAdminKeyEntry() {
  const [checking, setChecking] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const submit = useCallback(async (key: string) => {
    setChecking(true);
    setError(null);
    try {
      await adminListPlaces("spawn-points", key);
      saveAdminKey(key);
    } catch (e) {
      // 429 と通信の失敗は、ApiError の文言をそのまま出す
      setError(isUnauthorized(e) ? ADMIN_KEY_INVALID_MESSAGE : e instanceof ApiError ? e.message : ADMIN_KEY_INVALID_MESSAGE);
    } finally {
      setChecking(false);
    }
  }, []);

  return { submit, checking, error };
}
