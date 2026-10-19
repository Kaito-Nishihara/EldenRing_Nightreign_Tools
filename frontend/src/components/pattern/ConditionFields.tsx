"use client";

import { useState } from "react";
import type { Boss, Jihen, SpawnPoint } from "@/types";
import Select from "@/components/ui/Select";
import Badge from "@/components/ui/Badge";
import { bossSigil } from "@/lib/bossSigils";

/** 初めに出す夜の王の数。残りは「ほかの N 体を表示」で出す(パターン検索 3.1) */
const INITIAL_BOSS_COUNT = 6;

export interface ConditionFieldsProps {
  bosses: Boss[];
  jihenList: Jihen[];
  spawnPoints: SpawnPoint[];
  bossId: string | null;
  jihenId: string | null;
  spawnPointId: number | null;
  onChange: (patch: { bossId?: string | null; jihenId?: string | null; spawnPointId?: number | null }) => void;
}

/**
 * 条件の欄(パターン検索 3.1、プレイ記録 3.1)。夜の王は頭文字と名前のボタンを 2 列に並べ、地変と開始地点は選択の欄で選ぶ。
 * パネルの枠は持たない。パターン検索は ConditionPanel に、記録の画面は「今回の条件」のパネルに入れる。
 */
export default function ConditionFields({
  bosses,
  jihenList,
  spawnPoints,
  bossId,
  jihenId,
  spawnPointId,
  onChange,
}: ConditionFieldsProps) {
  const [showAll, setShowAll] = useState(false);
  // 選んだ夜の王が隠れる位置にあるときは、初めから全部出す
  const selectedIndex = bosses.findIndex((boss) => boss.id === bossId);
  const expanded = showAll || selectedIndex >= INITIAL_BOSS_COUNT;
  const visibleBosses = expanded ? bosses : bosses.slice(0, INITIAL_BOSS_COUNT);
  const hiddenCount = bosses.length - INITIAL_BOSS_COUNT;

  return (
    <div>
      <fieldset className="mb-4">
        <legend className="mb-2 text-sm text-ash-soft">夜の王</legend>
        <div className="grid grid-cols-2 gap-2">
          {visibleBosses.map((boss) => {
            const selected = boss.id === bossId;
            return (
              <button
                key={boss.id}
                type="button"
                aria-pressed={selected}
                onClick={() => onChange({ bossId: selected ? null : boss.id })}
                className={`flex min-h-11 items-center gap-2 border px-2 text-left text-sm transition-colors duration-200 ease-out ${
                  selected
                    ? "border-azure bg-gradient-to-r from-azure/15 to-transparent text-ash shadow-[inset_2px_0_0_var(--color-azure)]"
                    : "border-line text-ash hover:border-azure-deep"
                }`}
              >
                <span
                  aria-hidden
                  className={`flex h-7 w-7 flex-none items-center justify-center border font-serif text-sm font-extrabold ${
                    selected ? "border-azure text-azure-bright" : "border-azure-deep text-azure"
                  }`}
                >
                  {bossSigil(boss)}
                </span>
                <span className="min-w-0 truncate">{boss.nameJa}</span>
                {boss.isDlc && <span className="sr-only">(DLC)</span>}
              </button>
            );
          })}
        </div>
        {hiddenCount > 0 && !expanded && (
          <button type="button" onClick={() => setShowAll(true)} className="mt-2 min-h-11 w-full text-sm text-azure hover:underline">
            ほかの {hiddenCount} 体を表示
          </button>
        )}
        {bosses.some((boss) => boss.id === bossId && boss.weaknesses.length > 0) && (
          <div className="mt-2 flex flex-wrap items-center gap-1 text-xs text-ash-soft">
            弱点
            {bosses
              .find((boss) => boss.id === bossId)
              ?.weaknesses.map((weakness) => (
                <Badge key={weakness} tone="azure">
                  {weakness}
                </Badge>
              ))}
          </div>
        )}
      </fieldset>

      <div className="flex flex-col gap-4">
        <Select label="地変" value={jihenId ?? ""} onChange={(e) => onChange({ jihenId: e.target.value || null })}>
          <option value="">なし</option>
          {jihenList.map((jihen) => (
            <option key={jihen.id} value={jihen.id}>
              {jihen.nameJa}
              {jihen.isDlc ? "(DLC)" : ""}
            </option>
          ))}
        </Select>

        <Select
          label="開始地点"
          hint="マップの開始地点のアイコンを押しても選べます"
          value={spawnPointId ?? ""}
          onChange={(e) => onChange({ spawnPointId: e.target.value ? Number(e.target.value) : null })}
        >
          <option value="">選んでください</option>
          {spawnPoints.map((spawn) => (
            <option key={spawn.id} value={spawn.id}>
              {spawn.nameJa}
            </option>
          ))}
        </Select>
      </div>
    </div>
  );
}
