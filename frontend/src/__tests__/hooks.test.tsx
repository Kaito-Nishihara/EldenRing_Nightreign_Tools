import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import type { PatternResponse } from "@/types";

// API と URL の仕組みは差し替える
const getPatterns = vi.fn();
vi.mock("@/lib/api", () => ({ getPatterns: (...args: unknown[]) => getPatterns(...args) }));

const replace = vi.fn();
let query = "";
vi.mock("next/navigation", () => ({
  useRouter: () => ({ replace }),
  usePathname: () => "/",
  useSearchParams: () => new URLSearchParams(query),
}));

import { usePatterns } from "@/hooks/usePatterns";
import { useSearchConditions } from "@/hooks/useSearchConditions";

/** 後から解決できる Promise */
function deferred<T>() {
  let resolve!: (value: T) => void;
  const promise = new Promise<T>((r) => (resolve = r));
  return { promise, resolve };
}

function response(totalRecords: number): PatternResponse {
  return { totalRecords, patterns: [] };
}

beforeEach(() => {
  getPatterns.mockReset();
  replace.mockReset();
  query = "";
});

describe("usePatterns", () => {
  it("AC-PAT-14: 条件を素早く 2 回変えたとき、最後に選んだ条件の結果だけが出る", async () => {
    const slow = deferred<PatternResponse>();
    const fast = deferred<PatternResponse>();
    getPatterns.mockReturnValueOnce(slow.promise).mockReturnValueOnce(fast.promise);
    const { result } = renderHook(() => usePatterns());

    act(() => {
      void result.current.search("gladius", 1);
      void result.current.search("edere", 1);
    });
    await act(async () => {
      fast.resolve(response(2));
      slow.resolve(response(9));
    });

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.data?.totalRecords).toBe(2);
  });
});

describe("useSearchConditions(画面の一覧と遷移 3.2)", () => {
  const masters = { bossIds: ["gladius", "edere"], spawnPointIds: [1, 2], jihenIds: ["crater"], ready: true };

  it("AC-NAV-01: クエリの条件を戻す", async () => {
    query = "boss=gladius&spawn=1&jihen=crater";
    const { result } = renderHook(() => useSearchConditions(masters));

    await waitFor(() => expect(result.current.bossId).toBe("gladius"));
    expect(result.current.spawnPointId).toBe(1);
    expect(result.current.jihenId).toBe("crater");
  });

  it("AC-NAV-02: マスタにない値は、その条件だけを選んでいない状態にする", async () => {
    query = "boss=unknown&spawn=1";
    const { result } = renderHook(() => useSearchConditions(masters));

    await waitFor(() => expect(result.current.spawnPointId).toBe(1));
    expect(result.current.bossId).toBeNull();
  });

  it("条件を変えると、URL のクエリを履歴を増やさずに書き替える", async () => {
    const { result } = renderHook(() => useSearchConditions(masters));

    act(() => result.current.update({ bossId: "edere", spawnPointId: 2 }));

    expect(replace).toHaveBeenCalledWith("/?boss=edere&spawn=2", { scroll: false });
    expect(result.current.bossId).toBe("edere");
  });
});
