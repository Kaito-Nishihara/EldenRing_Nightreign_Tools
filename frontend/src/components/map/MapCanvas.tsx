"use client";

import { useRef, useState, useCallback, useEffect, type ReactNode } from "react";
import dynamic from "next/dynamic";

/**
 * react-konvaはSSR非対応のため、dynamic importで読み込む
 */
const Stage = dynamic(
  () => import("react-konva").then((mod) => mod.Stage),
  { ssr: false }
);
const Layer = dynamic(
  () => import("react-konva").then((mod) => mod.Layer),
  { ssr: false }
);
const Rect = dynamic(
  () => import("react-konva").then((mod) => mod.Rect),
  { ssr: false }
);
const KonvaImage = dynamic(
  () => import("react-konva").then((mod) => mod.Image),
  { ssr: false }
);

/** マップキャンバスのデフォルトサイズ */
const DEFAULT_MAP_WIDTH = 1024;
const DEFAULT_MAP_HEIGHT = 1024;

/** ズームの最小・最大倍率 */
const MIN_SCALE = 0.3;
const MAX_SCALE = 3.0;

interface MapCanvasProps {
  /** マップ画像のパス（nullの場合はプレースホルダーを表示） */
  mapImagePath?: string | null;
  /** マップの幅 */
  width?: number;
  /** マップの高さ */
  height?: number;
  /** オーバーレイとして描画する子要素 */
  children?: ReactNode;
}

/**
 * マップキャンバスコンポーネント
 * react-konvaを使用してマップ画像を表示し、ズーム・パン操作に対応する
 */
export default function MapCanvas({
  mapImagePath,
  width = DEFAULT_MAP_WIDTH,
  height = DEFAULT_MAP_HEIGHT,
  children,
}: MapCanvasProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const [stageSize, setStageSize] = useState({ width: 800, height: 600 });
  const [scale, setScale] = useState(1);
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [mapImage, setMapImage] = useState<HTMLImageElement | null>(null);
  const [isClient, setIsClient] = useState(false);

  /** クライアントサイドでのみレンダリングするためのフラグ */
  useEffect(() => {
    setIsClient(true);
  }, []);

  /** コンテナサイズの変更を監視してステージサイズを更新する */
  useEffect(() => {
    if (!containerRef.current) return;

    const updateSize = () => {
      if (containerRef.current) {
        setStageSize({
          width: containerRef.current.offsetWidth,
          height: containerRef.current.offsetHeight,
        });
      }
    };

    updateSize();

    const observer = new ResizeObserver(updateSize);
    observer.observe(containerRef.current);

    return () => observer.disconnect();
  }, []);

  /** マップ画像を読み込む */
  useEffect(() => {
    if (!mapImagePath || typeof window === "undefined") return;

    const img = new window.Image();
    img.src = mapImagePath;
    img.onload = () => setMapImage(img);
    img.onerror = () => {
      console.error("マップ画像の読み込みに失敗しました:", mapImagePath);
      setMapImage(null);
    };
  }, [mapImagePath]);

  /** マウスホイールによるズーム操作 */
  const handleWheel = useCallback(
    (e: { evt: WheelEvent; target: { getStage: () => { getPointerPosition: () => { x: number; y: number } | null } } }) => {
      e.evt.preventDefault();

      const stage = e.target.getStage();
      const pointer = stage.getPointerPosition();
      if (!pointer) return;

      const scaleBy = 1.1;
      const oldScale = scale;
      const newScale =
        e.evt.deltaY < 0
          ? Math.min(oldScale * scaleBy, MAX_SCALE)
          : Math.max(oldScale / scaleBy, MIN_SCALE);

      // ポインター位置を中心にズームする
      const mousePointTo = {
        x: (pointer.x - position.x) / oldScale,
        y: (pointer.y - position.y) / oldScale,
      };

      setScale(newScale);
      setPosition({
        x: pointer.x - mousePointTo.x * newScale,
        y: pointer.y - mousePointTo.y * newScale,
      });
    },
    [scale, position]
  );

  /** ドラッグ終了時の位置更新 */
  const handleDragEnd = useCallback(
    (e: { target: { x: () => number; y: () => number } }) => {
      setPosition({
        x: e.target.x(),
        y: e.target.y(),
      });
    },
    []
  );

  return (
    <div
      ref={containerRef}
      className="w-full h-full bg-gray-900 overflow-hidden relative"
    >
      {isClient && (
        <Stage
          width={stageSize.width}
          height={stageSize.height}
          scaleX={scale}
          scaleY={scale}
          x={position.x}
          y={position.y}
          draggable
          onWheel={handleWheel}
          onDragEnd={handleDragEnd}
        >
          <Layer>
            {/* マップ画像またはプレースホルダー */}
            {mapImage ? (
              <KonvaImage image={mapImage} width={width} height={height} />
            ) : (
              <Rect
                width={width}
                height={height}
                fill="#374151"
                stroke="#6B7280"
                strokeWidth={2}
              />
            )}
          </Layer>
          {/* オーバーレイ用レイヤー（子コンポーネント） */}
          <Layer>{children}</Layer>
        </Stage>
      )}

      {/* ズーム表示 */}
      <div className="absolute bottom-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded">
        {Math.round(scale * 100)}%
      </div>
    </div>
  );
}
