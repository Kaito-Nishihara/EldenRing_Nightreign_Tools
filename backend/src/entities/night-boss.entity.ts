import { Entity, PrimaryGeneratedColumn, Column } from 'typeorm';

/** 夜ボスエンティティ */
@Entity({ name: 'night_bosses', schema: 'game' })
export class NightBoss {
  @PrimaryGeneratedColumn()
  id: number;

  @Column({ name: 'name_ja', length: 100 })
  nameJa: string;

  @Column()
  day: number;
}
