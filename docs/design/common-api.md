# 共通設計: API の共通仕様

## 1. 目的

どのエンドポイントも、同じパス、同じ JSON、同じエラーの形で作れるようにする。
画面側が、エラーの扱いを 1 か所に書けるようにする。

## 2. 範囲

やること:

- パスと版の付け方
- JSON の名前と型、日時、座標
- エラーの形と HTTP の状態
- 入力の検証の決まり
- Gateway と各サービスの間の決まりと、Gateway の振り分け
- サービスの間だけで使う API の決まり
- OpenAPI の出し方

やらないこと:

- 個々のエンドポイントの入出力。機能ごとの仕様書に書く
- 管理用の鍵の確かめの細部。[マスタの管理](master-admin.md) の 4.3 に書く

## 3. 画面

なし。

## 4. API

### 4.1 パス

- パスはすべて `/api/v1` の下に置く。
- 名前は英小文字の複数形をハイフンでつなぐ。例: `/api/v1/shrink-zones`、`/api/v1/play-records`
- 地変は単数と複数の区別がないので `/api/v1/jihen` とする。
- 管理用の操作は `/api/v1/admin` の下に置く。プレイヤー向けと同じ資源でも、管理用のパスを分ける。権限の判定をパスの接頭辞で行えるようにするためである。
- Gateway は、パスの接頭辞で 3 つのサービスへ振り分ける(4.6)。Gateway 自身が答えるのは `/health` だけである。
- サービスの間だけで使う API は `/internal/v1` の下に置く(4.8)。

### 4.2 JSON

- 名前は camelCase にする。例: `nameJa`、`spawnPointId`
- 日本語の名前は `nameJa`、英語の名前は `nameEn` とする。
- 識別子は、マスタの種類によって文字列(ボス、地変)か整数(それ以外)である。画面側は型をそのまま受け取る。
- 値がないことは `null` で表す。項目を省かない。
- 日時は UTC の ISO 8601(`2026-10-17T12:34:56Z`)で返す。画面は日本時間に直して出す。
- 座標と半径は、1024 四方の座標空間の数で返す。小数は 2 桁までとする(決定の記録 [0004](../decisions/0004-pixel-coordinates.md))。
- 一覧は配列をそのまま返す。件数の上限を設けない一覧は、マスタ(数十件から百件程度)に限る。

### 4.3 エラーの形

エラーは、どのエンドポイントでも次の形で返す。

```json
{
  "status": 404,
  "error": "Not Found",
  "message": "bossId 'unknown' は存在しません",
  "timestamp": "2026-10-17T12:34:56Z"
}
```

| 項目 | 内容 |
|---|---|
| `status` | HTTP の状態の数 |
| `error` | HTTP の状態の英語の名前 |
| `message` | 利用者に見せてよい日本語の説明。入力の検証で複数の誤りがあるときは、読点でつなぐ |
| `timestamp` | エラーを返した日時(UTC) |

`message` に、スタックトレース、SQL、内部のクラス名を入れない。

### 4.4 HTTP の状態

| 状態 | 使う時 |
|---|---|
| 200 | 取得と更新に成功した |
| 201 | 作成に成功した。作った内容を返す |
| 204 | 削除に成功した。本文を返さない |
| 400 | 入力の形が誤っている。必須の項目がない、範囲の外、重複 |
| 401 | サービスが返すときは `X-Service-Key` がないか、誤っている。Gateway が返すときは `X-Admin-Key` がないか、誤っている |
| 404 | パスの識別子か、入力で参照した識別子の行がない |
| 409 | 今の状態と食い違う操作。今のところ使う所はない |
| 429 | 管理用の API を、同じ IP から決まった回数を超えて呼んだ。Gateway が返す |
| 500 | 想定していない誤り。`message` は「内部サーバーエラーが発生しました」とする |
| 502、504 | Gateway がサービスにつなげなかった。YARP が返す |
| 503 | サービスが、呼んだ別のサービスから応答を得られなかった。例: `record-service` が `master-service` を呼べなかった |

### 4.5 入力の検証

- 形の検証(必須、型、範囲、文字数)は DTO の注釈で書き、各サービスの入口で行う。
- 参照の検証(その識別子の行があるか)と、業務の規則(重複など)は、サービスで行う。
- 文字列は前後の空白を除いてから検証する。
- 文字数の上限を決めない文字列の入力を作らない。備考は 500 文字、名前は 100 文字を上限とする。
- 知らない項目が JSON にあっても拒まず、捨てる。

