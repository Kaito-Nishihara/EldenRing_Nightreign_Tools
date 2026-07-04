import {
  ExceptionFilter,
  Catch,
  ArgumentsHost,
  HttpException,
  HttpStatus,
  Logger,
} from '@nestjs/common';
import { Response } from 'express';

/** 統一エラーレスポンスフィルター */
@Catch()
export class HttpExceptionFilter implements ExceptionFilter {
  private readonly logger = new Logger(HttpExceptionFilter.name);

  catch(exception: unknown, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse<Response>();

    let status = HttpStatus.INTERNAL_SERVER_ERROR;
    let message = '内部サーバーエラーが発生しました';

    if (exception instanceof HttpException) {
      status = exception.getStatus();
      const res = exception.getResponse();
      message = typeof res === 'string' ? res : (res as any).message ?? message;
    } else if (exception instanceof Error) {
      this.logger.error(exception.message, exception.stack);
      // EntityNotFoundError はクライアントエラーとして扱う
      if (exception.constructor.name === 'EntityNotFoundError' || exception.message.includes('存在しません')) {
        status = HttpStatus.NOT_FOUND;
        message = exception.message;
      } else if (exception.message.includes('重複')) {
        status = HttpStatus.BAD_REQUEST;
        message = exception.message;
      }
    }

    response.status(status).json({
      status,
      error: HttpStatus[status],
      message: Array.isArray(message) ? message.join(', ') : message,
      timestamp: new Date().toISOString(),
    });
  }
}
