# 機能設計: マスタの管理

## 1. 目的

管理者が、マップを押して、開始地点、収縮候補地、第二収縮地点、拠点候補地の座標を登録し、直せる。
初期データのダミーの座標を、実際のゲームで確かめた座標に置き換えられるようにする。

## 2. 範囲

やること:

- 4 種類の場所の登録、修正、非表示(決定の記録 [0013](../decisions/0013-hide-masters-instead-of-delete.md))
- プレイヤー向けのマスタの取得の API(この文書の 4.1)
- `record-service` が使う、マスタの参照を解決する内部の API(この文書の 4.4)
- 管理用の鍵の確かめ(決定の記録 [0009](../decisions/0009-admin-key.md))

やらないこと:

- 夜の王、地変、夜ボス、遺物の効果の編集。マイグレーションの初期データで管理する(`add-migration` の Skill)
- 管理者ごとのアカウントと、誰が直したかの記録
- 変更の履歴
- 行を実際に消すこと

## 3. 画面

パス: `/admin`
試作: [画面: マスタの管理](../screens/admin.md)

### 3.1 管理用の鍵

- 画面を開いたとき、端末の `sessionStorage` の `nightreign_admin_key` に鍵がなければ、マップと入力の欄を出さず、鍵の入力の欄だけを出す。
- 鍵を入れて「入る」を押すと、`GET /api/v1/admin/spawn-points` を鍵を付けて呼ぶ。200 なら鍵を `sessionStorage` に置き、管理の画面を出す。401 なら「管理用の鍵が違います」と出し、入力の欄に留まる。
- 鍵の入力の欄は `type="password"` にする。
- 管理用の API はすべて、`frontend/src/lib/api.ts` が `X-Admin-Key` を付けて呼ぶ。プレイヤー向けの API には付けない。
- 管理の途中で 401 が返ったら、`sessionStorage` の鍵を消して鍵の入力の欄に戻す。入力中の名前と座標は、画面の状態に残す。
- ヘッダーの `ADMIN` の印の隣に「鍵を消す」を置く。押すと `sessionStorage` の鍵を消し、鍵の入力の欄に戻る。

### 3.2 登録するもの

| 種類 | マップの印 | 入力する項目 |
|---|---|---|
| 開始地点 | 開始地点のアイコン | 名前、X、Y |
| 収縮候補地 | `tide` の中心の印。直しているものは実際の大きさの円 | 名前、X、Y、半径 |
| 第二収縮地点 | `tide` の菱形 | 名前、X、Y |
| 拠点候補地 | 「?」の丸 | 名前(任意)、X、Y、マップ |

- 4 つのボタンで種類を切り替える。選んだ種類の印だけをマップに出す。
- 拠点候補地は、マップの種類(通常、各地変)ごとに持つ。マップの選択の欄で切り替える。

### 3.3 操作

- マップを押すと、押した場所の座標(小数 2 桁)を X と Y に入れ、新しく登録する状態にする。マップの左上に座標を出す。
- 登録済みの印か、一覧の行を押すと、その行を直す状態にする。題を「収縮候補地「最北」を直す」の形にする。
- 収縮候補地の半径は、100 から 500 の範囲のつまみで変える。直している円を金の破線でマップに出す。
- 「保存する」で登録か修正を送る。「取り消す」で入力を捨てる。
- 「非表示にする」は、確かめの枠を出してから送る。枠には「プレイヤーの画面と、新しい記録の入力から外れます。過去の記録には残ります」と出す。
- 非表示の行は、一覧に「非表示」の印を付けて残し、マップには点線の印で出す。押すと「表示に戻す」が使える。

### 3.4 入力の検証

| 項目 | 決まり | 誤りの文言 |
|---|---|---|
| 名前 | 1 文字以上 100 文字以下。拠点候補地では任意 | 「名前を入れてください」 |
| X、Y | 0 以上 1024 以下の数。小数は 2 桁まで | 「0 から 1024 の数を入れてください」 |
| 半径 | 10 以上 512 以下 | 「10 から 512 の数を入れてください」 |
| マップ | `normal` か地変の識別子 | |

## 4. API

### 4.1 プレイヤー向けの取得

必要な役割: なし

| メソッドとパス | 出力 |
|---|---|
| `GET /api/v1/spawn-points` | `[{ id, nameJa, x, y }]` |
| `GET /api/v1/shrink-zones` | `[{ id, nameJa, iconPath, centerX, centerY, radius }]` |
| `GET /api/v1/second-shrink-points` | `[{ id, nameJa, x, y }]` |
| `GET /api/v1/base-points?mapId=normal` | `[{ id, mapId, nameJa, x, y }]`。`mapId` を省くと `normal` |
| `GET /api/v1/bosses` | `[{ id, nameJa, nameEn, imagePath, weaknesses, isDlc, unlockCondition }]` |
| `GET /api/v1/jihen` | `[{ id, nameJa, imagePath, area, centerX, centerY, isDlc, unlockCondition }]` |
| `GET /api/v1/night-bosses` | `[{ id, nameJa, day }]` |

