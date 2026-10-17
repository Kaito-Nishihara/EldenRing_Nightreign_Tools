# 収縮パターン帳 仕様書

エルデンリング ナイトレインの収縮(夜の雨)の場所を記録し、同じ条件の過去のパターンを見るアプリの仕様書である。
このサイトは、リポジトリの `docs/` にある Markdown をそのまま表示している。

## 文書の一覧

| 文書 | 内容 |
|---|---|
| [要件定義](requirements.md) | 何を作るか、何を作らないか。ゲームの前提、機能の要点、技術構成、データモデル、API、段階、未決事項 |
| [アーキテクチャ図](architecture.html) | サービスの構成と通信の向き、記録を保存するときの流れを絵にしたもの |
| [共通設計: API の共通仕様](design/common-api.md) | パス、JSON、座標と日時、エラーの形と状態、入力の検証、Gateway とバックエンドの間の決まり |
| [共通設計: データベースの共通仕様](design/common-database.md) | スキーマと名前、型、行の消し方、EF Core Migrations、マスタの初期データ、テーブルの関係 |
| [共通設計: サービスの共通構成](design/common-service.md) | 3 つのサービスの分け方、クリーンアーキテクチャの層、サービスの間の呼び出し、例外の変換、設定と秘密の値、ログ、テストの方針 |
| [共通設計: 画面の共通仕様](design/common-frontend.md) | エルデンリングに寄せた色、書体、飾り枠、部品、マップの描き方、状態の表し方、API のエラー、アクセシビリティ |
| [共通設計: 実行環境](design/common-runtime.md) | compose のサービス、道具のコンテナ、環境変数、よく使うコマンド |
| [画面設計: 画面の一覧と遷移](design/screens.md) | 画面の一覧とパス、URL に残すもの、遷移、各画面の骨組み |
| [画面一覧](screens/index.md) | 画面ごとのページ。見た目の試作と、対応する設計へのリンク |
| [機能設計: パターン検索](design/pattern-search.md) | 条件の選び方、パターンのまとめ方と並び順、カードとマップ、検索の API |
| [機能設計: プレイ記録](design/play-record.md) | スロットと夜ボスの入力、下書き、保存の API と検証、記録のテーブル |
| [機能設計: マスタの管理](design/master-admin.md) | 管理用の鍵、座標の登録と修正と非表示、マスタの取得の API、参照を解決する内部の API |
| [機能設計: ビルドシミュレーター](design/build-simulator.md) | 効果の枠、合計の出し方と表示の桁、効果のマスタの API |
| [決定の記録 0001](decisions/0001-dotnet-backend.md) | バックエンドを ASP.NET Core と C# で作り直す |
| [決定の記録 0002](decisions/0002-keep-gateway-separate.md) | Gateway とバックエンドを 1 つにまとめない |
| [決定の記録 0003](decisions/0003-ef-core-migrations.md) | テーブルの変更を EF Core Migrations で管理する |
| [決定の記録 0004](decisions/0004-pixel-coordinates.md) | マップの座標をマップ画像の px で持つ |
| [決定の記録 0005](decisions/0005-keep-play-records.md) | プレイの記録を消さない |
| [決定の記録 0006](decisions/0006-build-calc-in-browser.md) | ビルドシミュレーターの計算をブラウザで行う |
| [決定の記録 0007](decisions/0007-ef-core.md) | データベースのアクセスに EF Core を使う |
| [決定の記録 0008](decisions/0008-second-shrink-by-point.md) | 第 2 次収縮を第二収縮地点で記録する |
| [決定の記録 0009](decisions/0009-admin-key.md) | 管理者を Gateway で管理用の鍵により確かめる |
| [決定の記録 0010](decisions/0010-night-boss-in-record.md) | 記録に夜ボスを持たせ、拠点の種類を持たせない |
| [決定の記録 0011](decisions/0011-clean-architecture.md) | サービスの中をクリーンアーキテクチャで分ける |
| [決定の記録 0012](decisions/0012-split-services-by-feature.md) | バックエンドを機能ごとに 3 つのサービスに分ける |
| [決定の記録 0013](decisions/0013-hide-masters-instead-of-delete.md) | マスタを消さずに非表示にする |

仕様に迷ったときは、要件定義を正本とする。
図や試作と要件定義が食い違っていたら、要件定義が正しい。

## 文書を足すとき

1. `docs/` の下に Markdown のファイルを作る。
2. `docs-site/mkdocs.yml` の `nav` に 1 行足す。
3. この一覧に 1 行足す。

設計の文書は `docs/design/`、決めたことの記録は `docs/decisions/`、画面の試作は `docs/mockups/` に置く。
書き方の決まりは `.claude/rules/spec-authoring.md` と `.claude/rules/writing-style.md` にある。

## 見るには

```bash
docker compose -f docs-site/compose.yaml up -d
```

ブラウザで <http://localhost:8000> を開く。
Markdown を保存すると、表示が自動で更新される。
