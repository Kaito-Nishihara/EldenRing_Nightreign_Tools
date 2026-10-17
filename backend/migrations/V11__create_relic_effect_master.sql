-- =====================================================
-- 遺物効果マスタ（ビルドシミュレーター用）
-- 通常効果と深層効果を同テーブルで管理し is_deep で区別する
-- =====================================================

-- ----------------------------
-- 効果カテゴリマスタ
-- ----------------------------
CREATE TABLE game.relic_effect_categories (
    id         VARCHAR(30) PRIMARY KEY,
    name_ja    VARCHAR(50) NOT NULL,
    sort_order SMALLINT    NOT NULL DEFAULT 0
);

INSERT INTO game.relic_effect_categories (id, name_ja, sort_order) VALUES
    ('stat',          '能力値',       0),
    ('attack',        '攻撃力',       1),
    ('cut_rate',      'カット率',     2),
    ('recovery',      '回復',         3),
    ('status_resist', '状態異常耐性', 4),
    ('arts_gauge',    'アーツゲージ', 5),
    ('starting_item', '出撃装備',     6),
    ('other',         'その他',       7);

-- ----------------------------
-- 遺物効果マスタ（通常・深層共通）
-- ----------------------------
-- calc_type:
--   multiply    … 全効果を乗算（例: 1.05 × 1.15）
--   percent_add … %値を合算して乗算（例: +4% + +6% → 1.10倍）
--   flat_add    … 固定値を合算（例: +100HP）
--   special     … コード側で個別処理
--
-- unit（relic_effect_ranks の value の単位）:
--   multiplier … 倍率そのもの（例: 1.05）
--   percent    … %ポイント（例: 4.0 → +4%）
--   flat       … 固定値
--   per_sec    … /秒
--   none       … 数値なし（special系）
-- ----------------------------
CREATE TABLE game.relic_effects (
    id           SERIAL       PRIMARY KEY,
    category_id  VARCHAR(30)  NOT NULL REFERENCES game.relic_effect_categories(id),
    name_ja      VARCHAR(200) NOT NULL,
    is_deep      BOOLEAN      NOT NULL DEFAULT FALSE,
    effect_sign  VARCHAR(10)  NOT NULL DEFAULT 'positive'
                     CHECK (effect_sign IN ('positive', 'negative')),
    calc_type    VARCHAR(20)  NOT NULL
                     CHECK (calc_type IN ('multiply', 'flat_add', 'percent_add', 'special')),
    attribute    VARCHAR(30),    -- 対象属性・能力値識別子（下記参照）
                                  -- 能力値: vigor / mind / endurance / strength / dexterity / intelligence / faith / arcane
                                  -- 攻撃: physical / magic / fire / lightning / holy / melee / skill / throw_pot / throw_knife / breath / toughness
                                  -- 耐性: poison / rot / bleed / frostbite / sleep / madness / death / all_status
    stackable    VARCHAR(15)  NOT NULL DEFAULT 'yes'
                     CHECK (stackable IN ('yes', 'no', 'conditional')),
    rank_min     SMALLINT     NOT NULL DEFAULT 0,   -- ランクなし効果は 0
    rank_max     SMALLINT     NOT NULL DEFAULT 0,   -- ランクなし効果は 0
    condition_ja TEXT,    -- 発動条件（例: HP40%未満）
    notes_ja     TEXT,    -- 備考
    created_at   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- ----------------------------
-- 効果ランク別数値
-- ----------------------------
-- ランクなし効果は rank=0 の1行のみ登録
-- has_demerit: 深層効果でこのランク以上のレベルにデメリットが付随する場合 TRUE
-- ----------------------------
CREATE TABLE game.relic_effect_ranks (
    id          SERIAL       PRIMARY KEY,
    effect_id   INTEGER      NOT NULL REFERENCES game.relic_effects(id) ON DELETE CASCADE,
    rank        SMALLINT     NOT NULL DEFAULT 0,
    value       DECIMAL(10,4),
    unit        VARCHAR(20)  NOT NULL
                    CHECK (unit IN ('multiplier', 'percent', 'flat', 'per_sec', 'none')),
    has_demerit BOOLEAN      NOT NULL DEFAULT FALSE,
    UNIQUE (effect_id, rank)
);

-- =====================================================
-- マスタデータ投入
-- =====================================================

-- ====================================================
-- [通常効果] カテゴリ: 能力値 (stat)
-- ====================================================

-- 最大HP上昇（固定値+100、重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('stat', '最大HP上昇', 'flat_add', 'no');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 100, 'flat');

