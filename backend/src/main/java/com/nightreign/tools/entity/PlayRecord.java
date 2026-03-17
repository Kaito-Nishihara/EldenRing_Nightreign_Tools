package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

/**
 * プレイ記録エンティティ
 * 1回のプレイセッションにおけるボス・地変・収縮パターンを記録する
 */
@Entity
@Table(name = "play_records", schema = "game")
public class PlayRecord {

    /** プレイ記録ID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 対戦したボス */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "boss_id", nullable = false)
    private Boss boss;

    /** 発生した地変（任意） */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "jihen_id")
    private Jihen jihen;

    /** スポーン地点（任意） */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "spawn_point_id")
    private SpawnPoint spawnPoint;

    /** メモ */
    @Column(columnDefinition = "TEXT")
    private String note;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    /** 収縮エントリ一覧（Day1-Phase1, Day1-Phase2, Day2-Phase1, Day2-Phase2） */
    @OneToMany(mappedBy = "playRecord", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<ShrinkEntry> shrinkEntries = new ArrayList<>();

    public PlayRecord() {
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

    public Boss getBoss() {
        return boss;
    }

    public void setBoss(Boss boss) {
        this.boss = boss;
    }

    public Jihen getJihen() {
        return jihen;
    }

    public void setJihen(Jihen jihen) {
        this.jihen = jihen;
    }

    public SpawnPoint getSpawnPoint() {
        return spawnPoint;
    }

    public void setSpawnPoint(SpawnPoint spawnPoint) {
        this.spawnPoint = spawnPoint;
    }

    public String getNote() {
        return note;
    }

    public void setNote(String note) {
        this.note = note;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public List<ShrinkEntry> getShrinkEntries() {
        return shrinkEntries;
    }

    public void setShrinkEntries(List<ShrinkEntry> shrinkEntries) {
        this.shrinkEntries = shrinkEntries;
    }
}
