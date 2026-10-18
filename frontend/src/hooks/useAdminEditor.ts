"use client";

import { useCallback, useMemo, useState } from "react";
import {
  ADMIN_PLACE_KINDS,
  NORMAL_MAP_ID,
  emptyInput,
  formatCoordinate,
  inputFromPlace,
  pickedCoordinate,
  validateInput,
  type AdminPlace,
  type AdminPlaceErrors,
  type AdminPlaceInput,
  type AdminPlaceKindSpec,
  type AdminPlaceSaveValues,
} from "@/lib/adminPlaces";
import type { AdminPlaceKind } from "@/types";

/** マップに出す、入力中の場所。X と Y が数として読めるときだけ出す */
export interface AdminPlacePreview {
  x: number;
  y: number;
  radius: number;
}

function previewOf(input: AdminPlaceInput): AdminPlacePreview | null {
  const x = Number(input.x);
  const y = Number(input.y);
  if (input.x.trim() === "" || input.y.trim() === "" || !Number.isFinite(x) || !Number.isFinite(y)) return null;
  return { x, y, radius: input.radius };
}

/**
 * 管理の画面の入力の状態(マスタの管理 3.2、3.3)。
 * 鍵の入力の欄に戻っても入力中の値を残せるよう、管理の画面の一番上で持つ(マスタの管理 3.1)。
 */
export function useAdminEditor() {
  const [kind, setKind] = useState<AdminPlaceKind>("spawn-points");
  const [mapId, setMapId] = useState(NORMAL_MAP_ID);
  const [editing, setEditing] = useState<AdminPlace | null>(null);
  const [input, setInput] = useState<AdminPlaceInput>(emptyInput);
  const [errors, setErrors] = useState<AdminPlaceErrors>({});

  const spec: AdminPlaceKindSpec = ADMIN_PLACE_KINDS[kind];

  /** 入力を捨てて、新しく登録する状態にする */
  const cancel = useCallback(() => {
    setEditing(null);
    setInput(emptyInput());
    setErrors({});
  }, []);

  const changeKind = useCallback(
    (next: AdminPlaceKind) => {
      setKind(next);
      cancel();
    },
    [cancel]
  );

  const changeMap = useCallback(
    (next: string) => {
      setMapId(next);
      cancel();
    },
    [cancel]
  );

  /**
   * マップを押した場所を X と Y に入れ、新しく登録する状態にする(マスタの管理 3.3)。
   * 直している行から移るときは、その行の名前を残さない。
   */
  const pick = useCallback(
    (x: number, y: number) => {
      const position = { x: formatCoordinate(pickedCoordinate(x)), y: formatCoordinate(pickedCoordinate(y)) };
      setInput((prev) => (editing ? { ...emptyInput(), ...position } : { ...prev, ...position }));
      setEditing(null);
      setErrors({});
    },
    [editing]
  );

  /** 登録済みの印か一覧の行を押したとき。その行を直す状態にする */
  const select = useCallback((place: AdminPlace) => {
    setEditing(place);
    setInput(inputFromPlace(place));
    setErrors({});
  }, []);

  const change = useCallback((patch: Partial<AdminPlaceInput>) => {
    setInput((prev) => ({ ...prev, ...patch }));
  }, []);

  /** 入力を確かめて、送れる値を返す。誤りがあれば欄ごとの文言を出し、null を返す */
  const toSaveValues = useCallback((): AdminPlaceSaveValues | null => {
    const result = validateInput(spec, input);
    setErrors(result.errors);
    if (!result.values) return null;
    return { ...result.values, mapId: editing?.mapId ?? mapId, iconPath: editing?.iconPath ?? null };
  }, [spec, input, editing, mapId]);

  /** 非表示にしたか表示に戻した行を、直している行として持ち直す。入力中の値は変えない */
  const replaceEditing = useCallback((place: AdminPlace) => setEditing(place), []);

  const preview = useMemo(() => previewOf(input), [input]);

  return {
    kind,
    spec,
    mapId,
    editing,
    input,
    errors,
    preview,
    changeKind,
    changeMap,
    pick,
    select,
    change,
    cancel,
    toSaveValues,
    replaceEditing,
  };
}
