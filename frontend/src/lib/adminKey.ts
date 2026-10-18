// 管理用の鍵の置き場(マスタの管理 3.1)。
// 鍵は端末の sessionStorage に置き、タブを閉じると消える。
// ヘッダーの「鍵を消す」と管理の画面が同じ鍵を見るので、変わったことを知らせる仕組みを持つ。
// 鍵を置いても、権限の判定は Gateway が行う(決定の記録 0009)。

export const ADMIN_KEY_STORAGE_KEY = "nightreign_admin_key";

const listeners = new Set<() => void>();

function notify(): void {
  listeners.forEach((listener) => listener());
}

/** 置いてある鍵。ないときと、sessionStorage が使えないときは null */
export function readAdminKey(): string | null {
  try {
    const key = sessionStorage.getItem(ADMIN_KEY_STORAGE_KEY);
    return key && key.length > 0 ? key : null;
  } catch {
    return null;
  }
}

export function saveAdminKey(key: string): void {
  try {
    sessionStorage.setItem(ADMIN_KEY_STORAGE_KEY, key);
  } catch {
    // 置けないときは、この画面を開いている間だけ使えなくなる。知らせは出さない
  }
  notify();
}

export function clearAdminKey(): void {
  try {
    sessionStorage.removeItem(ADMIN_KEY_STORAGE_KEY);
  } catch {
    // 消せないときも、鍵の入力の欄に戻す
  }
  notify();
}

/** 鍵が変わったら呼ぶ。useSyncExternalStore に渡す形で、止める関数を返す */
export function subscribeAdminKey(listener: () => void): () => void {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
}
