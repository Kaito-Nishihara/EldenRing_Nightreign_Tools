import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { AdminController } from './admin.controller';
import { AdminService } from './admin.service';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';
import { SecondShrinkPoint } from '../entities/second-shrink-point.entity';
import { BasePoint } from '../entities/base-point.entity';

@Module({
  imports: [TypeOrmModule.forFeature([SpawnPoint, ShrinkZone, SecondShrinkPoint, BasePoint])],
  controllers: [AdminController],
  providers: [AdminService],
})
export class AdminModule {}
