package com.nightreign.tools.dto;

import com.nightreign.tools.entity.Jihen;
import java.math.BigDecimal;

/**
 * 地変レスポンスDTO
 */
public record JihenResponse(
        String id,
        String nameJa,
        String imagePath,
        String area,
        BigDecimal centerX,
        BigDecimal centerY,
        boolean isDlc,
        String unlockCondition
) {
    /** EntityからDTOに変換する */
    public static JihenResponse from(Jihen jihen) {
        return new JihenResponse(
                jihen.getId(),
                jihen.getNameJa(),
                jihen.getImagePath(),
                jihen.getArea(),
                jihen.getCenterX(),
                jihen.getCenterY(),
                jihen.isDlc(),
                jihen.getUnlockCondition()
        );
    }
}
