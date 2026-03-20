"use client";

import { useState, useEffect } from "react";
import type { BasePoint } from "@/types";
import { BASE_TYPES } from "@/lib/baseTypes";

/** ?マーカーのサイズ */
const MARKER_SIZE = 24;

interface BasePointMarkersProps {
  /** 拠点候補地一覧 */
  basePoints: BasePoint[];
  /** 各拠点に設定された拠点タイプ（キー: basePointId, 値: baseTypeId） */
  assignments: Record<number, string | null>;
  /** 拠点タイプ選択ハンドラー */
  onAssign?: (basePointId: number, baseTypeId: string | null) => void;
  /** 編集可能かどうか（記録画面ではtrue） */
  editable?: boolean;
}

/**
 * 拠点候補地マーカー
 * マップ上に?マークまたは設定済み拠点アイコンを表示
 * クリックで拠点タイプ選択ポップアップを表示
 */
export default function BasePointMarkers({
  basePoints,
  assignments,
  onAssign,
  editable = false,
}: BasePointMarkersProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Group, Circle, Text, Image: KonvaImage } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [baseTypeImages, setBaseTypeImages] = useState<Record<string, HTMLImageElement>>({});

  /** 拠点タイプ画像をプリロード */
  useEffect(() => {
    if (typeof window === "undefined") return;
    const images: Record<string, HTMLImageElement> = {};
    let loaded = 0;
    BASE_TYPES.forEach((bt) => {
      const img = new window.Image();
      img.src = bt.imagePath;
      img.onload = () => {
        images[bt.id] = img;
        loaded++;
        if (loaded === BASE_TYPES.length) {
          setBaseTypeImages({ ...images });
        }
      };
    });
  }, []);

  /** マーカークリック */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const handleClick = (bp: BasePoint, e: any) => {
    if (!editable) return;
    e.cancelBubble = true;
    setSelectedId(selectedId === bp.id ? null : bp.id);
  };

  /** 拠点タイプ選択 */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const handleTypeSelect = (basePointId: number, typeId: string, e: any) => {
    e.cancelBubble = true;
    const current = assignments[basePointId];
    // 同じタイプの再クリックで解除
    onAssign?.(basePointId, current === typeId ? null : typeId);
    setSelectedId(null);
  };

  return (
    <>
      {basePoints.map((bp) => {
        const assignedTypeId = assignments[bp.id];
        const assignedImage = assignedTypeId ? baseTypeImages[assignedTypeId] : null;
        const isSelected = selectedId === bp.id;

        return (
          <Group key={bp.id}>
            {/* マーカー本体 */}
            <Group
              x={bp.x}
              y={bp.y}
              onClick={(e: unknown) => handleClick(bp, e)}
              onTap={(e: unknown) => handleClick(bp, e)}
            >
              {assignedImage ? (
                /* 設定済み: 拠点アイコン表示 */
                <>
                  <Circle
                    radius={MARKER_SIZE / 2 + 2}
                    fill="rgba(0,0,0,0.6)"
                    stroke="#FFD700"
                    strokeWidth={2}
                  />
                  <KonvaImage
                    image={assignedImage}
                    x={-MARKER_SIZE / 2}
                    y={-MARKER_SIZE / 2}
                    width={MARKER_SIZE}
                    height={MARKER_SIZE}
                  />
                </>
              ) : (
                /* 未設定: ?マーク表示 */
                <>
                  <Circle
                    radius={MARKER_SIZE / 2}
                    fill="rgba(80, 70, 50, 0.8)"
                    stroke="#C8B87C"
                    strokeWidth={2}
                  />
                  <Text
                    text="?"
                    x={-6}
                    y={-8}
                    fontSize={16}
                    fontStyle="bold"
                    fill="#C8B87C"
                  />
                </>
              )}
            </Group>

            {/* 拠点タイプ選択ポップアップ */}
            {isSelected && editable && (
              <Group x={bp.x} y={bp.y + MARKER_SIZE / 2 + 8}>
                {/* 背景 */}
                <Circle
                  radius={0}
                  x={0}
                  y={0}
                />
                {(() => {
                  const cols = 5;
                  const iconSize = 32;
                  const gap = 4;
                  const totalWidth = cols * (iconSize + gap) - gap;
                  const rows = Math.ceil(BASE_TYPES.length / cols);
                  const totalHeight = rows * (iconSize + gap) - gap;
                  const padX = 8;
                  const padY = 8;

                  return (
                    <Group>
                      {/* ポップアップ背景 */}
                      {(() => {
                        /* eslint-disable @typescript-eslint/no-require-imports */
                        const { Rect } = require("react-konva");
                        /* eslint-enable @typescript-eslint/no-require-imports */
                        return (
                          <Rect
                            x={-totalWidth / 2 - padX}
                            y={-padY}
                            width={totalWidth + padX * 2}
                            height={totalHeight + padY * 2}
                            fill="rgba(30, 35, 50, 0.95)"
                            cornerRadius={6}
                            stroke="#555"
                            strokeWidth={1}
                          />
                        );
                      })()}

                      {/* アイコン一覧 */}
                      {BASE_TYPES.map((bt, i) => {
                        const col = i % cols;
                        const row = Math.floor(i / cols);
                        const ix = -totalWidth / 2 + col * (iconSize + gap);
                        const iy = row * (iconSize + gap);
                        const img = baseTypeImages[bt.id];
                        const isAssigned = assignments[bp.id] === bt.id;

                        return (
                          <Group
                            key={bt.id}
                            x={ix}
                            y={iy}
                            onClick={(e: unknown) => handleTypeSelect(bp.id, bt.id, e)}
                            onTap={(e: unknown) => handleTypeSelect(bp.id, bt.id, e)}
                          >
                            {isAssigned && (
                              <Circle
                                x={iconSize / 2}
                                y={iconSize / 2}
                                radius={iconSize / 2 + 2}
                                stroke="#FFD700"
                                strokeWidth={2}
                              />
                            )}
                            {img && (
                              <KonvaImage
                                image={img}
                                width={iconSize}
                                height={iconSize}
                              />
                            )}
                          </Group>
                        );
                      })}
                    </Group>
                  );
                })()}
              </Group>
            )}
          </Group>
        );
      })}
    </>
  );
}
