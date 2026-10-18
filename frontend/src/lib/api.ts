import axios from "axios";
import { toApiError } from "@/lib/apiError";
import type {
  BasePoint,
  Boss,
  Jihen,
  NightBoss,
  RelicEffect,
  ShrinkZone,
  SecondShrinkPoint,
  SpawnPoint,
  PatternResponse,
  CreatePlayRecordRequest,
  CreatePlayRecordResponse,
} from "@/types";

/**
 * APIクライアント
 * Gateway経由で全てのAPIリクエストを行う
 */
const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080",
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10000,
});

/**
 * レスポンスインターセプター
 * 誤りを、画面に出してよい文言を持つ ApiError に変える(画面の共通仕様 3.10)。
 * 画面とフックは、受け取った誤りの message をそのまま出せばよい。
 */
apiClient.interceptors.response.use(
  (response) => response,
  (error: unknown) => {
    const apiError = toApiError(error);
    console.error(`APIエラー [${apiError.status ?? "通信の失敗"}]:`, apiError.message);
    return Promise.reject(apiError);
  }
);

/**
 * ボス一覧を取得する
 */
export async function getBosses(): Promise<Boss[]> {
  const response = await apiClient.get<Boss[]>("/api/v1/bosses");
  return response.data;
}

/**
 * 地変一覧を取得する
 */
export async function getJihen(): Promise<Jihen[]> {
  const response = await apiClient.get<Jihen[]>("/api/v1/jihen");
  return response.data;
}

/**
 * 収縮候補地一覧を取得する
 */
export async function getShrinkZones(): Promise<ShrinkZone[]> {
  const response = await apiClient.get<ShrinkZone[]>("/api/v1/shrink-zones");
  return response.data;
}

/**
 * 開始地点一覧を取得する
 */
export async function getSpawnPoints(): Promise<SpawnPoint[]> {
  const response = await apiClient.get<SpawnPoint[]>("/api/v1/spawn-points");
  return response.data;
}

/**
 * パターンを検索する
 * @param bossId - ボスID
 * @param spawnPointId - 開始地点ID
 * @param jihenId - 地変ID（任意）
 */
export async function getPatterns(
  bossId: string,
  spawnPointId: number,
  jihenId?: string
): Promise<PatternResponse> {
  const params: Record<string, string | number> = {
    bossId,
    spawnPointId,
  };
  if (jihenId) {
    params.jihenId = jihenId;
  }
  const response = await apiClient.get<PatternResponse>("/api/v1/patterns", {
    params,
  });
  return response.data;
}

/**
 * プレイ記録を保存する(プレイ記録 4 章)。受け持ちは record-service
 */
export async function createPlayRecord(request: CreatePlayRecordRequest): Promise<CreatePlayRecordResponse> {
  const response = await apiClient.post<CreatePlayRecordResponse>("/api/v1/play-records", request);
  return response.data;
}

// ==========================================
// 管理画面用API
// ==========================================

/** 管理用: 開始地点を登録する */
export async function adminCreateSpawnPoint(
  data: Omit<SpawnPoint, "id">
): Promise<SpawnPoint> {
  const response = await apiClient.post<SpawnPoint>(
    "/api/v1/admin/spawn-points",
    data
  );
  return response.data;
}

/** 管理用: 開始地点を更新する */
export async function adminUpdateSpawnPoint(
  id: number,
  data: Omit<SpawnPoint, "id">
): Promise<SpawnPoint> {
  const response = await apiClient.put<SpawnPoint>(
    `/api/v1/admin/spawn-points/${id}`,
    data
  );
  return response.data;
}

/** 管理用: 開始地点を削除する */
export async function adminDeleteSpawnPoint(id: number): Promise<void> {
  await apiClient.delete(`/api/v1/admin/spawn-points/${id}`);
}

/** 管理用: 収縮候補地を登録する */
export async function adminCreateShrinkZone(
  data: Omit<ShrinkZone, "id">
): Promise<ShrinkZone> {
  const response = await apiClient.post<ShrinkZone>(
    "/api/v1/admin/shrink-zones",
    data
  );
  return response.data;
}

/** 管理用: 収縮候補地を更新する */
export async function adminUpdateShrinkZone(
  id: number,
  data: Omit<ShrinkZone, "id">
): Promise<ShrinkZone> {
  const response = await apiClient.put<ShrinkZone>(
    `/api/v1/admin/shrink-zones/${id}`,
    data
  );
  return response.data;
}

/** 管理用: 収縮候補地を削除する */
export async function adminDeleteShrinkZone(id: number): Promise<void> {
  await apiClient.delete(`/api/v1/admin/shrink-zones/${id}`);
}

/**
 * 夜ボス一覧を取得する
 */
export async function getNightBosses(): Promise<NightBoss[]> {
  const response = await apiClient.get<NightBoss[]>("/api/v1/night-bosses");
  return response.data;
}

/** 第二収縮地点の一覧を取得する(プレイヤー向け。マスタの管理 4.1) */
export async function getSecondShrinkPoints(): Promise<SecondShrinkPoint[]> {
  const response = await apiClient.get<SecondShrinkPoint[]>(
    "/api/v1/second-shrink-points"
  );
  return response.data;
}

/** 管理用: 第二収縮地点候補を登録する */
export async function adminCreateSecondShrinkPoint(
  data: Omit<SecondShrinkPoint, "id">
): Promise<SecondShrinkPoint> {
  const response = await apiClient.post<SecondShrinkPoint>(
    "/api/v1/admin/second-shrink-points",
    data
  );
  return response.data;
}

/** 管理用: 第二収縮地点候補を更新する */
export async function adminUpdateSecondShrinkPoint(
  id: number,
  data: Omit<SecondShrinkPoint, "id">
): Promise<SecondShrinkPoint> {
  const response = await apiClient.put<SecondShrinkPoint>(
    `/api/v1/admin/second-shrink-points/${id}`,
    data
  );
  return response.data;
}

/** 管理用: 第二収縮地点候補を削除する */
export async function adminDeleteSecondShrinkPoint(id: number): Promise<void> {
  await apiClient.delete(`/api/v1/admin/second-shrink-points/${id}`);
}

/**
 * 拠点候補地一覧を取得する（マップIDで絞り込み）
 * @param mapId - マップ識別子（デフォルト: normal）
 */
export async function getBasePoints(mapId: string = "normal"): Promise<BasePoint[]> {
  const response = await apiClient.get<BasePoint[]>("/api/v1/base-points", {
    params: { mapId },
  });
  return response.data;
}

/** 管理用: 拠点候補地を登録する */
export async function adminCreateBasePoint(
  data: Omit<BasePoint, "id">
): Promise<BasePoint> {
  const response = await apiClient.post<BasePoint>(
    "/api/v1/admin/base-points",
    data
  );
  return response.data;
}

/** 管理用: 拠点候補地を更新する */
export async function adminUpdateBasePoint(
  id: number,
  data: Omit<BasePoint, "id">
): Promise<BasePoint> {
  const response = await apiClient.put<BasePoint>(
    `/api/v1/admin/base-points/${id}`,
    data
  );
  return response.data;
}

/** 管理用: 拠点候補地を削除する */
export async function adminDeleteBasePoint(id: number): Promise<void> {
  await apiClient.delete(`/api/v1/admin/base-points/${id}`);
}

/**
 * 遺物効果マスタ一覧を取得する
 */
export async function getRelicEffects(): Promise<RelicEffect[]> {
  const response = await apiClient.get<RelicEffect[]>("/api/v1/relic-effects");
  return response.data;
}

export default apiClient;
