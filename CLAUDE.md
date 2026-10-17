# CLAUDE.md - プロジェクト設定

## プロジェクト概要

エルデンリング ナイトレインのマップ収縮パターンを実プレイデータで蓄積・検索するWebアプリ。

## ドキュメント

仕様書は `docs/` にあり、`docker compose -f docs-site/compose.yaml up -d` で http://localhost:8000 から読める。
文書の一覧は `docs/index.md` にある。要件定義を正本とする。

| ドキュメント | 場所 |
|------------|------|
| 要件定義書 | `docs/requirements.md` |
| 共通の設計(API、データベース、サービス、画面、実行環境) | `docs/design/common-*.md` |
| 画面の一覧と遷移 | `docs/design/screens.md` |
| 機能の設計 | `docs/design/<機能名>.md` |
| 決定の記録 | `docs/decisions/` |
| 画面の試作 | `docs/mockups/`(画面ごとのページは `docs/screens/`) |

## アーキテクチャ

マイクロサービス構成。フロントエンドは必ず.NET Gatewayを経由する。

```
Next.js (3000) → .NET Gateway (8080) → .NET バックエンド (8081) → PostgreSQL (5432)
```

| サービス | 技術 | 責務 |
|---------|------|------|
| frontend | Next.js 15 + TypeScript + Tailwind CSS 4 + react-konva | UI・マップ描画・ビルドの計算 |
| gateway | ASP.NET Core 10 (Minimal API) + YARP | API Gateway・CORS・管理用の鍵の確認・サービス間キーの付与 |
| backend | ASP.NET Core 10 (Minimal API) + EF Core (Npgsql) + EF Core Migrations | マスタ管理・パターン検索・記録保存・DBマイグレーション |
| db | PostgreSQL 16 | データ永続化 |

バックエンドは 2026-10-17 に NestJS から .NET への作り直しを決めた(`docs/decisions/0001-dotnet-backend.md`)。
`backend/` の NestJS のコードは、段階 0(Issue #50)で .NET のコードに置き換える。
旧版の SQL と初期データはコミット `6529442` の `backend/migrations/` にある。

## コーディング規約

### 共通
- コメントは日本語で書く
- オブジェクト指向で拡張性・再利用性を考えて実装する
- 常に実運用を想定した実装にする（バリデーション・エラーハンドリング必須）
- 実装の前に、対象の機能の仕様書があり、利用者が承認していることを確かめる（`.claude/rules/spec-authoring.md`）
- テストの名前かコメントに、受け入れ条件の識別子（例: `AC-REC-03`）を書く

### フロントエンド（Next.js / TypeScript）
- App Router を使用（Pages Router は使わない）
- コンポーネントは `components/` 配下に機能単位で分類する。共通の部品は `components/ui/`
- APIアクセスは `lib/api.ts` のAxiosクライアントを通じて行う
- カスタムフックは `hooks/` に切り出す
- 色・書体・余白は `globals.css` の `@theme` の名前で指定し、値を直接書かない（`docs/design/common-frontend.md`）
- `any` を使わない

### バックエンド（ASP.NET Core / C#）
- プロジェクト: `backend/NightreignTools.Backend/`、テストは `backend/NightreignTools.Backend.Tests/`（xUnit）
- 名前空間: `NightreignTools.Backend.<ディレクトリ>`
- ディレクトリ構成: `Endpoints / Dtos / Services / Repositories（EfCore/） / Entities / Data / Common`
- エンドポイントは Minimal API で、機能ごとに `Map<機能>Endpoints` の拡張メソッドにまとめる
- サービスとリポジトリはインターフェースと実装に分ける。サービスから EF Core を直接使わない
- エンティティを応答に返さず、`Dtos/` の record に詰め替える
- バリデーションは DTO の注釈と .NET 10 の Minimal API の検証で行う
- 例外は `NotFoundException`（404）、`BusinessRuleException`（400）、`ConflictException`（409）を使い、共通の例外ハンドラで変換する
- `Nullable` を有効にし、警告を誤りとして扱う
- 外部からの直接アクセスを想定しない（Gateway経由のみ。`X-Service-Key` を検証する）
- エラーレスポンス形式: `{ "status", "error", "message", "timestamp" }`

### API Gateway（ASP.NET Core / C#）
- Minimal API スタイルで記述する
- ルーティングは `Program.cs` に集約する
- バックエンドへのプロキシは YARP を使用する

### データベース（バックエンドのみアクセス）
- .NET Gateway はDBに触れない（管理者の確かめは `X-Admin-Key` と環境変数 `ADMIN_KEY` の比較のみ）
- マイグレーションは EF Core Migrations で管理する（直接DDL実行禁止）
- マイグレーション名は英語の PascalCase（例: `AddNightBossToPlayRecord`）。`main` にあるマイグレーションを書き換えない
- スキーマ: `game`（EF Core の既定のスキーマと、マイグレーションの履歴のテーブルの両方に指定する）
- カラム名はスネークケース
- 座標はマップ画像（1024 px 四方）の px で持つ

## 環境変数

`.env` ファイルで管理する（`.env` はGit管理対象外）。

```
JWT_SECRET=        # JWT署名キー
SERVICE_KEY=       # Gateway↔バックエンド サービス間認証キー
DB_USER=           # PostgreSQLユーザー
DB_PASSWORD=       # PostgreSQLパスワード
CORS_ORIGINS=      # Gateway が許すオリジン（省略時 http://localhost:3000）
ADMIN_KEY=         # 管理用の鍵。16文字以上。/api/v1/admin の呼び出しに X-Admin-Key で付ける
```

## 起動方法

```bash
docker compose up --build
```

| URL | 用途 |
|-----|------|
| http://localhost:3000 | フロントエンド |
| http://localhost:8080 | API Gateway |
| http://localhost:8000 | 仕様書のサイト（`docker compose -f docs-site/compose.yaml up -d`） |

バックエンド（8081）はコンテナの間だけに公開する。

ビルドとテストはホストではなくコンテナの中で走らせる（`docs/design/common-runtime.md`）。

```bash
docker compose run --rm backend-sdk dotnet test
docker compose run --rm frontend-dev npm run test
```

`backend-sdk` と `frontend-dev` は段階 0 で compose に足す。足すまでは上のコマンドは動かない。

## Claude Code のコマンドと Skill

| 名前 | 使う時 |
|------|------|
| `/plan` | 機能の実装計画を立てる |
| `/implement` | 計画から実装、テストまでを進める |
| `/test-gen` | 既存の実装にテストを足す |
| `/review-code` | 変更を 5 観点でレビューする |
| `/pr` | PR の本文を作る |
| `add-endpoint` | バックエンドに API を足す、変える |
| `add-migration` | テーブルやマスタデータを足す、変える |

## 未確定事項（実装前に要確認）

`docs/requirements.md` の 12 章にまとめている。主なものは次のとおり。

- 収縮候補地と第二収縮地点の正確なマップ座標（初期データはダミー）
- 開始地点が9ヶ所ですべてか
- マップ画像の権利処理

2026-10-17 に決めたこと（決定の記録 0003、0008〜0010）:

- NestJS 版のデータベースの記録は移さずに捨てる
- 第 2 次収縮は第二収縮地点で記録する
- 記録に夜ボスを持たせ、拠点の種類は持たせない
- 管理者は Gateway が `X-Admin-Key` で確かめる
