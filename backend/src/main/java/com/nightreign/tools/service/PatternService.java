package com.nightreign.tools.service;

import com.nightreign.tools.dto.PatternEntryResponse;
import com.nightreign.tools.dto.PatternResponse;
import com.nightreign.tools.dto.PatternSearchResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.entity.PlayRecord;
import com.nightreign.tools.entity.ShrinkEntry;
import com.nightreign.tools.repository.PlayRecordRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.stream.Collectors;

/**
 * パターン検索サービス
 * 条件に一致するプレイ記録から収縮パターンを集計する
 */
@Service
@Transactional(readOnly = true)
public class PatternService {

    private final PlayRecordRepository playRecordRepository;

    public PatternService(PlayRecordRepository playRecordRepository) {
        this.playRecordRepository = playRecordRepository;
    }

    /**
     * 条件に一致するパターンを検索する
     * 同一の収縮パターン（day+phase+zoneIdの組み合わせ）をグルーピングし、
     * 記録件数と最終記録日時を含めて返す
     */
    public PatternSearchResponse searchPatterns(String bossId, Integer spawnPointId, String jihenId) {
        List<PlayRecord> records;
        if (jihenId != null && !jihenId.isBlank()) {
            records = playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId(bossId, spawnPointId, jihenId);
        } else {
            records = playRecordRepository.findByBossIdAndSpawnPointIdAndJihenIsNull(bossId, spawnPointId);
        }

        if (records.isEmpty()) {
            return new PatternSearchResponse(0L, List.of());
        }

        // パターンキー（各エントリのday-phase-zoneIdをソートして結合）でグルーピング
        Map<String, List<PlayRecord>> grouped = records.stream()
                .collect(Collectors.groupingBy(this::buildPatternKey));

        List<PatternResponse> patterns = new ArrayList<>();
        int patternIndex = 1;

        for (var entry : grouped.entrySet()) {
            List<PlayRecord> group = entry.getValue();
            long count = group.size();

            // 最新の記録日時を取得
            var lastRecordedAt = group.stream()
                    .map(PlayRecord::getCreatedAt)
                    .max(Comparator.naturalOrder())
                    .orElse(null);

            // 代表レコードからエントリを構築
            PlayRecord representative = group.getFirst();
            List<PatternEntryResponse> entries = representative.getShrinkEntries().stream()
                    .sorted(Comparator.comparingInt((ShrinkEntry e) -> e.getDay())
                            .thenComparingInt(ShrinkEntry::getPhase))
                    .map(se -> new PatternEntryResponse(
                            se.getDay(),
                            se.getPhase(),
                            ShrinkZoneResponse.from(se.getZone())
                    ))
                    .toList();

            patterns.add(new PatternResponse(patternIndex++, count, lastRecordedAt, entries));
        }

        // 記録件数の多い順にソート
        patterns.sort(Comparator.comparingLong(PatternResponse::recordCount).reversed());

        // patternIndexを振り直す
        List<PatternResponse> reindexed = new ArrayList<>();
        for (int i = 0; i < patterns.size(); i++) {
            var p = patterns.get(i);
            reindexed.add(new PatternResponse(i + 1, p.recordCount(), p.lastRecordedAt(), p.entries()));
        }

        long totalRecords = records.size();
        return new PatternSearchResponse(totalRecords, reindexed);
    }

    /** プレイ記録からパターンキーを生成する */
    private String buildPatternKey(PlayRecord record) {
        return record.getShrinkEntries().stream()
                .sorted(Comparator.comparingInt((ShrinkEntry e) -> e.getDay())
                        .thenComparingInt(ShrinkEntry::getPhase))
                .map(e -> e.getDay() + "-" + e.getPhase() + "-" + e.getZone().getId())
                .collect(Collectors.joining("|"));
    }
}
