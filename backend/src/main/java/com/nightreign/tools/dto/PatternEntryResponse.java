package com.nightreign.tools.dto;

/**
 * パターンエントリレスポンスDTO
 * 各フェーズの収縮地点情報
 */
public record PatternEntryResponse(
        Integer day,
        Integer phase,
        ShrinkZoneResponse zone
) {
}
