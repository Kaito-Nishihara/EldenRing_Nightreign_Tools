package com.nightreign.tools.repository;

import com.nightreign.tools.entity.SpawnPoint;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

/**
 * スポーン地点リポジトリ
 */
@Repository
public interface SpawnPointRepository extends JpaRepository<SpawnPoint, Integer> {
}
