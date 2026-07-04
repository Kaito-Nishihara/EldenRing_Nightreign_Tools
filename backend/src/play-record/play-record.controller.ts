import { Controller, Post, Body, HttpCode, HttpStatus, UseGuards } from '@nestjs/common';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { PlayRecordService } from './play-record.service';
import { CreatePlayRecordDto } from './dto/create-play-record.dto';
import { ServiceKeyGuard } from '../common/guards/service-key.guard';

/** プレイ記録コントローラー */
@ApiTags('play-record')
@UseGuards(ServiceKeyGuard)
@Controller('api/v1/play-records')
export class PlayRecordController {
  constructor(private readonly playRecordService: PlayRecordService) {}

  @ApiOperation({ summary: 'プレイ記録作成' })
  @Post()
  @HttpCode(HttpStatus.CREATED)
  create(@Body() dto: CreatePlayRecordDto) {
    return this.playRecordService.create(dto);
  }
}