-- 最大FP上昇（固定値+25、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('stat', '最大FP上昇', 'flat_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 25, 'flat');

-- 最大スタミナ上昇（固定値+10、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('stat', '最大スタミナ上昇', 'flat_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 10, 'flat');

-- 各能力値+1〜3（重ね掛け可）
DO $$
DECLARE
    v_attr  TEXT;
    v_name  TEXT;
    v_id    INTEGER;
BEGIN
    FOR v_attr, v_name IN VALUES
        ('vigor',        '生命力'),
        ('mind',         '精神力'),
        ('endurance',    '持久力'),
        ('strength',     '筋力'),
        ('dexterity',    '技量'),
        ('intelligence', '知力'),
        ('faith',        '信仰'),
        ('arcane',       '神秘')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable, rank_min, rank_max)
            VALUES ('stat', v_name || '+1〜3', 'flat_add', v_attr, 'yes', 1, 3)
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES
            (v_id, 1, 1, 'flat'),
            (v_id, 2, 2, 'flat'),
            (v_id, 3, 3, 'flat');
    END LOOP;
END $$;

-- 強靭度+1〜3（+5%/+10%/+15%、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable, rank_min, rank_max)
    VALUES ('stat', '強靭度+1〜3', 'percent_add', 'toughness', 'yes', 1, 3);
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES
    (currval('game.relic_effects_id_seq'), 1, 5,  'percent'),
    (currval('game.relic_effects_id_seq'), 2, 10, 'percent'),
    (currval('game.relic_effects_id_seq'), 3, 15, 'percent');

-- ====================================================
-- [通常効果] カテゴリ: 攻撃力 (attack)
-- ====================================================

-- 各属性攻撃力上昇+0〜2（+4%/+5%/+6%、重ね掛け可）
DO $$
DECLARE
    v_attr TEXT;
    v_name TEXT;
    v_id   INTEGER;
BEGIN
    FOR v_attr, v_name IN VALUES
        ('physical',  '物理'),
        ('magic',     '魔力'),
        ('fire',      '炎'),
        ('lightning', '雷'),
        ('holy',      '聖')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable, rank_min, rank_max)
            VALUES ('attack', v_name || '攻撃力上昇+0〜2', 'percent_add', v_attr, 'yes', 0, 2)
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES
            (v_id, 0, 4, 'percent'),
            (v_id, 1, 5, 'percent'),
            (v_id, 2, 6, 'percent');
    END LOOP;
END $$;

-- 近接攻撃力上昇（×1.05、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('attack', '近接攻撃力上昇', 'multiply', 'melee', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.05, 'multiplier');

-- 戦技攻撃力上昇（×1.15、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('attack', '戦技攻撃力上昇', 'multiply', 'skill', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.15, 'multiplier');

-- 通常攻撃の1段目強化（×1.15、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('attack', '通常攻撃の1段目強化', 'multiply', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.15, 'multiplier');

-- 致命の一撃強化（×1.17、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('attack', '致命の一撃強化', 'multiply', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.17, 'multiplier');

-- 致命の一撃強化+1（×1.24、重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('attack', '致命の一撃強化+1', 'multiply', 'no');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.24, 'multiplier');

-- ガードカウンター強化（×1.17、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('attack', 'ガードカウンター強化', 'multiply', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.17, 'multiplier');

-- 咆哮とブレス強化（×1.15、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('attack', '咆哮とブレス強化', 'multiply', 'breath', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.15, 'multiplier');

