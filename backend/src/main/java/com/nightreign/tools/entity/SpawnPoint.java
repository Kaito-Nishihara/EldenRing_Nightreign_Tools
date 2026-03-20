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

    /** X座標（ピクセル座標） */
    @Column(nullable = false, precision = 7, scale = 2)
    private BigDecimal x;

    /** Y座標（ピクセル座標） */
    @Column(nullable = false, precision = 7, scale = 2)
    private BigDecimal y;

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

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
