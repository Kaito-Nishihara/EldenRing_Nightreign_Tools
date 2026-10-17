import type { ReactNode } from "react";
import Button from "@/components/ui/Button";

// 読み込み中、空、エラーの表し方(画面の共通仕様 3.9)

/** 骨組みの表示。内容が入る場所に出す */
export function Skeleton({ className = "" }: { className?: string }) {
  return <div aria-hidden className={`animate-pulse bg-night-sunk ${className}`} />;
}

interface EmptyStateProps {
  /** 何がないか。例: この条件の記録はまだありません */
  title: string;
  description?: string;
  /** 次にできることの入口 */
  action?: ReactNode;
}

/** 空のときの文言と、次の操作の入口 */
export function EmptyState({ title, description, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center gap-3 border border-dashed border-line px-4 py-8 text-center">
      <span aria-hidden className="text-gold-deep">
        ◆ ◆ ◆
      </span>
      <p className="font-serif text-base font-extrabold text-gold-bright">{title}</p>
      {description && <p className="text-sm text-ash-soft">{description}</p>}
      {action}
    </div>
  );
}

interface ErrorNoticeProps {
  message: string;
  /** 「もう一度読み込む」を押したときの動き。渡さなければボタンを出さない */
  onRetry?: () => void;
}

/** エラーの知らせ。ember-soft の地、ember の枠、ash の文字 */
export function ErrorNotice({ message, onRetry }: ErrorNoticeProps) {
  return (
    <div className="flex flex-col gap-3">
      <div role="alert" className="flex gap-2 border border-ember bg-ember-soft px-3 py-2 text-sm text-ash">
        <span aria-hidden className="font-bold text-ember">
          !
        </span>
        <span>{message}</span>
      </div>
      {onRetry && <Button onClick={onRetry}>もう一度読み込む</Button>}
    </div>
  );
}
