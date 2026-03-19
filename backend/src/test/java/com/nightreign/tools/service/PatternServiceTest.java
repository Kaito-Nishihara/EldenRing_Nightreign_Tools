package com.nightreign.tools.service;

import com.nightreign.tools.dto.PatternSearchResponse;
import com.nightreign.tools.entity.*;
import com.nightreign.tools.repository.PlayRecordRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.when;

/**
 * PatternServiceのユニットテスト
 */
@ExtendWith(MockitoExtension.class)
class PatternServiceTest {

    @Mock
    private PlayRecordRepository playRecordRepository;

    @InjectMocks
    private PatternService patternService;

    private ShrinkZone zone1;
    private ShrinkZone zone2;

    @BeforeEach
    void setUp() {
        zone1 = new ShrinkZone();
        zone1.setId(1);
        zone1.setNameJa("北砦付近");
        zone1.setCenterX(new BigDecimal("0.4500"));
        zone1.setCenterY(new BigDecimal("0.2000"));
        zone1.setRadius(new BigDecimal("0.0800"));

        zone2 = new ShrinkZone();
        zone2.setId(7);
        zone2.setNameJa("南の教会跡");
        zone2.setCenterX(new BigDecimal("0.5500"));
        zone2.setCenterY(new BigDecimal("0.7500"));
        zone2.setRadius(new BigDecimal("0.0800"));
    }

    /** テスト用PlayRecordを生成するヘルパー */
    private PlayRecord createRecord(ShrinkZone day1Zone, ShrinkZone day2Zone, LocalDateTime createdAt) {
        PlayRecord record = new PlayRecord();
        record.setCreatedAt(createdAt);

        ShrinkEntry e1 = new ShrinkEntry();
        e1.setPlayRecord(record);
        e1.setDay(1);
        e1.setPhase(1);
        e1.setZone(day1Zone);

        ShrinkEntry e2 = new ShrinkEntry();
        e2.setPlayRecord(record);
        e2.setDay(2);
        e2.setPhase(1);
        e2.setZone(day2Zone);

        record.getShrinkEntries().addAll(List.of(e1, e2));
        return record;
    }

    @Test
    void searchPatterns_条件一致するレコードがない場合は空を返す() {
        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of());

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        assertThat(result.totalRecords()).isZero();
        assertThat(result.patterns()).isEmpty();
    }

    @Test
    void searchPatterns_同一パターンがグルーピングされる() {
        var now = LocalDateTime.now();
        var record1 = createRecord(zone1, zone2, now.minusDays(1));
        var record2 = createRecord(zone1, zone2, now);  // 同じパターン

        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of(record1, record2));

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        assertThat(result.totalRecords()).isEqualTo(2);
        assertThat(result.patterns()).hasSize(1);
        assertThat(result.patterns().getFirst().recordCount()).isEqualTo(2);
    }

    @Test
    void searchPatterns_異なるパターンは別々にグルーピングされる() {
        var now = LocalDateTime.now();
        var record1 = createRecord(zone1, zone2, now);
        var record2 = createRecord(zone2, zone1, now);  // 逆パターン

        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of(record1, record2));

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        assertThat(result.totalRecords()).isEqualTo(2);
        assertThat(result.patterns()).hasSize(2);
    }

    @Test
    void searchPatterns_記録件数の多い順にソートされる() {
        var now = LocalDateTime.now();
        // パターンA: 1件
        var recordA = createRecord(zone2, zone1, now);
        // パターンB: 2件
        var recordB1 = createRecord(zone1, zone2, now.minusDays(1));
        var recordB2 = createRecord(zone1, zone2, now);

        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of(recordA, recordB1, recordB2));

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        assertThat(result.patterns().getFirst().recordCount()).isEqualTo(2);
        assertThat(result.patterns().get(1).recordCount()).isEqualTo(1);
    }

    @Test
    void searchPatterns_jihenがnullの場合はjihenIsNullで検索する() {
        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenIsNull("gladius", 1))
                .thenReturn(List.of());

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, null);

        assertThat(result.totalRecords()).isZero();
    }

    @Test
    void searchPatterns_patternIndexが1から連番で振られる() {
        var now = LocalDateTime.now();
        var record1 = createRecord(zone1, zone2, now);
        var record2 = createRecord(zone2, zone1, now);

        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of(record1, record2));

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        assertThat(result.patterns().get(0).patternIndex()).isEqualTo(1);
        assertThat(result.patterns().get(1).patternIndex()).isEqualTo(2);
    }

    @Test
    void searchPatterns_エントリがday_phase順にソートされる() {
        var now = LocalDateTime.now();
        var record = createRecord(zone1, zone2, now);

        when(playRecordRepository.findByBossIdAndSpawnPointIdAndJihenId("gladius", 1, "crater"))
                .thenReturn(List.of(record));

        PatternSearchResponse result = patternService.searchPatterns("gladius", 1, "crater");

        var entries = result.patterns().getFirst().entries();
        assertThat(entries.get(0).day()).isEqualTo(1);
        assertThat(entries.get(1).day()).isEqualTo(2);
    }
}
