interface SectionHeadingProps {
  /** 英字の飾り。例: CONDITIONS */
  eyebrow: string;
  /** 日本語の見出し。例: 条件 */
  title: string;
  /** 見出しの要素の id。パネルの aria-labelledby から指す */
  id?: string;
  /** 見出しの段。既定は h2 */
  level?: "h1" | "h2" | "h3";
}

/**
 * 英字の飾りと日本語の見出しの組(画面の共通仕様 3.6)。
 * 英字は text-xs の ash-soft、見出しは font-serif の azure-bright。
 */
export default function SectionHeading({ eyebrow, title, id, level = "h2" }: SectionHeadingProps) {
  const Heading = level;
  return (
    <div className="mb-4">
      <span aria-hidden className="block font-display text-xs uppercase tracking-display text-ash-soft">
        {eyebrow}
      </span>
      <Heading id={id} className="font-serif text-lg font-extrabold text-azure-bright">
        {title}
      </Heading>
    </div>
  );
}
