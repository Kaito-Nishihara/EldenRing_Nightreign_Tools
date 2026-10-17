# 共通設計: 実行環境

## 1. 目的

手元で一式を起動する手順と、ビルドやテストを走らせる手順を、Docker の中で完結させる。
ホストに .NET の SDK と Node を入れなくても開発できるようにする。

## 2. 範囲

やること:

- compose のサービスと、ポート、環境変数
- ビルドとテストを走らせる道具のコンテナ
- よく使うコマンド
- 仕様書のサイトの起動

やらないこと:

- 本番の配置。置く場所が決まってから決める
- CI。段階 0 の後に決める

## 3. 画面

なし。

## 4. API

なし。

## 5. データ

### 5.1 compose のサービス

| サービス | イメージ | ポート | 役目 |
|---|---|---|---|
| `frontend` | `frontend/Dockerfile` | 3000 を外に出す | 画面 |
| `gateway` | `gateway/Dockerfile` | 8080 を外に出す | 入口 |
| `backend` | `backend/Dockerfile` | 8081 をコンテナの間だけに出す | 業務の API |
| `db` | `postgres:16` | 5432 を外に出す | データベース |
| `backend-sdk` | `mcr.microsoft.com/dotnet/sdk:10.0` | なし | ビルド、テスト、`dotnet ef` を走らせる道具。`tools` のプロファイル |
| `frontend-dev` | `node:20-alpine` | なし | 型検査、lint、テストを走らせる道具。`tools` のプロファイル |

- `backend` は `db` の稼働の確認が通ってから起動する。`gateway` は `backend` の稼働の確認(`/health`)が通ってから起動する。
- `backend-sdk` と `frontend-dev` は、ソースのディレクトリをコンテナに渡し、NuGet の置き場と `node_modules` を名前付きのボリュームに持つ。2 回目からの取得を速くし、ホストの `node_modules` と分けるためである。
- `backend-sdk` は `db` の稼働の確認を待って起動し、テスト用のデータベースへの接続文字列(`ConnectionStrings__Test`)を受け取る。
- `db` の 5432 番を外に出すのは、手元で中身を見るためである。公開する環境では出さない。

### 5.2 環境変数

`.env` に書き、Git に入れない。
`.env.example` に名前だけを書いてコミットする。

| 名前 | 使う所 |
|---|---|
| `JWT_SECRET` | Gateway |
| `ADMIN_KEY` | Gateway。管理用の鍵。16 文字以上 |
| `SERVICE_KEY` | Gateway、バックエンド |
| `DB_USER`、`DB_PASSWORD` | db、バックエンド |
| `CORS_ORIGINS` | Gateway。省いたら `http://localhost:3000` |

### 5.3 よく使うコマンド

```bash
# 一式の起動
docker compose up --build

# バックエンドのビルドとテスト
docker compose run --rm backend-sdk dotnet build
docker compose run --rm backend-sdk dotnet test

# マイグレーションを足す
# dotnet-ef は backend/.config/dotnet-tools.json で版を固定している。コンテナは毎回新しいので、先に復元する
docker compose run --rm backend-sdk sh -c "dotnet tool restore && dotnet ef migrations add <名前> --project NightreignTools.Backend --output-dir Data/Migrations"

# 画面の検査とテスト
docker compose run --rm frontend-dev npx tsc --noEmit
docker compose run --rm frontend-dev npm run lint
docker compose run --rm frontend-dev npm run test

# データベースを作り直す(データが消える)
docker compose down -v

# 仕様書のサイト
docker compose -f docs-site/compose.yaml up -d
```

`tools` のプロファイルのサービスは `docker compose run` で呼ぶと起動するので、`--profile` は要らない。

### 5.4 URL

| URL | 用途 |
|---|---|
| <http://localhost:3000> | 画面 |
| <http://localhost:8080> | Gateway |
| <http://localhost:8000> | 仕様書のサイト |

バックエンドの OpenAPI の文書は、Gateway を通さないと見られない。
開発の環境でだけ `docker compose exec backend` から確かめる。

## 6. サービスの間のやり取り

「API の共通仕様」6 章に書く。

## 7. 受け入れ条件

| 識別子 | 条件 |
|---|---|
| `AC-RT-01` | `.env` を用意して `docker compose up --build` を実行すると、4 つのサービスが起動し、<http://localhost:3000> に画面が出る |
| `AC-RT-02` | ホストから <http://localhost:8081> につなぐと、つながらない |
| `AC-RT-03` | `docker compose run --rm backend-sdk dotnet test` が、ホストに .NET の SDK がなくても走る |
| `AC-RT-04` | `docker compose run --rm frontend-dev npm run test` が、ホストに Node がなくても走る |
| `AC-RT-05` | `.env.example` があり、5.2 の名前がすべて書かれていて、値が空である |

## 8. 未決事項

- **CI**: GitHub Actions で何を走らせるか。段階 0 の後に決める。
- **本番の配置**: 置く場所と、HTTPS の終端。マップ画像の権利(要件定義の 12 章)が決まってから決める。
