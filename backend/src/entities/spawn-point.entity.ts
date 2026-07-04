import { Entity, PrimaryGeneratedColumn, Column, BeforeInsert } from 'typeorm';

const decimalTransformer = {
  from: (v: string) => (v !== null && v !== undefined ? Number(v) : null),
  to: (v: number) => v,
};

/** スポーン地点エンティティ */
@Entity({ name: 'spawn_points', schema: 'game' })
export class SpawnPoint {
  @PrimaryGeneratedColumn()
  id: number;

  @Column({ name: 'name_ja', length: 100 })
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
