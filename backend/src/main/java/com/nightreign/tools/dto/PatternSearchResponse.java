package com.nightreign.tools.dto;

import java.util.List;

/**
 * パターン検索レスポンスDTO
 * 検索結果全体を包含する
 */
public record PatternSearchResponse(
        Long totalRecords,
        List<PatternResponse> patterns
) {
}
