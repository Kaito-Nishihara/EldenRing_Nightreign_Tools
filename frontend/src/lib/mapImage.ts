/**
 * 地変IDからマップ画像パスを解決する
 * 地変未選択時はデフォルトマップを返す
 */

/** 地変ID→マップ画像ファイル名のマッピング */
const JIHEN_MAP: Record<string, string> = {
  kakou: "jihen-kakou.webp",
  sankan: "jihen-sankan.webp",
  fuhai: "jihen-fuhai.webp",
  nokurateou: "jihen-nokurateou.webp",
};

/**
 * 地変IDに対応するマップ画像パスを返す
 * @param jihenId - 地変ID（null=通常マップ）
 * @returns マップ画像のパス
 */
export function getMapImagePath(jihenId: string | null): string {
  if (!jihenId) return "/map/Default.webp";
  return `/map/${JIHEN_MAP[jihenId] ?? "Default.webp"}`;
}
