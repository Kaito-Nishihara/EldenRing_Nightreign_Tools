package com.nightreign.tools.dto;

import com.nightreign.tools.entity.RelicEffect;
import java.util.List;

/**
 * 遺物効果レスポンスDTO
 */
public record RelicEffectResponse(
        Integer id,
        String categoryId,
        String categoryNameJa,
        String nameJa,
        boolean isDeep,
        String effectSign,
        String calcType,
        String attribute,
        String stackable,
        Short rankMin,
        Short rankMax,
        String conditionJa,
        String notesJa,
        List<RelicEffectRankResponse> ranks
) {
    /** EntityからDTOに変換する */
    public static RelicEffectResponse from(RelicEffect e) {
        return new RelicEffectResponse(
                e.getId(),
                e.getCategory().getId(),
                e.getCategory().getNameJa(),
                e.getNameJa(),
                e.isDeep(),
                e.getEffectSign(),
                e.getCalcType(),
                e.getAttribute(),
                e.getStackable(),
                e.getRankMin(),
                e.getRankMax(),
                e.getConditionJa(),
                e.getNotesJa(),
                e.getRanks().stream().map(RelicEffectRankResponse::from).toList()
        );
    }
}
