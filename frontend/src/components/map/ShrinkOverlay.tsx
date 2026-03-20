"use client";

import type { PatternEntry } from "@/types";

/** マップ全体を覆うサイズ（十分大きいマージン付き） */
const OVERLAY_SIZE = 4096;
const OVERLAY_OFFSET = -OVERLAY_SIZE / 2;

interface ShrinkOverlayProps {
  /** 表示する収縮エントリ（選択中のパターンのエントリ） */
  entry: PatternEntry | null;
}

/**
 * 収縮エリア外を青暗く表示するオーバーレイ
 * Konvaの globalCompositeOperation を使い、
 * マップ全体を暗い半透明矩形で覆った後、収縮円の領域だけ切り抜く
 * 親のMapCanvasがdynamic importでSSR隔離済みのため、requireで直接取得する
 */
export default function ShrinkOverlay({ entry }: ShrinkOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Group, Rect, Circle } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  if (!entry) return null;

  return (
    <Group>
      {/* マップ全体を覆う青暗い半透明矩形 */}
      <Rect
        x={OVERLAY_OFFSET}
        y={OVERLAY_OFFSET}
        width={OVERLAY_SIZE}
        height={OVERLAY_SIZE}
        fill="rgba(10, 15, 40, 0.55)"
      />
      {/* 収縮円の領域を切り抜く（destination-out で穴を開ける） */}
      <Circle
        x={entry.zone.centerX}
        y={entry.zone.centerY}
        radius={entry.zone.radius}
        fill="white"
        globalCompositeOperation="destination-out"
      />
    </Group>
  );
}
