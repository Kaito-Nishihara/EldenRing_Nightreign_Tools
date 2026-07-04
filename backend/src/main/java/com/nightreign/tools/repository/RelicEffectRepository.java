package com.nightreign.tools.repository;

import com.nightreign.tools.entity.RelicEffect;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;

/**
 * 遺物効果リポジトリ
 */
@Repository
public interface RelicEffectRepository extends JpaRepository<RelicEffect, Integer> {

    /** カテゴリのソート順で全効果を取得する */
    @Query("SELECT e FROM RelicEffect e JOIN FETCH e.category c ORDER BY c.sortOrder, e.isDeep, e.id")
    List<RelicEffect> findAllOrderByCategorySort();
}
