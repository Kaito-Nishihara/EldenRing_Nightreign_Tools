package com.nightreign.tools.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import java.math.BigDecimal;

/**
 * 開始地点の登録・更新リクエスト
 */
public record SpawnPointRequest(
        @NotBlank(message = "日本語名は必須です")
        String nameJa,

        @NotNull(message = "X座標は必須です")
        BigDecimal x,

        @NotNull(message = "Y座標は必須です")
        BigDecimal y,

        @NotBlank(message = "拠点タイプは必須です")
        @Pattern(regexp = "church|camp", message = "拠点タイプはchurchまたはcampのみ有効です")
        String nearbyBaseType
) {
}
