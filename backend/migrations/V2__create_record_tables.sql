-- プレイ記録テーブル作成

CREATE TABLE game.play_records (
    id             SERIAL      PRIMARY KEY,
    boss_id        VARCHAR(50) NOT NULL REFERENCES game.bosses(id),
    jihen_id       VARCHAR(50) REFERENCES game.jihen(id),
    spawn_point_id INTEGER     REFERENCES game.spawn_points(id),
    note           TEXT,
    created_at     TIMESTAMP   NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE game.shrink_entries (
    id             SERIAL  PRIMARY KEY,
    play_record_id INTEGER NOT NULL REFERENCES game.play_records(id) ON DELETE CASCADE,
    day            INTEGER NOT NULL CHECK (day IN (1, 2)),
    phase          INTEGER NOT NULL CHECK (phase IN (1, 2)),
    zone_id        INTEGER NOT NULL REFERENCES game.shrink_zones(id),
    UNIQUE (play_record_id, day, phase)
);
