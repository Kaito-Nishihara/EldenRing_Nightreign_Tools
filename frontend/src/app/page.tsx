"use client";

import { useState, useEffect } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import ConditionPanel from "@/components/sidebar/ConditionPanel";
import PatternList from "@/components/sidebar/PatternList";
import { useMasterData } from "@/hooks/useMasterData";
import { usePatterns } from "@/hooks/usePatterns";
import { getShrinkZones } from "@/lib/api";
import type { ShrinkZone } from "@/types";

/**
 * メインページ
 * 左パネル（条件選択・パターン一覧）+ 右マップキャンバスのレイアウト
 * スマホ時はマップが全幅、パネルはボトムシート
 */
export default function HomePage() {
  const [selectedZoneId, setSelectedZoneId] = useState<number | null>(null);
  const [zones, setZones] = useState<ShrinkZone[]>([]);

  // 条件選択の状態
  const [selectedBossId, setSelectedBossId] = useState<string | null>(null);
  const [selectedJihenId, setSelectedJihenId] = useState<string | null>(null);
  const [selectedSpawnPointId, setSelectedSpawnPointId] = useState<number | null>(null);

  // マスタデータ取得
  const { bosses, jihenList, spawnPoints, loading: masterLoading, error: masterError } = useMasterData();

  // パターン検索
  const { data: patternData, loading: patternLoading, error: patternError, search, clear } = usePatterns();

  // 収縮候補地をAPI経由で取得
  useEffect(() => {
    getShrinkZones()
      .then(setZones)
      .catch(() => {
        // API未接続時はフォールバック表示なし
      });
  }, []);

  // ボスと開始地点が揃ったら自動検索
  useEffect(() => {
    if (selectedBossId && selectedSpawnPointId) {
      search(selectedBossId, selectedSpawnPointId, selectedJihenId ?? undefined);
    } else {
      clear();
    }
  }, [selectedBossId, selectedSpawnPointId, selectedJihenId, search, clear]);

  /** ゾーン選択ハンドラー */
  const handleZoneSelect = (zone: ShrinkZone) => {
    setSelectedZoneId((prev) => (prev === zone.id ? null : zone.id));
  };

  return (
    <div className="h-screen flex flex-col">
      {/* ヘッダー */}
      <header className="flex-shrink-0 bg-gray-800 border-b border-gray-700 px-4 py-3">
        <h1 className="text-xl font-bold text-gray-100">
          Nightreign Tools
          <span className="text-sm font-normal text-gray-400 ml-2">
            マップ収縮パターン検索
          </span>
        </h1>
      </header>

      {/* メインコンテンツ */}
      <div className="flex-1 flex flex-col md:flex-row overflow-hidden">
        {/* サイドパネル（PC: 左カラム / SP: ボトムシート） */}
        <aside className="order-2 md:order-1 md:w-80 md:flex-shrink-0 md:h-full overflow-y-auto bg-gray-900 border-t md:border-t-0 md:border-r border-gray-700 p-4 space-y-4">
          <ConditionPanel
            bosses={bosses}
            jihenList={jihenList}
            spawnPoints={spawnPoints}
            selectedBossId={selectedBossId}
            selectedJihenId={selectedJihenId}
            selectedSpawnPointId={selectedSpawnPointId}
            onBossSelect={setSelectedBossId}
            onJihenSelect={setSelectedJihenId}
            onSpawnPointSelect={setSelectedSpawnPointId}
            loading={masterLoading}
            error={masterError}
          />
          <PatternList
            data={patternData}
            loading={patternLoading}
            error={patternError}
          />
        </aside>

        {/* マップキャンバス */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas mapImagePath={null}>
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
