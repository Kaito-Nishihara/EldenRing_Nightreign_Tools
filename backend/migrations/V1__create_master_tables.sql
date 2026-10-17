-- マスタテーブル作成

CREATE SCHEMA IF NOT EXISTS game;

CREATE TABLE game.bosses (
    id               VARCHAR(50)  PRIMARY KEY,
    name_ja          VARCHAR(100) NOT NULL,
    name_en          VARCHAR(100),
    image_path       VARCHAR(200),
    is_dlc           BOOLEAN      NOT NULL DEFAULT FALSE,
    unlock_condition VARCHAR(200),
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE game.boss_weaknesses (
    id        SERIAL      PRIMARY KEY,
    boss_id   VARCHAR(50) NOT NULL REFERENCES game.bosses(id),
    attribute VARCHAR(30) NOT NULL
);

CREATE TABLE game.jihen (
    id               VARCHAR(50)  PRIMARY KEY,
    name_ja          VARCHAR(100) NOT NULL,
    image_path       VARCHAR(200),
    area             VARCHAR(50)  NOT NULL,
    center_x         DECIMAL(5,4) NOT NULL,
    center_y         DECIMAL(5,4) NOT NULL,
    unlock_condition VARCHAR(200),
    is_dlc           BOOLEAN      NOT NULL DEFAULT FALSE,
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE game.shrink_zones (
    id         SERIAL       PRIMARY KEY,
    name_ja    VARCHAR(100) NOT NULL,
    icon_path  VARCHAR(200),
    center_x   DECIMAL(5,4) NOT NULL,
    center_y   DECIMAL(5,4) NOT NULL,
    radius     DECIMAL(5,4) NOT NULL,
    created_at TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE game.spawn_points (
    id               SERIAL       PRIMARY KEY,
    name_ja          VARCHAR(100) NOT NULL,
    x                DECIMAL(5,4) NOT NULL,
    y                DECIMAL(5,4) NOT NULL,
    nearby_base_type VARCHAR(30)  NOT NULL,
    created_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);
