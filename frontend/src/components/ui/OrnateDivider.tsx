/**
 * 飾りの区切り線(画面の共通仕様 3.4)。両端が透けて消える gold-deep の 1px の線で、中央に小さな菱形を置く。
 */
export default function OrnateDivider() {
  return (
    <div aria-hidden className="my-4 flex items-center gap-3 text-xs text-gold-deep">
      <span className="h-px flex-1 bg-gradient-to-r from-transparent to-gold-deep" />
      <span>◆</span>
      <span className="h-px flex-1 bg-gradient-to-l from-transparent to-gold-deep" />
    </div>
  );
}
