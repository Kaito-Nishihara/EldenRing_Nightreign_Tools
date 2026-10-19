import { describe, it, expect, vi, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup, act } from "@testing-library/react";
import SlotPanel from "@/components/record/SlotPanel";
import RecordSaveControls from "@/components/record/RecordSaveControls";
import RecordConditionSummary from "@/components/record/RecordConditionSummary";
import Toast, { TOAST_DURATION_MS } from "@/components/ui/Toast";
import { emptyDraft, type RecordDraft } from "@/lib/recordDraft";
import type { NightBoss, SecondShrinkPoint, ShrinkZone } from "@/types";

afterEach(() => {
  cleanup();
  vi.useRealTimers();
});

const zones: ShrinkZone[] = [
  { id: 3, nameJa: "最北", centerX: 500, centerY: 200, radius: 150 },
  { id: 9, nameJa: "最南", centerX: 500, centerY: 850, radius: 150 },
];
const points: SecondShrinkPoint[] = [{ id: 4, nameJa: "北の丘", x: 520, y: 220 }];
const nightBosses: NightBoss[] = [
  { id: 1, nameJa: "鈴玉狩り", day: 1 },
  { id: 20, nameJa: "古竜", day: 2 },
];

function renderSlots(draft: RecordDraft = emptyDraft(), activeSlot: { day: 1 | 2; phase: 1 | 2 } | null = { day: 1, phase: 1 }) {
  const handlers = { onSelectSlot: vi.fn(), onFill: vi.fn(), onNightBossChange: vi.fn() };
  render(
    <SlotPanel
      draft={draft}
      activeSlot={activeSlot}
      filledCount={Object.values(draft.slots).filter((id) => id !== null).length}
      shrinkZones={zones}
      secondShrinkPoints={points}
      nightBosses={nightBosses}
      {...handlers}
    />,
  );
  return handlers;
}

describe("SlotPanel(プレイ記録 3.2)", () => {
  it("入れたスロットに名前と ✓ を出し、選んでいるスロットを aria-pressed で示す", () => {
    renderSlots({ ...emptyDraft(), slots: { ...emptyDraft().slots, "1-1": 3 } }, { day: 1, phase: 2 });

    const filled = screen.getByRole("button", { name: "DAY I 第 1 次: 最北" });
    expect(filled.textContent).toContain("✓");
    expect(filled.getAttribute("aria-pressed")).toBe("false");
    expect(screen.getByRole("button", { name: "DAY I 第 2 次: 未入力" }).getAttribute("aria-pressed")).toBe("true");
    expect(screen.getByText("1 / 4")).toBeTruthy();
  });

  it("AC-REC-15: 一覧から選ぶ欄で、選んでいるスロットの収縮先を入れられる。第 2 次では第二収縮地点を並べる", () => {
    const { onFill } = renderSlots(emptyDraft(), { day: 1, phase: 2 });

    const list = screen.getByLabelText("DAY I 第 2 次の第二収縮地点");
    expect(Array.from((list as HTMLSelectElement).options).map((o) => o.text)).toEqual(["選んでください", "北の丘"]);
    fireEvent.change(list, { target: { value: "4" } });

    expect(onFill).toHaveBeenCalledWith(4);
  });

  it("夜ボスの欄には、その日の夜ボスだけを並べ、初めは「選ばない」にする", () => {
    const { onNightBossChange } = renderSlots();

    const day2 = screen.getByLabelText("DAY II の夜ボス(任意)") as HTMLSelectElement;
    expect(Array.from(day2.options).map((o) => o.text)).toEqual(["選ばない", "古竜"]);
    expect(day2.value).toBe("");
    fireEvent.change(day2, { target: { value: "20" } });

    expect(onNightBossChange).toHaveBeenCalledWith(2, 20);
  });

  it("スロットを押すと、そのスロットを選ぶ", () => {
    const { onSelectSlot } = renderSlots();

    fireEvent.click(screen.getByRole("button", { name: "DAY II 第 1 次: 未入力" }));

    expect(onSelectSlot).toHaveBeenCalledWith({ day: 2, phase: 1 });
  });
});

function renderSave(props: Partial<Parameters<typeof RecordSaveControls>[0]> = {}) {
  const handlers = { onNoteChange: vi.fn(), onSubmit: vi.fn(), onReset: vi.fn() };
  render(<RecordSaveControls note="" missing={[]} save={{ kind: "idle" }} draftSavedAt={null} {...handlers} {...props} />);
  return handlers;
}

