package com.nightreign.tools.dto;

import com.nightreign.tools.entity.SpawnPoint;
import java.math.BigDecimal;

/**
 * 開始地点レスポンスDTO
 */
public record SpawnPointResponse(
        Integer id,
        String nameJa,
        BigDecimal x,
        BigDecimal y,
        String nearbyBaseType
) {
    /** EntityからDTOに変換する */
    public static SpawnPointResponse from(SpawnPoint sp) {
        return new SpawnPointResponse(
                sp.getId(),
                sp.getNameJa(),
                sp.getX(),
                sp.getY(),
                sp.getNearbyBaseType()
        );
    }
}
