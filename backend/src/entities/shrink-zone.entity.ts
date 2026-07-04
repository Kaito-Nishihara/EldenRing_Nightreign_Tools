import { Entity, PrimaryGeneratedColumn, Column, BeforeInsert } from 'typeorm';

const decimalTransformer = {
  from: (v: string) => (v !== null && v !== undefined ? Number(v) : null),
  to: (v: number) => v,
};

/** 収縮地点エンティティ */
@Entity({ name: 'shrink_zones', schema: 'game' })
export class ShrinkZone {
  @PrimaryGeneratedColumn()
  id: number;

  @Column({ name: 'name_ja', length: 100 })
  nameJa: string;

  @Column({ name: 'icon_path', length: 200, nullable: true })
  iconPath: string;

  @Column({ name: 'center_x', type: 'decimal', precision: 7, scale: 2, transformer: decimalTransformer })
  centerX: number;

  @Column({ name: 'center_y', type: 'decimal', precision: 7, scale: 2, transformer: decimalTransformer })
  centerY: number;

  @Column({ type: 'decimal', precision: 7, scale: 2, transformer: decimalTransformer })
  radius: number;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
