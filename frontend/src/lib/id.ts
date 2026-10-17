/**
 * 画面の中だけで使う識別子を作る。
 * crypto.randomUUID は HTTPS か localhost でしか使えないので、スマートフォンから LAN の IP で開いたときは
 * crypto.getRandomValues で同じ形(UUID の版 4)を作る。
 */
export function createId(): string {
  if (typeof crypto.randomUUID === "function") {
    return crypto.randomUUID();
  }

  const bytes = crypto.getRandomValues(new Uint8Array(16));
  // 版 4 と、RFC 4122 の変種の印を立てる
  bytes[6] = (bytes[6] & 0x0f) | 0x40;
  bytes[8] = (bytes[8] & 0x3f) | 0x80;
  const hex = Array.from(bytes, (b) => b.toString(16).padStart(2, "0")).join("");
  return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`;
}
