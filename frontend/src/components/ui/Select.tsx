import { useId, type ComponentPropsWithoutRef } from "react";

interface SelectProps extends ComponentPropsWithoutRef<"select"> {
  /** 欄の名前。select と label を結ぶ */
  label: string;
  /** 欄の下に出す補足 */
  hint?: string;
}

/**
 * 名前付きの選択の欄(画面の共通仕様 3.7)。文字は 16px(iPhone の Safari が入力のときに拡大しないように)。
 */
export default function Select({ label, hint, id, className = "", children, ...props }: SelectProps) {
  const generatedId = useId();
  const selectId = id ?? generatedId;
  const hintId = hint ? `${selectId}-hint` : undefined;

  return (
    <div className={`flex flex-col gap-1 ${className}`}>
      <label htmlFor={selectId} className="text-sm text-ash-soft">
        {label}
      </label>
      {/* OS の矢印を消し、試作と同じ azure の下向きの三角を右端に置く */}
      <div className="relative">
        <select
          {...props}
          id={selectId}
          aria-describedby={hintId}
          className="min-h-11 w-full appearance-none border border-line-strong bg-night pl-3 pr-9 text-base text-ash transition-colors duration-200 ease-out hover:border-azure disabled:border-line disabled:text-ash-soft"
        >
          {children}
        </select>
        <span
          aria-hidden
          className="pointer-events-none absolute right-4 top-1/2 h-0 w-0 -translate-y-1/2 border-x-[5px] border-t-[6px] border-x-transparent border-t-azure"
        />
      </div>
      {hint && (
        <span id={hintId} className="text-xs text-ash-soft">
          {hint}
        </span>
      )}
    </div>
  );
}
