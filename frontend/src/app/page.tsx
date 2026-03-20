"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import MapCanvas from "@/components/map/MapCanvas";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import PatternLayer from "@/components/map/PatternLayer";
import ConditionPanel from "@/components/sidebar/ConditionPanel";
import PatternList from "@/components/sidebar/PatternList";
import ShrinkOverlay from "@/components/map/ShrinkOverlay";
import { useMasterData } from "@/hooks/useMasterData";
import { usePatterns } from "@/hooks/usePatterns";
import { getShrinkZones } from "@/lib/api";
import { getMapImagePath } from "@/lib/mapImage";
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

  // パターン選択の状態（マップ⇔リスト連動）
  const [selectedPatternIndex, setSelectedPatternIndex] = useState<number | null>(null);

  // マスタデータ取得
  const { bosses, jihenList, spawnPoints, loading: masterLoading, error: masterError } = useMasterData();

  // パターン検索
  const { data: patternData, loading: patternLoading, error: patternError, search, clear } = usePatterns();

  // 地変に応じたマップ画像パス
  const mapImagePath = getMapImagePath(selectedJihenId);

  // 収縮候補地をAPI経由で取得
  useEffect(() => {
    getShrinkZones()
      .then(setZones)
      .catch((err) => {
        console.error("収縮候補地の取得に失敗:", err);
      });
  }, []);

  // ボスと開始地点が揃ったら自動検索
  useEffect(() => {
    if (selectedBossId && selectedSpawnPointId) {
      search(selectedBossId, selectedSpawnPointId, selectedJihenId ?? undefined);
    } else {
      clear();
    }
    // 検索条件変更時にパターン選択をリセット
    setSelectedPatternIndex(null);
  }, [selectedBossId, selectedSpawnPointId, selectedJihenId, search, clear]);

  // 選択中パターンの最初のエントリ（収縮暗転表示用）
  const selectedEntry = (() => {
    if (!patternData || selectedPatternIndex === null) return null;
    const pattern = patternData.patterns.find(
      (p) => p.patternIndex === selectedPatternIndex
    );
    if (!pattern || pattern.entries.length === 0) return null;
    // Day1-Phase1を優先表示
    const sorted = [...pattern.entries].sort((a, b) =>
      a.day !== b.day ? a.day - b.day : a.phase - b.phase
    );
    return sorted[0];
  })();

  /** ゾーン選択ハンドラー */
  const handleZoneSelect = (zone: ShrinkZone) => {
    setSelectedZoneId((prev) => (prev === zone.id ? null : zone.id));
  };

  return (
    <div className="h-screen flex flex-col">
      {/* ヘッダー */}
      <header className="flex-shrink-0 bg-gray-800 border-b border-gray-700 px-4 py-3 flex items-center justify-between">
        <h1 className="text-xl font-bold text-gray-100">
          Nightreign Tools
          <span className="text-sm font-normal text-gray-400 ml-2">
            マップ収縮パターン検索
          </span>
        </h1>
        <Link
          href="/record"
          className="bg-blue-600 hover:bg-blue-500 text-white text-sm px-4 py-2 rounded-md transition-colors font-medium"
        >
          記録入力
        </Link>
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
            selectedPatternIndex={selectedPatternIndex}
            onPatternSelect={setSelectedPatternIndex}
          />
        </aside>

        {/* マップキャンバス */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas
            mapImagePath={mapImagePath}
            shrinkOverlay={<ShrinkOverlay entry={selectedEntry} />}
          >
            <ZoneOverlay
              zones={zones}
              selectedZoneId={selectedZoneId}
              onZoneSelect={handleZoneSelect}
            />
            {patternData && (
              <PatternLayer
                patterns={patternData.patterns}
                selectedPatternIndex={selectedPatternIndex}
                onPatternSelect={setSelectedPatternIndex}
              />
            )}
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
