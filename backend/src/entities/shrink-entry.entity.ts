import { Entity, PrimaryGeneratedColumn, Column, ManyToOne, JoinColumn } from 'typeorm';
import { PlayRecord } from './play-record.entity';
import { ShrinkZone } from './shrink-zone.entity';

/** 収縮エントリエンティティ */
@Entity({ name: 'shrink_entries', schema: 'game' })
export class ShrinkEntry {
  @PrimaryGeneratedColumn()
  id: number;

  @ManyToOne(() => PlayRecord, (pr) => pr.shrinkEntries)
  @JoinColumn({ name: 'play_record_id' })
  playRecord: PlayRecord;

  @Column()
  day: number;

  @Column()
  phase: number;

  @ManyToOne(() => ShrinkZone, { eager: true })
  @JoinColumn({ name: 'zone_id' })
  zone: ShrinkZone;
}
