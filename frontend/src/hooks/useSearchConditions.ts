"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";

/** 検索の条件。null は選んでいない */
export interface SearchConditions {
  bossId: string | null;
  spawnPointId: number | null;
  jihenId: string | null;
}

interface UseSearchConditionsOptions {
  /** マスタの識別子。クエリの値がこの中にないときは、その条件だけを選んでいない状態にする */
  bossIds: readonly string[];
  spawnPointIds: readonly number[];
  jihenIds: readonly string[];
  /** マスタを読み終えたか。読み終えるまでクエリを読まない */
  ready: boolean;
}

const EMPTY: SearchConditions = { bossId: null, spawnPointId: null, jihenId: null };

/**
 * 選んだ条件を URL のクエリ(boss、spawn、jihen)に残す(画面の一覧と遷移 3.2)。
 * 再読み込みと共有で条件を保つ。クエリの値がマスタにないときは、エラーにせず、その条件だけを選んでいない状態にする(AC-NAV-02)。
 */
export function useSearchConditions({ bossIds, spawnPointIds, jihenIds, ready }: UseSearchConditionsOptions) {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const [conditions, setConditions] = useState<SearchConditions>(EMPTY);
  const restored = useRef(false);

  // マスタを読み終えたら、1 回だけクエリから条件を戻す
  useEffect(() => {
    if (!ready || restored.current) return;
    restored.current = true;
    const boss = searchParams.get("boss");
    const spawn = Number(searchParams.get("spawn"));
    const jihen = searchParams.get("jihen");
    setConditions({
      bossId: boss && bossIds.includes(boss) ? boss : null,
      spawnPointId: Number.isInteger(spawn) && spawnPointIds.includes(spawn) ? spawn : null,
      jihenId: jihen && jihenIds.includes(jihen) ? jihen : null,
    });
  }, [ready, searchParams, bossIds, spawnPointIds, jihenIds]);

  /** 条件を変え、URL のクエリも書き替える。履歴を増やさない */
  const update = useCallback(
    (patch: Partial<SearchConditions>) => {
      const next = { ...conditions, ...patch };
      setConditions(next);
      const params = new URLSearchParams();
      if (next.bossId) params.set("boss", next.bossId);
      if (next.spawnPointId !== null) params.set("spawn", String(next.spawnPointId));
      if (next.jihenId) params.set("jihen", next.jihenId);
      const query = params.toString();
      router.replace(query ? `${pathname}?${query}` : pathname, { scroll: false });
    },
    [conditions, router, pathname],
  );

  return useMemo(() => ({ ...conditions, update }), [conditions, update]);
}
