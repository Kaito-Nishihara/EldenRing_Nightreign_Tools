import { describe, it, expect } from "vitest";
import { getPatternColor } from "@/lib/theme";

describe("getPatternColor", () => {
  it("インデックス0は青を返す", () => {
    expect(getPatternColor(0)).toBe("#4477AA");
  });

  it("インデックス1はローズを返す", () => {
    expect(getPatternColor(1)).toBe("#EE6677");
  });

  it("各インデックスが異なる色を返す（8色パレット）", () => {
    const colors = new Set<string>();
    for (let i = 0; i < 8; i++) {
      colors.add(getPatternColor(i));
    }
    expect(colors.size).toBe(8);
  });

  it("インデックスがパレットサイズを超えるとループする", () => {
    expect(getPatternColor(8)).toBe(getPatternColor(0));
    expect(getPatternColor(9)).toBe(getPatternColor(1));
    expect(getPatternColor(16)).toBe(getPatternColor(0));
  });

  it("負のインデックスでもエラーにならない", () => {
    expect(() => getPatternColor(-1)).not.toThrow();
  });
});
