"use client";

import { useState } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import ZoneOverlay from "@/components/map/ZoneOverlay";
import ConditionPanel from "@/components/sidebar/ConditionPanel";
import PatternList from "@/components/sidebar/PatternList";
import type { ShrinkZone } from "@/types";

/**
 * ダミーの収縮候補地データ（API接続前の開発用）
 * 実際の座標は後続Issueで正確な値に差し替える
 */
const DUMMY_ZONES: ShrinkZone[] = [
  { id: 1, nameJa: "候補地1", centerX: 150, centerY: 150, radius: 80 },
  { id: 2, nameJa: "候補地2", centerX: 350, centerY: 120, radius: 80 },
  { id: 3, nameJa: "候補地3", centerX: 550, centerY: 180, radius: 80 },
  { id: 4, nameJa: "候補地4", centerX: 750, centerY: 150, radius: 80 },
  { id: 5, nameJa: "候補地5", centerX: 130, centerY: 400, radius: 80 },
  { id: 6, nameJa: "候補地6", centerX: 350, centerY: 380, radius: 80 },
  { id: 7, nameJa: "候補地7", centerX: 550, centerY: 420, radius: 80 },
  { id: 8, nameJa: "候補地8", centerX: 780, centerY: 400, radius: 80 },
  { id: 9, nameJa: "候補地9", centerX: 150, centerY: 650, radius: 80 },
  { id: 10, nameJa: "候補地10", centerX: 370, centerY: 680, radius: 80 },
  { id: 11, nameJa: "候補地11", centerX: 570, centerY: 640, radius: 80 },
  { id: 12, nameJa: "候補地12", centerX: 770, centerY: 670, radius: 80 },
];

/**
 * メインページ
 * 左パネル（条件選択・パターン一覧）+ 右マップキャンバスのレイアウト
 * スマホ時はマップが全幅、パネルはボトムシート
 */
export default function HomePage() {
  const [selectedZoneId, setSelectedZoneId] = useState<number | null>(null);

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
          <ConditionPanel />
          <PatternList />

          {/* 選択中のゾーン情報 */}
          {selectedZoneId && (
            <div className="p-4 border border-yellow-700 rounded-lg bg-yellow-900/20">
              <h3 className="text-sm font-bold text-yellow-300 mb-1">
                選択中のゾーン
              </h3>
              <p className="text-sm text-gray-300">
                {DUMMY_ZONES.find((z) => z.id === selectedZoneId)?.nameJa}
              </p>
            </div>
          )}
        </aside>

        {/* マップキャンバス */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas mapImagePath={null}>
            <ZoneOverlay
              zones={DUMMY_ZONES}
              selectedZoneId={selectedZoneId}
              onZoneSelect={handleZoneSelect}
            />
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
