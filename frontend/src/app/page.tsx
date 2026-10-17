"use client";

import { Suspense, useCallback, useEffect, useMemo, useState } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import ShrinkOverlay from "@/components/map/ShrinkOverlay";
import SpawnPointMarkers from "@/components/map/SpawnPointMarkers";
import PatternRouteLayer from "@/components/map/PatternRouteLayer";
import ConditionPanel from "@/components/pattern/ConditionPanel";
import PatternListPanel from "@/components/pattern/PatternListPanel";
import MapLegend from "@/components/pattern/MapLegend";
import Panel from "@/components/ui/Panel";
import BottomSheet from "@/components/ui/BottomSheet";
import SegmentedControl from "@/components/ui/SegmentedControl";
import { ErrorNotice, Skeleton } from "@/components/ui/StatusViews";
import { useMasterData } from "@/hooks/useMasterData";
import { usePatterns } from "@/hooks/usePatterns";
import { useSearchConditions } from "@/hooks/useSearchConditions";
import { getMapImagePath } from "@/lib/mapImage";
import { DAY_FILTER_OPTIONS, patternMapView, type DayFilter } from "@/lib/patternMap";
import { getPatternColor } from "@/lib/theme";

/**
 * パターン検索の画面(パターン検索 3 章)。
 * 広い幅では左に条件とパターンの一覧、右にマップを並べる。狭い幅ではマップを全面に出し、操作は下から引き出す。
 */
function PatternSearchPage() {
  const { bosses, jihenList, spawnPoints, loading: masterLoading, error: masterError } = useMasterData();
  const { data, loading, error, search, clear } = usePatterns();

  const bossIds = useMemo(() => bosses.map((boss) => boss.id), [bosses]);
  const spawnPointIds = useMemo(() => spawnPoints.map((spawn) => spawn.id), [spawnPoints]);
  const jihenIds = useMemo(() => jihenList.map((jihen) => jihen.id), [jihenList]);
  const { bossId, spawnPointId, jihenId, update } = useSearchConditions({
    bossIds,
    spawnPointIds,
    jihenIds,
    ready: !masterLoading && !masterError,
  });

  const [selectedPatternIndex, setSelectedPatternIndex] = useState<number | null>(null);
  const [dayFilter, setDayFilter] = useState<DayFilter>("all");
  const ready = bossId !== null && spawnPointId !== null;

  /** 今の条件で検索する。条件がそろっていなければ、結果を消す */
  const runSearch = useCallback(() => {
    if (bossId && spawnPointId !== null) {
      void search(bossId, spawnPointId, jihenId ?? undefined);
    } else {
      clear();
    }
  }, [bossId, spawnPointId, jihenId, search, clear]);

  // 夜の王と開始地点がそろうと、検索のボタンを押さずに検索する(AC-PAT-10)
  useEffect(runSearch, [runSearch]);

  // 結果が替わったら、1 番のパターンを選んだ状態にする(パターン検索 3.2)
  useEffect(() => {
    setSelectedPatternIndex(data?.patterns[0]?.patternIndex ?? null);
  }, [data]);

  const selectedPattern = data?.patterns.find((p) => p.patternIndex === selectedPatternIndex) ?? null;
  const selectedSpawn = spawnPoints.find((spawn) => spawn.id === spawnPointId) ?? null;
  const view = patternMapView(selectedPattern, dayFilter, selectedSpawn);
  const patternColor = selectedPattern ? getPatternColor(selectedPattern.patternIndex - 1) : null;

  const conditionText = [
    bosses.find((boss) => boss.id === bossId)?.nameJa ?? "夜の王を選んでください",
    jihenList.find((jihen) => jihen.id === jihenId)?.nameJa ?? "地変なし",
    selectedSpawn?.nameJa ?? "開始地点を選んでください",
  ].join(" ／ ");

  const recordParams = new URLSearchParams();
  if (bossId) recordParams.set("boss", bossId);
  if (spawnPointId !== null) recordParams.set("spawn", String(spawnPointId));
  if (jihenId) recordParams.set("jihen", jihenId);
  const recordHref = `/record?${recordParams.toString()}`;

  const controls = masterError ? (
    <Panel>
      <ErrorNotice message={masterError} onRetry={() => window.location.reload()} />
    </Panel>
  ) : masterLoading ? (
    <Panel aria-busy>
      <Skeleton className="mb-3 h-6 w-24" />
      <Skeleton className="h-48" />
    </Panel>
  ) : (
    <>
      <ConditionPanel
        bosses={bosses}
        jihenList={jihenList}
        spawnPoints={spawnPoints}
        bossId={bossId}
        jihenId={jihenId}
        spawnPointId={spawnPointId}
        onChange={update}
      />
      <PatternListPanel
        ready={ready}
        data={data}
        loading={loading}
        error={error}
        selectedPatternIndex={selectedPatternIndex}
        onSelect={setSelectedPatternIndex}
        onRetry={runSearch}
        recordHref={recordHref}
      />
    </>
  );

  return (
    // 狭い幅では、畳んだ操作の枠の取っ手(高さ 64px)にマップの下端が隠れないよう、下を空ける
    <div className="flex h-full flex-col gap-4 pb-16 md:flex-row md:p-4">
      <h1 className="sr-only">パターン検索</h1>

      <section aria-label="マップ" className="flex min-h-0 flex-1 flex-col md:order-2">
        <Panel ornate className="flex min-h-0 flex-1 flex-col !p-2 md:!p-4">
          <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
            <p className="flex min-w-0 items-center gap-2">
              <span aria-hidden className="font-display text-xs uppercase tracking-display text-ash-soft">
                Limveld
              </span>
              <span className="truncate font-serif text-sm font-extrabold text-gold-bright">{conditionText}</span>
            </p>
            <SegmentedControl
              ariaLabel="マップに出す日"
              options={DAY_FILTER_OPTIONS}
              value={dayFilter}
              onChange={setDayFilter}
              className="w-56"
            />
          </div>
          <div className="relative min-h-0 flex-1 border border-line">
            <MapCanvas
              mapImagePath={getMapImagePath(jihenId)}
              shrinkOverlay={<ShrinkOverlay circles={view.rainCircles} />}
            >
              {patternColor && <PatternRouteLayer view={view} color={patternColor} />}
              <SpawnPointMarkers
                spawnPoints={spawnPoints}
                selectedId={spawnPointId}
                onSelect={(spawn) => update({ spawnPointId: spawn.id })}
              />
            </MapCanvas>
            {patternColor && <MapLegend color={patternColor} />}
          </div>
        </Panel>
      </section>

      <aside className="md:order-1 md:w-[360px] md:flex-none md:overflow-y-auto">
        <BottomSheet handleLabel="条件とパターン">
          <div className="flex flex-col gap-4 p-4 md:p-0">{controls}</div>
        </BottomSheet>
      </aside>
    </div>
  );
}

/** useSearchParams を使うので、Suspense で包む(Next.js の決まり) */
export default function HomePage() {
  return (
    <Suspense>
      <PatternSearchPage />
    </Suspense>
  );
}
