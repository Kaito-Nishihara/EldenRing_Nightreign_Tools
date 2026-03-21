"use client";

import { useCallback } from "react";
import type { Boss, Jihen, SpawnPoint, ShrinkZone } from "@/types";
import type { PlayRecordFormState } from "@/hooks/usePlayRecord";
import { slotKey } from "@/hooks/usePlayRecord";

/** スロット定義 */
const SLOTS: Array<{ day: 1 | 2; phase: 1 | 2; label: string }> = [
  { day: 1, phase: 1, label: "Day1 第1次収縮" },
  { day: 1, phase: 2, label: "Day1 第2次収縮" },
  { day: 2, phase: 1, label: "Day2 第1次収縮" },
  { day: 2, phase: 2, label: "Day2 第2次収縮" },
];

interface RecordFormProps {
  /** マスタデータ */
  bosses: Boss[];
  jihenList: Jihen[];
  spawnPoints: SpawnPoint[];
  zones: ShrinkZone[];
  /** フォーム状態 */
  form: PlayRecordFormState;
  /** 表示中の日 */
  visibleDay: 1 | 2;
  /** アクティブスロット */
  activeSlot: { day: 1 | 2; phase: 1 | 2 } | null;
  /** 保存中 */
  saving: boolean;
  /** エラー */
  error: string | null;
  /** 保存成功 */
  saved: boolean;
  /** 入力済み数 */
  filledCount: number;
  /** マスタ読み込み中 */
  masterLoading: boolean;
  /** マスタエラー */
  masterError: string | null;
  /** ハンドラー */
  onBossSelect: (id: string | null) => void;
  onJihenSelect: (id: string | null) => void;
  onSpawnPointSelect: (id: number | null) => void;
  onNoteChange: (note: string) => void;
  onVisibleDayChange: (day: 1 | 2) => void;
  onSlotSelect: (slot: { day: 1 | 2; phase: 1 | 2 } | null) => void;
  onEntryRemove: (day: 1 | 2, phase: 1 | 2) => void;
  onSubmit: () => void;
  onReset: () => void;
}

/**
 * プレイ記録入力フォーム
 * 条件選択・4スロット・備考入力を1つのサイドパネルに表示
 */
