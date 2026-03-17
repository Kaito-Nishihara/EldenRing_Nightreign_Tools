# システムアーキテクチャ設計書

**バージョン**: 2.0
**作成日**: 2026-03-16

---

## 1. アーキテクチャ概要

マイクロサービスアーキテクチャを採用する。
フロントエンドは常に.NETのAPI Gatewayを経由してバックエンドにアクセスする。

```
┌──────────────────────────────────────────────────────────┐
│  frontend (Next.js :3000)                                 │
│         │                                                 │
│         ▼                                                 │
│  ┌─────────────────────────────┐                         │
│  │  .NET API Gateway + 認証    │  :8080                  │
│  │  (ASP.NET Core)             │                         │
│  └──────────┬──────────────────┘                         │
│             │  リクエストをプロキシ                        │
│             ▼                                             │
│  ┌─────────────────────────────┐                         │
│  │  Spring Boot                │  :8081                  │
│  │  マスタ管理 + パターン検索   │                         │
│  │  + 収縮記録集計             │                         │
│  └──────────┬──────────────────┘                         │
│             │                                             │
│             ▼                                             │
│  ┌─────────────────────────────┐                         │
│  │  PostgreSQL                 │  :5432                  │
│  └─────────────────────────────┘                         │
└──────────────────────────────────────────────────────────┘
```

---

## 2. 各サービスの責務

### .NET API Gateway（ASP.NET Core）

| 責務 | 詳細 |
|------|------|
| API Gateway | フロントからのリクエストを受け取り、Spring Bootへプロキシ |
| 認証・認可 | JWT発行・検証。Spring Bootへはサービス間トークンで通信 |
| レート制限 | 過剰リクエストの制御 |
| CORSハンドリング | フロントエンドとのCORS設定を一元管理 |
| ルーティング | パスベースで各サービスへルーティング |

**採用する言語特性の理由**
- ASP.NET Core はWebフレームワーク中トップクラスのスループット性能
- `Minimal API` でゲートウェイのルーティングを軽量に記述可能
- JWT認証ライブラリが充実しており認証実装がシンプル

---

### Spring Boot（Java）

| 責務 | 詳細 |
|------|------|
| マスタデータ管理 | ボス・地変・収縮候補地・開始地点のCRUD |
| パターン検索 | 「ボス × 開始地点 × 地変」での収縮パターン検索・グルーピング |
| 収縮記録保存 | プレイ記録・収縮実績の保存 |
| 集計処理 | パターンの出現回数・最終記録日などの統計計算 |

**採用する言語特性の理由**
- `Spring Data JPA` の `@Query` / `Specifications` は複雑な検索条件の実装に強い
- Java `Stream API` のグルーピング・集計処理がパターン集計ロジックにマッチ
- `Spring Batch`（将来）を使った統計バッチ処理への拡張が容易

---

### DBはシングルインスタンス

Spring Bootのみがデータベースに直接アクセスする。
.NETはDBを持たず、業務データはすべてSpring Boot経由で扱う。

```
.NET API Gateway  ──(HTTP)──▶  Spring Boot  ──(JDBC)──▶  PostgreSQL
```

---

## 3. 技術スタック

```
フロントエンド:
  フレームワーク: Next.js 15 (App Router) + TypeScript
  スタイル:      Tailwind CSS + shadcn/ui
  マップ描画:    Konva.js（Canvas APIラッパー）
  PWA:          next-pwa
  HTTP Client:  Axios

API Gateway (.NET):
  フレームワーク: ASP.NET Core 10 (.NET 10 / Minimal API)
  認証:          ASP.NET Core JWT Bearer
  プロキシ:      YARP (Yet Another Reverse Proxy)

バックエンド (Java):
  フレームワーク: Spring Boot 3.5.x (Java 25 LTS)
  API形式:       RESTful API
  ORM:          Spring Data JPA (Hibernate)
  バリデーション: Bean Validation
  API仕様:       SpringDoc OpenAPI (Swagger UI)

データベース:
  RDBMS:           PostgreSQL 16
  マイグレーション: Flyway

インフラ:
  コンテナ: Docker + Docker Compose
```