-- 投擲壺の攻撃力上昇（×1.15、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('attack', '投擲壺の攻撃力上昇', 'multiply', 'throw_pot', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.15, 'multiplier');

-- 投擲ナイフの攻撃力上昇（×1.14、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('attack', '投擲ナイフの攻撃力上昇', 'multiply', 'throw_knife', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 1.14, 'multiplier');

-- 攻撃連続時、攻撃力上昇（段階的強化、特殊処理）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('attack', '攻撃連続時、攻撃力上昇', 'special', 'conditional',
            '段階的に上昇: 1段目×1.05 → 累計×1.12 → 累計×1.22');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- ====================================================
-- [通常効果] カテゴリ: カット率 (cut_rate)
-- ====================================================

-- 物理カット率上昇（+8%、重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
    VALUES ('cut_rate', '物理カット率上昇', 'percent_add', 'physical', 'no');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 8, 'percent');

-- 各属性カット率上昇（+10%、重ね掛け可）
DO $$
DECLARE
    v_attr TEXT;
    v_name TEXT;
    v_id   INTEGER;
BEGIN
    FOR v_attr, v_name IN VALUES
        ('magic',     '魔力'),
        ('fire',      '炎'),
        ('lightning', '雷'),
        ('holy',      '聖')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
            VALUES ('cut_rate', v_name || 'カット率上昇', 'percent_add', v_attr, 'yes')
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
            VALUES (v_id, 0, 10, 'percent');
    END LOOP;
END $$;

-- HP低下時、カット率上昇（+15%、条件付き）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, condition_ja)
    VALUES ('cut_rate', 'HP低下時、カット率上昇', 'percent_add', 'conditional', 'HP40%未満で発動');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 15, 'percent');

-- ダメージで吹き飛ばされた時、強靭度とカット率上昇（特殊）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('cut_rate', 'ダメージで吹き飛ばされた時、強靭度とカット率上昇', 'special', 'no',
            'カット率+20%・強靭度+20%、20秒持続');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- ====================================================
-- [通常効果] カテゴリ: 状態異常耐性 (status_resist)
-- ====================================================

DO $$
DECLARE
    v_attr TEXT;
    v_name TEXT;
    v_id   INTEGER;
BEGIN
    FOR v_attr, v_name IN VALUES
        ('poison',    '毒'),
        ('rot',       '腐敗'),
        ('bleed',     '出血'),
        ('frostbite', '冷気'),
        ('sleep',     '睡眠'),
        ('madness',   '発狂'),
        ('death',     '抗死')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, calc_type, attribute, stackable)
            VALUES ('status_resist', v_name || '耐性上昇', 'flat_add', v_attr, 'yes')
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
            VALUES (v_id, 0, 75, 'flat');
    END LOOP;
END $$;

-- ====================================================
-- [通常効果] カテゴリ: 回復 (recovery)
-- ====================================================

-- HP持続回復（+2/秒、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('recovery', 'HP持続回復', 'flat_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 2, 'per_sec');

-- HP低下時、周囲の味方を含めHPをゆっくり回復（重ね掛け不可、特殊）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, condition_ja, notes_ja)
    VALUES ('recovery', 'HP低下時、周囲の味方を含めHPをゆっくり回復', 'special', 'no',
            'HP50%以下で発動', '最大HP0.5%+1/秒、50秒持続');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- ガード成功時、HP回復（固定15、重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('recovery', 'ガード成功時、HP回復', 'flat_add', 'no', 'クールタイムあり');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 15, 'flat');

-- 刺突カウンター発生時、HP回復（最大HPの2.5%、重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable)
    VALUES ('recovery', '刺突カウンター発生時、HP回復', 'percent_add', 'no');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 2.5, 'percent');

-- 発狂状態になると、FP持続回復（特殊・条件付き）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, condition_ja, notes_ja)
    VALUES ('recovery', '発狂状態になると、FP持続回復', 'special', 'conditional',
            '発狂状態中のみ', '+2/秒、25秒間（合計+50）');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 2, 'per_sec');

