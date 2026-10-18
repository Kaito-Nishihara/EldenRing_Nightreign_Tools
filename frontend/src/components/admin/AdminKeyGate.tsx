"use client";

import { useId, useState, type FormEvent } from "react";
import Button from "@/components/ui/Button";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import { ErrorNotice } from "@/components/ui/StatusViews";
import { useAdminKeyEntry } from "@/hooks/useAdminKey";

/**
 * 管理用の鍵の入力の欄(マスタの管理 3.1)。鍵がないときは、管理の画面にこれだけを出す。
 * 鍵が合えば鍵を置き、管理の画面は置いた鍵を見て切り替わる。
 */
export default function AdminKeyGate() {
  const { submit, checking, error } = useAdminKeyEntry();
  const [key, setKey] = useState("");
  const inputId = useId();
  const headingId = useId();

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    if (key.length > 0) void submit(key);
  };

  return (
    <div className="flex h-full items-center justify-center p-4">
      <Panel ornate aria-labelledby={headingId} className="w-full max-w-sm">
        <SectionHeading eyebrow="Admin Key" title="管理用の鍵" id={headingId} />
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          <div className="flex flex-col gap-1">
            <label htmlFor={inputId} className="text-sm text-ash-soft">
              管理用の鍵
            </label>
            <input
              id={inputId}
              type="password"
              autoComplete="current-password"
              value={key}
              onChange={(event) => setKey(event.target.value)}
              className="min-h-11 w-full border border-line-strong bg-night px-3 text-base text-ash hover:border-azure"
            />
          </div>
          {error && <ErrorNotice message={error} />}
          <Button type="submit" variant="primary" disabled={checking || key.length === 0}>
            {checking ? "確かめています" : "入る"}
          </Button>
        </form>
      </Panel>
    </div>
  );
}
