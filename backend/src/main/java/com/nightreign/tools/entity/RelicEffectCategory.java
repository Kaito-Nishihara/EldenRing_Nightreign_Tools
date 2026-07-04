package com.nightreign.tools.entity;

import jakarta.persistence.*;

/**
 * 遺物効果カテゴリエンティティ
 * stat / attack / cut_rate / recovery / status_resist / arts_gauge / starting_item / other
 */
@Entity
@Table(name = "relic_effect_categories", schema = "game")
public class RelicEffectCategory {

    @Id
    @Column(length = 30)
    private String id;

    @Column(name = "name_ja", nullable = false, length = 50)
    private String nameJa;

    @Column(name = "sort_order", nullable = false)
    private Short sortOrder;

    public RelicEffectCategory() {}

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getNameJa() { return nameJa; }
    public void setNameJa(String nameJa) { this.nameJa = nameJa; }

    public Short getSortOrder() { return sortOrder; }
    public void setSortOrder(Short sortOrder) { this.sortOrder = sortOrder; }
}
