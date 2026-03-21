package com.nightreign.tools.service;

import com.nightreign.tools.dto.BasePointResponse;
import com.nightreign.tools.dto.BossResponse;
import com.nightreign.tools.dto.JihenResponse;
import com.nightreign.tools.dto.NightBossResponse;
import com.nightreign.tools.dto.ShrinkZoneResponse;
import com.nightreign.tools.dto.SpawnPointResponse;
import com.nightreign.tools.repository.BasePointRepository;
import com.nightreign.tools.repository.BossRepository;
import com.nightreign.tools.repository.JihenRepository;
import com.nightreign.tools.repository.NightBossRepository;
import com.nightreign.tools.repository.ShrinkZoneRepository;
import com.nightreign.tools.repository.SpawnPointRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

/**
 * マスタデータサービス
 * ボス・地変・収縮候補地・開始地点のマスタデータを提供する
 */
@Service
@Transactional(readOnly = true)
public class MasterService {

    private final BasePointRepository basePointRepository;
    private final BossRepository bossRepository;
    private final JihenRepository jihenRepository;
    private final NightBossRepository nightBossRepository;
    private final ShrinkZoneRepository shrinkZoneRepository;
    private final SpawnPointRepository spawnPointRepository;

    public MasterService(BasePointRepository basePointRepository,
                         BossRepository bossRepository,
                         JihenRepository jihenRepository,
                         NightBossRepository nightBossRepository,
                         ShrinkZoneRepository shrinkZoneRepository,
                         SpawnPointRepository spawnPointRepository) {
        this.basePointRepository = basePointRepository;
        this.bossRepository = bossRepository;
        this.jihenRepository = jihenRepository;
        this.nightBossRepository = nightBossRepository;
        this.shrinkZoneRepository = shrinkZoneRepository;
        this.spawnPointRepository = spawnPointRepository;
    }

    /** ボス一覧を取得する */
    public List<BossResponse> getAllBosses() {
        return bossRepository.findAllBy().stream()
                .map(BossResponse::from)
                .toList();
    }

    /** 地変一覧を取得する */
    public List<JihenResponse> getAllJihen() {
        return jihenRepository.findAll().stream()
                .map(JihenResponse::from)
                .toList();
    }

    /** 収縮候補地一覧を取得する */
    public List<ShrinkZoneResponse> getAllShrinkZones() {
        return shrinkZoneRepository.findAll().stream()
                .map(ShrinkZoneResponse::from)
                .toList();
    }

    /** 開始地点一覧を取得する */
    public List<SpawnPointResponse> getAllSpawnPoints() {
        return spawnPointRepository.findAll().stream()
                .map(SpawnPointResponse::from)
                .toList();
    }

    /** 夜ボス一覧を取得する */
    public List<NightBossResponse> getAllNightBosses() {
        return nightBossRepository.findAll().stream()
                .map(NightBossResponse::from)
                .toList();
    }

    /** 拠点候補地一覧を取得する（マップIDで絞り込み） */
    public List<BasePointResponse> getBasePointsByMapId(String mapId) {
        return basePointRepository.findByMapId(mapId).stream()
                .map(BasePointResponse::from)
                .toList();
    }
}
