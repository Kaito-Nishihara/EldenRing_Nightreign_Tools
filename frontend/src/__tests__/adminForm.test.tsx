import { describe, it, expect, vi, afterEach } from "vitest";
import { render, screen, fireEvent, cleanup, renderHook, act } from "@testing-library/react";
import AdminPlaceForm, { HIDE_DESCRIPTION } from "@/components/admin/AdminPlaceForm";
import AdminPlaceList from "@/components/admin/AdminPlaceList";
import { useAdminEditor } from "@/hooks/useAdminEditor";
import { ADMIN_PLACE_KINDS, COORDINATE_MESSAGE, emptyInput, type AdminPlace } from "@/lib/adminPlaces";

afterEach(() => {
  cleanup();
});

const zones = ADMIN_PLACE_KINDS["shrink-zones"];
const north: AdminPlace = { id: 4, nameJa: "最北", x: 583, y: 297, radius: 255, mapId: null, baseKind: null, iconPath: "/icons/zones/north.webp", isHidden: false };
const east: AdminPlace = { ...north, id: 7, nameJa: "最東", x: 739.5, y: 518, isHidden: true };
const crater: AdminPlace = { id: 30, nameJa: null, x: 100, y: 200, radius: null, mapId: "crater", baseKind: "minor", iconPath: null, isHidden: false };

function renderForm(props: Partial<Parameters<typeof AdminPlaceForm>[0]> = {}) {
  const handlers = { onChange: vi.fn(), onSave: vi.fn(), onCancel: vi.fn(), onHide: vi.fn(), onRestore: vi.fn() };
  render(
    <AdminPlaceForm spec={zones} editing={null} input={emptyInput()} errors={{}} pending={false} actionError={null} {...handlers} {...props} />
  );
  return handlers;
}

describe("AdminPlaceForm(マスタの管理 3.3、3.4)", () => {
  it("新しく登録するときは「非表示にする」を出さない", () => {
    renderForm();

    expect(screen.getByRole("heading", { name: "収縮候補地を登録する" })).toBeTruthy();
    expect(screen.queryByRole("button", { name: "非表示にする" })).toBeNull();
  });

  it("直しているときは題を「収縮候補地「最北」を直す」にし、「非表示にする」を押すと呼ぶ側に知らせる", () => {
    const handlers = renderForm({ editing: north });

    fireEvent.click(screen.getByRole("button", { name: "非表示にする" }));

    expect(screen.getByRole("heading", { name: "収縮候補地「最北」を直す" })).toBeTruthy();
    expect(screen.getByText(`非表示にすると、${HIDE_DESCRIPTION}。`)).toBeTruthy();
    expect(handlers.onHide).toHaveBeenCalledTimes(1);
  });

  it("非表示の行を直しているときは「表示に戻す」を出す", () => {
    const handlers = renderForm({ editing: east });

    fireEvent.click(screen.getByRole("button", { name: "表示に戻す" }));

    expect(handlers.onRestore).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("button", { name: "非表示にする" })).toBeNull();
  });

  it("欄の誤りを欄の下に出し、欄に aria-invalid を付ける", () => {
    renderForm({ errors: { x: COORDINATE_MESSAGE } });

    const x = screen.getByLabelText("X");
    expect(x.getAttribute("aria-invalid")).toBe("true");
    expect(document.getElementById(x.getAttribute("aria-describedby") ?? "")?.textContent).toBe(COORDINATE_MESSAGE);
  });

  it("収縮候補地だけ、100 から 500 の半径のつまみを出す", () => {
    renderForm();
    const slider = screen.getByLabelText("半径 255");
    expect([slider.getAttribute("min"), slider.getAttribute("max")]).toEqual(["100", "500"]);
    cleanup();

    renderForm({ spec: ADMIN_PLACE_KINDS["spawn-points"] });
    expect(screen.queryByLabelText(/半径/)).toBeNull();
  });

  it("拠点候補地だけ、大拠点、小拠点、強敵を選ぶ欄を出し、選ぶと呼ぶ側に知らせる", () => {
    const handlers = renderForm({ spec: ADMIN_PLACE_KINDS["base-points"] });

    const select = screen.getByLabelText("種類");
    expect(Array.from(select.querySelectorAll("option")).map((o) => o.textContent)).toEqual(["大拠点", "小拠点", "強敵"]);
    fireEvent.change(select, { target: { value: "field_boss" } });
    expect(handlers.onChange).toHaveBeenCalledWith({ baseKind: "field_boss" });
    cleanup();

    renderForm();
    expect(screen.queryByLabelText("種類")).toBeNull();
  });
});

