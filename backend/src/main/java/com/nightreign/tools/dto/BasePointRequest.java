package com.nightreign.tools.dto;

import jakarta.validation.constraints.NotNull;
import java.math.BigDecimal;

/**
 * 拠点候補地の登録・更新リクエスト
 */
public record BasePointRequest(
        /** マップ識別子（normal, kakou, sankan, fuhai, nokurateou） */
        @NotNull(message = "マップIDは必須です")
        String mapId,

        String nameJa,

        @NotNull(message = "X座標は必須です")
        BigDecimal x,

        @NotNull(message = "Y座標は必須です")
        BigDecimal y
) {
}
