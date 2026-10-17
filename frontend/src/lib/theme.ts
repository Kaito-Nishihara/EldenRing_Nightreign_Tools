/**
 * マップ(Konva)を描くときに使う色と大きさ(画面の共通仕様 3.2、3.8)。
 * Konva は CSS の変数を読めないので、色の値はここから取り、部品に直接書かない(AC-UI-01)。
 * THEME の値は globals.css の @theme と同じにする。食い違いは theme.test.ts が見つける。
 */
export const THEME = {
  night: "#0e0c0a",
  nightRaised: "#1a1612",
  nightSunk: "#24201a",
  ash: "#e8dfcc",
  ashSoft: "#a89c86",
  gold: "#c9a45c",
  goldBright: "#e6c886",
  goldDeep: "#8a6d32",
  ember: "#d0654a",
  emberSoft: "#3a1d16",
  tide: "#8c7cf0",
  tideSoft: "#221d3d",
  grace: "#9cc08a",
  line: "#3a3226",
  lineStrong: "#7a6a4e",
  abyss: "#07060a",
} as const;

/** マップの上の印の決まり(画面の共通仕様 3.8) */
export const MAP_STYLE = {
  /** 選んだ収縮の円の外を塗る、夜の雨の暗がり */
  rain: "rgba(11, 9, 32, 0.72)",
  /** 暗がりの縁をぼかす幅(px) */
  rainEdge: 10,
  /** 暗がりを切り抜くときの、穴の色(不透明)と、縁の外の色(透明) */
  maskOpaque: "rgba(255, 255, 255, 1)",
  maskClear: "rgba(255, 255, 255, 0)",
  /** 収縮候補地の中心の印の半径と線の太さ */
  zoneMarkerRadius: 16,
  zoneStrokeWidth: 3,
  /** 中心の印の地。地図の上で印を見分けやすくする */
  zoneMarkerFill: "rgba(14, 12, 10, 0.75)",
  /** 地図は明るいので、文字に night の縁取りを付ける太さ */
  labelStrokeWidth: 4,
  /** 開始地点のアイコンの大きさ。選んだものは大きくする */
  spawnIconSize: 40,
  spawnIconSizeSelected: 56,
  /** 選んでいない開始地点の不透明度 */
  spawnDimmedOpacity: 0.45,
  /** 拠点候補地の印の地 */
  baseMarkerFill: "rgba(26, 22, 18, 0.85)",
} as const;

/**
 * パターンの色。色覚の多様性に配慮した Paul Tol の 8 色(画面の共通仕様 3.2)。
 * 線と円としては night との比が 3 以上ある。文字には使わない。
 */
export const PATTERN_COLORS = [
  "#4477AA",
  "#EE6677",
  "#228833",
  "#CCBB44",
  "#66CCEE",
  "#AA3377",
  "#BBBBBB",
  "#EE7733",
] as const;

/**
 * パターンの順(0 から)に対応する色。8 を超えたら同じ色を繰り返し、番号で見分ける。
 */
export function getPatternColor(index: number): string {
  const length = PATTERN_COLORS.length;
  return PATTERN_COLORS[((index % length) + length) % length];
}
