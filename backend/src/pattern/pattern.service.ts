import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { PlayRecord } from '../entities/play-record.entity';
import { ShrinkEntry } from '../entities/shrink-entry.entity';

/** パターン検索サービス */
@Injectable()
export class PatternService {
  constructor(
    @InjectRepository(PlayRecord) private readonly playRecordRepo: Repository<PlayRecord>,
  ) {}

  async searchPatterns(bossId: string, spawnPointId: number, jihenId?: string) {
    const qb = this.playRecordRepo
      .createQueryBuilder('pr')
      .leftJoinAndSelect('pr.shrinkEntries', 'se')
      .leftJoinAndSelect('se.zone', 'zone')
      .where('pr.boss_id = :bossId', { bossId })
      .andWhere('pr.spawn_point_id = :spawnPointId', { spawnPointId });

    if (jihenId) {
      qb.andWhere('pr.jihen_id = :jihenId', { jihenId });
    } else {
      qb.andWhere('pr.jihen_id IS NULL');
    }

    const records = await qb.getMany();

    if (!records.length) {
      return { totalRecords: 0, patterns: [] };
    }

    // day+phase+zoneId の組み合わせでグルーピング
    const grouped = new Map<string, PlayRecord[]>();
    for (const record of records) {
      const key = this.buildPatternKey(record.shrinkEntries);
      if (!grouped.has(key)) grouped.set(key, []);
      grouped.get(key).push(record);
    }

    const patterns = [...grouped.values()].map((group, i) => {
      const lastRecordedAt = group.reduce((max, r) =>
        r.createdAt > max ? r.createdAt : max, group[0].createdAt);

      const entries = [...group[0].shrinkEntries]
        .sort((a, b) => a.day - b.day || a.phase - b.phase)
        .map((se) => ({
          day: se.day,
          phase: se.phase,
          zone: {
            id: se.zone.id,
            nameJa: se.zone.nameJa,
            iconPath: se.zone.iconPath,
            centerX: se.zone.centerX,
            centerY: se.zone.centerY,
            radius: se.zone.radius,
          },
        }));

      return { patternIndex: i + 1, recordCount: group.length, lastRecordedAt, entries };
    });

    // 記録件数降順でソートしてインデックス振り直し
    patterns.sort((a, b) => b.recordCount - a.recordCount);
    patterns.forEach((p, i) => { p.patternIndex = i + 1; });

    return { totalRecords: records.length, patterns };
  }

  private buildPatternKey(entries: ShrinkEntry[]): string {
    return [...entries]
      .sort((a, b) => a.day - b.day || a.phase - b.phase)
      .map((e) => `${e.day}-${e.phase}-${e.zone.id}`)
      .join('|');
  }
}
