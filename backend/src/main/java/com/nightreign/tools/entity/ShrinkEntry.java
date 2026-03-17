package com.nightreign.tools.entity;

import jakarta.persistence.*;

/**
 * 収縮エントリエンティティ
 * プレイ記録に紐づく各フェーズの収縮地点を記録する
 * Day1-Phase1, Day1-Phase2, Day2-Phase1, Day2-Phase2 の最大4エントリ
 */
@Entity
@Table(name = "shrink_entries", schema = "game",
        uniqueConstraints = @UniqueConstraint(columnNames = {"play_record_id", "day", "phase"}))
public class ShrinkEntry {

    /** 収縮エントリID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 紐づくプレイ記録 */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "play_record_id", nullable = false)
    private PlayRecord playRecord;

    /** 日（1 or 2） */
    @Column(nullable = false)
    private Integer day;

    /** フェーズ（1 or 2） */
    @Column(nullable = false)
    private Integer phase;

    /** 収縮先の地点 */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "zone_id", nullable = false)
    private ShrinkZone zone;

    public ShrinkEntry() {
    }

    // --- Getter / Setter ---

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public PlayRecord getPlayRecord() {
        return playRecord;
    }

    public void setPlayRecord(PlayRecord playRecord) {
        this.playRecord = playRecord;
    }

    public Integer getDay() {
        return day;
    }

    public void setDay(Integer day) {
        this.day = day;
    }

    public Integer getPhase() {
        return phase;
    }

    public void setPhase(Integer phase) {
        this.phase = phase;
    }

    public ShrinkZone getZone() {
        return zone;
    }

    public void setZone(ShrinkZone zone) {
        this.zone = zone;
    }
}
