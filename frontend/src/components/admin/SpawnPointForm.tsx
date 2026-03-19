"use client";

import { useState, useEffect } from "react";
import type { SpawnPoint } from "@/types";

interface SpawnPointFormProps {
  /** クリック位置 */
  position: { x: number; y: number } | null;
  /** 編集中のデータ（null=新規作成） */
  editing: SpawnPoint | null;
  /** 保存中 */
  saving: boolean;
  /** 保存ハンドラー */
  onSave: (data: Omit<SpawnPoint, "id">) => void;
  /** 削除ハンドラー */
  onDelete: (id: number) => void;
  /** キャンセルハンドラー */
  onCancel: () => void;
}

/**
 * 開始地点登録フォーム
 * マップクリックした座標を元に開始地点を登録・編集する
 */
export default function SpawnPointForm({
  position,
  editing,
  saving,
  onSave,
  onDelete,
  onCancel,
}: SpawnPointFormProps) {
  const [nameJa, setNameJa] = useState("");
  const [nearbyBaseType, setNearbyBaseType] = useState<"church" | "camp">("church");

  /** 編集データが変わったらフォームを更新 */
  useEffect(() => {
    if (editing) {
      setNameJa(editing.nameJa);
      setNearbyBaseType(editing.nearbyBaseType);
    } else {
      setNameJa("");
      setNearbyBaseType("church");
    }
  }, [editing]);

  if (!position) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h3 className="text-sm font-bold text-gray-300 mb-2">開始地点登録</h3>
        <p className="text-xs text-gray-500">
          マップをクリックして座標を選択してください
        </p>
      </div>
    );
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      nameJa,
      x: position.x,
      y: position.y,
      nearbyBaseType,
    });
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="p-4 border border-blue-700 rounded-lg bg-blue-900/20 space-y-3"
    >
      <h3 className="text-sm font-bold text-blue-300">
        {editing ? "開始地点を編集" : "開始地点を登録"}
      </h3>

      {/* 座標表示 */}
      <div className="flex gap-2 text-xs">
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">
          X: {position.x}
        </span>
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">
          Y: {position.y}
        </span>
      </div>

      {/* 名前 */}
      <div>
        <label className="block text-xs text-gray-400 mb-1">名前</label>
        <input
          type="text"
          value={nameJa}
          onChange={(e) => setNameJa(e.target.value)}
          required
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm"
          placeholder="例: 北の高台"
        />
      </div>

      {/* 拠点タイプ */}
      <div>
        <label className="block text-xs text-gray-400 mb-1">拠点タイプ</label>
        <select
          value={nearbyBaseType}
          onChange={(e) => setNearbyBaseType(e.target.value as "church" | "camp")}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm"
        >
          <option value="church">教会</option>
          <option value="camp">野営地</option>
        </select>
      </div>

      {/* ボタン */}
      <div className="flex gap-2">
        <button
          type="submit"
          disabled={saving || !nameJa}
          className="flex-1 bg-blue-600 hover:bg-blue-500 disabled:bg-gray-600 text-white text-sm px-3 py-2 rounded-md transition-colors"
        >
          {saving ? "保存中..." : editing ? "更新" : "登録"}
        </button>
        {editing && (
          <button
            type="button"
            onClick={() => onDelete(editing.id)}
            disabled={saving}
            className="bg-red-600 hover:bg-red-500 disabled:bg-gray-600 text-white text-sm px-3 py-2 rounded-md transition-colors"
          >
            削除
          </button>
        )}
        <button
          type="button"
          onClick={onCancel}
          className="bg-gray-600 hover:bg-gray-500 text-white text-sm px-3 py-2 rounded-md transition-colors"
        >
          取消
        </button>
      </div>
    </form>
  );
}
