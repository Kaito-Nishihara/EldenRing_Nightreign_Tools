package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 収縮地点エンティティ
 * マップ収縮の候補地点を管理する
 */
@Entity
@Table(name = "shrink_zones", schema = "game")
public class ShrinkZone {

    /** 収縮地点ID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 日本語名 */
    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    /** アイコンパス */
    @Column(name = "icon_path", length = 200)
    private String iconPath;

    /** 中心X座標（0.0～1.0の正規化座標） */
    @Column(name = "center_x", nullable = false, precision = 5, scale = 4)
    private BigDecimal centerX;

    /** 中心Y座標（0.0～1.0の正規化座標） */
    @Column(name = "center_y", nullable = false, precision = 5, scale = 4)
    private BigDecimal centerY;

    /** 収縮半径（0.0～1.0の正規化値） */
    @Column(nullable = false, precision = 5, scale = 4)
    private BigDecimal radius;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    public ShrinkZone() {
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

    public String getIconPath() {
        return iconPath;
    }

    public void setIconPath(String iconPath) {
        this.iconPath = iconPath;
    }

    public BigDecimal getCenterX() {
        return centerX;
    }

    public void setCenterX(BigDecimal centerX) {
        this.centerX = centerX;
    }

    public BigDecimal getCenterY() {
        return centerY;
    }

    public void setCenterY(BigDecimal centerY) {
        this.centerY = centerY;
    }

    public BigDecimal getRadius() {
        return radius;
    }

    public void setRadius(BigDecimal radius) {
        this.radius = radius;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
