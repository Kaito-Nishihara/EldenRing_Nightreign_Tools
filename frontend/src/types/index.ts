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

/** 開始地点 */
export interface SpawnPoint {
  id: number;
  nameJa: string;
  x: number;
  y: number;
  nearbyBaseType: "camp" | "church";
}

/** パターン */
export interface Pattern {
  patternIndex: number;
  recordCount: number;
  lastRecordedAt: string;
  entries: PatternEntry[];
}

/** パターンエントリ（日・フェーズごとの収縮情報） */
export interface PatternEntry {
  day: 1 | 2;
  phase: 1 | 2;
  zone: ShrinkZone;
}

/** パターン検索レスポンス */
export interface PatternResponse {
  totalRecords: number;
  patterns: Pattern[];
}

/** プレイ記録リクエスト */
export interface PlayRecordRequest {
  bossId: string;
  jihenId?: string | null;
  spawnPointId: number;
  note?: string;
  entries: ShrinkEntryRequest[];
}

/** 収縮エントリリクエスト */
export interface ShrinkEntryRequest {
  day: 1 | 2;
  phase: 1 | 2;
  zoneId: number;
}
