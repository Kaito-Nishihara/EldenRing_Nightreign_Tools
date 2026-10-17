import {
  Entity,
  PrimaryColumn,
  Column,
  OneToMany,
  BeforeInsert,
} from 'typeorm';
import { BossWeakness } from './boss-weakness.entity';

/** ボスエンティティ */
@Entity({ name: 'bosses', schema: 'game' })
export class Boss {
  @PrimaryColumn({ length: 50 })
  id: string;

  @Column({ name: 'name_ja', length: 100 })
  nameJa: string;

  @Column({ name: 'name_en', length: 100, nullable: true })
  nameEn: string;

  @Column({ name: 'image_path', length: 200, nullable: true })
  imagePath: string;

  @Column({ name: 'is_dlc' })
  isDlc: boolean;

  @Column({ name: 'unlock_condition', length: 200, nullable: true })
  unlockCondition: string;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @OneToMany(() => BossWeakness, (w) => w.boss, { eager: true, cascade: true })
  weaknesses: BossWeakness[];

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
