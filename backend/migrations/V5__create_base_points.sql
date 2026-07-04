-- 拠点候補地テーブル
-- マップ上の固定座標で、ゲームごとに異なる拠点タイプが出現する
CREATE TABLE game.base_points (
    id         SERIAL         PRIMARY KEY,
    name_ja    VARCHAR(100),
    x          DECIMAL(5, 4)  NOT NULL,
    y          DECIMAL(5, 4)  NOT NULL,
    created_at TIMESTAMP      NOT NULL DEFAULT CURRENT_TIMESTAMP
);
