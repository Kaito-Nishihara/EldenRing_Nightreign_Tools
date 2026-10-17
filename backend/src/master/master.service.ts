import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { Boss } from '../entities/boss.entity';
import { Jihen } from '../entities/jihen.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { NightBoss } from '../entities/night-boss.entity';
import { BasePoint } from '../entities/base-point.entity';
import { RelicEffect } from '../entities/relic-effect.entity';

/** マスタデータ取得サービス */
@Injectable()
export class MasterService {
  constructor(
    @InjectRepository(Boss) private readonly bossRepo: Repository<Boss>,
    @InjectRepository(Jihen) private readonly jihenRepo: Repository<Jihen>,
    @InjectRepository(ShrinkZone) private readonly shrinkZoneRepo: Repository<ShrinkZone>,
    @InjectRepository(SpawnPoint) private readonly spawnPointRepo: Repository<SpawnPoint>,
    @InjectRepository(NightBoss) private readonly nightBossRepo: Repository<NightBoss>,
    @InjectRepository(BasePoint) private readonly basePointRepo: Repository<BasePoint>,
    @InjectRepository(RelicEffect) private readonly relicEffectRepo: Repository<RelicEffect>,
  ) {}

  getAllBosses() {
    return this.bossRepo.find();
  }

  getAllJihen() {
    return this.jihenRepo.find();
  }

  getAllShrinkZones() {
    return this.shrinkZoneRepo.find();
  }

  getAllSpawnPoints() {
    return this.spawnPointRepo.find();
  }

  getAllNightBosses() {
    return this.nightBossRepo.find();
  }

  getBasePointsByMapId(mapId: string) {
    return this.basePointRepo.findBy({ mapId });
  }

  getAllRelicEffects() {
    return this.relicEffectRepo
      .createQueryBuilder('e')
      .leftJoinAndSelect('e.category', 'c')
      .leftJoinAndSelect('e.ranks', 'r')
      .orderBy('c.sortOrder', 'ASC')
      .addOrderBy('e.isDeep', 'ASC')
      .addOrderBy('r.rank', 'ASC')
      .getMany();
  }
}
