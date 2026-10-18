import Link from "next/link";
import type { PatternResponse } from "@/types";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import OrnateDivider from "@/components/ui/OrnateDivider";
import { buttonClassName } from "@/components/ui/Button";
import { EmptyState, ErrorNotice, Skeleton } from "@/components/ui/StatusViews";
import PatternCard from "@/components/pattern/PatternCard";

interface PatternListPanelProps {
  /** 夜の王と開始地点がそろっているか */
  ready: boolean;
  data: PatternResponse | null;
  loading: boolean;
  error: string | null;
  selectedPatternIndex: number | null;
  onSelect: (patternIndex: number) => void;
  onRetry: () => void;
  /** 「この条件で記録する」の行き先(条件を引き継ぐ) */
  recordHref: string;
}

/**
 * パターンの一覧のパネル(パターン検索 3.2、3.4)。
 * 条件がそろっていない、読み込み中、エラー、記録がない、の状態ごとに出すものを替える。
 */
export default function PatternListPanel({
  ready,
  data,
  loading,
  error,
  selectedPatternIndex,
  onSelect,
  onRetry,
  recordHref,
}: PatternListPanelProps) {
  const recordLink = (
    <Link href={recordHref} className={buttonClassName("primary", "w-full")}>
      この条件で記録する
    </Link>
  );

  return (
    <Panel ornate aria-labelledby="patterns-title">
      <SectionHeading eyebrow="Patterns" title="過去のパターン" id="patterns-title" />

      {!ready ? (
        <p className="text-sm text-ash-soft">夜の王と開始地点を選ぶと、過去のパターンが出ます</p>
      ) : loading ? (
        <div className="flex flex-col gap-2" aria-busy>
          <Skeleton className="h-28" />
          <Skeleton className="h-28" />
        </div>
      ) : error ? (
        <ErrorNotice message={error} onRetry={onRetry} />
      ) : !data || data.patterns.length === 0 ? (
        <EmptyState title="この条件の記録はまだありません" description="今回のプレイで収縮先を記録しましょう。" action={recordLink} />
      ) : (
        <>
          <p role="status" className="mb-3 text-sm text-ash-soft">
            この条件の記録 {data.totalRecords} 件、{data.patterns.length} パターン
          </p>
          <ul className="flex flex-col gap-2">
            {data.patterns.map((pattern) => (
              <li key={pattern.patternIndex}>
                <PatternCard
                  pattern={pattern}
                  totalRecords={data.totalRecords}
                  selected={pattern.patternIndex === selectedPatternIndex}
                  onSelect={() => onSelect(pattern.patternIndex)}
                />
              </li>
            ))}
          </ul>
          <OrnateDivider />
          {recordLink}
        </>
      )}
    </Panel>
  );
}
