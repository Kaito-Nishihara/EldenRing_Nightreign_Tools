"use client";

import { useState, useEffect } from "react";
import type { SpawnPoint } from "@/types";
import { MAP_STYLE, THEME } from "@/lib/theme";

interface SpawnPointMarkersProps {
  /** 表示する開始地点一覧 */
  spawnPoints: SpawnPoint[];
  /** 選択中の開始地点ID */
  selectedId: number | null;
  /** 開始地点クリック時のコールバック */
  onSelect: (sp: SpawnPoint) => void;
}

/**
 * 開始地点の印(画面の共通仕様 3.8)。spawn.webp のアイコンを置く。
 * 選んだものは大きくして gold-bright の輪で囲み、ほかは薄く出す。選び直せるよう、選んだ後もほかを消さない。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function SpawnPointMarkers({ spawnPoints, selectedId, onSelect }: SpawnPointMarkersProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Text, Group, Image: KonvaImage } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  /** アイコン画像 */
  const [icon, setIcon] = useState<HTMLImageElement | null>(null);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const img = new window.Image();
    img.src = "/icons/zones/spawn.webp";
    img.onload = () => setIcon(img);
  }, []);

  return (
    <>
      {spawnPoints.map((sp) => {
        const isSelected = sp.id === selectedId;
        const dimmed = selectedId !== null && !isSelected;
        const size = isSelected ? MAP_STYLE.spawnIconSizeSelected : MAP_STYLE.spawnIconSize;

        return (
          <Group
            key={sp.id}
            opacity={dimmed ? MAP_STYLE.spawnDimmedOpacity : 1}
            onClick={() => onSelect(sp)}
            onTap={() => onSelect(sp)}
          >
            {isSelected && (
              <Circle
                x={sp.x}
                y={sp.y}
                radius={size / 2 + 4}
                fill={MAP_STYLE.zoneMarkerFill}
                stroke={THEME.goldBright}
                strokeWidth={3}
                shadowColor={THEME.gold}
                shadowBlur={12}
              />
            )}
            {icon ? (
              <KonvaImage image={icon} x={sp.x - size / 2} y={sp.y - size / 2} width={size} height={size} />
            ) : (
              <Circle x={sp.x} y={sp.y} radius={8} fill={THEME.goldBright} stroke={THEME.night} strokeWidth={2} />
            )}
            {/* 押しやすくするため、アイコンより少し広い当たりを置く */}
            <Circle x={sp.x} y={sp.y} radius={size / 2 + 6} fill={MAP_STYLE.maskClear} />
            <Text
              x={sp.x - 60}
              y={sp.y + size / 2 + 4}
              width={120}
              align="center"
              text={sp.nameJa}
              fontSize={14}
              fontStyle="bold"
              fill={isSelected ? THEME.goldBright : THEME.ash}
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
