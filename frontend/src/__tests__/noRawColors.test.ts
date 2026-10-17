import { describe, it, expect } from "vitest";
import { readdirSync, readFileSync, statSync } from "node:fs";
import { join } from "node:path";

/**
 * AC-UI-01: 部品のファイルに、# で始まる色の値と bg-[ の任意の値を書かない(画面の共通仕様 3.2)。
 * 作り直しを終えたディレクトリから対象にし、画面ごとの PR で広げる(#53)。
 */
const CHECKED_DIRS = ["components/ui", "components/layout", "components/map"];

/** ディレクトリの下の .tsx を集める */
function listTsx(dir: string): string[] {
  return readdirSync(dir).flatMap((name) => {
    const path = join(dir, name);
    if (statSync(path).isDirectory()) return listTsx(path);
    return path.endsWith(".tsx") ? [path] : [];
  });
}

describe("AC-UI-01: 部品に色の値を直接書かない", () => {
  const root = join(__dirname, "..");
  const files = CHECKED_DIRS.flatMap((dir) => listTsx(join(root, dir)));

  it.each(files.map((file) => [file.slice(root.length + 1)]))("%s", (file) => {
    const source = readFileSync(join(root, file), "utf8");

    expect(source).not.toMatch(/#[0-9a-fA-F]{3,8}\b/);
    expect(source).not.toMatch(/bg-\[/);
  });
});
