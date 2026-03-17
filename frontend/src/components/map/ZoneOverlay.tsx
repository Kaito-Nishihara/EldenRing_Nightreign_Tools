"use client";

import dynamic from "next/dynamic";
import type { ShrinkZone } from "@/types";

/**
 * react-konvaはSSR非対応のため、dynamic importで読み込む
 */
const Circle = dynamic(
  () => import("react-konva").then((mod) => mod.Circle),
  { ssr: false }
);
const Text = dynamic(
  () => import("react-konva").then((mod) => mod.Text),
  { ssr: false }
);
const Group = dynamic(
  () => import("react-konva").then((mod) => mod.Group),
  { ssr: false }
);

/** ゾーンの表示色（通常・選択時） */
const ZONE_COLOR = {
  normal: "rgba(59, 130, 246, 0.25)",
  normalStroke: "rgba(59, 130, 246, 0.6)",
  selected: "rgba(234, 179, 8, 0.4)",
  selectedStroke: "rgba(234, 179, 8, 0.9)",
};

interface ZoneOverlayProps {
  /** 表示する収縮候補地の一覧 */
  zones: ShrinkZone[];
  /** 現在選択中のゾーンID */
  selectedZoneId?: number | null;
  /** ゾーンクリック時のコールバック */
  onZoneSelect?: (zone: ShrinkZone) => void;
}

/**
 * 収縮候補地オーバーレイコンポーネント
 * マップ上に12ゾーンを半透明の円で表示する
 */
export default function ZoneOverlay({
  zones,
  selectedZoneId,
  onZoneSelect,
}: ZoneOverlayProps) {
  return (
    <>
      {zones.map((zone) => {
        const isSelected = zone.id === selectedZoneId;

        return (
          <Group key={zone.id}>
            {/* ゾーンの円 */}
            <Circle
              x={zone.centerX}
              y={zone.centerY}
              radius={zone.radius}
              fill={isSelected ? ZONE_COLOR.selected : ZONE_COLOR.normal}
              stroke={
                isSelected ? ZONE_COLOR.selectedStroke : ZONE_COLOR.normalStroke
              }
              strokeWidth={isSelected ? 3 : 2}
              onClick={() => onZoneSelect?.(zone)}
              onTap={() => onZoneSelect?.(zone)}
              hitStrokeWidth={10}
            />
            {/* ゾーン名ラベル */}
            <Text
              x={zone.centerX - 40}
              y={zone.centerY - 8}
              width={80}
              text={zone.nameJa}
              fontSize={14}
              fontStyle={isSelected ? "bold" : "normal"}
              fill={isSelected ? "#FDE047" : "#93C5FD"}
              align="center"
              listening={false}
            />
          </Group>
        );
      })}
    </>
  );
}
