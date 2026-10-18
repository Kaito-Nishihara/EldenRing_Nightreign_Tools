"use client";

import { useId, useState } from "react";
import Panel from "@/components/ui/Panel";
import Button from "@/components/ui/Button";
import { ErrorNotice } from "@/components/ui/StatusViews";
import { NOTE_MAX_LENGTH } from "@/lib/recordDraft";
import type { SaveState } from "@/hooks/usePlayRecordForm";

interface RecordSavePanelProps {
  note: string;
  /** 保存できない理由。空なら保存できる */
  missing: string[];
  save: SaveState;
  onNoteChange: (note: string) => void;
  onSubmit: () => void;
  /** 「やり直す」。確かめてから呼ぶ */
  onReset: () => void;
}

/**
 * 備考と保存(プレイ記録 3.5)。
 * 「記録を保存する」は、足りないものがあるときは使えず、ボタンの下に足りないものを出す(AC-REC-12)。
 * 「やり直す」は、確かめを挟んでから入力を空にする。
 */
export default function RecordSavePanel({ note, missing, save, onNoteChange, onSubmit, onReset }: RecordSavePanelProps) {
  const [confirming, setConfirming] = useState(false);
  const noteId = useId();
  const reasonsId = useId();
  const saving = save.kind === "saving";
  const noteLength = note.trim().length;

  return (
    <Panel aria-label="備考と保存">
      <div className="flex flex-col gap-3">
        <div className="flex flex-col gap-1">
          <label htmlFor={noteId} className="text-sm text-ash-soft">
            備考(任意、{NOTE_MAX_LENGTH} 文字まで)
          </label>
          {/* 上限を超えても入力は止めない。保存できなくし、下に今の文字数を出す */}
          <textarea
            id={noteId}
            value={note}
            onChange={(e) => onNoteChange(e.target.value)}
            rows={3}
            placeholder="例: 2 日目は地変の方へ寄った"
            aria-invalid={noteLength > NOTE_MAX_LENGTH}
            className="w-full resize-y border border-line-strong bg-night px-3 py-2 text-base text-ash placeholder:text-ash-soft hover:border-azure"
          />
        </div>

        <p className="text-xs text-ash-soft">入力中の内容はこの端末に残ります</p>

        {save.kind === "failed" && <ErrorNotice message={save.message} />}

        {confirming ? (
          <div role="group" aria-label="やり直しの確かめ" className="flex flex-col gap-2 border border-line-strong p-3">
            <p className="text-sm text-ash">入れた収縮先と夜ボスと備考を空にします。条件は残ります。</p>
            <div className="flex gap-2">
              <Button onClick={() => setConfirming(false)}>やめる</Button>
              <Button
                variant="danger"
                className="flex-1"
                onClick={() => {
                  onReset();
                  setConfirming(false);
                }}
              >
                空にする
              </Button>
            </div>
          </div>
        ) : (
          <div className="flex gap-2">
            <Button onClick={() => setConfirming(true)} disabled={saving}>
              やり直す
            </Button>
            <Button
              variant="primary"
              className="flex-1"
              onClick={onSubmit}
              disabled={saving || missing.length > 0}
              aria-describedby={missing.length > 0 ? reasonsId : undefined}
            >
              {saving ? "保存しています" : "記録を保存する"}
            </Button>
          </div>
        )}

        {missing.length > 0 && (
          <ul id={reasonsId} className="flex flex-col gap-1 text-sm text-ash-soft">
            {missing.map((reason) => (
              <li key={reason}>{reason}</li>
            ))}
          </ul>
        )}
      </div>
    </Panel>
  );
}
