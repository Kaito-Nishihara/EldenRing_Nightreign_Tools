"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAdminKey } from "@/hooks/useAdminKey";
import { clearAdminKey } from "@/lib/adminKey";

/** ヘッダーの入口(画面の共通仕様 3.6)。管理の画面の入口は出さない */
const NAV_ITEMS = [
  { href: "/", label: "パターン" },
  { href: "/record", label: "記録する" },
  { href: "/build", label: "ビルド" },
] as const;

/**
 * 全画面に共通のヘッダー。高さ 56px。
 * 左に英字の飾りと題、右に 3 つの入口。今いる画面の入口は青の文字と下線にし、aria-current を付ける。
 * 管理の画面では入口を出さず、ADMIN の印を出す。鍵を置いているときは、隣に「鍵を消す」を出す(マスタの管理 3.1)。
 */
export default function SiteHeader() {
  const pathname = usePathname();
  const isAdmin = pathname.startsWith("/admin");
  const adminKey = useAdminKey();

  return (
    <header className="flex h-14 flex-none items-center justify-between gap-3 border-b border-line bg-night/90 px-3 md:px-6">
      <Link href="/" className="flex min-w-0 items-baseline gap-2 md:gap-3">
        <span className="font-display text-sm font-bold tracking-display text-azure-bright md:text-base">
          NIGHTREIGN
        </span>
        <span className="hidden truncate font-serif text-sm text-ash-soft sm:inline">収縮パターン帳</span>
      </Link>

      {isAdmin ? (
        <div className="flex items-center gap-3">
          <span className="border border-ember px-2 font-display text-xs tracking-display text-ember">ADMIN</span>
          {adminKey && (
            <button
              type="button"
              onClick={clearAdminKey}
              className="min-h-11 px-2 text-sm text-ash-soft transition-colors duration-200 ease-out hover:text-azure-bright"
            >
              鍵を消す
            </button>
          )}
        </div>
      ) : (
        <nav aria-label="画面" className="flex h-full items-stretch">
          {NAV_ITEMS.map((item) => {
            const current = pathname === item.href;
            return (
              <Link
                key={item.href}
                href={item.href}
                aria-current={current ? "page" : undefined}
                className={`flex items-center whitespace-nowrap border-b-2 px-2 text-sm transition-colors duration-200 ease-out md:px-4 ${
                  current ? "border-azure text-azure" : "border-transparent text-ash-soft hover:text-azure-bright"
                }`}
              >
                {item.label}
              </Link>
            );
          })}
        </nav>
      )}
    </header>
  );
}
