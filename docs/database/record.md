# テーブルの定義書: record-service

このファイルは `backend/tools/NightreignTools.SchemaDoc` がエンティティから作った生成物である。
手で直さない。説明を変えるときは、Domain のエンティティの `<summary>` を直して作り直す([データベースの共通仕様](../design/common-database.md) 5.8)。

- **データベース**: `nightreign_record`
- **テーブル**: 2

## game.play_records

1 回のプレイの記録(プレイ記録 5 章)。作った後は直さず、消さない(決定の記録 0005)。夜の王、地変、開始地点、夜ボスは master-service の行の識別子だけを持つ。あるかどうかはアプリケーションサービスが確かめる。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `boss_id` | `character varying(50)` | 許さない |  | 夜の王の識別子。master-service の bosses の id。 |
| `jihen_id` | `character varying(50)` | 許す |  | 地変の識別子。null は地変なし。 |
| `spawn_point_id` | `integer` | 許さない |  | 開始地点の識別子。master-service の spawn_points の id。 |
| `day1_night_boss_id` | `integer` | 許す |  | 1 日目の夜ボスの識別子。null は選ばなかった。 |
| `day2_night_boss_id` | `integer` | 許す |  | 2 日目の夜ボスの識別子。null は選ばなかった。 |
| `note` | `character varying(500)` | 許す |  | 備考。前後の空白を除いたもの。空なら null。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 記録を保存した日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー: なし

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_play_records_boss_id_spawn_point_id_jihen_id` | `boss_id`、`spawn_point_id`、`jihen_id` |  |

検査の制約: なし

## game.shrink_entries

1 つの記録の中の、日と段階ごとの収縮先(プレイ記録 5 章)。第 1 次(phase=1)は収縮候補地(ZoneId)だけ、第 2 次(phase=2)は第二収縮地点(SecondShrinkPointId)だけを持つ(決定の記録 0008)。マスタの行は master-service が持つので、識別子だけを持つ。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `play_record_id` | `integer` | 許さない |  | この収縮先を持つ記録の識別子。 |
| `day` | `integer` | 許さない |  | 日。1 か 2。 |
| `phase` | `integer` | 許さない |  | 段階。1 か 2。 |
| `zone_id` | `integer` | 許す |  | 第 1 次の収縮先の収縮候補地の識別子。第 2 次では null。 |
| `second_shrink_point_id` | `integer` | 許す |  | 第 2 次の収縮先の第二収縮地点の識別子。第 1 次では null。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー:

| 列 | 参照先 | 親を消したとき |
|---|---|---|
| `play_record_id` | `game.play_records`(`id`) | 子も消す(CASCADE) |

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_shrink_entries_play_record_id_day_phase` | `play_record_id`、`day`、`phase` | 一意 |

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_shrink_entries_day` | `day IN (1, 2)` |
| `ck_shrink_entries_phase` | `phase IN (1, 2)` |
| `ck_shrink_entries_target` | `(phase = 1 AND zone_id IS NOT NULL AND second_shrink_point_id IS NULL) OR (phase = 2 AND zone_id IS NULL AND second_shrink_point_id IS NOT NULL)` |
