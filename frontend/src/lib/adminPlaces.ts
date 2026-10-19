import type { AdminPlaceDtoMap, AdminPlaceKind } from "@/types";

// マスタの管理の画面で、4 種類の場所を同じ形で扱うための決まり(マスタの管理 3.2、3.4)

/** 座標と半径の範囲。座標空間は 1024 四方(決定の記録 0004) */
export const COORDINATE_MIN = 0;
export const COORDINATE_MAX = 1024;
export const RADIUS_MIN = 10;
export const RADIUS_MAX = 512;
export const NAME_MAX_LENGTH = 100;

/** 半径のつまみの範囲と、新しく登録するときの半径(マスタの管理 3.3) */
export const RADIUS_SLIDER_MIN = 100;
export const RADIUS_SLIDER_MAX = 500;
export const DEFAULT_RADIUS = 255;

/** 誤りの文言。master-service の文言と同じにする(マスタの管理 3.4) */
export const NAME_REQUIRED_MESSAGE = "名前を入れてください";
export const NAME_TOO_LONG_MESSAGE = `名前は ${NAME_MAX_LENGTH} 文字までです`;
export const COORDINATE_MESSAGE = `${COORDINATE_MIN} から ${COORDINATE_MAX} の数を入れてください`;
export const RADIUS_MESSAGE = `${RADIUS_MIN} から ${RADIUS_MAX} の数を入れてください`;

/** 通常のマップの識別子。拠点候補地はマップごとに持つ */
export const NORMAL_MAP_ID = "normal";

/**
 * 画面の中で扱う場所の形。種類にない項目は null にする。
 * 収縮候補地の中心も x と y に入れる。
 */
export interface AdminPlace {
  id: number;
  nameJa: string | null;
  x: number;
  y: number;
  /** 収縮候補地だけ */
  radius: number | null;
  /** 拠点候補地だけ */
  mapId: string | null;
  /** 収縮候補地だけ。画面では直さず、修正のときにそのまま送り返す */
  iconPath: string | null;
  isHidden: boolean;
}

/** 入力の欄の値。X と Y は、入れた文字のまま持つ */
export interface AdminPlaceInput {
  nameJa: string;
  x: string;
  y: string;
  radius: number;
}

export type AdminPlaceErrors = Partial<Record<keyof AdminPlaceInput, string>>;

/** 確かめを通った、送れる値 */
export interface AdminPlaceValues {
  nameJa: string | null;
  x: number;
  y: number;
  radius: number;
}

/** 管理用の API に送る値。isHidden を省くと、登録では表示中、修正では今のまま(マスタの管理 4.2) */
export interface AdminPlaceSaveValues extends AdminPlaceValues {
  mapId: string;
  iconPath: string | null;
  isHidden?: boolean;
}

/** 種類ごとの違い。画面の部品は、この表を見て項目を出し分ける */
export interface AdminPlaceKindSpec<K extends AdminPlaceKind = AdminPlaceKind> {
  kind: K;
  label: string;
  /** 名前が必須か。拠点候補地では任意 */
  nameRequired: boolean;
  hasRadius: boolean;
  /** マップの種類(通常、各地変)ごとに持つか */
  hasMap: boolean;
  // 種類を決めずに扱えるよう、メソッドの書き方にする(引数の型を厳しく比べない)
  fromDto(dto: AdminPlaceDtoMap[K]): AdminPlace;
  toRequest(values: AdminPlaceSaveValues): object;
}

const BASE = { radius: null, mapId: null, iconPath: null } as const;

export const ADMIN_PLACE_KINDS: { [K in AdminPlaceKind]: AdminPlaceKindSpec<K> } = {
  "spawn-points": {
    kind: "spawn-points",
    label: "開始地点",
    nameRequired: true,
    hasRadius: false,
    hasMap: false,
    fromDto: (d) => ({ ...BASE, id: d.id, nameJa: d.nameJa, x: d.x, y: d.y, isHidden: d.isHidden }),
    toRequest: (v) => ({ nameJa: v.nameJa, x: v.x, y: v.y, isHidden: v.isHidden }),
  },
  "shrink-zones": {
    kind: "shrink-zones",
    label: "収縮候補地",
    nameRequired: true,
    hasRadius: true,
    hasMap: false,
    fromDto: (d) => ({
      ...BASE,
      id: d.id,
      nameJa: d.nameJa,
      x: d.centerX,
      y: d.centerY,
      radius: d.radius,
      iconPath: d.iconPath ?? null,
      isHidden: d.isHidden,
    }),
    toRequest: (v) => ({ nameJa: v.nameJa, iconPath: v.iconPath, centerX: v.x, centerY: v.y, radius: v.radius, isHidden: v.isHidden }),
  },
  "second-shrink-points": {
    kind: "second-shrink-points",
    label: "第二収縮地点",
    nameRequired: true,
    hasRadius: false,
    hasMap: false,
    fromDto: (d) => ({ ...BASE, id: d.id, nameJa: d.nameJa, x: d.x, y: d.y, isHidden: d.isHidden }),
    toRequest: (v) => ({ nameJa: v.nameJa, x: v.x, y: v.y, isHidden: v.isHidden }),
  },
  "base-points": {
    kind: "base-points",
    label: "拠点候補地",
    nameRequired: false,
    hasRadius: false,
    hasMap: true,
    fromDto: (d) => ({ ...BASE, id: d.id, nameJa: d.nameJa ?? null, x: d.x, y: d.y, mapId: d.mapId, isHidden: d.isHidden }),
    toRequest: (v) => ({ mapId: v.mapId, nameJa: v.nameJa, x: v.x, y: v.y, isHidden: v.isHidden }),
  },
};

