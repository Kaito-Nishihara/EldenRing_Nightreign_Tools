import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { PatternController } from './pattern.controller';
import { PatternService } from './pattern.service';
import { PlayRecord } from '../entities/play-record.entity';

@Module({
  imports: [TypeOrmModule.forFeature([PlayRecord])],
  controllers: [PatternController],
  providers: [PatternService],
})
export class PatternModule {}
