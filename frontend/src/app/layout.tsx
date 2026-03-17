import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Nightreign Tools - マップ収縮パターン検索",
  description:
    "エルデンリング ナイトレインのマップ収縮パターンを蓄積・検索するWebアプリ",
};

/**
 * ルートレイアウト
 * アプリケーション全体の共通レイアウトを定義する
 */
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ja">
      <body className="antialiased">{children}</body>
    </html>
  );
}
