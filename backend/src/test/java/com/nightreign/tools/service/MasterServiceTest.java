package com.nightreign.tools.service;

import com.nightreign.tools.dto.BossResponse;
import com.nightreign.tools.dto.JihenResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.dto.SpawnPointResponse;
import com.nightreign.tools.entity.*;
import com.nightreign.tools.repository.BasePointRepository;
import com.nightreign.tools.repository.BossRepository;
import com.nightreign.tools.repository.JihenRepository;
import com.nightreign.tools.repository.ShrinkZoneRepository;
import com.nightreign.tools.repository.SpawnPointRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.math.BigDecimal;
import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.when;

/**
 * MasterServiceのユニットテスト
 */
@ExtendWith(MockitoExtension.class)
class MasterServiceTest {

    @Mock
    private BasePointRepository basePointRepository;

    @Mock
    private BossRepository bossRepository;

    @Mock
    private JihenRepository jihenRepository;

    @Mock
    private ShrinkZoneRepository shrinkZoneRepository;

    @Mock
    private SpawnPointRepository spawnPointRepository;

    @InjectMocks
    private MasterService masterService;

    private Boss testBoss;
    private Jihen testJihen;
    private ShrinkZone testZone;
    private SpawnPoint testSpawnPoint;

    @BeforeEach
    void setUp() {
        // テスト用ボス
        testBoss = new Boss();
        testBoss.setId("gladius");
        testBoss.setNameJa("三つ首の獣（グラディウス）");
        testBoss.setNameEn("Gladius, Beast Regent");
        testBoss.setImagePath("/images/bosses/gladius.webp");
        testBoss.setDlc(false);
        testBoss.setUnlockCondition("最初から選択可能");

        BossWeakness weakness = new BossWeakness();
        weakness.setBoss(testBoss);
        weakness.setAttribute("holy");
        testBoss.setWeaknesses(List.of(weakness));

        // テスト用地変
        testJihen = new Jihen();
        testJihen.setId("crater");
        testJihen.setNameJa("火口");
        testJihen.setArea("north");
        testJihen.setCenterX(new BigDecimal("0.5000"));
        testJihen.setCenterY(new BigDecimal("0.2000"));
        testJihen.setDlc(false);

        // テスト用収縮候補地
        testZone = new ShrinkZone();
        testZone.setId(1);
        testZone.setNameJa("北砦付近");
        testZone.setCenterX(new BigDecimal("0.4500"));
        testZone.setCenterY(new BigDecimal("0.2000"));
        testZone.setRadius(new BigDecimal("0.0800"));

        // テスト用開始地点
        testSpawnPoint = new SpawnPoint();
        testSpawnPoint.setId(1);
        testSpawnPoint.setNameJa("北東・野営地付近");
        testSpawnPoint.setX(new BigDecimal("0.7500"));
        testSpawnPoint.setY(new BigDecimal("0.2500"));
    }

    @Test
    void getAllBosses_ボス一覧を返す() {
        when(bossRepository.findAllBy()).thenReturn(List.of(testBoss));

        List<BossResponse> result = masterService.getAllBosses();

        assertThat(result).hasSize(1);
        assertThat(result.getFirst().id()).isEqualTo("gladius");
        assertThat(result.getFirst().nameJa()).isEqualTo("三つ首の獣（グラディウス）");
        assertThat(result.getFirst().weaknesses()).containsExactly("holy");
        assertThat(result.getFirst().isDlc()).isFalse();
    }

    @Test
    void getAllBosses_空の場合は空リストを返す() {
        when(bossRepository.findAllBy()).thenReturn(List.of());

        List<BossResponse> result = masterService.getAllBosses();

        assertThat(result).isEmpty();
    }

    @Test
    void getAllJihen_地変一覧を返す() {
        when(jihenRepository.findAll()).thenReturn(List.of(testJihen));

        List<JihenResponse> result = masterService.getAllJihen();

        assertThat(result).hasSize(1);
        assertThat(result.getFirst().id()).isEqualTo("crater");
        assertThat(result.getFirst().nameJa()).isEqualTo("火口");
        assertThat(result.getFirst().centerX()).isEqualByComparingTo("0.5000");
    }

    @Test
    void getAllShrinkZones_収縮候補地一覧を返す() {
        when(shrinkZoneRepository.findAll()).thenReturn(List.of(testZone));

        List<ShrinkZoneResponse> result = masterService.getAllShrinkZones();

        assertThat(result).hasSize(1);
        assertThat(result.getFirst().id()).isEqualTo(1);
        assertThat(result.getFirst().nameJa()).isEqualTo("北砦付近");
        assertThat(result.getFirst().radius()).isEqualByComparingTo("0.0800");
    }

    @Test
    void getAllSpawnPoints_開始地点一覧を返す() {
        when(spawnPointRepository.findAll()).thenReturn(List.of(testSpawnPoint));

        List<SpawnPointResponse> result = masterService.getAllSpawnPoints();

        assertThat(result).hasSize(1);
        assertThat(result.getFirst().id()).isEqualTo(1);
        assertThat(result.getFirst().nameJa()).isEqualTo("北東・野営地付近");
        assertThat(result.getFirst().nameJa()).isEqualTo("北東・野営地付近");
    }
}
