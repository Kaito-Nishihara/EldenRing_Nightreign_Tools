package com.nightreign.tools.repository;

import com.nightreign.tools.entity.ShrinkZone;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

/**
 * 収縮地点リポジトリ
 */
@Repository
public interface ShrinkZoneRepository extends JpaRepository<ShrinkZone, Integer> {
}
