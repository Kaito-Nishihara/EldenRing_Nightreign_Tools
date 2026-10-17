import { IsString, IsNumber, IsOptional } from 'class-validator';
import { ApiProperty } from '@nestjs/swagger';

export class SpawnPointDto {
  @ApiProperty() @IsString() nameJa: string;
  @ApiProperty() @IsNumber() x: number;
  @ApiProperty() @IsNumber() y: number;
}

export class ShrinkZoneDto {
  @ApiProperty() @IsString() nameJa: string;
  @ApiProperty({ required: false }) @IsOptional() @IsString() iconPath?: string;
  @ApiProperty() @IsNumber() centerX: number;
  @ApiProperty() @IsNumber() centerY: number;
  @ApiProperty() @IsNumber() radius: number;
}

export class SecondShrinkPointDto {
  @ApiProperty() @IsString() nameJa: string;
  @ApiProperty() @IsNumber() x: number;
  @ApiProperty() @IsNumber() y: number;
}

export class BasePointDto {
  @ApiProperty() @IsString() mapId: string;
  @ApiProperty({ required: false }) @IsOptional() @IsString() nameJa?: string;
  @ApiProperty() @IsNumber() x: number;
  @ApiProperty() @IsNumber() y: number;
}
