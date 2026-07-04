import 'reflect-metadata';
import { NestFactory } from '@nestjs/core';
import { ValidationPipe } from '@nestjs/common';
import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';
import { AppModule } from './app.module';
import { HttpExceptionFilter } from './common/filters/http-exception.filter';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  // バリデーションパイプ（class-validatorと連携）
  app.useGlobalPipes(new ValidationPipe({ whitelist: true, transform: true }));

  // 統一エラーレスポンス
  app.useGlobalFilters(new HttpExceptionFilter());

  // Swagger UI
  const doc = new DocumentBuilder()
    .setTitle('Nightreign Tools API')
    .setDescription('エルデンリング ナイトレイン ツール API')
    .setVersion('1.0')
    .build();
  SwaggerModule.setup('swagger-ui.html', app, SwaggerModule.createDocument(app, doc));

  await app.listen(8081);
}

bootstrap();
