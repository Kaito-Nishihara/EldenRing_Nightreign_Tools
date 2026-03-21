package com.nightreign.tools.controller;

import com.nightreign.tools.dto.*;
import com.nightreign.tools.service.AdminService;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * 管理画面用コントローラー
 * マスタデータ（開始地点・収縮候補地）のCRUD操作を提供する
 */
@RestController
@RequestMapping("/api/v1/admin")
public class AdminController {

    private final AdminService adminService;

    public AdminController(AdminService adminService) {
        this.adminService = adminService;
    }

    // ==========================================
    // SpawnPoint CRUD
    // ==========================================

    /** 開始地点一覧を取得する */
    @GetMapping("/spawn-points")
    public List<SpawnPointResponse> getSpawnPoints() {
        return adminService.getAllSpawnPoints().stream()
                .map(SpawnPointResponse::from)
                .toList();
    }

    /** 開始地点を登録する */
    @PostMapping("/spawn-points")
    @ResponseStatus(HttpStatus.CREATED)
    public SpawnPointResponse createSpawnPoint(@Valid @RequestBody SpawnPointRequest request) {
        return SpawnPointResponse.from(adminService.createSpawnPoint(request));
    }

    /** 開始地点を更新する */
    @PutMapping("/spawn-points/{id}")
    public SpawnPointResponse updateSpawnPoint(
            @PathVariable Integer id,
            @Valid @RequestBody SpawnPointRequest request) {
        return SpawnPointResponse.from(adminService.updateSpawnPoint(id, request));
    }

    /** 開始地点を削除する */
    @DeleteMapping("/spawn-points/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deleteSpawnPoint(@PathVariable Integer id) {
        adminService.deleteSpawnPoint(id);
    }

    // ==========================================
    // ShrinkZone CRUD
    // ==========================================

    /** 収縮候補地一覧を取得する */
    @GetMapping("/shrink-zones")
    public List<ShrinkZoneResponse> getShrinkZones() {
        return adminService.getAllShrinkZones().stream()
                .map(ShrinkZoneResponse::from)
                .toList();
    }

    /** 収縮候補地を登録する */
    @PostMapping("/shrink-zones")
    @ResponseStatus(HttpStatus.CREATED)
    public ShrinkZoneResponse createShrinkZone(@Valid @RequestBody ShrinkZoneRequest request) {
        return ShrinkZoneResponse.from(adminService.createShrinkZone(request));
    }

    /** 収縮候補地を更新する */
    @PutMapping("/shrink-zones/{id}")
    public ShrinkZoneResponse updateShrinkZone(
            @PathVariable Integer id,
            @Valid @RequestBody ShrinkZoneRequest request) {
        return ShrinkZoneResponse.from(adminService.updateShrinkZone(id, request));
    }

    /** 収縮候補地を削除する */
    @DeleteMapping("/shrink-zones/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deleteShrinkZone(@PathVariable Integer id) {
        adminService.deleteShrinkZone(id);
    }

    // ==========================================
    // SecondShrinkPoint CRUD
    // ==========================================

    /** 第二収縮地点候補一覧を取得する */
    @GetMapping("/second-shrink-points")
    public List<SecondShrinkPointResponse> getSecondShrinkPoints() {
        return adminService.getAllSecondShrinkPoints().stream()
                .map(SecondShrinkPointResponse::from)
                .toList();
    }

    /** 第二収縮地点候補を登録する */
    @PostMapping("/second-shrink-points")
    @ResponseStatus(HttpStatus.CREATED)
    public SecondShrinkPointResponse createSecondShrinkPoint(
            @Valid @RequestBody SecondShrinkPointRequest request) {
        return SecondShrinkPointResponse.from(adminService.createSecondShrinkPoint(request));
    }

    /** 第二収縮地点候補を更新する */
    @PutMapping("/second-shrink-points/{id}")
    public SecondShrinkPointResponse updateSecondShrinkPoint(
            @PathVariable Integer id,
            @Valid @RequestBody SecondShrinkPointRequest request) {
        return SecondShrinkPointResponse.from(adminService.updateSecondShrinkPoint(id, request));
    }

    /** 第二収縮地点候補を削除する */
    @DeleteMapping("/second-shrink-points/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deleteSecondShrinkPoint(@PathVariable Integer id) {
        adminService.deleteSecondShrinkPoint(id);
    }

    // ==========================================
    // BasePoint CRUD
    // ==========================================

    /** 拠点候補地一覧を取得する（マップIDで絞り込み） */
    @GetMapping("/base-points")
    public List<BasePointResponse> getBasePoints(
            @RequestParam(defaultValue = "normal") String mapId) {
        return adminService.getBasePointsByMapId(mapId).stream()
                .map(BasePointResponse::from)
                .toList();
    }

    /** 拠点候補地を登録する */
    @PostMapping("/base-points")
    @ResponseStatus(HttpStatus.CREATED)
    public BasePointResponse createBasePoint(@Valid @RequestBody BasePointRequest request) {
        return BasePointResponse.from(adminService.createBasePoint(request));
    }

    /** 拠点候補地を更新する */
    @PutMapping("/base-points/{id}")
    public BasePointResponse updateBasePoint(
            @PathVariable Integer id,
            @Valid @RequestBody BasePointRequest request) {
        return BasePointResponse.from(adminService.updateBasePoint(id, request));
    }

    /** 拠点候補地を削除する */
    @DeleteMapping("/base-points/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    public void deleteBasePoint(@PathVariable Integer id) {
        adminService.deleteBasePoint(id);
    }
}
