import { describe, it, expect } from "vitest";
import {
  DRAFT_VERSION,
  emptyDraft,
  fillSlot,
  firstEmptySlot,
  firstEmptySlotOfDay,
  missingRequirements,
  parseDraft,
  toRequest,
  clearAfterSave,
  type RecordDraft,
} from "@/lib/recordDraft";

/** 夜の王と開始地点を選んだ入力 */
function withConditions(): RecordDraft {
  return { ...emptyDraft(), bossId: "gladius", spawnPointId: 1 };
}

describe("lib/recordDraft のスロット(プレイ記録 3.2)", () => {
  it("AC-REC-09: 1-1 を選んで収縮候補地を入れると、1-1 に入り、次は 1-2 を選ぶ", () => {
    const { draft, next } = fillSlot(emptyDraft(), { day: 1, phase: 1 }, 4);

    expect(draft.slots["1-1"]).toBe(4);
    expect(next).toEqual({ day: 1, phase: 2 });
  });

  it("AC-REC-10: 4 つのスロットをすべて入れると、どのスロットも選んでいない状態になる", () => {
    let draft = emptyDraft();
    let next = null;
    for (const [slot, id] of [[{ day: 1, phase: 1 }, 1], [{ day: 1, phase: 2 }, 2], [{ day: 2, phase: 1 }, 3], [{ day: 2, phase: 2 }, 4]] as const) {
      ({ draft, next } = fillSlot(draft, slot, id));
    }

    expect(next).toBeNull();
    expect(Object.values(draft.slots)).toEqual([1, 2, 3, 4]);
  });

  it("入れたスロットの後ろが埋まっていれば、前に戻って空いたスロットを選ぶ", () => {
    const start = { ...emptyDraft(), slots: { "1-1": null, "1-2": 2, "2-1": 3, "2-2": null } };

    const { next } = fillSlot(start, { day: 2, phase: 2 }, 4);

    expect(next).toEqual({ day: 1, phase: 1 });
  });

  it("下書きを戻したときは、順に見て最初の空いたスロットを選ぶ", () => {
    const draft = { ...emptyDraft(), slots: { "1-1": 1, "1-2": null, "2-1": 3, "2-2": null } };

    expect(firstEmptySlot(draft)).toEqual({ day: 1, phase: 2 });
  });

  it("日を替えたときは、その日の最初の空いたスロットを選ぶ。両方入れていれば null", () => {
    const draft = { ...emptyDraft(), slots: { "1-1": 1, "1-2": 2, "2-1": 3, "2-2": null } };

    expect(firstEmptySlotOfDay(draft, 2)).toEqual({ day: 2, phase: 2 });
    expect(firstEmptySlotOfDay(draft, 1)).toBeNull();
  });
});

describe("lib/recordDraft の保存の条件(プレイ記録 3.5)", () => {
  it("AC-REC-12: スロットを 1 つも入れていないと「収縮先を 1 つ以上入れてください」で保存できない", () => {
    expect(missingRequirements(withConditions())).toEqual(["収縮先を 1 つ以上入れてください"]);
  });

  it("夜の王と開始地点を選んでいなければ、それぞれを理由に挙げる", () => {
    const draft = { ...emptyDraft(), slots: { ...emptyDraft().slots, "1-1": 4 } };

    expect(missingRequirements(draft)).toEqual(["夜の王を選んでください", "開始地点を選んでください"]);
  });

  it("備考が 500 文字を超えると、今の文字数を出して保存できない", () => {
    const draft = { ...withConditions(), note: "あ".repeat(523), slots: { ...emptyDraft().slots, "1-1": 4 } };

    expect(missingRequirements(draft)).toEqual(["500 文字までです(今 523 文字)"]);
  });

  it("第 1 次は zoneId、第 2 次は secondShrinkPointId で送り、空の備考は null にする", () => {
    const draft: RecordDraft = {
      ...withConditions(),
      day1NightBossId: 3,
      note: "  ",
      slots: { "1-1": 4, "1-2": 7, "2-1": null, "2-2": null },
    };

    expect(toRequest(draft)).toEqual({
      bossId: "gladius",
      jihenId: null,
      spawnPointId: 1,
      day1NightBossId: 3,
      day2NightBossId: null,
      note: null,
      entries: [
        { day: 1, phase: 1, zoneId: 4, secondShrinkPointId: null },
        { day: 1, phase: 2, zoneId: null, secondShrinkPointId: 7 },
      ],
    });
  });

  it("AC-REC-13: 保存の後は条件を残し、スロットと夜ボスと備考を空にする", () => {
    const draft: RecordDraft = {
      ...withConditions(),
      jihenId: "crater",
      day2NightBossId: 9,
      note: "メモ",
      slots: { "1-1": 4, "1-2": 7, "2-1": 1, "2-2": 2 },
    };

    expect(clearAfterSave(draft)).toEqual({ ...emptyDraft(), bossId: "gladius", jihenId: "crater", spawnPointId: 1 });
  });
});

describe("lib/recordDraft の下書き(プレイ記録 3.6)", () => {
  it("版 2 の下書きは、そのまま読める", () => {
    const draft = { ...withConditions(), slots: { ...emptyDraft().slots, "1-2": 7 } };

    expect(parseDraft(JSON.stringify(draft))).toEqual(draft);
  });

  it("AC-REC-22: 版 1 の下書きは読まない", () => {
    const old = { bossId: "gladius", jihenId: null, spawnPointId: 1, note: "", entries: { "1-1": 4, "1-2": 5 } };

    expect(parseDraft(JSON.stringify(old))).toBeNull();
    expect(parseDraft(JSON.stringify({ ...old, version: 1 }))).toBeNull();
  });

  it("JSON として読めない下書きと、形の違う下書きは読まない", () => {
    expect(parseDraft("{")).toBeNull();
    expect(parseDraft(JSON.stringify({ version: DRAFT_VERSION }))).toBeNull();
    expect(parseDraft(null)).toBeNull();
  });

  it("値の型が違う項目は、選んでいない状態として読む", () => {
    const raw = JSON.stringify({ version: DRAFT_VERSION, bossId: 5, spawnPointId: "1", note: 3, slots: { "1-1": "4", "1-2": 2.5 } });

    expect(parseDraft(raw)).toEqual(emptyDraft());
  });
});
