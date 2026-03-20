package com.nightreign.tools.service;

import com.nightreign.tools.dto.BasePointRequest;
import com.nightreign.tools.dto.ShrinkZoneRequest;
import com.nightreign.tools.dto.SpawnPointRequest;
import com.nightreign.tools.entity.BasePoint;
import com.nightreign.tools.entity.ShrinkZone;
import com.nightreign.tools.entity.SpawnPoint;
import com.nightreign.tools.repository.BasePointRepository;
import com.nightreign.tools.repository.ShrinkZoneRepository;
import com.nightreign.tools.repository.SpawnPointRepository;
import jakarta.persistence.EntityNotFoundException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

/**
 * 管理画面用サービス
 * マスタデータのCRUD操作を提供する
 */
@Service
@Transactional
public class AdminService {

    private final SpawnPointRepository spawnPointRepository;
    private final ShrinkZoneRepository shrinkZoneRepository;
    private final BasePointRepository basePointRepository;

    public AdminService(SpawnPointRepository spawnPointRepository,
                        ShrinkZoneRepository shrinkZoneRepository,
                        BasePointRepository basePointRepository) {
        this.spawnPointRepository = spawnPointRepository;
        this.shrinkZoneRepository = shrinkZoneRepository;
        this.basePointRepository = basePointRepository;
    }

    // ==========================================
    // SpawnPoint CRUD
    // ==========================================

    /** 開始地点一覧を取得する */
    @Transactional(readOnly = true)
    public List<SpawnPoint> getAllSpawnPoints() {
        return spawnPointRepository.findAll();
    }

    /** 開始地点を登録する */
    public SpawnPoint createSpawnPoint(SpawnPointRequest request) {
        SpawnPoint entity = new SpawnPoint();
        entity.setNameJa(request.nameJa());
        entity.setX(request.x());
        entity.setY(request.y());
        entity.setNearbyBaseType(request.nearbyBaseType());
        return spawnPointRepository.save(entity);
    }

    /** 開始地点を更新する */
    public SpawnPoint updateSpawnPoint(Integer id, SpawnPointRequest request) {
        SpawnPoint entity = spawnPointRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(
                        "開始地点が見つかりません: id=" + id));
        entity.setNameJa(request.nameJa());
        entity.setX(request.x());
        entity.setY(request.y());
        entity.setNearbyBaseType(request.nearbyBaseType());
        return spawnPointRepository.save(entity);
    }

    /** 開始地点を削除する */
    public void deleteSpawnPoint(Integer id) {
        if (!spawnPointRepository.existsById(id)) {
            throw new EntityNotFoundException("開始地点が見つかりません: id=" + id);
        }
        spawnPointRepository.deleteById(id);
    }

    // ==========================================
    // ShrinkZone CRUD
    // ==========================================

    /** 収縮候補地一覧を取得する */
    @Transactional(readOnly = true)
    public List<ShrinkZone> getAllShrinkZones() {
        return shrinkZoneRepository.findAll();
    }

    /** 収縮候補地を登録する */
    public ShrinkZone createShrinkZone(ShrinkZoneRequest request) {
        ShrinkZone entity = new ShrinkZone();
        entity.setNameJa(request.nameJa());
        entity.setIconPath(request.iconPath());
        entity.setCenterX(request.centerX());
        entity.setCenterY(request.centerY());
        entity.setRadius(request.radius());
        return shrinkZoneRepository.save(entity);
    }

    /** 収縮候補地を更新する */
    public ShrinkZone updateShrinkZone(Integer id, ShrinkZoneRequest request) {
        ShrinkZone entity = shrinkZoneRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(
                        "収縮候補地が見つかりません: id=" + id));
        entity.setNameJa(request.nameJa());
        entity.setIconPath(request.iconPath());
        entity.setCenterX(request.centerX());
        entity.setCenterY(request.centerY());
        entity.setRadius(request.radius());
        return shrinkZoneRepository.save(entity);
    }

    /** 収縮候補地を削除する */
    public void deleteShrinkZone(Integer id) {
        if (!shrinkZoneRepository.existsById(id)) {
            throw new EntityNotFoundException("収縮候補地が見つかりません: id=" + id);
        }
        shrinkZoneRepository.deleteById(id);
    }

    // ==========================================
    // BasePoint CRUD
    // ==========================================

    /** 拠点候補地一覧を取得する（マップIDで絞り込み） */
    @Transactional(readOnly = true)
    public List<BasePoint> getBasePointsByMapId(String mapId) {
        return basePointRepository.findByMapId(mapId);
    }

    /** 拠点候補地を登録する */
    public BasePoint createBasePoint(BasePointRequest request) {
        BasePoint entity = new BasePoint();
        entity.setMapId(request.mapId());
        entity.setNameJa(request.nameJa());
        entity.setX(request.x());
        entity.setY(request.y());
        return basePointRepository.save(entity);
    }

    /** 拠点候補地を更新する */
    public BasePoint updateBasePoint(Integer id, BasePointRequest request) {
        BasePoint entity = basePointRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(
                        "拠点候補地が見つかりません: id=" + id));
        entity.setMapId(request.mapId());
        entity.setNameJa(request.nameJa());
        entity.setX(request.x());
        entity.setY(request.y());
        return basePointRepository.save(entity);
    }

    /** 拠点候補地を削除する */
    public void deleteBasePoint(Integer id) {
        if (!basePointRepository.existsById(id)) {
            throw new EntityNotFoundException("拠点候補地が見つかりません: id=" + id);
        }
        basePointRepository.deleteById(id);
    }
}
