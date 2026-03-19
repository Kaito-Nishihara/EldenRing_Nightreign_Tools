"use client";

import type { PatternResponse } from "@/types";

interface PatternListProps {
  /** パターン検索結果 */
  data: PatternResponse | null;
  /** ローディング状態 */
  loading?: boolean;
  /** エラー情報 */
  error?: string | null;
}

/**
 * パターン一覧
 * 検索結果のパターンを記録件数順に表示する
 */
export default function PatternList({
  data,
  loading = false,
  error = null,
}: PatternListProps) {
  if (error) {
    return (
      <div className="p-4 border border-red-700 rounded-lg bg-red-900/20">
        <h2 className="text-lg font-bold text-red-300 mb-2">検索エラー</h2>
        <p className="text-sm text-red-400">{error}</p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h2 className="text-lg font-bold text-gray-200 mb-2">パターン一覧</h2>
        <div className="flex items-center gap-2 text-gray-400">
          <div className="w-4 h-4 border-2 border-gray-400 border-t-transparent rounded-full animate-spin" />
          <span className="text-sm">検索中...</span>
        </div>
      </div>
    );
  }

  if (!data) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h2 className="text-lg font-bold text-gray-200 mb-2">パターン一覧</h2>
        <p className="text-sm text-gray-400">
          条件を選択するとパターンが表示されます
        </p>
      </div>
    );
  }

  return (
    <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50 space-y-3">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-bold text-gray-200">パターン一覧</h2>
        <span className="text-xs text-gray-400">
          全{data.totalRecords}件
        </span>
      </div>

      {data.patterns.length === 0 ? (
        <p className="text-sm text-gray-400">
          該当するパターンが見つかりませんでした
        </p>
      ) : (
        <div className="space-y-2">
          {data.patterns.map((pattern) => (
            <div
              key={pattern.patternIndex}
              className="p-3 border border-gray-600 rounded-md bg-gray-700/50"
            >
              <div className="flex items-center justify-between mb-2">
                <span className="text-sm font-bold text-blue-300">
                  パターン {pattern.patternIndex}
                </span>
                <span className="text-xs text-gray-400">
                  {pattern.recordCount}件記録
                </span>
              </div>
              <div className="space-y-1">
                {pattern.entries.map((entry) => (
                  <div
                    key={`${entry.day}-${entry.phase}`}
                    className="flex items-center gap-2 text-xs text-gray-300"
                  >
                    <span className="bg-gray-600 px-1.5 py-0.5 rounded text-gray-200">
                      Day{entry.day}-P{entry.phase}
                    </span>
                    <span>{entry.zone.nameJa}</span>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
