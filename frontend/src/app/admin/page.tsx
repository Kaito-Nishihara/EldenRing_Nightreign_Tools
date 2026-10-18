"use client";

import { useCallback, useState } from "react";
import MapCanvas from "@/components/map/MapCanvas";
import AdminKeyGate from "@/components/admin/AdminKeyGate";
import AdminModePanel from "@/components/admin/AdminModePanel";
import AdminPlaceForm, { HIDE_DESCRIPTION } from "@/components/admin/AdminPlaceForm";
import AdminPlaceList from "@/components/admin/AdminPlaceList";
import AdminPlaceOverlay from "@/components/admin/AdminPlaceOverlay";
import BottomSheet from "@/components/ui/BottomSheet";
import ConfirmDialog from "@/components/ui/ConfirmDialog";
import Panel from "@/components/ui/Panel";
import { ErrorNotice, Skeleton } from "@/components/ui/StatusViews";
import { useAdminEditor } from "@/hooks/useAdminEditor";
import { useAdminKey } from "@/hooks/useAdminKey";
import { useAdminPlaces } from "@/hooks/useAdminPlaces";
import { useMasterData } from "@/hooks/useMasterData";
import { NORMAL_MAP_ID, placesOnMap } from "@/lib/adminPlaces";
import { getMapImagePath } from "@/lib/mapImage";

/**
 * マスタの管理の画面(マスタの管理 3 章)。
 * 鍵がないときは鍵の入力の欄だけを出す。鍵があれば、広い幅では左に入力の枠、右にマップを並べる。
 * 入力の状態はこの画面で持ち、途中で鍵の入力の欄に戻っても残す(マスタの管理 3.1)。
 */
export default function AdminPage() {
  const adminKey = useAdminKey();
  const editor = useAdminEditor();
  const admin = useAdminPlaces(adminKey ?? null);
  const { jihenList } = useMasterData();
  const [confirmingHide, setConfirmingHide] = useState(false);

  const { kind, spec, mapId, editing } = editor;
  const places = placesOnMap(spec, admin.places[kind], mapId);

  const save = useCallback(async () => {
    const values = editor.toSaveValues();
    if (!values) return;
    const saved = await admin.save(kind, editing?.id ?? null, values);
    if (saved) editor.cancel();
  }, [admin, editor, editing, kind]);

  /** 非表示にするか表示に戻す。直している行は、返ってきた行で持ち直す */
  const changeHidden = useCallback(
    async (isHidden: boolean) => {
      if (!editing) return;
      const saved = await admin.setHidden(kind, editing, isHidden);
      setConfirmingHide(false);
      if (saved) editor.replaceEditing(saved);
    },
    [admin, editor, editing, kind]
  );

  const closeConfirm = useCallback(() => setConfirmingHide(false), []);

  // sessionStorage を読むまでは何も出さない
  if (adminKey === undefined) return null;
  if (adminKey === null) return <AdminKeyGate />;

  const controls = admin.error ? (
    <Panel>
      <ErrorNotice message={admin.error} onRetry={admin.retry} />
    </Panel>
  ) : admin.loading ? (
    <Panel aria-busy>
      <Skeleton className="mb-3 h-6 w-24" />
      <Skeleton className="h-48" />
    </Panel>
  ) : (
    <>
      <AdminModePanel
        kind={kind}
        mapId={mapId}
        jihenList={jihenList}
        onKindChange={editor.changeKind}
        onMapChange={editor.changeMap}
      />
      <AdminPlaceForm
        spec={spec}
        editing={editing}
        input={editor.input}
        errors={editor.errors}
        pending={admin.pending}
        actionError={admin.actionError}
        onChange={editor.change}
        onSave={() => void save()}
        onCancel={() => {
          editor.cancel();
          admin.dismissActionError();
        }}
        onHide={() => setConfirmingHide(true)}
        onRestore={() => void changeHidden(false)}
      />
      <AdminPlaceList spec={spec} places={places} selectedId={editing?.id ?? null} onSelect={editor.select} />
    </>
  );

  return (
    // 狭い幅では、畳んだ入力の枠の取っ手にマップの下端が隠れないよう、下を空ける
    <div className="flex h-full flex-col gap-4 pb-16 md:flex-row md:p-4">
      <h1 className="sr-only">マスタの管理</h1>

      <section aria-label="マップ" className="flex min-h-0 flex-1 flex-col md:order-2">
        <Panel ornate className="flex min-h-0 flex-1 flex-col !p-2 md:!p-4">
          <p className="mb-2 text-sm text-ash-soft">マップを押すと、その場所の座標を入れます</p>
          <div className="relative min-h-0 flex-1 border border-line">
            <MapCanvas mapImagePath={getMapImagePath(mapId === NORMAL_MAP_ID ? null : mapId)} onStageClick={editor.pick}>
              <AdminPlaceOverlay
                kind={kind}
                places={places}
                editingId={editing?.id ?? null}
                preview={editor.preview}
                onSelect={editor.select}
              />
            </MapCanvas>
            {editor.preview && (
              <div className="pointer-events-none absolute left-2 top-2 border border-line bg-night/85 px-2 text-sm tabular-nums text-azure-bright">
                X {editor.input.x} ／ Y {editor.input.y}
              </div>
            )}
          </div>
        </Panel>
      </section>

      <aside className="md:order-1 md:w-[360px] md:flex-none md:overflow-y-auto">
        <BottomSheet handleLabel={editing ? `${spec.label}を直しています` : "登録と修正"}>
          <div className="flex flex-col gap-4 p-4 md:p-0">{controls}</div>
        </BottomSheet>
      </aside>

      {confirmingHide && editing && (
        <ConfirmDialog
          title={`${spec.label}を非表示にしますか`}
          message={HIDE_DESCRIPTION}
          confirmLabel="非表示にする"
          busy={admin.pending}
          onConfirm={() => void changeHidden(true)}
          onCancel={closeConfirm}
        />
      )}
    </div>
  );
}
