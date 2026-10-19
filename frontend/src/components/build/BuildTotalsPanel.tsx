"use client";

import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import { describeEntries, formatTotal, type BuildTotalCategory } from "@/lib/buildCalc";

export const TOTALS_EMPTY_MESSAGE = "効果を選ぶと、ここに合計が出ます";

interface BuildTotalsPanelProps {
  totals: BuildTotalCategory[];
}

/**
 * 効果の合計(ビルドシミュレーター 3.2)。分類ごとに区切り、各行の下に、どの枠の何が合わさったかを出す。
 * 枠を変えるたびに中身が変わるので、読み上げソフトに伝える(aria-live)
 */
export default function BuildTotalsPanel({ totals }: BuildTotalsPanelProps) {
  return (
    <Panel ornate aria-labelledby="build-totals-title" aria-live="polite">
      <SectionHeading id="build-totals-title" eyebrow="Sum of Grace" title="効果の合計" />
      {totals.length === 0 ? (
        <p className="text-sm text-ash-soft">{TOTALS_EMPTY_MESSAGE}</p>
      ) : (
        <div className="flex flex-col gap-3">
          {totals.map((category) => (
            <section
              key={category.categoryId}
              aria-label={category.nameJa}
              className="border-t border-line pt-3 first:border-t-0 first:pt-0"
            >
              <h3 className="mb-1 font-display text-xs tracking-display text-ash-soft">{category.nameJa}</h3>
              <ul>
                {category.rows.map((row) => (
                  <li key={row.key} className="flex items-baseline justify-between gap-2 py-1 text-sm">
                    <span className="flex min-w-0 flex-col">
                      <span>{row.nameJa}</span>
                      <span className="text-xs text-ash-soft">
                        {row.calcType === "special" ? (row.conditionJa ?? describeEntries(row.entries)) : describeEntries(row.entries)}
                      </span>
                    </span>
                    {row.total !== null && (
                      <span
                        data-testid="build-total-value"
                        className={`whitespace-nowrap font-serif text-xl font-extrabold ${row.isNegative ? "text-ember" : "text-azure-bright"}`}
                      >
                        {formatTotal(row)}
                      </span>
                    )}
                  </li>
                ))}
              </ul>
            </section>
          ))}
        </div>
      )}
      <p className="mt-4 text-xs text-ash-soft">計算はこの端末の中で行い、保存しません。</p>
    </Panel>
  );
}
