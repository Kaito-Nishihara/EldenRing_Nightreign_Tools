import { Entity, PrimaryGeneratedColumn, Column, BeforeInsert } from 'typeorm';

const decimalTransformer = {
  from: (v: string) => (v !== null && v !== undefined ? Number(v) : null),
  to: (v: number) => v,
};

/** 拠点候補地エンティティ */
@Entity({ name: 'base_points', schema: 'game' })
export class BasePoint {
  @PrimaryGeneratedColumn()
  id: number;

  @Column({ name: 'map_id', length: 30 })
  mapId: string;

  @Column({ name: 'name_ja', length: 100, nullable: true })
  nameJa: string;

  @Column({ type: 'decimal', precision: 7, scale: 2, transformer: decimalTransformer })
  x: number;

  @Column({ type: 'decimal', precision: 7, scale: 2, transformer: decimalTransformer })
  y: number;

  @Column({ name: 'created_at' })
  createdAt: Date;

  @BeforeInsert()
  setCreatedAt() {
    this.createdAt = new Date();
  }
}
