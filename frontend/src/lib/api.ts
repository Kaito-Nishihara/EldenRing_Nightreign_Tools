import axios from "axios";
import type {
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
  const response = await apiClient.get<Boss[]>("/api/bosses");
  return response.data;
}

/**
 * 地変一覧を取得する
 */
export async function getJihen(): Promise<Jihen[]> {
  const response = await apiClient.get<Jihen[]>("/api/jihen");
  return response.data;
}

/**
 * 収縮候補地一覧を取得する
 */
export async function getShrinkZones(): Promise<ShrinkZone[]> {
  const response = await apiClient.get<ShrinkZone[]>("/api/shrink-zones");
  return response.data;
}

/**
 * 開始地点一覧を取得する
 */
export async function getSpawnPoints(): Promise<SpawnPoint[]> {
  const response = await apiClient.get<SpawnPoint[]>("/api/spawn-points");
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
  const response = await apiClient.get<PatternResponse>("/api/patterns", {
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
    "/api/play-records",
    request
  );
  return response.data;
}

export default apiClient;
