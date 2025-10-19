# 데이터 구조 정의

## 핵심 인터페이스

### StatDefinition (스탯 정의 메타데이터)

```csharp
using System;
using System.Collections.Generic;

public class StatDefinition
{
    public string Id { get; set; }                      // "attack", "crit_rate", "lifesteal"
    public Dictionary<string, string> DisplayName { get; set; }  // {"en": "Attack", "ko": "공격력"}
    public string StatType { get; set; }                // "base", "combat", "special"
    public string DataType { get; set; }                // "int", "float", "percent"
    public float DefaultValue { get; set; }
    public float? MinValue { get; set; }
    public float? MaxValue { get; set; }
    public int Version { get; set; }                    // 스탯 스키마 버전
    public DateTime CreatedAt { get; set; }
}

// 클라이언트 동기화 응답
public class StatDefinitionsResponse
{
    public int Version { get; set; }                    // 최신 버전
    public List<StatDefinition> Stats { get; set; }
    public bool UpdateRequired { get; set; }            // 클라이언트 업데이트 필요 여부
}
```

### Hero (영웅)

```csharp
using System;
using System.Collections.Generic;

public class Hero
{
    // 식별
    public string Id { get; set; }              // 고유 ID
    public string TemplateId { get; set; }      // 영웅 종류
    public string Type { get; set; }            // 영웅 타입(공격, 방어, 지원)

    // 등급
    public int StarGrade { get; set; }          // 1 | 2 | 3 | 4 | 5 | 6

    // 기본 정보
    public string Name { get; set; }
    public Element Element { get; set; }

    // 레벨 & 경험치
    public int Level { get; set; }
    public int CurrentExp { get; set; }

    // 스탯
    public HeroStats Stats { get; set; }

    // 성장
    public Stats BaseStats { get; set; }        // 레벨 1 기준
    public Stats GrowthRates { get; set; }      // 레벨당 증가량
    public int Rebirths { get; set; }           // 환생 횟수, 환생 시 growthRates증가, 기존 레벨당 증가량 격차만큼 능력치 보정

    // 효과 (★4+)
    public List<Effect>? UniqueEffects { get; set; }

    // 상태
    public bool IsDead { get; set; }
    public bool IsInParty { get; set; }
    public EquippedItems EquippedItems { get; set; }
}

public class HeroStats
{
    // 핵심 스탯 (DB 컬럼, 쿼리 최적화)
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }

    // 확장 스탯 (JSONB, 동적 추가 가능)
    public Dictionary<string, float> ExtendedStats { get; set; }

    // 헬퍼 메서드: 스탯 통합 조회
    public float GetStat(string statId, float defaultValue = 0f)
    {
        return statId switch
        {
            "hp" => BaseHp,
            "attack" => BaseAttack,
            "defense" => BaseDefense,
            _ => ExtendedStats?.GetValueOrDefault(statId, defaultValue) ?? defaultValue
        };
    }

    // 편의 속성 (현재 스탯)
    public int CurrentHp { get; set; }     // 현재 HP (전투 중 변동)
    public int MaxHp => BaseHp;
}

// 장비 보너스
public class StatBonuses
{
    public Dictionary<string, int> Flat { get; set; } = new();      // +N
    public Dictionary<string, float> Percent { get; set; } = new(); // +N%
}

// 계산된 최종 스탯
public class ComputedStats
{
    public int Hp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public float CritRate { get; set; }
    public float CritDamage { get; set; }
    public float Evasion { get; set; }
    public float ArmorPenetration { get; set; }
    public float EvasionPierce { get; set; }
    public int CombatPower { get; set; }
}

// 전투 스냅샷
public class HeroCombatSnapshot
{
    public string HeroId { get; set; }

    // 전투 시작 시점 스탯 (불변)
    public int MaxHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public float CritRate { get; set; }
    public float CritDamage { get; set; }
    public float Evasion { get; set; }

    // 전투 중 변동 값
    public int CurrentHp { get; set; }
    public bool IsDead { get; set; }

    // 버프/디버프 (전투 중만)
    public List<CombatModifier> ActiveModifiers { get; set; }
}

public class CombatModifier
{
    public string EffectId { get; set; }
    public string StatId { get; set; }
    public float Value { get; set; }
    public int RemainingRounds { get; set; }
}

public class EquippedItems
{
    public string? Weapon { get; set; }         // equipment ID
    public string? Armor { get; set; }
    public string? Accessory1 { get; set; }
    public string? Accessory2 { get; set; }
}

public enum Element
{
    Water,
    Fire,
    Earth,
    None
}

public class Stats // critical_rate, critical_damage, block_rate는 성장치와 영향 없음
{
    public float Hp { get; set; }
    public float Attack { get; set; }
    public float Defense { get; set; }
}

// 고유 효과 정의 (마스터 데이터)
public class UniqueEffectDefinition
{
    public int EffectId { get; set; }           // 고유 효과 인덱스
    public string EffectName { get; set; }
    public string Description { get; set; }
    public EffectCategory Category { get; set; }
    public EffectValueType ValueType { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}

// 영웅 고유 효과 (인스턴스)
public class HeroUniqueEffect
{
    public string HeroTemplateId { get; set; }
    public int EffectId { get; set; }           // UniqueEffectDefinition 참조
    public float EffectValue { get; set; }      // 고정된 수치
    public int StarGradeRequired { get; set; }
}

public enum EffectCategory
{
    Offensive,    // 공격형
    Defensive,    // 방어형
    Utility,      // 유틸리티
    Debuff,       // 디버프
    Aura          // 오라 (아군/적군 버프/디버프)
}

public enum EffectValueType
{
    FixedValue,   // 고정 수치
    Percentage    // 비율
}
```

