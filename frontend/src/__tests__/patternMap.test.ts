import { describe, it, expect } from "vitest";
import type { Pattern, PatternEntry, SpawnPoint } from "@/types";
import { patternMapView } from "@/lib/patternMap";

/** 第 1 次の収縮先(収縮候補地) */
function first(day: 1 | 2, id: number, x: number, y: number): PatternEntry {
  return { day, phase: 1, zone: { id, nameJa: `候補${id}`, centerX: x, centerY: y, radius: 255 }, secondShrinkPoint: null };
}

/** 第 2 次の収縮先(第二収縮地点) */
function second(day: 1 | 2, x: number, y: number): PatternEntry {
  return { day, phase: 2, zone: null, secondShrinkPoint: { id: 9, nameJa: "地点", x, y } };
}

function pattern(entries: PatternEntry[]): Pattern {
  return { patternIndex: 2, recordCount: 1, lastRecordedAt: "2026-10-12T02:00:00Z", entries };
}

const spawn: SpawnPoint = { id: 1, nameJa: "北東の教会", x: 700, y: 200 };

describe("パターンのマップの描き方(パターン検索 3.3)", () => {
  const north = pattern([first(1, 1, 500, 150), second(1, 520, 120), first(2, 4, 800, 850)]);

  it("AC-PAT-17: 選んだパターンの 2 日分の第 1 次の円だけを出す", () => {
    const view = patternMapView(north, "all", spawn);
    expect(view.days.map((d) => d.zone?.id)).toEqual([1, 4]);
    expect(view.rainCircles).toEqual([
      { x: 500, y: 150, radius: 255 },
      { x: 800, y: 850, radius: 255 },
    ]);
  });

  it("AC-PAT-19: 「すべて」では開始地点から日の順に第 1 次の中心を線でつなぐ", () => {
    expect(patternMapView(north, "all", spawn).route).toEqual([700, 200, 500, 150, 800, 850]);
  });

  it("AC-PAT-18: 「DAY I」では 1 日目の円だけを出し、線を引かない", () => {
    const view = patternMapView(north, "1", spawn);
    expect(view.days).toHaveLength(1);
    expect(view.days[0].day).toBe(1);
    expect(view.days[0].point).toEqual({ id: 9, nameJa: "地点", x: 520, y: 120 });
    expect(view.rainCircles).toEqual([{ x: 500, y: 150, radius: 255 }]);
    expect(view.route).toEqual([]);
  });

  it("第 1 次を記録していない日は、第 2 次の菱形だけを出し、暗がりの円にしない", () => {
    const view = patternMapView(pattern([second(2, 300, 300)]), "all", spawn);
    expect(view.days).toEqual([{ day: 2, zone: null, point: { id: 9, nameJa: "地点", x: 300, y: 300 } }]);
    expect(view.rainCircles).toEqual([]);
    expect(view.route).toEqual([]);
  });

  it("パターンを選んでいないときは、何も描かない", () => {
    expect(patternMapView(null, "all", spawn)).toEqual({ days: [], route: [], rainCircles: [] });
  });
});
