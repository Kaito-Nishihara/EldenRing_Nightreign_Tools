import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { SpawnPoint } from '../entities/spawn-point.entity';
import { ShrinkZone } from '../entities/shrink-zone.entity';
import { SecondShrinkPoint } from '../entities/second-shrink-point.entity';
import { BasePoint } from '../entities/base-point.entity';
import { SpawnPointDto, ShrinkZoneDto, SecondShrinkPointDto, BasePointDto } from './dto/admin.dto';

/** 管理画面用CRUDサービス */
@Injectable()
export class AdminService {
  constructor(
    @InjectRepository(SpawnPoint) private readonly spawnPointRepo: Repository<SpawnPoint>,
    @InjectRepository(ShrinkZone) private readonly shrinkZoneRepo: Repository<ShrinkZone>,
    @InjectRepository(SecondShrinkPoint) private readonly sspRepo: Repository<SecondShrinkPoint>,
    @InjectRepository(BasePoint) private readonly basePointRepo: Repository<BasePoint>,
  ) {}

  // --- SpawnPoint ---
  getAllSpawnPoints() { return this.spawnPointRepo.find(); }

  createSpawnPoint(dto: SpawnPointDto) {
    return this.spawnPointRepo.save(this.spawnPointRepo.create(dto));
  }

  async updateSpawnPoint(id: number, dto: SpawnPointDto) {
    const entity = await this.spawnPointRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`開始地点が見つかりません: id=${id}`);
    return this.spawnPointRepo.save({ ...entity, ...dto });
  }

  async deleteSpawnPoint(id: number) {
    const entity = await this.spawnPointRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`開始地点が見つかりません: id=${id}`);
    await this.spawnPointRepo.delete(id);
  }

  // --- ShrinkZone ---
  getAllShrinkZones() { return this.shrinkZoneRepo.find(); }

  createShrinkZone(dto: ShrinkZoneDto) {
    return this.shrinkZoneRepo.save(this.shrinkZoneRepo.create(dto));
  }

  async updateShrinkZone(id: number, dto: ShrinkZoneDto) {
    const entity = await this.shrinkZoneRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`収縮候補地が見つかりません: id=${id}`);
    return this.shrinkZoneRepo.save({ ...entity, ...dto });
  }

  async deleteShrinkZone(id: number) {
    const entity = await this.shrinkZoneRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`収縮候補地が見つかりません: id=${id}`);
    await this.shrinkZoneRepo.delete(id);
  }

  // --- SecondShrinkPoint ---
  getAllSecondShrinkPoints() { return this.sspRepo.find(); }

  createSecondShrinkPoint(dto: SecondShrinkPointDto) {
    return this.sspRepo.save(this.sspRepo.create(dto));
  }

  async updateSecondShrinkPoint(id: number, dto: SecondShrinkPointDto) {
    const entity = await this.sspRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`第二収縮地点候補が見つかりません: id=${id}`);
    return this.sspRepo.save({ ...entity, ...dto });
  }

  async deleteSecondShrinkPoint(id: number) {
    const entity = await this.sspRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`第二収縮地点候補が見つかりません: id=${id}`);
    await this.sspRepo.delete(id);
  }

  // --- BasePoint ---
  getBasePointsByMapId(mapId: string) { return this.basePointRepo.findBy({ mapId }); }

  createBasePoint(dto: BasePointDto) {
    return this.basePointRepo.save(this.basePointRepo.create(dto));
  }

  async updateBasePoint(id: number, dto: BasePointDto) {
    const entity = await this.basePointRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`拠点候補地が見つかりません: id=${id}`);
    return this.basePointRepo.save({ ...entity, ...dto });
  }

  async deleteBasePoint(id: number) {
    const entity = await this.basePointRepo.findOneBy({ id });
    if (!entity) throw new NotFoundException(`拠点候補地が見つかりません: id=${id}`);
    await this.basePointRepo.delete(id);
  }
}
