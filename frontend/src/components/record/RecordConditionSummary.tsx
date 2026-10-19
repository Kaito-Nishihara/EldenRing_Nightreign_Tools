"use client";

import { useState } from "react";
import ConditionFields, { type ConditionFieldsProps } from "@/components/pattern/ConditionFields";
import Button from "@/components/ui/Button";
import { bossSigil } from "@/lib/bossSigils";

/**
 * 今回の条件(プレイ記録 3.1)。選び方はパターン検索と同じである。
 * 夜の王と開始地点を選んだ後は、頭文字と名前だけの短い表示に畳み、「変える」で開く。
 * パネルの枠は持たない。備考と保存と同じ「今回の条件」のパネルに入れる(試作)。
 */
export default function RecordConditionSummary(props: ConditionFieldsProps) {
  const { bosses, jihenList, spawnPoints, bossId, jihenId, spawnPointId } = props;
  const [editing, setEditing] = useState(false);

  const boss = bosses.find((b) => b.id === bossId);
  const spawn = spawnPoints.find((s) => s.id === spawnPointId);

  if (editing || !boss || !spawn) {
    return (
      <div className="flex flex-col gap-2">
        {/* 選び終えているときだけ、畳んで短い表示に戻せる */}
        {boss && spawn && (
          <Button variant="ghost" onClick={() => setEditing(false)} className="ml-auto min-h-11 px-2 text-sm">
            条件を閉じる
          </Button>
        )}
        <ConditionFields {...props} />
      </div>
    );
  }

  const jihenName = jihenList.find((j) => j.id === jihenId)?.nameJa ?? "地変なし";

  return (
    <div className="flex items-center gap-3">
      <span
        aria-hidden
        className="flex h-9 w-9 flex-none items-center justify-center border border-azure font-serif text-base font-extrabold text-azure-bright"
      >
        {bossSigil(boss)}
      </span>
      <div className="min-w-0">
        <p className="truncate font-serif font-extrabold text-azure-bright">{boss.nameJa}</p>
        <p className="truncate text-sm text-ash-soft">
          {jihenName} ／ {spawn.nameJa}
        </p>
      </div>
      <Button variant="ghost" onClick={() => setEditing(true)} className="ml-auto min-h-11 px-2 text-sm">
        変える
      </Button>
    </div>
  );
}
