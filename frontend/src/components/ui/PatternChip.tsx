import { getPatternColor } from "@/lib/theme";

/**
 * パターンの番号の丸(画面の共通仕様 3.2)。パターンの色の丸の中に、night の文字で番号を書く。
 * 色だけでパターンを見分けさせないためである。
 */
export default function PatternChip({ patternIndex }: { patternIndex: number }) {
  return (
    <span
      aria-label={`パターン ${patternIndex}`}
      className="inline-flex h-7 w-7 flex-none items-center justify-center rounded-full font-display text-sm font-bold text-night"
      style={{ backgroundColor: getPatternColor(patternIndex - 1) }}
    >
      {patternIndex}
    </span>
  );
}
