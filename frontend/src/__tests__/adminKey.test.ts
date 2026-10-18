import { describe, it, expect, vi, beforeEach } from "vitest";
import { ADMIN_KEY_STORAGE_KEY, clearAdminKey, readAdminKey, saveAdminKey, subscribeAdminKey } from "@/lib/adminKey";

beforeEach(() => {
  sessionStorage.clear();
});

describe("lib/adminKey の鍵の置き場(マスタの管理 3.1)", () => {
  it("置いた鍵は sessionStorage の nightreign_admin_key に入り、読み出せる", () => {
    saveAdminKey("correct-admin-key-0001");

    expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBe("correct-admin-key-0001");
    expect(readAdminKey()).toBe("correct-admin-key-0001");
  });

  it("AC-ADM-17: 鍵を消すと sessionStorage から消え、読み出すと null になる", () => {
    saveAdminKey("correct-admin-key-0001");

    clearAdminKey();

    expect(sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY)).toBeNull();
    expect(readAdminKey()).toBeNull();
  });

  it("置いたときと消したときに、見ている側へ知らせる。止めた後は知らせない", () => {
    const listener = vi.fn();
    const unsubscribe = subscribeAdminKey(listener);

    saveAdminKey("correct-admin-key-0001");
    clearAdminKey();
    unsubscribe();
    saveAdminKey("correct-admin-key-0001");

    expect(listener).toHaveBeenCalledTimes(2);
  });

  it("sessionStorage が使えないときは、鍵がないものとして扱う", () => {
    const spy = vi.spyOn(Storage.prototype, "getItem").mockImplementation(() => {
      throw new Error("blocked");
    });

    expect(readAdminKey()).toBeNull();
    spy.mockRestore();
  });
});