describe("AdminPlaceList(マスタの管理 3.3)", () => {
  it("AC-ADM-08: 非表示の行に「非表示」の印を付けて残す", () => {
    render(<AdminPlaceList spec={zones} places={[north, east]} selectedId={null} onSelect={vi.fn()} />);

    expect(screen.getByRole("heading", { name: "登録済み 2 件" })).toBeTruthy();
    expect(screen.getByRole("button", { name: /最東/ }).textContent).toContain("非表示");
    expect(screen.getByRole("button", { name: /最北/ }).textContent).not.toContain("非表示");
    expect(screen.getByText("739.50")).toBeTruthy();
  });

  it("拠点候補地の一覧には種類の名前を出す", () => {
    render(<AdminPlaceList spec={ADMIN_PLACE_KINDS["base-points"]} places={[crater]} selectedId={null} onSelect={vi.fn()} />);

    expect(screen.getByRole("columnheader", { name: "種類" })).toBeTruthy();
    expect(screen.getByText("小拠点")).toBeTruthy();
  });

  it("行の名前を押すと、その行を知らせる", () => {
    const onSelect = vi.fn();
    render(<AdminPlaceList spec={zones} places={[north]} selectedId={null} onSelect={onSelect} />);

    fireEvent.click(screen.getByRole("button", { name: "最北" }));

    expect(onSelect).toHaveBeenCalledWith(north);
  });
});

describe("useAdminEditor(マスタの管理 3.3)", () => {
  it("AC-ADM-07: マップを押すと、X と Y に押した場所の座標を小数 2 桁で入れる", () => {
    const { result } = renderHook(() => useAdminEditor());

    act(() => result.current.pick(583.456, 297.1));

    expect([result.current.input.x, result.current.input.y]).toEqual(["583.46", "297.10"]);
    expect(result.current.preview).toEqual({ x: 583.46, y: 297.1, radius: 255 });
  });

  it("直している行があるときにマップを押すと、新しく登録する状態にし、その行の名前を残さない", () => {
    const { result } = renderHook(() => useAdminEditor());
    act(() => result.current.select(north));

    act(() => result.current.pick(10, 20));

    expect(result.current.editing).toBeNull();
    expect(result.current.input).toEqual({ ...emptyInput(), x: "10.00", y: "20.00" });
  });

  it("直している行の、マップと種類とアイコンのパスを送る値に入れる", () => {
    const { result } = renderHook(() => useAdminEditor());
    act(() => result.current.changeKind("base-points"));
    act(() => result.current.select(crater));

    let values: ReturnType<typeof result.current.toSaveValues> = null;
    act(() => {
      values = result.current.toSaveValues();
    });

    expect(values).toEqual({ nameJa: null, x: 100, y: 200, radius: 255, baseKind: "minor", mapId: "crater", iconPath: null });
  });

  it("誤りがあると送る値を作らず、欄ごとの文言を持つ", () => {
    const { result } = renderHook(() => useAdminEditor());
    act(() => result.current.change({ nameJa: "北", x: "1025", y: "1" }));

    let values: ReturnType<typeof result.current.toSaveValues> = null;
    act(() => {
      values = result.current.toSaveValues();
    });

    expect(values).toBeNull();
    expect(result.current.errors).toEqual({ x: COORDINATE_MESSAGE });
  });

  it("種類を替えると、入力を捨てる", () => {
    const { result } = renderHook(() => useAdminEditor());
    act(() => result.current.select(north));

    act(() => result.current.changeKind("spawn-points"));

    expect(result.current.editing).toBeNull();
    expect(result.current.input).toEqual(emptyInput());
  });
});
