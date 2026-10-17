"use client";

import { useMemo } from "react";
import type { PatternResponse } from "@/types";
import { getPatternColor } from "@/lib/theme";

interface PatternListProps {
  /** パターン検索結果 */
  data: PatternResponse | null;
  /** ローディング状態 */
  loading?: boolean;
  /** エラー情報 */
  error?: string | null;
  /** 選択中のパターンインデックス */
  selectedPatternIndex?: number | null;
  /** パターン選択ハンドラー */
  onPatternSelect?: (patternIndex: number | null) => void;
}

/**
 * パターン一覧
 * 検索結果のパターンを記録件数順に表示し、マップとハイライト連動する
 */
export default function PatternList({
  data,
  loading = false,
  error = null,
  selectedPatternIndex = null,
  onPatternSelect,
}: PatternListProps) {
  /** 統計テキストを生成 */
  const statsText = useMemo(() => {
    if (!data || data.patterns.length === 0 || data.totalRecords === 0) return null;

    const top = data.patterns[0];
    if (!top || top.entries.length === 0) return null;

    const ratio = Math.round((top.recordCount / data.totalRecords) * 100);
    /** Day1-P1のゾーン名を統計テキストに使用 */
    const firstEntry = top.entries.find((e) => e.day === 1 && e.phase === 1);
    if (!firstEntry) return null;

    return `${data.totalRecords}回中${top.recordCount}回(${ratio}%)は${firstEntry.zone.nameJa}付近`;
  }, [data]);

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

      {/* 統計テキスト */}
      {statsText && (
        <p className="text-xs text-gray-400 bg-gray-700/30 px-2 py-1 rounded">
          {statsText}
        </p>
      )}

      {data.patterns.length === 0 ? (
        <p className="text-sm text-gray-400">
          まだデータがありません。今回記録しましょう
        </p>
      ) : (
        <div className="space-y-2">
          {data.patterns.map((pattern) => {
            const color = getPatternColor(pattern.patternIndex - 1);
            const isSelected = selectedPatternIndex === pattern.patternIndex;

            return (
              <button
                key={pattern.patternIndex}
                type="button"
                onClick={() =>
                  onPatternSelect?.(isSelected ? null : pattern.patternIndex)
                }
                className={`w-full text-left p-3 border rounded-md transition-colors ${
                  isSelected
                    ? "border-white/50 bg-gray-600/50"
                    : "border-gray-600 bg-gray-700/50 hover:bg-gray-700/80"
                }`}
              >
                <div className="flex items-center justify-between mb-2">
                  <span className="text-sm font-bold flex items-center gap-2">
                    {/* カラーインジケーター */}
                    <span
                      className="inline-block w-3 h-3 rounded-full"
                      style={{ backgroundColor: color }}
                    />
                    <span style={{ color }}>
                      パターン {pattern.patternIndex}
                    </span>
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
              </button>
            );
          })}
        </div>
      )}
    </div>
  );
}
