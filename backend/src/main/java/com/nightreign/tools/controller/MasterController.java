package com.nightreign.tools.controller;

import com.nightreign.tools.dto.BossResponse;
import com.nightreign.tools.dto.JihenResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.dto.SpawnPointResponse;
import com.nightreign.tools.service.MasterService;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

/**
 * マスタデータAPIコントローラー
 * ボス・地変・収縮候補地・開始地点の一覧取得エンドポイント
 */
@RestController
@RequestMapping("/api/v1")
public class MasterController {

    private final MasterService masterService;

    public MasterController(MasterService masterService) {
        this.masterService = masterService;
    }

    /** ボス一覧を取得する */
    @GetMapping("/bosses")
    public List<BossResponse> getBosses() {
        return masterService.getAllBosses();
    }

    /** 地変一覧を取得する */
    @GetMapping("/jihen")
    public List<JihenResponse> getJihen() {
        return masterService.getAllJihen();
    }

    /** 収縮候補地一覧を取得する */
    @GetMapping("/shrink-zones")
    public List<ShrinkZoneResponse> getShrinkZones() {
        return masterService.getAllShrinkZones();
    }

    /** 開始地点一覧を取得する */
    @GetMapping("/spawn-points")
    public List<SpawnPointResponse> getSpawnPoints() {
        return masterService.getAllSpawnPoints();
    }
}
