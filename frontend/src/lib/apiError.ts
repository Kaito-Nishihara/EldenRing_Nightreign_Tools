import axios from "axios";

/**
 * API の誤り。画面に出してよい日本語の文言を message に持つ(画面の共通仕様 3.10)。
 * status は HTTP の状態。通信の失敗と期限切れのときは null。
 */
export class ApiError extends Error {
  readonly status: number | null;

  constructor(status: number | null, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

/** 接続できなかったときの文言 */
export const CONNECTION_ERROR_MESSAGE = "サーバーに接続できません。時間をおいてやり直してください";

/** サーバーの中の誤りの文言 */
export const SERVER_ERROR_MESSAGE = "サーバーで問題が起きました。時間をおいてやり直してください";

/** 権限がないときの文言 */
export const FORBIDDEN_MESSAGE = "この操作は許可されていません";

/** 応答の本文の message を取り出す。本文がエラーの形でなければ null */
function messageOf(data: unknown): string | null {
  if (typeof data === "object" && data !== null && "message" in data) {
    const message = (data as { message: unknown }).message;
    return typeof message === "string" && message.length > 0 ? message : null;
  }
  return null;
}

/**
 * axios の誤りを、状態ごとの文言を持つ ApiError に変える(画面の共通仕様 3.10)。
 * - 400、404、409、429、503: 応答の message をそのまま出す
 * - 401、403: 「この操作は許可されていません」
 * - 500: 「サーバーで問題が起きました…」
 * - 502、504、通信の失敗、期限切れ: 「サーバーに接続できません…」
 */
export function toApiError(error: unknown): ApiError {
  if (!axios.isAxiosError(error) || !error.response) {
    return new ApiError(null, CONNECTION_ERROR_MESSAGE);
  }

  const status = error.response.status;
  const message = messageOf(error.response.data);
  switch (status) {
    case 401:
    case 403:
      return new ApiError(status, FORBIDDEN_MESSAGE);
    case 502:
    case 504:
      return new ApiError(status, CONNECTION_ERROR_MESSAGE);
    case 503:
      // 呼んだ先のサービスが応答しない。サービスが理由を message に書いている
      return new ApiError(status, message ?? CONNECTION_ERROR_MESSAGE);
    default:
      if (status >= 500) {
        return new ApiError(status, SERVER_ERROR_MESSAGE);
      }
      // 400、404、409、429 など。サービスが利用者向けに書いた message を出す
      return new ApiError(status, message ?? SERVER_ERROR_MESSAGE);
  }
}
