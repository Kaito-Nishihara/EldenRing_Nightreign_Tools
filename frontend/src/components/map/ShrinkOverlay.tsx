"use client";

import { MAP_STYLE } from "@/lib/theme";

/** マップ全体を覆う大きさ(拡大と移動をしても端が見えないよう、十分に大きくする) */
const OVERLAY_SIZE = 4096;
const OVERLAY_OFFSET = -OVERLAY_SIZE / 2;

/** 収縮の円。座標と半径は 1024 四方の座標空間の数 */
export interface RainCircle {
  x: number;
  y: number;
  radius: number;
}

interface ShrinkOverlayProps {
  /** 収縮の円。2 つあるとき(1 日目と 2 日目)は、どちらの円にも入らない所を塗る */
  circles: readonly RainCircle[];
}

/**
 * 選んだ収縮の円の外を、夜の雨の暗がりで塗る(画面の共通仕様 3.8)。
 * マップ全体を暗がりで覆い、円の所を destination-out で切り抜く。円の中は動ける場所なので塗らない。
 * 縁は外へ向けて薄くなる放射状のぼかしで切り抜き、境目を 10px ぼかす。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function ShrinkOverlay({ circles }: ShrinkOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Group, Rect, Circle } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  if (circles.length === 0) return null;

  const edge = MAP_STYLE.rainEdge;

  return (
    <Group listening={false}>
      <Rect x={OVERLAY_OFFSET} y={OVERLAY_OFFSET} width={OVERLAY_SIZE} height={OVERLAY_SIZE} fill={MAP_STYLE.rain} />
      {circles.map((circle) => {
        const outer = circle.radius + edge;
        // 半径 - 縁 までは完全に切り抜き、半径 + 縁 にかけて切り抜きを弱める
        const solidStop = Math.max(0, (circle.radius - edge) / outer);
        return (
          <Circle
            key={`${circle.x}-${circle.y}-${circle.radius}`}
            x={circle.x}
            y={circle.y}
            radius={outer}
            fillRadialGradientStartPoint={{ x: 0, y: 0 }}
            fillRadialGradientEndPoint={{ x: 0, y: 0 }}
            fillRadialGradientStartRadius={0}
            fillRadialGradientEndRadius={outer}
            fillRadialGradientColorStops={[0, MAP_STYLE.maskOpaque, solidStop, MAP_STYLE.maskOpaque, 1, MAP_STYLE.maskClear]}
            globalCompositeOperation="destination-out"
          />
        );
      })}
    </Group>
  );
}
