import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { DatabaseModule } from './database/database.module';
import { MasterModule } from './master/master.module';
import { PatternModule } from './pattern/pattern.module';
import { PlayRecordModule } from './play-record/play-record.module';
import { AdminModule } from './admin/admin.module';

@Module({
  imports: [
    ConfigModule.forRoot({ isGlobal: true }),
    DatabaseModule,
    MasterModule,
    PatternModule,
    PlayRecordModule,
    AdminModule,
  ],
})
export class AppModule {}
