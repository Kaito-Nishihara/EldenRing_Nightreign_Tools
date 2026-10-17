import { describe, it, expect } from "vitest";
import type { Pattern, PatternEntry } from "@/types";
import { dayRoutes, formatJstDate, patternShare, patternTitle, shareText } from "@/lib/patterns";

/** 第 1 次の収縮先(収縮候補地) */
function first(day: 1 | 2, nameJa: string): PatternEntry {
  return { day, phase: 1, zone: { id: 1, nameJa, centerX: 0, centerY: 0, radius: 255 }, secondShrinkPoint: null };
}

/** 第 2 次の収縮先(第二収縮地点) */
function second(day: 1 | 2, nameJa: string): PatternEntry {
  return { day, phase: 2, zone: null, secondShrinkPoint: { id: 1, nameJa, x: 0, y: 0 } };
}

function pattern(entries: PatternEntry[]): Pattern {
  return { patternIndex: 1, recordCount: 3, lastRecordedAt: "2026-10-12T02:00:00Z", entries };
}

describe("パターンのカードの文言(パターン検索 3.2)", () => {
  it("題は 1 日目と 2 日目の第 1 次の収縮先を矢印でつなぐ", () => {
    expect(patternTitle(pattern([first(1, "最北"), second(1, "最北"), first(2, "最南東")]))).toBe("最北 → 最南東");
  });

  it("片方の日の第 1 次がないときは「未記録」", () => {
    expect(patternTitle(pattern([first(1, "中央西")]))).toBe("中央西 → 未記録");
  });

  it("日ごとの収縮先は、第 2 次を第二収縮地点の名前で出し、記録のない日を出さない", () => {
    expect(dayRoutes(pattern([first(1, "最北"), second(1, "最北の奥")]))).toEqual([
      { day: 1, first: "最北", second: "最北の奥" },
    ]);
  });

  it("AC-PAT-11: 6 件中 3 件のパターンは「6 回中 3 回」で、割合は 50%", () => {
    expect(shareText(3, 6)).toBe("6 回中 3 回");
    expect(patternShare(3, 6)).toBe(50);
  });

  it("記録の総数が 0 のときも割合で割らない", () => {
    expect(patternShare(0, 0)).toBe(0);
  });

  it("最後の記録の日付は日本時間で出す(UTC の 10 月 11 日 20 時は、日本時間の 10 月 12 日)", () => {
    expect(formatJstDate("2026-10-11T20:00:00Z")).toBe("2026-10-12");
  });
});
