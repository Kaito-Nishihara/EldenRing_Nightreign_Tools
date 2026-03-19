package com.nightreign.tools.service;

import com.nightreign.tools.dto.ShrinkZoneRequest;
import com.nightreign.tools.dto.SpawnPointRequest;
import com.nightreign.tools.entity.ShrinkZone;
import com.nightreign.tools.entity.SpawnPoint;
import com.nightreign.tools.repository.ShrinkZoneRepository;
import com.nightreign.tools.repository.SpawnPointRepository;
import jakarta.persistence.EntityNotFoundException;
import org.junit.jupiter.api.DisplayName;
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

@ExtendWith(MockitoExtension.class)
class AdminServiceTest {

    @Mock
    private SpawnPointRepository spawnPointRepository;

    @Mock
    private ShrinkZoneRepository shrinkZoneRepository;

    @InjectMocks
    private AdminService adminService;

    // ==========================================
    // SpawnPoint
    // ==========================================

    @Nested
    @DisplayName("SpawnPoint CRUD")
    class SpawnPointTests {

        @Test
        @DisplayName("一覧取得: 全件返す")
        void getAllSpawnPoints() {
            var sp = new SpawnPoint();
            sp.setNameJa("テスト地点");
            when(spawnPointRepository.findAll()).thenReturn(List.of(sp));

            var result = adminService.getAllSpawnPoints();

            assertThat(result).hasSize(1);
            assertThat(result.get(0).getNameJa()).isEqualTo("テスト地点");
        }

        @Test
        @DisplayName("登録: 正常にエンティティが保存される")
        void createSpawnPoint() {
            var request = new SpawnPointRequest(
                    "北の高台", new BigDecimal("100"), new BigDecimal("200"), "campfire");

            var saved = new SpawnPoint();
            saved.setId(1);
            saved.setNameJa("北の高台");
            saved.setX(new BigDecimal("100"));
            saved.setY(new BigDecimal("200"));
            saved.setNearbyBaseType("campfire");
            when(spawnPointRepository.save(any())).thenReturn(saved);

            var result = adminService.createSpawnPoint(request);

            assertThat(result.getId()).isEqualTo(1);
            assertThat(result.getNameJa()).isEqualTo("北の高台");
            verify(spawnPointRepository).save(any());
        }

        @Test
        @DisplayName("更新: 既存データが更新される")
        void updateSpawnPoint() {
            var existing = new SpawnPoint();
            existing.setId(1);
            existing.setNameJa("旧名");
            when(spawnPointRepository.findById(1)).thenReturn(Optional.of(existing));
            when(spawnPointRepository.save(any())).thenAnswer(i -> i.getArgument(0));

            var request = new SpawnPointRequest(
                    "新名", new BigDecimal("300"), new BigDecimal("400"), "ruins");

            var result = adminService.updateSpawnPoint(1, request);

            assertThat(result.getNameJa()).isEqualTo("新名");
            assertThat(result.getNearbyBaseType()).isEqualTo("ruins");
        }

        @Test
        @DisplayName("更新: 存在しないIDで404")
        void updateSpawnPointNotFound() {
            when(spawnPointRepository.findById(999)).thenReturn(Optional.empty());

            var request = new SpawnPointRequest(
                    "名前", BigDecimal.ZERO, BigDecimal.ZERO, "campfire");

            assertThatThrownBy(() -> adminService.updateSpawnPoint(999, request))
                    .isInstanceOf(EntityNotFoundException.class);
        }

        @Test
        @DisplayName("削除: 正常に削除される")
        void deleteSpawnPoint() {
            when(spawnPointRepository.existsById(1)).thenReturn(true);

            adminService.deleteSpawnPoint(1);

            verify(spawnPointRepository).deleteById(1);
        }

        @Test
        @DisplayName("削除: 存在しないIDで404")
        void deleteSpawnPointNotFound() {
            when(spawnPointRepository.existsById(999)).thenReturn(false);

            assertThatThrownBy(() -> adminService.deleteSpawnPoint(999))
                    .isInstanceOf(EntityNotFoundException.class);
        }
    }

    // ==========================================
    // ShrinkZone
    // ==========================================

    @Nested
    @DisplayName("ShrinkZone CRUD")
    class ShrinkZoneTests {

        @Test
        @DisplayName("一覧取得: 全件返す")
        void getAllShrinkZones() {
            var sz = new ShrinkZone();
            sz.setNameJa("テストゾーン");
            when(shrinkZoneRepository.findAll()).thenReturn(List.of(sz));

            var result = adminService.getAllShrinkZones();

            assertThat(result).hasSize(1);
            assertThat(result.get(0).getNameJa()).isEqualTo("テストゾーン");
        }

        @Test
        @DisplayName("登録: 正常にエンティティが保存される")
        void createShrinkZone() {
            var request = new ShrinkZoneRequest(
                    "北の砦", "/icons/fort.webp",
                    new BigDecimal("500"), new BigDecimal("300"), new BigDecimal("80"));

            var saved = new ShrinkZone();
            saved.setId(1);
            saved.setNameJa("北の砦");
            saved.setIconPath("/icons/fort.webp");
            saved.setCenterX(new BigDecimal("500"));
            saved.setCenterY(new BigDecimal("300"));
            saved.setRadius(new BigDecimal("80"));
            when(shrinkZoneRepository.save(any())).thenReturn(saved);

            var result = adminService.createShrinkZone(request);

            assertThat(result.getId()).isEqualTo(1);
            assertThat(result.getNameJa()).isEqualTo("北の砦");
            assertThat(result.getIconPath()).isEqualTo("/icons/fort.webp");
        }

        @Test
        @DisplayName("更新: 既存データが更新される")
        void updateShrinkZone() {
            var existing = new ShrinkZone();
            existing.setId(1);
            existing.setNameJa("旧名");
            when(shrinkZoneRepository.findById(1)).thenReturn(Optional.of(existing));
            when(shrinkZoneRepository.save(any())).thenAnswer(i -> i.getArgument(0));

            var request = new ShrinkZoneRequest(
                    "新名", null,
                    new BigDecimal("600"), new BigDecimal("400"), new BigDecimal("100"));

            var result = adminService.updateShrinkZone(1, request);

            assertThat(result.getNameJa()).isEqualTo("新名");
            assertThat(result.getRadius()).isEqualTo(new BigDecimal("100"));
        }

        @Test
        @DisplayName("更新: 存在しないIDで404")
        void updateShrinkZoneNotFound() {
            when(shrinkZoneRepository.findById(999)).thenReturn(Optional.empty());

            var request = new ShrinkZoneRequest(
                    "名前", null, BigDecimal.ZERO, BigDecimal.ZERO, BigDecimal.ONE);

            assertThatThrownBy(() -> adminService.updateShrinkZone(999, request))
                    .isInstanceOf(EntityNotFoundException.class);
        }

        @Test
        @DisplayName("削除: 正常に削除される")
        void deleteShrinkZone() {
            when(shrinkZoneRepository.existsById(1)).thenReturn(true);

            adminService.deleteShrinkZone(1);

            verify(shrinkZoneRepository).deleteById(1);
        }

        @Test
        @DisplayName("削除: 存在しないIDで404")
        void deleteShrinkZoneNotFound() {
            when(shrinkZoneRepository.existsById(999)).thenReturn(false);

            assertThatThrownBy(() -> adminService.deleteShrinkZone(999))
                    .isInstanceOf(EntityNotFoundException.class);
        }
    }
}
