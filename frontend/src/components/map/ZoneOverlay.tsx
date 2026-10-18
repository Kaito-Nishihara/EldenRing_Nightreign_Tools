"use client";

import type { ShrinkZone } from "@/types";
import { MAP_STYLE, THEME } from "@/lib/theme";

interface SelectedZoneEntry {
  zoneId: number;
  label: string;
}

interface ZoneOverlayProps {
  /** 表示する収縮候補地の一覧 */
  zones: ShrinkZone[];
  /** 今表示している日で選んだ収縮先。実際の大きさの円で出す */
  selectedEntries?: SelectedZoneEntry[];
  /** 今入力しているスロットの収縮先 */
  activeZoneId?: number | null;
  /** 中心の印を押したときの動き */
  onZoneSelect?: (zone: ShrinkZone) => void;
}

/**
 * 収縮候補地(画面の共通仕様 3.8)。
 * 12 の円をすべて実際の大きさで描くと重なって押せないので、中心の丸の印と名前で出し、選んだものだけを円で描く。
 * 円の中は動ける場所なので塗らない(外の暗がりは ShrinkOverlay が塗る)。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function ZoneOverlay({ zones, selectedEntries = [], activeZoneId, onZoneSelect }: ZoneOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Text, Group } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const markerRadius = MAP_STYLE.zoneMarkerRadius;

  return (
    <>
      {zones.map((zone) => {
        const selectedEntry = selectedEntries.find((entry) => entry.zoneId === zone.id);
        const isSelected = Boolean(selectedEntry);
        const isActive = zone.id === activeZoneId;
        const label = selectedEntry ? `${selectedEntry.label} ${zone.nameJa}` : zone.nameJa;

        return (
          <Group key={zone.id}>
            {isSelected && (
              <Circle
                x={zone.centerX}
                y={zone.centerY}
                radius={zone.radius}
                stroke={THEME.tide}
                strokeWidth={MAP_STYLE.zoneStrokeWidth}
                listening={false}
              />
            )}

            {/* 中心の印。選んだものは tide で塗り、今入力しているスロットの収縮先は azure-bright の線にする */}
            <Circle
              x={zone.centerX}
              y={zone.centerY}
              radius={markerRadius}
              fill={isSelected ? THEME.tide : MAP_STYLE.zoneMarkerFill}
              stroke={isActive ? THEME.azureBright : THEME.tide}
              strokeWidth={MAP_STYLE.zoneStrokeWidth}
              hitStrokeWidth={18}
              onClick={() => onZoneSelect?.(zone)}
              onTap={() => onZoneSelect?.(zone)}
            />

            <Text
              x={zone.centerX - 60}
              y={zone.centerY - markerRadius - 24}
              width={120}
              align="center"
              text={label}
              fontSize={16}
              fontStyle="bold"
              fill={isActive ? THEME.azureBright : THEME.ash}
              stroke={THEME.night}
              strokeWidth={MAP_STYLE.labelStrokeWidth}
              fillAfterStrokeEnabled
              listening={false}
            />
          </Group>
        );
      })}
    </>
  );
}
