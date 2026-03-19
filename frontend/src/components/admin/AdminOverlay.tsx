"use client";

import type { SpawnPoint, ShrinkZone } from "@/types";

interface AdminOverlayProps {
  /** 登録済み開始地点 */
  spawnPoints: SpawnPoint[];
  /** 登録済み収縮候補地 */
  shrinkZones: ShrinkZone[];
  /** 編集モード */
  editMode: "spawn-point" | "shrink-zone";
  /** クリック位置 */
  clickedPosition: { x: number; y: number } | null;
  /** 編集中のSpawnPoint ID */
  editingSpawnPointId: number | null;
  /** 編集中のShrinkZone ID */
  editingShrinkZoneId: number | null;
  /** SpawnPointクリック */
  onSpawnPointClick: (sp: SpawnPoint) => void;
  /** ShrinkZoneクリック */
  onShrinkZoneClick: (sz: ShrinkZone) => void;
}

/**
 * 管理画面用マップオーバーレイ
 * 登録済みマスタデータとクリック位置を表示する
 * 親のMapCanvasがdynamic importでSSR隔離済みのため、requireで直接取得する
 */
export default function AdminOverlay({
  spawnPoints,
  shrinkZones,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  editMode,
  clickedPosition,
  editingSpawnPointId,
  editingShrinkZoneId,
  onSpawnPointClick,
  onShrinkZoneClick,
}: AdminOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Text, Group } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  return (
    <>
      {/* 収縮候補地を円で表示 */}
      {shrinkZones.map((sz) => {
        const isEditing = editingShrinkZoneId === sz.id;
        return (
          <Group
            key={`sz-${sz.id}`}
            onClick={() => onShrinkZoneClick(sz)}
            onTap={() => onShrinkZoneClick(sz)}
          >
            <Circle
              x={sz.centerX}
              y={sz.centerY}
              radius={sz.radius}
              fill={isEditing ? "rgba(34, 197, 94, 0.3)" : "rgba(34, 197, 94, 0.15)"}
              stroke={isEditing ? "#22c55e" : "#4ade80"}
              strokeWidth={isEditing ? 3 : 1.5}
              hitStrokeWidth={10}
            />
            <Text
              x={sz.centerX - 40}
              y={sz.centerY - 6}
              text={sz.nameJa}
              fontSize={11}
              fill={isEditing ? "#22c55e" : "#86efac"}
              align="center"
              width={80}
            />
          </Group>
        );
      })}

      {/* 開始地点をマーカーで表示 */}
      {spawnPoints.map((sp) => {
        const isEditing = editingSpawnPointId === sp.id;
        return (
          <Group
            key={`sp-${sp.id}`}
            onClick={() => onSpawnPointClick(sp)}
            onTap={() => onSpawnPointClick(sp)}
          >
            <Circle
              x={sp.x}
              y={sp.y}
              radius={isEditing ? 8 : 6}
              fill={isEditing ? "#3b82f6" : "#60a5fa"}
              stroke={isEditing ? "#ffffff" : "#93c5fd"}
              strokeWidth={isEditing ? 3 : 2}
              hitStrokeWidth={15}
            />
            <Text
              x={sp.x - 30}
              y={sp.y - 20}
              text={sp.nameJa}
              fontSize={10}
              fill={isEditing ? "#93c5fd" : "#bfdbfe"}
              align="center"
              width={60}
            />
          </Group>
        );
      })}

      {/* クリック位置のカーソル */}
      {clickedPosition && (
        <Group>
          <Circle
            x={clickedPosition.x}
            y={clickedPosition.y}
            radius={5}
            fill="#f59e0b"
            stroke="#ffffff"
            strokeWidth={2}
          />
          <Text
            x={clickedPosition.x + 10}
            y={clickedPosition.y - 8}
            text={`(${clickedPosition.x}, ${clickedPosition.y})`}
            fontSize={10}
            fill="#fbbf24"
          />
        </Group>
      )}
    </>
  );
}
