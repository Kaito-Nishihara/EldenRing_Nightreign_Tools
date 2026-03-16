# データベース設計書

**バージョン**: 1.0
**作成日**: 2026-03-16

---

## 1. テーブル一覧

| テーブル名 | 概要 |
|-----------|------|
| `bosses` | 夜の王（ボス）マスタ |
| `boss_weaknesses` | ボスの弱点属性（1対多） |
| `jihen` | 地変マスタ |
| `shrink_zones` | 収縮候補地マスタ（12ヶ所） |
| `spawn_points` | 開始地点マスタ |
| `play_records` | プレイ記録（1セッション = 1レコード） |
| `shrink_entries` | 収縮実績詳細（1レコードあたり最大4エントリ） |

---

## 2. テーブル定義

### bosses（ボスマスタ）

```sql
CREATE TABLE bosses (
    id               VARCHAR(50)  PRIMARY KEY,        -- 例: 'gladius'
    name_ja          VARCHAR(100) NOT NULL,
    name_en          VARCHAR(100),
    is_dlc           BOOLEAN      NOT NULL DEFAULT FALSE,
    unlock_condition VARCHAR(200),
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### boss_weaknesses（ボス弱点属性）

```sql
CREATE TABLE boss_weaknesses (
    id        SERIAL      PRIMARY KEY,
    boss_id   VARCHAR(50) NOT NULL REFERENCES bosses(id),
    attribute VARCHAR(30) NOT NULL  -- 'holy', 'fire', 'lightning', 'poison', 'madness', 'sleep'
);
```

### jihen（地変マスタ）

```sql
CREATE TABLE jihen (
    id               VARCHAR(50)  PRIMARY KEY,        -- 例: 'crater'
    name_ja          VARCHAR(100) NOT NULL,
    area             VARCHAR(50)  NOT NULL,
    center_x         DECIMAL(5,4) NOT NULL,            -- マップ上のX座標（0.0〜1.0）
    center_y         DECIMAL(5,4) NOT NULL,            -- マップ上のY座標（0.0〜1.0）
    unlock_condition VARCHAR(200),
    is_dlc           BOOLEAN      NOT NULL DEFAULT FALSE,
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### shrink_zones（収縮候補地マスタ）

```sql
CREATE TABLE shrink_zones (
    id         SERIAL       PRIMARY KEY,
    name_ja    VARCHAR(100) NOT NULL,
    center_x   DECIMAL(5,4) NOT NULL,                  -- マップ上のX座標（0.0〜1.0）
    center_y   DECIMAL(5,4) NOT NULL,                  -- マップ上のY座標（0.0〜1.0）
    radius     DECIMAL(5,4) NOT NULL,                  -- 収縮円の半径
    created_at TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### spawn_points（開始地点マスタ）

```sql
CREATE TABLE spawn_points (
    id               SERIAL       PRIMARY KEY,
    name_ja          VARCHAR(100) NOT NULL,
    x                DECIMAL(5,4) NOT NULL,
    y                DECIMAL(5,4) NOT NULL,
    nearby_base_type VARCHAR(30)  NOT NULL,             -- 'camp' or 'church'
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### play_records（プレイ記録）

```sql
CREATE TABLE play_records (
    id             SERIAL      PRIMARY KEY,
    boss_id        VARCHAR(50) NOT NULL REFERENCES bosses(id),
    jihen_id       VARCHAR(50) REFERENCES jihen(id),   -- NULLは地変なし
    spawn_point_id INTEGER     REFERENCES spawn_points(id),
    note           TEXT,
    created_at     TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### shrink_entries（収縮実績詳細）

```sql
CREATE TABLE shrink_entries (
    id             SERIAL  PRIMARY KEY,
    play_record_id INTEGER NOT NULL REFERENCES play_records(id),
    day            INTEGER NOT NULL CHECK (day IN (1, 2)),      -- 1日目 or 2日目
    phase          INTEGER NOT NULL CHECK (phase IN (1, 2)),    -- 第1次 or 第2次収縮
    zone_id        INTEGER NOT NULL REFERENCES shrink_zones(id),
    UNIQUE (play_record_id, day, phase)
);
```

---

## 3. ER図

```
bosses 1──* boss_weaknesses

bosses 1──* play_records *──1 jihen
                │
                └──* shrink_entries *──1 shrink_zones

play_records *──1 spawn_points
```

---

## 4. パターン検索クエリ

「ボス × 開始地点 × 地変」で過去パターンを取得し、
同じ `(day1_zone, day2_zone)` の組み合わせをグループ化して件数も返す。

```sql
SELECT
    d1.zone_id         AS day1_phase1_zone_id,
    d2.zone_id         AS day1_phase2_zone_id,
    d3.zone_id         AS day2_phase1_zone_id,
    d4.zone_id         AS day2_phase2_zone_id,
    COUNT(*)           AS record_count,
    MAX(pr.created_at) AS last_recorded_at
FROM play_records pr
LEFT JOIN shrink_entries d1 ON d1.play_record_id = pr.id AND d1.day = 1 AND d1.phase = 1
LEFT JOIN shrink_entries d2 ON d2.play_record_id = pr.id AND d1.day = 1 AND d2.phase = 2
LEFT JOIN shrink_entries d3 ON d3.play_record_id = pr.id AND d3.day = 2 AND d3.phase = 1
LEFT JOIN shrink_entries d4 ON d4.play_record_id = pr.id AND d4.day = 2 AND d4.phase = 2
WHERE pr.boss_id        = :bossId
  AND pr.spawn_point_id = :spawnPointId
  AND (
        (:jihenId IS NULL     AND pr.jihen_id IS NULL)
     OR (:jihenId IS NOT NULL AND pr.jihen_id = :jihenId)
  )
GROUP BY d1.zone_id, d2.zone_id, d3.zone_id, d4.zone_id
ORDER BY record_count DESC;
```

---

## 5. Flywayマイグレーションファイル構成

```
resources/db/migration/
├── V1__create_master_tables.sql   # bosses, boss_weaknesses, jihen, shrink_zones, spawn_points
├── V2__create_record_tables.sql   # play_records, shrink_entries
└── V3__insert_master_data.sql     # 初期マスタデータ投入（ボス・地変・収縮候補地）
```
