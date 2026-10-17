import type { Pattern, PatternEntry } from "@/types";

/**
 * パターンの一覧のカードに出す文言を作る(パターン検索 3.2)。
 * 画面に依らない計算なので、ここに置いて単体のテストで確かめる。
 */

/** 日ごとの収縮先の名前。記録のない段階は null */
export interface DayRoute {
  day: 1 | 2;
  first: string | null;
  second: string | null;
}

/** 収縮先の名前。第 1 次は収縮候補地、第 2 次は第二収縮地点の名前 */
function entryName(entry: PatternEntry): string | null {
  return (entry.phase === 1 ? entry.zone?.nameJa : entry.secondShrinkPoint?.nameJa) ?? null;
}

/** 日ごとの収縮先。記録のない日は出さない */
export function dayRoutes(pattern: Pattern): DayRoute[] {
  return ([1, 2] as const)
    .map((day) => {
      const find = (phase: 1 | 2) => {
        const entry = pattern.entries.find((e) => e.day === day && e.phase === phase);
        return entry ? entryName(entry) : null;
      };
      return { day, first: find(1), second: find(2) };
    })
    .filter((route) => route.first !== null || route.second !== null);
}

/** 題。1 日目と 2 日目の第 1 次の収縮先を矢印でつなぐ。片方がないときは「未記録」 */
export function patternTitle(pattern: Pattern): string {
  const first = (day: 1 | 2) =>
    pattern.entries.find((e) => e.day === day && e.phase === 1)?.zone?.nameJa ?? "未記録";
  return `${first(1)} → ${first(2)}`;
}

/** 記録の数を、条件の記録の総数で割った割合(0 から 100 の整数) */
export function patternShare(recordCount: number, totalRecords: number): number {
  if (totalRecords <= 0) return 0;
  return Math.round((recordCount / totalRecords) * 100);
}

/** 「6 回中 3 回」 */
export function shareText(recordCount: number, totalRecords: number): string {
  return `${totalRecords} 回中 ${recordCount} 回`;
}

/** UTC の日時を、日本時間の日付(2026-10-12)にする */
export function formatJstDate(iso: string): string {
  const parts = new Intl.DateTimeFormat("ja-JP", {
    timeZone: "Asia/Tokyo",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(new Date(iso));
  const part = (type: Intl.DateTimeFormatPartTypes) => parts.find((p) => p.type === type)?.value ?? "";
  return `${part("year")}-${part("month")}-${part("day")}`;
}
