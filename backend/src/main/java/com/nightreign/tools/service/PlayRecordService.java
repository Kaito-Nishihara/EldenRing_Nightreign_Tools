package com.nightreign.tools.service;

import com.nightreign.tools.dto.PlayRecordRequest;
import com.nightreign.tools.dto.PlayRecordResponse;
import com.nightreign.tools.dto.ShrinkEntryRequest;
import com.nightreign.tools.entity.*;
import com.nightreign.tools.repository.*;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.HashSet;
import java.util.Set;

/**
 * プレイ記録サービス
 * プレイ記録の保存とバリデーションを行う
 */
@Service
public class PlayRecordService {

    private final PlayRecordRepository playRecordRepository;
    private final BossRepository bossRepository;
    private final JihenRepository jihenRepository;
    private final SpawnPointRepository spawnPointRepository;
    private final ShrinkZoneRepository shrinkZoneRepository;

    public PlayRecordService(PlayRecordRepository playRecordRepository,
                             BossRepository bossRepository,
                             JihenRepository jihenRepository,
                             SpawnPointRepository spawnPointRepository,
                             ShrinkZoneRepository shrinkZoneRepository) {
        this.playRecordRepository = playRecordRepository;
        this.bossRepository = bossRepository;
        this.jihenRepository = jihenRepository;
        this.spawnPointRepository = spawnPointRepository;
        this.shrinkZoneRepository = shrinkZoneRepository;
    }

    /**
     * プレイ記録を保存する
     * 各IDの存在チェックとday+phaseの重複チェックを行う
     */
    @Transactional
    public PlayRecordResponse createPlayRecord(PlayRecordRequest request) {
        // ボスの存在チェック
        Boss boss = bossRepository.findById(request.bossId())
                .orElseThrow(() -> new IllegalArgumentException(
                        "bossId '" + request.bossId() + "' は存在しません"));

        // 地変の存在チェック（任意項目）
        Jihen jihen = null;
        if (request.jihenId() != null && !request.jihenId().isBlank()) {
            jihen = jihenRepository.findById(request.jihenId())
                    .orElseThrow(() -> new IllegalArgumentException(
                            "jihenId '" + request.jihenId() + "' は存在しません"));
        }

        // 開始地点の存在チェック
        SpawnPoint spawnPoint = spawnPointRepository.findById(request.spawnPointId())
                .orElseThrow(() -> new IllegalArgumentException(
                        "spawnPointId '" + request.spawnPointId() + "' は存在しません"));

        // day+phaseの重複チェック
        Set<String> dayPhaseKeys = new HashSet<>();
        for (ShrinkEntryRequest entry : request.entries()) {
            String key = entry.day() + "-" + entry.phase();
            if (!dayPhaseKeys.add(key)) {
                throw new IllegalArgumentException(
                        "同一レコード内で day=" + entry.day() + ", phase=" + entry.phase() + " が重複しています");
            }
        }

        // プレイ記録エンティティの作成
        PlayRecord record = new PlayRecord();
        record.setBoss(boss);
        record.setJihen(jihen);
        record.setSpawnPoint(spawnPoint);
        record.setNote(request.note());

        // 収縮エントリの作成
        for (ShrinkEntryRequest entryReq : request.entries()) {
            ShrinkZone zone = shrinkZoneRepository.findById(entryReq.zoneId())
                    .orElseThrow(() -> new IllegalArgumentException(
                            "zoneId '" + entryReq.zoneId() + "' は存在しません"));

            ShrinkEntry entry = new ShrinkEntry();
            entry.setPlayRecord(record);
            entry.setDay(entryReq.day());
            entry.setPhase(entryReq.phase());
            entry.setZone(zone);
            record.getShrinkEntries().add(entry);
        }

        PlayRecord saved = playRecordRepository.save(record);
        return new PlayRecordResponse(saved.getId(), saved.getCreatedAt());
    }
}
