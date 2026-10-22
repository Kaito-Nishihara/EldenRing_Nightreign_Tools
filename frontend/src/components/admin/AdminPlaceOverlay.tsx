"use client";

import { useEffect, useState } from "react";
import { MAP_STYLE, THEME } from "@/lib/theme";
import { BASE_POINT_KIND_LABELS, placeName, type AdminPlace } from "@/lib/adminPlaces";
import type { AdminPlacePreview } from "@/hooks/useAdminEditor";
import type { AdminPlaceKind, BasePointKind } from "@/types";

interface AdminPlaceOverlayProps {
  kind: AdminPlaceKind;
  /** 選んだ種類の、選んだマップの行 */
  places: AdminPlace[];
  editingId: number | null;
  /** 入力中の場所。直している収縮候補地は、この大きさの円を破線で出す */
  preview: AdminPlacePreview | null;
  onSelect: (place: AdminPlace) => void;
}

/** 印の当たりの半径。指で押しやすくする */
const HIT_RADIUS = 22;
const BASE_MARKER_RADIUS = 14;

/** 拠点の種類ごとの丸の地と字と枠の色(画面の共通仕様 3.8) */
const BASE_KIND_STYLE: Record<BasePointKind, { fill: string; text: string; stroke: string }> = {
  major: { fill: THEME.ember, text: THEME.night, stroke: THEME.night },
  minor: { fill: THEME.azure, text: THEME.night, stroke: THEME.night },
  field_boss: { fill: THEME.night, text: THEME.ember, stroke: THEME.ember },
};

/**
 * 管理の画面のマップの印(マスタの管理 3.2、3.3)。選んだ種類の印だけを出す。
 * 非表示の行は点線の印にして薄く出す。直している行の印は azure-bright の線にする。
 * 印を押したときは、マップを押したこと(新しく登録する)にならないよう、ステージへ伝えない。
 * 親の MapCanvas が dynamic import で SSR から隔離しているので、require で react-konva を取る。
 */
export default function AdminPlaceOverlay({ kind, places, editingId, preview, onSelect }: AdminPlaceOverlayProps) {
  /* eslint-disable @typescript-eslint/no-require-imports */
  const { Circle, Group, Image: KonvaImage, Rect, Text } = require("react-konva");
  /* eslint-enable @typescript-eslint/no-require-imports */

  const [spawnIcon, setSpawnIcon] = useState<HTMLImageElement | null>(null);
  useEffect(() => {
    if (kind !== "spawn-points" || typeof window === "undefined") return;
    const img = new window.Image();
    img.src = "/icons/zones/spawn.webp";
    img.onload = () => setSpawnIcon(img);
  }, [kind]);

  /** 種類ごとの印。線の色と点線は、直しているかと非表示かで変える。stroke が null ならふだんの色 */
  const renderMarker = (place: AdminPlace, stateStroke: string | null, dash: readonly number[] | undefined) => {
    const stroke = stateStroke ?? normalStroke;
    switch (kind) {
      case "spawn-points": {
        const size = MAP_STYLE.spawnIconSize;
        return (
          <>
            <Circle x={place.x} y={place.y} radius={size / 2 + 3} stroke={stroke} strokeWidth={2} dash={dash} />
            {spawnIcon ? (
              <KonvaImage image={spawnIcon} x={place.x - size / 2} y={place.y - size / 2} width={size} height={size} />
            ) : (
              <Circle x={place.x} y={place.y} radius={8} fill={THEME.azureBright} />
            )}
          </>
        );
      }
      case "shrink-zones":
        return (
          <Circle
            x={place.x}
            y={place.y}
            radius={MAP_STYLE.zoneMarkerRadius}
            fill={MAP_STYLE.zoneMarkerFill}
            stroke={stroke}
            strokeWidth={MAP_STYLE.zoneStrokeWidth}
            dash={dash}
          />
        );
      case "second-shrink-points": {
        const size = MAP_STYLE.secondPointSize;
        return (
          <Rect
            x={place.x}
            y={place.y}
            width={size}
            height={size}
            offsetX={size / 2}
            offsetY={size / 2}
            rotation={45}
            fill={MAP_STYLE.zoneMarkerFill}
            stroke={stroke}
            strokeWidth={MAP_STYLE.zoneStrokeWidth}
            dash={dash}
          />
        );
      }
      case "base-points": {
        const style = place.baseKind ? BASE_KIND_STYLE[place.baseKind] : null;
        return (
          <>
            <Circle
              x={place.x}
              y={place.y}
              radius={BASE_MARKER_RADIUS}
              fill={style?.fill ?? MAP_STYLE.baseMarkerFill}
              stroke={stateStroke ?? style?.stroke ?? THEME.lineStrong}
              strokeWidth={2}
              dash={dash}
            />
            <Text
              x={place.x - BASE_MARKER_RADIUS}
              y={place.y - 8}
              width={BASE_MARKER_RADIUS * 2}
              align="center"
              text={place.baseKind ? BASE_POINT_KIND_LABELS[place.baseKind].mark : "?"}
              fontSize={16}
              fontStyle="bold"
              fill={style?.text ?? THEME.ash}
            />
          </>
        );
      }
    }
  };

  /** 種類ごとの、ふだんの線の色(画面の共通仕様 3.8)。拠点候補地は拠点の種類ごとに決める */
  const normalStroke = kind === "spawn-points" ? MAP_STYLE.maskClear : THEME.tide;
  const editingZone = kind === "shrink-zones" && preview;

  return (
    <>
      {editingZone && (
        <Circle
          x={preview.x}
          y={preview.y}
          radius={preview.radius}
          stroke={THEME.azure}
          strokeWidth={3}
          dash={MAP_STYLE.editingDash}
          listening={false}
        />
      )}
      {places.map((place) => {
        const editing = place.id === editingId;
        const stroke = editing ? THEME.azureBright : place.isHidden ? THEME.ashSoft : null;
        const select = (event: { cancelBubble: boolean }) => {
          event.cancelBubble = true;
          onSelect(place);
        };
        return (
          <Group
            key={place.id}
            opacity={place.isHidden && !editing ? MAP_STYLE.hiddenOpacity : 1}
            onClick={select}
            onTap={select}
          >
            {renderMarker(place, stroke, place.isHidden ? MAP_STYLE.hiddenDash : undefined)}
            <Circle x={place.x} y={place.y} radius={HIT_RADIUS} fill={MAP_STYLE.maskClear} />
            <Text
              x={place.x - 60}
              y={place.y + HIT_RADIUS + 2}
              width={120}
              align="center"
              text={placeName(place)}
              fontSize={14}
              fontStyle="bold"
              fill={editing ? THEME.azureBright : THEME.ash}
              stroke={THEME.night}
              strokeWidth={MAP_STYLE.labelStrokeWidth}
              fillAfterStrokeEnabled
              listening={false}
            />
          </Group>
        );
      })}
      {/* 入力中の場所。マップを押した場所か、入れた X と Y */}
      {preview && (
        <Group listening={false}>
          <Circle x={preview.x} y={preview.y} radius={10} stroke={THEME.azureBright} strokeWidth={2} />
          <Circle x={preview.x} y={preview.y} radius={3} fill={THEME.azureBright} />
        </Group>
      )}
    </>
  );
}
