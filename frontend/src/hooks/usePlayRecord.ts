"use client";

import { useState, useCallback, useEffect, useRef } from "react";
import type { CreatePlayRecordEntry, CreatePlayRecordRequest } from "@/types";
import { createPlayRecord } from "@/lib/api";

/** localStorage保存キー */
const STORAGE_KEY = "nightreign_play_record_draft";

/** 自動保存間隔（ミリ秒） */
const AUTO_SAVE_INTERVAL = 5000;

/** フォームの状態 */
export interface PlayRecordFormState {
  bossId: string | null;
  jihenId: string | null;
  spawnPointId: number | null;
  note: string;
  /** キー: "day-phase"（例: "1-1"）、値: zoneId */
  entries: Record<string, number | null>;
}

/** スロットキー生成（例: "1-1", "1-2", "2-1", "2-2"） */
export function slotKey(day: 1 | 2, phase: 1 | 2): string {
  return `${day}-${phase}`;
}

/** 全スロット定義 */
const ALL_SLOTS: Array<{ day: 1 | 2; phase: 1 | 2 }> = [
  { day: 1, phase: 1 },
  { day: 1, phase: 2 },
  { day: 2, phase: 1 },
  { day: 2, phase: 2 },
];

/** 初期状態 */
function createInitialState(): PlayRecordFormState {
  return {
    bossId: null,
    jihenId: null,
    spawnPointId: null,
    note: "",
    entries: {
      [slotKey(1, 1)]: null,
      [slotKey(1, 2)]: null,
      [slotKey(2, 1)]: null,
      [slotKey(2, 2)]: null,
    },
  };
}

/** フック戻り値 */
interface UsePlayRecordReturn {
  /** フォーム状態 */
  form: PlayRecordFormState;
  /** 保存中 */
  saving: boolean;
  /** エラー */
  error: string | null;
  /** 保存成功フラグ */
  saved: boolean;
  /** ボスを選択 */
  setBossId: (id: string | null) => void;
  /** 地変を選択 */
  setJihenId: (id: string | null) => void;
  /** 開始地点を選択 */
  setSpawnPointId: (id: number | null) => void;
  /** 備考を入力 */
  setNote: (note: string) => void;
  /** 収縮エントリを設定（ゾーンタップ時） */
  setEntry: (day: 1 | 2, phase: 1 | 2, zoneId: number | null) => void;
  /** 現在アクティブなスロット */
  activeSlot: { day: 1 | 2; phase: 1 | 2 } | null;
  /** アクティブスロットを切り替え */
  setActiveSlot: (slot: { day: 1 | 2; phase: 1 | 2 } | null) => void;
  /** 記録を保存 */
  submit: () => Promise<void>;
  /** フォームをリセット */
  reset: () => void;
  /** 入力済みエントリ数 */
  filledCount: number;
}

/**
 * プレイ記録入力カスタムフック
 * 状態管理・localStorage自動保存・API送信を行う
 */
export function usePlayRecord(): UsePlayRecordReturn {
  const [form, setForm] = useState<PlayRecordFormState>(createInitialState);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saved, setSaved] = useState(false);
  const [activeSlot, setActiveSlot] = useState<{
    day: 1 | 2;
    phase: 1 | 2;
  } | null>({ day: 1, phase: 1 });
  const formRef = useRef(form);
  formRef.current = form;

  /** localStorageから復元 */
  useEffect(() => {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored) {
        const parsed = JSON.parse(stored) as PlayRecordFormState;
        setForm(parsed);
      }
    } catch {
      // 破損データは無視
    }
  }, []);

  /** 5秒間隔でlocalStorageに自動保存 */
  useEffect(() => {
    const timer = setInterval(() => {
      try {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(formRef.current));
      } catch {
        // 保存失敗は無視
      }
    }, AUTO_SAVE_INTERVAL);
    return () => clearInterval(timer);
  }, []);

  const setBossId = useCallback((id: string | null) => {
    setForm((prev) => ({ ...prev, bossId: id }));
    setSaved(false);
  }, []);

  const setJihenId = useCallback((id: string | null) => {
    setForm((prev) => ({ ...prev, jihenId: id }));
    setSaved(false);
  }, []);

  const setSpawnPointId = useCallback((id: number | null) => {
    setForm((prev) => ({ ...prev, spawnPointId: id }));
    setSaved(false);
  }, []);

  const setNote = useCallback((note: string) => {
    setForm((prev) => ({ ...prev, note }));
  }, []);

  /** 収縮エントリを設定し、次の未入力スロットに自動移動 */
  const setEntry = useCallback(
    (day: 1 | 2, phase: 1 | 2, zoneId: number | null) => {
      setForm((prev) => ({
        ...prev,
        entries: { ...prev.entries, [slotKey(day, phase)]: zoneId },
      }));
      setSaved(false);

      // ゾーン設定時は次の未入力スロットへ自動移動
      if (zoneId !== null) {
        const currentIdx = ALL_SLOTS.findIndex(
          (s) => s.day === day && s.phase === phase
        );
        setForm((prev) => {
          const updated = {
            ...prev.entries,
            [slotKey(day, phase)]: zoneId,
          };
          for (let i = 1; i <= ALL_SLOTS.length; i++) {
            const next = ALL_SLOTS[(currentIdx + i) % ALL_SLOTS.length];
            if (updated[slotKey(next.day, next.phase)] === null) {
              setActiveSlot(next);
              return prev;
            }
          }
          setActiveSlot(null);
          return prev;
        });
      }
    },
    []
  );

  /** 入力済みエントリ数 */
  const filledCount = Object.values(form.entries).filter(
    (v) => v !== null
  ).length;

  /** 記録を保存 */
  const submit = useCallback(async () => {
    setError(null);

    if (!form.bossId) {
      setError("ボスを選択してください");
      return;
    }
    if (!form.spawnPointId) {
      setError("開始地点を選択してください");
      return;
    }

    const entries: CreatePlayRecordEntry[] = [];
    for (const [key, zoneId] of Object.entries(form.entries)) {
      if (zoneId !== null) {
        const [d, p] = key.split("-").map(Number);
        // 第 2 次は第二収縮地点で送る(決定の記録 0008)。この画面は次の PR で作り直す
        entries.push({ day: d as 1 | 2, phase: p as 1 | 2, zoneId: p === 1 ? zoneId : null, secondShrinkPointId: p === 2 ? zoneId : null });
      }
    }

    if (entries.length === 0) {
      setError("収縮先を最低1つ入力してください");
      return;
    }

    const request: CreatePlayRecordRequest = {
      bossId: form.bossId,
      jihenId: form.jihenId,
      spawnPointId: form.spawnPointId,
      day1NightBossId: null,
      day2NightBossId: null,
      note: form.note || null,
      entries,
    };

    setSaving(true);
    try {
      await createPlayRecord(request);
      localStorage.removeItem(STORAGE_KEY);
      setSaved(true);
    } catch (err) {
      const message =
        err instanceof Error ? err.message : "記録の保存に失敗しました";
      setError(message);
    } finally {
      setSaving(false);
    }
  }, [form]);

  /** フォームリセット */
  const reset = useCallback(() => {
    setForm(createInitialState());
    setActiveSlot({ day: 1, phase: 1 });
    setSaved(false);
    setError(null);
    localStorage.removeItem(STORAGE_KEY);
  }, []);

  return {
    form,
    saving,
    error,
    saved,
    setBossId,
    setJihenId,
    setSpawnPointId,
    setNote,
    setEntry,
    activeSlot,
    setActiveSlot,
    submit,
    reset,
    filledCount,
  };
}
