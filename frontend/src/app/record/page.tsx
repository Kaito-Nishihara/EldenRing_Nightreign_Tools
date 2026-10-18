"use client";

import { Suspense, useCallback, useEffect, useRef, useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import MapCanvas from "@/components/map/MapCanvas";
import ShrinkOverlay from "@/components/map/ShrinkOverlay";
import SpawnPointMarkers from "@/components/map/SpawnPointMarkers";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import SecondShrinkPointOverlay from "@/components/map/SecondShrinkPointOverlay";
import SlotPanel, { slotLabel } from "@/components/record/SlotPanel";
import RecordConditionPanel from "@/components/record/RecordConditionPanel";
import RecordSavePanel from "@/components/record/RecordSavePanel";
import Panel from "@/components/ui/Panel";
import BottomSheet from "@/components/ui/BottomSheet";
import SegmentedControl from "@/components/ui/SegmentedControl";
import Toast from "@/components/ui/Toast";
import { ErrorNotice, Skeleton } from "@/components/ui/StatusViews";
import { useMasterData } from "@/hooks/useMasterData";
import { usePlayRecordForm } from "@/hooks/usePlayRecordForm";
import { useRecordTargets } from "@/hooks/useRecordTargets";
import { getMapImagePath } from "@/lib/mapImage";
import { firstEmptySlotOfDay, type Day } from "@/lib/recordDraft";
import { recordMapView } from "@/lib/recordMap";
import { buttonClassName } from "@/components/ui/Button";

const DAY_OPTIONS = [
  { value: "1", label: "DAY I", display: true },
  { value: "2", label: "DAY II", display: true },
] as const;

const SHRINK_OPTIONS = [
  { value: "show", label: "収縮を表示" },
  { value: "hide", label: "隠す" },
] as const;

/**
 * プレイ記録の画面(プレイ記録 3 章)。
 * 広い幅では左に入力の枠、右にマップを並べる。狭い幅ではマップを全面に出し、入力の枠は下から引き出す。
 * 第 1 次のスロットはマップの収縮候補地、第 2 次は第二収縮地点を押して入れる。
 */
function RecordPage() {
  const masters = useMasterData();
  const targets = useRecordTargets();
  const form = usePlayRecordForm();
  const { draft, activeSlot, setActiveSlot, fillActiveSlot } = form;

  const [day, setDay] = useState<Day>(1);
  const [showShrink, setShowShrink] = useState(true);

  // 選んでいるスロットの日に合わせて、マップの日を切り替える(プレイ記録 3.3)
  useEffect(() => {
    if (activeSlot) setDay(activeSlot.day);
  }, [activeSlot]);

  // パターン検索から移ったときは、その条件を選んだ状態にする(プレイ記録 3.1)。マスタを読み終えてから 1 度だけ行う
  const searchParams = useSearchParams();
  const appliedQuery = useRef(false);
  const { setBossId, setJihenId, setSpawnPointId } = form;
  useEffect(() => {
    if (appliedQuery.current || masters.loading || masters.error) return;
    appliedQuery.current = true;
    const boss = searchParams.get("boss");
    const spawn = Number(searchParams.get("spawn"));
    const jihen = searchParams.get("jihen");
    if (boss && masters.bosses.some((b) => b.id === boss)) setBossId(boss);
    if (masters.spawnPoints.some((s) => s.id === spawn)) setSpawnPointId(spawn);
    if (jihen && masters.jihenList.some((j) => j.id === jihen)) setJihenId(jihen);
  }, [masters, searchParams, setBossId, setJihenId, setSpawnPointId]);

  /** マップの日を替える。その日の空いたスロットを選ぶ。空いていなければ第 1 次を選ぶ */
  const changeDay = useCallback(
    (next: Day) => {
      setDay(next);
      setActiveSlot(firstEmptySlotOfDay(draft, next) ?? { day: next, phase: 1 });
    },
    [draft, setActiveSlot]
  );

  const view = recordMapView(draft, activeSlot, day, targets.shrinkZones, targets.secondShrinkPoints);
  const conditionsHref = (() => {
    const params = new URLSearchParams();
    if (draft.bossId) params.set("boss", draft.bossId);
    if (draft.spawnPointId !== null) params.set("spawn", String(draft.spawnPointId));
    if (draft.jihenId) params.set("jihen", draft.jihenId);
    return `/?${params.toString()}`;
  })();

  const loadError = masters.error ?? targets.error;
  const controls = loadError ? (
    <Panel>
      <ErrorNotice message={loadError} onRetry={() => (masters.error ? window.location.reload() : targets.retry())} />
    </Panel>
  ) : masters.loading || targets.loading ? (
    <Panel aria-busy>
      <Skeleton className="mb-3 h-6 w-24" />
      <Skeleton className="h-48" />
    </Panel>
  ) : (
    <>
      <SlotPanel
        draft={draft}
        activeSlot={activeSlot}
        filledCount={form.filledCount}
        shrinkZones={targets.shrinkZones}
        secondShrinkPoints={targets.secondShrinkPoints}
        nightBosses={targets.nightBosses}
        onSelectSlot={setActiveSlot}
        onFill={fillActiveSlot}
        onNightBossChange={form.setNightBossId}
      />
      <RecordConditionPanel
        bosses={masters.bosses}
        jihenList={masters.jihenList}
        spawnPoints={masters.spawnPoints}
        bossId={draft.bossId}
        jihenId={draft.jihenId}
        spawnPointId={draft.spawnPointId}
        onChange={(patch) => {
          if (patch.bossId !== undefined) setBossId(patch.bossId);
          if (patch.jihenId !== undefined) setJihenId(patch.jihenId);
          if (patch.spawnPointId !== undefined) setSpawnPointId(patch.spawnPointId);
        }}
      />
      <RecordSavePanel
        note={draft.note}
        missing={form.missing}
        save={form.save}
        onNoteChange={form.setNote}
        onSubmit={() => void form.submit()}
        onReset={form.reset}
      />
    </>
  );

  return (
    // 狭い幅では、畳んだ入力の枠の取っ手(高さ 64px)にマップの下端が隠れないよう、下を空ける
    <div className="flex h-full flex-col gap-4 pb-16 md:flex-row md:p-4">
      <h1 className="sr-only">プレイ記録</h1>

      <section aria-label="マップ" className="flex min-h-0 flex-1 flex-col md:order-2">
        <Panel ornate className="flex min-h-0 flex-1 flex-col !p-2 md:!p-4">
          <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
            <SegmentedControl
              ariaLabel="マップに出す日"
              options={DAY_OPTIONS}
              value={String(day) as "1" | "2"}
              onChange={(value) => changeDay(value === "1" ? 1 : 2)}
              className="w-48"
            />
            <SegmentedControl
              ariaLabel="収縮の円と暗がり"
              options={SHRINK_OPTIONS}
              value={showShrink ? "show" : "hide"}
              onChange={(value) => setShowShrink(value === "show")}
            />
          </div>
          <div className="relative min-h-0 flex-1 border border-line">
            <MapCanvas
              mapImagePath={getMapImagePath(draft.jihenId)}
              shrinkOverlay={showShrink ? <ShrinkOverlay circles={view.rainCircles} /> : null}
            >
              <ZoneOverlay
                zones={targets.shrinkZones}
                selectedEntries={showShrink && view.selectedZone ? [{ zoneId: view.selectedZone.id, label: "" }] : []}
                activeZoneId={view.mode === "zones" ? view.activeTargetId : null}
                pickable={view.mode === "zones"}
                onZoneSelect={(zone) => fillActiveSlot(zone.id)}
              />
              <SecondShrinkPointOverlay
                points={view.points}
                selectedId={view.selectedPointId}
                activeId={view.mode === "points" ? view.activeTargetId : null}
                onSelect={view.mode === "points" ? (point) => fillActiveSlot(point.id) : undefined}
              />
              <SpawnPointMarkers
                spawnPoints={masters.spawnPoints}
                selectedId={draft.spawnPointId}
                onSelect={(spawn) => setSpawnPointId(spawn.id)}
              />
            </MapCanvas>
          </div>
        </Panel>
      </section>

      <aside className="md:order-1 md:w-[360px] md:flex-none md:overflow-y-auto">
        {/* マップを押す間シートを開かなくてよいよう、畳んだ取っ手にも選んでいるスロットを出す(プレイ記録 3.2) */}
        <BottomSheet handleLabel={activeSlot ? `${slotLabel(activeSlot)}を入力中` : "記録の入力"}>
          <div className="flex flex-col gap-4 p-4 md:p-0">{controls}</div>
        </BottomSheet>
      </aside>

      {form.restored && <Toast message="前回の入力を戻しました" onDismiss={form.dismissRestored} />}
      {form.save.kind === "saved" && (
        <Toast
          message="記録を保存しました"
          action={
            <Link href={conditionsHref} className={buttonClassName("ghost", "px-2 text-sm")}>
              パターンを見る
            </Link>
          }
          onDismiss={form.dismissSave}
        />
      )}
    </div>
  );
}

/** useSearchParams を使うので、Suspense で包む(Next.js の決まり) */
export default function RecordPageRoute() {
  return (
    <Suspense>
      <RecordPage />
    </Suspense>
  );
}
