"use client";

import type { ShrinkZone } from "@/types";

/** 候補マーカーのサイズ */
const CANDIDATE_MARKER_RADIUS = 18;

/** ゾーンの表示色 */
const ZONE_COLOR = {
  candidateFill: "rgba(17, 24, 39, 0.75)",
  candidateStroke: "rgba(148, 163, 184, 0.95)",
  candidateText: "#E2E8F0",
  filled: "rgba(34, 197, 94, 0.32)",
  filledStroke: "rgba(34, 197, 94, 0.95)",
  active: "rgba(234, 179, 8, 0.42)",
  activeStroke: "rgba(234, 179, 8, 0.95)",
};

interface SelectedZoneEntry {
  zoneId: number;
  label: string;
}

interface ZoneOverlayProps {
  /** 表示する収縮候補地の一覧 */
  zones: ShrinkZone[];
  /** 現在表示中の日で選択済みのゾーン */
  selectedEntries?: SelectedZoneEntry[];
  /** 現在アクティブなゾーンID */
  activeZoneId?: number | null;
  /** ゾーンクリック時のコールバック */
  onZoneSelect?: (zone: ShrinkZone) => void;
}

/**
 * 収縮候補地オーバーレイコンポーネント
 * マップ上に12ゾーンを半透明の円で表示する
 * 親のMapCanvasがdynamic importでSSR隔離済みのため、requireで直接取得する
 */
export default function ZoneOverlay({
  zones,
  selectedEntries = [],
  activeZoneId,
  onZoneSelect,
}: ZoneOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Text, Group } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  return (
    <>
      {zones.map((zone: ShrinkZone) => {
        const selectedEntry = selectedEntries.find((entry) => entry.zoneId === zone.id);
        const isFilled = Boolean(selectedEntry);
        const isActive = zone.id === activeZoneId;

        return (
          <Group key={zone.id}>
            {selectedEntry && (
              <>
                <Circle
                  x={zone.centerX}
                  y={zone.centerY}
                  radius={zone.radius}
                  fill={isActive ? ZONE_COLOR.active : ZONE_COLOR.filled}
                  stroke={isActive ? ZONE_COLOR.activeStroke : ZONE_COLOR.filledStroke}
                  strokeWidth={isActive ? 3 : 2.5}
                  listening={false}
                />
                <Text
                  x={zone.centerX - 44}
                  y={zone.centerY - 22}
                  width={88}
                  text={`${selectedEntry.label}\n${zone.nameJa}`}
                  fontSize={13}
                  fontStyle={isActive ? "bold" : "normal"}
                  fill={isActive ? "#FDE047" : "#86EFAC"}
                  align="center"
                  listening={false}
                />
              </>
            )}

            <Circle
              x={zone.centerX}
              y={zone.centerY}
              radius={CANDIDATE_MARKER_RADIUS}
              fill={ZONE_COLOR.candidateFill}
              stroke={isActive ? ZONE_COLOR.activeStroke : ZONE_COLOR.candidateStroke}
              strokeWidth={isActive ? 3 : 2}
              onClick={() => onZoneSelect?.(zone)}
              onTap={() => onZoneSelect?.(zone)}
              hitStrokeWidth={18}
            />
            <Circle
              x={zone.centerX}
              y={zone.centerY}
              radius={5}
              fill={isFilled ? (isActive ? "#FDE047" : "#86EFAC") : ZONE_COLOR.candidateText}
              listening={false}
            />
            <Text
              x={zone.centerX - 18}
              y={zone.centerY - 9}
              width={36}
              text={isFilled ? "" : "?"}
              fontSize={20}
              fontStyle="bold"
              fill={ZONE_COLOR.candidateText}
              align="center"
              listening={false}
            />
          </Group>
        );
      })}
    </>
  );
}
