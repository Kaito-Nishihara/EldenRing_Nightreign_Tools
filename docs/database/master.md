# テーブルの定義書: master-service

このファイルは `backend/tools/NightreignTools.SchemaDoc` がエンティティから作った生成物である。
手で直さない。説明を変えるときは、Domain のエンティティの `<summary>` を直して作り直す([データベースの共通仕様](../design/common-database.md) 5.8)。

- **データベース**: `nightreign_master`
- **テーブル**: 8

## game.base_points

拠点候補地。マップ(通常か地変)ごとに持ち、大拠点、小拠点、強敵の種類を持つ(決定の記録 0018)。その場所に出る施設の種類は持たない(決定の記録 0010)。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。 |
| `hidden_at` | `timestamp with time zone` | 許す |  | 非表示にした日時。null は表示中。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |
| `map_id` | `character varying(50)` | 許さない | `normal` | どのマップの拠点か。normal は通常のマップ、ほかは地変の識別子。 |
| `kind` | `character varying(20)` | 許さない | `major` | 拠点の種類。major は大拠点、minor は小拠点、field_boss は強敵。 |
| `name_ja` | `character varying(100)` | 許す |  | 日本語の名前。null は名前なし。 |
| `x` | `numeric(7,2)` | 許さない |  | x 座標。1024 四方の座標空間の数。 |
| `y` | `numeric(7,2)` | 許さない |  | y 座標。1024 四方の座標空間の数。 |

主キー: `id`

外部キー: なし

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_base_points_map_id` | `map_id` |  |

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_base_points_kind` | `kind IN ('major', 'minor', 'field_boss')` |
| `ck_base_points_x` | `x BETWEEN 0 AND 1024` |
| `ck_base_points_y` | `y BETWEEN 0 AND 1024` |

## game.boss_weaknesses

夜の王の弱点の属性。例: holy、fire。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `boss_id` | `character varying(50)` | 許さない |  | 夜の王の識別子。 |
| `attribute` | `character varying(30)` | 許さない |  | 弱点の属性。例: holy、fire。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー:

| 列 | 参照先 | 親を消したとき |
|---|---|---|
| `boss_id` | `game.bosses`(`id`) | 子も消す(CASCADE) |

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_boss_weaknesses_boss_id` | `boss_id` |  |

検査の制約: なし

## game.bosses

夜の王(3 日目に戦う相手)。識別子は英小文字の固定の値。マイグレーションの初期データだけで管理する。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `character varying(50)` | 許さない |  | 識別子。英小文字の固定の値。例: gladius。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `name_en` | `character varying(100)` | 許す |  | 英語の名前。 |
| `image_path` | `character varying(200)` | 許す |  | 画像のパス。null は画像なし。 |
| `is_dlc` | `boolean` | 許さない | `False` | ダウンロードコンテンツで足された夜の王か。 |
| `unlock_condition` | `character varying(200)` | 許す |  | 戦えるようになる条件。null は最初から戦える。 |
| `sort_order` | `integer` | 許さない |  | 画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約: なし

## game.jihen

地変。地変ごとにマップ画像が替わる。中心の座標は 1024 四方の座標空間の数。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `character varying(50)` | 許さない |  | 識別子。英小文字の固定の値。例: crater。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `image_path` | `character varying(200)` | 許す |  | 地変のマップ画像のパス。null は画像なし。 |
| `area` | `character varying(50)` | 許さない |  | 地変が起きる方角。例: north、center、all。 |
| `center_x` | `numeric(7,2)` | 許さない |  | 地変の中心の x 座標。1024 四方の座標空間の数。 |
| `center_y` | `numeric(7,2)` | 許さない |  | 地変の中心の y 座標。1024 四方の座標空間の数。 |
| `is_dlc` | `boolean` | 許さない | `False` | ダウンロードコンテンツで足された地変か。 |
| `unlock_condition` | `character varying(200)` | 許す |  | 起きるようになる条件。null は最初から起きる。 |
| `sort_order` | `integer` | 許さない |  | 画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_jihen_center_x` | `center_x BETWEEN 0 AND 1024` |
| `ck_jihen_center_y` | `center_y BETWEEN 0 AND 1024` |

## game.night_bosses

1 日目か 2 日目の夜に出る夜ボス。夜の王とは別に持つ。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `day` | `integer` | 許さない |  | 出る日。1 か 2。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_night_bosses_day` | `day IN (1, 2)` |

## game.second_shrink_points

第二収縮地点。第 2 次収縮の向かう点(決定の記録 0008)。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。 |
| `hidden_at` | `timestamp with time zone` | 許す |  | 非表示にした日時。null は表示中。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `x` | `numeric(7,2)` | 許さない |  | x 座標。1024 四方の座標空間の数。 |
| `y` | `numeric(7,2)` | 許さない |  | y 座標。1024 四方の座標空間の数。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_second_shrink_points_x` | `x BETWEEN 0 AND 1024` |
| `ck_second_shrink_points_y` | `y BETWEEN 0 AND 1024` |

## game.shrink_zones

収縮候補地。第 1 次収縮の円の中心と半径。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。 |
| `hidden_at` | `timestamp with time zone` | 許す |  | 非表示にした日時。null は表示中。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `icon_path` | `character varying(200)` | 許す |  | アイコンの画像のパス。null はアイコンなし。 |
| `center_x` | `numeric(7,2)` | 許さない |  | 円の中心の x 座標。1024 四方の座標空間の数。 |
| `center_y` | `numeric(7,2)` | 許さない |  | 円の中心の y 座標。1024 四方の座標空間の数。 |
| `radius` | `numeric(7,2)` | 許さない |  | 円の半径。1024 四方の座標空間の数。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_shrink_zones_center_x` | `center_x BETWEEN 0 AND 1024` |
| `ck_shrink_zones_center_y` | `center_y BETWEEN 0 AND 1024` |
| `ck_shrink_zones_radius` | `radius BETWEEN 0 AND 1024` |

## game.spawn_points

開始地点。プレイの始まりの場所。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。 |
| `hidden_at` | `timestamp with time zone` | 許す |  | 非表示にした日時。null は表示中。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `x` | `numeric(7,2)` | 許さない |  | x 座標。1024 四方の座標空間の数。 |
| `y` | `numeric(7,2)` | 許さない |  | y 座標。1024 四方の座標空間の数。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_spawn_points_x` | `x BETWEEN 0 AND 1024` |
| `ck_spawn_points_y` | `y BETWEEN 0 AND 1024` |
