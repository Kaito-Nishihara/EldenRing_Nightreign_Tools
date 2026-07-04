import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { MasterController } from './master.controller';
import { MasterService } from './master.service';
import { Boss } from '../entities/boss.entity';
import { Jihen } from '../entities/jihen.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { NightBoss } from '../entities/night-boss.entity';
import { BasePoint } from '../entities/base-point.entity';
import { RelicEffect } from '../entities/relic-effect.entity';

@Module({
  imports: [TypeOrmModule.forFeature([Boss, Jihen, ShrinkZone, SpawnPoint, NightBoss, BasePoint, RelicEffect])],
  controllers: [MasterController],
  providers: [MasterService],
})
export class MasterModule {}
