import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { renderHook, act } from "@testing-library/react";
import { ApiError } from "@/lib/apiError";
import { DRAFT_STORAGE_KEY, emptyDraft } from "@/lib/recordDraft";

// 保存の API は差し替える
const createPlayRecord = vi.fn();
vi.mock("@/lib/api", () => ({ createPlayRecord: (...args: unknown[]) => createPlayRecord(...args) }));

import { DRAFT_SAVE_DELAY_MS, SAVE_FAILED_MESSAGE, usePlayRecordForm } from "@/hooks/usePlayRecordForm";

beforeEach(() => {
  createPlayRecord.mockReset();
  localStorage.clear();
  vi.useFakeTimers();
});

afterEach(() => {
  vi.useRealTimers();
});

/** 夜の王と開始地点を選び、1-1 に収縮候補地 4 を入れた状態にする */
function fillMinimum(result: { current: ReturnType<typeof usePlayRecordForm> }) {
  act(() => {
    result.current.setBossId("gladius");
    result.current.setSpawnPointId(1);
  });
  act(() => result.current.fillActiveSlot(4));
}

describe("usePlayRecordForm の下書き(プレイ記録 3.6)", () => {
  it("AC-REC-11: スロットを入れて 1 秒たつと下書きが残り、開き直すと戻って「前回の入力を戻しました」を出す", () => {
    const first = renderHook(() => usePlayRecordForm());
    fillMinimum(first.result);
    act(() => vi.advanceTimersByTime(DRAFT_SAVE_DELAY_MS));
    first.unmount();

    const second = renderHook(() => usePlayRecordForm());

    expect(second.result.current.draft.slots["1-1"]).toBe(4);
    expect(second.result.current.restored).toBe(true);
    expect(second.result.current.activeSlot).toEqual({ day: 1, phase: 2 });
  });

  it("AC-REC-22: 版 1 の下書きが残っていると、戻さずに消し、知らせを出さない", () => {
    localStorage.setItem(DRAFT_STORAGE_KEY, JSON.stringify({ bossId: "gladius", spawnPointId: 1, note: "", entries: { "1-1": 4 } }));

    const { result } = renderHook(() => usePlayRecordForm());

    expect(result.current.draft).toEqual(emptyDraft());
    expect(result.current.restored).toBe(false);
    expect(localStorage.getItem(DRAFT_STORAGE_KEY)).toBeNull();
  });
});

describe("usePlayRecordForm の保存(プレイ記録 3.5)", () => {
  it("AC-REC-13: 保存に成功すると、スロットと備考が空になり、条件は残り、下書きが消える", async () => {
    createPlayRecord.mockResolvedValue({ id: 42, createdAt: "2026-10-18T00:00:00Z" });
    const { result } = renderHook(() => usePlayRecordForm());
    fillMinimum(result);
    act(() => result.current.setNote("メモ"));
    act(() => vi.advanceTimersByTime(DRAFT_SAVE_DELAY_MS));

    await act(async () => {
      await result.current.submit();
    });

    expect(result.current.save).toEqual({ kind: "saved", id: 42 });
    expect(result.current.draft.slots["1-1"]).toBeNull();
    expect(result.current.draft.note).toBe("");
    expect(result.current.draft.bossId).toBe("gladius");
    expect(result.current.draft.spawnPointId).toBe(1);
    expect(localStorage.getItem(DRAFT_STORAGE_KEY)).toBeNull();
  });

  it("AC-REC-14: サービスに届かずに保存できないと、入力が残っていることを伝え、スロットを消さない", async () => {
    createPlayRecord.mockRejectedValue(new ApiError(502, "サーバーに接続できません。時間をおいてやり直してください"));
    const { result } = renderHook(() => usePlayRecordForm());
    fillMinimum(result);

    await act(async () => {
      await result.current.submit();
    });

    expect(result.current.save).toEqual({ kind: "failed", message: SAVE_FAILED_MESSAGE });
    expect(result.current.draft.slots["1-1"]).toBe(4);
  });

  it("入力の誤り(400)は、サービスの文言をそのまま出す", async () => {
    createPlayRecord.mockRejectedValue(new ApiError(400, "zoneId '4' は非表示のため、記録に使えません"));
    const { result } = renderHook(() => usePlayRecordForm());
    fillMinimum(result);

    await act(async () => {
      await result.current.submit();
    });

    expect(result.current.save).toEqual({ kind: "failed", message: "zoneId '4' は非表示のため、記録に使えません" });
  });

  it("AC-REC-12: スロットを 1 つも入れていなければ、保存の API を呼ばない", async () => {
    const { result } = renderHook(() => usePlayRecordForm());
    act(() => {
      result.current.setBossId("gladius");
      result.current.setSpawnPointId(1);
    });

    await act(async () => {
      await result.current.submit();
    });

    expect(createPlayRecord).not.toHaveBeenCalled();
    expect(result.current.missing).toEqual(["収縮先を 1 つ以上入れてください"]);
  });

  it("夜ボスはその日の欄に入り、保存の入力に載る", async () => {
    createPlayRecord.mockResolvedValue({ id: 1, createdAt: "2026-10-18T00:00:00Z" });
    const { result } = renderHook(() => usePlayRecordForm());
    fillMinimum(result);
    act(() => result.current.setNightBossId(2, 20));

    await act(async () => {
      await result.current.submit();
    });

    expect(createPlayRecord).toHaveBeenCalledWith(expect.objectContaining({ day1NightBossId: null, day2NightBossId: 20 }));
  });
});
