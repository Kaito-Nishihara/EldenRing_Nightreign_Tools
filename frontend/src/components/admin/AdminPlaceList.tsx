"use client";

import { useId } from "react";
import Badge from "@/components/ui/Badge";
import Panel from "@/components/ui/Panel";
import { EmptyState } from "@/components/ui/StatusViews";
import { BASE_POINT_KIND_LABELS, placeName, type AdminPlace, type AdminPlaceKindSpec } from "@/lib/adminPlaces";

interface AdminPlaceListProps {
  spec: AdminPlaceKindSpec;
  places: AdminPlace[];
  selectedId: number | null;
  onSelect: (place: AdminPlace) => void;
}

/** 一覧の座標は、整数なら整数で、小数があれば 2 桁まで出す */
function formatNumber(value: number): string {
  return Number.isInteger(value) ? String(value) : value.toFixed(2);
}

/**
 * 登録済みの一覧(マスタの管理 3.3)。非表示の行も「非表示」の印を付けて残す。
 * 行の名前を押すと、その行を直す状態にする。
 */
export default function AdminPlaceList({ spec, places, selectedId, onSelect }: AdminPlaceListProps) {
  const headingId = useId();

  return (
    <Panel aria-labelledby={headingId}>
      <h2 id={headingId} className="mb-2 font-serif text-base font-extrabold text-azure-bright">
        登録済み {places.length} 件
      </h2>
      {places.length === 0 ? (
        <EmptyState title={`${spec.label}はまだありません`} description="マップを押して登録します" />
      ) : (
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-line text-left text-xs text-ash-soft">
              <th className="py-2 font-normal">名前</th>
              {spec.hasBaseKind && <th className="py-2 font-normal">種類</th>}
              <th className="py-2 text-right font-normal">X</th>
              <th className="py-2 text-right font-normal">Y</th>
              {spec.hasRadius && <th className="py-2 text-right font-normal">半径</th>}
            </tr>
          </thead>
          <tbody>
            {places.map((place) => {
              const selected = place.id === selectedId;
              return (
                <tr
                  key={place.id}
                  className={`border-b border-line ${selected ? "bg-night-sunk text-azure" : "text-ash"}`}
                >
                  <td className="py-1">
                    <button
                      type="button"
                      onClick={() => onSelect(place)}
                      aria-current={selected ? "true" : undefined}
                      className="flex min-h-11 w-full items-center gap-2 text-left hover:text-azure-bright"
                    >
                      <span className="truncate">{placeName(place)}</span>
                      {place.isHidden && <Badge>非表示</Badge>}
                    </button>
                  </td>
                  {spec.hasBaseKind && (
                    <td className="py-1 text-ash-soft">{place.baseKind && BASE_POINT_KIND_LABELS[place.baseKind].label}</td>
                  )}
                  <td className="py-1 text-right tabular-nums">{formatNumber(place.x)}</td>
                  <td className="py-1 text-right tabular-nums">{formatNumber(place.y)}</td>
                  {spec.hasRadius && <td className="py-1 text-right tabular-nums">{formatNumber(place.radius ?? 0)}</td>}
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </Panel>
  );
}
