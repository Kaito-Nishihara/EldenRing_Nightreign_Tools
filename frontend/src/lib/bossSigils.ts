import type { Boss } from "@/types";

/**
 * 夜の王のボタンに添える頭文字(パターン検索 3.1、画面の試作)。
 * 名前の最初の文字だと「三」「喰」のように見分けにくいので、名前を表す 1 文字を決めておく。
 */
const SIGILS: Record<string, string> = {
  gladius: "獣",
  edere: "顎",
  gnostor: "蟲",
  maris: "兆",
  libra: "律",
  fulgor: "狩",
  caligo: "霧",
  nameless: "夜",
  harmonia: "娘",
  strages: "逆",
};

/** 頭文字。決めていない夜の王は、名前の最初の文字にする */
export function bossSigil(boss: Pick<Boss, "id" | "nameJa">): string {
  return SIGILS[boss.id] ?? boss.nameJa.charAt(0);
}
