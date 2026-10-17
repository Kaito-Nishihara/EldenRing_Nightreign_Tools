"use client";

import { useId, useState, type ReactNode } from "react";

interface BottomSheetProps {
  /** 畳んだときに取っ手に出す文言。例: 条件とパターンを開く */
  handleLabel: string;
  /** 初めに開いておくか */
  defaultOpen?: boolean;
  children: ReactNode;
}

/**
 * 狭い幅(767px まで)の操作の枠(画面の共通仕様 3.5、3.7)。下から引き出して使う。
 * 広い幅(768px から)では枠を作らず、中身をそのまま出す(左のパネルとして並べる)。
 */
export default function BottomSheet({ handleLabel, defaultOpen = false, children }: BottomSheetProps) {
  const [open, setOpen] = useState(defaultOpen);
  const contentId = useId();

  return (
    <div className="fixed inset-x-0 bottom-0 z-20 flex max-h-[75dvh] flex-col border-t border-gold-deep bg-night-raised md:static md:z-auto md:max-h-none md:border-0 md:bg-transparent">
      <button
        type="button"
        aria-expanded={open}
        aria-controls={contentId}
        onClick={() => setOpen((value) => !value)}
        className="flex min-h-11 flex-none flex-col items-center gap-1 py-2 text-sm text-ash-soft md:hidden"
      >
        <span aria-hidden className="h-1 w-10 bg-line-strong" />
        <span>
          {handleLabel} ／ {open ? "閉じる" : "開く"}
        </span>
      </button>
      <div id={contentId} className={`${open ? "block" : "hidden"} min-h-0 overflow-y-auto md:block md:overflow-visible`}>
        {children}
      </div>
    </div>
  );
}
