"use client";

import { useCallback } from "react";
import dynamic from "next/dynamic";
import type { Pattern } from "@/types";

const Group = dynamic(
  () => import("react-konva").then((mod) => mod.Group),
  { ssr: false }
);
const Circle = dynamic(
  () => import("react-konva").then((mod) => mod.Circle),
  { ssr: false }
);
const Line = dynamic(
  () => import("react-konva").then((mod) => mod.Line),
  { ssr: false }
);
const Text = dynamic(
  () => import("react-konva").then((mod) => mod.Text),
  { ssr: false }
);

/**
 * 色覚多様性対応カラーパレット
 * 赤緑のみの区別に依存しないよう設計
 * Paul Tol's色覚バリアフリーパレットをベースに選定
 */
const PATTERN_COLORS = [
  "#4477AA", // 青
  "#EE6677", // ローズ
  "#228833", // 緑
  "#CCBB44", // 黄
  "#66CCEE", // シアン
  "#AA3377", // 紫
  "#BBBBBB", // グレー
  "#EE8866", // オレンジ
];

/** パターンの色を取得 */
export function getPatternColor(index: number): string {
  return PATTERN_COLORS[index % PATTERN_COLORS.length];
}

interface PatternLayerProps {
  /** パターン一覧 */
  patterns: Pattern[];
  /** 選択中のパターンインデックス（null=未選択） */
  selectedPatternIndex: number | null;
  /** パターン選択ハンドラー */
  onPatternSelect: (patternIndex: number | null) => void;
}

/**
 * パターン描画レイヤー
 * パターンごとに色分けした収縮円を描画し、
 * Day間の遷移を線で接続する
 */
export default function PatternLayer({
  patterns,
  selectedPatternIndex,
  onPatternSelect,
}: PatternLayerProps) {
  /** パターンクリック時のハンドラー */
  const handlePatternClick = useCallback(
    (patternIndex: number) => {
      onPatternSelect(
        selectedPatternIndex === patternIndex ? null : patternIndex
      );
    },
    [selectedPatternIndex, onPatternSelect]
  );

  if (patterns.length === 0) return null;

  return (
    <>
      {patterns.map((pattern) => {
        const color = getPatternColor(pattern.patternIndex - 1);
        const isSelected = selectedPatternIndex === pattern.patternIndex;
        const isOther =
          selectedPatternIndex !== null &&
          selectedPatternIndex !== pattern.patternIndex;
        /** 未選択パターンは薄く表示 */
        const opacity = isOther ? 0.15 : 1;

        /** エントリをday-phaseの昇順でソート */
        const sortedEntries = [...pattern.entries].sort((a, b) =>
          a.day !== b.day ? a.day - b.day : a.phase - b.phase
        );

        /** 接続線の座標配列を生成 */
        const linePoints = sortedEntries.flatMap((entry) => [
          entry.zone.centerX,
          entry.zone.centerY,
        ]);

        return (
          <Group key={pattern.patternIndex} opacity={opacity}>
            {/* エントリ間の接続線 */}
            {sortedEntries.length > 1 && (
              <Line
                points={linePoints}
                stroke={color}
                strokeWidth={isSelected ? 3 : 2}
                dash={[8, 4]}
                lineCap="round"
                lineJoin="round"
              />
            )}

            {/* 各エントリの収縮円 */}
            {sortedEntries.map((entry) => {
              const label = `D${entry.day}-P${entry.phase}`;
              return (
                <Group
                  key={`${pattern.patternIndex}-${entry.day}-${entry.phase}`}
                  onClick={() => handlePatternClick(pattern.patternIndex)}
                  onTap={() => handlePatternClick(pattern.patternIndex)}
                >
                  {/* 収縮円 */}
                  <Circle
                    x={entry.zone.centerX}
                    y={entry.zone.centerY}
                    radius={entry.zone.radius * (isSelected ? 1.05 : 1)}
                    fill={color}
                    opacity={isSelected ? 0.35 : 0.2}
                    stroke={color}
                    strokeWidth={isSelected ? 3 : 1.5}
                    hitStrokeWidth={10}
                  />
                  {/* Day-Phaseラベル */}
                  <Text
                    x={entry.zone.centerX - 20}
                    y={entry.zone.centerY - 8}
                    text={label}
                    fontSize={isSelected ? 14 : 12}
                    fontStyle={isSelected ? "bold" : "normal"}
                    fill={color}
                    align="center"
                    width={40}
                  />
                </Group>
              );
            })}

            {/* パターン番号ラベル（最初のエントリの位置に表示） */}
            {sortedEntries.length > 0 && (
              <Text
                x={sortedEntries[0].zone.centerX - 30}
                y={sortedEntries[0].zone.centerY - sortedEntries[0].zone.radius - 20}
                text={`#${pattern.patternIndex} (${pattern.recordCount}件)`}
                fontSize={isSelected ? 13 : 11}
                fontStyle="bold"
                fill={color}
                align="center"
                width={60}
              />
            )}
          </Group>
        );
      })}
    </>
  );
}
