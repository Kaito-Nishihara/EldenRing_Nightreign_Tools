package com.nightreign.tools.repository;

import com.nightreign.tools.entity.Boss;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

/**
 * ボスリポジトリ
 */
@Repository
public interface BossRepository extends JpaRepository<Boss, String> {

    /** DLCフラグで絞り込み */
    List<Boss> findByIsDlc(boolean isDlc);
}
