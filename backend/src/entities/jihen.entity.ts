import { Entity, PrimaryColumn, Column, BeforeInsert } from 'typeorm';

const decimalTransformer = {
  from: (v: string) => (v !== null && v !== undefined ? Number(v) : null),
  to: (v: number) => v,
};

/** 地変エンティティ */
@Entity({ name: 'jihen', schema: 'game' })
export class Jihen {
  @PrimaryColumn({ length: 50 })
  id: string;

  @Column({ name: 'name_ja', length: 100 })
  nameJa: string;

  @Column({ name: 'image_path', length: 200, nullable: true })
  imagePath: string;

  @Column({ length: 50 })
  area: string;

  @Column({
    name: 'center_x',
    type: 'decimal',
    precision: 5,
    scale: 4,
    transformer: decimalTransformer,
  })
  centerX: number;

  @Column({
    name: 'center_y',
    type: 'decimal',
    precision: 5,
    scale: 4,
    transformer: decimalTransformer,
  })
  centerY: number;

  @Column({ name: 'unlock_condition', length: 200, nullable: true })
  unlockCondition: string;

  @Column({ name: 'is_dlc' })
  isDlc: boolean;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
