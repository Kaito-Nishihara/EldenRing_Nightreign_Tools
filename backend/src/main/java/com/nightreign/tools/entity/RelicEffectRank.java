package com.nightreign.tools.entity;

import jakarta.persistence.*;
import java.math.BigDecimal;

/**
 * 遺物効果ランク別数値エンティティ
 * ランクごとの効果倍率・数値を管理する
 */
@Entity
@Table(name = "relic_effect_ranks", schema = "game",
       uniqueConstraints = @UniqueConstraint(columnNames = {"effect_id", "rank"}))
public class RelicEffectRank {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    /** 対応する遺物効果 */
    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "effect_id", nullable = false)
    private RelicEffect effect;

    /** ランク番号（ランクなし効果は 0） */
    @Column(nullable = false)
    private Short rank;

    /** 数値 */
    @Column(nullable = false, precision = 10, scale = 4)
    private BigDecimal value;

    /**
     * 単位
     * multiplier / percent / flat / per_sec / none
     */
    @Column(nullable = false, length = 20)
    private String unit;

    /** デメリットあり（rank=0の効果で正負が条件付きの場合） */
    @Column(name = "has_demerit", nullable = false)
    private boolean hasDemerit = false;

    public RelicEffectRank() {}

    // --- Getter / Setter ---

    public Integer getId() { return id; }
    public void setId(Integer id) { this.id = id; }

    public RelicEffect getEffect() { return effect; }
    public void setEffect(RelicEffect effect) { this.effect = effect; }

    public Short getRank() { return rank; }
    public void setRank(Short rank) { this.rank = rank; }

    public BigDecimal getValue() { return value; }
    public void setValue(BigDecimal value) { this.value = value; }

    public String getUnit() { return unit; }
    public void setUnit(String unit) { this.unit = unit; }

    public boolean isHasDemerit() { return hasDemerit; }
    public void setHasDemerit(boolean hasDemerit) { this.hasDemerit = hasDemerit; }
}
