package com.nightreign.tools.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import java.util.List;

/**
 * プレイ記録リクエストDTO
 */
public record PlayRecordRequest(
        @NotBlank(message = "bossIdは必須です")
        String bossId,

        String jihenId,

        @NotNull(message = "spawnPointIdは必須です")
        Integer spawnPointId,

        String note,

        @NotEmpty(message = "entriesは1件以上必要です")
        @Valid
        List<ShrinkEntryRequest> entries
) {
}
