/**
 * パターン検索のマップの凡例(画面の試作)。マップの左下に重ねる。
 * 印の色は選んだパターンの色にする。
 */
export default function MapLegend({ color }: { color: string }) {
  return (
    <div
      aria-hidden
      className="pointer-events-none absolute bottom-2 left-2 flex max-w-[calc(100%-5rem)] flex-wrap gap-x-3 gap-y-1 border border-line bg-night/85 px-2 py-1 text-xs text-ash-soft"
    >
      <span className="flex items-center gap-1">
        <i className="inline-block h-2.5 w-2.5 rounded-full border-[1.5px] border-azure-bright" />
        開始地点
      </span>
      <span className="flex items-center gap-1">
        <i className="inline-block h-2.5 w-2.5 rounded-full border-2" style={{ borderColor: color }} />
        第 1 次の収縮
      </span>
      <span className="flex items-center gap-1">
        <i className="inline-block h-2 w-2 rotate-45" style={{ backgroundColor: color }} />
        第 2 次の収縮
      </span>
      <span>I ／ II は日</span>
    </div>
  );
}
