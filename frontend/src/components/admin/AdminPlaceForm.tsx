"use client";

import { useId, type FormEvent, type ReactNode } from "react";
import Button from "@/components/ui/Button";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import Select from "@/components/ui/Select";
import { ErrorNotice } from "@/components/ui/StatusViews";
import type { BasePointKind } from "@/types";
import {
  BASE_POINT_KIND_LABELS,
  BASE_POINT_KIND_ORDER,
  RADIUS_SLIDER_MAX,
  RADIUS_SLIDER_MIN,
  formTitle,
  type AdminPlace,
  type AdminPlaceErrors,
  type AdminPlaceInput,
  type AdminPlaceKindSpec,
} from "@/lib/adminPlaces";

/** 非表示にすると何が起きるか(マスタの管理 3.3) */
export const HIDE_DESCRIPTION = "プレイヤーの画面と、新しい記録の入力から外れます。過去の記録には残ります";

interface AdminPlaceFormProps {
  spec: AdminPlaceKindSpec;
  editing: AdminPlace | null;
  input: AdminPlaceInput;
  errors: AdminPlaceErrors;
  /** 保存か非表示を送っている間 */
  pending: boolean;
  /** 送って返ってきた誤り */
  actionError: string | null;
  onChange: (patch: Partial<AdminPlaceInput>) => void;
  onSave: () => void;
  onCancel: () => void;
  /** 「非表示にする」を押したとき。確かめの枠は呼ぶ側が出す */
  onHide: () => void;
  onRestore: () => void;
}

const INPUT_CLASS =
  "min-h-11 w-full border bg-night px-3 text-base text-ash hover:border-azure";

/** 名前付きの入力の欄。誤りがあれば欄の下に出し、欄と結ぶ */
function Field({ label, error, children, id }: { label: string; error?: string; children: ReactNode; id: string }) {
  return (
    <div className="flex min-w-0 flex-1 flex-col gap-1">
      <label htmlFor={id} className="text-sm text-ash-soft">
        {label}
      </label>
      {children}
      {error && (
        <span id={`${id}-error`} className="text-xs text-ember">
          {error}
        </span>
      )}
    </div>
  );
}

/**
 * 登録と修正の入力の枠(マスタの管理 3.3、3.4)。
 * 直している行があるときだけ「非表示にする」か「表示に戻す」を出す。行を消すボタンは置かない(決定の記録 0013)。
 */
export default function AdminPlaceForm({
  spec,
  editing,
  input,
  errors,
  pending,
  actionError,
  onChange,
  onSave,
  onCancel,
  onHide,
  onRestore,
}: AdminPlaceFormProps) {
  const headingId = useId();
  const nameId = useId();
  const xId = useId();
  const yId = useId();
  const radiusId = useId();

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    onSave();
  };

  const fieldProps = (id: string, error?: string) => ({
    id,
    "aria-invalid": error ? true : undefined,
    "aria-describedby": error ? `${id}-error` : undefined,
    className: `${INPUT_CLASS} ${error ? "border-ember" : "border-line-strong"}`,
  });

  return (
    <Panel ornate aria-labelledby={headingId}>
      <SectionHeading eyebrow={editing ? "Edit" : "New"} title={formTitle(spec, editing)} id={headingId} />
      <form className="flex flex-col gap-3" onSubmit={handleSubmit} noValidate>
        {spec.hasBaseKind && (
          <Select
            label="種類"
            value={input.baseKind}
            onChange={(e) => onChange({ baseKind: e.target.value as BasePointKind })}
          >
            {BASE_POINT_KIND_ORDER.map((value) => (
              <option key={value} value={value}>
                {BASE_POINT_KIND_LABELS[value].label}
              </option>
            ))}
          </Select>
        )}
        <Field label={spec.nameRequired ? "名前" : "名前(任意)"} error={errors.nameJa} id={nameId}>
          <input {...fieldProps(nameId, errors.nameJa)} value={input.nameJa} onChange={(e) => onChange({ nameJa: e.target.value })} />
        </Field>
        <div className="flex gap-3">
          <Field label="X" error={errors.x} id={xId}>
            <input
              {...fieldProps(xId, errors.x)}
              inputMode="decimal"
              value={input.x}
              onChange={(e) => onChange({ x: e.target.value })}
            />
          </Field>
          <Field label="Y" error={errors.y} id={yId}>
            <input
              {...fieldProps(yId, errors.y)}
              inputMode="decimal"
              value={input.y}
              onChange={(e) => onChange({ y: e.target.value })}
            />
          </Field>
        </div>
        {spec.hasRadius && (
          <Field label={`半径 ${input.radius}`} error={errors.radius} id={radiusId}>
            <input
              id={radiusId}
              type="range"
              min={RADIUS_SLIDER_MIN}
              max={RADIUS_SLIDER_MAX}
              value={input.radius}
              onChange={(e) => onChange({ radius: Number(e.target.value) })}
              className="w-full accent-azure"
            />
          </Field>
        )}
        {editing && <p className="text-xs text-ash-soft">非表示にすると、{HIDE_DESCRIPTION}。</p>}
        {actionError && <ErrorNotice message={actionError} />}
        <div className="flex flex-wrap gap-2">
          {editing &&
            (editing.isHidden ? (
              <Button onClick={onRestore} disabled={pending}>
                表示に戻す
              </Button>
            ) : (
              <Button variant="danger" onClick={onHide} disabled={pending}>
                非表示にする
              </Button>
            ))}
          <Button onClick={onCancel} disabled={pending}>
            取り消す
          </Button>
          <Button type="submit" variant="primary" disabled={pending} className="flex-1">
            {pending ? "送っています" : "保存する"}
          </Button>
        </div>
      </form>
    </Panel>
  );
}
