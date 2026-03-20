"use client";

import { useState, useEffect } from "react";
import type { SpawnPoint } from "@/types";

/** 開始地点アイコンのサイズ */
const ICON_SIZE = 48;

interface SpawnPointMarkersProps {
  /** 表示する開始地点一覧 */
  spawnPoints: SpawnPoint[];
  /** 選択中の開始地点ID */
  selectedId: number | null;
  /** 開始地点クリック時のコールバック */
  onSelect: (sp: SpawnPoint) => void;
}

/**
 * 開始地点マーカーコンポーネント
 * マップ上にspawn.webpアイコンを表示する
 * 選択後は選択した地点のみ表示する
 */
export default function SpawnPointMarkers({
  spawnPoints,
  selectedId,
  onSelect,
}: SpawnPointMarkersProps) {
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

  /** 選択済みの場合は選択した地点のみ表示 */
  const visiblePoints = selectedId
    ? spawnPoints.filter((sp) => sp.id === selectedId)
    : spawnPoints;

  return (
    <>
      {visiblePoints.map((sp) => {
        const isSelected = sp.id === selectedId;
        return (
          <Group
            key={sp.id}
            onClick={() => onSelect(sp)}
            onTap={() => onSelect(sp)}
          >
            {icon ? (
              <>
                <Circle
                  x={sp.x}
                  y={sp.y}
                  radius={ICON_SIZE / 2 + 2}
                  fill="rgba(0,0,0,0.5)"
                  stroke={isSelected ? "#FFD700" : "#93c5fd"}
                  strokeWidth={isSelected ? 3 : 2}
                  hitStrokeWidth={15}
                />
                <KonvaImage
                  image={icon}
                  x={sp.x - ICON_SIZE / 2}
                  y={sp.y - ICON_SIZE / 2}
                  width={ICON_SIZE}
                  height={ICON_SIZE}
                />
              </>
            ) : (
              <Circle
                x={sp.x}
                y={sp.y}
                radius={6}
                fill="#60a5fa"
                stroke="#93c5fd"
                strokeWidth={2}
                hitStrokeWidth={15}
              />
            )}
            <Text
              x={sp.x - 30}
              y={sp.y + ICON_SIZE / 2 + 4}
              text={sp.nameJa}
              fontSize={10}
              fill={isSelected ? "#FFD700" : "#bfdbfe"}
              align="center"
              width={60}
            />
          </Group>
        );
      })}
    </>
  );
}