export default function RecordForm({
  bosses,
  jihenList,
  spawnPoints,
  zones,
  form,
  visibleDay,
  activeSlot,
  saving,
  error,
  saved,
  filledCount,
  masterLoading,
  masterError,
  onBossSelect,
  onJihenSelect,
  onSpawnPointSelect,
  onNoteChange,
  onVisibleDayChange,
  onSlotSelect,
  onEntryRemove,
  onSubmit,
  onReset,
}: RecordFormProps) {
  /** セレクト変更ハンドラー */
  const handleBossChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) =>
      onBossSelect(e.target.value || null),
    [onBossSelect]
  );
  const handleJihenChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) =>
      onJihenSelect(e.target.value || null),
    [onJihenSelect]
  );
  /** ゾーン名を取得 */
  const getZoneName = (zoneId: number | null): string => {
    if (zoneId === null) return "";
    const zone = zones.find((z) => z.id === zoneId);
    return zone?.nameJa ?? `Zone #${zoneId}`;
  };

  const visibleSlots = SLOTS.filter(({ day }) => day === visibleDay);

  if (masterError) {
    return (
      <div className="p-4 border border-red-700 rounded-lg bg-red-900/20">
        <h2 className="text-lg font-bold text-red-300 mb-2">エラー</h2>
        <p className="text-sm text-red-400">{masterError}</p>
      </div>
    );
  }

  if (masterLoading) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h2 className="text-lg font-bold text-gray-200 mb-2">記録入力</h2>
        <div className="flex items-center gap-2 text-gray-400">
          <div className="w-4 h-4 border-2 border-gray-400 border-t-transparent rounded-full animate-spin" />
          <span className="text-sm">読み込み中...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* 条件選択 */}
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50 space-y-3">
        <h2 className="text-lg font-bold text-gray-200">条件</h2>

        {/* ボス */}
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">
            ボス
          </label>
          <select
            value={form.bossId ?? ""}
            onChange={handleBossChange}
            className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">-- ボスを選択 --</option>
            {bosses.map((b) => (
              <option key={b.id} value={b.id}>
                {b.nameJa}
              </option>
            ))}
          </select>
        </div>

        {/* 地変 */}
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">
            地変
          </label>
          <select
            value={form.jihenId ?? ""}
            onChange={handleJihenChange}
            className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">なし</option>
            {jihenList.map((j) => (
              <option key={j.id} value={j.id}>
                {j.nameJa}
              </option>
            ))}
          </select>
        </div>

        {/* 開始地点（マップ上で選択） */}
        <div>
          <label className="block text-sm font-medium text-gray-300 mb-1">
            開始地点
          </label>
          {form.spawnPointId ? (
            <div className="flex items-center gap-2">
              <span className="flex-1 bg-gray-700 border border-green-600 rounded-md px-3 py-2 text-green-200 text-sm">
                {spawnPoints.find((sp) => sp.id === form.spawnPointId)?.nameJa ?? "選択済み"}
              </span>
              <button
                type="button"
                onClick={() => onSpawnPointSelect(null)}
                className="text-gray-400 hover:text-red-400 text-xs px-2 py-2"
                title="変更"
              >
                変更
              </button>
            </div>
          ) : (
            <p className="bg-gray-700/50 border border-gray-600 rounded-md px-3 py-2 text-gray-500 text-sm">
              マップ上のアイコンをタップして選択
            </p>
          )}
        </div>
      </div>

      {/* 収縮スロット */}
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50 space-y-2">
        <h2 className="text-lg font-bold text-gray-200">
          収縮先
          <span className="text-sm font-normal text-gray-400 ml-2">
            {filledCount}/4
          </span>
        </h2>
        <p className="text-xs text-gray-500 mb-2">
          スロットを選んでマップ上のゾーンをタップ
        </p>

        <div className="flex gap-2 mb-2">
          <button
            type="button"
            onClick={() => onVisibleDayChange(1)}
            className={`flex-1 rounded-md border px-3 py-2 text-sm transition-colors ${
              visibleDay === 1
                ? "border-blue-500 bg-blue-900/30 text-blue-200"
                : "border-gray-600 bg-gray-700/50 text-gray-400 hover:bg-gray-700"
            }`}
          >
            Day1
          </button>
          <button
            type="button"
            onClick={() => onVisibleDayChange(2)}
            className={`flex-1 rounded-md border px-3 py-2 text-sm transition-colors ${
              visibleDay === 2
                ? "border-blue-500 bg-blue-900/30 text-blue-200"
                : "border-gray-600 bg-gray-700/50 text-gray-400 hover:bg-gray-700"
            }`}
          >
            Day2
          </button>
        </div>

        {visibleSlots.map(({ day, phase, label }) => {
          const key = slotKey(day, phase);
          const zoneId = form.entries[key];
          const isActive =
            activeSlot?.day === day && activeSlot?.phase === phase;
          const isFilled = zoneId !== null;

          return (
            <button
              key={key}
              type="button"
              onClick={() =>
                onSlotSelect(isActive ? null : { day, phase })
              }
              className={`w-full flex items-center gap-2 px-3 py-2 rounded-md text-sm transition-colors text-left ${
                isActive
                  ? "bg-blue-600/30 border border-blue-500 text-blue-200"
                  : isFilled
                    ? "bg-green-900/20 border border-green-700 text-green-200"
                    : "bg-gray-700/50 border border-gray-600 text-gray-400 hover:bg-gray-700"
              }`}
            >
              {/* チェックマーク */}
              <span className="w-5 h-5 flex items-center justify-center flex-shrink-0">
                {isFilled ? (
                  <svg
                    className="w-4 h-4 text-green-400"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={3}
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                ) : (
                  <span className="w-3 h-3 rounded-full border-2 border-gray-500" />
                )}
              </span>

              {/* ラベル */}
              <span className="flex-1">{label}</span>

              {/* ゾーン名 or 未選択 */}
              {isFilled ? (
                <span className="text-xs text-green-300 truncate max-w-[120px]">
                  {getZoneName(zoneId)}
                </span>
              ) : isActive ? (
                <span className="text-xs text-blue-300 animate-pulse">
                  選択中...
                </span>
              ) : null}

              {/* 取り消しボタン */}
              {isFilled && (
                <button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation();
                    onEntryRemove(day, phase);
                  }}
                  className="text-gray-500 hover:text-red-400 transition-colors"
                  title="取り消し"
                >
                  <svg
                    className="w-4 h-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    strokeWidth={2}
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M6 18L18 6M6 6l12 12"
                    />
                  </svg>
                </button>
              )}
            </button>
          );
        })}
      </div>

      {/* 備考 */}
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <label className="block text-sm font-medium text-gray-300 mb-1">
          備考
        </label>
        <textarea
          value={form.note}
          onChange={(e) => onNoteChange(e.target.value)}
          placeholder="メモがあれば記入（任意）"
          rows={2}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
        />
      </div>

      {/* エラー */}
      {error && (
        <div className="p-3 border border-red-700 rounded-lg bg-red-900/20 text-sm text-red-400">
          {error}
        </div>
      )}

      {/* 保存成功 */}
      {saved && (
        <div className="p-3 border border-green-700 rounded-lg bg-green-900/20 text-sm text-green-400">
          記録を保存しました
        </div>
      )}

      {/* ボタン */}
      <div className="flex gap-2">
        <button
          type="button"
          onClick={onSubmit}
          disabled={saving}
          className="flex-1 bg-blue-600 hover:bg-blue-500 disabled:bg-gray-600 text-white text-sm px-4 py-2.5 rounded-md transition-colors font-medium"
        >
          {saving ? "保存中..." : "記録を保存"}
        </button>
        <button
          type="button"
          onClick={onReset}
          disabled={saving}
          className="bg-gray-600 hover:bg-gray-500 disabled:bg-gray-700 text-white text-sm px-4 py-2.5 rounded-md transition-colors"
        >
          リセット
        </button>
      </div>
    </div>
  );
}
