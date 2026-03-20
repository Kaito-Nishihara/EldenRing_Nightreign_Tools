package com.nightreign.tools.dto;

import com.nightreign.tools.entity.BasePoint;
import java.math.BigDecimal;

/**
 * 拠点候補地レスポンスDTO
 */
public record BasePointResponse(
        Integer id,
        String mapId,
        String nameJa,
        BigDecimal x,
        BigDecimal y
) {
    /** EntityからDTOに変換する */
    public static BasePointResponse from(BasePoint bp) {
        return new BasePointResponse(
                bp.getId(),
                bp.getMapId(),
                bp.getNameJa(),
                bp.getX(),
                bp.getY()
        );
    }
}
