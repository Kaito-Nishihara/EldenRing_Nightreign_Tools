/** ボス */
export interface Boss {
  id: string;
  nameJa: string;
  nameEn?: string;
  imagePath?: string;
  weaknesses: string[];
  isDlc: boolean;
  unlockCondition?: string;
}

/** 地変 */
export interface Jihen {
  id: string;
  nameJa: string;
  imagePath?: string;
  area: string;
  centerX: number;
  centerY: number;
  isDlc: boolean;
  unlockCondition?: string;
}

/** 収縮候補地 */
export interface ShrinkZone {
  id: number;
  nameJa: string;
  iconPath?: string;
  centerX: number;
  centerY: number;
  radius: number;
}

/** 夜ボス（最終ボス=夜の王とは別） */
export interface NightBoss {
  id: number;
  nameJa: string;
  day: number;
}

/** 第二収縮地点候補（座標のみ管理、夜ボスとの紐付けは記録画面で行う） */
export interface SecondShrinkPoint {
  id: number;
  nameJa: string;
  x: number;
  y: number;
}

/** 開始地点 */
export interface SpawnPoint {
  id: number;
  nameJa: string;
  x: number;
  y: number;
}

/** 拠点候補地（マップ上の固定座標、?マーク） */
export interface BasePoint {
  id: number;
  mapId: string;
  nameJa?: string;
  x: number;
  y: number;
}

/** パターン */
export interface Pattern {
  patternIndex: number;
  recordCount: number;
  lastRecordedAt: string;
  entries: PatternEntry[];
}

/**
 * パターンの収縮先(日と段階ごと)。パターン検索 4 章。
 * 第 1 次は zone(収縮候補地)だけ、第 2 次は secondShrinkPoint(第二収縮地点)だけを持つ(決定の記録 0008)。
 */
export interface PatternEntry {
  day: 1 | 2;
  phase: 1 | 2;
  zone: ShrinkZone | null;
  secondShrinkPoint: SecondShrinkPoint | null;
}

/** パターン検索レスポンス */
export interface PatternResponse {
  totalRecords: number;
  patterns: Pattern[];
}

/** プレイ記録の保存の入力(POST /api/v1/play-records、プレイ記録 4 章) */
export interface CreatePlayRecordRequest {
  bossId: string;
  jihenId: string | null;
  spawnPointId: number;
  day1NightBossId: number | null;
  day2NightBossId: number | null;
  note: string | null;
  entries: CreatePlayRecordEntry[];
}

/**
 * 保存する収縮先。第 1 次は zoneId(収縮候補地)だけ、第 2 次は secondShrinkPointId(第二収縮地点)だけを持つ(決定の記録 0008)。
 */
export interface CreatePlayRecordEntry {
  day: 1 | 2;
  phase: 1 | 2;
  zoneId: number | null;
  secondShrinkPointId: number | null;
}

/** プレイ記録の保存の出力(201) */
export interface CreatePlayRecordResponse {
  id: number;
  createdAt: string;
}

// ==========================================
// ビルドシミュレーター
// ==========================================

/** 遺物効果ランク別数値 */
export interface RelicEffectRank {
  rank: number;
  /** 数で表せない効果(unit が none)では null */
  value: number | null;
  unit: "multiplier" | "percent" | "flat" | "per_sec" | "none";
  hasDemerit: boolean;
}

/** 遺物効果マスタ */
export interface RelicEffect {
  id: number;
  categoryId: string;
  categoryNameJa: string;
  /** 分類の並び順。小さいほうが先(ビルドシミュレーター 3.2) */
  categorySortOrder: number;
  nameJa: string;
  isDeep: boolean;
  effectSign: "positive" | "negative";
  calcType: "multiply" | "percent_add" | "flat_add" | "special";
  attribute: string | null;
  stackable: "yes" | "no" | "conditional";
  rankMin: number;
  rankMax: number;
  conditionJa: string | null;
  notesJa: string | null;
  ranks: RelicEffectRank[];
}

/** ビルドシミュレーター: 遺物スロット内の効果エントリ */
export interface BuildEffectEntry {
  id: string;
  effectId: number | null;
  rank: number;
}

/** ビルドシミュレーター: 遺物セクション（通常3枠 or 深層3枠） */
export interface BuildRelicSlot {
  id: number;
  isDeep: boolean;
  effects: BuildEffectEntry[]; // 固定3枠
}

/** ビルドシミュレーター: 計算結果グループ */
export interface BuildResultGroup {
  key: string;
  nameJa: string;
  categoryId: string;
  categoryNameJa: string;
  calcType: "multiply" | "percent_add" | "flat_add" | "special";
  attribute: string | null;
  unit: string;
  combined: number;
  entries: { effectId: number; nameJa: string; value: number }[];
}
