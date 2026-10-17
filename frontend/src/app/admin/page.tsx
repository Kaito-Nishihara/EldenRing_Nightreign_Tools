"use client";

import { useState, useEffect, useCallback } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import AdminOverlay from "@/components/admin/AdminOverlay";
import SpawnPointForm from "@/components/admin/SpawnPointForm";
import ShrinkZoneForm, { DEFAULT_RADIUS } from "@/components/admin/ShrinkZoneForm";
import SecondShrinkPointForm from "@/components/admin/SecondShrinkPointForm";
import { getMapImagePath } from "@/lib/mapImage";
import {
  getSpawnPoints,
  getShrinkZones,
  getSecondShrinkPoints,
  getBasePoints,
  adminCreateSpawnPoint,
  adminUpdateSpawnPoint,
  adminDeleteSpawnPoint,
  adminCreateShrinkZone,
  adminUpdateShrinkZone,
  adminDeleteShrinkZone,
  adminCreateSecondShrinkPoint,
  adminUpdateSecondShrinkPoint,
  adminDeleteSecondShrinkPoint,
  adminCreateBasePoint,
  adminDeleteBasePoint,
} from "@/lib/api";
import type { SpawnPoint, ShrinkZone, BasePoint, SecondShrinkPoint } from "@/types";

/** 編集モード */
type EditMode = "spawn-point" | "shrink-zone" | "second-shrink-point" | "base-point";

/**
 * 管理画面
 * マップクリックでマスタデータ（開始地点・収縮候補地）を登録する
 */
