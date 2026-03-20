"use client";

import { useState, useEffect, useCallback } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import MapCanvas from "@/components/map/MapCanvas";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import ShrinkOverlay from "@/components/map/ShrinkOverlay";
import RecordForm from "@/components/record/RecordForm";
import { useMasterData } from "@/hooks/useMasterData";
import { usePlayRecord, slotKey } from "@/hooks/usePlayRecord";
import { getShrinkZones } from "@/lib/api";
import { getMapImagePath } from "@/lib/mapImage";
import type { ShrinkZone } from "@/types";

/**
 * プレイ記録入力ページ
 * マップ上のゾーンをタップして収縮先を入力し、APIに保存する
 */
export default function RecordPage() {
  const router = useRouter();
  const [zones, setZones] = useState<ShrinkZone[]>([]);

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

  // 保存成功時に3秒後にメイン画面へ遷移
  useEffect(() => {
    if (!saved) return;
    const timer = setTimeout(() => router.push("/"), 3000);
    return () => clearTimeout(timer);
  }, [saved, router]);

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
    },
    [setEntry, setActiveSlot]
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
  const selectedZoneId = (() => {
    if (!activeSlot) return null;
    return form.entries[slotKey(activeSlot.day, activeSlot.phase)] ?? null;
  })();

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
            onSlotSelect={setActiveSlot}
            onEntryRemove={handleEntryRemove}
            onSubmit={submit}
            onReset={reset}
          />
        </aside>

        {/* マップ */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas
            mapImagePath={mapImagePath}
            shrinkOverlay={<ShrinkOverlay entry={overlayEntry} />}
          >
            <ZoneOverlay
              zones={zones}
              selectedZoneId={selectedZoneId}
              onZoneSelect={handleZoneSelect}
            />
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
