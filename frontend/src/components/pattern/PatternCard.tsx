import type { Pattern } from "@/types";
import PatternChip from "@/components/ui/PatternChip";
import Badge from "@/components/ui/Badge";
import { dayRoutes, formatJstDate, patternShare, patternTitle, shareText } from "@/lib/patterns";

interface PatternCardProps {
  pattern: Pattern;
  totalRecords: number;
  selected: boolean;
  onSelect: () => void;
}

/**
 * パターンのカード 1 枚(パターン検索 3.2)。
 * 番号の丸、題、記録の数、日ごとの収縮先、割合の帯、「6 回中 3 回、最後の記録 2026-10-12」を出す。
 * 押すとマップをそのパターンに替える。選んだカードは青の帯と aria-pressed で示す。
 */
export default function PatternCard({ pattern, totalRecords, selected, onSelect }: PatternCardProps) {
  const share = patternShare(pattern.recordCount, totalRecords);

  return (
    <button
      type="button"
      aria-pressed={selected}
      onClick={onSelect}
      className={`flex w-full flex-col gap-2 border p-3 text-left transition-colors duration-200 ease-out ${
        selected
          ? "border-azure-deep bg-gradient-to-r from-azure/15 to-transparent shadow-[inset_2px_0_0_var(--color-azure)]"
          : "border-line bg-night hover:border-azure-deep"
      }`}
    >
      <span className="flex items-center gap-2">
        <PatternChip patternIndex={pattern.patternIndex} />
        <span className="min-w-0 flex-1 truncate font-serif text-base font-extrabold text-ash">{patternTitle(pattern)}</span>
        <Badge tone={pattern.patternIndex === 1 ? "azure" : "neutral"}>{pattern.recordCount} 回</Badge>
      </span>

      <dl className="grid grid-cols-[auto_1fr] gap-x-3 text-sm">
        {dayRoutes(pattern).map((route) => (
          <div key={route.day} className="contents">
            <dt className="font-display text-xs tracking-display text-ash-soft">{route.day === 1 ? "DAY I" : "DAY II"}</dt>
            <dd className="text-ash">
              {route.first && <>第 1 次 {route.first}</>}
              {route.first && route.second && <span className="text-ash-soft"> ／ </span>}
              {route.second && <>第 2 次 {route.second}</>}
            </dd>
          </div>
        ))}
      </dl>

      <span aria-hidden className="block h-1 w-full bg-night-sunk">
        <span className="block h-full bg-azure" style={{ width: `${share}%` }} />
      </span>
      <span className="text-xs text-ash-soft">
        {shareText(pattern.recordCount, totalRecords)}、最後の記録 {formatJstDate(pattern.lastRecordedAt)}
      </span>
    </button>
  );
}
