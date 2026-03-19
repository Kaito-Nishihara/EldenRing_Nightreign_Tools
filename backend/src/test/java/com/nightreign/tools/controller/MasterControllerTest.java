package com.nightreign.tools.controller;

import com.nightreign.tools.dto.BossResponse;
import com.nightreign.tools.dto.JihenResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.dto.SpawnPointResponse;
import com.nightreign.tools.service.MasterService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.math.BigDecimal;
import java.util.List;

import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

/**
 * MasterControllerの統合テスト
 */
@WebMvcTest(MasterController.class)
class MasterControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private MasterService masterService;

    @Test
    void getBosses_200でボス一覧を返す() throws Exception {
        var boss = new BossResponse("gladius", "三つ首の獣", "Gladius",
                "/images/bosses/gladius.webp", List.of("holy"), false, "最初から選択可能");
        when(masterService.getAllBosses()).thenReturn(List.of(boss));

        mockMvc.perform(get("/api/v1/bosses"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].id").value("gladius"))
                .andExpect(jsonPath("$[0].nameJa").value("三つ首の獣"))
                .andExpect(jsonPath("$[0].weaknesses[0]").value("holy"))
                .andExpect(jsonPath("$[0].isDlc").value(false));
    }

    @Test
    void getBosses_空の場合は空配列を返す() throws Exception {
        when(masterService.getAllBosses()).thenReturn(List.of());

        mockMvc.perform(get("/api/v1/bosses"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$").isArray())
                .andExpect(jsonPath("$").isEmpty());
    }

    @Test
    void getJihen_200で地変一覧を返す() throws Exception {
        var jihen = new JihenResponse("crater", "火口", "/images/jihen/crater.webp",
                "north", new BigDecimal("0.5000"), new BigDecimal("0.2000"), false, null);
        when(masterService.getAllJihen()).thenReturn(List.of(jihen));

        mockMvc.perform(get("/api/v1/jihen"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].id").value("crater"))
                .andExpect(jsonPath("$[0].nameJa").value("火口"))
                .andExpect(jsonPath("$[0].area").value("north"));
    }

    @Test
    void getShrinkZones_200で収縮候補地一覧を返す() throws Exception {
        var zone = new ShrinkZoneResponse(1, "北砦付近", null,
                new BigDecimal("0.4500"), new BigDecimal("0.2000"), new BigDecimal("0.0800"));
        when(masterService.getAllShrinkZones()).thenReturn(List.of(zone));

        mockMvc.perform(get("/api/v1/shrink-zones"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].id").value(1))
                .andExpect(jsonPath("$[0].nameJa").value("北砦付近"))
                .andExpect(jsonPath("$[0].radius").value(0.0800));
    }

    @Test
    void getSpawnPoints_200で開始地点一覧を返す() throws Exception {
        var sp = new SpawnPointResponse(1, "北東・野営地付近",
                new BigDecimal("0.7500"), new BigDecimal("0.2500"), "campfire");
        when(masterService.getAllSpawnPoints()).thenReturn(List.of(sp));

        mockMvc.perform(get("/api/v1/spawn-points"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].id").value(1))
                .andExpect(jsonPath("$[0].nameJa").value("北東・野営地付近"))
                .andExpect(jsonPath("$[0].nearbyBaseType").value("campfire"));
    }
}
