package com.nightreign.tools.dto;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;

/**
 * 収縮エントリリクエストDTO
 */
public record ShrinkEntryRequest(
        @NotNull(message = "dayは必須です")
        @Min(value = 1, message = "dayは1または2です")
        @Max(value = 2, message = "dayは1または2です")
        Integer day,

        @NotNull(message = "phaseは必須です")
        @Min(value = 1, message = "phaseは1または2です")
        @Max(value = 2, message = "phaseは1または2です")
        Integer phase,

        @NotNull(message = "zoneIdは必須です")
        Integer zoneId
) {
}
