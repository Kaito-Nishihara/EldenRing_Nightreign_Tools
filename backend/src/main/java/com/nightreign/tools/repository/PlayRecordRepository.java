package com.nightreign.tools.repository;

import com.nightreign.tools.entity.PlayRecord;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import org.springframework.data.jpa.repository.EntityGraph;
import java.util.List;

/**
 * プレイ記録リポジトリ
 */
@Repository
public interface PlayRecordRepository extends JpaRepository<PlayRecord, Integer> {

    /** ボスIDで絞り込み */
    List<PlayRecord> findByBossId(String bossId);

    /** 地変IDで絞り込み */
    List<PlayRecord> findByJihenId(String jihenId);

    /** ボスID + 開始地点ID + 地変IDでパターン検索（N+1防止） */
    @EntityGraph(attributePaths = {"shrinkEntries", "shrinkEntries.zone"})
    List<PlayRecord> findByBossIdAndSpawnPointIdAndJihenId(String bossId, Integer spawnPointId, String jihenId);

    /** ボスID + 開始地点IDで地変なしのパターン検索（N+1防止） */
    @EntityGraph(attributePaths = {"shrinkEntries", "shrinkEntries.zone"})
    List<PlayRecord> findByBossIdAndSpawnPointIdAndJihenIsNull(String bossId, Integer spawnPointId);
}
