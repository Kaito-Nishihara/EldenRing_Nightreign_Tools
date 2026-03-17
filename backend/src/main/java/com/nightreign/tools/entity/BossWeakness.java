package com.nightreign.tools.entity;

import jakarta.persistence.*;

/**
 * ボス弱点エンティティ
 * ボスが持つ弱点属性を管理する
 */
@Entity
@Table(name = "boss_weaknesses", schema = "game")
public class BossWeakness {

    /** 弱点ID（自動採番） */
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 紐づくボス */
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "boss_id", nullable = false)
    private Boss boss;

    /** 弱点属性（例: holy, fire, lightning） */
    @Column(nullable = false, length = 30)
    private String attribute;

    public BossWeakness() {
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

    public String getAttribute() {
        return attribute;
    }

    public void setAttribute(String attribute) {
        this.attribute = attribute;
    }
}
