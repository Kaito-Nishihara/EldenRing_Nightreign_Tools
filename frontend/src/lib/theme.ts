/**
 * マップ(Konva)を描くときに使う色と大きさ(画面の共通仕様 3.2、3.8)。
 * Konva は CSS の変数を読めないので、色の値はここから取り、部品に直接書かない(AC-UI-01)。
 * THEME の値は globals.css の @theme と同じにする。食い違いは theme.test.ts が見つける。
 */
export const THEME = {
  night: "#070b16",
  nightRaised: "#0e1526",
  nightSunk: "#172038",
  ash: "#e4eaf3",
  ashSoft: "#8f9ab0",
  azure: "#5b93ff",
  azureBright: "#a8c8ff",
  azureDeep: "#3b63b3",
  ember: "#ec6a72",
  emberSoft: "#3a1520",
  tide: "#b18cff",
  tideSoft: "#231a40",
  grace: "#86c9a0",
  line: "#1f2a44",
  lineStrong: "#5a70a8",
  abyss: "#03050b",
} as const;

/** マップの上の印の決まり(画面の共通仕様 3.8) */
export const MAP_STYLE = {
  /** 選んだ収縮の円の外を塗る、夜の雨の暗がり */
  rain: "rgba(8, 10, 34, 0.72)",
  /** 暗がりの縁をぼかす幅(px) */
  rainEdge: 10,
  /** 暗がりを切り抜くときの、穴の色(不透明)と、縁の外の色(透明) */
  maskOpaque: "rgba(255, 255, 255, 1)",
  maskClear: "rgba(255, 255, 255, 0)",
  /** 収縮候補地の中心の印の半径と線の太さ */
  zoneMarkerRadius: 16,
  zoneStrokeWidth: 3,
  /** 中心の印の地。地図の上で印を見分けやすくする */
  zoneMarkerFill: "rgba(7, 11, 22, 0.75)",
  /** 地図は明るいので、文字に night の縁取りを付ける太さ */
  labelStrokeWidth: 4,
  /** 開始地点のアイコンの大きさ。選んだものは大きくする */
  spawnIconSize: 40,
  spawnIconSizeSelected: 56,
  /** 選んでいない開始地点の不透明度 */
  spawnDimmedOpacity: 0.45,
  /** 拠点候補地の印の地 */
  baseMarkerFill: "rgba(14, 21, 38, 0.85)",
  /** パターンの第 1 次の中心に置く、日の丸の半径 */
  dayChipRadius: 18,
  /** 第 2 次の菱形の一辺と、第 1 次の中心とつなぐ破線 */
  secondPointSize: 20,
  secondPointDash: [6, 5],
  /** 同じ日の第 1 次の円の外にある第二収縮地点の不透明度(プレイ記録 3.3) */
  dimmedPointOpacity: 0.4,
  /** 第 2 次のスロットを選んでいる間の、押せない収縮候補地の印の不透明度(プレイ記録 3.3) */
  inactiveZoneOpacity: 0.35,
  /** 開始地点から第 1 次の中心へつなぐ線の太さ */
  routeStrokeWidth: 4,
  /** 管理の画面で、直している収縮候補地の円の破線(マスタの管理 3.3) */
  editingDash: [12, 8],
  /** 管理の画面で、非表示の行の印の点線と不透明度(マスタの管理 3.3) */
  hiddenDash: [3, 4],
  hiddenOpacity: 0.6,
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
