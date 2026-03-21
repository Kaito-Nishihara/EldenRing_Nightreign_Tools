package com.nightreign.tools.dto;

import com.nightreign.tools.entity.NightBoss;

/**
 * 夜ボスレスポンスDTO
 */
public record NightBossResponse(
        Integer id,
        String nameJa,
        Integer day
) {
    public static NightBossResponse from(NightBoss nightBoss) {
        return new NightBossResponse(
                nightBoss.getId(),
                nightBoss.getNameJa(),
                nightBoss.getDay()
        );
    }
}
