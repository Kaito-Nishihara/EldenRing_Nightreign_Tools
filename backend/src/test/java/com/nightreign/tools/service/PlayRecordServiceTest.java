package com.nightreign.tools.service;

import com.nightreign.tools.dto.PlayRecordRequest;
import com.nightreign.tools.dto.PlayRecordResponse;
import com.nightreign.tools.dto.ShrinkEntryRequest;
import com.nightreign.tools.entity.*;
import com.nightreign.tools.repository.*;
import jakarta.persistence.EntityNotFoundException;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.math.BigDecimal;
import java.util.List;
import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

/**
 * PlayRecordServiceのユニットテスト
 */
@ExtendWith(MockitoExtension.class)
class PlayRecordServiceTest {

    @Mock
    private PlayRecordRepository playRecordRepository;

    @Mock
    private BossRepository bossRepository;

    @Mock
    private JihenRepository jihenRepository;

    @Mock
    private SpawnPointRepository spawnPointRepository;

    @Mock
    private ShrinkZoneRepository shrinkZoneRepository;

    @InjectMocks
    private PlayRecordService playRecordService;

    private Boss testBoss;
    private Jihen testJihen;
    private SpawnPoint testSpawnPoint;
    private ShrinkZone testZone;

    @BeforeEach
    void setUp() {
        testBoss = new Boss();
        testBoss.setId("gladius");
        testBoss.setNameJa("三つ首の獣");

        testJihen = new Jihen();
        testJihen.setId("crater");
        testJihen.setNameJa("火口");

        testSpawnPoint = new SpawnPoint();
        testSpawnPoint.setId(1);
        testSpawnPoint.setNameJa("北東・野営地付近");

        testZone = new ShrinkZone();
        testZone.setId(1);
        testZone.setNameJa("北砦付近");
        testZone.setCenterX(new BigDecimal("0.4500"));
        testZone.setCenterY(new BigDecimal("0.2000"));
        testZone.setRadius(new BigDecimal("0.0800"));
    }

    /** 正常系リクエストを生成する */
    private PlayRecordRequest createValidRequest() {
        return new PlayRecordRequest(
                "gladius", "crater", 1, "テストメモ",
                List.of(
                        new ShrinkEntryRequest(1, 1, 1),
                        new ShrinkEntryRequest(1, 2, 1),
                        new ShrinkEntryRequest(2, 1, 1),
                        new ShrinkEntryRequest(2, 2, 1)
                )
        );
    }

    @Nested
    class 正常系 {

        @Test
        void 全フィールド指定で保存できる() {
            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(jihenRepository.findById("crater")).thenReturn(Optional.of(testJihen));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(testZone));

            PlayRecord saved = new PlayRecord();
            saved.setId(42);
            when(playRecordRepository.save(any(PlayRecord.class))).thenReturn(saved);

            PlayRecordResponse result = playRecordService.createPlayRecord(createValidRequest());

            assertThat(result.id()).isEqualTo(42);
            verify(playRecordRepository).save(any(PlayRecord.class));
        }

        @Test
        void jihenIdがnullでも保存できる() {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(testZone));

            PlayRecord saved = new PlayRecord();
            saved.setId(1);
            when(playRecordRepository.save(any(PlayRecord.class))).thenReturn(saved);

            PlayRecordResponse result = playRecordService.createPlayRecord(request);

            assertThat(result.id()).isEqualTo(1);
            verify(jihenRepository, never()).findById(any());
        }

        @Test
        void jihenIdが空文字でもnullとして扱う() {
            var request = new PlayRecordRequest(
                    "gladius", "", 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(testZone));

            PlayRecord saved = new PlayRecord();
            saved.setId(1);
            when(playRecordRepository.save(any(PlayRecord.class))).thenReturn(saved);

            playRecordService.createPlayRecord(request);

            verify(jihenRepository, never()).findById(any());
        }

        @Test
        void エントリ1件のみでも保存できる() {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(testZone));

            PlayRecord saved = new PlayRecord();
            saved.setId(1);
            when(playRecordRepository.save(any(PlayRecord.class))).thenReturn(saved);

            PlayRecordResponse result = playRecordService.createPlayRecord(request);

            assertThat(result.id()).isEqualTo(1);
        }
    }

    @Nested
    class 異常系_存在チェック {

        @Test
        void 存在しないbossIdで404例外() {
            var request = createValidRequest();
            when(bossRepository.findById("gladius")).thenReturn(Optional.empty());

            assertThatThrownBy(() -> playRecordService.createPlayRecord(request))
                    .isInstanceOf(EntityNotFoundException.class)
                    .hasMessageContaining("bossId")
                    .hasMessageContaining("gladius");
        }

        @Test
        void 存在しないjihenIdで404例外() {
            var request = createValidRequest();
            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(jihenRepository.findById("crater")).thenReturn(Optional.empty());

            assertThatThrownBy(() -> playRecordService.createPlayRecord(request))
                    .isInstanceOf(EntityNotFoundException.class)
                    .hasMessageContaining("jihenId")
                    .hasMessageContaining("crater");
        }

        @Test
        void 存在しないspawnPointIdで404例外() {
            var request = createValidRequest();
            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(jihenRepository.findById("crater")).thenReturn(Optional.of(testJihen));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.empty());

            assertThatThrownBy(() -> playRecordService.createPlayRecord(request))
                    .isInstanceOf(EntityNotFoundException.class)
                    .hasMessageContaining("spawnPointId");
        }

        @Test
        void 存在しないzoneIdで404例外() {
            var request = new PlayRecordRequest(
                    "gladius", "crater", 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 999))
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(jihenRepository.findById("crater")).thenReturn(Optional.of(testJihen));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(999)).thenReturn(Optional.empty());

            assertThatThrownBy(() -> playRecordService.createPlayRecord(request))
                    .isInstanceOf(EntityNotFoundException.class)
                    .hasMessageContaining("zoneId")
                    .hasMessageContaining("999");
        }
    }

    @Nested
    class 異常系_重複チェック {

        @Test
        void 同一day_phaseの重複でIllegalArgumentException() {
            var request = new PlayRecordRequest(
                    "gladius", "crater", 1, null,
                    List.of(
                            new ShrinkEntryRequest(1, 1, 1),
                            new ShrinkEntryRequest(1, 1, 1)  // 重複
                    )
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(jihenRepository.findById("crater")).thenReturn(Optional.of(testJihen));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));

            assertThatThrownBy(() -> playRecordService.createPlayRecord(request))
                    .isInstanceOf(IllegalArgumentException.class)
                    .hasMessageContaining("重複");
        }

        @Test
        void dayが同じでもphaseが違えば重複しない() {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(
                            new ShrinkEntryRequest(1, 1, 1),
                            new ShrinkEntryRequest(1, 2, 1)
                    )
            );

            when(bossRepository.findById("gladius")).thenReturn(Optional.of(testBoss));
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(testSpawnPoint));
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(testZone));

            PlayRecord saved = new PlayRecord();
            saved.setId(1);
            when(playRecordRepository.save(any(PlayRecord.class))).thenReturn(saved);

            PlayRecordResponse result = playRecordService.createPlayRecord(request);

            assertThat(result.id()).isEqualTo(1);
        }
    }
}
