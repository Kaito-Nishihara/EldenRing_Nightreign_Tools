package com.nightreign.tools.dto;

import com.nightreign.tools.entity.RelicEffectRank;
import java.math.BigDecimal;

/**
 * 遺物効果ランク別数値レスポンスDTO
 */
public record RelicEffectRankResponse(
        Short rank,
        BigDecimal value,
        String unit,
        boolean hasDemerit
) {
    /** EntityからDTOに変換する */
    public static RelicEffectRankResponse from(RelicEffectRank r) {
        return new RelicEffectRankResponse(r.getRank(), r.getValue(), r.getUnit(), r.isHasDemerit());
    }
}