---

## 4. サービス間通信

```
【フロントエンド → .NET Gateway】
  - 認証: JWT（Bearerトークン）
  - プロトコル: HTTPS

【.NET Gateway → Spring Boot】
  - 認証: サービス間APIキー（X-Service-Key ヘッダー）
  - プロトコル: HTTP（コンテナ内部ネットワーク）
  - .NETはSpring Bootのエンドポイントを外部に公開しない
```

---

## 5. プロジェクト構成

```
EldenRing_Nightreign_Tools/
├── docker-compose.yml
├── docs/
│   ├── requirements.md
│   └── design/
│       ├── architecture.md   # 本ドキュメント
│       ├── database.md
│       └── api.md
├── frontend/                              # Next.js
│   ├── Dockerfile
│   ├── public/
│   │   └── images/
│   │       ├── map/                       # マップ画像
│   │       ├── bosses/                    # ボス画像（サムネイル）
│   │       ├── jihen/                     # 地変画像（サムネイル）
│   │       └── zones/                     # 収縮候補地アイコン
│   └── src/
│       ├── app/
│       │   ├── page.tsx                   # メイン画面（条件選択 + パターン表示）
│       │   └── record/page.tsx            # プレイ記録入力画面
│       ├── components/
│       │   ├── map/
│       │   │   ├── MapCanvas.tsx          # Konva.jsマップキャンバス
│       │   │   ├── ZoneOverlay.tsx        # 収縮候補地オーバーレイ
│       │   │   └── PatternLayer.tsx       # パターン描画レイヤー
│       │   ├── sidebar/
│       │   │   ├── ConditionPanel.tsx     # ボス・地変・開始地点選択
│       │   │   ├── PatternList.tsx        # パターン一覧表示
│       │   │   └── RecordForm.tsx         # 記録入力フォーム
│       │   └── ui/                        # shadcn/ui共通コンポーネント
│       ├── hooks/
│       │   ├── usePatterns.ts             # パターン検索フック
│       │   └── usePlayRecord.ts           # プレイ記録管理フック
│       ├── lib/
│       │   └── api.ts                     # Axiosクライアント
│       └── types/
│           └── index.ts                   # 型定義
├── gateway/                               # ASP.NET Core API Gateway
│   ├── Dockerfile
│   └── src/
│       ├── Program.cs                     # エントリポイント・ルーティング設定
│       ├── Auth/
│       │   ├── JwtService.cs              # JWT発行・検証
│       │   └── AuthController.cs          # 認証エンドポイント
│       ├── Proxy/
│       │   └── YarpConfig.cs              # YARPプロキシ設定
│       └── appsettings.json
└── backend/                               # Spring Boot
    ├── Dockerfile
    └── src/main/
        ├── java/com/nightreign/tools/
        │   ├── NightreignToolsApplication.java
        │   ├── controller/
        │   │   ├── BossController.java
        │   │   ├── JihenController.java
        │   │   ├── PatternController.java
        │   │   └── PlayRecordController.java
        │   ├── service/
        │   │   ├── PatternService.java
        │   │   └── PlayRecordService.java
        │   ├── repository/
        │   │   ├── PlayRecordRepository.java
        │   │   └── ShrinkEntryRepository.java
        │   ├── entity/
        │   │   ├── Boss.java
        │   │   ├── Jihen.java
        │   │   ├── ShrinkZone.java
        │   │   ├── SpawnPoint.java
        │   │   ├── PlayRecord.java
        │   │   └── ShrinkEntry.java
        │   ├── dto/
        │   │   ├── PatternResponse.java
        │   │   └── PlayRecordRequest.java
        │   ├── config/
        │   │   └── ServiceAuthConfig.java  # サービス間APIキー検証
        │   └── security/
        │       └── ServiceKeyFilter.java   # X-Service-Keyフィルター
        └── resources/
            ├── application.yml
            └── db/migration/
                ├── V1__create_master_tables.sql
                ├── V2__create_record_tables.sql
                └── V3__insert_master_data.sql
```

