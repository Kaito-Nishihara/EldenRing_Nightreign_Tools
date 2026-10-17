import { CanActivate, ExecutionContext, Injectable, UnauthorizedException } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';

/** Gateway からのサービス間認証キーを検証するガード */
@Injectable()
export class ServiceKeyGuard implements CanActivate {
  constructor(private readonly config: ConfigService) {}

  canActivate(ctx: ExecutionContext): boolean {
    const req = ctx.switchToHttp().getRequest();
    const key = req.headers['x-service-key'];
    const expected = this.config.get<string>('SERVICE_KEY');
    if (!key || key !== expected) {
      throw new UnauthorizedException('無効なサービスキーです');
    }
    return true;
  }
}
