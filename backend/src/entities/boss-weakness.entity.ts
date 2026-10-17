import { Entity, PrimaryGeneratedColumn, Column, ManyToOne, JoinColumn } from 'typeorm';
import { Boss } from './boss.entity';

/** ボス弱点エンティティ */
@Entity({ name: 'boss_weaknesses', schema: 'game' })
export class BossWeakness {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => Boss, (boss) => boss.weaknesses)
  @JoinColumn({ name: 'boss_id' })
  boss: Boss;

  @Column({ length: 30 })
  attribute: string;
}
