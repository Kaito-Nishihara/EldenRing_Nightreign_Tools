package com.nightreign.tools.repository;

import com.nightreign.tools.entity.BasePoint;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

/**
 * 拠点候補地リポジトリ
 */
public interface BasePointRepository extends JpaRepository<BasePoint, Integer> {
    /** マップIDで絞り込み */
    List<BasePoint> findByMapId(String mapId);
}
