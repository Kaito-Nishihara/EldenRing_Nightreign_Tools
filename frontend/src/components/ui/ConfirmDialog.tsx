"use client";

import { useEffect, useId, useRef } from "react";
import Button from "@/components/ui/Button";
import Panel from "@/components/ui/Panel";

interface ConfirmDialogProps {
  title: string;
  message: string;
  /** 確かめるボタンの文言。例: 非表示にする */
  confirmLabel: string;
  onConfirm: () => void;
  onCancel: () => void;
  /** 送っている間は、どちらのボタンも押せなくする */
  busy?: boolean;
}

/**
 * 取り返しにくい操作の前に出す確かめの枠(画面の共通仕様 3.4 の飾り枠を付ける)。
 * 開いたら「やめる」に移り、Esc でも閉じる。
 */
export default function ConfirmDialog({ title, message, confirmLabel, onConfirm, onCancel, busy = false }: ConfirmDialogProps) {
  const titleId = useId();
  const messageId = useId();
  const actionsRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // 先頭のボタン(やめる)に移る
    actionsRef.current?.querySelector("button")?.focus();
    const handleKey = (event: KeyboardEvent) => {
      if (event.key === "Escape") onCancel();
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [onCancel]);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-abyss/80 p-4">
      <Panel
        ornate
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        aria-describedby={messageId}
        className="w-full max-w-md"
      >
        <h2 id={titleId} className="mb-3 font-serif text-lg font-extrabold text-azure-bright">
          {title}
        </h2>
        <p id={messageId} className="mb-4 text-sm text-ash">
          {message}
        </p>
        <div ref={actionsRef} className="flex gap-2">
          <Button onClick={onCancel} disabled={busy} className="flex-1">
            やめる
          </Button>
          <Button variant="danger" onClick={onConfirm} disabled={busy} className="flex-1">
            {confirmLabel}
          </Button>
        </div>
      </Panel>
    </div>
  );
}
