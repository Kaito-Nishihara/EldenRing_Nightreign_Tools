"use client";

import { useState, useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import MapCanvas from "@/components/map/MapCanvas";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import ShrinkOverlay from "@/components/map/ShrinkOverlay";
import SpawnPointMarkers from "@/components/map/SpawnPointMarkers";
import BasePointMarkers from "@/components/map/BasePointMarkers";
import RecordForm from "@/components/record/RecordForm";
import { useMasterData } from "@/hooks/useMasterData";
import { usePlayRecord, slotKey } from "@/hooks/usePlayRecord";
import { getShrinkZones, getBasePoints } from "@/lib/api";
import { getMapImagePath } from "@/lib/mapImage";
import type { SpawnPoint, ShrinkZone, BasePoint } from "@/types";

/**
 * プレイ記録入力ページ
 * マップ上のゾーンをタップして収縮先を入力し、APIに保存する
 */
export default function RecordPage() {
  const router = useRouter();
  const [zones, setZones] = useState<ShrinkZone[]>([]);
  const [basePoints, setBasePoints] = useState<BasePoint[]>([]);
  const [visibleDay, setVisibleDay] = useState<1 | 2>(1);
  const [showShrinkZones, setShowShrinkZones] = useState(true);

  /** 拠点タイプの割り当て状態（キー: basePointId, 値: baseTypeId） */
  const [baseAssignments, setBaseAssignments] = useState<Record<number, string | null>>({});

  // マスタデータ
  const {
    bosses,
    jihenList,
    spawnPoints,
    loading: masterLoading,
    error: masterError,
  } = useMasterData();

  // プレイ記録フック
  const {
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
  } = usePlayRecord();

  // 地変に応じたマップ画像
  const mapImagePath = getMapImagePath(form.jihenId);

  // 収縮候補地を取得
  useEffect(() => {
    getShrinkZones()
      .then(setZones)
      .catch((err) => console.error("収縮候補地の取得に失敗:", err));
  }, []);

  // 拠点候補地を取得（現在は全マップ共通で通常マップのデータを使用）
  useEffect(() => {
    getBasePoints("normal")
      .then(setBasePoints)
      .catch((err) => console.error("拠点候補地の取得に失敗:", err));
  }, []);

  // 保存成功時に3秒後にメイン画面へ遷移
  useEffect(() => {
    if (!saved) return;
    const timer = setTimeout(() => router.push("/"), 3000);
    return () => clearTimeout(timer);
  }, [saved, router]);

  useEffect(() => {
    if (activeSlot) {
      setVisibleDay(activeSlot.day);
    }
  }, [activeSlot]);

  /** 開始地点を選択 */
  const handleSpawnPointSelect = useCallback(
    (sp: SpawnPoint) => {
      if (form.spawnPointId === sp.id) {
        // 再クリックで選択解除
        setSpawnPointId(null);
      } else {
        setSpawnPointId(sp.id);
      }
    },
    [form.spawnPointId, setSpawnPointId]
  );

  /** 拠点タイプ割り当て */
  const handleBaseAssign = useCallback(
    (basePointId: number, baseTypeId: string | null) => {
      setBaseAssignments((prev) => ({
        ...prev,
        [basePointId]: baseTypeId,
      }));
    },
    []
  );

  /** ゾーンタップ: アクティブスロットに設定 */
  const handleZoneSelect = useCallback(
    (zone: ShrinkZone) => {
      if (!activeSlot) return;
      const currentZoneId = form.entries[slotKey(activeSlot.day, activeSlot.phase)];
      // 同じゾーンの再タップで取り消し
      if (currentZoneId === zone.id) {
        setEntry(activeSlot.day, activeSlot.phase, null);
      } else {
        setEntry(activeSlot.day, activeSlot.phase, zone.id);
      }
    },
    [activeSlot, form.entries, setEntry]
  );

  /** エントリ取り消し */
  const handleEntryRemove = useCallback(
    (day: 1 | 2, phase: 1 | 2) => {
      setEntry(day, phase, null);
      setActiveSlot({ day, phase });
      setVisibleDay(day);
    },
    [setEntry, setActiveSlot]
  );

  const handleSlotSelect = useCallback(
    (slot: { day: 1 | 2; phase: 1 | 2 } | null) => {
      setActiveSlot(slot);
      if (slot) {
        setVisibleDay(slot.day);
      }
    },
    [setActiveSlot]
  );

  const handleVisibleDayChange = useCallback(
    (day: 1 | 2) => {
      setVisibleDay(day);
      if (!activeSlot || activeSlot.day !== day) {
        setActiveSlot({ day, phase: 1 });
      }
    },
    [activeSlot, setActiveSlot]
  );

  /** アクティブスロットに対応するゾーンのShrinkOverlay用エントリ */
  const overlayEntry = (() => {
    if (!activeSlot) return null;
    const zoneId = form.entries[slotKey(activeSlot.day, activeSlot.phase)];
    if (zoneId === null) return null;
    const zone = zones.find((z) => z.id === zoneId);
    if (!zone) return null;
    return {
      day: activeSlot.day,
      phase: activeSlot.phase,
      zone,
    };
  })();

  /** 選択済みゾーンIDのセット（ハイライト表示用） */
  const activeZoneId = (() => {
    if (!activeSlot) return null;
    return form.entries[slotKey(activeSlot.day, activeSlot.phase)] ?? null;
  })();

  const visibleDaySelectedEntries = ([1, 2] as const)
    .map((phase) => {
      const zoneId = form.entries[slotKey(visibleDay, phase)];
      if (zoneId === null) return null;
      return {
        zoneId,
        label: `D${visibleDay}-P${phase}`,
      };
    })
    .filter((entry): entry is { zoneId: number; label: string } => entry !== null);

  return (
    <div className="h-screen flex flex-col">
      {/* ヘッダー */}
      <header className="flex-shrink-0 bg-gray-800 border-b border-gray-700 px-4 py-3 flex items-center gap-4">
        <Link
          href="/"
          className="text-gray-400 hover:text-gray-200 transition-colors text-sm"
        >
          &larr; 戻る
        </Link>
        <h1 className="text-xl font-bold text-gray-100">
          プレイ記録入力
        </h1>
      </header>

      {/* メインコンテンツ */}
      <div className="flex-1 flex flex-col md:flex-row overflow-hidden">
        {/* サイドパネル */}
        <aside className="order-2 md:order-1 md:w-80 md:flex-shrink-0 md:h-full overflow-y-auto bg-gray-900 border-t md:border-t-0 md:border-r border-gray-700 p-4">
          <RecordForm
            bosses={bosses}
            jihenList={jihenList}
            spawnPoints={spawnPoints}
            zones={zones}
            form={form}
            visibleDay={visibleDay}
            activeSlot={activeSlot}
            saving={saving}
            error={error}
            saved={saved}
            filledCount={filledCount}
            masterLoading={masterLoading}
            masterError={masterError}
            onBossSelect={setBossId}
            onJihenSelect={setJihenId}
            onSpawnPointSelect={setSpawnPointId}
            onNoteChange={setNote}
            onVisibleDayChange={handleVisibleDayChange}
            onSlotSelect={handleSlotSelect}
            onEntryRemove={handleEntryRemove}
            onSubmit={submit}
            onReset={reset}
          />
        </aside>

        {/* マップ */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0 relative">
          <div className="absolute left-3 top-3 z-10 flex gap-2">
            <button
              type="button"
              onClick={() => handleVisibleDayChange(1)}
              className={`rounded-md border px-3 py-2 text-xs font-medium backdrop-blur transition-colors ${
                visibleDay === 1
                  ? "border-blue-500 bg-blue-600/80 text-white"
                  : "border-gray-600 bg-gray-900/70 text-gray-300 hover:bg-gray-800/80"
              }`}
            >
              Day1表示
            </button>
            <button
              type="button"
              onClick={() => handleVisibleDayChange(2)}
              className={`rounded-md border px-3 py-2 text-xs font-medium backdrop-blur transition-colors ${
                visibleDay === 2
                  ? "border-blue-500 bg-blue-600/80 text-white"
                  : "border-gray-600 bg-gray-900/70 text-gray-300 hover:bg-gray-800/80"
              }`}
            >
              Day2表示
            </button>
            <button
              type="button"
              onClick={() => setShowShrinkZones((prev) => !prev)}
              className={`rounded-md border px-3 py-2 text-xs font-medium backdrop-blur transition-colors ${
                showShrinkZones
                  ? "border-cyan-500 bg-cyan-600/80 text-white"
                  : "border-gray-600 bg-gray-900/70 text-gray-300 hover:bg-gray-800/80"
              }`}
            >
              {showShrinkZones ? "収縮表示ON" : "収縮表示OFF"}
            </button>
          </div>
          <MapCanvas
            mapImagePath={mapImagePath}
            shrinkOverlay={showShrinkZones ? <ShrinkOverlay entry={overlayEntry} /> : null}
          >
            {/* 開始地点アイコン */}
            <SpawnPointMarkers
              spawnPoints={spawnPoints}
              selectedId={form.spawnPointId}
              onSelect={handleSpawnPointSelect}
            />

            {/* 開始地点選択後に拠点候補地を表示 */}
            {form.spawnPointId && (
              <BasePointMarkers
                basePoints={basePoints}
                assignments={baseAssignments}
                onAssign={handleBaseAssign}
                editable
              />
            )}

            {/* 収縮候補地ゾーン */}
            {showShrinkZones && (
              <ZoneOverlay
                zones={zones}
                selectedEntries={visibleDaySelectedEntries}
                activeZoneId={activeZoneId}
                onZoneSelect={handleZoneSelect}
              />
            )}
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
