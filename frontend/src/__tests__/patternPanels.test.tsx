import { describe, it, expect, vi, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup, within } from "@testing-library/react";
import type { Boss, Pattern, PatternResponse } from "@/types";
import ConditionPanel from "@/components/pattern/ConditionPanel";
import PatternListPanel from "@/components/pattern/PatternListPanel";

afterEach(() => {
  cleanup();
});

const bosses: Boss[] = ["gladius", "edere", "gnostor", "maris", "libra", "fulgor", "caligo", "nameless", "harmonia", "strages"].map(
  (id) => ({ id, nameJa: `王-${id}`, weaknesses: [], isDlc: false }),
);

function pattern(patternIndex: number, recordCount: number): Pattern {
  return {
    patternIndex,
    recordCount,
    lastRecordedAt: "2026-10-12T02:00:00Z",
    entries: [{ day: 1, phase: 1, zone: { id: patternIndex, nameJa: `候補${patternIndex}`, centerX: 0, centerY: 0, radius: 255 }, secondShrinkPoint: null }],
  };
}

const response: PatternResponse = { totalRecords: 6, patterns: [pattern(1, 3), pattern(2, 2), pattern(3, 1)] };

function renderList(props: Partial<Parameters<typeof PatternListPanel>[0]> = {}) {
  const handlers = { onSelect: vi.fn(), onRetry: vi.fn() };
  render(
    <PatternListPanel
      ready
      data={response}
      loading={false}
      error={null}
      selectedPatternIndex={1}
      recordHref="/record?boss=gladius&spawn=1"
      {...handlers}
      {...props}
    />,
  );
  return handlers;
}

describe("条件のパネル(パターン検索 3.1)", () => {
  it("夜の王は初めに 6 体を出し、「ほかの 4 体を表示」で残りを出す", () => {
    render(<ConditionPanel bosses={bosses} jihenList={[]} spawnPoints={[]} bossId={null} jihenId={null} spawnPointId={null} onChange={vi.fn()} />);
    expect(screen.getAllByRole("button", { pressed: false })).toHaveLength(6);
    fireEvent.click(screen.getByRole("button", { name: "ほかの 4 体を表示" }));
    expect(screen.getAllByRole("button", { pressed: false })).toHaveLength(10);
  });

  it("夜の王を押すと、その王を選ぶ。選んだ王には aria-pressed が付く", () => {
    const onChange = vi.fn();
    render(<ConditionPanel bosses={bosses} jihenList={[]} spawnPoints={[]} bossId="edere" jihenId={null} spawnPointId={null} onChange={onChange} />);
    expect(screen.getByRole("button", { name: /王-edere/ }).getAttribute("aria-pressed")).toBe("true");
    fireEvent.click(screen.getByRole("button", { name: /王-gladius/ }));
    expect(onChange).toHaveBeenCalledWith({ bossId: "gladius" });
  });

  it("隠れる位置の夜の王を選んでいるときは、初めから全部を出す", () => {
    render(<ConditionPanel bosses={bosses} jihenList={[]} spawnPoints={[]} bossId="strages" jihenId={null} spawnPointId={null} onChange={vi.fn()} />);
    expect(screen.getByRole("button", { name: /王-strages/ })).toBeTruthy();
    expect(screen.queryByRole("button", { name: /ほかの/ })).toBeNull();
  });
});

describe("パターンの一覧のパネル(パターン検索 3.2、3.4)", () => {
  it("条件がそろっていないときは、選び方の案内を出す", () => {
    renderList({ ready: false, data: null });
    expect(screen.getByText("夜の王と開始地点を選ぶと、過去のパターンが出ます")).toBeTruthy();
  });

  it("件数とパターンの数を出し、1 番のカードを選んだ状態にする", () => {
    renderList();
    expect(screen.getByRole("status").textContent).toBe("この条件の記録 6 件、3 パターン");
    const cards = screen.getAllByRole("listitem").map((item) => within(item).getByRole("button"));
    expect(cards.map((card) => card.getAttribute("aria-pressed"))).toEqual(["true", "false", "false"]);
  });

  it("AC-PAT-11: 6 回中 3 回のカードに「6 回中 3 回」と出て、割合の帯が 50% である", () => {
    renderList();
    const card = screen.getAllByRole("listitem")[0];
    expect(within(card).getByText(/6 回中 3 回/)).toBeTruthy();
    const bar = card.querySelector<HTMLElement>("[aria-hidden] > span");
    expect(bar?.style.width).toBe("50%");
  });

  it("AC-PAT-12: 2 番目のカードを押すと、2 番のパターンを選ぶ", () => {
    const { onSelect } = renderList();
    fireEvent.click(within(screen.getAllByRole("listitem")[1]).getByRole("button"));
    expect(onSelect).toHaveBeenCalledWith(2);
  });

  it("AC-PAT-13: 記録のない条件では、空の文言と「この条件で記録する」を出す", () => {
    renderList({ data: { totalRecords: 0, patterns: [] } });
    expect(screen.getByText("この条件の記録はまだありません")).toBeTruthy();
    expect(screen.getByRole("link", { name: "この条件で記録する" }).getAttribute("href")).toBe("/record?boss=gladius&spawn=1");
  });

  it("エラーのときは知らせを出し、「もう一度読み込む」で検索し直す", () => {
    const { onRetry } = renderList({ data: null, error: "サービスが応答しません" });
    expect(screen.getByRole("alert").textContent).toContain("サービスが応答しません");
    fireEvent.click(screen.getByRole("button", { name: "もう一度読み込む" }));
    expect(onRetry).toHaveBeenCalled();
  });

  it("読み込み中は、カード 2 枚分の骨組みを出す", () => {
    const { container } = render(
      <PatternListPanel ready data={null} loading error={null} selectedPatternIndex={null} onSelect={vi.fn()} onRetry={vi.fn()} recordHref="/record" />,
    );
    expect(container.querySelectorAll(".animate-pulse")).toHaveLength(2);
  });
});