夜の王と地変は、初期データの並び順(`sort_order`)、識別子の順に並べる。
そのほかは識別子の順とする。
受け持ちはすべて `master-service` である。
開始地点、収縮候補地、第二収縮地点、拠点候補地の一覧には、非表示の行を入れない。

**エラー**

| 状態 | 条件 | `message` の例 |
|---|---|---|
| 404 | `mapId` が `normal` でも地変の識別子でもない | `mapId 'unknown' は存在しません` |

`mapId` が地変の識別子で、その地変の拠点候補地がまだないときは、200 と空の配列を返す。

### 4.2 管理用

必要な役割: 管理者(Gateway が `X-Admin-Key` を確かめる)

| メソッドとパス | 内容 | 成功 |
|---|---|---|
| `GET /api/v1/admin/{種類}` | 一覧。非表示の行も入れる | 200 |
| `POST /api/v1/admin/{種類}` | 登録。作った行を返す | 201 |
| `PUT /api/v1/admin/{種類}/{id}` | 修正、非表示、表示に戻す。直した行を返す | 200 |

受け持ちは `master-service` である。
`{種類}` は `spawn-points`、`shrink-zones`、`second-shrink-points`、`base-points` の 4 つである。
出力と入力の形は 4.1 の出力に `isHidden`(真偽)を足したものとする。
入力には `id` を含めない。
`isHidden` を真にすると `hidden_at` に今の日時を入れ、偽にすると `NULL` に戻す。
行を消す API は持たない(決定の記録 [0013](../decisions/0013-hide-masters-instead-of-delete.md))。

**エラー**

| 状態 | 条件 |
|---|---|
| 400 | 3.4 の決まりに反する |
| 401 | `X-Admin-Key` がないか、`ADMIN_KEY` と合わない。Gateway が返し、`message` は「管理用の鍵が違います」 |
| 404 | `id` の行がない |
| 429 | 同じ IP から 1 分に 60 回を超えて呼んだ。Gateway が返し、`message` は「しばらく時間をおいてやり直してください」 |


### 4.3 Gateway の確かめ

- `ADMIN_KEY` は 16 文字以上とする。ないか、短いときは、Gateway の起動を止め、ログに `ADMIN_KEY` の名前を出す。
- 比べ方は、時間で中身を推測されない比べ方(`CryptographicOperations.FixedTimeEquals`)にする。
- 鍵がないときと合わないときで、応答を変えない。
- 回数の制限は、`/api/v1/admin` の下への要求だけに、送り元の IP ごとにかける。鍵が合っている要求も数える。
- 確かめに通った要求は、`X-Admin-Key` を消してから `master-service` へ渡す。
- 鍵の値と、鍵が合わなかった要求の見出しをログに出さない。合わなかったことと送り元の IP だけを `Warning` で出す。

### 4.4 マスタの参照の解決(内部)

`record-service` が、記録の保存とパターン検索のときに呼ぶ(サービスの共通構成 5.5)。
Gateway は振り分けない。

`POST /internal/v1/references/resolve`

受け持ち: `master-service`
必要な役割: サービス間キー

**入力**

```json
{
  "bossIds": ["gladius"],
  "jihenIds": [],
  "spawnPointIds": [1],
  "zoneIds": [4, 7],
  "secondShrinkPointIds": [4],
  "nightBossIds": [3]
}
```

どの配列も省ける。
1 つの配列は 100 件までとする。

**出力(200)**

```json
{
  "bosses": [{ "id": "gladius", "nameJa": "三つ首の獣" }],
  "jihen": [],
  "spawnPoints": [{ "id": 1, "nameJa": "北東の教会", "x": 351.0, "y": 387.0, "isHidden": false }],
  "zones": [{ "id": 4, "nameJa": "最北", "centerX": 583.0, "centerY": 297.0, "radius": 255.0, "isHidden": false }],
  "secondShrinkPoints": [{ "id": 4, "nameJa": "最北", "x": 583.0, "y": 297.0, "isHidden": false }],
  "nightBosses": [{ "id": 3, "nameJa": "貪食ドラゴン", "day": 1 }]
}
```

- 見つかった行だけを返す。見つからない識別子は、エラーにせず出力から抜く。足りないものを 404 にするかは、呼ぶ側が決める。
- 非表示の行も返し、`isHidden` で分かるようにする。過去の記録のパターンを出すためである。

**エラー**

| 状態 | 条件 |
|---|---|
| 400 | 配列が 100 件を超える |
| 401 | サービス間キーがないか、誤っている |

## 5. データ

持ち主: `master-service`(データベース `nightreign_master`)

