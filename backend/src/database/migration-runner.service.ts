import { Injectable, Logger, OnModuleInit } from '@nestjs/common';
import { InjectDataSource } from '@nestjs/typeorm';
import { DataSource } from 'typeorm';
import * as fs from 'fs';
import * as path from 'path';

/**
 * カスタムマイグレーションランナー
 * migrations/ 配下の V{n}__*.sql を順に実行し、
 * game.flyway_schema_history で適用済みを判定する（Flyway互換）
 */
@Injectable()
export class MigrationRunnerService implements OnModuleInit {
  private readonly logger = new Logger(MigrationRunnerService.name);

  constructor(@InjectDataSource() private readonly dataSource: DataSource) {}

  async onModuleInit() {
    await this.runMigrations();
  }

  private async runMigrations() {
    const queryRunner = this.dataSource.createQueryRunner();
    await queryRunner.connect();

    try {
      // game スキーマが存在しない場合は作成
      await queryRunner.query(`CREATE SCHEMA IF NOT EXISTS game`);

      // flyway_schema_history が存在しない場合は作成
      await queryRunner.query(`
        CREATE TABLE IF NOT EXISTS game.flyway_schema_history (
          installed_rank  SERIAL PRIMARY KEY,
          version         VARCHAR(50),
          description     VARCHAR(200) NOT NULL,
          type            VARCHAR(20)  NOT NULL,
          script          VARCHAR(1000) NOT NULL,
          checksum        INTEGER,
          installed_by    VARCHAR(100) NOT NULL,
          installed_on    TIMESTAMP NOT NULL DEFAULT now(),
          execution_time  INTEGER NOT NULL,
          success         BOOLEAN NOT NULL
        )
      `);

      // 適用済みバージョン一覧を取得
      const applied: { version: string }[] = await queryRunner.query(
        `SELECT version FROM game.flyway_schema_history WHERE success = true`,
      );
      const appliedSet = new Set(applied.map((r) => r.version));

      // migrations/ ディレクトリを探索（Docker内: /app/migrations, 開発時: ./migrations）
      const candidates = [
        path.join(process.cwd(), 'migrations'),
        path.join(__dirname, '..', '..', 'migrations'),
      ];
      const migrationsDir = candidates.find((p) => fs.existsSync(p));
      if (!migrationsDir) {
        this.logger.warn('migrations ディレクトリが見つかりません。スキップします。');
        return;
      }

      const files = fs
        .readdirSync(migrationsDir)
        .filter((f) => /^V\d+__.*\.sql$/.test(f))
        .sort((a, b) => {
          const va = parseInt(a.match(/^V(\d+)/)[1], 10);
          const vb = parseInt(b.match(/^V(\d+)/)[1], 10);
          return va - vb;
        });

      for (const file of files) {
        const version = file.match(/^V(\d+)/)[1];
        if (appliedSet.has(version)) {
          this.logger.debug(`スキップ (適用済み): ${file}`);
          continue;
        }

        const sql = fs.readFileSync(path.join(migrationsDir, file), 'utf-8');
        this.logger.log(`実行中: ${file}`);
        const start = Date.now();

        await queryRunner.query(sql);

        await queryRunner.query(
          `INSERT INTO game.flyway_schema_history
             (version, description, type, script, checksum, installed_by, execution_time, success)
           VALUES ($1, $2, 'SQL', $3, 0, current_user, $4, true)`,
          [version, file.replace(/^V\d+__/, '').replace(/\.sql$/, '').replace(/_/g, ' '), file, Date.now() - start],
        );
        this.logger.log(`完了: ${file}`);
      }

      this.logger.log('マイグレーション完了');
    } catch (err) {
      this.logger.error('マイグレーション失敗', err);
      throw err;
    } finally {
      await queryRunner.release();
    }
  }
}
