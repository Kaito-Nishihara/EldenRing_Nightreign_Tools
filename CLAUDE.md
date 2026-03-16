# CLAUDE.md - プロジェクト設定

## プロジェクト概要

エルデンリング ナイトレインのマップ収縮パターンを実プレイデータで蓄積・検索するWebアプリ。

## ドキュメント

| ドキュメント | 場所 |
|------------|------|
| 要件定義書 | `docs/requirements.md` |
| アーキテクチャ設計書 | `docs/design/architecture.md` |
| データベース設計書 | `docs/design/database.md` |
| API設計書 | `docs/design/api.md` |

## アーキテクチャ

マイクロサービス構成。フロントエンドは必ず.NET Gatewayを経由する。

```
Next.js (3000) → .NET Gateway (8080) → Spring Boot (8081) → PostgreSQL (5432)
```

| サービス | 技術 | 責務 |
|---------|------|------|
| frontend | Next.js 15 + TypeScript | UI・マップ描画・PWA |
| gateway | ASP.NET Core 10 (Minimal API) + YARP | API Gateway・JWT認証 |
| backend | Spring Boot 3.5.x (Java 25) + Flyway | マスタ管理・パターン検索・記録集計・DBマイグレーション |
| db | PostgreSQL 16 | データ永続化 |

## コーディング規約

### 共通
- コメントは日本語で書く
- オブジェクト指向で拡張性・再利用性を考えて実装する
- 常に実運用を想定した実装にする（バリデーション・エラーハンドリング必須）

### フロントエンド（Next.js / TypeScript）
- App Router を使用（Pages Router は使わない）
- コンポーネントは `components/` 配下に機能単位で分類する
- APIアクセスは `lib/api.ts` のAxiosクライアントを通じて行う
- カスタムフックは `hooks/` に切り出す

### バックエンド（Spring Boot / Java）
- ベースパッケージ: `com.nightreign.tools`
- パッケージ構成: `controller / service / repository / entity / dto / config`
- DBアクセスは Spring Data JPA を使用する
- バリデーションは Bean Validation（`@Valid`）で行う
- 複雑なクエリは `@Query` または Specifications を使用する
- Spring Bootは外部からの直接アクセスを想定しない（Gateway経由のみ）
- エラーレスポンス形式: `{ "status", "error", "message", "timestamp" }`

### API Gateway（ASP.NET Core / C#）
- Minimal API スタイルで記述する
- ルーティングは `Program.cs` に集約する
- Spring Bootへのプロキシは YARP を使用する

### データベース（Spring Boot のみアクセス）
- .NET はDBに触れない（認証はステートレスなJWTのみ）
- マイグレーションは Flyway で管理する（直接DDL実行禁止）
- ファイル命名: `V{連番}__{説明}.sql`
- スキーマ: `game`（Flyway と Hibernate の両方に指定が必要）
  - `spring.flyway.schemas=game`
  - `spring.jpa.properties.hibernate.default_schema=game`
- カラム名はスネークケース

## 環境変数

`.env` ファイルで管理する（`.env` はGit管理対象外）。

```
JWT_SECRET=        # JWT署名キー
SERVICE_KEY=       # .NET↔Spring Boot サービス間認証キー
DB_USER=           # PostgreSQLユーザー
DB_PASSWORD=       # PostgreSQLパスワード
```

## 起動方法

```bash
docker-compose up --build
```

| URL | 用途 |
|-----|------|
| http://localhost:3000 | フロントエンド |
| http://localhost:8080 | API Gateway |
| http://localhost:8081 | Spring Boot（外部非公開・コンテナ内部のみ・Swagger UI: /swagger-ui.html） |

## 未確定事項（実装前に要確認）

- 収縮候補12ヶ所の正確なマップ座標
- 開始地点の候補数と座標
- マップ画像の権利処理
