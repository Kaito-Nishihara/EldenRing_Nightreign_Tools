package com.nightreign.tools.repository;

import com.nightreign.tools.entity.ShrinkEntry;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

/**
 * 収縮エントリリポジトリ
 */
@Repository
public interface ShrinkEntryRepository extends JpaRepository<ShrinkEntry, Integer> {

    /** プレイ記録IDで絞り込み */
    List<ShrinkEntry> findByPlayRecordId(Integer playRecordId);

    /** 収縮地点IDで絞り込み */
    List<ShrinkEntry> findByZoneId(Integer zoneId);
}
