package com.nightreign.tools.controller;

import com.nightreign.tools.dto.PatternEntryResponse;
import com.nightreign.tools.dto.PatternResponse;
import com.nightreign.tools.dto.PatternSearchResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.service.PatternService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

/**
 * PatternControllerの統合テスト
 */
@WebMvcTest(PatternController.class)
class PatternControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private PatternService patternService;

    @Test
    void getPatterns_200でパターン検索結果を返す() throws Exception {
        var zone = new ShrinkZoneResponse(1, "北砦付近", null,
                new BigDecimal("0.4500"), new BigDecimal("0.2000"), new BigDecimal("0.0800"));
        var entry = new PatternEntryResponse(1, 1, zone);
        var pattern = new PatternResponse(1, 3L, LocalDateTime.of(2026, 3, 15, 20, 30), List.of(entry));
        var response = new PatternSearchResponse(3L, List.of(pattern));

        when(patternService.searchPatterns("gladius", 1, "crater")).thenReturn(response);

        mockMvc.perform(get("/api/v1/patterns")
                        .param("bossId", "gladius")
                        .param("spawnPointId", "1")
                        .param("jihenId", "crater"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.totalRecords").value(3))
                .andExpect(jsonPath("$.patterns[0].patternIndex").value(1))
                .andExpect(jsonPath("$.patterns[0].recordCount").value(3))
                .andExpect(jsonPath("$.patterns[0].entries[0].day").value(1))
                .andExpect(jsonPath("$.patterns[0].entries[0].zone.nameJa").value("北砦付近"));
    }

    @Test
    void getPatterns_jihenId省略でも検索できる() throws Exception {
        var response = new PatternSearchResponse(0L, List.of());
        when(patternService.searchPatterns("gladius", 1, null)).thenReturn(response);

        mockMvc.perform(get("/api/v1/patterns")
                        .param("bossId", "gladius")
                        .param("spawnPointId", "1"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.totalRecords").value(0))
                .andExpect(jsonPath("$.patterns").isEmpty());
    }

    @Test
    void getPatterns_bossId未指定で400を返す() throws Exception {
        mockMvc.perform(get("/api/v1/patterns")
                        .param("spawnPointId", "1"))
                .andExpect(status().isBadRequest());
    }

    @Test
    void getPatterns_spawnPointId未指定で400を返す() throws Exception {
        mockMvc.perform(get("/api/v1/patterns")
                        .param("bossId", "gladius"))
                .andExpect(status().isBadRequest());
    }
}
