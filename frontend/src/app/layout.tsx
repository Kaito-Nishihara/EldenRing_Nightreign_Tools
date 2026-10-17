import type { Metadata } from "next";
import { Cinzel, Noto_Sans_JP, Shippori_Mincho_B1 } from "next/font/google";
import SiteHeader from "@/components/layout/SiteHeader";
import "./globals.css";

// 書体は next/font で自分のサイトから配る(画面の共通仕様 3.3)
const notoSansJp = Noto_Sans_JP({
  weight: ["400", "700"],
  subsets: ["latin"],
  variable: "--font-noto-sans-jp",
  display: "swap",
});

const shipporiMincho = Shippori_Mincho_B1({
  weight: ["600", "800"],
  subsets: ["latin"],
  variable: "--font-shippori-mincho",
  display: "swap",
});

// 英字の飾り(NIGHTREIGN、DAY I)だけに使う
const cinzel = Cinzel({
  weight: ["500", "700"],
  subsets: ["latin"],
  variable: "--font-cinzel",
  display: "swap",
});

export const metadata: Metadata = {
  title: "収縮パターン帳 | Nightreign Tools",
  description:
    "エルデンリング ナイトレインのマップ収縮(夜の雨)を記録し、同じ条件の過去のパターンを見るアプリ",
};

/**
 * ルートレイアウト。
 * 上に高さ 56px のヘッダーを固定し、残りの高さを各画面に渡す(画面の共通仕様 3.6)。
 */
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ja" className={`${notoSansJp.variable} ${shipporiMincho.variable} ${cinzel.variable}`}>
      <body className="flex h-dvh flex-col antialiased">
        <SiteHeader />
        <div className="min-h-0 flex-1 overflow-y-auto">{children}</div>
      </body>
    </html>
  );
}
