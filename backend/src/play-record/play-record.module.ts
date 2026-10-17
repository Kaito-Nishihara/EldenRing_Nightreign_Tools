import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { PlayRecordController } from './play-record.controller';
import { PlayRecordService } from './play-record.service';
import { PlayRecord } from '../entities/play-record.entity';
import { Boss } from '../entities/boss.entity';
import { Jihen } from '../entities/jihen.entity';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';

@Module({
  imports: [TypeOrmModule.forFeature([PlayRecord, Boss, Jihen, SpawnPoint, ShrinkZone])],
  controllers: [PlayRecordController],
  providers: [PlayRecordService],
})
export class PlayRecordModule {}
