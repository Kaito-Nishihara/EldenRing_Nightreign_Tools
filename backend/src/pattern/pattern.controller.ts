import { Controller, Get, Query, UseGuards } from '@nestjs/common';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { PatternService } from './pattern.service';
import { ServiceKeyGuard } from '../common/guards/service-key.guard';

/** パターン検索コントローラー */
@ApiTags('pattern')
@UseGuards(ServiceKeyGuard)
@Controller('api/v1')
export class PatternController {
  constructor(private readonly patternService: PatternService) {}

  @ApiOperation({ summary: '収縮パターン検索' })
  @Get('patterns')
  getPatterns(
    @Query('bossId') bossId: string,
    @Query('spawnPointId') spawnPointId: string,
    @Query('jihenId') jihenId?: string,
  ) {
    return this.patternService.searchPatterns(bossId, Number(spawnPointId), jihenId);
  }
}
