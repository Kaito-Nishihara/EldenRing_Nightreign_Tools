"use client";

import { useState, useEffect } from "react";
import type { SpawnPoint, ShrinkZone, BasePoint } from "@/types";

/** 開始地点アイコンのサイズ */
const SPAWN_ICON_SIZE = 48;

interface AdminOverlayProps {
  /** 登録済み開始地点 */
  spawnPoints: SpawnPoint[];
  /** 登録済み収縮候補地 */
  shrinkZones: ShrinkZone[];
  /** 登録済み拠点候補地 */
  basePoints: BasePoint[];
  /** 編集モード */
  editMode: "spawn-point" | "shrink-zone" | "base-point";
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
  basePoints,
  editMode,
  clickedPosition,
  editingSpawnPointId,
  editingShrinkZoneId,
  onSpawnPointClick,
  onShrinkZoneClick,
}: AdminOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Text, Group, Image: KonvaImage } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  /** 開始地点アイコン画像 */
  const [spawnIcon, setSpawnIcon] = useState<HTMLImageElement | null>(null);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const img = new window.Image();
    img.src = "/icons/zones/spawn.webp";
    img.onload = () => setSpawnIcon(img);
  }, []);

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

      {/* 開始地点をアイコンで表示 */}
      {spawnPoints.map((sp) => {
        const isEditing = editingSpawnPointId === sp.id;
        return (
          <Group
            key={`sp-${sp.id}`}
            onClick={() => onSpawnPointClick(sp)}
            onTap={() => onSpawnPointClick(sp)}
          >
            {spawnIcon ? (
              <>
                <Circle
                  x={sp.x}
                  y={sp.y}
                  radius={SPAWN_ICON_SIZE / 2 + 2}
                  fill="rgba(0,0,0,0.5)"
                  stroke={isEditing ? "#ffffff" : "#93c5fd"}
                  strokeWidth={isEditing ? 3 : 2}
                  hitStrokeWidth={15}
                />
                <KonvaImage
                  image={spawnIcon}
                  x={sp.x - SPAWN_ICON_SIZE / 2}
                  y={sp.y - SPAWN_ICON_SIZE / 2}
                  width={SPAWN_ICON_SIZE}
                  height={SPAWN_ICON_SIZE}
                />
              </>
            ) : (
              <Circle
                x={sp.x}
                y={sp.y}
                radius={isEditing ? 8 : 6}
                fill={isEditing ? "#3b82f6" : "#60a5fa"}
                stroke={isEditing ? "#ffffff" : "#93c5fd"}
                strokeWidth={isEditing ? 3 : 2}
                hitStrokeWidth={15}
              />
            )}
            <Text
              x={sp.x - 30}
              y={sp.y + SPAWN_ICON_SIZE / 2 + 4}
              text={sp.nameJa}
              fontSize={10}
              fill={isEditing ? "#93c5fd" : "#bfdbfe"}
              align="center"
              width={60}
            />
          </Group>
        );
      })}

      {/* 拠点候補地を?マークで表示（拠点候補モード時のみ） */}
      {editMode === "base-point" && basePoints.map((bp) => (
        <Group key={`bp-${bp.id}`}>
          <Circle
            x={bp.x}
            y={bp.y}
            radius={32}
            fill={editMode === "base-point" ? "rgba(80, 70, 50, 0.9)" : "rgba(80, 70, 50, 0.5)"}
            stroke="#C8B87C"
            strokeWidth={editMode === "base-point" ? 2 : 1}
          />
          <Text
            x={bp.x - 20}
            y={bp.y - 20}
            width={40}
            height={48}
            align="center"
            verticalAlign="middle"
            text="?"
            fontSize={40}
            fontStyle="bold"
            fill="#C8B87C"
          />
        </Group>
      ))}

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
