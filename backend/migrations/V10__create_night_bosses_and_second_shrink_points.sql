-- 夜ボステーブル（最終ボス=夜の王とは別管理）
CREATE TABLE game.night_bosses (
    id         SERIAL       PRIMARY KEY,
    name_ja    VARCHAR(100) NOT NULL,
    day        INTEGER      NOT NULL,
    created_at TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 1日目夜ボス
INSERT INTO game.night_bosses (name_ja, day) VALUES
    ('亜人の女王＆亜人の剣聖', 1),
    ('鈴玉狩り', 1),
    ('貪食ドラゴン', 1),
    ('夜の騎兵：グレイブ＆夜の騎兵：フレイル', 1),
    ('英雄のガーゴイル', 1),
    ('ミミズ顔', 1),
    ('百足のデーモン', 1),
    ('熔鉄デーモン', 1),
    ('戦場の宿将', 1),
    ('爛れた樹霊', 1),
    ('接ぎ木の君主', 1),
    ('ティビアの呼び舟＆死に生きる者たち', 1),
    ('王族の幽鬼', 1),
    ('公のフレイディア', 1);

-- 2日目夜ボス
INSERT INTO game.night_bosses (name_ja, day) VALUES
    ('ツリーガード＆王都の騎兵', 2),
    ('忌み鬼', 2),
    ('古竜', 2),
    ('坩堝の騎士＆黄金カバ', 2),
    ('僻地の宿将', 2),
    ('ノクスの竜人兵', 2),
    ('竜のツリーガード＆王都の騎兵', 2),
    ('大土竜', 2),
    ('神肌の貴種＆神肌の使徒', 2),
    ('降る星の成獣', 2),
    ('冷たい谷の踊り子', 2),
    ('無名の王', 2),
    ('死儀礼の鳥', 2),
    ('傷ついたデーモン＆うろ底のデーモン', 2),
    ('デーモンの王子', 2),
    ('呪剣士＆神獣の戦士', 2),
    ('血の君主', 2),
    ('大赤熊', 2),
    ('神獣獅子舞', 2),
    ('死の騎士', 2),
    ('騎士アルトリウス', 2);

-- 第二収縮地点候補テーブル（座標のみ管理、夜ボスとの紐付けは記録画面で行う）
CREATE TABLE game.second_shrink_points (
    id         SERIAL       PRIMARY KEY,
    name_ja    VARCHAR(100) NOT NULL,
    x          DECIMAL(7,2) NOT NULL,
    y          DECIMAL(7,2) NOT NULL,
    created_at TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 第二収縮地点候補 初期マスタデータ（通常マップ12ヶ所）
INSERT INTO game.second_shrink_points (name_ja, x, y) VALUES
    ('最北西',     219.00, 233.00),
    ('中央西',     342.00, 488.00),
    ('最北東',     789.00, 248.00),
    ('最北',       583.00, 297.00),
    ('最東',       739.00, 518.00),
    ('中央東',     558.00, 552.00),
    ('最南東',     715.00, 692.00),
    ('最南',       493.00, 774.00),
    ('中央南西',   383.00, 711.00),
    ('最南西',     277.00, 728.00),
    ('最西',       211.00, 402.00),
    ('中央南',     408.00, 624.00);
