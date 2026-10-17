"use client";

import { MAP_STYLE, THEME } from "@/lib/theme";
import { dayNumeral, type PatternMapView } from "@/lib/patternMap";

interface PatternRouteLayerProps {
  /** 選んだパターンを描く形にしたもの */
  view: PatternMapView;
  /** パターンの色 */
  color: string;
}

/**
 * 選んだパターンで実際に起きた収縮を描く(パターン検索 3.3)。
 * 第 1 次はパターンの色の円(中は塗らない)と、中心の日の丸、円の上の「DAY I 最北」。
 * 第 2 次はパターンの色の菱形で、同じ日の第 1 次の中心と細い破線でつなぐ。
 * 「すべて」のときは、開始地点から日の順に第 1 次の中心を線でつなぐ。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function PatternRouteLayer({ view, color }: PatternRouteLayerProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Group, Circle, Line, Text, Rect } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const chipRadius = MAP_STYLE.dayChipRadius;
  const diamond = MAP_STYLE.secondPointSize;

  return (
    <Group listening={false}>
      {view.route.length > 0 && (
        <Line
          points={view.route}
          stroke={color}
          strokeWidth={MAP_STYLE.routeStrokeWidth}
          lineCap="round"
          lineJoin="round"
        />
      )}

      {/* 円と、第 2 次への破線と菱形 */}
      {view.days.map(({ day, zone, point }) => (
        <Group key={`shape-${day}`}>
          {zone && (
            <Circle x={zone.centerX} y={zone.centerY} radius={zone.radius} stroke={color} strokeWidth={MAP_STYLE.zoneStrokeWidth} />
          )}
          {zone && point && (
            <Line
              points={[zone.centerX, zone.centerY, point.x, point.y]}
              stroke={color}
              strokeWidth={2}
              dash={MAP_STYLE.secondPointDash}
            />
          )}
          {point && (
            <Rect
              x={point.x}
              y={point.y}
              width={diamond}
              height={diamond}
              offsetX={diamond / 2}
              offsetY={diamond / 2}
              rotation={45}
              fill={color}
              stroke={THEME.night}
              strokeWidth={2}
            />
          )}
        </Group>
      ))}

      {/* 日の丸と名前は、ほかの日の円や線に隠れないよう、最後にまとめて描く */}
      {view.days.map(({ day, zone }) =>
        zone ? (
          <Group key={`label-${day}`}>
            <Circle x={zone.centerX} y={zone.centerY} radius={chipRadius} fill={color} stroke={THEME.night} strokeWidth={2} />
            <Text
              x={zone.centerX - chipRadius}
              y={zone.centerY - chipRadius}
              width={chipRadius * 2}
              height={chipRadius * 2}
              align="center"
              verticalAlign="middle"
              text={dayNumeral(day)}
              fontSize={16}
              fontStyle="bold"
              fill={THEME.night}
            />
            <Text
              x={zone.centerX - 120}
              y={zone.centerY - zone.radius + 14}
              width={240}
              align="center"
              text={`DAY ${dayNumeral(day)} ${zone.nameJa}`}
              fontSize={18}
              fontStyle="bold"
              fill={THEME.ash}
              stroke={THEME.night}
              strokeWidth={MAP_STYLE.labelStrokeWidth}
              fillAfterStrokeEnabled
            />
          </Group>
        ) : null,
      )}
    </Group>
  );
}
