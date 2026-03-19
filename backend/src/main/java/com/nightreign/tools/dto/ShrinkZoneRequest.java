package com.nightreign.tools.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Positive;
import java.math.BigDecimal;

/**
 * 収縮候補地の登録・更新リクエスト
 */
public record ShrinkZoneRequest(
        @NotBlank(message = "日本語名は必須です")
        String nameJa,

        String iconPath,

        @NotNull(message = "中心X座標は必須です")
        BigDecimal centerX,

        @NotNull(message = "中心Y座標は必須です")
        BigDecimal centerY,

        @NotNull(message = "半径は必須です")
        @Positive(message = "半径は正の値である必要があります")
        BigDecimal radius
) {
}
