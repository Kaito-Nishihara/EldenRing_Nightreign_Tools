# テーブルの定義書: relic-service

このファイルは `backend/tools/NightreignTools.SchemaDoc` がエンティティから作った生成物である。
手で直さない。説明を変えるときは、Domain のエンティティの `<summary>` を直して作り直す([データベースの共通仕様](../design/common-database.md) 5.8)。

- **データベース**: `nightreign_relic`
- **テーブル**: 3

## game.relic_effect_categories

遺物の効果の分類。例: 能力値、攻撃力。マイグレーションの初期データだけで管理する。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `character varying(50)` | 許さない |  | 識別子。英小文字の固定の値。例: attack。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `sort_order` | `integer` | 許さない |  | 画面での並び順。小さいほうが先(ビルドシミュレーター 3.2)。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー: なし

索引: なし

検査の制約: なし

## game.relic_effect_ranks

遺物の効果のランクごとの値。効果とランクの組は 1 行だけ。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `effect_id` | `integer` | 許さない |  | 効果の識別子。 |
| `rank` | `integer` | 許さない |  | ランク。ランクのない効果は 0。 |
| `value` | `numeric(10,4)` | 許す |  | このランクの値。単位は Unit による。null は数で表せない効果(special)。 |
| `unit` | `character varying(20)` | 許さない |  | 値の単位。multiplier(倍率)、percent(% の点数)、flat(固定値)、per_sec(1 秒ごと)、none(数なし)のどれか。 |
| `has_demerit` | `boolean` | 許さない | `False` | このランクからデメリットが付くか。深層効果で使う。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー:

| 列 | 参照先 | 親を消したとき |
|---|---|---|
| `effect_id` | `game.relic_effects`(`id`) | 子も消す(CASCADE) |

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_relic_effect_ranks_effect_id_rank` | `effect_id`、`rank` | 一意 |

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_relic_effect_ranks_rank` | `rank >= 0` |
| `ck_relic_effect_ranks_unit` | `unit IN ('multiplier', 'percent', 'flat', 'per_sec', 'none')` |

## game.relic_effects

遺物に付く 1 つの効果。通常効果と深層効果を同じテーブルで持ち、深層効果かで分ける。マイグレーションの初期データだけで管理する。

| 列 | 型 | NULL | 既定値 | 説明 |
|---|---|---|---|---|
| `id` | `integer` | 許さない | 自動の採番(identity) | 識別子。 |
| `category_id` | `character varying(50)` | 許さない |  | 分類の識別子。 |
| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |
| `is_deep` | `boolean` | 許さない | `False` | 深き夜の遺物に付く効果(深層効果)か。 |
| `effect_sign` | `character varying(20)` | 許さない | `positive` | 効果がメリットかデメリットか。positive か negative。 |
| `calc_type` | `character varying(20)` | 許さない |  | 効果を重ねるときの合わせ方。multiply、percent_add、flat_add、special のどれか。 |
| `attribute` | `character varying(50)` | 許す |  | 効果がかかる対象。例: physical、strength。null は効果ごとに別に合わせる。 |
| `stackable` | `character varying(20)` | 許さない | `yes` | 同じ効果を重ねて付けられるか。yes、no、conditional(条件付き)のどれか。 |
| `rank_min` | `integer` | 許さない |  | ランクの下限。ランクのない効果は 0。 |
| `rank_max` | `integer` | 許さない |  | ランクの上限。ランクのない効果は 0。 |
| `condition_ja` | `character varying(500)` | 許す |  | 発動の条件。例: HP40%未満で発動。null は条件なし。 |
| `notes_ja` | `character varying(500)` | 許す |  | 備考。null はなし。 |
| `created_at` | `timestamp with time zone` | 許さない | `now()` | 行を作った日時(UTC)。データベースの既定値で入る。 |

主キー: `id`

外部キー:

| 列 | 参照先 | 親を消したとき |
|---|---|---|
| `category_id` | `game.relic_effect_categories`(`id`) | 消せない(RESTRICT) |

索引:

| 名前 | 列 | 一意 |
|---|---|---|
| `ix_relic_effects_category_id` | `category_id` |  |

検査の制約:

| 名前 | 式 |
|---|---|
| `ck_relic_effects_calc_type` | `calc_type IN ('multiply', 'percent_add', 'flat_add', 'special')` |
| `ck_relic_effects_effect_sign` | `effect_sign IN ('positive', 'negative')` |
| `ck_relic_effects_rank` | `rank_min >= 0 AND rank_max >= rank_min` |
| `ck_relic_effects_stackable` | `stackable IN ('yes', 'no', 'conditional')` |
