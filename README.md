# Elden Ring Nightreign Tools

エルデンリング ナイトレインのマップ収縮パターンを実プレイデータで蓄積・検索するWebアプリケーション。

## 概要

プレイ中にマップ収縮（夜の雨）の実績データを入力し、「ボス × 開始地点 × 地変」の組み合わせで過去のパターンを検索・表示する。
2回目以降の同条件プレイで過去パターンを参照することで、立ち回りの改善を支援する。

## システム構成

```
Next.js (3000) → .NET Gateway (8080) → Spring Boot (8081) → PostgreSQL (5432)
```

| サービス | 技術 | 責務 |
|---------|------|------|
| frontend | Next.js 15 + TypeScript | UI・マップ描画・PWA |
| gateway | ASP.NET Core 9 + YARP | API Gateway・JWT認証 |
| backend | Spring Boot 3.x (Java 21) + Flyway | マスタ管理・パターン検索・記録集計 |
| db | PostgreSQL 16 | データ永続化 |

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
docker-compose up --build
```

| URL | 用途 |
|-----|------|
| http://localhost:3000 | フロントエンド |
| http://localhost:8080 | API Gateway |
| http://localhost:8081/swagger-ui.html | Spring Boot Swagger UI（開発用） |

## ドキュメント

| ドキュメント | 場所 |
|------------|------|
| 要件定義書 | [`docs/requirements.md`](docs/requirements.md) |
| アーキテクチャ設計書 | [`docs/design/architecture.md`](docs/design/architecture.md) |
| データベース設計書 | [`docs/design/database.md`](docs/design/database.md) |
| API設計書 | [`docs/design/api.md`](docs/design/api.md) |

## 主な機能

- **パターン表示** — 同条件（ボス × 開始地点 × 地変）の過去収縮パターンをマップ上に色分け表示
- **プレイ記録** — プレイ中にマップをタップして収縮先をリアルタイム入力
- **統計表示** — 同条件で複数パターンがある場合、出現回数を表示
- **PWA対応** — プレイ中のネット不安定環境でも動作

## 開発状況

| フェーズ | 内容 | 状態 |
|---------|------|------|
| Phase 1 | Docker環境構築・DBマイグレーション | 未着手 |
| Phase 2 | Spring Boot API実装 | 未着手 |
| Phase 3 | .NET API Gateway実装 | 未着手 |
| Phase 4 | Next.jsフロントエンド実装 | 未着手 |
| Phase 5 | 記録入力UI・タイムライン | 未着手 |
| Phase 6 | PWA対応・モバイル最適化 | 未着手 |
