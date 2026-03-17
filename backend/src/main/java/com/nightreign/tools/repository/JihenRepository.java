package com.nightreign.tools.repository;

import com.nightreign.tools.entity.Jihen;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

/**
 * 地変リポジトリ
 */
@Repository
public interface JihenRepository extends JpaRepository<Jihen, String> {

    /** エリアで絞り込み */
    List<Jihen> findByArea(String area);

    /** DLCフラグで絞り込み */
    List<Jihen> findByIsDlc(boolean isDlc);
}
