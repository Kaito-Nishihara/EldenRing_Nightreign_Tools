import { Entity, PrimaryColumn, Column } from 'typeorm';

/** 遺物効果カテゴリエンティティ */
@Entity({ name: 'relic_effect_categories', schema: 'game' })
export class RelicEffectCategory {
  @PrimaryColumn({ length: 30 })
  id: string;

  @Column({ name: 'name_ja', length: 50 })
  nameJa: string;

  @Column({ name: 'sort_order' })
  sortOrder: number;
}
