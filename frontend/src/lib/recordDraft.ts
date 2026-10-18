import type { CreatePlayRecordRequest } from "@/types";

/**
 * プレイ記録の入力の決まり(プレイ記録 3.2、3.5、3.6)。
 * 画面の状態に頼らない計算だけを置き、フックと部品から使う。
 */

/** 日 */
export type Day = 1 | 2;

/** 段階。1 は第 1 次(収縮候補地)、2 は第 2 次(第二収縮地点) */
export type Phase = 1 | 2;

/** スロット。日と段階の組 */
export interface Slot {
  day: Day;
  phase: Phase;
}

/** スロットの鍵。例: "1-2" は DAY I の第 2 次 */
export type SlotKey = "1-1" | "1-2" | "2-1" | "2-2";

/** スロットを選ぶ順。入れたら、この順で次の空いたスロットへ移る */
export const SLOT_ORDER: readonly Slot[] = [
  { day: 1, phase: 1 },
  { day: 1, phase: 2 },
  { day: 2, phase: 1 },
  { day: 2, phase: 2 },
];

/** 備考の上限の文字数(前後の空白を除いて数える) */
export const NOTE_MAX_LENGTH = 500;

/** 下書きの版。形を変えたら上げる。版の違う下書きは戻さずに捨てる(AC-REC-22) */
export const DRAFT_VERSION = 2;

/** 下書きを置く localStorage の鍵 */
export const DRAFT_STORAGE_KEY = "nightreign_play_record_draft";

/**
 * 記録の入力。下書きにもこの形で書く。
 * スロットの値は、第 1 次なら収縮候補地の識別子、第 2 次なら第二収縮地点の識別子(決定の記録 0008)。
 */
export interface RecordDraft {
  version: typeof DRAFT_VERSION;
  bossId: string | null;
  jihenId: string | null;
  spawnPointId: number | null;
  day1NightBossId: number | null;
  day2NightBossId: number | null;
  note: string;
  slots: Record<SlotKey, number | null>;
}

export function slotKey({ day, phase }: Slot): SlotKey {
  return `${day}-${phase}` as SlotKey;
}

export function sameSlot(a: Slot | null, b: Slot | null): boolean {
  return a !== null && b !== null && a.day === b.day && a.phase === b.phase;
}

/** 何も入れていない入力 */
export function emptyDraft(): RecordDraft {
  return {
    version: DRAFT_VERSION,
    bossId: null,
    jihenId: null,
    spawnPointId: null,
    day1NightBossId: null,
    day2NightBossId: null,
    note: "",
    slots: { "1-1": null, "1-2": null, "2-1": null, "2-2": null },
  };
}

/**
 * 保存に成功した後の入力。条件(夜の王、地変、開始地点)は残し、スロットと夜ボスと備考を空にする(プレイ記録 3.5)。
 * 続けて同じ条件を記録しやすくするためである。
 */
export function clearAfterSave(draft: RecordDraft): RecordDraft {
  return { ...emptyDraft(), bossId: draft.bossId, jihenId: draft.jihenId, spawnPointId: draft.spawnPointId };
}

/** 入れたスロットの数 */
export function filledCount(draft: RecordDraft): number {
  return Object.values(draft.slots).filter((id) => id !== null).length;
}

/** SLOT_ORDER の順で最初の空いたスロット。すべて入れていれば null。下書きを戻したときに選ぶ */
export function firstEmptySlot(draft: RecordDraft): Slot | null {
  return SLOT_ORDER.find((slot) => draft.slots[slotKey(slot)] === null) ?? null;
}

/** その日のスロットのうち、最初の空いたスロット。両方入れていれば null。マップの日を替えたときに選ぶ */
export function firstEmptySlotOfDay(draft: RecordDraft, day: Day): Slot | null {
  return SLOT_ORDER.find((slot) => slot.day === day && draft.slots[slotKey(slot)] === null) ?? null;
}

/**
 * スロットに収縮先を入れ、次に選ぶスロットを返す(AC-REC-09、AC-REC-10)。
 * 次は、入れたスロットの後ろから SLOT_ORDER の順に探した、最初の空いたスロットである。空いたスロットがなければ null。
 */
