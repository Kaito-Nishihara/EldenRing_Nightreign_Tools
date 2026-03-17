package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

/**
 * ボスエンティティ
 * ナイトレインに登場するボスの情報を保持する
 */
@Entity
@Table(name = "bosses", schema = "game")
public class Boss {

    /** ボスID（例: gladius, edere） */
    @Id
    @Column(length = 50)
    private String id;

    /** 日本語名 */
    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    /** 英語名 */
    @Column(name = "name_en", length = 100)
    private String nameEn;

    /** 画像パス */
    @Column(name = "image_path", length = 200)
    private String imagePath;

    /** DLCコンテンツかどうか */
    @Column(name = "is_dlc", nullable = false)
    private boolean isDlc;

    /** 解放条件 */
    @Column(name = "unlock_condition", length = 200)
    private String unlockCondition;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    /** 弱点一覧 */
    @OneToMany(mappedBy = "boss", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<BossWeakness> weaknesses = new ArrayList<>();

    /** プレイ記録一覧 */
    @OneToMany(mappedBy = "boss")
    private List<PlayRecord> playRecords = new ArrayList<>();

    public Boss() {
    }

    /** 初回永続化時にcreatedAtを自動設定する */
    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
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

    public String getNameEn() {
        return nameEn;
    }

    public void setNameEn(String nameEn) {
        this.nameEn = nameEn;
    }

    public String getImagePath() {
        return imagePath;
    }

    public void setImagePath(String imagePath) {
        this.imagePath = imagePath;
    }

    public boolean isDlc() {
        return isDlc;
    }

    public void setDlc(boolean dlc) {
        isDlc = dlc;
    }

    public String getUnlockCondition() {
        return unlockCondition;
    }

    public void setUnlockCondition(String unlockCondition) {
        this.unlockCondition = unlockCondition;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public List<BossWeakness> getWeaknesses() {
        return weaknesses;
    }

    public void setWeaknesses(List<BossWeakness> weaknesses) {
        this.weaknesses = weaknesses;
    }

    public List<PlayRecord> getPlayRecords() {
        return playRecords;
    }

    public void setPlayRecords(List<PlayRecord> playRecords) {
        this.playRecords = playRecords;
    }
}
