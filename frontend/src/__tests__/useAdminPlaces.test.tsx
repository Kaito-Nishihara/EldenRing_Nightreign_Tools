import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import { ApiError } from "@/lib/apiError";
import { ADMIN_KEY_STORAGE_KEY, saveAdminKey } from "@/lib/adminKey";
import type { AdminPlaceKind } from "@/types";

// 管理用の API は差し替える
const adminListPlaces = vi.fn();
const adminCreatePlace = vi.fn();
const adminUpdatePlace = vi.fn();
vi.mock("@/lib/api", () => ({
  adminListPlaces: (...args: unknown[]) => adminListPlaces(...args),
  adminCreatePlace: (...args: unknown[]) => adminCreatePlace(...args),
  adminUpdatePlace: (...args: unknown[]) => adminUpdatePlace(...args),
}));

import { useAdminPlaces } from "@/hooks/useAdminPlaces";

const KEY = "correct-admin-key-0001";
const north = { id: 4, nameJa: "最北", iconPath: "/icons/zones/north.webp", centerX: 583, centerY: 297, radius: 255, isHidden: false };

/** 収縮候補地だけ「最北」の 1 件があり、ほかの種類は空 */
function listWithNorth(kind: AdminPlaceKind) {
  return Promise.resolve(kind === "shrink-zones" ? [north] : []);
}

beforeEach(() => {
  adminListPlaces.mockReset().mockImplementation(listWithNorth);
  adminCreatePlace.mockReset();
  adminUpdatePlace.mockReset();
  sessionStorage.clear();
});

async function loaded() {
  const hook = renderHook(() => useAdminPlaces(KEY));
  await waitFor(() => expect(hook.result.current.places["shrink-zones"]).toHaveLength(1));
  return hook;
}

describe("useAdminPlaces(マスタの管理 3.3、4.2)", () => {
  it("鍵を付けて 4 種類の一覧を読み、画面の形にする", async () => {
    const { result } = await loaded();

    expect(adminListPlaces).toHaveBeenCalledTimes(4);
    expect(adminListPlaces).toHaveBeenCalledWith("base-points", KEY);
    expect(result.current.places["shrink-zones"][0]).toMatchObject({ id: 4, x: 583, y: 297, radius: 255 });
  });

  it("登録すると、作った行を一覧に足す", async () => {
    adminCreatePlace.mockResolvedValue({ id: 1001, nameJa: "最南", iconPath: null, centerX: 500, centerY: 900, radius: 150, isHidden: false });
    const { result } = await loaded();

    await act(() =>
      result.current.save("shrink-zones", null, { nameJa: "最南", x: 500, y: 900, radius: 150, baseKind: "major", mapId: "normal", iconPath: null })
    );

    expect(adminCreatePlace).toHaveBeenCalledWith(
      "shrink-zones",
      expect.objectContaining({ centerX: 500, centerY: 900, radius: 150 }),
      KEY
    );
    expect(result.current.places["shrink-zones"].map((p) => p.id)).toEqual([4, 1001]);
  });

  it("AC-ADM-08: 非表示にすると、登録済みの値のまま isHidden を真にして送り、一覧の行を非表示にする", async () => {
    adminUpdatePlace.mockResolvedValue({ ...north, isHidden: true });
    const { result } = await loaded();

    await act(() => result.current.setHidden("shrink-zones", result.current.places["shrink-zones"][0], true));

    expect(adminUpdatePlace).toHaveBeenCalledWith(
      "shrink-zones",
      4,
      { nameJa: "最北", iconPath: "/icons/zones/north.webp", centerX: 583, centerY: 297, radius: 255, isHidden: true },
      KEY
    );
    expect(result.current.places["shrink-zones"][0].isHidden).toBe(true);
  });

  it("保存で 400 が返ると、サービスの文言を出し、一覧を変えない", async () => {
    adminUpdatePlace.mockRejectedValue(new ApiError(400, "0 から 1024 の数を入れてください"));
    const { result } = await loaded();

    await act(() =>
      result.current.save("shrink-zones", 4, { nameJa: "最北", x: 583, y: 297, radius: 255, baseKind: "major", mapId: "normal", iconPath: null })
    );

    expect(result.current.actionError).toBe("0 から 1024 の数を入れてください");
    expect(result.current.places["shrink-zones"][0].x).toBe(583);
  });

  it("途中で 401 が返ると、sessionStorage の鍵を消す", async () => {
    saveAdminKey(KEY);
    adminUpdatePlace.mockRejectedValue(new ApiError(401, "この操作は許可されていません"));
    const { result } = await loaded();

    await act(() => result.current.setHidden("shrink-zones", result.current.places["shrink-zones"][0], true));

    expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBeNull();
    expect(result.current.actionError).toBeNull();
  });

  it("一覧を読めないときは誤りを出し、やり直すと読み直す", async () => {
    adminListPlaces.mockRejectedValueOnce(new ApiError(null, "サーバーに接続できません。時間をおいてやり直してください"));
    const { result } = renderHook(() => useAdminPlaces(KEY));
    await waitFor(() => expect(result.current.error).toBe("サーバーに接続できません。時間をおいてやり直してください"));

    act(() => result.current.retry());

    await waitFor(() => expect(result.current.places["shrink-zones"]).toHaveLength(1));
    expect(result.current.error).toBeNull();
  });
});
