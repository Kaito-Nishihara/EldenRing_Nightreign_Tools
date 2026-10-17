"use client";

interface SegmentedControlOption<T extends string> {
  value: T;
  label: string;
  /** 英字の飾りの書体(DAY I など)で出す */
  display?: boolean;
}

interface SegmentedControlProps<T extends string> {
  options: readonly SegmentedControlOption<T>[];
  value: T;
  onChange: (value: T) => void;
  /** 何を切り替えるか。読み上げソフトに伝える */
  ariaLabel: string;
  className?: string;
}

/**
 * 2 から 4 の選択肢を横に並べた切り替え(画面の共通仕様 3.7)。1 日目と 2 日目の切り替えなどに使う。
 * 選んだものは金の文字と下の線にし、aria-pressed を付ける。色だけで選んだことを示さない。
 */
export default function SegmentedControl<T extends string>({
  options,
  value,
  onChange,
  ariaLabel,
  className = "",
}: SegmentedControlProps<T>) {
  return (
    <div role="group" aria-label={ariaLabel} className={`inline-flex border border-line-strong ${className}`}>
      {options.map((option) => {
        const pressed = option.value === value;
        return (
          <button
            key={option.value}
            type="button"
            aria-pressed={pressed}
            onClick={() => onChange(option.value)}
            className={`min-h-11 flex-1 whitespace-nowrap border-b-2 px-3 text-sm transition-colors duration-200 ease-out ${
              option.display ? "font-display tracking-display" : ""
            } ${pressed ? "border-gold bg-night-sunk text-gold" : "border-transparent text-ash-soft hover:text-gold-bright"}`}
          >
            {option.label}
          </button>
        );
      })}
    </div>
  );
}
