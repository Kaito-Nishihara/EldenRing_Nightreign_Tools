import type { ReactNode } from "react";

export type BadgeTone = "neutral" | "gold" | "tide" | "grace";

const TONE_CLASSES: Record<BadgeTone, string> = {
  neutral: "border-line-strong text-ash-soft",
  gold: "border-gold text-gold",
  tide: "border-tide text-tide",
  grace: "border-grace text-grace",
};

/** 短い印(画面の共通仕様 3.7)。記録の数、DLC、弱点など */
export default function Badge({ tone = "neutral", children }: { tone?: BadgeTone; children: ReactNode }) {
  return <span className={`inline-flex items-center border px-2 text-xs ${TONE_CLASSES[tone]}`}>{children}</span>;
}
