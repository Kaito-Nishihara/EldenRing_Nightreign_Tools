package com.nightreign.tools.dto;

import jakarta.validation.constraints.NotNull;
import java.math.BigDecimal;

/**
 * 拠点候補地の登録・更新リクエスト
 */
public record BasePointRequest(
        String nameJa,

        @NotNull(message = "X座標は必須です")
        BigDecimal x,

        @NotNull(message = "Y座標は必須です")
        BigDecimal y
) {
}
