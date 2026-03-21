package com.nightreign.tools.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;

import java.math.BigDecimal;

/**
 * 第二収縮地点候補の登録・更新リクエスト
 */
public record SecondShrinkPointRequest(
        @NotBlank(message = "日本語名は必須です")
        String nameJa,

        @NotNull(message = "X座標は必須です")
        BigDecimal x,

        @NotNull(message = "Y座標は必須です")
        BigDecimal y
) {
}