describe("RecordSaveControls(プレイ記録 3.5)", () => {
  it("AC-REC-12: 足りないものがあると「記録を保存する」が使えず、ボタンの下に理由を出す", () => {
    renderSave({ missing: ["収縮先を 1 つ以上入れてください"] });

    expect((screen.getByRole("button", { name: "記録を保存する" }) as HTMLButtonElement).disabled).toBe(true);
    expect(screen.getByText("収縮先を 1 つ以上入れてください")).toBeTruthy();
  });

  it("保存している間は、ボタンを使えなくし、文字を「保存しています」にする", () => {
    renderSave({ save: { kind: "saving" } });

    expect((screen.getByRole("button", { name: "保存しています" }) as HTMLButtonElement).disabled).toBe(true);
  });

  it("AC-REC-14: 保存に失敗すると、その文言をエラーの知らせで出す", () => {
    renderSave({ save: { kind: "failed", message: "保存できませんでした。入力は残っています。時間をおいてもう一度保存してください" } });

    expect(screen.getByRole("alert").textContent).toContain("保存できませんでした。入力は残っています");
  });

  it("「やり直す」は、確かめの枠を出してから空にする", () => {
    const { onReset } = renderSave();

    fireEvent.click(screen.getByRole("button", { name: "やり直す" }));
    expect(screen.getByRole("dialog", { name: "入力をやり直す" })).toBeTruthy();
    expect(onReset).not.toHaveBeenCalled();
    fireEvent.click(screen.getByRole("button", { name: "空にする" }));

    expect(onReset).toHaveBeenCalledOnce();
    expect(screen.queryByRole("dialog")).toBeNull();
  });

  it("確かめの枠で「やめる」を押すと、空にせずに閉じる", () => {
    const { onReset } = renderSave();

    fireEvent.click(screen.getByRole("button", { name: "やり直す" }));
    fireEvent.click(screen.getByRole("button", { name: "やめる" }));

    expect(onReset).not.toHaveBeenCalled();
    expect(screen.queryByRole("dialog")).toBeNull();
  });

  it("下書きを残した後は、最後に残した時刻を日本時間で出す", () => {
    renderSave({ draftSavedAt: new Date("2026-10-19T12:42:00Z") });

    expect(screen.getByText("入力中の内容はこの端末に残ります。最後に残した時刻 21:42")).toBeTruthy();
  });

  it("下書きをまだ残していなければ、時刻を出さない", () => {
    renderSave();

    expect(screen.getByText("入力中の内容はこの端末に残ります。")).toBeTruthy();
  });
});

describe("RecordConditionSummary(プレイ記録 3.1)", () => {
  const props = {
    bosses: [{ id: "gladius", nameJa: "三つ首の獣", weaknesses: [], isDlc: false }],
    jihenList: [],
    spawnPoints: [{ id: 1, nameJa: "北東の教会", x: 0, y: 0 }],
    jihenId: null,
    onChange: vi.fn(),
  };

  it("夜の王と開始地点を選んだ後は、短い表示に畳み、「変える」で開く", () => {
    render(<RecordConditionSummary {...props} bossId="gladius" spawnPointId={1} />);

    expect(screen.getByText("地変なし ／ 北東の教会")).toBeTruthy();
    fireEvent.click(screen.getByRole("button", { name: "変える" }));

    expect(screen.getByLabelText("開始地点")).toBeTruthy();
  });

  it("開いた後に「条件を閉じる」を押すと、短い表示に戻る", () => {
    render(<RecordConditionSummary {...props} bossId="gladius" spawnPointId={1} />);

    fireEvent.click(screen.getByRole("button", { name: "変える" }));
    fireEvent.click(screen.getByRole("button", { name: "条件を閉じる" }));

    expect(screen.queryByLabelText("開始地点")).toBeNull();
    expect(screen.getByText("地変なし ／ 北東の教会")).toBeTruthy();
  });

  it("選び終えていなければ、選ぶ欄を開いたまま出す", () => {
    render(<RecordConditionSummary {...props} bossId={null} spawnPointId={null} />);

    expect(screen.getByLabelText("開始地点")).toBeTruthy();
    expect(screen.queryByRole("button", { name: "条件を閉じる" })).toBeNull();
  });
});

describe("Toast(画面の共通仕様 3.9)", () => {
  it("知らせを role=status で出し、4 秒で消す", () => {
    vi.useFakeTimers();
    const onDismiss = vi.fn();
    render(<Toast message="記録を保存しました" onDismiss={onDismiss} />);

    expect(screen.getByRole("status").textContent).toContain("記録を保存しました");
    act(() => vi.advanceTimersByTime(TOAST_DURATION_MS));

    expect(onDismiss).toHaveBeenCalledOnce();
  });
});
