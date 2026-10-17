"use client";

import Link from "next/link";
import { useBuildSimulator } from "@/hooks/useBuildSimulator";
import RelicSlotPanel from "@/components/build/RelicSlotPanel";
import BuildResultPanel from "@/components/build/BuildResultPanel";

/**
 * ビルドシミュレーターページ
 * 通常効果3枠 + 深層効果3枠を設定し合算効果をリアルタイム計算する
 */
export default function BuildPage() {
  const {
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
  } = useBuildSimulator();

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100">
      {/* ヘッダー */}
      <header className="border-b border-gray-800 px-4 py-3 flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Link href="/" className="text-gray-400 hover:text-gray-200 text-sm">
            ← トップ
          </Link>
          <h1 className="text-lg font-bold text-amber-400">
            ビルドシミュレーター
          </h1>
        </div>

        <button
          type="button"
          onClick={resetAll}
          className="text-xs text-gray-400 hover:text-red-400 border border-gray-700 hover:border-red-700 rounded px-2 py-1 transition-colors"
        >
          リセット
        </button>
      </header>

      {error && (
        <div className="m-4 p-3 bg-red-900/50 border border-red-700 rounded text-sm text-red-300">
          {error}
        </div>
      )}

      {loading && (
        <div className="flex items-center justify-center h-64 text-gray-400 text-sm">
          データ読み込み中...
        </div>
      )}

      {!loading && !error && (
        <div className="flex flex-col lg:flex-row h-[calc(100vh-57px)]">
          {/* 左ペイン: 通常効果 + 深層効果 */}
          <div className="lg:w-2/3 p-4 space-y-4 overflow-y-auto">
            <p className="text-xs text-gray-500">
              遺物の通常効果（3枠）と深層効果（3枠）を選択すると、右に合算効果が表示されます。
            </p>

            {slots.map((slot) => (
              <RelicSlotPanel
                key={slot.id}
                slot={slot}
                effectsByCategory={
                  slot.isDeep ? deepEffectsByCategory : normalEffectsByCategory
                }
                effectMap={effectMap}
                onEffectChange={(entryId, effectId) =>
                  updateEffect(slot.id, entryId, effectId)
                }
                onRankChange={(entryId, rank) =>
                  updateRank(slot.id, entryId, rank)
                }
              />
            ))}
          </div>

          {/* 右ペイン: 計算結果 */}
          <div className="lg:w-1/3 border-t lg:border-t-0 lg:border-l border-gray-800 p-4 overflow-y-auto bg-gray-900/50">
            <h2 className="text-sm font-semibold text-gray-300 mb-4">
              合算効果
            </h2>
            <BuildResultPanel resultGroups={resultGroups} />
          </div>
        </div>
      )}
    </div>
  );
}
