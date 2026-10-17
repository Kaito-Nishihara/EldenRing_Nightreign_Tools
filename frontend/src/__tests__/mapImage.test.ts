import { describe, it, expect } from "vitest";
import { getMapImagePath } from "@/lib/mapImage";

describe("getMapImagePath", () => {
  it("地変未選択時はデフォルトマップを返す", () => {
    expect(getMapImagePath(null)).toBe("/map/Default.webp");
  });

  it("火口(crater)を選択するとjihen-kakouを返す", () => {
    expect(getMapImagePath("crater")).toBe("/map/jihen-kakou.webp");
  });

  it("山嶺(mountain)を選択するとjihen-sankanを返す", () => {
    expect(getMapImagePath("mountain")).toBe("/map/jihen-sankan.webp");
  });

  it("腐れ森(rotted_forest)を選択するとjihen-fuhaiを返す", () => {
    expect(getMapImagePath("rotted_forest")).toBe("/map/jihen-fuhai.webp");
  });

  it("隠れ都ノクラテオ(noklateo)を選択するとjihen-nokurateouを返す", () => {
    expect(getMapImagePath("noklateo")).toBe("/map/jihen-nokurateou.webp");
  });

  it("大空洞(great_cavern)は通常のマップを返す", () => {
    expect(getMapImagePath("great_cavern")).toBe("/map/Default.webp");
  });

  it("未知の地変IDはデフォルトにフォールバック", () => {
    expect(getMapImagePath("unknown_jihen")).toBe("/map/Default.webp");
  });

  it("空文字はデフォルトマップを返す", () => {
    expect(getMapImagePath("")).toBe("/map/Default.webp");
  });
});
