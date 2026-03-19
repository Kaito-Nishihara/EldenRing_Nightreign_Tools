"use client";

import { useState, useEffect } from "react";
import type { ShrinkZone } from "@/types";

interface ShrinkZoneFormProps {
  /** クリック位置 */
  position: { x: number; y: number } | null;
  /** 編集中のデータ（null=新規作成） */
  editing: ShrinkZone | null;
  /** 保存中 */
  saving: boolean;
  /** 保存ハンドラー */
  onSave: (data: Omit<ShrinkZone, "id">) => void;
  /** 削除ハンドラー */
  onDelete: (id: number) => void;
  /** キャンセルハンドラー */
  onCancel: () => void;
}

/**
 * 収縮候補地登録フォーム
 * マップクリックした座標を中心に収縮候補地を登録・編集する
 */
export default function ShrinkZoneForm({
  position,
  editing,
  saving,
  onSave,
  onDelete,
  onCancel,
}: ShrinkZoneFormProps) {
  const [nameJa, setNameJa] = useState("");
  const [iconPath, setIconPath] = useState("");
  const [radius, setRadius] = useState(80);

  /** 編集データが変わったらフォームを更新 */
  useEffect(() => {
    if (editing) {
      setNameJa(editing.nameJa);
      setIconPath(editing.iconPath ?? "");
      setRadius(editing.radius);
    } else {
      setNameJa("");
      setIconPath("");
      setRadius(80);
    }
  }, [editing]);

  if (!position) {
    return (
      <div className="p-4 border border-gray-700 rounded-lg bg-gray-800/50">
        <h3 className="text-sm font-bold text-gray-300 mb-2">収縮候補地登録</h3>
        <p className="text-xs text-gray-500">
          マップをクリックして中心座標を選択してください
        </p>
      </div>
    );
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      nameJa,
      iconPath: iconPath || undefined,
      centerX: position.x,
      centerY: position.y,
      radius,
    });
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="p-4 border border-green-700 rounded-lg bg-green-900/20 space-y-3"
    >
      <h3 className="text-sm font-bold text-green-300">
        {editing ? "収縮候補地を編集" : "収縮候補地を登録"}
      </h3>

      {/* 座標表示 */}
      <div className="flex gap-2 text-xs">
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">
          中心X: {position.x}
        </span>
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">
          中心Y: {position.y}
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
          placeholder="例: 北の砦"
        />
      </div>

      {/* アイコンパス */}
      <div>
        <label className="block text-xs text-gray-400 mb-1">アイコンパス（任意）</label>
        <input
          type="text"
          value={iconPath}
          onChange={(e) => setIconPath(e.target.value)}
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm"
          placeholder="例: /images/zones/Fort.webp"
        />
      </div>

      {/* 半径 */}
      <div>
        <label className="block text-xs text-gray-400 mb-1">
          半径: {radius}
        </label>
        <input
          type="range"
          min={20}
          max={200}
          value={radius}
          onChange={(e) => setRadius(Number(e.target.value))}
          className="w-full"
        />
      </div>

      {/* ボタン */}
      <div className="flex gap-2">
        <button
          type="submit"
          disabled={saving || !nameJa}
          className="flex-1 bg-green-600 hover:bg-green-500 disabled:bg-gray-600 text-white text-sm px-3 py-2 rounded-md transition-colors"
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
