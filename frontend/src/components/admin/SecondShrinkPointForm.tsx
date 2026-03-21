"use client";

import { useEffect, useState } from "react";
import type { SecondShrinkPoint } from "@/types";

interface SecondShrinkPointFormProps {
  position: { x: number; y: number } | null;
  editing: SecondShrinkPoint | null;
  saving: boolean;
  onSave: (data: Omit<SecondShrinkPoint, "id">) => void;
  onDelete: (id: number) => void;
  onCancel: () => void;
}

/**
 * 第二収縮地点候補登録フォーム
 * 座標のみ管理（夜ボスとの紐付けは記録画面で行う）
 */
export default function SecondShrinkPointForm({
  position,
  editing,
  saving,
  onSave,
  onDelete,
  onCancel,
}: SecondShrinkPointFormProps) {
  const [nameJa, setNameJa] = useState("");

  useEffect(() => {
    if (editing) {
      setNameJa(editing.nameJa);
    } else {
      setNameJa("");
    }
  }, [editing]);

  if (!position) {
    return (
      <div className="p-4 border border-fuchsia-700 rounded-lg bg-fuchsia-900/20">
        <h3 className="text-sm font-bold text-fuchsia-300 mb-2">第二収縮地点候補登録</h3>
        <p className="text-xs text-fuchsia-200/70">
          マップをクリックして候補地点を登録してください
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
    });
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="p-4 border border-fuchsia-700 rounded-lg bg-fuchsia-900/20 space-y-3"
    >
      <h3 className="text-sm font-bold text-fuchsia-300">
        {editing ? "第二収縮地点候補を編集" : "第二収縮地点候補を登録"}
      </h3>

      <div className="flex gap-2 text-xs">
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">X: {position.x}</span>
        <span className="bg-gray-700 px-2 py-1 rounded text-gray-300">Y: {position.y}</span>
      </div>

      <div>
        <label className="block text-xs text-gray-400 mb-1">名前</label>
        <input
          type="text"
          value={nameJa}
          onChange={(e) => setNameJa(e.target.value)}
          required
          className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm"
          placeholder="例: 北側崖上"
        />
      </div>

      <div className="flex gap-2">
        <button
          type="submit"
          disabled={saving || !nameJa}
          className="flex-1 bg-fuchsia-600 hover:bg-fuchsia-500 disabled:bg-gray-600 text-white text-sm px-3 py-2 rounded-md transition-colors"
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
