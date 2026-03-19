"use client";

import { useState, useEffect } from "react";
import type { Boss, Jihen, SpawnPoint } from "@/types";
import { getBosses, getJihen, getSpawnPoints } from "@/lib/api";

/** マスタデータフックの戻り値 */
interface UseMasterDataReturn {
  /** ボス一覧 */
  bosses: Boss[];
  /** 地変一覧 */
  jihenList: Jihen[];
  /** 開始地点一覧 */
  spawnPoints: SpawnPoint[];
  /** ローディング状態 */
  loading: boolean;
  /** エラー情報 */
  error: string | null;
}

/**
 * マスタデータ取得カスタムフック
 * 初回マウント時にボス・地変・開始地点を一括取得する
 */
export function useMasterData(): UseMasterDataReturn {
  const [bosses, setBosses] = useState<Boss[]>([]);
  const [jihenList, setJihenList] = useState<Jihen[]>([]);
  const [spawnPoints, setSpawnPoints] = useState<SpawnPoint[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    async function fetchAll() {
      try {
        const [bossData, jihenData, spawnData] = await Promise.all([
          getBosses(),
          getJihen(),
          getSpawnPoints(),
        ]);
        if (!cancelled) {
          setBosses(bossData);
          setJihenList(jihenData);
          setSpawnPoints(spawnData);
        }
      } catch (err) {
        if (!cancelled) {
          const message =
            err instanceof Error
              ? err.message
              : "マスタデータの取得に失敗しました";
          setError(message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    fetchAll();
    return () => {
      cancelled = true;
    };
  }, []);

  return { bosses, jihenList, spawnPoints, loading, error };
}
