-- マスタデータ投入

-- ==============================
-- ボスデータ
-- ==============================
INSERT INTO game.bosses (id, name_ja, name_en, is_dlc, unlock_condition) VALUES
    ('gladius',  '三つ首の獣',       'Gladius',  FALSE, NULL),
    ('edere',    '喰らいつく顎',     'Edere',    FALSE, NULL),
    ('gnostor',  '知性の蟲',         'Gnostor',  FALSE, NULL),
    ('maris',    '兆し',             'Maris',    FALSE, NULL),
    ('libra',    '調律の魔物',       'Libra',    FALSE, NULL),
    ('fulgor',   '闇駆ける狩人',     'Fulgor',   FALSE, NULL),
    ('caligo',   '霧の裂け目',       'Caligo',   FALSE, NULL),
    ('nameless', '夜を象る者',       'Nameless',  FALSE, '4体撃破後'),
    ('harmonia', '英雄武器の娘たち', 'Harmonia', TRUE,  NULL),
    ('strages',  '反逆のストラゲス', 'Strages',  TRUE,  NULL);

-- ==============================
-- ボス弱点データ
-- ==============================
INSERT INTO game.boss_weaknesses (boss_id, attribute) VALUES
    ('gladius',  'holy'),
    ('edere',    'poison'),
    ('gnostor',  'fire'),
    ('maris',    'lightning'),
    ('libra',    'holy'),
    ('libra',    'fire'),
    ('libra',    'madness'),
    ('fulgor',   'lightning'),
    ('caligo',   'fire'),
    ('nameless', 'holy'),
    ('nameless', 'fire'),
    ('nameless', 'lightning'),
    ('harmonia', 'sleep'),
    ('strages',  'holy'),
    ('strages',  'fire');

-- ==============================
-- 地変データ
-- ==============================
INSERT INTO game.jihen (id, name_ja, area, center_x, center_y, is_dlc) VALUES
    ('crater',       '火口',           'north',     0.5000, 0.2000, FALSE),
    ('mountain',     '山嶺',           'northwest', 0.2500, 0.2500, FALSE),
    ('rotted_forest','腐れ森',         'southwest', 0.2500, 0.7500, FALSE),
    ('noklateo',     '隠れ都ノクラテオ','center',    0.5000, 0.5000, FALSE),
    ('great_cavern', '大空洞',         'all',       0.5000, 0.5000, TRUE);

-- ==============================
-- 収縮候補地（ダミー座標）
-- ==============================
INSERT INTO game.shrink_zones (name_ja, center_x, center_y, radius) VALUES
    ('収縮地点A',  0.1500, 0.1500, 0.1200),
    ('収縮地点B',  0.3500, 0.1500, 0.1200),
    ('収縮地点C',  0.5500, 0.1500, 0.1200),
    ('収縮地点D',  0.7500, 0.1500, 0.1200),
    ('収縮地点E',  0.1500, 0.4000, 0.1200),
    ('収縮地点F',  0.3500, 0.4000, 0.1200),
    ('収縮地点G',  0.5500, 0.4000, 0.1200),
    ('収縮地点H',  0.7500, 0.4000, 0.1200),
    ('収縮地点I',  0.1500, 0.6500, 0.1200),
    ('収縮地点J',  0.3500, 0.6500, 0.1200),
    ('収縮地点K',  0.5500, 0.6500, 0.1200),
    ('収縮地点L',  0.7500, 0.6500, 0.1200);

-- ==============================
-- スポーン地点（ダミー座標）
-- ==============================
INSERT INTO game.spawn_points (name_ja, x, y, nearby_base_type) VALUES
    ('スポーン地点1', 0.2000, 0.2000, 'campfire'),
    ('スポーン地点2', 0.5000, 0.2000, 'ruins'),
    ('スポーン地点3', 0.8000, 0.2000, 'campfire'),
    ('スポーン地点4', 0.2000, 0.5000, 'ruins'),
    ('スポーン地点5', 0.5000, 0.5000, 'campfire'),
    ('スポーン地点6', 0.8000, 0.5000, 'ruins'),
    ('スポーン地点7', 0.2000, 0.8000, 'campfire'),
    ('スポーン地点8', 0.5000, 0.8000, 'ruins');
