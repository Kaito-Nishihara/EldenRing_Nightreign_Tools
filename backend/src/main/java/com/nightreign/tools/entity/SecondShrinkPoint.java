package com.nightreign.tools.entity;

import jakarta.persistence.*;

import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 第二収縮地点候補エンティティ
 * マップ上の第二収縮候補地点の座標を管理する
 * 夜ボスとの紐付けは記録画面で行う
 */
@Entity
@Table(name = "second_shrink_points", schema = "game")
public class SecondShrinkPoint {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    @Column(nullable = false, precision = 7, scale = 2)
    private BigDecimal x;

    @Column(nullable = false, precision = 7, scale = 2)
    private BigDecimal y;

    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getNameJa() {
        return nameJa;
    }

    public void setNameJa(String nameJa) {
        this.nameJa = nameJa;
    }

    public BigDecimal getX() {
        return x;
    }

    public void setX(BigDecimal x) {
        this.x = x;
    }

    public BigDecimal getY() {
        return y;
    }

    public void setY(BigDecimal y) {
        this.y = y;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
