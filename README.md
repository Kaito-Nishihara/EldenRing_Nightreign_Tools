# Elden Ring Nightreign Tools

> ⚠️ **本プロジェクトは現在開発中です。** 動作する成果物はまだありません。

エルデンリング ナイトレインのマップ収縮パターンを実プレイデータで蓄積・検索するWebアプリケーション。

## 概要

プレイ中にマップ収縮（夜の雨）の実績データを入力し、「ボス × 開始地点 × 地変」の組み合わせで過去のパターンを検索・表示する。
2回目以降の同条件プレイで過去パターンを参照することで、立ち回りの改善を支援する。

## システム構成

```
Next.js (3000) → .NET Gateway (8080) → 3 つのサービス(master、record、relic) → PostgreSQL (5432)
```

| サービス | 技術 | 責務 |
|---------|------|------|
| frontend | Next.js 15 + TypeScript | UI・マップ描画 |
| gateway | ASP.NET Core 10 + YARP | API Gateway・管理用の鍵の確認・サービス間キーの付与 |
| master-service (8081) | ASP.NET Core 10 + EF Core | マスタの取得と管理 |
| record-service (8082) | ASP.NET Core 10 + EF Core | プレイ記録とパターン検索 |
| relic-service (8083) | ASP.NET Core 10 + EF Core | ビルドシミュレーターの遺物の効果 |
| db | PostgreSQL 16 | データ永続化 |

バックエンドは NestJS から .NET へ作り直している途中である。
`backend/` の NestJS のコードは、段階 0(#50)で .NET の 3 つのサービスに置き換える。
どのサービスも、中はクリーンアーキテクチャ(Domain、Application、Infrastructure、Api)で書く。
作り直しの理由は [`docs/decisions/0001-dotnet-backend.md`](docs/decisions/0001-dotnet-backend.md) にある。

## 起動方法

### 前提条件

- Docker
- Docker Compose

### 環境変数の設定

```bash
cp .env.example .env
# .env を編集して各値を設定
```

### 起動

```bash
docker compose up --build
```

| URL | 用途 |
|-----|------|
| http://localhost:3000 | フロントエンド |
| http://localhost:8080 | API Gateway |

## ドキュメント

仕様書は `docs/` にある。
公開しているサイトは https://kaito-nishihara.github.io/EldenRing_Nightreign_Tools/ にある。`main` に入った変更が反映される。

手元では、次のコマンドでサイトを起動できる。

```bash
docker compose -f docs-site/compose.yaml up -d
# http://localhost:8000
```

| ドキュメント | 場所 |
|------------|------|
| 文書の一覧 | [`docs/index.md`](docs/index.md) |
| 要件定義書 | [`docs/requirements.md`](docs/requirements.md) |
| 画面の試作 | [`docs/screens/index.md`](docs/screens/index.md) |

## 主な機能

- **パターン表示** — 同条件（ボス × 開始地点 × 地変）の過去収縮パターンをマップ上に色分け表示
- **プレイ記録** — プレイ中にマップをタップして収縮先をリアルタイム入力
- **統計表示** — 同条件で複数パターンがある場合、出現回数を表示
- **PWA対応** — プレイ中のネット不安定環境でも動作

## 開発状況

段階と終わりの条件は、[要件定義の 11 章](docs/requirements.md) にある。

| 段階 | 内容 | 状態 |
|---------|------|------|
| 0 | 共通の部品と master-service の土台(#50) | 着手 |
| 1 | record-service: パターン検索とプレイ記録(#51) | 未着手 |
| 2 | マスタの管理と権限(#52) | 未着手 |
| 3 | 画面の見た目の作り直し(#53) | 試作まで |
| 4 | relic-service: ビルドシミュレーターの API(#54) | 未着手 |
| 5 | 収縮のタイムライン、オフラインでの利用 | 未着手 |
