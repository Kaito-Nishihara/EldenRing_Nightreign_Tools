"use client";

import { useId } from "react";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import Select from "@/components/ui/Select";
import { ADMIN_PLACE_KINDS, ADMIN_PLACE_KIND_ORDER, NORMAL_MAP_ID } from "@/lib/adminPlaces";
import type { AdminPlaceKind, Jihen } from "@/types";

/** 種類のボタンに添える、マップの印に似せた小さな印(マスタの管理 3.2) */
const KIND_DOTS: Record<AdminPlaceKind, string> = {
  "spawn-points": "rounded-full bg-azure-bright",
  "shrink-zones": "rounded-full border-2 border-tide",
  "second-shrink-points": "rotate-45 bg-tide",
  "base-points": "rounded-full border-2 border-line-strong",
};

interface AdminModePanelProps {
  kind: AdminPlaceKind;
  mapId: string;
  jihenList: Jihen[];
  onKindChange: (kind: AdminPlaceKind) => void;
  onMapChange: (mapId: string) => void;
}

/**
 * 登録するものの種類とマップの種類を選ぶ枠(マスタの管理 3.2)。
 * マップの種類は、マップの画像と、拠点候補地の一覧を切り替える。
 */
export default function AdminModePanel({ kind, mapId, jihenList, onKindChange, onMapChange }: AdminModePanelProps) {
  const headingId = useId();

  return (
    <Panel ornate aria-labelledby={headingId}>
      <SectionHeading eyebrow="Cartography" title="登録するもの" id={headingId} />
      <div role="group" aria-label="登録するものの種類" className="grid grid-cols-2 gap-2">
        {ADMIN_PLACE_KIND_ORDER.map((value) => {
          const pressed = value === kind;
          return (
            <button
              key={value}
              type="button"
              aria-pressed={pressed}
              onClick={() => onKindChange(value)}
              className={`flex min-h-11 items-center gap-2 border px-3 text-sm transition-colors duration-200 ease-out ${
                pressed ? "border-azure bg-night-sunk text-azure" : "border-line-strong text-ash-soft hover:text-azure-bright"
              }`}
            >
              <span aria-hidden className={`h-2.5 w-2.5 flex-none ${KIND_DOTS[value]}`} />
              {ADMIN_PLACE_KINDS[value].label}
            </button>
          );
        })}
      </div>
      <Select
        label="マップ"
        hint="拠点候補地は、マップごとに持ちます"
        value={mapId}
        onChange={(event) => onMapChange(event.target.value)}
        className="mt-4"
      >
        <option value={NORMAL_MAP_ID}>通常</option>
        {jihenList.map((jihen) => (
          <option key={jihen.id} value={jihen.id}>
            {jihen.nameJa}
          </option>
        ))}
      </Select>
    </Panel>
  );
}
