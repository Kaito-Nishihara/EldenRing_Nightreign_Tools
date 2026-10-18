import { describe, it, expect } from "vitest";
import { emptyDraft, type RecordDraft } from "@/lib/recordDraft";
import { isInsideZone, recordMapView } from "@/lib/recordMap";
import type { SecondShrinkPoint, ShrinkZone } from "@/types";

const north: ShrinkZone = { id: 3, nameJa: "最北", centerX: 500, centerY: 200, radius: 150 };
const south: ShrinkZone = { id: 9, nameJa: "最南", centerX: 500, centerY: 850, radius: 150 };
const inside: SecondShrinkPoint = { id: 1, nameJa: "最北", x: 520, y: 220 };
const outside: SecondShrinkPoint = { id: 2, nameJa: "最南", x: 500, y: 900 };
const zones = [north, south];
const points = [inside, outside];

function draftWith(slots: Partial<RecordDraft["slots"]>): RecordDraft {
  return { ...emptyDraft(), slots: { ...emptyDraft().slots, ...slots } };
}

describe("lib/recordMap(プレイ記録 3.3)", () => {
  it("第 1 次のスロットを選んでいると、収縮候補地を押せて、第二収縮地点は出さない", () => {
    const view = recordMapView(emptyDraft(), { day: 1, phase: 1 }, 1, zones, points);

    expect(view.mode).toBe("zones");
    expect(view.points).toEqual([]);
  });

  it("AC-REC-21: 第 2 次のスロットを選んでいると、押せるのは第二収縮地点で、収縮候補地は押せない", () => {
    const view = recordMapView(emptyDraft(), { day: 1, phase: 2 }, 1, zones, points);

    expect(view.mode).toBe("points");
    expect(view.points.map((p) => p.point.id)).toEqual([1, 2]);
  });

  it("同じ日の第 1 次を入れていれば、円の中の地点を濃く、外の地点を薄く出す", () => {
    const view = recordMapView(draftWith({ "1-1": north.id }), { day: 1, phase: 2 }, 1, zones, points);

    expect(view.points).toEqual([
      { point: inside, emphasis: "strong" },
      { point: outside, emphasis: "dim" },
    ]);
  });

  it("第 1 次を入れた日は、その円を実際の大きさで出し、外を暗がりで塗る", () => {
    const view = recordMapView(draftWith({ "1-1": north.id }), { day: 1, phase: 2 }, 1, zones, points);

    expect(view.selectedZone).toBe(north);
    expect(view.rainCircles).toEqual([{ x: 500, y: 200, radius: 150 }]);
  });

  it("出している日のものだけを描く。DAY II を出しているときは DAY I の円を出さない", () => {
    const view = recordMapView(draftWith({ "1-1": north.id, "2-2": outside.id }), { day: 2, phase: 1 }, 2, zones, points);

    expect(view.selectedZone).toBeNull();
    expect(view.rainCircles).toEqual([]);
    expect(view.points.map((p) => p.point.id)).toEqual([outside.id]);
  });

  it("どのスロットも選んでいないと、何も押せない", () => {
    expect(recordMapView(emptyDraft(), null, 1, zones, points).mode).toBe("none");
  });

  it("選んでいるスロットに入っている収縮先を返す", () => {
    expect(recordMapView(draftWith({ "1-2": inside.id }), { day: 1, phase: 2 }, 1, zones, points).activeTargetId).toBe(inside.id);
  });

  it("円の縁の上の点は、円の中とみなす", () => {
    expect(isInsideZone({ x: 650, y: 200 }, north)).toBe(true);
    expect(isInsideZone({ x: 651, y: 200 }, north)).toBe(false);
  });
});
