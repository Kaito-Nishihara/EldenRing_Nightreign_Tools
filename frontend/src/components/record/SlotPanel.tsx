"use client";

import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import Select from "@/components/ui/Select";
import Badge from "@/components/ui/Badge";
import { dayNumeral } from "@/lib/patternMap";
import { sameSlot, slotKey, type Day, type Phase, type RecordDraft, type Slot } from "@/lib/recordDraft";
import type { NightBoss, SecondShrinkPoint, ShrinkZone } from "@/types";

interface SlotPanelProps {
  draft: RecordDraft;
  activeSlot: Slot | null;
  filledCount: number;
  shrinkZones: ShrinkZone[];
  secondShrinkPoints: SecondShrinkPoint[];
  nightBosses: NightBoss[];
  onSelectSlot: (slot: Slot) => void;
  /** 選んでいるスロットに収縮先を入れる。第 1 次なら収縮候補地、第 2 次なら第二収縮地点の識別子 */
  onFill: (targetId: number) => void;
  onNightBossChange: (day: Day, id: number | null) => void;
}

const PHASE_LABEL: Record<Phase, string> = { 1: "第 1 次", 2: "第 2 次" };

/** スロットの名前。例: DAY I 第 2 次。狭い幅のシートの取っ手にも使う */
export function slotLabel(slot: Slot): string {
  return `DAY ${dayNumeral(slot.day)} ${PHASE_LABEL[slot.phase]}`;
}

/**
 * 収縮先のパネル(プレイ記録 3.2)。
 * 4 つのスロットを DAY I と DAY II に分けて並べ、その日の夜ボスを選ぶ欄を添える。
 * マップを使えない人のために、選んでいるスロットの収縮先を一覧から選ぶ欄を置く(AC-REC-15)。
 */
export default function SlotPanel({
  draft,
  activeSlot,
  filledCount,
  shrinkZones,
  secondShrinkPoints,
  nightBosses,
  onSelectSlot,
  onFill,
  onNightBossChange,
}: SlotPanelProps) {
  /** スロットに入っている収縮先の名前 */
  const targetName = (slot: Slot): string | null => {
    const id = draft.slots[slotKey(slot)];
    if (id === null) return null;
    const targets = slot.phase === 1 ? shrinkZones : secondShrinkPoints;
    return targets.find((target) => target.id === id)?.nameJa ?? `#${id}`;
  };

  const candidates = activeSlot?.phase === 1 ? shrinkZones : secondShrinkPoints;

  return (
    <Panel ornate aria-labelledby="slot-title">
      <div className="flex items-start justify-between">
        <SectionHeading eyebrow="Night Rain" title="収縮先" id="slot-title" />
        <Badge tone="azure">{filledCount} / 4</Badge>
      </div>

      <div className="flex flex-col gap-4">
        {([1, 2] as const).map((day) => (
          <div key={day} className="flex flex-col gap-2">
            <span aria-hidden className="font-display text-xs uppercase tracking-display text-ash-soft">
              Day {dayNumeral(day)}
            </span>
            <div className="flex flex-col gap-2" role="group" aria-label={`DAY ${dayNumeral(day)} の収縮先`}>
              {([1, 2] as const).map((phase) => {
                const slot = { day, phase };
                const name = targetName(slot);
                const active = sameSlot(slot, activeSlot);
                return (
                  <button
                    key={phase}
                    type="button"
                    aria-pressed={active}
                    aria-label={`${slotLabel(slot)}: ${name ?? "未入力"}`}
                    onClick={() => onSelectSlot(slot)}
                    className={`flex min-h-12 items-center gap-3 border px-3 text-left transition-colors duration-200 ease-out ${
                      active
                        ? "border-azure bg-gradient-to-r from-azure/15 to-transparent shadow-[inset_2px_0_0_var(--color-azure)]"
                        : "border-line hover:border-azure-deep"
                    }`}
                  >
                    <span className="w-14 flex-none text-xs text-ash-soft">{PHASE_LABEL[phase]}</span>
                    <span className={`min-w-0 flex-1 truncate ${name ? "font-bold text-ash" : "text-sm text-ash-soft"}`}>
                      {name ?? (active ? `マップで${phase === 1 ? "収縮候補地" : "第二収縮地点"}を押す` : "未入力")}
                    </span>
                    {/* 色だけで入れたことを示さない(プレイ記録 3.2) */}
                    <span aria-hidden className="w-4 flex-none text-grace">
                      {name ? "✓" : ""}
                    </span>
                  </button>
                );
              })}
            </div>
            <Select
              label={`DAY ${dayNumeral(day)} の夜ボス(任意)`}
              value={(day === 1 ? draft.day1NightBossId : draft.day2NightBossId) ?? ""}
              onChange={(e) => onNightBossChange(day, e.target.value ? Number(e.target.value) : null)}
            >
              <option value="">選ばない</option>
              {nightBosses
                .filter((boss) => boss.day === day)
                .map((boss) => (
                  <option key={boss.id} value={boss.id}>
                    {boss.nameJa}
                  </option>
                ))}
            </Select>
          </div>
        ))}

        {activeSlot && (
          <details className="text-sm">
            <summary className="min-h-11 cursor-pointer py-2 text-ash-soft">一覧から選ぶ(キーボードで操作するとき)</summary>
            <Select
              label={`${slotLabel(activeSlot)}の${activeSlot.phase === 1 ? "収縮候補地" : "第二収縮地点"}`}
              value={draft.slots[slotKey(activeSlot)] ?? ""}
              onChange={(e) => e.target.value && onFill(Number(e.target.value))}
            >
              <option value="">選んでください</option>
              {candidates.map((target) => (
                <option key={target.id} value={target.id}>
                  {target.nameJa}
                </option>
              ))}
            </Select>
          </details>
        )}
      </div>
    </Panel>
  );
}
