"use client";

import { useCallback } from "react";
import type { Boss, Jihen, SpawnPoint } from "@/types";

interface ConditionPanelProps {
  /** ボス一覧 */
  bosses: Boss[];
  /** 地変一覧 */
  jihenList: Jihen[];
  /** 開始地点一覧 */
  spawnPoints: SpawnPoint[];
  /** 選択中のボスID */
  selectedBossId: string | null;
  /** 選択中の地変ID */
  selectedJihenId: string | null;
  /** 選択中の開始地点ID */
  selectedSpawnPointId: number | null;
  /** ボス選択ハンドラー */
  onBossSelect: (bossId: string | null) => void;
  /** 地変選択ハンドラー */
  onJihenSelect: (jihenId: string | null) => void;
  /** 開始地点選択ハンドラー */
  onSpawnPointSelect: (spawnPointId: number | null) => void;
  /** ローディング状態 */
  loading?: boolean;
  /** エラー情報 */
  error?: string | null;
}

/**
 * 条件選択パネル
 * ボス・地変・開始地点を選択してパターン検索の条件を指定する
 */
export default function ConditionPanel({
  bosses,
  jihenList,
  spawnPoints,
  selectedBossId,
  selectedJihenId,
  selectedSpawnPointId,
  onBossSelect,
  onJihenSelect,
  onSpawnPointSelect,
  loading = false,
  error = null,
}: ConditionPanelProps) {
  /** ボス選択変更 */
  const handleBossChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      const value = e.target.value;
      onBossSelect(value || null);
    },
    [onBossSelect]
  );

  /** 地変選択変更 */
  const handleJihenChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      const value = e.target.value;
      onJihenSelect(value || null);
    },
    [onJihenSelect]
  );

  /** 開始地点選択変更 */
  const handleSpawnPointChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      const value = e.target.value;
      onSpawnPointSelect(value ? Number(value) : null);
    },
    [onSpawnPointSelect]
  );

  if (error) {
    return (
      <div className="p-4 border border-red-700 rounded-lg bg-red-900/20">
        <h2 className="text-lg font-bold text-red-300 mb-2">エラー</h2>
        <p className="text-sm text-red-400">{error}</p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h2 className="text-lg font-bold text-gray-200 mb-2">条件選択</h2>
        <div className="flex items-center gap-2 text-gray-400">
          <div className="w-4 h-4 border-2 border-gray-400 border-t-transparent rounded-full animate-spin" />
          <span className="text-sm">読み込み中...</span>
        </div>
      </div>
    );
  }

  /** 選択中のボス情報 */
  const selectedBoss = bosses.find((b) => b.id === selectedBossId);

  return (
    <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50 space-y-4">
      <h2 className="text-lg font-bold text-gray-200">条件選択</h2>

      {/* ボス選択 */}
      <div>
        <label
          htmlFor="boss-select"
          className="block text-sm font-medium text-gray-300 mb-1"
        >
          ボス
        </label>
        <select
          id="boss-select"
          value={selectedBossId ?? ""}
          onChange={handleBossChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">-- ボスを選択 --</option>
          {bosses.map((boss) => (
            <option key={boss.id} value={boss.id}>
              {boss.nameJa}
            </option>
          ))}
        </select>
        {/* 弱点表示 */}
        {selectedBoss && selectedBoss.weaknesses.length > 0 && (
          <div className="mt-1 flex gap-1 flex-wrap">
            {selectedBoss.weaknesses.map((w) => (
              <span
                key={w}
                className="text-xs px-2 py-0.5 rounded-full bg-yellow-900/50 text-yellow-300 border border-yellow-700"
              >
                {w}
              </span>
            ))}
          </div>
        )}
      </div>

      {/* 地変選択 */}
      <div>
        <label
          htmlFor="jihen-select"
          className="block text-sm font-medium text-gray-300 mb-1"
        >
          地変
        </label>
        <select
          id="jihen-select"
          value={selectedJihenId ?? ""}
          onChange={handleJihenChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">なし</option>
          {jihenList.map((jihen) => (
            <option key={jihen.id} value={jihen.id}>
              {jihen.nameJa}
            </option>
          ))}
        </select>
      </div>

      {/* 開始地点選択 */}
      <div>
        <label
          htmlFor="spawn-select"
          className="block text-sm font-medium text-gray-300 mb-1"
        >
          開始地点
        </label>
        <select
          id="spawn-select"
          value={selectedSpawnPointId ?? ""}
          onChange={handleSpawnPointChange}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">-- 開始地点を選択 --</option>
          {spawnPoints.map((sp) => (
            <option key={sp.id} value={sp.id}>
              {sp.nameJa}
            </option>
          ))}
        </select>
      </div>

      {/* 検索条件ステータス */}
      <div className="text-xs text-gray-500 pt-2 border-t border-gray-700">
        {selectedBossId && selectedSpawnPointId
          ? "条件が揃いました。パターンを検索中..."
          : "ボスと開始地点を選択してください"}
      </div>
    </div>
  );
}
