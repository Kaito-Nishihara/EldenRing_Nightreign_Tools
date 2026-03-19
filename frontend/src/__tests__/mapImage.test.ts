import { describe, it, expect } from "vitest";
import { getMapImagePath } from "@/lib/mapImage";

describe("getMapImagePath", () => {
  it("地変未選択時はデフォルトマップを返す", () => {
    expect(getMapImagePath(null)).toBe("/map/Default.webp");
  });

  it("火口を選択するとjihen-kakouを返す", () => {
    expect(getMapImagePath("kakou")).toBe("/map/jihen-kakou.webp");
  });

  it("山寒を選択するとjihen-sankanを返す", () => {
    expect(getMapImagePath("sankan")).toBe("/map/jihen-sankan.webp");
  });

  it("腐敗を選択するとjihen-fuhaiを返す", () => {
    expect(getMapImagePath("fuhai")).toBe("/map/jihen-fuhai.webp");
  });

  it("ノクラテオを選択するとjihen-nokurateouを返す", () => {
    expect(getMapImagePath("nokurateou")).toBe("/map/jihen-nokurateou.webp");
  });

  it("未知の地変IDはデフォルトにフォールバック", () => {
    expect(getMapImagePath("unknown_jihen")).toBe("/map/Default.webp");
  });

  it("空文字はデフォルトマップを返す", () => {
    expect(getMapImagePath("")).toBe("/map/Default.webp");
  });
});
