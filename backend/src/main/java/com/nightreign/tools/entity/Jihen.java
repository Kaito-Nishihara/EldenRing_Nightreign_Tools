package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;
import java.time.LocalDateTime;

/**
 * 地変エンティティ
 * ナイトレインの地変（特殊地形イベント）を管理する
 */
@Entity
@Table(name = "jihen", schema = "game")
public class Jihen {

    /** 地変ID（例: crater, mountain） */
    @Id
    @Column(length = 50)
    private String id;

    /** 日本語名 */
    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    /** 画像パス */
    @Column(name = "image_path", length = 200)
    private String imagePath;

    /** エリア（例: north, southwest） */
    @Column(nullable = false, length = 50)
    private String area;

    /** 中心X座標（0.0～1.0の正規化座標） */
    @Column(name = "center_x", nullable = false, precision = 5, scale = 4)
    private BigDecimal centerX;

    /** 中心Y座標（0.0～1.0の正規化座標） */
    @Column(name = "center_y", nullable = false, precision = 5, scale = 4)
    private BigDecimal centerY;

    /** 解放条件 */
    @Column(name = "unlock_condition", length = 200)
    private String unlockCondition;

    /** DLCコンテンツかどうか */
    @Column(name = "is_dlc", nullable = false)
    private boolean isDlc;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    public Jihen() {
    }

    // --- Getter / Setter ---

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getNameJa() {
        return nameJa;
    }

    public void setNameJa(String nameJa) {
        this.nameJa = nameJa;
    }

    public String getImagePath() {
        return imagePath;
    }

    public void setImagePath(String imagePath) {
        this.imagePath = imagePath;
    }

    public String getArea() {
        return area;
    }

    public void setArea(String area) {
        this.area = area;
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

    public String getUnlockCondition() {
        return unlockCondition;
    }

    public void setUnlockCondition(String unlockCondition) {
        this.unlockCondition = unlockCondition;
    }

    public boolean isDlc() {
        return isDlc;
    }

    public void setDlc(boolean dlc) {
        isDlc = dlc;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
