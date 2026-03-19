"use client";

import { useState, useEffect, useCallback } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import AdminOverlay from "@/components/admin/AdminOverlay";
import SpawnPointForm from "@/components/admin/SpawnPointForm";
import ShrinkZoneForm from "@/components/admin/ShrinkZoneForm";
import { getMapImagePath } from "@/lib/mapImage";
import {
  getSpawnPoints,
  getShrinkZones,
  adminCreateSpawnPoint,
  adminUpdateSpawnPoint,
  adminDeleteSpawnPoint,
  adminCreateShrinkZone,
  adminUpdateShrinkZone,
  adminDeleteShrinkZone,
} from "@/lib/api";
import type { SpawnPoint, ShrinkZone } from "@/types";

/** 編集モード */
type EditMode = "spawn-point" | "shrink-zone";

/**
 * 管理画面
 * マップクリックでマスタデータ（開始地点・収縮候補地）を登録する
 */
export default function AdminPage() {
  // データ
  const [spawnPoints, setSpawnPoints] = useState<SpawnPoint[]>([]);
  const [shrinkZones, setShrinkZones] = useState<ShrinkZone[]>([]);

  // 編集状態
  const [editMode, setEditMode] = useState<EditMode>("spawn-point");
  const [clickedPosition, setClickedPosition] = useState<{ x: number; y: number } | null>(null);
  const [editingSpawnPoint, setEditingSpawnPoint] = useState<SpawnPoint | null>(null);
  const [editingShrinkZone, setEditingShrinkZone] = useState<ShrinkZone | null>(null);

  // 地変選択（マップ画像切替用）
  const [selectedJihenId, setSelectedJihenId] = useState<string | null>(null);

  // エラー・ローディング
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  /** データ取得 */
  const fetchData = useCallback(async () => {
    try {
      const [sp, sz] = await Promise.all([getSpawnPoints(), getShrinkZones()]);
      setSpawnPoints(sp);
      setShrinkZones(sz);
    } catch (err) {
      console.error("マスタデータ取得エラー:", err);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  /** マップクリック時のハンドラー */
  const handleMapClick = useCallback(
    (x: number, y: number) => {
      setClickedPosition({ x, y });
      setEditingSpawnPoint(null);
      setEditingShrinkZone(null);
      setError(null);
    },
    []
  );

  /** 既存SpawnPointの編集 */
  const handleEditSpawnPoint = useCallback((sp: SpawnPoint) => {
    setEditMode("spawn-point");
    setEditingSpawnPoint(sp);
    setEditingShrinkZone(null);
    setClickedPosition({ x: sp.x, y: sp.y });
    setError(null);
  }, []);

  /** 既存ShrinkZoneの編集 */
  const handleEditShrinkZone = useCallback((sz: ShrinkZone) => {
    setEditMode("shrink-zone");
    setEditingShrinkZone(sz);
    setEditingSpawnPoint(null);
    setClickedPosition({ x: sz.centerX, y: sz.centerY });
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
    <div className="h-screen flex flex-col">
      {/* ヘッダー */}
      <header className="flex-shrink-0 bg-gray-800 border-b border-gray-700 px-4 py-3 flex items-center justify-between">
        <h1 className="text-xl font-bold text-gray-100">
          管理画面
          <span className="text-sm font-normal text-gray-400 ml-2">
            マスタデータ座標登録
          </span>
        </h1>
        <a href="/" className="text-sm text-blue-400 hover:text-blue-300">
          メインページへ戻る
        </a>
      </header>

      {/* メインコンテンツ */}
      <div className="flex-1 flex flex-col md:flex-row overflow-hidden">
        {/* サイドパネル */}
        <aside className="order-2 md:order-1 md:w-96 md:flex-shrink-0 md:h-full overflow-y-auto bg-gray-900 border-t md:border-t-0 md:border-r border-gray-700 p-4 space-y-4">
          {/* 編集モード切替 */}
          <div className="flex gap-2">
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
          {editMode === "spawn-point" ? (
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
          ) : (
            <ShrinkZoneForm
              position={clickedPosition}
              editing={editingShrinkZone}
              saving={saving}
              onSave={handleSaveShrinkZone}
              onDelete={handleDeleteShrinkZone}
              onCancel={() => {
                setClickedPosition(null);
                setEditingShrinkZone(null);
              }}
            />
          )}

          {/* 登録済みデータ一覧 */}
          <div className="border border-gray-700 rounded-lg bg-gray-800/50 p-3 space-y-2">
            <h3 className="text-sm font-bold text-gray-300">
              登録済み{editMode === "spawn-point" ? "開始地点" : "収縮候補地"}
              <span className="text-xs font-normal text-gray-500 ml-1">
                ({editMode === "spawn-point" ? spawnPoints.length : shrinkZones.length}件)
              </span>
            </h3>
            {editMode === "spawn-point" ? (
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
                    <span className="text-gray-500 ml-2">
                      ({sp.x}, {sp.y}) {sp.nearbyBaseType}
                    </span>
                  </button>
                ))
              )
            ) : shrinkZones.length === 0 ? (
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
            )}
          </div>
        </aside>

        {/* マップキャンバス */}
        <main className="order-1 md:order-2 flex-1 min-h-[50vh] md:min-h-0">
          <MapCanvas mapImagePath={mapImagePath} onStageClick={handleMapClick}>
            <AdminOverlay
              spawnPoints={spawnPoints}
              shrinkZones={shrinkZones}
              editMode={editMode}
              clickedPosition={clickedPosition}
              editingSpawnPointId={editingSpawnPoint?.id ?? null}
              editingShrinkZoneId={editingShrinkZone?.id ?? null}
              onSpawnPointClick={handleEditSpawnPoint}
              onShrinkZoneClick={handleEditShrinkZone}
            />
          </MapCanvas>
        </main>
      </div>
    </div>
  );
}
