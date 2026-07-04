import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  ManyToOne,
  OneToMany,
  JoinColumn,
  BeforeInsert,
} from 'typeorm';
import { RelicEffectCategory } from './relic-effect-category.entity';
import { RelicEffectRank } from './relic-effect-rank.entity';

/** 遺物効果エンティティ */
@Entity({ name: 'relic_effects', schema: 'game' })
export class RelicEffect {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => RelicEffectCategory, { eager: true })
  @JoinColumn({ name: 'category_id' })
  category: RelicEffectCategory;

  @Column({ name: 'name_ja', length: 200 })
  nameJa: string;

  @Column({ name: 'is_deep', default: false })
  isDeep: boolean;

  @Column({ name: 'effect_sign', length: 10 })
  effectSign: string;

  @Column({ name: 'calc_type', length: 20 })
  calcType: string;

  @Column({ length: 30, nullable: true })
  attribute: string;

  @Column({ length: 15 })
  stackable: string;

  @Column({ name: 'rank_min', type: 'smallint', default: 0 })
  rankMin: number;

  @Column({ name: 'rank_max', type: 'smallint', default: 0 })
  rankMax: number;

  @Column({ name: 'condition_ja', type: 'text', nullable: true })
  conditionJa: string;

  @Column({ name: 'notes_ja', type: 'text', nullable: true })
  notesJa: string;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @OneToMany(() => RelicEffectRank, (r) => r.effect, { eager: true, cascade: true })
  ranks: RelicEffectRank[];

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