-- 攻撃命中時、スタミナ回復（特殊）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('recovery', '攻撃命中時、スタミナ回復', 'special', 'no', '1ヒットにつき約2〜3回復');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- 致命の一撃で、スタミナ回復速度上昇（特殊）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('recovery', '致命の一撃で、スタミナ回復速度上昇', 'special', 'no',
            '+15%、15秒間');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- リゲイン有効化（特殊・重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('recovery', 'リゲイン有効化', 'special', 'no',
            'ダメージ後の攻撃で一部HP回復');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- ====================================================
-- [通常効果] カテゴリ: アーツゲージ (arts_gauge)
-- ====================================================

-- スキルクールタイム軽減+1〜3（5%/7.5%/10%、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, rank_min, rank_max)
    VALUES ('arts_gauge', 'スキルクールタイム軽減+1〜3', 'percent_add', 'yes', 1, 3);
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES
    (currval('game.relic_effects_id_seq'), 1, 5,   'percent'),
    (currval('game.relic_effects_id_seq'), 2, 7.5, 'percent'),
    (currval('game.relic_effects_id_seq'), 3, 10,  'percent');

-- アーツゲージ自然蓄積+1〜3（5%/7.5%/10%、重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, rank_min, rank_max)
    VALUES ('arts_gauge', 'アーツゲージ自然蓄積+1〜3', 'percent_add', 'yes', 1, 3);
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES
    (currval('game.relic_effects_id_seq'), 1, 5,   'percent'),
    (currval('game.relic_effects_id_seq'), 2, 7.5, 'percent'),
    (currval('game.relic_effects_id_seq'), 3, 10,  'percent');

-- 敵撃破時、アーツゲージ増加（重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('arts_gauge', '敵撃破時、アーツゲージ増加', 'special', 'no', '約5%増加');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- 致命一撃でアーツゲージ増加（重ね掛け不可）
INSERT INTO game.relic_effects (category_id, name_ja, calc_type, stackable, notes_ja)
    VALUES ('arts_gauge', '致命の一撃でアーツゲージ増加', 'special', 'no', '約5%増加');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, NULL, 'none');

-- ====================================================
-- [深層効果] メリット（is_deep=TRUE, effect_sign='positive'）
-- ====================================================

-- 最大HP+10%（重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, stackable)
    VALUES ('stat', '最大HP上昇+10%', TRUE, 'percent_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 10, 'percent', TRUE);

-- 最大FP+15%（重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, stackable)
    VALUES ('stat', '最大FP上昇+15%', TRUE, 'percent_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 15, 'percent', TRUE);

-- 最大スタミナ+12%（重ね掛け可）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, stackable)
    VALUES ('stat', '最大スタミナ上昇+12%', TRUE, 'percent_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 12, 'percent', TRUE);

-- 物理攻撃力上昇+0〜4（深層・+3以上でデメリット）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, attribute, stackable, rank_min, rank_max)
    VALUES ('attack', '物理攻撃力上昇+0〜4（深層）', TRUE, 'percent_add', 'physical', 'yes', 0, 4);
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit) VALUES
    (currval('game.relic_effects_id_seq'), 0, 4,    'percent', FALSE),
    (currval('game.relic_effects_id_seq'), 1, 5,    'percent', FALSE),
    (currval('game.relic_effects_id_seq'), 2, 6,    'percent', FALSE),
    (currval('game.relic_effects_id_seq'), 3, 10.5, 'percent', TRUE),
    (currval('game.relic_effects_id_seq'), 4, 12,   'percent', TRUE);

-- 各属性攻撃力上昇+0〜2（深層・+1以上でデメリット）
DO $$
DECLARE
    v_attr TEXT;
    v_name TEXT;
    v_id   INTEGER;
