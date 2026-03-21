"use client";

import { useState, useEffect } from "react";
import type { SpawnPoint, ShrinkZone, BasePoint, SecondShrinkPoint } from "@/types";

/** 開始地点アイコンのサイズ */
const SPAWN_ICON_SIZE = 48;

interface AdminOverlayProps {
  /** 登録済み開始地点 */
  spawnPoints: SpawnPoint[];
  /** 登録済み収縮候補地 */
  shrinkZones: ShrinkZone[];
  /** 登録済み拠点候補地 */
  basePoints: BasePoint[];
  /** 登録済み第二収縮地点候補 */
  secondShrinkPoints: SecondShrinkPoint[];
  /** 編集モード */
  editMode: "spawn-point" | "shrink-zone" | "second-shrink-point" | "base-point";
  /** クリック位置 */
  clickedPosition: { x: number; y: number } | null;
  /** 編集中のSpawnPoint ID */
  editingSpawnPointId: number | null;
  /** 編集中のShrinkZone ID */
  editingShrinkZoneId: number | null;
  /** 編集中の第二収縮地点候補ID */
  editingSecondShrinkPointId: number | null;
  /** 収縮候補地のプレビュー半径 */
  shrinkZoneRadius: number;
  /** SpawnPointクリック */
  onSpawnPointClick: (sp: SpawnPoint) => void;
  /** ShrinkZoneクリック */
  onShrinkZoneClick: (sz: ShrinkZone) => void;
  /** 第二収縮地点候補クリック */
  onSecondShrinkPointClick: (point: SecondShrinkPoint) => void;
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
  secondShrinkPoints,
  editMode,
  clickedPosition,
  editingSpawnPointId,
  editingShrinkZoneId,
  editingSecondShrinkPointId,
  shrinkZoneRadius,
  onSpawnPointClick,
  onShrinkZoneClick,
  onSecondShrinkPointClick,
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
      {/* 収縮候補地は選択中または新規作成プレビューのみ表示 */}
      {(() => {
        const selectedZone = shrinkZones.find((sz) => sz.id === editingShrinkZoneId) ?? null;
        const previewPosition =
          clickedPosition ??
          (selectedZone
            ? { x: selectedZone.centerX, y: selectedZone.centerY }
            : null);

        if (editMode !== "shrink-zone" || !previewPosition) {
          return null;
        }

        return (
          <Group
            key={selectedZone ? `sz-${selectedZone.id}` : "shrink-zone-preview"}
            onClick={selectedZone ? () => onShrinkZoneClick(selectedZone) : undefined}
            onTap={selectedZone ? () => onShrinkZoneClick(selectedZone) : undefined}
          >
            <Circle
              x={previewPosition.x}
              y={previewPosition.y}
              radius={shrinkZoneRadius}
              fill={selectedZone ? "rgba(34, 197, 94, 0.3)" : "rgba(34, 197, 94, 0.22)"}
              stroke={selectedZone ? "#22c55e" : "#4ade80"}
              strokeWidth={selectedZone ? 3 : 2}
            />
            <Text
              x={previewPosition.x - 60}
              y={previewPosition.y - 6}
              text={selectedZone ? `${selectedZone.nameJa} r=${shrinkZoneRadius}` : `新規候補 r=${shrinkZoneRadius}`}
              fontSize={11}
              fill="#86efac"
              align="center"
              width={120}
            />
          </Group>
        );
      })()}

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

      {/* 第二収縮地点候補をマーカーで表示 */}
      {secondShrinkPoints.map((point) => {
        const isEditing = editingSecondShrinkPointId === point.id;
        return (
          <Group
            key={`ssp-${point.id}`}
            onClick={() => onSecondShrinkPointClick(point)}
            onTap={() => onSecondShrinkPointClick(point)}
          >
            <Circle
              x={point.x}
              y={point.y}
              radius={20}
              fill="rgba(88, 28, 135, 0.82)"
              stroke={isEditing ? "#f5d0fe" : "#e879f9"}
              strokeWidth={isEditing ? 3 : 2}
              hitStrokeWidth={15}
            />
            <Text
              x={point.x - 16}
              y={point.y - 11}
              width={32}
              text="II"
              fontSize={16}
              fontStyle="bold"
              fill="#fdf4ff"
              align="center"
              listening={false}
            />
            <Text
              x={point.x - 48}
              y={point.y + 24}
              width={96}
              text={point.nameJa}
              fontSize={10}
              fill={isEditing ? "#f5d0fe" : "#f0abfc"}
              align="center"
              listening={false}
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
