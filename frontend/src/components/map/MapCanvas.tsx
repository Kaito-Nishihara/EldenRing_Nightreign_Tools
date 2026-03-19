"use client";

import { useRef, useState, useCallback, useEffect, type ReactNode } from "react";
import dynamic from "next/dynamic";

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
  /** 収縮暗転レイヤー（destination-outが必要なため専用Layerで描画） */
  shrinkOverlay?: ReactNode;
  /** マップ上のクリックイベント（マップ座標系のx,yを返す） */
  onStageClick?: (x: number, y: number) => void;
}

/**
 * MapCanvasの内部実装
 * react-konvaのimportをSSRから隔離するため、このコンポーネントをdynamic importする
 */
function MapCanvasInner({
  mapImagePath,
  width = DEFAULT_MAP_WIDTH,
  height = DEFAULT_MAP_HEIGHT,
  children,
  shrinkOverlay,
  onStageClick,
}: MapCanvasProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Stage, Layer, Rect, Image: KonvaImage } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const containerRef = useRef<HTMLDivElement>(null);
  const [stageSize, setStageSize] = useState({ width: 800, height: 600 });
  const [scale, setScale] = useState(1);
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [mapImage, setMapImage] = useState<HTMLImageElement | null>(null);

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
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (e: any) => {
      e.evt.preventDefault();

      const stage = e.target.getStage();
      if (!stage) return;
      const pointer = stage.getPointerPosition();
      if (!pointer) return;

      const scaleBy = 1.1;
      const oldScale = scale;
      const newScale =
        e.evt.deltaY < 0
          ? Math.min(oldScale * scaleBy, MAX_SCALE)
          : Math.max(oldScale / scaleBy, MIN_SCALE);

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

  /** ステージクリック時にマップ座標を計算して親に通知する */
  const handleStageClick = useCallback(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (e: any) => {
      if (!onStageClick) return;
      const stage = e.target.getStage();
      if (!stage) return;
      const pointer = stage.getPointerPosition();
      if (!pointer) return;

      const mapX = (pointer.x - position.x) / scale;
      const mapY = (pointer.y - position.y) / scale;

      onStageClick(Math.round(mapX), Math.round(mapY));
    },
    [onStageClick, position, scale]
  );

  /** ドラッグ終了時の位置更新 */
  const handleDragEnd = useCallback(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (e: any) => {
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
        onClick={handleStageClick}
        onTap={handleStageClick}
      >
        <Layer>
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
        {/* 収縮暗転レイヤー（destination-outで円形切り抜きするため専用Layer） */}
        {shrinkOverlay && <Layer listening={false}>{shrinkOverlay}</Layer>}
        <Layer>{children}</Layer>
      </Stage>

      {/* ズーム表示 */}
      <div className="absolute bottom-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded">
        {Math.round(scale * 100)}%
      </div>
    </div>
  );
}

/**
 * マップキャンバスコンポーネント
 * react-konvaはSSR非対応のため、dynamic importでクライアントのみ描画する
 */
const MapCanvas = dynamic(() => Promise.resolve(MapCanvasInner), {
  ssr: false,
  loading: () => (
    <div className="w-full h-full bg-gray-900 flex items-center justify-center">
      <span className="text-gray-500 text-sm">マップを読み込み中...</span>
    </div>
  ),
});

export default MapCanvas;