### Equipment (장비)

```csharp
using System;
using System.Collections.Generic;

public class Equipment
{
    // 식별
    public string Id { get; set; }
    public string TemplateId { get; set; }

    // 기본 정보
    public string Name { get; set; }
    public Grade Grade { get; set; }
    public EquipmentType Type { get; set; }
    public int RequiredLevel { get; set; }

    // 스탯
    public BaseStat BaseStat { get; set; }

    // 옵션
    public List<EquipmentOption> Options { get; set; }

    // 상태
    public bool Equipped { get; set; }
    public string? EquippedBy { get; set; }     // hero ID
}

public class BaseStat
{
    public string Type { get; set; }            // "attack" | "defense" | "hp"
    public float Value { get; set; }
}

public enum Grade
{
    C,
    UC,
    R,
    H,
    L
}

public enum EquipmentType
{
    Weapon,
    Armor,
    Ring,
    Neckless,
    Belt
}

public class EquipmentOption
{
    public string Type { get; set; }            // "offensive" | "defensive" | "utility"
    public string Stat { get; set; }            // "추가 공격", "드랍률" 등
    public float Value { get; set; }
}
```

### Combat (전투)

```csharp
using System;
using System.Collections.Generic;

public class Combat
{
    public string CombatId { get; set; }

    // 참가자
    public List<Hero> PlayerParty { get; set; }
    public List<Enemy> EnemyParty { get; set; }

    // 배치
    public GridPositions GridPositions { get; set; }

    // 진행 상태
    public int CurrentRound { get; set; }
    public int MaxRound { get; set; }
    public bool IsFinished { get; set; }

    // 기록
    public List<CombatAction> Log { get; set; }
}

public class GridPositions
{
    public Hero?[] Player { get; set; }         // length 9
    public Enemy?[] Enemy { get; set; }         // length 9
}

public class CombatAction
{
    public int Round { get; set; }
    public string ActorId { get; set; }
    public string TargetId { get; set; }
    public float Damage { get; set; }
    public float ElementBonus { get; set; }
    public bool TargetDied { get; set; }
}

public class CombatResult
{
    public bool Victory { get; set; }
    public List<Hero> Survivors { get; set; }
    public List<Hero> Dead { get; set; }
    public Dictionary<string, int> ExpGained { get; set; }  // heroId -> exp
    public List<Equipment> ItemsDropped { get; set; }
    public int GoldGained { get; set; }
    public int RoundsElapsed { get; set; }
}
```

