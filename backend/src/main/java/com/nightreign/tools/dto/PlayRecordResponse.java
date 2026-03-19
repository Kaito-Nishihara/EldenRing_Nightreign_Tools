package com.nightreign.tools.dto;

import java.time.LocalDateTime;

/**
 * プレイ記録作成レスポンスDTO
 */
public record PlayRecordResponse(
        Integer id,
        LocalDateTime createdAt
) {
}
