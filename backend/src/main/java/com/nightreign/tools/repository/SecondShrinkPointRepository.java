package com.nightreign.tools.repository;

import com.nightreign.tools.entity.SecondShrinkPoint;
import org.springframework.data.jpa.repository.JpaRepository;

/**
 * 第二収縮地点候補リポジトリ
 */
public interface SecondShrinkPointRepository extends JpaRepository<SecondShrinkPoint, Integer> {
}