### Territory (영토)

```csharp
using System;
using System.Collections.Generic;

public class Territory
{
    // 소유 타일
    public HashSet<string> OwnedTiles { get; set; }   // "x,y" 형식

    // 깃발
    public List<Flag> Flags { get; set; }

    // 통계
    public int TotalArea { get; set; }
    public List<string> ConnectedTowns { get; set; }  // town IDs
}

public class Flag
{
    public string Id { get; set; }
    public int Size { get; set; }               // 3 | 5 | 7 | 9
    public Position Position { get; set; }
    public long PlacedAt { get; set; }          // timestamp
}

public class Position
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

### World (월드)

```csharp
using System;
using System.Collections.Generic;

public class WorldMap
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Tile[][] Tiles { get; set; }

    public List<Region> Regions { get; set; }
    public List<Town> Towns { get; set; }
    public List<Fortress> Fortresses { get; set; }
}

public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    public TileType Type { get; set; }
    public List<string> Tags { get; set; }      // ["초원_풀숲", "밤"]

    public bool IsWalkable { get; set; }
    public float SpeedModifier { get; set; }

    public FogState FogState { get; set; }
    public bool OwnedByPlayer { get; set; }
}

public enum TileType
{
    Grass,
    Road,
    Forest,
    Swamp,
    Mountain,
    Water,
    Lava
}

public enum FogState
{
    Unexplored,
    Explored,
    Owned
}
```

### Settlement (정착지)

```csharp
using System;
using System.Collections.Generic;

public class Town
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Position Position { get; set; }
    public int Size { get; set; }               // 기본 영토 크기

    public bool Owned { get; set; }
    public long? ConqueredAt { get; set; }

    public string Region { get; set; }
    public int Difficulty { get; set; }

    public TownFeatures Features { get; set; }
}

public class TownFeatures
{
    public bool Recovery { get; set; }          // 회복 기능
    public Shop? Shop { get; set; }
}

public class Fortress
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Position Position { get; set; }
    public int Size { get; set; }

    public bool Unlocked { get; set; }
    public long? UnlockedAt { get; set; }

    public FirstBattle FirstBattle { get; set; }

    public FortressFeatures Features { get; set; }
}

public class FirstBattle
{
    public List<Enemy> Enemies { get; set; }
    public List<Equipment> Rewards { get; set; }
}

public class FortressFeatures
{
    public Shop Shop { get; set; }
    public Exchange Exchange { get; set; }
    public CraftingStation Crafting { get; set; }
}
```

### Player (플레이어)

```csharp
using System;
using System.Collections.Generic;

public class Player
{
    // 기본 정보
    public string Name { get; set; }

    // 위치
    public Position Position { get; set; }
    public CampingState? Camping { get; set; }

    // 소유
    public List<Hero> Heroes { get; set; }
    public List<Equipment> Equipment { get; set; }
    public int Gold { get; set; }

    // 파티
    public List<Hero> Party { get; set; }       // 최대 4명

    // 진행도
    public Territory Territory { get; set; }
    public Rank Rank { get; set; }
    public HashSet<string> OwnedTowns { get; set; }     // town IDs
    public HashSet<string> UnlockedFortresses { get; set; }

    // 메타
    public int PlayTime { get; set; }           // 초
}

public class CampingState
{
    public Position Position { get; set; }
    public long StartTime { get; set; }
    public float HealingRate { get; set; }
    public bool IsActive { get; set; }
}
```

### Progression (진행)

```csharp
using System;
using System.Collections.Generic;

public class Rank
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }

    public RankRequirement Requirement { get; set; }

    public RankBenefits Benefits { get; set; }
}

public class RankRequirement
{
    public int Area { get; set; }
    public int? Towns { get; set; }
    public int? Fortresses { get; set; }
}

