# ナイトレイン 収縮パターン帳

エルデンリング ナイトレインのマップ収縮(夜の雨)が起きた場所を実プレイで記録し、同じ条件(夜の王、開始地点、地変)の過去のパターンを見るアプリ。
あわせて、遺物の効果を重ねたときの合計を出すビルドシミュレーターを持つ。

バックエンドは機能ごとに 3 つのサービスに分け、どのサービスも中はクリーンアーキテクチャにする。
この構成は利用者が決めたもので(決定の記録 [0011](docs/decisions/0011-clean-architecture.md)、[0012](docs/decisions/0012-split-services-by-feature.md))、サービスをまとめるなど構成を簡単にする提案はしない。

要件定義は [docs/requirements.md](docs/requirements.md) が正本である。
仕様に迷ったら、推測せずにここを読む。書かれていなければ利用者に聞く。
文書の一覧は [docs/index.md](docs/index.md)、全体の図は [docs/architecture.html](docs/architecture.html) にある。構成を変えたら図も直す。

## 構成

```
Next.js (3000) → .NET Gateway (8080) → 3 つのサービス(master、record、relic) → PostgreSQL (5432)
```

| ディレクトリ | 中身 | 決まり |
|---|---|---|
| `frontend/` | Next.js 15。画面、マップの描画、ビルドの計算 | [frontend/CLAUDE.md](frontend/CLAUDE.md) |
| `gateway/` | ASP.NET Core 10 + YARP。入口、CORS、管理用の鍵の確認、サービスへの振り分け | [gateway/CLAUDE.md](gateway/CLAUDE.md) |
| `backend/` | ASP.NET Core 10 + EF Core の 3 つのサービス(`master`、`record`、`relic`)と共通の部品(`shared`) | [backend/CLAUDE.md](backend/CLAUDE.md) |
| `db/init/` | PostgreSQL が空のボリュームで初めて起動したときに、サービスごとのデータベースと利用者を作る手順 | [データベースの共通仕様](docs/design/common-database.md) 5.1 |
| `docs/` | 仕様書の中身。Markdown、図、画面の試作 | `.claude/rules/spec-authoring.md` |
| `docs-site/` | 仕様書をブラウザで読むサイト。アプリ本体とは別のもの | [docs-site/README.md](docs-site/README.md) |

アプリ本体と仕様書のサイトは分けて管理する。
直下の `docker-compose.yml` にはアプリのコンテナだけを書き、仕様書のサイトは `docs-site/compose.yaml` で起動する。
`docs/` には文書の中身だけを置き、サイトの設定や見た目を置かない。

バックエンドは 2026-10-17 に NestJS から .NET への作り直しを決めた(決定の記録 [0001](docs/decisions/0001-dotnet-backend.md))。
旧版の SQL と初期データはコミット `6529442` の `backend/migrations/` にある。

## 破ってはいけない決まり

- **利用者が承認していない仕様書をもとに実装しない。** 実装の途中で仕様を変えるときは、コードより先に仕様書を直す(`.claude/rules/spec-authoring.md`)。
- **権限は Gateway とサービスで判定する。** 画面で管理の入口を隠すのは表示の都合で、権限の判定にならない。
- **サービスは、ほかのサービスのデータベースに触れない。** データが要るときは、持ち主のサービスの `/internal/v1` の API を呼ぶ。呼ぶ向きは `record-service` から `master-service` への一方向だけにする。
- **Domain と Application から EF Core と ASP.NET Core を使わない。** 層の向きはテスト(`AC-SVC-06`)で守る。テストを外して通さない。
- **記録とマスタの行を消さない。** 記録は直さず、要らなくなったマスタは `hidden_at` で非表示にする(決定の記録 [0005](docs/decisions/0005-keep-play-records.md)、[0013](docs/decisions/0013-hide-masters-instead-of-delete.md))。
- **ビルドシミュレーターの計算はブラウザだけで行う。** サービスに同じ計算を書かない(決定の記録 [0006](docs/decisions/0006-build-calc-in-browser.md))。
- **テーブルは EF Core Migrations だけで変える。** `psql` で DDL を流さない。`main` にあるマイグレーションを書き換えない。
- **秘密の値(`SERVICE_KEY`、`ADMIN_KEY`、データベースの合言葉)を、リポジトリにもログにも出さない。** `.env` は Git に入れない。

## 確かめてから終える

作ったものは、動かして確かめてから「できた」と報告する。
確かめられなかったときは、何を確かめていないかを報告に書く。

確かめ方は 2 段階である。

1. **書いている間**: 道具のコンテナで、ビルド、型検査、lint、テストを走らせる。
2. **つないで確かめる**: `docker compose up --build` で一式を動かし、Gateway(8080)から呼んで確かめる。サービスをまたぐ変更、Gateway の変更、画面を変えたときに行う。

```bash
docker compose run --rm backend-sdk dotnet build NightreignTools.slnx
docker compose run --rm backend-sdk dotnet test NightreignTools.slnx
docker compose run --rm frontend-dev npx tsc --noEmit
docker compose run --rm frontend-dev npm run lint
docker compose run --rm frontend-dev npm run test
```

.NET と Node は常に Docker のコンテナの中で動かす。ホスト側に依存を入れない。
Docker が起動していないときは、確かめられないことを利用者に伝え、起動を頼む。

画面を変えたときは、Playwright でその画面を開き、スクリーンショットを見て確かめる。
型検査とテストが通っても、表示が崩れていることがある。

## 座標と日時

- 座標と半径は、1024 四方の座標空間の数で持つ。マップ画像(`frontend/public/map/`)は 1000 px 四方で、座標空間いっぱいに引き伸ばして描く(決定の記録 [0004](docs/decisions/0004-pixel-coordinates.md))。
- 日時は UTC の `timestamp with time zone` で持ち、API では末尾に `Z` の付いた ISO 8601 で返す。画面は日本時間に直して出す。