---

## 6. Docker Compose 構成

```yaml
services:
  frontend:
    build: ./frontend
    ports: ["3000:3000"]
    depends_on: [gateway]

  gateway:
    build: ./gateway
    ports: ["8080:8080"]
    depends_on: [backend]
    environment:
      - BACKEND_URL=http://backend:8081
      - JWT_SECRET=${JWT_SECRET}
      - SERVICE_KEY=${SERVICE_KEY}

  backend:
    build: ./backend
    expose:
      - "8081"             # 外部非公開・Gatewayコンテナからのみアクセス可
    depends_on: [db]
    environment:
      - SPRING_DATASOURCE_URL=jdbc:postgresql://db:5432/nightreign
      - SERVICE_KEY=${SERVICE_KEY}

  db:
    image: postgres:16
    ports: ["5432:5432"]
    environment:
      - POSTGRES_DB=nightreign
      - POSTGRES_USER=${DB_USER}
      - POSTGRES_PASSWORD=${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

---

## 7. 画面構成

### 7.1 メイン画面（条件選択 + パターン表示）

```
┌──────────────────────────────────────────────────────┐
│  ヘッダー                                             │
├─────────────────┬────────────────────────────────────┤
│  左パネル        │  マップキャンバス                    │
│                 │                                     │
│  [ボス選択 ▼]   │  ★ 開始地点                         │
│  [地変選択 ▼]   │                                     │
│  [開始地点選択]  │  ● パターン1（赤）1日目収縮先         │
│                 │  ● パターン1（赤）2日目収縮先         │
│  ─────────────  │  ● パターン2（青）1日目収縮先         │
│                 │  ● パターン2（青）2日目収縮先         │
│  過去パターン    │                                     │
│  ┌───────────┐  │                                     │
│  │パターン1  │  │                                     │
│  │3回記録   │  │                                     │
│  └───────────┘  │                                     │
│  ┌───────────┐  │                                     │
│  │パターン2  │  │                                     │
│  │1回記録   │  │                                     │
│  └───────────┘  │                                     │
│  [+ 今回記録する]│                                     │
└─────────────────┴────────────────────────────────────┘
```

### 7.2 記録入力画面（プレイ中）

```
┌──────────────────────────────────────────────────────┐
│  ボス: グラディウス  地変: 火口  開始: 北東             │
│  ⏱ タイムライン: 4:30 第1収縮 → 11:10 第2収縮         │
├─────────────────┬────────────────────────────────────┤
│  入力状況        │  マップキャンバス                    │
│  1日目          │  ★ 開始地点                         │
│  第1収縮: [未]  │  ① ② ③ ④  ←タップで収縮先選択     │
│  第2収縮: [未]  │  ⑤ ⑥ ⑦ ⑧  （12候補地）            │
│  2日目          │  ⑨ ⑩ ⑪ ⑫                         │
│  第1収縮: [未]  │                                     │
│  第2収縮: [未]  │                                     │
│  備考: [      ] │                                     │
│  [保存して完了]  │                                     │
└─────────────────┴────────────────────────────────────┘
```

### 7.3 スマホ表示

- マップを上部に大きく表示
- 下部にボトムシートで操作パネル表示
- 片手タップで全操作完結

---

## 8. マイルストーン（暫定）

| フェーズ | 内容 |
|---------|------|
| Phase 1 | Docker環境構築・DB設計確定・Flywayマイグレーション・マスタデータ投入 |
| Phase 2 | Spring Boot APIの実装（マスタ取得・パターン検索・記録保存） |
| Phase 3 | .NET API Gatewayの実装（YARPプロキシ・JWT認証） |
| Phase 4 | Next.jsフロントエンド実装（マップ表示・条件選択・パターン表示） |
| Phase 5 | 記録入力UI・タイムライン表示 |
| Phase 6 | PWA対応・モバイル最適化 |
