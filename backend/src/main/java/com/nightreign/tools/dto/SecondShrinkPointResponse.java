package com.nightreign.tools.dto;

import com.nightreign.tools.entity.SecondShrinkPoint;

import java.math.BigDecimal;

/**
 * 第二収縮地点候補レスポンスDTO
 */
public record SecondShrinkPointResponse(
        Integer id,
        String nameJa,
        BigDecimal x,
        BigDecimal y
) {
    public static SecondShrinkPointResponse from(SecondShrinkPoint point) {
        return new SecondShrinkPointResponse(
                point.getId(),
                point.getNameJa(),
                point.getX(),
                point.getY()
        );
    }
}
