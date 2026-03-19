package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * スポーン地点エンティティ
 * プレイヤーの初期出現地点を管理する
 */
@Entity
@Table(name = "spawn_points", schema = "game")
public class SpawnPoint {

    /** スポーン地点ID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 日本語名 */
    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    /** X座標（0.0～1.0の正規化座標） */
    @Column(nullable = false, precision = 5, scale = 4)
    private BigDecimal x;

    /** Y座標（0.0～1.0の正規化座標） */
    @Column(nullable = false, precision = 5, scale = 4)
    private BigDecimal y;

    /** 近隣の拠点タイプ（例: church, camp） */
    @Column(name = "nearby_base_type", nullable = false, length = 30)
    private String nearbyBaseType;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    public SpawnPoint() {
    }

    /** 初回永続化時にcreatedAtを自動設定する */
    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    // --- Getter / Setter ---

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

    public String getNearbyBaseType() {
        return nearbyBaseType;
    }

    public void setNearbyBaseType(String nearbyBaseType) {
        this.nearbyBaseType = nearbyBaseType;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
