import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ConfigService } from '@nestjs/config';
import { MigrationRunnerService } from './migration-runner.service';
import { Boss } from '../entities/boss.entity';
import { BossWeakness } from '../entities/boss-weakness.entity';
import { Jihen } from '../entities/jihen.entity';
import { PlayRecord } from '../entities/play-record.entity';
import { ShrinkEntry } from '../entities/shrink-entry.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { BasePoint } from '../entities/base-point.entity';
import { NightBoss } from '../entities/night-boss.entity';
import { SecondShrinkPoint } from '../entities/second-shrink-point.entity';
import { RelicEffectCategory } from '../entities/relic-effect-category.entity';
import { RelicEffect } from '../entities/relic-effect.entity';
import { RelicEffectRank } from '../entities/relic-effect-rank.entity';

@Module({
  imports: [
    TypeOrmModule.forRootAsync({
      inject: [ConfigService],
      useFactory: (config: ConfigService) => ({
        type: 'postgres',
        url: config.get<string>('DATABASE_URL'),
        schema: 'game',
        entities: [
          Boss, BossWeakness, Jihen,
          PlayRecord, ShrinkEntry, ShrinkZone,
          SpawnPoint, BasePoint, NightBoss,
          SecondShrinkPoint, RelicEffectCategory, RelicEffect, RelicEffectRank,
        ],
        synchronize: false,
        logging: config.get('NODE_ENV') === 'development',
      }),
    }),
  ],
  providers: [MigrationRunnerService],
  exports: [MigrationRunnerService],
})
export class DatabaseModule {}