public class RankBenefits
{
    public float? ExpBonus { get; set; }
    public float? DropRateBonus { get; set; }
    public float? CampingEfficiency { get; set; }
    public float? MovementSpeed { get; set; }
    public float? FortressDiscount { get; set; }
    public float? UpgradeCostReduction { get; set; }
    public float? BossRewardMultiplier { get; set; }
    public float? RareDropWeight { get; set; }
    public int? SpecialTokens { get; set; }
}
```

## 헬퍼 타입

```csharp
using System;
using System.Collections.Generic;

// 이벤트
public abstract class GameEvent
{
    public string Type { get; set; }
}

public class CombatStartEvent : GameEvent
{
    public CombatStartData Data { get; set; }
}

public class CombatStartData
{
    public List<Enemy> Enemies { get; set; }
}

public class CombatEndEvent : GameEvent
{
    public CombatEndData Data { get; set; }
}

public class CombatEndData
{
    public CombatResult Result { get; set; }
}

public class TownConqueredEvent : GameEvent
{
    public TownConqueredData Data { get; set; }
}

public class TownConqueredData
{
    public Town Town { get; set; }
}

public class RankUpEvent : GameEvent
{
    public RankUpData Data { get; set; }
}

public class RankUpData
{
    public Rank NewRank { get; set; }
}

public class LevelUpEvent : GameEvent
{
    public LevelUpData Data { get; set; }
}

public class LevelUpData
{
    public Hero Hero { get; set; }
}

public class FlagPlacedEvent : GameEvent
{
    public FlagPlacedData Data { get; set; }
}

public class FlagPlacedData
{
    public Flag Flag { get; set; }
}
```

## 데이터베이스 스키마

### PostgreSQL 스키마 예시

```sql
-- 스탯 정의 테이블
CREATE TABLE stat_definitions (
    id VARCHAR(50) PRIMARY KEY,
    display_name JSONB NOT NULL,
    stat_type VARCHAR(20) NOT NULL,
    data_type VARCHAR(20) DEFAULT 'float',
    default_value FLOAT DEFAULT 0,
    min_value FLOAT,
    max_value FLOAT,
    version INT DEFAULT 1,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 영웅 테이블
CREATE TABLE heroes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    template_id VARCHAR(100) NOT NULL,
    name VARCHAR(100),
    star_grade INT CHECK (star_grade BETWEEN 1 AND 6),
    level INT DEFAULT 1,
    current_exp BIGINT DEFAULT 0,  -- 누적 경험치 (long)
    current_hp INT NOT NULL,       -- 필드/전투 중 HP

    -- 핵심 스탯 (쿼리 최적화)
    base_hp INT NOT NULL DEFAULT 100,
    base_attack INT NOT NULL DEFAULT 10,
    base_defense INT NOT NULL DEFAULT 5,

    -- 확장 스탯 (동적)
    extended_stats JSONB DEFAULT '{
        "crit_rate": 0.10,
        "crit_damage": 1.0,
        "evasion": 0.05,
        "armor_penetration": 0.0,
        "evasion_pierce": 0.0,
        "exp_bonus": 0.0,
        "gold_bonus": 0.0,
        "drop_rate_bonus": 0.0
    }'::jsonb,

    -- 성장 스탯
    growth_rates JSONB NOT NULL,
    rebirths INT DEFAULT 0,

    -- 장비 (ID만 저장, Base Stats와 독립)
    equipped_weapon UUID REFERENCES equipment(id),
    equipped_armor UUID REFERENCES equipment(id),
    equipped_accessory1 UUID REFERENCES equipment(id),
    equipped_accessory2 UUID REFERENCES equipment(id),

    -- 상태
    is_dead BOOLEAN DEFAULT FALSE,
    is_in_party BOOLEAN DEFAULT FALSE,

    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- 고유 효과 정의 테이블 (마스터 데이터)
