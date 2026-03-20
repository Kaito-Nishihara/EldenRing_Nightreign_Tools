import type { BaseType } from "@/types";

/**
 * 拠点タイプ一覧
 * 各拠点候補地に出現しうる拠点の種類と画像パス
 */
export const BASE_TYPES: BaseType[] = [
  { id: "camp", nameJa: "野営地", imagePath: "/images/zones/Camp.webp" },
  { id: "church", nameJa: "教会", imagePath: "/images/zones/Church.webp" },
  { id: "fort", nameJa: "砦", imagePath: "/images/zones/Fort.webp" },
  { id: "great-church", nameJa: "大教会", imagePath: "/images/zones/GreatChurch.webp" },
  { id: "ruins", nameJa: "遺跡", imagePath: "/images/zones/Ruins.webp" },
  { id: "sorcerers-rise", nameJa: "魔術師の塔", imagePath: "/images/zones/SorcerersRise.webp" },
  { id: "township", nameJa: "城塞町", imagePath: "/images/zones/Township.webp" },
  { id: "evergaol", nameJa: "封牢", imagePath: "/images/zones/evergaol.webp" },
  { id: "field-boss", nameJa: "フィールドボス", imagePath: "/images/zones/field-boss.webp" },
  { id: "red-boss", nameJa: "赤ボス", imagePath: "/images/zones/redBoss.webp" },
];

/** IDから拠点タイプを取得する */
export function getBaseType(id: string): BaseType | undefined {
  return BASE_TYPES.find((t) => t.id === id);
}
