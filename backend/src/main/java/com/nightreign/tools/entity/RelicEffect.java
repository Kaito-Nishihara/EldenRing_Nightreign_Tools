package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

/**
 * 遺物効果エンティティ
 * 通常効果（is_deep=false）と深層効果（is_deep=true）を同テーブルで管理する
 */
@Entity
@Table(name = "relic_effects", schema = "game")
public class RelicEffect {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** カテゴリ */
    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "category_id", nullable = false)
    private RelicEffectCategory category;

    /** 効果名（日本語） */
    @Column(name = "name_ja", nullable = false, length = 200)
    private String nameJa;

    /** 深層効果フラグ */
    @Column(name = "is_deep", nullable = false)
    private boolean isDeep;

    /**
     * 効果の正負
     * positive: メリット / negative: デメリット
     */
    @Column(name = "effect_sign", nullable = false, length = 10)
    private String effectSign;

    /**
     * 計算種別
     * multiply / percent_add / flat_add / special
     */
    @Column(name = "calc_type", nullable = false, length = 20)
    private String calcType;

    /** 対象属性（physical / magic / vigor など。汎用効果は null） */
    @Column(length = 30)
    private String attribute;

    /**
     * 重ね掛け可否
     * yes / no / conditional
     */
    @Column(nullable = false, length = 15)
    private String stackable;

    /** ランク最小値（ランクなし効果は 0） */
    @Column(name = "rank_min", nullable = false)
    private Short rankMin;

    /** ランク最大値（ランクなし効果は 0） */
    @Column(name = "rank_max", nullable = false)
    private Short rankMax;

    /** 発動条件テキスト */
    @Column(name = "condition_ja", columnDefinition = "TEXT")
    private String conditionJa;

    /** 備考 */
    @Column(name = "notes_ja", columnDefinition = "TEXT")
    private String notesJa;

    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    /** ランク別数値（eager: 一覧取得時に一括ロード） */
    @OneToMany(mappedBy = "effect", fetch = FetchType.EAGER,
               cascade = CascadeType.ALL, orphanRemoval = true)
    @OrderBy("rank ASC")
    private List<RelicEffectRank> ranks = new ArrayList<>();

    @PrePersist
    protected void onCreate() {
        this.createdAt = LocalDateTime.now();
    }

    public RelicEffect() {}

    // --- Getter / Setter ---

    public Integer getId() { return id; }
    public void setId(Integer id) { this.id = id; }

    public RelicEffectCategory getCategory() { return category; }
    public void setCategory(RelicEffectCategory category) { this.category = category; }

    public String getNameJa() { return nameJa; }
    public void setNameJa(String nameJa) { this.nameJa = nameJa; }

    public boolean isDeep() { return isDeep; }
    public void setDeep(boolean deep) { isDeep = deep; }

    public String getEffectSign() { return effectSign; }
    public void setEffectSign(String effectSign) { this.effectSign = effectSign; }

    public String getCalcType() { return calcType; }
    public void setCalcType(String calcType) { this.calcType = calcType; }

    public String getAttribute() { return attribute; }
    public void setAttribute(String attribute) { this.attribute = attribute; }

    public String getStackable() { return stackable; }
    public void setStackable(String stackable) { this.stackable = stackable; }

    public Short getRankMin() { return rankMin; }
    public void setRankMin(Short rankMin) { this.rankMin = rankMin; }

    public Short getRankMax() { return rankMax; }
    public void setRankMax(Short rankMax) { this.rankMax = rankMax; }

    public String getConditionJa() { return conditionJa; }
    public void setConditionJa(String conditionJa) { this.conditionJa = conditionJa; }

    public String getNotesJa() { return notesJa; }
    public void setNotesJa(String notesJa) { this.notesJa = notesJa; }

    public LocalDateTime getCreatedAt() { return createdAt; }
    public void setCreatedAt(LocalDateTime createdAt) { this.createdAt = createdAt; }

    public List<RelicEffectRank> getRanks() { return ranks; }
    public void setRanks(List<RelicEffectRank> ranks) { this.ranks = ranks; }
}
