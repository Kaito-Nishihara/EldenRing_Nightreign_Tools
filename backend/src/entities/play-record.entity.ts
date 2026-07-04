import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  ManyToOne,
  OneToMany,
  JoinColumn,
  BeforeInsert,
} from 'typeorm';
import { Boss } from './boss.entity';
import { Jihen } from './jihen.entity';
import { SpawnPoint } from './spawn-point.entity';
import { ShrinkEntry } from './shrink-entry.entity';

/** プレイ記録エンティティ */
@Entity({ name: 'play_records', schema: 'game' })
export class PlayRecord {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => Boss)
  @JoinColumn({ name: 'boss_id' })
  boss: Boss;

  @ManyToOne(() => Jihen, { nullable: true })
  @JoinColumn({ name: 'jihen_id' })
  jihen: Jihen;

  @ManyToOne(() => SpawnPoint, { nullable: true })
  @JoinColumn({ name: 'spawn_point_id' })
  spawnPoint: SpawnPoint;

  @Column({ type: 'text', nullable: true })
  note: string;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @OneToMany(() => ShrinkEntry, (e) => e.playRecord, { cascade: true })
  shrinkEntries: ShrinkEntry[];

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
