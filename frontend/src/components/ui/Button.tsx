import type { ComponentPropsWithoutRef } from "react";

export type ButtonVariant = "primary" | "secondary" | "danger" | "ghost";

/**
 * ボタンの種類ごとの見た目(画面の共通仕様 3.7)。
 * 使えない状態は、文字を ash-soft、枠を line にする。透明度で薄くしない。
 */
const VARIANT_CLASSES: Record<ButtonVariant, string> = {
  primary:
    "bg-gold text-night font-bold hover:bg-gold-bright hover:shadow-glow-strong disabled:bg-night-sunk disabled:text-ash-soft disabled:shadow-none",
  secondary:
    "border border-line-strong text-ash hover:border-gold hover:text-gold disabled:border-line disabled:text-ash-soft",
  danger: "border border-ember text-ember hover:bg-ember-soft disabled:border-line disabled:text-ash-soft disabled:bg-transparent",
  ghost: "text-gold hover:underline disabled:text-ash-soft disabled:no-underline",
};

/** ボタンと、ボタンの見た目のリンクで共通の形。高さは 44px 以上(指で押しやすくするため) */
export function buttonClassName(variant: ButtonVariant, className = ""): string {
  return `inline-flex min-h-11 items-center justify-center gap-2 whitespace-nowrap px-4 text-base transition-[color,background-color,border-color,box-shadow] duration-200 ease-out disabled:cursor-not-allowed ${VARIANT_CLASSES[variant]} ${className}`;
}

interface ButtonProps extends ComponentPropsWithoutRef<"button"> {
  variant?: ButtonVariant;
}

/**
 * 主(primary)、副(secondary)、危険(danger)、文字だけ(ghost)の 4 種のボタン。
 * 1 つの画面の 1 つの区画に、主のボタンは 1 つまでとする。
 */
export default function Button({ variant = "secondary", className = "", type = "button", ...props }: ButtonProps) {
  return <button type={type} {...props} className={buttonClassName(variant, className)} />;
}
