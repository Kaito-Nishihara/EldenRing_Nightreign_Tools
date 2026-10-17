import { Injectable, NotFoundException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { CreatePlayRecordDto } from './dto/create-play-record.dto';
import { PlayRecord } from '../entities/play-record.entity';
import { ShrinkEntry } from '../entities/shrink-entry.entity';
import { Boss } from '../entities/boss.entity';
import { Jihen } from '../entities/jihen.entity';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';

/** プレイ記録サービス */
@Injectable()
export class PlayRecordService {
  constructor(
    @InjectRepository(PlayRecord) private readonly playRecordRepo: Repository<PlayRecord>,
    @InjectRepository(Boss) private readonly bossRepo: Repository<Boss>,
    @InjectRepository(Jihen) private readonly jihenRepo: Repository<Jihen>,
    @InjectRepository(SpawnPoint) private readonly spawnPointRepo: Repository<SpawnPoint>,
    @InjectRepository(ShrinkZone) private readonly shrinkZoneRepo: Repository<ShrinkZone>,
  ) {}

  async create(dto: CreatePlayRecordDto) {
    const boss = await this.bossRepo.findOneBy({ id: dto.bossId });
    if (!boss) throw new NotFoundException(`bossId '${dto.bossId}' は存在しません`);

    let jihen: Jihen = null;
    if (dto.jihenId) {
      jihen = await this.jihenRepo.findOneBy({ id: dto.jihenId });
      if (!jihen) throw new NotFoundException(`jihenId '${dto.jihenId}' は存在しません`);
    }

    const spawnPoint = await this.spawnPointRepo.findOneBy({ id: dto.spawnPointId });
    if (!spawnPoint) throw new NotFoundException(`spawnPointId '${dto.spawnPointId}' は存在しません`);

    // day+phase 重複チェック
    const dayPhaseKeys = new Set<string>();
    for (const e of dto.entries) {
      const key = `${e.day}-${e.phase}`;
      if (dayPhaseKeys.has(key)) {
        throw new BadRequestException(`day=${e.day}, phase=${e.phase} が重複しています`);
      }
      dayPhaseKeys.add(key);
    }

    const record = this.playRecordRepo.create({ boss, jihen, spawnPoint, note: dto.note });
    record.shrinkEntries = [];

    for (const entryDto of dto.entries) {
      const zone = await this.shrinkZoneRepo.findOneBy({ id: entryDto.zoneId });
      if (!zone) throw new NotFoundException(`zoneId '${entryDto.zoneId}' は存在しません`);

      const entry = new ShrinkEntry();
      entry.day = entryDto.day;
      entry.phase = entryDto.phase;
      entry.zone = zone;
      record.shrinkEntries.push(entry);
    }

    const saved = await this.playRecordRepo.save(record);
    return { id: saved.id, createdAt: saved.createdAt };
  }
}
