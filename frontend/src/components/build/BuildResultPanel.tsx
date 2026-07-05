"use client";

import { useMemo } from "react";
import type { BuildResultGroup } from "@/types";
import { formatCombined } from "@/lib/buildCalc";

interface BuildResultPanelProps {
  resultGroups: BuildResultGroup[];
}

/** カテゴリ表示名マップ */
const CATEGORY_ORDER = [
  "stat",
  "attack",
  "cut_rate",
  "recovery",
  "status_resist",
  "arts_gauge",
  "starting_item",
  "other",
];

/**
 * ビルド計算結果パネル
 * カテゴリごとにグループ化して合算値を表示する
 */
export default function BuildResultPanel({ resultGroups }: BuildResultPanelProps) {
  /** カテゴリ別にグループ化 */
  const byCategory = useMemo(() => {
    const map = new Map<string, { nameJa: string; groups: BuildResultGroup[] }>();
    for (const g of resultGroups) {
      if (!map.has(g.categoryId)) {
        map.set(g.categoryId, { nameJa: g.categoryNameJa, groups: [] });
      }
      map.get(g.categoryId)!.groups.push(g);
    }
    return CATEGORY_ORDER
      .filter((id) => map.has(id))
      .map((id) => ({ categoryId: id, ...map.get(id)! }));
  }, [resultGroups]);

  if (resultGroups.length === 0) {
    return (
      <div className="text-gray-500 text-sm text-center py-8">
        効果を選択すると計算結果が表示されます
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {byCategory.map(({ categoryId, nameJa, groups }) => (
        <div key={categoryId}>
          <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-2 border-b border-gray-700 pb-1">
            {nameJa}
          </h3>
          <div className="space-y-1">
            {groups.map((g) => (
              <ResultRow key={g.key} group={g} />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

function ResultRow({ group }: { group: BuildResultGroup }) {
  const displayValue = formatCombined(group);
  const isNegative =
    group.calcType === "multiply" ? group.combined < 1 : group.combined < 0;

  return (
    <div className="flex items-start justify-between gap-2 py-1">
      <div className="flex-1 min-w-0">
        <span className="text-sm text-gray-200 truncate block">{group.nameJa}</span>
        {group.entries.length > 1 && (
          <span className="text-xs text-gray-500">
            {group.entries.map((e) => {
              const v =
                group.unit === "multiplier"
                  ? `×${e.value}`
                  : group.unit === "percent"
                  ? `+${e.value}%`
                  : `+${e.value}`;
              return v;
            }).join(" → ")}
          </span>
        )}
      </div>
      <span
        className={`text-sm font-bold tabular-nums whitespace-nowrap ${
          isNegative ? "text-red-400" : "text-amber-400"
        }`}
      >
        {displayValue}
      </span>
    </div>
  );
}
