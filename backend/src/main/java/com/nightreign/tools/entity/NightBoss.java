package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;

/**
 * 夜ボスエンティティ
 * 1日目・2日目の夜に出現するボスの情報を保持する
 * 最終ボス（夜の王）とは別管理
 */
@Entity
@Table(name = "night_bosses", schema = "game")
public class NightBoss {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 日本語名 */
    @Column(name = "name_ja", nullable = false, length = 100)
    private String nameJa;

    /** 出現日（1=1日目夜, 2=2日目夜） */
    @Column(nullable = false)
    private Integer day;

    /** 作成日時 */
    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

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

    public Integer getDay() {
        return day;
    }

    public void setDay(Integer day) {
        this.day = day;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}
