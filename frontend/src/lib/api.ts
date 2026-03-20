import axios from "axios";
import type {
  BasePoint,
  Boss,
  Jihen,
  ShrinkZone,
  SpawnPoint,
  PatternResponse,
  PlayRecordRequest,
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
 * エラーハンドリングを共通化する
 */
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // ネットワークエラーやタイムアウトの場合
    if (!error.response) {
      console.error("ネットワークエラーが発生しました:", error.message);
    } else {
      console.error(
        `APIエラー [${error.response.status}]:`,
        error.response.data?.message || error.message
      );
    }
    return Promise.reject(error);
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
 * プレイ記録を作成する
 * @param request - プレイ記録リクエスト
 */
export async function createPlayRecord(
  request: PlayRecordRequest
): Promise<{ id: number; createdAt: string }> {
  const response = await apiClient.post<{ id: number; createdAt: string }>(
    "/api/v1/play-records",
    request
  );
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
 * 拠点候補地一覧を取得する
 */
export async function getBasePoints(): Promise<BasePoint[]> {
  const response = await apiClient.get<BasePoint[]>("/api/v1/base-points");
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

export default apiClient;
