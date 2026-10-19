import type { RainCircle } from "@/components/map/ShrinkOverlay";
import { slotKey, type Day, type RecordDraft, type Slot } from "@/lib/recordDraft";
import type { SecondShrinkPoint, ShrinkZone } from "@/types";

/**
 * マップで押せるもの(プレイ記録 3.3)。
 * zones は第 1 次のスロットを選んでいるとき、points は第 2 次のスロットを選んでいるとき、none はどれも選んでいないとき。
 */
export type PickMode = "zones" | "points" | "none";

/** 第二収縮地点の見せ方。dim は、同じ日の第 1 次の円の外にあるもの(不透明度 0.4)。押せることは変わらない */
export type PointEmphasis = "strong" | "dim";

/** 記録の画面のマップに描くための値 */
export interface RecordMapView {
  /** 押せるもの */
  mode: PickMode;
  /** 出している日の第 1 次の収縮候補地。実際の大きさの円で出す */
  selectedZone: ShrinkZone | null;
  /** 出している日の第 2 次の第二収縮地点 */
  selectedPointId: number | null;
  /** 選んでいるスロットに入っている収縮先。印の輪郭を強める */
  activeTargetId: number | null;
  /** 円の外を暗がりで塗るための円 */
  rainCircles: RainCircle[];
  /** 菱形で出す第二収縮地点と、その見せ方。第 1 次を選んでいるときは、入れたものだけを出す */
  points: { point: SecondShrinkPoint; emphasis: PointEmphasis }[];
}

/** 点が収縮候補地の円の中(縁を含む)にあるか */
export function isInsideZone(point: { x: number; y: number }, zone: ShrinkZone): boolean {
  return Math.hypot(point.x - zone.centerX, point.y - zone.centerY) <= zone.radius;
}

/**
 * 入力と、選んでいるスロットと、出している日から、マップに描くものを決める(プレイ記録 3.3)。
 * 円の中かどうかで押せるものを絞らない。座標がダミーの間は、円の外の地点も選べる必要があるためである。
 */
export function recordMapView(
  draft: RecordDraft,
  activeSlot: Slot | null,
  day: Day,
  zones: readonly ShrinkZone[],
  points: readonly SecondShrinkPoint[]
): RecordMapView {
  const mode: PickMode = activeSlot === null || activeSlot.day !== day ? "none" : activeSlot.phase === 1 ? "zones" : "points";
  const zoneId = draft.slots[slotKey({ day, phase: 1 })];
  const pointId = draft.slots[slotKey({ day, phase: 2 })];
  const selectedZone = zones.find((zone) => zone.id === zoneId) ?? null;

  const shown = mode === "points" ? points : points.filter((point) => point.id === pointId);

  return {
    mode,
    selectedZone,
    selectedPointId: pointId,
    activeTargetId: activeSlot ? draft.slots[slotKey(activeSlot)] : null,
    rainCircles: selectedZone ? [{ x: selectedZone.centerX, y: selectedZone.centerY, radius: selectedZone.radius }] : [],
    points: shown.map((point) => ({
      point,
      emphasis: selectedZone === null || isInsideZone(point, selectedZone) ? "strong" : "dim",
    })),
  };
}
