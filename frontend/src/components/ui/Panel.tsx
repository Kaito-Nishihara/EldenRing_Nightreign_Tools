import type { ComponentPropsWithoutRef } from "react";

interface PanelProps extends ComponentPropsWithoutRef<"section"> {
  /** 四隅に飾り枠を付ける(画面の共通仕様 3.4)。条件のパネル、マップの枠、ダイアログだけに使う */
  ornate?: boolean;
}

/** 飾り枠の 1 つの角。長さ 12px の azure-deep の鉤形の線 */
const CORNERS = [
  "-left-px -top-px border-l border-t",
  "-right-px -top-px border-r border-t",
  "-bottom-px -left-px border-b border-l",
  "-bottom-px -right-px border-b border-r",
] as const;

/**
 * night-raised の地と line の枠のパネル(画面の共通仕様 3.7)。
 */
export default function Panel({ ornate = false, className = "", children, ...props }: PanelProps) {
  return (
    <section
      {...props}
      className={`relative border border-line bg-night-raised p-4 md:p-6 ${ornate ? "shadow-glow" : ""} ${className}`}
    >
      {ornate &&
        CORNERS.map((position) => (
          <span key={position} aria-hidden className={`pointer-events-none absolute h-3 w-3 border-azure-deep ${position}`} />
        ))}
      {children}
    </section>
  );
}
