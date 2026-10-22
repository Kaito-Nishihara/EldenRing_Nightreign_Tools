import { describe, it, expect } from "vitest";
import {
  ADMIN_PLACE_KINDS,
  BASE_POINT_KIND_LABELS,
  COORDINATE_MESSAGE,
  NAME_REQUIRED_MESSAGE,
  NAME_TOO_LONG_MESSAGE,
  RADIUS_MESSAGE,
  emptyInput,
  formTitle,
  inputFromPlace,
  pickedCoordinate,
  placesOnMap,
  validateInput,
  type AdminPlace,
} from "@/lib/adminPlaces";

const zones = ADMIN_PLACE_KINDS["shrink-zones"];
const bases = ADMIN_PLACE_KINDS["base-points"];

/** 収縮候補地「最北」を、画面の形にしたもの */
const north: AdminPlace = zones.fromDto({
  id: 4,
  nameJa: "最北",
  iconPath: "/icons/zones/north.webp",
  centerX: 583,
  centerY: 297,
  radius: 255,
  isHidden: false,
});

describe("lib/adminPlaces のマップを押した座標(マスタの管理 3.3)", () => {
  it("AC-ADM-07: 押した場所の座標を小数 2 桁に丸める", () => {
    expect(pickedCoordinate(583.456)).toBe(583.46);
    expect(pickedCoordinate(297.004)).toBe(297);
  });

  it("座標空間の外を押したときは、0 か 1024 に収める", () => {
    expect(pickedCoordinate(-3.2)).toBe(0);
    expect(pickedCoordinate(1030)).toBe(1024);
  });

  it("AC-ADM-07: 登録済みの場所を直すときは、入力の欄に小数 2 桁で出す", () => {
    expect(inputFromPlace(north)).toEqual({ nameJa: "最北", x: "583.00", y: "297.00", radius: 255, baseKind: "major" });
  });
});

describe("lib/adminPlaces の入力の検証(マスタの管理 3.4)", () => {
  it("正しい入力は、名前の前後の空白を除いて送れる値にする", () => {
    const { values, errors } = validateInput(zones, { ...emptyInput(), nameJa: " 最北 ", x: "583.5", y: "297", radius: 150 });

    expect(errors).toEqual({});
    expect(values).toEqual({ nameJa: "最北", x: 583.5, y: 297, radius: 150, baseKind: "major" });
  });

  it("名前が空なら「名前を入れてください」を出す。拠点候補地では名前を省ける", () => {
    expect(validateInput(zones, { ...emptyInput(), x: "1", y: "1" }).errors.nameJa).toBe(NAME_REQUIRED_MESSAGE);
    expect(validateInput(bases, { ...emptyInput(), x: "1", y: "1" }).values?.nameJa).toBeNull();
  });

  it("名前が 101 文字なら、文字数の誤りを出す", () => {
    expect(validateInput(zones, { ...emptyInput(), nameJa: "あ".repeat(101), x: "1", y: "1" }).errors.nameJa).toBe(
      NAME_TOO_LONG_MESSAGE
    );
  });

  it.each(["1025", "-1", "1.234", "abc", "", "1e2"])("X に %s を入れると「0 から 1024 の数を入れてください」を出す", (x) => {
    const { values, errors } = validateInput(zones, { ...emptyInput(), nameJa: "最北", x, y: "1" });

    expect(values).toBeNull();
    expect(errors.x).toBe(COORDINATE_MESSAGE);
  });

  it("収縮候補地の半径が 10 より小さいと、半径の誤りを出す。ほかの種類では半径を見ない", () => {
    expect(validateInput(zones, { ...emptyInput(), nameJa: "最北", x: "1", y: "1", radius: 9 }).errors.radius).toBe(RADIUS_MESSAGE);
    expect(validateInput(ADMIN_PLACE_KINDS["spawn-points"], { ...emptyInput(), nameJa: "北", x: "1", y: "1", radius: 9 }).errors).toEqual({});
  });
});

describe("lib/adminPlaces の種類ごとの違い(マスタの管理 3.2、4.2)", () => {
  it("収縮候補地は中心を centerX と centerY で送り、アイコンのパスを送り返す", () => {
    const request = zones.toRequest({
      nameJa: "最北",
      x: 583,
      y: 297,
      radius: 150,
      baseKind: "major",
      mapId: "normal",
      iconPath: north.iconPath,
    });

    expect(request).toEqual({
      nameJa: "最北",
      iconPath: "/icons/zones/north.webp",
      centerX: 583,
      centerY: 297,
      radius: 150,
      isHidden: undefined,
    });
  });

  it("isHidden を省くと、送る本文に入らない", () => {
    const body = JSON.stringify(
      ADMIN_PLACE_KINDS["spawn-points"].toRequest({ nameJa: "北", x: 1, y: 2, radius: 255, baseKind: "major", mapId: "normal", iconPath: null })
    );

    expect(JSON.parse(body)).toEqual({ nameJa: "北", x: 1, y: 2 });
  });

  it("拠点候補地は、選んだマップの行だけを出す", () => {
    const places = [
      bases.fromDto({ id: 1, mapId: "normal", kind: "major", x: 1, y: 1, isHidden: false }),
      bases.fromDto({ id: 2, mapId: "crater", kind: "major", x: 1, y: 1, isHidden: false }),
    ];

    expect(placesOnMap(bases, places, "crater").map((p) => p.id)).toEqual([2]);
    expect(placesOnMap(zones, [north], "crater")).toEqual([north]);
  });

  it("直しているときの題は「収縮候補地「最北」を直す」の形。名前のない拠点候補地は番号で出す", () => {
    expect(formTitle(zones, north)).toBe("収縮候補地「最北」を直す");
    expect(formTitle(zones, null)).toBe("収縮候補地を登録する");
    expect(formTitle(bases, bases.fromDto({ id: 7, mapId: "normal", kind: "minor", x: 1, y: 1, isHidden: false }))).toBe(
      "拠点候補地「7 番」を直す"
    );
  });

  it("AC-ADM-24: 拠点候補地は種類を kind で受け取り、kind で送る", () => {
    const place = bases.fromDto({ id: 28, mapId: "normal", kind: "field_boss", x: 247, y: 198, isHidden: false });

    expect(place.baseKind).toBe("field_boss");
    expect(inputFromPlace(place).baseKind).toBe("field_boss");
    expect(bases.toRequest({ nameJa: null, x: 247, y: 198, radius: 255, baseKind: "field_boss", mapId: "normal", iconPath: null })).toEqual({
      mapId: "normal",
      kind: "field_boss",
      nameJa: null,
      x: 247,
      y: 198,
      isHidden: undefined,
    });
  });

  it("新しく登録するときの種類は大拠点にする", () => {
    expect(emptyInput().baseKind).toBe("major");
  });

  it("AC-ADM-30: 大拠点は「大」、小拠点は「小」、強敵は「強」の丸で出す", () => {
    expect([BASE_POINT_KIND_LABELS.major.mark, BASE_POINT_KIND_LABELS.minor.mark, BASE_POINT_KIND_LABELS.field_boss.mark]).toEqual([
      "大",
      "小",
      "強",
    ]);
  });
});
