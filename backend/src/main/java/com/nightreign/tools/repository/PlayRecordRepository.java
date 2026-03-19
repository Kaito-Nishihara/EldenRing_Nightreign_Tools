package com.nightreign.tools.repository;

import com.nightreign.tools.entity.PlayRecord;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

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

    /** ボスID + 開始地点ID + 地変IDでパターン検索（DISTINCTでJOIN重複を防止） */
    @Query("SELECT DISTINCT pr FROM PlayRecord pr " +
            "LEFT JOIN FETCH pr.shrinkEntries se " +
            "LEFT JOIN FETCH se.zone " +
            "WHERE pr.boss.id = :bossId AND pr.spawnPoint.id = :spawnPointId AND pr.jihen.id = :jihenId")
    List<PlayRecord> findByBossIdAndSpawnPointIdAndJihenId(
            @Param("bossId") String bossId,
            @Param("spawnPointId") Integer spawnPointId,
            @Param("jihenId") String jihenId);

    /** ボスID + 開始地点IDで地変なしのパターン検索（DISTINCTでJOIN重複を防止） */
    @Query("SELECT DISTINCT pr FROM PlayRecord pr " +
            "LEFT JOIN FETCH pr.shrinkEntries se " +
            "LEFT JOIN FETCH se.zone " +
            "WHERE pr.boss.id = :bossId AND pr.spawnPoint.id = :spawnPointId AND pr.jihen IS NULL")
    List<PlayRecord> findByBossIdAndSpawnPointIdAndJihenIsNull(
            @Param("bossId") String bossId,
            @Param("spawnPointId") Integer spawnPointId);
}
