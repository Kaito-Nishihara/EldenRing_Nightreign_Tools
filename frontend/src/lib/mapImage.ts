/**
 * 地変IDからマップ画像パスを解決する
 * 地変未選択時はデフォルトマップを返す
 */

/**
 * 地変ID→マップ画像ファイル名のマッピング。
 * 鍵は API が返す地変の識別子(データベースの共通仕様 5.6)。大空洞(great_cavern)は通常のマップを使う
 */
const JIHEN_MAP: Record<string, string> = {
  crater: "jihen-kakou.webp",
  mountain: "jihen-sankan.webp",
  rotted_forest: "jihen-fuhai.webp",
  noklateo: "jihen-nokurateou.webp",
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