/** 種類を切り替える順(マスタの管理 3.2) */
export const ADMIN_PLACE_KIND_ORDER: readonly AdminPlaceKind[] = [
  "spawn-points",
  "shrink-zones",
  "second-shrink-points",
  "base-points",
];

/** 座標を入力の欄に出す形(小数 2 桁)にする */
export function formatCoordinate(value: number): string {
  return value.toFixed(2);
}

/** マップを押した場所を、座標空間に収めて小数 2 桁に丸める(マスタの管理 3.3) */
export function pickedCoordinate(value: number): number {
  const clamped = Math.min(Math.max(value, COORDINATE_MIN), COORDINATE_MAX);
  return Math.round(clamped * 100) / 100;
}

export function emptyInput(): AdminPlaceInput {
  return { nameJa: "", x: "", y: "", radius: DEFAULT_RADIUS };
}

/** 登録済みの場所を直すときの、入力の欄の値 */
export function inputFromPlace(place: AdminPlace): AdminPlaceInput {
  return {
    nameJa: place.nameJa ?? "",
    x: formatCoordinate(place.x),
    y: formatCoordinate(place.y),
    radius: place.radius ?? DEFAULT_RADIUS,
  };
}

// 小数は 2 桁まで。符号と指数の書き方は受け付けない
const COORDINATE_PATTERN = /^\d+(\.\d{1,2})?$/;

function parseCoordinate(text: string): number | null {
  const trimmed = text.trim();
  if (!COORDINATE_PATTERN.test(trimmed)) return null;
  const value = Number(trimmed);
  return value >= COORDINATE_MIN && value <= COORDINATE_MAX ? value : null;
}

/**
 * 入力の欄の値を確かめる(マスタの管理 3.4)。
 * 誤りがなければ values に送れる値を入れ、あれば errors に欄ごとの文言を入れる。
 */
export function validateInput(
  spec: AdminPlaceKindSpec,
  input: AdminPlaceInput
): { values: AdminPlaceValues | null; errors: AdminPlaceErrors } {
  const errors: AdminPlaceErrors = {};
  const name = input.nameJa.trim();
  if (name.length === 0 && spec.nameRequired) errors.nameJa = NAME_REQUIRED_MESSAGE;
  else if (name.length > NAME_MAX_LENGTH) errors.nameJa = NAME_TOO_LONG_MESSAGE;

  const x = parseCoordinate(input.x);
  const y = parseCoordinate(input.y);
  if (x === null) errors.x = COORDINATE_MESSAGE;
  if (y === null) errors.y = COORDINATE_MESSAGE;

  if (spec.hasRadius && !(input.radius >= RADIUS_MIN && input.radius <= RADIUS_MAX)) errors.radius = RADIUS_MESSAGE;

  if (Object.keys(errors).length > 0 || x === null || y === null) return { values: null, errors };
  return { values: { nameJa: name.length > 0 ? name : null, x, y, radius: input.radius }, errors };
}

/** 一覧と題に出す名前。名前のない拠点候補地は番号で出す */
export function placeName(place: AdminPlace): string {
  return place.nameJa ?? `${place.id} 番`;
}

/** 入力の枠の題(マスタの管理 3.3)。例: 収縮候補地「最北」を直す */
export function formTitle(spec: AdminPlaceKindSpec, editing: AdminPlace | null): string {
  return editing ? `${spec.label}「${placeName(editing)}」を直す` : `${spec.label}を登録する`;
}

/** 拠点候補地は、選んだマップの行だけを出す。ほかの種類はそのまま */
export function placesOnMap(spec: AdminPlaceKindSpec, places: readonly AdminPlace[], mapId: string): AdminPlace[] {
  return spec.hasMap ? places.filter((p) => p.mapId === mapId) : [...places];
}
