import { describe, it, expect } from "vitest";
import { readFileSync } from "node:fs";
import { join } from "node:path";
import { THEME } from "@/lib/theme";

/** "nightRaised" を "night-raised" に直す */
function toCssName(key: string): string {
  return key.replace(/[A-Z]/g, (c) => `-${c.toLowerCase()}`);
}

describe("lib/theme.ts(画面の共通仕様 3.2)", () => {
  it("THEME の色は、globals.css の @theme の同じ名前の色と同じ値である", () => {
    const css = readFileSync(join(__dirname, "../app/globals.css"), "utf8");

    for (const [key, value] of Object.entries(THEME)) {
      const match = css.match(new RegExp(`--color-${toCssName(key)}:\\s*(#[0-9a-fA-F]{6});`));
      expect(match, `--color-${toCssName(key)} が globals.css にない`).not.toBeNull();
      expect(match?.[1].toLowerCase()).toBe(value.toLowerCase());
    }
  });
});
