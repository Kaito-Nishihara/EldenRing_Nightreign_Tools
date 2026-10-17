import {
  Controller, Get, Post, Put, Delete,
  Body, Param, Query, HttpCode, HttpStatus, UseGuards, ParseIntPipe,
} from '@nestjs/common';
import { ApiTags, ApiOperation } from '@nestjs/swagger';
import { AdminService } from './admin.service';
import { SpawnPointDto, ShrinkZoneDto, SecondShrinkPointDto, BasePointDto } from './dto/admin.dto';
import { ServiceKeyGuard } from '../common/guards/service-key.guard';

/** 管理画面用CRUDコントローラー */
@ApiTags('admin')
@UseGuards(ServiceKeyGuard)
@Controller('api/v1/admin')
export class AdminController {
  constructor(private readonly adminService: AdminService) {}

  // --- SpawnPoint ---
  @Get('spawn-points') getSpawnPoints() { return this.adminService.getAllSpawnPoints(); }

  @Post('spawn-points') @HttpCode(HttpStatus.CREATED)
  createSpawnPoint(@Body() dto: SpawnPointDto) { return this.adminService.createSpawnPoint(dto); }

  @Put('spawn-points/:id')
  updateSpawnPoint(@Param('id', ParseIntPipe) id: number, @Body() dto: SpawnPointDto) {
    return this.adminService.updateSpawnPoint(id, dto);
  }

  @Delete('spawn-points/:id') @HttpCode(HttpStatus.NO_CONTENT)
  deleteSpawnPoint(@Param('id', ParseIntPipe) id: number) { return this.adminService.deleteSpawnPoint(id); }

  // --- ShrinkZone ---
  @Get('shrink-zones') getShrinkZones() { return this.adminService.getAllShrinkZones(); }

  @Post('shrink-zones') @HttpCode(HttpStatus.CREATED)
  createShrinkZone(@Body() dto: ShrinkZoneDto) { return this.adminService.createShrinkZone(dto); }

  @Put('shrink-zones/:id')
  updateShrinkZone(@Param('id', ParseIntPipe) id: number, @Body() dto: ShrinkZoneDto) {
    return this.adminService.updateShrinkZone(id, dto);
  }

  @Delete('shrink-zones/:id') @HttpCode(HttpStatus.NO_CONTENT)
  deleteShrinkZone(@Param('id', ParseIntPipe) id: number) { return this.adminService.deleteShrinkZone(id); }

  // --- SecondShrinkPoint ---
  @Get('second-shrink-points') getSecondShrinkPoints() { return this.adminService.getAllSecondShrinkPoints(); }

  @Post('second-shrink-points') @HttpCode(HttpStatus.CREATED)
  createSecondShrinkPoint(@Body() dto: SecondShrinkPointDto) { return this.adminService.createSecondShrinkPoint(dto); }

  @Put('second-shrink-points/:id')
  updateSecondShrinkPoint(@Param('id', ParseIntPipe) id: number, @Body() dto: SecondShrinkPointDto) {
    return this.adminService.updateSecondShrinkPoint(id, dto);
  }

  @Delete('second-shrink-points/:id') @HttpCode(HttpStatus.NO_CONTENT)
  deleteSecondShrinkPoint(@Param('id', ParseIntPipe) id: number) { return this.adminService.deleteSecondShrinkPoint(id); }

  // --- BasePoint ---
  @Get('base-points') getBasePoints(@Query('mapId') mapId: string = 'normal') {
    return this.adminService.getBasePointsByMapId(mapId);
  }

  @Post('base-points') @HttpCode(HttpStatus.CREATED)
  createBasePoint(@Body() dto: BasePointDto) { return this.adminService.createBasePoint(dto); }

  @Put('base-points/:id')
  updateBasePoint(@Param('id', ParseIntPipe) id: number, @Body() dto: BasePointDto) {
    return this.adminService.updateBasePoint(id, dto);
  }

  @Delete('base-points/:id') @HttpCode(HttpStatus.NO_CONTENT)
  deleteBasePoint(@Param('id', ParseIntPipe) id: number) { return this.adminService.deleteBasePoint(id); }
}
