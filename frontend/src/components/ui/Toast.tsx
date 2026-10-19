"use client";

import { useEffect, type ReactNode } from "react";

/** 知らせを出しておく時間(画面の共通仕様 3.9) */
export const TOAST_DURATION_MS = 4000;

interface ToastProps {
  /** 知らせの文言。例: 記録を保存しました */
  message: string;
  /** 文言の後ろに置く入口。例: 「パターンを見る」のリンク */
  action?: ReactNode;
  /** 消えるときの動き。4 秒たつか、閉じるを押すと呼ぶ */
  onDismiss: () => void;
}

/**
 * 画面の下に出す短い知らせ(画面の共通仕様 3.7、3.9)。grace の印を添え、4 秒で消す。
 * 狭い幅では、ボトムシートの取っ手に重ならないよう、取っ手の上に出す。
 */
export default function Toast({ message, action, onDismiss }: ToastProps) {
  useEffect(() => {
    const timer = setTimeout(onDismiss, TOAST_DURATION_MS);
    return () => clearTimeout(timer);
  }, [onDismiss]);

  return (
    <div
      role="status"
      className="fixed inset-x-4 bottom-20 z-30 flex items-center gap-3 border border-grace bg-night-raised px-4 py-3 text-sm text-ash shadow-glow md:bottom-6 md:left-auto md:right-6 md:max-w-sm"
    >
      <span aria-hidden className="font-bold text-grace">
        ✓
      </span>
      <span className="flex-1">{message}</span>
      {action}
      <button type="button" onClick={onDismiss} aria-label="知らせを閉じる" className="min-h-11 min-w-11 text-ash-soft hover:text-ash">
        ×
      </button>
    </div>
  );
}