export function fillSlot(draft: RecordDraft, slot: Slot, targetId: number): { draft: RecordDraft; next: Slot | null } {
  const filled: RecordDraft = { ...draft, slots: { ...draft.slots, [slotKey(slot)]: targetId } };
  const start = SLOT_ORDER.findIndex((candidate) => sameSlot(candidate, slot));
  for (let offset = 1; offset <= SLOT_ORDER.length; offset++) {
    const candidate = SLOT_ORDER[(start + offset) % SLOT_ORDER.length];
    if (filled.slots[slotKey(candidate)] === null) {
      return { draft: filled, next: candidate };
    }
  }
  return { draft: filled, next: null };
}

/**
 * 「記録を保存する」を使えない理由。空なら保存できる(プレイ記録 3.5、AC-REC-12)。
 */
export function missingRequirements(draft: RecordDraft): string[] {
  const reasons: string[] = [];
  if (draft.bossId === null) {
    reasons.push("夜の王を選んでください");
  }
  if (draft.spawnPointId === null) {
    reasons.push("開始地点を選んでください");
  }
  if (filledCount(draft) === 0) {
    reasons.push("収縮先を 1 つ以上入れてください");
  }
  const noteLength = draft.note.trim().length;
  if (noteLength > NOTE_MAX_LENGTH) {
    reasons.push(`${NOTE_MAX_LENGTH} 文字までです(今 ${noteLength} 文字)`);
  }
  return reasons;
}

/**
 * 保存の API(POST /api/v1/play-records)に送る形にする。
 * 第 1 次は zoneId だけ、第 2 次は secondShrinkPointId だけを入れる。空の備考は null にする。
 * 夜の王と開始地点を選んでいないときは null を返す。
 */
export function toRequest(draft: RecordDraft): CreatePlayRecordRequest | null {
  if (draft.bossId === null || draft.spawnPointId === null) {
    return null;
  }

  const entries = SLOT_ORDER.flatMap((slot) => {
    const id = draft.slots[slotKey(slot)];
    if (id === null) {
      return [];
    }
    return [{
      day: slot.day,
      phase: slot.phase,
      zoneId: slot.phase === 1 ? id : null,
      secondShrinkPointId: slot.phase === 2 ? id : null,
    }];
  });

  const note = draft.note.trim();
  return {
    bossId: draft.bossId,
    jihenId: draft.jihenId,
    spawnPointId: draft.spawnPointId,
    day1NightBossId: draft.day1NightBossId,
    day2NightBossId: draft.day2NightBossId,
    note: note.length > 0 ? note : null,
    entries,
  };
}

/**
 * localStorage の下書きを読む(プレイ記録 3.6)。
 * 形が読めないもの、版の違うもの(旧版の画面の version 1 を含む)は null を返す。呼んだ側で捨てる。
 */
export function parseDraft(raw: string | null): RecordDraft | null {
  if (raw === null) {
    return null;
  }

  let value: unknown;
  try {
    value = JSON.parse(raw);
  } catch {
    return null;
  }

  if (!isRecord(value) || value.version !== DRAFT_VERSION || !isRecord(value.slots)) {
    return null;
  }

  const slots = value.slots;
  const draft: RecordDraft = {
    version: DRAFT_VERSION,
    bossId: nullableString(value.bossId),
    jihenId: nullableString(value.jihenId),
    spawnPointId: nullableNumber(value.spawnPointId),
    day1NightBossId: nullableNumber(value.day1NightBossId),
    day2NightBossId: nullableNumber(value.day2NightBossId),
    note: typeof value.note === "string" ? value.note : "",
    slots: {
      "1-1": nullableNumber(slots["1-1"]),
      "1-2": nullableNumber(slots["1-2"]),
      "2-1": nullableNumber(slots["2-1"]),
      "2-2": nullableNumber(slots["2-2"]),
    },
  };
  return draft;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function nullableString(value: unknown): string | null {
  return typeof value === "string" && value.length > 0 ? value : null;
}

function nullableNumber(value: unknown): number | null {
  return typeof value === "number" && Number.isInteger(value) ? value : null;
}
