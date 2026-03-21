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

/** 拠点タイプ定義 */
export interface BaseType {
  id: string;
  nameJa: string;
  imagePath: string;
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
