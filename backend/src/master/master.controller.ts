import { Controller, Get, Query, UseGuards } from '@nestjs/common';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { MasterService } from './master.service';
import { ServiceKeyGuard } from '../common/guards/service-key.guard';

/** マスタデータ取得コントローラー */
@ApiTags('master')
@UseGuards(ServiceKeyGuard)
@Controller('api/v1')
export class MasterController {
  constructor(private readonly masterService: MasterService) {}

  @ApiOperation({ summary: 'ボス一覧取得' })
  @Get('bosses')
  getBosses() {
    return this.masterService.getAllBosses();
  }

  @ApiOperation({ summary: '地変一覧取得' })
  @Get('jihen')
  getJihen() {
    return this.masterService.getAllJihen();
  }

  @ApiOperation({ summary: '収縮候補地一覧取得' })
  @Get('shrink-zones')
  getShrinkZones() {
    return this.masterService.getAllShrinkZones();
  }

  @ApiOperation({ summary: '開始地点一覧取得' })
  @Get('spawn-points')
  getSpawnPoints() {
    return this.masterService.getAllSpawnPoints();
  }

  @ApiOperation({ summary: '夜ボス一覧取得' })
  @Get('night-bosses')
  getNightBosses() {
    return this.masterService.getAllNightBosses();
  }

  @ApiOperation({ summary: '拠点候補地一覧取得' })
  @Get('base-points')
  getBasePoints(@Query('mapId') mapId: string = 'normal') {
    return this.masterService.getBasePointsByMapId(mapId);
  }

  @ApiOperation({ summary: '遺物効果一覧取得' })
  @Get('relic-effects')
  getRelicEffects() {
    return this.masterService.getAllRelicEffects();
  }
}
