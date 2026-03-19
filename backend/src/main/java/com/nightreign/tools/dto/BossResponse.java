package com.nightreign.tools.dto;

import com.nightreign.tools.entity.Boss;
import java.util.List;

/**
 * ボスレスポンスDTO
 */
public record BossResponse(
        String id,
        String nameJa,
        String nameEn,
        String imagePath,
        List<String> weaknesses,
        boolean isDlc,
        String unlockCondition
) {
    /** EntityからDTOに変換する */
    public static BossResponse from(Boss boss) {
        List<String> weaknessList = boss.getWeaknesses().stream()
                .map(w -> w.getAttribute())
                .toList();
        return new BossResponse(
                boss.getId(),
                boss.getNameJa(),
                boss.getNameEn(),
                boss.getImagePath(),
                weaknessList,
                boss.isDlc(),
                boss.getUnlockCondition()
        );
    }
}
