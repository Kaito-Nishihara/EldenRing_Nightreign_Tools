package com.nightreign.tools.repository;

import com.nightreign.tools.entity.PlayRecord;
import org.springframework.data.jpa.repository.JpaRepository;
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
}
