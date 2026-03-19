package com.nightreign.tools.dto;

import java.time.LocalDateTime;
import java.util.List;

/**
 * パターンレスポンスDTO
 * 同一収縮パターンの記録件数と詳細
 */
public record PatternResponse(
        Integer patternIndex,
        Long recordCount,
        LocalDateTime lastRecordedAt,
        List<PatternEntryResponse> entries
) {
}