export default function AdminPage() {
  // データ
  const [spawnPoints, setSpawnPoints] = useState<SpawnPoint[]>([]);
  const [shrinkZones, setShrinkZones] = useState<ShrinkZone[]>([]);
  const [secondShrinkPoints, setSecondShrinkPoints] = useState<SecondShrinkPoint[]>([]);
  const [basePoints, setBasePoints] = useState<BasePoint[]>([]);

  // 編集状態
  const [editMode, setEditMode] = useState<EditMode>("spawn-point");
  const [clickedPosition, setClickedPosition] = useState<{ x: number; y: number } | null>(null);
  const [editingSpawnPoint, setEditingSpawnPoint] = useState<SpawnPoint | null>(null);
  const [editingShrinkZone, setEditingShrinkZone] = useState<ShrinkZone | null>(null);
  const [editingSecondShrinkPoint, setEditingSecondShrinkPoint] = useState<SecondShrinkPoint | null>(null);
  const [shrinkZoneRadius, setShrinkZoneRadius] = useState(DEFAULT_RADIUS);

  // 地変選択（マップ画像切替用）
  const [selectedJihenId, setSelectedJihenId] = useState<string | null>(null);

  // エラー・ローディング
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  /** マップIDを算出（地変未選択=通常マップ） */
  const mapId = selectedJihenId ?? "normal";

  /**
   * データ取得。マスタごとに取り、1 つが失敗してもほかは出す。
   * 失敗したものは、どのマスタかと理由を誤りの欄に出す
   */
  const fetchData = useCallback(async () => {
    const [sp, sz, ssp, bp] = await Promise.allSettled([
      getSpawnPoints(),
      getShrinkZones(),
      getSecondShrinkPoints(),
      getBasePoints(mapId),
    ]);
    const failures: string[] = [];
    const apply = <T,>(result: PromiseSettledResult<T>, label: string, set: (value: T) => void) => {
      if (result.status === "fulfilled") {
        set(result.value);
      } else {
        const reason = result.reason instanceof Error ? result.reason.message : "取得に失敗しました";
        failures.push(`${label}: ${reason}`);
      }
    };
    apply(sp, "開始地点", setSpawnPoints);
    apply(sz, "収縮候補地", setShrinkZones);
    apply(ssp, "第二収縮地点", setSecondShrinkPoints);
    apply(bp, "拠点候補地", setBasePoints);
    setError(failures.length > 0 ? failures.join(" / ") : null);
  }, [mapId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  /** マップクリック時のハンドラー */
  const handleMapClick = useCallback(
    async (x: number, y: number) => {
      // base-pointモードではクリックで即座に登録
      if (editMode === "base-point") {
        setSaving(true);
        setError(null);
        try {
          await adminCreateBasePoint({ mapId, x, y });
          await fetchData();
        } catch (err) {
          setError(err instanceof Error ? err.message : "登録に失敗しました");
        } finally {
          setSaving(false);
        }
        return;
      }
      setClickedPosition({ x, y });
      setEditingSpawnPoint(null);
      if (editMode !== "shrink-zone") {
        setEditingShrinkZone(null);
      }
      if (editMode !== "second-shrink-point") {
        setEditingSecondShrinkPoint(null);
      }
      setError(null);
    },
    [editMode, mapId, fetchData]
  );

  /** BasePoint削除 */
  const handleDeleteBasePoint = useCallback(
    async (id: number) => {
      setSaving(true);
      setError(null);
      try {
        await adminDeleteBasePoint(id);
        await fetchData();
      } catch (err) {
        setError(err instanceof Error ? err.message : "削除に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [fetchData]
  );

  /** 既存SpawnPointの編集 */
  const handleEditSpawnPoint = useCallback((sp: SpawnPoint) => {
    setEditMode("spawn-point");
    setEditingSpawnPoint(sp);
    setEditingShrinkZone(null);
    setEditingSecondShrinkPoint(null);
    setClickedPosition({ x: sp.x, y: sp.y });
    setError(null);
  }, []);

  /** 既存ShrinkZoneの編集 */
  const handleEditShrinkZone = useCallback((sz: ShrinkZone) => {
    setEditMode("shrink-zone");
    setEditingShrinkZone(sz);
    setEditingSpawnPoint(null);
    setEditingSecondShrinkPoint(null);
    setClickedPosition({ x: sz.centerX, y: sz.centerY });
    setShrinkZoneRadius(sz.radius);
    setError(null);
  }, []);

  /** 既存SecondShrinkPointの編集 */
  const handleEditSecondShrinkPoint = useCallback((point: SecondShrinkPoint) => {
    setEditMode("second-shrink-point");
    setEditingSecondShrinkPoint(point);
    setEditingSpawnPoint(null);
    setEditingShrinkZone(null);
    setClickedPosition({ x: point.x, y: point.y });
    setError(null);
  }, []);

  /** SpawnPoint保存 */
  const handleSaveSpawnPoint = useCallback(
    async (data: Omit<SpawnPoint, "id">) => {
      setSaving(true);
      setError(null);
      try {
        if (editingSpawnPoint) {
          await adminUpdateSpawnPoint(editingSpawnPoint.id, data);
        } else {
          await adminCreateSpawnPoint(data);
        }
        await fetchData();
        setClickedPosition(null);
        setEditingSpawnPoint(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "保存に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [editingSpawnPoint, fetchData]
  );

  /** SpawnPoint削除 */
  const handleDeleteSpawnPoint = useCallback(
    async (id: number) => {
      setSaving(true);
      setError(null);
      try {
        await adminDeleteSpawnPoint(id);
        await fetchData();
        setClickedPosition(null);
        setEditingSpawnPoint(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "削除に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [fetchData]
  );

  /** ShrinkZone保存 */
  const handleSaveShrinkZone = useCallback(
    async (data: Omit<ShrinkZone, "id">) => {
      setSaving(true);
      setError(null);
      try {
        if (editingShrinkZone) {
          await adminUpdateShrinkZone(editingShrinkZone.id, data);
        } else {
          await adminCreateShrinkZone(data);
        }
        await fetchData();
        setClickedPosition(null);
        setEditingShrinkZone(null);
        setShrinkZoneRadius(DEFAULT_RADIUS);
      } catch (err) {
        setError(err instanceof Error ? err.message : "保存に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [editingShrinkZone, fetchData]
  );

  /** ShrinkZone削除 */
  const handleDeleteShrinkZone = useCallback(
    async (id: number) => {
      setSaving(true);
      setError(null);
      try {
        await adminDeleteShrinkZone(id);
        await fetchData();
        setClickedPosition(null);
        setEditingShrinkZone(null);
        setShrinkZoneRadius(DEFAULT_RADIUS);
      } catch (err) {
        setError(err instanceof Error ? err.message : "削除に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [fetchData]
  );

  /** SecondShrinkPoint保存 */
  const handleSaveSecondShrinkPoint = useCallback(
    async (data: Omit<SecondShrinkPoint, "id">) => {
      setSaving(true);
      setError(null);
      try {
        if (editingSecondShrinkPoint) {
          await adminUpdateSecondShrinkPoint(editingSecondShrinkPoint.id, data);
        } else {
          await adminCreateSecondShrinkPoint(data);
        }
        await fetchData();
        setClickedPosition(null);
        setEditingSecondShrinkPoint(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "保存に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [editingSecondShrinkPoint, fetchData]
  );

  /** SecondShrinkPoint削除 */
  const handleDeleteSecondShrinkPoint = useCallback(
    async (id: number) => {
      setSaving(true);
      setError(null);
      try {
        await adminDeleteSecondShrinkPoint(id);
        await fetchData();
        setClickedPosition(null);
        setEditingSecondShrinkPoint(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "削除に失敗しました");
      } finally {
        setSaving(false);
      }
    },
    [fetchData]
  );

  const mapImagePath = getMapImagePath(selectedJihenId);

  return (
    <div className="h-full flex flex-col">
      <h1 className="sr-only">マスタの管理</h1>

      {/* メインコンテンツ */}
      <div className="flex-1 flex flex-col md:flex-row overflow-hidden">
        {/* サイドパネル */}
        <aside className="order-2 md:order-1 md:w-96 md:flex-shrink-0 md:h-full overflow-y-auto bg-gray-900 border-t md:border-t-0 md:border-r border-gray-700 p-4 space-y-4">
          {/* 編集モード切替 */}
          <div className="grid grid-cols-2 gap-2">
            <button
              type="button"
              onClick={() => setEditMode("spawn-point")}
              className={`flex-1 px-3 py-2 text-sm rounded-md border transition-colors ${
                editMode === "spawn-point"
                  ? "bg-blue-600 border-blue-500 text-white"
                  : "bg-gray-700 border-gray-600 text-gray-300 hover:bg-gray-600"
              }`}
            >
              開始地点
            </button>
            <button
              type="button"
              onClick={() => setEditMode("shrink-zone")}
              className={`flex-1 px-3 py-2 text-sm rounded-md border transition-colors ${
                editMode === "shrink-zone"
                  ? "bg-green-600 border-green-500 text-white"
                  : "bg-gray-700 border-gray-600 text-gray-300 hover:bg-gray-600"
              }`}
            >
              収縮候補地
            </button>
            <button
              type="button"
              onClick={() => setEditMode("second-shrink-point")}
              className={`px-3 py-2 text-sm rounded-md border transition-colors ${
                editMode === "second-shrink-point"
                  ? "bg-fuchsia-600 border-fuchsia-500 text-white"
                  : "bg-gray-700 border-gray-600 text-gray-300 hover:bg-gray-600"
              }`}
            >
              第二収縮地点
            </button>
            <button
              type="button"
              onClick={() => setEditMode("base-point")}
              className={`px-3 py-2 text-sm rounded-md border transition-colors ${
                editMode === "base-point"
                  ? "bg-yellow-600 border-yellow-500 text-white"
                  : "bg-gray-700 border-gray-600 text-gray-300 hover:bg-gray-600"
              }`}
            >
              拠点候補
            </button>
          </div>

          {/* 地変切替 */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">マップ切替</label>
            <select
              value={selectedJihenId ?? ""}
              onChange={(e) => setSelectedJihenId(e.target.value || null)}
              className="w-full bg-gray-700 border border-gray-600 rounded-md px-3 py-2 text-gray-200 text-sm"
            >
              <option value="">通常マップ</option>
              <option value="kakou">火口</option>
              <option value="sankan">山寒</option>
              <option value="fuhai">腐敗</option>
              <option value="nokurateou">ノクラテオ</option>
            </select>
          </div>

          {/* エラー表示 */}
          {error && (
            <div className="p-3 border border-red-700 rounded-lg bg-red-900/20">
              <p className="text-sm text-red-400">{error}</p>
            </div>
          )}

          {/* 登録フォーム */}
          {editMode === "base-point" ? (
            <div className="p-4 border border-yellow-700 rounded-lg bg-yellow-900/20">
              <h3 className="text-sm font-bold text-yellow-300 mb-2">拠点候補地登録</h3>
              <p className="text-xs text-yellow-200/70">
                マップをクリックすると拠点候補地（?マーク）が登録されます
              </p>
              {saving && (
                <p className="text-xs text-yellow-400 mt-2">登録中...</p>
              )}
            </div>
          ) : editMode === "spawn-point" ? (
            <SpawnPointForm
              position={clickedPosition}
              editing={editingSpawnPoint}
              saving={saving}
              onSave={handleSaveSpawnPoint}
              onDelete={handleDeleteSpawnPoint}
              onCancel={() => {
                setClickedPosition(null);
                setEditingSpawnPoint(null);
              }}
            />
          ) : editMode === "shrink-zone" ? (
            <ShrinkZoneForm
              position={clickedPosition}
              editing={editingShrinkZone}
              radius={shrinkZoneRadius}
              onRadiusChange={setShrinkZoneRadius}
              saving={saving}
              onSave={handleSaveShrinkZone}
              onDelete={handleDeleteShrinkZone}
              onCancel={() => {
                setClickedPosition(null);
                setEditingShrinkZone(null);
                setShrinkZoneRadius(DEFAULT_RADIUS);
              }}
            />
          ) : (
            <SecondShrinkPointForm
              position={clickedPosition}
              editing={editingSecondShrinkPoint}
              saving={saving}
              onSave={handleSaveSecondShrinkPoint}
              onDelete={handleDeleteSecondShrinkPoint}
              onCancel={() => {
                setClickedPosition(null);
                setEditingSecondShrinkPoint(null);
              }}
            />
          )}

          {/* 登録済みデータ一覧 */}
          <div className="border border-gray-700 rounded-lg bg-gray-800/50 p-3 space-y-2">
            <h3 className="text-sm font-bold text-gray-300">
              登録済み{editMode === "spawn-point"
                ? "開始地点"
                : editMode === "shrink-zone"
                  ? "収縮候補地"
                  : editMode === "second-shrink-point"
                    ? "第二収縮地点"
                    : "拠点候補地"}
              <span className="text-xs font-normal text-gray-500 ml-1">
                ({editMode === "spawn-point"
                  ? spawnPoints.length
                  : editMode === "shrink-zone"
                    ? shrinkZones.length
                    : editMode === "second-shrink-point"
                      ? secondShrinkPoints.length
                      : basePoints.length}件)
              </span>
            </h3>
            {editMode === "base-point" ? (
              basePoints.length === 0 ? (
                <p className="text-xs text-gray-500">データなし</p>
              ) : (
                basePoints.map((bp) => (
                  <div
                    key={bp.id}
                    className="flex items-center justify-between p-2 rounded text-xs border border-gray-600 bg-gray-700/50 text-gray-300"
                  >
                    <span>
                      #{bp.id} ({bp.x}, {bp.y})
                      {bp.nameJa && <span className="text-gray-400 ml-1">{bp.nameJa}</span>}
                    </span>
                    <button
                      type="button"
                      onClick={() => handleDeleteBasePoint(bp.id)}
                      disabled={saving}
                      className="text-red-400 hover:text-red-300 text-xs"
                    >
                      削除
                    </button>
                  </div>
                ))
              )
            ) : editMode === "spawn-point" ? (
              spawnPoints.length === 0 ? (
                <p className="text-xs text-gray-500">データなし</p>
              ) : (
                spawnPoints.map((sp) => (
                  <button
                    key={sp.id}
                    type="button"
                    onClick={() => handleEditSpawnPoint(sp)}
                    className={`w-full text-left p-2 rounded text-xs border transition-colors ${
                      editingSpawnPoint?.id === sp.id
                        ? "border-blue-500 bg-blue-900/30 text-blue-200"
                        : "border-gray-600 bg-gray-700/50 text-gray-300 hover:bg-gray-700"
                    }`}
                  >
                    <span className="font-bold">{sp.nameJa}</span>
                    <span className="text-gray-500 ml-2">({sp.x}, {sp.y})</span>
                  </button>
                ))
              )
            ) : editMode === "shrink-zone" ? (
              shrinkZones.length === 0 ? (
                <p className="text-xs text-gray-500">データなし</p>
              ) : (
                shrinkZones.map((sz) => (
                  <button
                    key={sz.id}
                    type="button"
                    onClick={() => handleEditShrinkZone(sz)}
                    className={`w-full text-left p-2 rounded text-xs border transition-colors ${
                      editingShrinkZone?.id === sz.id
                        ? "border-green-500 bg-green-900/30 text-green-200"
                        : "border-gray-600 bg-gray-700/50 text-gray-300 hover:bg-gray-700"
                    }`}
                  >
                    <span className="font-bold">{sz.nameJa}</span>
                    <span className="text-gray-500 ml-2">
                      ({sz.centerX}, {sz.centerY}) r={sz.radius}
                    </span>
                  </button>
                ))
              )
            ) : secondShrinkPoints.length === 0 ? (
              <p className="text-xs text-gray-500">データなし</p>
            ) : (
              secondShrinkPoints.map((point) => (
                <button
                  key={point.id}
                  type="button"
                  onClick={() => handleEditSecondShrinkPoint(point)}
                  className={`w-full text-left p-2 rounded text-xs border transition-colors ${
                    editingSecondShrinkPoint?.id === point.id
                      ? "border-fuchsia-500 bg-fuchsia-900/30 text-fuchsia-200"
                      : "border-gray-600 bg-gray-700/50 text-gray-300 hover:bg-gray-700"
                  }`}
                >
                  <span className="font-bold">{point.nameJa}</span>
                  <span className="text-gray-500 ml-2">({point.x}, {point.y})</span>
                </button>
              ))
            )}
          </div>
        </aside>

        {/* マップキャンバス */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas mapImagePath={mapImagePath} onStageClick={handleMapClick}>
            <AdminOverlay
              spawnPoints={spawnPoints}
              shrinkZones={shrinkZones}
              basePoints={basePoints}
              secondShrinkPoints={secondShrinkPoints}
              editMode={editMode}
              clickedPosition={clickedPosition}
              editingSpawnPointId={editingSpawnPoint?.id ?? null}
              editingShrinkZoneId={editingShrinkZone?.id ?? null}
              editingSecondShrinkPointId={editingSecondShrinkPoint?.id ?? null}
              shrinkZoneRadius={shrinkZoneRadius}
              onSpawnPointClick={handleEditSpawnPoint}
              onShrinkZoneClick={handleEditShrinkZone}
              onSecondShrinkPointClick={handleEditSecondShrinkPoint}
            />
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
