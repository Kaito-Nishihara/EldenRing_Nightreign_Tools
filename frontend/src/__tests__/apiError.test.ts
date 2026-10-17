import { describe, it, expect } from "vitest";
import { AxiosError, AxiosHeaders, type AxiosResponse } from "axios";
import {
  ApiError,
  toApiError,
  CONNECTION_ERROR_MESSAGE,
  SERVER_ERROR_MESSAGE,
  FORBIDDEN_MESSAGE,
} from "@/lib/apiError";

/** 指定した状態と本文の応答を持つ axios の誤りを作る */
function responseError(status: number, data: unknown): AxiosError {
  const headers = new AxiosHeaders();
  const response = { status, data, statusText: "", headers, config: { headers } } as AxiosResponse;
  return new AxiosError("Request failed", "ERR_BAD_RESPONSE", { headers }, null, response);
}

describe("toApiError(画面の共通仕様 3.10)", () => {
  it("AC-UI-10: 404 は応答の message をそのまま出す", () => {
    const error = toApiError(responseError(404, { status: 404, message: "bossId 'x' は存在しません" }));

    expect(error).toBeInstanceOf(ApiError);
    expect(error.status).toBe(404);
    expect(error.message).toBe("bossId 'x' は存在しません");
  });

  it("400 と 409 も応答の message をそのまま出す", () => {
    expect(toApiError(responseError(400, { message: "day=1, phase=1 が重複しています" })).message).toBe(
      "day=1, phase=1 が重複しています",
    );
    expect(toApiError(responseError(409, { message: "食い違い" })).message).toBe("食い違い");
  });

  it("401 と 403 は「この操作は許可されていません」を出す", () => {
    expect(toApiError(responseError(401, { message: "サービス間キーがないか、誤っています" })).message).toBe(FORBIDDEN_MESSAGE);
    expect(toApiError(responseError(403, {})).message).toBe(FORBIDDEN_MESSAGE);
  });

  it("500 は中身を出さず、決まった文言を出す", () => {
    expect(toApiError(responseError(500, { message: "内部サーバーエラーが発生しました" })).message).toBe(SERVER_ERROR_MESSAGE);
  });

  it("AC-UI-09: 502 と 504 は「サーバーに接続できません」を出す", () => {
    expect(toApiError(responseError(502, {})).message).toBe(CONNECTION_ERROR_MESSAGE);
    expect(toApiError(responseError(504, "")).message).toBe(CONNECTION_ERROR_MESSAGE);
  });

  it("503 はサービスの書いた message を出す", () => {
    const error = toApiError(responseError(503, { message: "マスタのサービスに接続できません。時間をおいてやり直してください" }));

    expect(error.message).toBe("マスタのサービスに接続できません。時間をおいてやり直してください");
  });

  it("通信の失敗(応答がない)は「サーバーに接続できません」を出し、status は null", () => {
    const error = toApiError(new AxiosError("Network Error", "ERR_NETWORK"));

    expect(error.status).toBeNull();
    expect(error.message).toBe(CONNECTION_ERROR_MESSAGE);
  });

  it("本文がエラーの形でない 404 でも、空の文言にしない", () => {
    expect(toApiError(responseError(404, "<html>")).message).toBe(SERVER_ERROR_MESSAGE);
  });
});