## 作業の進め方

- 実装の前に、GitHub の Issue を作るか、対応する Issue を確かめる。段階ごとの Issue は #50 から #54 である。
- 1 回の作業は、1 つのサービスの 1 つの機能か、1 つの画面にする。
- サービスをまたぐ変更は、呼ばれる側、呼ぶ側、画面の順に分けて進める。
- 変更は PR で `main` に入れる。`main` に直接コミットしない。
- ブランチの名前は `<種類>/<Issue 番号>-<内容>` とする。例: `docs/49-dotnet-specs`、`feature/50-master-api`
- PR は draft で作り、本文は [.claude/rules/pr-authoring.md](.claude/rules/pr-authoring.md) に従う。
- 1 つの PR の修正量が多くなるときは、複数の PR に分けて積む。目安と分け方は [.claude/rules/pr-size.md](.claude/rules/pr-size.md) に従う。
- マージは利用者が行う。Claude はマージしない。前の PR がマージされたら、次の PR の base を付け替えるのは Claude が行う。
- 作業を終えたら、今いるブランチと、まだマージされていない PR を報告に書く。
- 受け入れ条件(各機能の仕様書の 7 章)を先にテストにし、それが通るように実装する。テストの名前かコメントに識別子(例: `AC-REC-03`)を書く。
- うまくいかなかったことと、その直し方は、この文書か該当する Skill に書き足す。

## 並行して作業するとき

- `git add -A` と `git add .` を、範囲を付けずに使わない。ファイルかディレクトリの名前を挙げて足す。並行の作業場所(`.claude/worktrees/`)や、生成物を巻き込まないためである。
- 並行する作業には、決定の記録の番号の範囲を先に割り当てる。
- 仕様書のサイトの `nav`、`docs/index.md`、`README.md` は、どの作業も行を足すので衝突する。マージの順に解く。

## Windows のシェルでの注意

- 複数行の文字列をファイルに書くときは、ヒアドキュメントを使わず Write を使う。Bash のヒアドキュメントは、中身の記号で解釈が崩れることがある。
- Python でファイルを直すときは、Windows のパス(`\N` など)を含む文字列を、普通の文字列リテラルに書かない。エスケープとして読まれて壊れる。
- 日本語を含む応答を `curl` や Python の `print` でそのまま見ると、文字が化ける。化けても中身は正しいことが多いので、比べるときは UTF-8 で読み直すか、識別子と数で確かめる。
- Git Bash から `docker compose run` に `/src/...` のようなパスを渡すときは、先頭に `MSYS_NO_PATHCONV=1` を付ける。付けないとパスが Windows の形に書き換えられる。
- ホストの IDE(Visual Studio)が開いていると、新しいプロジェクトの `obj/` に Windows のパスで restore の結果を書く。コンテナの restore と build が止まるか失敗するので、その `obj/` と `bin/` を消してからコンテナで restore し直す。
- コンテナから外のネットワークにつながらないことがある(ホストからはつながる)。NuGet の restore が止まったら、`dotnet restore NightreignTools.slnx --source /root/.nuget/packages` で手元のキャッシュから restore する。キャッシュにないパッケージが要るときは、Docker Desktop を起動し直してもらう。
- Playwright のコンテナから画面(3000)を開くときは `--network host` を付ける。画面は `localhost:8080` の Gateway を呼ぶので、`host.docker.internal` で開くと API が届かず、接続のエラーの表示になる。

## 文章の規約

日本語の文章は [.claude/rules/writing-style.md](.claude/rules/writing-style.md) に従う。
コード中のコメントは日本語で書く。

## 環境変数

`.env` に書き、Git に入れない。名前の一覧は `.env.example` にある。

```
SERVICE_KEY=        # Gateway と各サービスの間、record と master の間のサービス間キー
DB_USER=            # PostgreSQL の管理者。db/init/ とテスト用のデータベースの作り直しに使う
DB_PASSWORD=        # PostgreSQL の管理者の合言葉
MASTER_DB_PASSWORD= # nightreign_master の利用者の合言葉
RECORD_DB_PASSWORD= # nightreign_record の利用者の合言葉
RELIC_DB_PASSWORD=  # nightreign_relic の利用者の合言葉
CORS_ORIGINS=       # Gateway が許すオリジン。省くと http://localhost:3000
ADMIN_KEY=          # 管理用の鍵。16 文字以上。/api/v1/admin の呼び出しに X-Admin-Key で付ける
```

## 起動

```bash
docker compose up --build
```

| URL | 用途 |
|-----|------|
| http://localhost:3000 | 画面 |
| http://localhost:8080 | Gateway |
| http://localhost:8000 | 仕様書のサイト(`docker compose -f docs-site/compose.yaml up -d`) |

3 つのサービス(8081 から 8083)は、コンテナの間だけに公開する。

## Claude Code のコマンドと Skill

| 名前 | 使う時 |
|------|------|
| `/plan` | 機能の実装計画を立てる |
| `/implement` | 計画から実装、テストまでを進める |
| `/test-gen` | 既存の実装にテストを足す |
| `/review-code` | 変更を 5 観点でレビューする |
| `/pr` | PR の本文を作る |
| `add-endpoint` | サービスに API を足す、変える |
| `add-migration` | テーブルやマスタデータを足す、変える |

## 未決事項

[要件定義](docs/requirements.md) の 12 章にまとめている。
主なものは、収縮候補地と第二収縮地点の正しい座標(初期データはダミー)、開始地点が 9 か所ですべてか、マップ画像の権利である。
