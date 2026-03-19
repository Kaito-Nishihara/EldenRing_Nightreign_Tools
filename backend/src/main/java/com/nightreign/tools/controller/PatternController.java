package com.nightreign.tools.controller;

import com.nightreign.tools.dto.PatternSearchResponse;
import com.nightreign.tools.service.PatternService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

/**
 * パターン検索APIコントローラー
 * ボス・開始地点・地変の条件で過去の収縮パターンを検索する
 */
@RestController
@RequestMapping("/api/v1")
public class PatternController {

    private final PatternService patternService;

    public PatternController(PatternService patternService) {
        this.patternService = patternService;
    }

    /** 過去パターンを検索する */
    @GetMapping("/patterns")
    public PatternSearchResponse getPatterns(
            @RequestParam String bossId,
            @RequestParam Integer spawnPointId,
            @RequestParam(required = false) String jihenId) {
        return patternService.searchPatterns(bossId, spawnPointId, jihenId);
    }
}
