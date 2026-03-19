package com.nightreign.tools.controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.nightreign.tools.dto.PlayRecordRequest;
import com.nightreign.tools.dto.PlayRecordResponse;
import com.nightreign.tools.dto.ShrinkEntryRequest;
import com.nightreign.tools.service.PlayRecordService;
import jakarta.persistence.EntityNotFoundException;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.http.MediaType;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDateTime;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

/**
 * PlayRecordControllerの統合テスト
 */
@WebMvcTest(PlayRecordController.class)
class PlayRecordControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private ObjectMapper objectMapper;

    @MockitoBean
    private PlayRecordService playRecordService;

    @Nested
    class 正常系 {

        @Test
        void 正常リクエストで201を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", "crater", 1, "テストメモ",
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );
            var response = new PlayRecordResponse(42, LocalDateTime.of(2026, 3, 16, 10, 0));
            when(playRecordService.createPlayRecord(any())).thenReturn(response);

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isCreated())
                    .andExpect(jsonPath("$.id").value(42));
        }
    }

    @Nested
    class 異常系_バリデーション {

        @Test
        void bossIdが空で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest())
                    .andExpect(jsonPath("$.message").exists());
        }

        @Test
        void bossIdがnullで400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    null, null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void spawnPointIdがnullで400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, null, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void entriesが空配列で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of()
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void entriesがnullで400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null, null
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void dayが0で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(0, 1, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void dayが3で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(3, 1, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void phaseが0で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 0, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void phaseが3で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 3, 1))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }

        @Test
        void zoneIdがnullで400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, null))
            );

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest());
        }
    }

    @Nested
    class 異常系_ビジネスロジック {

        @Test
        void 存在しないbossIdで404を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "unknown", null, 1, null,
                    List.of(new ShrinkEntryRequest(1, 1, 1))
            );
            when(playRecordService.createPlayRecord(any()))
                    .thenThrow(new EntityNotFoundException("bossId 'unknown' は存在しません"));

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isNotFound())
                    .andExpect(jsonPath("$.status").value(404))
                    .andExpect(jsonPath("$.message").value("bossId 'unknown' は存在しません"));
        }

        @Test
        void day_phase重複で400を返す() throws Exception {
            var request = new PlayRecordRequest(
                    "gladius", null, 1, null,
                    List.of(
                            new ShrinkEntryRequest(1, 1, 1),
                            new ShrinkEntryRequest(1, 1, 2)
                    )
            );
            when(playRecordService.createPlayRecord(any()))
                    .thenThrow(new IllegalArgumentException("同一レコード内で day=1, phase=1 が重複しています"));

            mockMvc.perform(post("/api/v1/play-records")
                            .contentType(MediaType.APPLICATION_JSON)
                            .content(objectMapper.writeValueAsString(request)))
                    .andExpect(status().isBadRequest())
                    .andExpect(jsonPath("$.status").value(400))
                    .andExpect(jsonPath("$.message").exists());
        }
    }
}
