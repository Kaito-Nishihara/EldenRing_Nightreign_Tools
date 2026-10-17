import { Entity, PrimaryGeneratedColumn, Column, ManyToOne, JoinColumn } from 'typeorm';
import { RelicEffect } from './relic-effect.entity';

const decimalTransformer = {
  from: (v: string) => (v !== null && v !== undefined ? Number(v) : null),
  to: (v: number) => v,
};

/** 遺物効果ランク別数値エンティティ */
@Entity({ name: 'relic_effect_ranks', schema: 'game' })
export class RelicEffectRank {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => RelicEffect, (e) => e.ranks)
  @JoinColumn({ name: 'effect_id' })
  effect: RelicEffect;

  @Column({ type: 'smallint' })
  rank: number;

  @Column({ type: 'decimal', precision: 10, scale: 4, nullable: true, transformer: decimalTransformer })
  value: number;

  @Column({ length: 20 })
  unit: string;

  @Column({ name: 'has_demerit', default: false })
  hasDemerit: boolean;
}
