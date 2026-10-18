import type { Pattern, SecondShrinkPoint, ShrinkZone, SpawnPoint } from "@/types";
import type { RainCircle } from "@/components/map/ShrinkOverlay";

/** マップに出す日の絞り込み(パターン検索 3.3)。初めは「すべて」 */
export type DayFilter = "all" | "1" | "2";

export const DAY_FILTER_OPTIONS = [
  { value: "all", label: "すべて" },
  { value: "1", label: "DAY I", display: true },
  { value: "2", label: "DAY II", display: true },
] as const satisfies readonly { value: DayFilter; label: string; display?: boolean }[];

/** 日のローマ数字。マップの日の丸と名前に使う */
export function dayNumeral(day: 1 | 2): "I" | "II" {
  return day === 1 ? "I" : "II";
}

/** 1 日分の収縮先。第 1 次は収縮候補地、第 2 次は第二収縮地点。記録のない段階は null */
export interface DayShrink {
  day: 1 | 2;
  zone: ShrinkZone | null;
  point: SecondShrinkPoint | null;
}

/** 選んだパターンをマップに描くための値 */
export interface PatternMapView {
  /** 出す日の収縮先。記録のない日は入れない */
  days: DayShrink[];
  /** 開始地点から日の順に第 1 次の中心をつなぐ線の点([x1, y1, x2, y2, ...])。「すべて」のときだけ作る */
  route: number[];
  /** 円の外を暗がりで塗るための円 */
  rainCircles: RainCircle[];
}

/**
 * 選んだパターンで実際に起きた収縮だけを、マップに描く形にする(パターン検索 3.3)。
 * 収縮候補地の 12 の円と、選んでいないパターンは入れない(AC-PAT-12、AC-PAT-17)。
 */
export function patternMapView(pattern: Pattern | null, filter: DayFilter, spawn: SpawnPoint | null): PatternMapView {
  if (!pattern) return { days: [], route: [], rainCircles: [] };

  const targetDays: (1 | 2)[] = filter === "all" ? [1, 2] : [filter === "1" ? 1 : 2];
  const days = targetDays
    .map((day): DayShrink => {
      const entry = (phase: 1 | 2) => pattern.entries.find((e) => e.day === day && e.phase === phase);
      return { day, zone: entry(1)?.zone ?? null, point: entry(2)?.secondShrinkPoint ?? null };
    })
    .filter((shrink) => shrink.zone !== null || shrink.point !== null);

  const zones = days.flatMap((shrink) => (shrink.zone ? [shrink.zone] : []));
  // 開始地点と第 1 次の中心が合わせて 2 点以上あるときだけ線にする
  const routePoints = filter === "all" && spawn ? [{ x: spawn.x, y: spawn.y }, ...zones.map((z) => ({ x: z.centerX, y: z.centerY }))] : [];

  return {
    days,
    route: routePoints.length >= 2 ? routePoints.flatMap((p) => [p.x, p.y]) : [],
    rainCircles: zones.map((zone) => ({ x: zone.centerX, y: zone.centerY, radius: zone.radius })),
  };
}
