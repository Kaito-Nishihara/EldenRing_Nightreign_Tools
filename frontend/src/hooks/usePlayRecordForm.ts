"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { createPlayRecord } from "@/lib/api";
import { ApiError } from "@/lib/apiError";
import {
  DRAFT_STORAGE_KEY,
  clearAfterSave,
  emptyDraft,
  fillSlot,
  filledCount,
  firstEmptySlot,
  missingRequirements,
  parseDraft,
  slotKey,
  toRequest,
  type Day,
  type RecordDraft,
  type Slot,
} from "@/lib/recordDraft";

/** 下書きを書くまで待つ時間。入力が止まってから書く(プレイ記録 3.6) */
export const DRAFT_SAVE_DELAY_MS = 1000;

/** 保存に失敗したときの文言。入力は消さない(AC-REC-14) */
export const SAVE_FAILED_MESSAGE = "保存できませんでした。入力は残っています。時間をおいてもう一度保存してください";

/** 保存の状態 */
export type SaveState =
  | { kind: "idle" }
  | { kind: "saving" }
  | { kind: "saved"; id: number }
  | { kind: "failed"; message: string };

/**
 * プレイ記録の入力の状態(プレイ記録 3.2、3.5、3.6)。
 * 入力は localStorage に下書きとして残し、画面を開いたときに戻す。保存の API の呼び出しもここで行う。
 */
export function usePlayRecordForm() {
  const [draft, setDraft] = useState<RecordDraft>(emptyDraft);
  const [activeSlot, setActiveSlot] = useState<Slot | null>({ day: 1, phase: 1 });
  const [restored, setRestored] = useState(false);
  const [save, setSave] = useState<SaveState>({ kind: "idle" });
  // 下書きを読み終えるまで書かない。読む前の空の入力で、残っていた下書きを上書きしないためである
  const loaded = useRef(false);

  // 画面を開いたときに下書きを戻す。読めない下書きと版の違う下書きは捨てる(AC-REC-11、AC-REC-22)
  useEffect(() => {
    const raw = readStorage();
    const stored = parseDraft(raw);
    if (stored !== null) {
      setDraft(stored);
      setActiveSlot(firstEmptySlot(stored));
      setRestored(filledCount(stored) > 0 || stored.note.length > 0);
    } else if (raw !== null) {
      removeStorage();
    }
    loaded.current = true;
  }, []);

  // 入力が止まってから 1 秒後に下書きを書く
  useEffect(() => {
    if (!loaded.current) {
      return;
    }
    const timer = setTimeout(() => writeStorage(draft), DRAFT_SAVE_DELAY_MS);
    return () => clearTimeout(timer);
  }, [draft]);

  const update = useCallback((change: (previous: RecordDraft) => RecordDraft) => {
    setDraft(change);
    setSave((previous) => (previous.kind === "saving" ? previous : { kind: "idle" }));
  }, []);

  const setBossId = useCallback((bossId: string | null) => update((d) => ({ ...d, bossId })), [update]);
  const setJihenId = useCallback((jihenId: string | null) => update((d) => ({ ...d, jihenId })), [update]);
  const setSpawnPointId = useCallback((spawnPointId: number | null) => update((d) => ({ ...d, spawnPointId })), [update]);
  const setNote = useCallback((note: string) => update((d) => ({ ...d, note })), [update]);

  const setNightBossId = useCallback(
    (day: Day, id: number | null) =>
      update((d) => (day === 1 ? { ...d, day1NightBossId: id } : { ...d, day2NightBossId: id })),
    [update]
  );

  /**
   * 選んでいるスロットに収縮先を入れ、次の空いたスロットを選ぶ(AC-REC-09、AC-REC-10)。
   * 第 1 次なら収縮候補地、第 2 次なら第二収縮地点の識別子を渡す。どちらを渡すかは呼んだ側が段階で決める(AC-REC-21)。
   */
  const fillActiveSlot = useCallback(
    (targetId: number) => {
      if (activeSlot === null) {
        return;
      }
      const result = fillSlot(draft, activeSlot, targetId);
      update(() => result.draft);
      setActiveSlot(result.next);
    },
    [activeSlot, draft, update]
  );

  /** スロットを空にして選び直す */
  const clearSlot = useCallback(
    (slot: Slot) => {
      update((d) => ({ ...d, slots: { ...d.slots, [slotKey(slot)]: null } }));
      setActiveSlot(slot);
    },
    [update]
  );

  /** 「やり直す」。条件は残し、スロットと夜ボスと備考を空にする(プレイ記録 3.5) */
  const reset = useCallback(() => {
    update(clearAfterSave);
    setActiveSlot({ day: 1, phase: 1 });
  }, [update]);

  /** 保存する。成功したら下書きを消し、条件だけを残す(AC-REC-13)。失敗したら入力を残す(AC-REC-14) */
  const submit = useCallback(async () => {
    const request = toRequest(draft);
    if (request === null || missingRequirements(draft).length > 0) {
      return;
    }

    setSave({ kind: "saving" });
    try {
      const created = await createPlayRecord(request);
      removeStorage();
      setDraft(clearAfterSave(draft));
      setActiveSlot({ day: 1, phase: 1 });
      setRestored(false);
      setSave({ kind: "saved", id: created.id });
    } catch (error) {
      setSave({ kind: "failed", message: saveFailedMessage(error) });
    }
  }, [draft]);

  // 知らせの部品は閉じる処理が替わると時間を数え直すので、同じ関数を返す
  const dismissRestored = useCallback(() => setRestored(false), []);

  /** 保存の結果の知らせを閉じる */
  const dismissSave = useCallback(() => setSave((previous) => (previous.kind === "saving" ? previous : { kind: "idle" })), []);

  return {
    draft,
    activeSlot,
    setActiveSlot,
    restored,
    dismissRestored,
    save,
    dismissSave,
    filledCount: filledCount(draft),
    missing: missingRequirements(draft),
    setBossId,
    setJihenId,
    setSpawnPointId,
    setNightBossId,
    setNote,
    fillActiveSlot,
    clearSlot,
    reset,
    submit,
  };
}

/**
 * 保存の失敗の文言。入力の誤り(400、404)は、どこが誤りかをサービスの文言で出す。
 * それ以外(つながらない、サービスが止まっている)は、入力が残っていることを伝える(AC-REC-14)。
 */
function saveFailedMessage(error: unknown): string {
  if (error instanceof ApiError && (error.status === 400 || error.status === 404)) {
    return error.message;
  }
  return SAVE_FAILED_MESSAGE;
}

// localStorage は、使えない設定(プライベートの窓など)では例外を投げる。下書きは補助なので、使えなければ黙って諦める

function readStorage(): string | null {
  try {
    return localStorage.getItem(DRAFT_STORAGE_KEY);
  } catch {
    return null;
  }
}

function writeStorage(draft: RecordDraft): void {
  try {
    localStorage.setItem(DRAFT_STORAGE_KEY, JSON.stringify(draft));
  } catch {
    // 書けなくても入力は画面に残っている
  }
}

function removeStorage(): void {
  try {
    localStorage.removeItem(DRAFT_STORAGE_KEY);
  } catch {
    // 消せなくても、次に開いたときの下書きが 1 つ残るだけである
  }
}
