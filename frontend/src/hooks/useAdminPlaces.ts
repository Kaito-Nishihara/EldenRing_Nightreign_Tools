"use client";

import { useCallback, useEffect, useState } from "react";
import { adminCreatePlace, adminListPlaces, adminUpdatePlace } from "@/lib/api";
import { ApiError, SERVER_ERROR_MESSAGE } from "@/lib/apiError";
import { clearAdminKey } from "@/lib/adminKey";
import {
  ADMIN_PLACE_KINDS,
  ADMIN_PLACE_KIND_ORDER,
  DEFAULT_RADIUS,
  NORMAL_MAP_ID,
  type AdminPlace,
  type AdminPlaceKindSpec,
  type AdminPlaceSaveValues,
} from "@/lib/adminPlaces";
import { isUnauthorized } from "@/hooks/useAdminKey";
import type { AdminPlaceKind } from "@/types";

export type AdminPlacesByKind = Record<AdminPlaceKind, AdminPlace[]>;

const EMPTY_PLACES: AdminPlacesByKind = {
  "spawn-points": [],
  "shrink-zones": [],
  "second-shrink-points": [],
  "base-points": [],
};

function specOf(kind: AdminPlaceKind): AdminPlaceKindSpec {
  return ADMIN_PLACE_KINDS[kind];
}

/** 保存した行で一覧を直す。新しい行は足し、識別子の順に並べる(マスタの管理 4.1) */
function upsert(places: readonly AdminPlace[], saved: AdminPlace): AdminPlace[] {
  return [...places.filter((p) => p.id !== saved.id), saved].sort((a, b) => a.id - b.id);
}

/** 登録済みの値のまま、非表示かどうかだけを変えて送る値 */
function valuesWithHidden(place: AdminPlace, isHidden: boolean): AdminPlaceSaveValues {
  return {
    nameJa: place.nameJa,
    x: place.x,
    y: place.y,
    radius: place.radius ?? DEFAULT_RADIUS,
    mapId: place.mapId ?? NORMAL_MAP_ID,
    iconPath: place.iconPath,
    isHidden,
  };
}

/**
 * 管理用の 4 種類の一覧と、登録、修正、非表示(マスタの管理 3.3、4.2)。
 * 401 が返ったら鍵を消す。画面は鍵の入力の欄に戻り、入力中の値は画面の側に残る(マスタの管理 3.1)。
 */
export function useAdminPlaces(adminKey: string | null) {
  const [places, setPlaces] = useState<AdminPlacesByKind>(EMPTY_PLACES);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [attempt, setAttempt] = useState(0);
  const [pending, setPending] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);

  /** 401 は鍵を消して null を返す。ほかは画面に出す文言を返す */
  const messageOf = useCallback((e: unknown): string | null => {
    if (isUnauthorized(e)) {
      clearAdminKey();
      return null;
    }
    return e instanceof ApiError ? e.message : SERVER_ERROR_MESSAGE;
  }, []);

  useEffect(() => {
    if (!adminKey) return;
    let cancelled = false;
    setLoading(true);
    setError(null);
    Promise.all(
      ADMIN_PLACE_KIND_ORDER.map(async (kind) => {
        const dtos = await adminListPlaces(kind, adminKey);
        return [kind, dtos.map((dto) => specOf(kind).fromDto(dto))] as const;
      })
    )
      .then((entries) => {
        if (!cancelled) setPlaces(Object.fromEntries(entries) as AdminPlacesByKind);
      })
      .catch((e: unknown) => {
        if (!cancelled) setError(messageOf(e));
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [adminKey, attempt, messageOf]);

  /** 登録(id が null)か修正を送る。成功したら保存した行を返し、失敗したら null を返す */
  const save = useCallback(
    async (kind: AdminPlaceKind, id: number | null, values: AdminPlaceSaveValues): Promise<AdminPlace | null> => {
      if (!adminKey) return null;
      const spec = specOf(kind);
      setPending(true);
      setActionError(null);
      try {
        const request = spec.toRequest(values);
        const dto =
          id === null
            ? await adminCreatePlace(kind, request, adminKey)
            : await adminUpdatePlace(kind, id, request, adminKey);
        const saved = spec.fromDto(dto);
        setPlaces((prev) => ({ ...prev, [kind]: upsert(prev[kind], saved) }));
        return saved;
      } catch (e) {
        setActionError(messageOf(e));
        return null;
      } finally {
        setPending(false);
      }
    },
    [adminKey, messageOf]
  );

  /** 非表示にするか、表示に戻す。入力の欄で直しかけた値ではなく、登録済みの値のまま送る */
  const setHidden = useCallback(
    (kind: AdminPlaceKind, place: AdminPlace, isHidden: boolean) => save(kind, place.id, valuesWithHidden(place, isHidden)),
    [save]
  );

  const retry = useCallback(() => setAttempt((n) => n + 1), []);
  const dismissActionError = useCallback(() => setActionError(null), []);

  return { places, loading, error, retry, save, setHidden, pending, actionError, dismissActionError };
}