### 4.6 Gateway と各サービスの間

Gateway は、パスの接頭辞で次のとおり振り分ける。
上から順に照らし、最初に合ったものへ渡す。

| パス | 渡す先 |
|---|---|
| `/api/v1/patterns`、`/api/v1/play-records` の下 | `record-service` |
| `/api/v1/relic-effects` の下 | `relic-service` |
| そのほかの `/api/v1` の下(`/api/v1/admin` を含む) | `master-service` |

- Gateway は `/internal/v1` を振り分けない。Gateway に来たら 404 を返す。
- Gateway は、画面から来た `X-Service-Key` を消し、自分の持つ値を付け直して渡す。
- 各サービスは、`/health` を除くすべてのパスで `X-Service-Key` を確かめる。値の比べ方は、時間で中身を推測されない比べ方(`CryptographicOperations.FixedTimeEquals`)にする。
- 各サービスの稼働の確認(`/health`)だけは、キーなしで答える。compose の稼働の確認に使うためである。
- Gateway の CORS で許すオリジンは、環境変数で渡す。`http://localhost:3000` を既定とする。
- Gateway は、`/api/v1/admin` の下への要求に `X-Admin-Key` を求め、確かめてから消して渡す(決定の記録 [0009](../decisions/0009-admin-key.md))。各サービスは管理者を確かめない。
- Gateway が自分で返すエラー(401、429)も、4.3 の形にする。

### 4.7 OpenAPI

- 各サービスは、開発の環境でだけ OpenAPI の文書(`/openapi/v1.json`)を出す。
- 画面側の型は、今は `frontend/src/types/index.ts` に手で書く。OpenAPI から型を作るかは未決である(8 章)。

### 4.8 サービスの間だけで使う API

- パスは `/internal/v1` の下に置く。
- 呼ぶ側は `X-Service-Key` を付ける。値は Gateway と同じ `SERVICE_KEY` とする。
- 入力、出力、エラーの形は、`/api/v1` と同じ決まりに従う。

## 5. データ

なし。

## 6. サービスの間のやり取り

- Gateway から各サービスへの要求の期限は 10 秒とする。
- サービスが応答しないとき、Gateway は 502 か 504 を返す。画面は「サーバーに接続できません。時間をおいてやり直してください」と出す。
- サービスの間の呼び出しの期限と再試行は、[サービスの共通構成](common-service.md) の 5.5 に書く。
- 画面の axios の期限は 10 秒とする(`frontend/src/lib/api.ts`)。

## 7. 受け入れ条件

| 識別子 | 条件 |
|---|---|
| `AC-API-01` | 存在しないボスの識別子で記録を保存すると、404 と、`status`、`error`、`message`、`timestamp` の 4 つの項目を持つ JSON が返る |
| `AC-API-02` | `day` に 3 を入れて記録を保存すると 400 が返り、`message` に `day` を含む説明が入る |
| `AC-API-03` | `master-service` の 8081 番に `X-Service-Key` なしで `GET /api/v1/bosses` を送ると 401 が返る |
| `AC-API-04` | 誤った `X-Service-Key` を付けて送ると 401 が返る |
| `AC-API-05` | Gateway の 8080 番に `X-Service-Key: wrong` を付けて `GET /api/v1/bosses` を送ると、Gateway が付け直すので 200 が返る |
| `AC-API-06` | サービスで想定していない例外が起きると 500 が返り、`message` にスタックトレースと SQL が入らない |
| `AC-API-07` | `master-service` を止めた状態で Gateway に `GET /api/v1/bosses` を送ると、502 か 504 が返る |
| `AC-API-08` | 応答の日時が、末尾に `Z` の付いた UTC の ISO 8601 の文字列である |
| `AC-API-09` | 各サービスに `X-Service-Key` なしで `GET /health` を送ると 200 が返る |
| `AC-API-10` | Gateway に `GET /api/v1/patterns`、`GET /api/v1/relic-effects`、`GET /api/v1/bosses` を送ると、それぞれ `record-service`、`relic-service`、`master-service` が答える |
| `AC-API-11` | Gateway に `/internal/v1` の下のパスを送ると 404 が返り、どのサービスにも渡らない |

## 8. 未決事項

- **OpenAPI からの型の生成**: 画面側の型を OpenAPI から作るか。API が増えて手で書く型が食い違い始めたら決める。
