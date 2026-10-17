import { IsString, IsInt, IsOptional, IsArray, ValidateNested, Min, Max } from 'class-validator';
import { Type } from 'class-transformer';
import { ApiProperty } from '@nestjs/swagger';

export class ShrinkEntryDto {
  @ApiProperty({ example: 1 })
  @IsInt() @Min(1) @Max(2)
  day: number;

  @ApiProperty({ example: 1 })
  @IsInt() @Min(1) @Max(2)
  phase: number;

  @ApiProperty({ example: 3 })
  @IsInt()
  zoneId: number;
}

export class CreatePlayRecordDto {
  @ApiProperty({ example: 'gladius' })
  @IsString()
  bossId: string;

  @ApiProperty({ required: false })
  @IsOptional() @IsString()
  jihenId?: string;

  @ApiProperty({ example: 1 })
  @IsInt()
  spawnPointId: number;

  @ApiProperty({ required: false })
  @IsOptional() @IsString()
  note?: string;

  @ApiProperty({ type: [ShrinkEntryDto] })
  @IsArray() @ValidateNested({ each: true }) @Type(() => ShrinkEntryDto)
  entries: ShrinkEntryDto[];
}
