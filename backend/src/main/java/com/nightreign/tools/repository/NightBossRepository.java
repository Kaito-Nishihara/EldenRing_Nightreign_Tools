package com.nightreign.tools.repository;

import com.nightreign.tools.entity.NightBoss;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

/**
 * 夜ボスリポジトリ
 */
public interface NightBossRepository extends JpaRepository<NightBoss, Integer> {
    List<NightBoss> findByDay(Integer day);
}