BEGIN
    FOR v_attr, v_name IN VALUES
        ('magic',     '魔力'),
        ('fire',      '炎'),
        ('lightning', '雷'),
        ('holy',      '聖')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, attribute, stackable, rank_min, rank_max)
            VALUES ('attack', v_name || '攻撃力上昇+0〜2（深層）', TRUE, 'percent_add', v_attr, 'yes', 0, 2)
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit) VALUES
            (v_id, 0, 5,  'percent', FALSE),
            (v_id, 1, 8,  'percent', TRUE),
            (v_id, 2, 10, 'percent', TRUE);
    END LOOP;
END $$;

-- ガードカウンター強化+0〜2（深層・+1以上でデメリット）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, calc_type, stackable, rank_min, rank_max)
    VALUES ('attack', 'ガードカウンター強化+0〜2（深層）', TRUE, 'multiply', 'yes', 0, 2);
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit, has_demerit) VALUES
    (currval('game.relic_effects_id_seq'), 0, 1.17, 'multiplier', FALSE),
    (currval('game.relic_effects_id_seq'), 1, 1.25, 'multiplier', TRUE),
    (currval('game.relic_effects_id_seq'), 2, 1.29, 'multiplier', TRUE);

-- ====================================================
-- [深層効果] デメリット（is_deep=TRUE, effect_sign='negative'）
-- ====================================================

-- HP持続減少（-2/秒）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, stackable)
    VALUES ('recovery', 'HP持続減少', TRUE, 'negative', 'flat_add', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, -2, 'per_sec');

-- すべての状態異常耐性低下（-80）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, attribute, stackable)
    VALUES ('status_resist', 'すべての状態異常耐性低下', TRUE, 'negative', 'flat_add', 'all_status', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, -80, 'flat');

-- 能力値低下（複合ペア）
DO $$
DECLARE
    v_attr1 TEXT;
    v_name1 TEXT;
    v_attr2 TEXT;
    v_name2 TEXT;
    v_id    INTEGER;
BEGIN
    FOR v_attr1, v_name1, v_attr2, v_name2 IN VALUES
        ('vigor',        '生命力', 'arcane',       '神秘'),
        ('strength',     '筋力',   'intelligence', '知力'),
        ('dexterity',    '技量',   'faith',        '信仰'),
        ('intelligence', '知力',   'dexterity',    '技量'),
        ('faith',        '信仰',   'strength',     '筋力')
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, attribute, stackable)
            VALUES ('stat', v_name1 || 'と' || v_name2 || 'が低下', TRUE, 'negative', 'flat_add',
                    v_attr1 || '_' || v_attr2, 'yes')
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
            VALUES (v_id, 0, -3, 'flat');
    END LOOP;
END $$;

-- 取得ルーン減少（×0.9）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, stackable)
    VALUES ('other', '取得ルーン減少', TRUE, 'negative', 'multiply', 'yes');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, 0.9, 'multiplier');

-- 被ダメージ時、状態異常蓄積（複数属性）
DO $$
DECLARE
    v_attr TEXT;
    v_name TEXT;
    v_val  DECIMAL;
    v_id   INTEGER;
BEGIN
    FOR v_attr, v_name, v_val IN VALUES
        ('poison',    '毒',   65::DECIMAL),
        ('rot',       '腐敗', 55::DECIMAL),
        ('bleed',     '出血', 55::DECIMAL),
        ('frostbite', '冷気', 55::DECIMAL)
    LOOP
        INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, attribute, stackable)
            VALUES ('status_resist', '被ダメージ時、' || v_name || 'を蓄積',
                    TRUE, 'negative', 'special', v_attr, 'yes')
            RETURNING id INTO v_id;
        INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
            VALUES (v_id, 0, v_val, 'flat');
    END LOOP;
END $$;

-- 回避連続時、カット率低下（-35%、15秒持続）
INSERT INTO game.relic_effects (category_id, name_ja, is_deep, effect_sign, calc_type, stackable, notes_ja)
    VALUES ('cut_rate', '回避連続時、カット率低下', TRUE, 'negative', 'percent_add', 'conditional',
            '-35%、15秒持続');
INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit)
    VALUES (currval('game.relic_effects_id_seq'), 0, -35, 'percent');