CREATE TABLE unique_effect_definitions (
    effect_id SERIAL PRIMARY KEY,
    effect_name VARCHAR(100) NOT NULL,
    effect_category VARCHAR(20) NOT NULL,  -- offensive, defensive, utility, debuff, aura
    description TEXT,
    value_type VARCHAR(20) NOT NULL,       -- fixed_value, percentage
    min_value FLOAT,
    max_value FLOAT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 영웅 템플릿별 고유 효과
CREATE TABLE hero_unique_effects (
    hero_template_id VARCHAR(100) NOT NULL,
    effect_id INT REFERENCES unique_effect_definitions(effect_id),
    effect_value FLOAT NOT NULL,           -- 고정된 수치
    star_grade_required INT DEFAULT 4,
    PRIMARY KEY (hero_template_id, effect_id)
);

-- JSONB 인덱스 (성능 최적화)
CREATE INDEX idx_heroes_extended_stats ON heroes USING GIN (extended_stats);
CREATE INDEX idx_heroes_user_id ON heroes (user_id);
CREATE INDEX idx_heroes_template_id ON heroes (template_id);

-- 스탯 정의 초기 데이터
INSERT INTO stat_definitions (id, display_name, stat_type, data_type, default_value, max_value) VALUES
('hp', '{"en":"HP","ko":"체력"}', 'base', 'int', 100, NULL),
('attack', '{"en":"Attack","ko":"공격력"}', 'base', 'int', 10, NULL),
('defense', '{"en":"Defense","ko":"방어력"}', 'base', 'int', 5, NULL),
('crit_rate', '{"en":"Crit Rate","ko":"크리티컬 확률"}', 'combat', 'float', 0.10, 1.0),
('crit_damage', '{"en":"Crit Damage","ko":"크리티컬 데미지"}', 'combat', 'float', 1.0, NULL),
('evasion', '{"en":"Evasion","ko":"회피율"}', 'combat', 'float', 0.05, 1.0),
('armor_penetration', '{"en":"Armor Penetration","ko":"방어도 무시"}', 'special', 'float', 0.0, 1.0),
('evasion_pierce', '{"en":"Evasion Pierce","ko":"회피 무시"}', 'special', 'float', 0.0, 1.0),
('exp_bonus', '{"en":"EXP Bonus","ko":"경험치 획득률"}', 'utility', 'float', 0.0, NULL),
('gold_bonus', '{"en":"Gold Bonus","ko":"골드 획득률"}', 'utility', 'float', 0.0, NULL),
('drop_rate_bonus', '{"en":"Drop Rate Bonus","ko":"드랍률"}', 'utility', 'float', 0.0, NULL);

-- 고유 효과 초기 데이터 예시
INSERT INTO unique_effect_definitions (effect_id, effect_name, effect_category, value_type, min_value, max_value) VALUES
(1, '강력한 일격', 'offensive', 'percentage', 0.10, 0.25),
(2, '받는 피해 감소', 'defensive', 'percentage', 0.08, 0.20),
(3, '흡혈', 'defensive', 'percentage', 0.10, 0.25),
(4, '반사 피해', 'defensive', 'percentage', 0.15, 0.35),
(5, '아군 전체 공격력 증가', 'aura', 'percentage', 0.08, 0.20),
(6, '적 전체 방어력 감소', 'aura', 'percentage', 0.10, 0.25);
```

## 데이터 검증

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

// 예시: 영웅 생성 검증
public static bool ValidateHero(Hero hero)
{
    if (hero.StarGrade < 1 || hero.StarGrade > 6) return false;
    if (hero.Level < 1) return false;
    if (hero.Stats.BaseHp < 0) return false;
    return true;
}

// 파티 검증
public static bool ValidateParty(List<Hero> party)
{
    if (party.Count > 4) return false;
    var uniqueIds = new HashSet<string>(party.Select(h => h.Id));
    if (uniqueIds.Count != party.Count) return false;  // 중복 불가
    return true;
}

// 스탯 정의 검증
public static bool ValidateStatDefinition(StatDefinition stat)
{
    if (string.IsNullOrEmpty(stat.Id)) return false;
    if (stat.DisplayName == null || !stat.DisplayName.Any()) return false;
    if (stat.MinValue.HasValue && stat.MaxValue.HasValue)
    {
        if (stat.MinValue > stat.MaxValue) return false;
    }
    return true;
}
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
