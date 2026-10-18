# backend(ASP.NET Core の 3 つのサービス)

ASP.NET Core 10(Minimal API)、C#、EF Core(Npgsql)、EF Core Migrations、PostgreSQL 16。
サービスは 3 つで、それぞれが自分のデータベースを持つ。

| サービス | ポート | 受け持つもの | データベース |
|---|---|---|---|
| `master-service` | 8081 | マスタの取得と管理 | `nightreign_master` |
| `record-service` | 8082 | プレイ記録とパターン検索。`master-service` の内部の API を呼ぶ | `nightreign_record` |
| `relic-service` | 8083 | ビルドシミュレーターの遺物の効果 | `nightreign_relic` |

詳しくは [共通設計: サービスの共通構成](../docs/design/common-service.md) にある。

## ディレクトリの役割

```
backend/
├── NightreignTools.slnx
├── Directory.Build.props          # 全プロジェクトの共通の設定
├── Directory.Packages.props       # パッケージの版。ここだけで決める
├── shared/
│   ├── NightreignTools.SharedKernel/     # 業務の例外。どこにも頼らない
│   ├── NightreignTools.ServiceDefaults/  # エラーの形、例外の変換、サービス間キー、/health、接続文字列の検証、起動のときのマイグレーション
│   └── NightreignTools.Shared.Tests/
├── <master|record|relic>/
│   ├── NightreignTools.<Service>.Domain/          # エンティティと業務の規則
│   ├── NightreignTools.<Service>.Application/     # アプリケーションサービス、ポート、DTO
│   ├── NightreignTools.<Service>.Infrastructure/  # DbContext、構成、マイグレーション、初期データ、リポジトリ、ほかのサービスのクライアント
│   ├── NightreignTools.<Service>.Api/             # Program.cs とエンドポイント
│   ├── NightreignTools.<Service>.Tests/           # Unit/、Integration/、Architecture/
│   └── Dockerfile                                 # ビルドの文脈は backend/
└── .config/dotnet-tools.json      # dotnet-ef の版を固定する
```

## 層の決まり

- **頼る向きは、Api → Application → Domain、Infrastructure → Application の一方だけ。** Domain と Application は EF Core と ASP.NET Core を参照しない。
- **業務の規則はエンティティ(Domain)に書く。** エンティティの setter は private にし、作るときと変えるときに規則を確かめる。
- **アプリケーションサービスとポート(リポジトリ、ほかのサービスの呼び出し)は Application に書く。** インターフェースと実装に分ける。
- **EF Core、マイグレーション、初期データ、ほかのサービスを呼ぶ `HttpClient` は Infrastructure に置く。**
- **Api は登録の組み立てとエンドポイントだけを書く。** `Program.cs` は `AddServiceDefaults`、`AddValidation`、`AddApplication`、`AddInfrastructure` を呼ぶだけにする。
- **`AddValidation` は各サービスの `Program.cs` で呼ぶ。** ソースジェネレーターがこの呼び出しに検証の情報を差し込むので、ServiceDefaults の中で呼ぶと検証が働かない(呼んでも誤りにならず、黙って素通りする)。
- **エンティティを応答に返さない。** Application の DTO(record)に詰め替える。
- 登録は各層の `DependencyInjection.cs` に足す。
- エラーの形、例外の変換、サービス間キーは `shared/` に置く。サービスの中に写しを作らない。`shared/` に業務の規則を置かない。

## API の決まり

- **エンドポイントは Minimal API で、機能ごとに `Map<機能>Endpoints` の拡張メソッドにまとめる。** `/api/v1` のグループに足す。
- **外に出す API は `/api/v1`、サービスの間だけで使う API は `/internal/v1` の下に置く。** `/internal/v1` は Gateway が振り分けない。
- **サービス間キー(`X-Service-Key`)は ServiceDefaults のミドルウェアが `/health` 以外のすべてのパスで確かめる。** エンドポイントごとに付けない。
- **入力は DTO で受け、DTO の注釈と .NET 10 の Minimal API の検証で確かめる。** 文字列には長さの上限を付ける。
- **エラーは SharedKernel の例外を投げて伝える。** `NotFoundException`(404)、`BusinessRuleException`(400)、`ConflictException`(409)、`ServiceUnavailableException`(503)。応答を自分で組み立てない。
- エラーの応答の形は `{ "status", "error", "message", "timestamp" }` とする。
- すべての非同期のメソッドは `CancellationToken` を受け取り、下の層へ渡す。読み取りだけのクエリには `AsNoTracking()` を付ける。
- `Nullable` を有効にし、警告を誤りとして扱う。

## サービスの間の決まり

- **ほかのサービスのデータベースに触れない。** 接続の設定も持たない。
- **呼ぶ向きは `record-service` から `master-service` への一方向だけ。** 逆向きを足すときは、決定の記録を書く。
- **ほかのサービスを呼ぶときは、期限と再試行を付ける。** 応答がなければ `ServiceUnavailableException`(503)にし、何も保存しない。
- **ほかのサービスが持つ情報を、自分のデータベースに写さない。** 識別子だけを持つ。

## データベースの決まり

- **テーブルは EF Core Migrations だけで変える。** マイグレーションの名前は英語の PascalCase(例: `AddNightBossToPlayRecord`)。`main` にあるマイグレーションを書き換えない。
- スキーマは `game` の 1 つ。EF Core の既定のスキーマと、マイグレーションの履歴のテーブル(`game.__ef_migrations_history`)の両方に指定する。
- テーブルと列の名前はスネークケース。`EFCore.NamingConventions` で変換する。
- すべてのテーブルに `created_at`(データベースの既定値)を持たせる。
- 座標と半径は `numeric(7,2)` で、0 以上 1024 以下の検査の制約を付ける。
- 初期データは Infrastructure の `Persistence/Seed/` に匿名の型で持ち、`HasData` で渡す。整数の識別子は 1000 以下にし、自動の採番は 1001 から始める。
- マスタの行は消さず、`hidden_at` で非表示にする。
- マイグレーションを足す手順は `add-migration` の Skill にある。
- テーブルと列の説明は、Domain のエンティティの `<summary>` に書く。エンティティかテーブルの設定を変えたら、同じ PR でテーブルの定義書(`docs/database/`)を作り直す。CI が作り直して比べる(データベースの共通仕様 5.8)。

## テスト

- サービスごとに 1 つのテストのプロジェクトを持つ。`Unit/` はエンティティとアプリケーションサービス、`Integration/` は API とデータベース、`Architecture/` は層の参照を確かめる。
- 結合のテストは、本物の PostgreSQL の `nightreign_<サービス>_test` に対して走らせる。EF Core の InMemory を使わない。
- `record-service` の結合のテストでは、`master-service` を呼ぶポートを偽物に差し替える。
- テストは 1 つのデータベースを共有するので、並べて走らせない。
- テストの名前かコメントに、受け入れ条件の識別子を書く。

```bash
docker compose run --rm backend-sdk dotnet test NightreignTools.slnx
# テーブルの定義書を作り直す
MSYS_NO_PATHCONV=1 docker compose run --rm backend-sdk dotnet run --project tools/NightreignTools.SchemaDoc -- /src/docs/database
# マイグレーションを足す(dotnet-ef は毎回の新しいコンテナで復元する)
docker compose run --rm backend-sdk sh -c "dotnet tool restore && dotnet ef migrations add <名前> --project <サービス>/NightreignTools.<サービス>.Infrastructure --output-dir Persistence/Migrations"
```
