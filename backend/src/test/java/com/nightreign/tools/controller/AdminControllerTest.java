package com.nightreign.tools.controller;

import com.nightreign.tools.config.ServiceKeyFilter;
import com.nightreign.tools.entity.ShrinkZone;
import com.nightreign.tools.entity.SpawnPoint;
import com.nightreign.tools.service.AdminService;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.context.TestConfiguration;
import org.springframework.boot.web.servlet.FilterRegistrationBean;
import org.springframework.context.annotation.Bean;
import org.springframework.http.MediaType;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import jakarta.persistence.EntityNotFoundException;
import java.math.BigDecimal;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.*;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@WebMvcTest(AdminController.class)
class AdminControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private AdminService adminService;

    /** テスト時はServiceKeyFilterを無効化 */
    @TestConfiguration
    static class TestConfig {
        @Bean
        public FilterRegistrationBean<ServiceKeyFilter> serviceKeyFilterRegistration() {
            var registration = new FilterRegistrationBean<ServiceKeyFilter>();
            registration.setFilter(new ServiceKeyFilter("test-key"));
            registration.setEnabled(false);
            return registration;
        }
    }

    // ==========================================
    // SpawnPoint
    // ==========================================

    @Nested
    @DisplayName("SpawnPoint API")
    class SpawnPointApiTests {

        @Test
        @DisplayName("GET /api/v1/admin/spawn-points: 一覧取得")
        void listSpawnPoints() throws Exception {
            var sp = new SpawnPoint();
            sp.setId(1);
            sp.setNameJa("テスト");
            sp.setX(new BigDecimal("100"));
            sp.setY(new BigDecimal("200"));
            sp.setNearbyBaseType("church");
            when(adminService.getAllSpawnPoints()).thenReturn(List.of(sp));

            mockMvc.perform(get("/api/v1/admin/spawn-points"))
                    .andExpect(status().isOk())
                    .andExpect(jsonPath("$[0].nameJa").value("テスト"))
                    .andExpect(jsonPath("$[0].nearbyBaseType").value("church"));
        }

        @Test
        @DisplayName("POST /api/v1/admin/spawn-points: 登録で201")
        void createSpawnPoint() throws Exception {
            var saved = new SpawnPoint();
            saved.setId(1);
            saved.setNameJa("新地点");
            saved.setX(new BigDecimal("100"));
            saved.setY(new BigDecimal("200"));
            saved.setNearbyBaseType("church");
            when(adminService.createSpawnPoint(any())).thenReturn(saved);

            mockMvc.perform(post("/api/v1/admin/spawn-points")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"新地点","x":100,"y":200,"nearbyBaseType":"church"}
                                    """))
                    .andExpect(status().isCreated())
                    .andExpect(jsonPath("$.nameJa").value("新地点"));
        }

        @Test
        @DisplayName("POST: nameJa空でバリデーションエラー400")
        void createSpawnPointValidationError() throws Exception {
            mockMvc.perform(post("/api/v1/admin/spawn-points")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"","x":100,"y":200,"nearbyBaseType":"church"}
                                    """))
                    .andExpect(status().isBadRequest());
        }

        @Test
        @DisplayName("POST: 不正なnearbyBaseTypeでバリデーションエラー400")
        void createSpawnPointInvalidBaseType() throws Exception {
            mockMvc.perform(post("/api/v1/admin/spawn-points")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"テスト","x":100,"y":200,"nearbyBaseType":"invalid"}
                                    """))
                    .andExpect(status().isBadRequest());
        }

        @Test
        @DisplayName("PUT /api/v1/admin/spawn-points/{id}: 更新で200")
        void updateSpawnPoint() throws Exception {
            var updated = new SpawnPoint();
            updated.setId(1);
            updated.setNameJa("更新後");
            updated.setX(new BigDecimal("300"));
            updated.setY(new BigDecimal("400"));
            updated.setNearbyBaseType("camp");
            when(adminService.updateSpawnPoint(eq(1), any())).thenReturn(updated);

            mockMvc.perform(put("/api/v1/admin/spawn-points/1")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"更新後","x":300,"y":400,"nearbyBaseType":"camp"}
                                    """))
                    .andExpect(status().isOk())
                    .andExpect(jsonPath("$.nameJa").value("更新後"));
        }

        @Test
        @DisplayName("DELETE /api/v1/admin/spawn-points/{id}: 削除で204")
        void deleteSpawnPoint() throws Exception {
            doNothing().when(adminService).deleteSpawnPoint(1);

            mockMvc.perform(delete("/api/v1/admin/spawn-points/1"))
                    .andExpect(status().isNoContent());
        }

        @Test
        @DisplayName("DELETE: 存在しないIDで404")
        void deleteSpawnPointNotFound() throws Exception {
            doThrow(new EntityNotFoundException("見つかりません"))
                    .when(adminService).deleteSpawnPoint(999);

            mockMvc.perform(delete("/api/v1/admin/spawn-points/999"))
                    .andExpect(status().isNotFound());
        }
    }

    // ==========================================
    // ShrinkZone
    // ==========================================

    @Nested
    @DisplayName("ShrinkZone API")
    class ShrinkZoneApiTests {

        @Test
        @DisplayName("GET /api/v1/admin/shrink-zones: 一覧取得")
        void listShrinkZones() throws Exception {
            var sz = new ShrinkZone();
            sz.setId(1);
            sz.setNameJa("テストゾーン");
            sz.setCenterX(new BigDecimal("500"));
            sz.setCenterY(new BigDecimal("300"));
            sz.setRadius(new BigDecimal("80"));
            when(adminService.getAllShrinkZones()).thenReturn(List.of(sz));

            mockMvc.perform(get("/api/v1/admin/shrink-zones"))
                    .andExpect(status().isOk())
                    .andExpect(jsonPath("$[0].nameJa").value("テストゾーン"))
                    .andExpect(jsonPath("$[0].radius").value(80));
        }

        @Test
        @DisplayName("POST /api/v1/admin/shrink-zones: 登録で201")
        void createShrinkZone() throws Exception {
            var saved = new ShrinkZone();
            saved.setId(1);
            saved.setNameJa("北の砦");
            saved.setCenterX(new BigDecimal("500"));
            saved.setCenterY(new BigDecimal("300"));
            saved.setRadius(new BigDecimal("80"));
            when(adminService.createShrinkZone(any())).thenReturn(saved);

            mockMvc.perform(post("/api/v1/admin/shrink-zones")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"北の砦","centerX":500,"centerY":300,"radius":80}
                                    """))
                    .andExpect(status().isCreated())
                    .andExpect(jsonPath("$.nameJa").value("北の砦"));
        }

        @Test
        @DisplayName("POST: 半径0以下でバリデーションエラー400")
        void createShrinkZoneInvalidRadius() throws Exception {
            mockMvc.perform(post("/api/v1/admin/shrink-zones")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"テスト","centerX":500,"centerY":300,"radius":0}
                                    """))
                    .andExpect(status().isBadRequest());
        }

        @Test
        @DisplayName("POST: nameJa空でバリデーションエラー400")
        void createShrinkZoneEmptyName() throws Exception {
            mockMvc.perform(post("/api/v1/admin/shrink-zones")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"","centerX":500,"centerY":300,"radius":80}
                                    """))
                    .andExpect(status().isBadRequest());
        }

        @Test
        @DisplayName("PUT /api/v1/admin/shrink-zones/{id}: 更新で200")
        void updateShrinkZone() throws Exception {
            var updated = new ShrinkZone();
            updated.setId(1);
            updated.setNameJa("更新後");
            updated.setCenterX(new BigDecimal("600"));
            updated.setCenterY(new BigDecimal("400"));
            updated.setRadius(new BigDecimal("100"));
            when(adminService.updateShrinkZone(eq(1), any())).thenReturn(updated);

            mockMvc.perform(put("/api/v1/admin/shrink-zones/1")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content("""
                                    {"nameJa":"更新後","centerX":600,"centerY":400,"radius":100}
                                    """))
                    .andExpect(status().isOk())
                    .andExpect(jsonPath("$.nameJa").value("更新後"));
        }

        @Test
        @DisplayName("DELETE /api/v1/admin/shrink-zones/{id}: 削除で204")
        void deleteShrinkZone() throws Exception {
            doNothing().when(adminService).deleteShrinkZone(1);

            mockMvc.perform(delete("/api/v1/admin/shrink-zones/1"))
                    .andExpect(status().isNoContent());
        }

        @Test
        @DisplayName("DELETE: 存在しないIDで404")
        void deleteShrinkZoneNotFound() throws Exception {
            doThrow(new EntityNotFoundException("見つかりません"))
                    .when(adminService).deleteShrinkZone(999);

            mockMvc.perform(delete("/api/v1/admin/shrink-zones/999"))
                    .andExpect(status().isNotFound());
        }
    }
}
