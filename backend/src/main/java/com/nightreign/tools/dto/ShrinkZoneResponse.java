package com.nightreign.tools.dto;

import com.nightreign.tools.entity.ShrinkZone;
import java.math.BigDecimal;

/**
 * 収縮候補地レスポンスDTO
 */
public record ShrinkZoneResponse(
        Integer id,
        String nameJa,
        String iconPath,
        BigDecimal centerX,
        BigDecimal centerY,
        BigDecimal radius
) {
    /** EntityからDTOに変換する */
    public static ShrinkZoneResponse from(ShrinkZone zone) {
        return new ShrinkZoneResponse(
                zone.getId(),
                zone.getNameJa(),
                zone.getIconPath(),
                zone.getCenterX(),
                zone.getCenterY(),
                zone.getRadius()
        );
    }
}
