package com.nightreign.tools.controller;

import com.nightreign.tools.dto.PlayRecordRequest;
import com.nightreign.tools.dto.PlayRecordResponse;
import com.nightreign.tools.service.PlayRecordService;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

/**
 * プレイ記録APIコントローラー
 * プレイ記録の保存エンドポイント
 */
@RestController
@RequestMapping("/api/v1")
public class PlayRecordController {

    private final PlayRecordService playRecordService;

    public PlayRecordController(PlayRecordService playRecordService) {
        this.playRecordService = playRecordService;
    }

    /** プレイ記録を保存する */
    @PostMapping("/play-records")
    @ResponseStatus(HttpStatus.CREATED)
    public PlayRecordResponse createPlayRecord(@Valid @RequestBody PlayRecordRequest request) {
        return playRecordService.createPlayRecord(request);
    }
}