| テーブル | 列 |
|---|---|
| `spawn_points` | `id`、`name_ja`、`x`、`y`、`hidden_at`、`created_at` |
| `shrink_zones` | `id`、`name_ja`、`icon_path`、`center_x`、`center_y`、`radius`、`hidden_at`、`created_at` |
| `second_shrink_points` | `id`、`name_ja`、`x`、`y`、`hidden_at`、`created_at` |
| `base_points` | `id`、`map_id`、`name_ja`、`x`、`y`、`hidden_at`、`created_at` |

座標と半径は `numeric(7,2)` とし、0 以上 1024 以下の検査の制約を付ける。
`hidden_at` は `timestamp with time zone` で、`NULL` は表示中を表す。

## 6. サービスの間のやり取り

画面 → Gateway → `master-service` の順に呼ぶ。
管理者の確かめは Gateway で行い、`master-service` は行わない(4.3)。
`master-service` は、ほかのサービスを呼ばない。
非表示にするときも、記録から参照されているかを確かめない(決定の記録 [0013](../decisions/0013-hide-masters-instead-of-delete.md))。

## 7. 受け入れ条件

| 識別子 | 条件 |
|---|---|
| `AC-ADM-01` | 収縮候補地を `{ nameJa: "最北", centerX: 583, centerY: 297, radius: 150 }` で登録すると 201 が返り、`GET /api/v1/shrink-zones` に入っている |
| `AC-ADM-02` | `x` に 1025 を入れて開始地点を登録すると 400 が返る |
| `AC-ADM-03` | 存在しない `id` を修正すると 404 が返る |
| `AC-ADM-04` | 第二収縮地点を `isHidden: true` で修正すると 200 が返り、`GET /api/v1/second-shrink-points` から消え、`GET /api/v1/admin/second-shrink-points` には `isHidden: true` で残る |
| `AC-ADM-05` | 非表示の開始地点を `isHidden: false` で修正すると、`GET /api/v1/spawn-points` に戻る |
| `AC-ADM-06` | `GET /api/v1/base-points?mapId=crater` は、`map_id` が `crater` の行だけを返す |
| `AC-ADM-21` | `GET /api/v1/base-points?mapId=unknown` は 404 を返す |
| `AC-ADM-22` | 非表示の開始地点は、`GET /api/v1/spawn-points` に入らない |
| `AC-ADM-23` | `GET /api/v1/bosses` の先頭が `gladius`、`GET /api/v1/jihen` の先頭が `crater` で、どちらも DLC のものが最後に来る |
| `AC-ADM-07` | 画面でマップを押すと、X と Y にその場所の座標が小数 2 桁で入る |
| `AC-ADM-08` | 画面で「非表示にする」を押すと確かめの枠が出て、確かめると一覧の行に「非表示」の印が付く |
| `AC-ADM-09` | Gateway に `X-Admin-Key` なしで `POST /api/v1/admin/spawn-points` を送ると 401 が返り、行が増えない |
| `AC-ADM-10` | Gateway に誤った `X-Admin-Key` を付けて `GET /api/v1/admin/spawn-points` を送ると 401 が返り、`message` が「管理用の鍵が違います」である |
| `AC-ADM-11` | Gateway に `X-Admin-Key` なしで `GET /api/v1/shrink-zones` を送ると 200 が返る |
| `AC-ADM-12` | `ADMIN_KEY` を 15 文字にして Gateway を起動すると、起動が止まり、ログに `ADMIN_KEY` の名前が出る |
| `AC-ADM-13` | 同じ IP から 1 分の間に `/api/v1/admin/spawn-points` を 61 回呼ぶと、61 回目が 429 になる |
| `AC-ADM-14` | `DELETE /api/v1/admin/spawn-points/1` を送ると 405 か 404 が返り、行が残る |
| `AC-ADM-18` | 内部の API に `zoneIds: [4, 99]` を送ると、`zones` に 4 だけが入って返る |
| `AC-ADM-19` | 非表示の収縮候補地を内部の API で引くと、`isHidden: true` で返る |
| `AC-ADM-20` | Gateway に `POST /internal/v1/references/resolve` を送ると 404 が返る |
| `AC-ADM-15` | 画面で、鍵のない状態で `/admin` を開くと、鍵の入力の欄だけが出て、マップが出ない |
| `AC-ADM-16` | 画面で誤った鍵を入れて「入る」を押すと、「管理用の鍵が違います」と出て、鍵の入力の欄に留まる |
| `AC-ADM-17` | 画面で「鍵を消す」を押すと、`sessionStorage` から鍵が消え、鍵の入力の欄に戻る |

## 8. 未決事項

- **Gateway の受け入れ条件の確かめ方**: `AC-ADM-09` から `AC-ADM-13` は Gateway の動きである。Gateway のテストのプロジェクトで確かめるか、compose で動かして確かめるかは、[サービスの共通構成](common-service.md) の未決事項と合わせて段階 0 で決める。
