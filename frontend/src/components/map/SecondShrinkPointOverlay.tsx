"use client";

import { MAP_STYLE, THEME } from "@/lib/theme";
import type { PointEmphasis } from "@/lib/recordMap";
import type { SecondShrinkPoint } from "@/types";

interface SecondShrinkPointOverlayProps {
  /** 出す第二収縮地点と見せ方 */
  points: { point: SecondShrinkPoint; emphasis: PointEmphasis }[];
  /** 入れた第二収縮地点。菱形を塗る */
  selectedId: number | null;
  /** 選んでいるスロットに入っている第二収縮地点。輪郭を強める */
  activeId: number | null;
  /** 押したときの動き。渡さなければ押せない */
  onSelect?: (point: SecondShrinkPoint) => void;
}

/**
 * 第二収縮地点の菱形と名前(画面の共通仕様 3.8、プレイ記録 3.3)。
 * 同じ日の第 1 次の円の外にある地点は、不透明度 0.4 で出す。外の地点も押せる。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function SecondShrinkPointOverlay({ points, selectedId, activeId, onSelect }: SecondShrinkPointOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Group, Rect, Text } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const size = MAP_STYLE.secondPointSize;

  return (
    <>
      {points.map(({ point, emphasis }) => {
        const selected = point.id === selectedId;
        const active = point.id === activeId;
        return (
          <Group key={point.id} opacity={emphasis === "dim" ? MAP_STYLE.dimmedPointOpacity : 1}>
            <Rect
              x={point.x}
              y={point.y}
              width={size}
              height={size}
              offsetX={size / 2}
              offsetY={size / 2}
              rotation={45}
              fill={selected ? THEME.tide : MAP_STYLE.zoneMarkerFill}
              stroke={active ? THEME.azureBright : THEME.tide}
              strokeWidth={MAP_STYLE.zoneStrokeWidth}
              hitStrokeWidth={18}
              listening={Boolean(onSelect)}
              onClick={() => onSelect?.(point)}
              onTap={() => onSelect?.(point)}
            />
            <Text
              x={point.x - 60}
              y={point.y + size / 2 + 6}
              width={120}
              align="center"
              text={point.nameJa}
              fontSize={14}
              fontStyle="bold"
              fill={active ? THEME.azureBright : THEME.ash}
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
