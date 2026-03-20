package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 拠点候補地エンティティ
 * マップ上の固定座標で、ゲームごとに異なる拠点タイプが出現する
 */
@Entity
@Table(name = "base_points", schema = "game")
public class BasePoint {

    /** 拠点候補地ID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** マップ識別子（normal, kakou, sankan, fuhai, nokurateou） */
    @Column(name = "map_id", nullable = false, length = 20)
    private String mapId = "normal";

    /** 識別用の名前（任意） */
    @Column(name = "name_ja", length = 100)
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

    public BasePoint() {
    }

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

    public String getMapId() {
        return mapId;
    }

    public void setMapId(String mapId) {
        this.mapId = mapId;
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
