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
    public string Type { get; set; }            // "offensive" | "defensive" | "balanced"

    // 등급 & 성장
    public int StarGrade { get; set; }          // 1 | 2 | 3 | 4 | 5 | 6 (획득 시 결정)
    public int BaseGrowthRate { get; set; }     // 성장치: 1★=11, 2★=23, 3★=34, 4★=44, 5★=53, 6★=61
    public float GrowthRateBonus { get; set; }  // 환생 보너스: sacrifice_level × 0.05
    public int Rebirths { get; set; }           // 환생 횟수

    // 기본 정보
    public string Name { get; set; }
    public Element Element { get; set; }

    // 레벨 & 경험치
    public int Level { get; set; }
    public long CurrentExp { get; set; }        // 누적 경험치 (레벨 증가 시 감소하지 않음)

    // 스탯
    public HeroStats Stats { get; set; }

    // 성장 스탯 (레벨 1 기준)
    public Stats BaseStats { get; set; }        // 타입별 기본 스탯 (Lv1)
    public Stats GrowthRates { get; set; }      // 레벨당 증가량 공식 참조

    // 효과 (★4+)
    public List<HeroUniqueEffect>? UniqueEffects { get; set; }

    // 상태
    public bool IsDead { get; set; }
    public bool IsInParty { get; set; }
    public EquippedItems EquippedItems { get; set; }
}

// 영웅 템플릿 (마스터 데이터)
public class HeroTemplate
{
    public string TemplateId { get; set; }      // "warrior_001"
    public string Name { get; set; }
    public string Type { get; set; }            // "offensive" | "defensive" | "balanced"
    public Element Element { get; set; }

    // 레벨 1 기본 스탯
    public BaseStatsTemplate BaseStats { get; set; }

    // 고유 효과 (★4+만 가짐)
    public List<int>? UniqueEffectIds { get; set; }  // UniqueEffectDefinition 참조
}

public class BaseStatsTemplate
{
    public int Hp { get; set; }                 // Lv1 기준 HP
    public Dictionary<int, int> AttackByStarGrade { get; set; }  // 1★~6★: 공격력
    public Dictionary<int, int> DefenseByStarGrade { get; set; } // 1★~6★: 방어력
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
    public EffectTrigger Trigger { get; set; }  // 발동 시점
    public EffectValueType ValueType { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public int Duration { get; set; }           // -1: 영구, 0: 즉시, N: N라운드
}

// 영웅 고유 효과 (인스턴스)
public class HeroUniqueEffect
{
    public string HeroTemplateId { get; set; }
    public int EffectId { get; set; }           // UniqueEffectDefinition 참조
    public float EffectValue { get; set; }      // 고정된 수치
    public int StarGradeRequired { get; set; }  // 4 이상
}

public enum EffectCategory
{
    Offensive,    // 공격형
    Defensive,    // 방어형
    Utility,      // 유틸리티
    Debuff,       // 디버프
    Aura          // 오라 (아군/적군 버프/디버프)
}

public enum EffectTrigger
{
    COMBAT_START,      // 전투 시작 시 (1회)
    ROUND_START,       // 라운드 시작 시
    MY_TURN,           // 본인 턴 시작 시
    FIRST_ATTACK,      // 첫 공격 시 (전투당 1회)
    ON_ATTACK,         // 공격 시 (데미지 계산 전)
    ON_HIT,            // 공격 성공 시 (데미지 계산 후)
    ON_CRIT,           // 크리티컬 발생 시
    ON_DAMAGED,        // 피해를 받을 시
    ON_EVADE,          // 회피 성공 시
    ON_KILL,           // 적 처치 시
    ALLY_DAMAGED,      // 아군 피해 시
    ALLY_DEAD,         // 아군 사망 시
    ON_DEATH,          // 본인 사망 시
    ALWAYS             // 전투 중 지속 (패시브)
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
    public string Name { get; set; }            // "강력한 빛나는 검 of 행운"
    public string BaseName { get; set; }        // "빛나는 검"
    public Grade Grade { get; set; }
    public EquipmentType Type { get; set; }
    public int EquipmentLevel { get; set; }     // 장비 레벨 (착용 레벨)

    // 스탯
    public BaseStat BaseStat { get; set; }      // 기본 스탯 (레벨 & 등급 기반)

    // 수식어 옵션 (Prefix/Suffix)
    public EquipmentModifier? Prefix { get; set; }
    public EquipmentModifier? Suffix { get; set; }

    // 상태
    public bool Equipped { get; set; }
    public string? EquippedBy { get; set; }     // hero ID
}

public class BaseStat
{
    public string Type { get; set; }            // "attack" | "defense" | "hp" | "crit_rate" | "evasion"
    public float Value { get; set; }            // (equipment_level × grade_multiplier) + grade_base
}

// 장비 수식어 (Prefix/Suffix)
public class EquipmentModifier
{
    public int ModifierId { get; set; }         // EquipmentModifierDefinition 참조
    public string Name { get; set; }            // "강력한", "행운"
    public string Stat { get; set; }            // "attack", "drop_rate_bonus", etc.
    public ModifierType ModifierType { get; set; }  // Flat | Percent
    public float Value { get; set; }            // 계산된 값 (±25% 범위)
}

// 수식어 정의 (마스터 데이터)
public class EquipmentModifierDefinition
{
    public int ModifierId { get; set; }
    public string Name { get; set; }            // "강력한", "맹공의", "행운"
    public ModifierPosition Position { get; set; }  // Prefix | Suffix
    public string Stat { get; set; }            // "attack", "crit_rate", "drop_rate_bonus"
    public ModifierType ModifierType { get; set; }  // Flat | Percent
    public float BaseRatio { get; set; }        // 기본 스탯 대비 비율 (예: 0.5)
}

public enum ModifierPosition
{
    Prefix,
    Suffix
}

public enum ModifierType
{
    Flat,       // +N (절대값)
    Percent     // +N% (비율)
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
```

### Monster (몬스터)

```csharp
using System;
using System.Collections.Generic;

// 몬스터 템플릿 (마스터 데이터)
public class MonsterTemplate
{
    public string MonsterId { get; set; }       // "slime", "wolf", "boss_slime"
    public string Name { get; set; }
    public string Type { get; set; }            // "normal" | "boss"
    public Element Element { get; set; }

    // 기본 스탯 (Lv1 기준)
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }

    // 성장치
    public int BaseGrowthRate { get; set; }     // 기본 성장치 (예: 슬라임 20, 늑대 30)

    // 드랍 테이블
    public List<DropTableEntry> DropTable { get; set; }
}

// 몬스터 인스턴스 (조우 시 생성)
public class Enemy
{
    public string Id { get; set; }              // 인스턴스 ID
    public string TemplateId { get; set; }      // MonsterTemplate 참조
    public string Name { get; set; }
    public Element Element { get; set; }

    // 레벨 & 성장치
    public int Level { get; set; }
    public int GrowthRate { get; set; }         // 조우 시 base ±10% 랜덤

    // 전투 스탯 (조우 시 계산)
    public int MaxHp { get; set; }              // base_hp + level × (100+growth_rate)/100
    public int CurrentHp { get; set; }
    public int Attack { get; set; }             // base_attack + level × (100+growth_rate)/100
    public int Defense { get; set; }            // base_defense + level × (100+growth_rate)/100

    // 상태
    public bool IsDead { get; set; }
}

public class DropTableEntry
{
    public string ItemType { get; set; }        // "equipment" | "gold" | "material"
    public int Weight { get; set; }             // 가중치
    public int MinAmount { get; set; }
    public int MaxAmount { get; set; }
}
```

### Combat (전투)

```csharp
using System;
using System.Collections.Generic;

public class Combat
{
    public string CombatId { get; set; }
    public CombatType CombatType { get; set; }  // Normal | Fortress

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

public enum CombatType
{
    Normal,     // 최대 4인 파티
    Fortress    // 최대 6인 파티 (요새 보스)
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

    public FortressType FortressType { get; set; }  // Route | Side
    public int Difficulty { get; set; }

    public bool Unlocked { get; set; }
    public long? UnlockedAt { get; set; }

    public FirstBattle FirstBattle { get; set; }

    public FortressFeatures Features { get; set; }
}

public enum FortressType
{
    Route,      // 마을 간 경로 요새 (필수, 중상급)
    Side        // 별개 독립 요새 (선택, 최상급)
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
    public List<Hero> Party { get; set; }       // 일반 전투: 최대 4명, 요새 전투: 최대 6명

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
    type VARCHAR(20) NOT NULL,  -- "offensive" | "defensive" | "balanced"
    element VARCHAR(20) NOT NULL,  -- "water" | "fire" | "earth" | "none"

    -- 등급 & 성장
    star_grade INT CHECK (star_grade BETWEEN 1 AND 6),
    base_growth_rate INT NOT NULL,  -- 1★=11, 2★=23, 3★=34, 4★=44, 5★=53, 6★=61
    growth_rate_bonus FLOAT DEFAULT 0.0,  -- 환생 보너스: sacrifice_level × 0.05
    rebirths INT DEFAULT 0,

    -- 레벨 & 경험치
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

    -- 성장 스탯 (레벨 1 기준 스탯)
    growth_rates JSONB NOT NULL,

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
    effect_trigger VARCHAR(30) NOT NULL,   -- COMBAT_START, ROUND_START, MY_TURN, etc.
    description TEXT,
    value_type VARCHAR(20) NOT NULL,       -- fixed_value, percentage
    min_value FLOAT,
    max_value FLOAT,
    duration INT DEFAULT -1,               -- -1: 영구, 0: 즉시, N: N라운드
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

-- 장비 수식어 정의 테이블 (마스터 데이터)
CREATE TABLE equipment_modifier_definitions (
    modifier_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,             -- "강력한", "맹공의", "행운"
    position VARCHAR(10) NOT NULL,         -- "prefix" | "suffix"
    stat VARCHAR(50) NOT NULL,             -- "attack", "crit_rate", "drop_rate_bonus"
    modifier_type VARCHAR(10) NOT NULL,    -- "flat" | "percent"
    base_ratio FLOAT NOT NULL,             -- 기본 스탯 대비 비율 (예: 0.5)
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 몬스터 템플릿 테이블 (마스터 데이터)
CREATE TABLE monster_templates (
    monster_id VARCHAR(100) PRIMARY KEY,   -- "slime", "wolf", "boss_slime"
    name VARCHAR(100) NOT NULL,
    type VARCHAR(20) NOT NULL,             -- "normal" | "boss"
    element VARCHAR(20) NOT NULL,          -- "water" | "fire" | "earth" | "none"

    -- 기본 스탯 (Lv1 기준)
    base_hp INT NOT NULL,
    base_attack INT NOT NULL,
    base_defense INT NOT NULL,

    -- 성장치
    base_growth_rate INT NOT NULL,         -- 슬라임: 20, 늑대: 30, 보스: 60+

    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 장비 테이블
CREATE TABLE equipment (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    template_id VARCHAR(100) NOT NULL,

    -- 기본 정보
    name VARCHAR(200) NOT NULL,            -- "강력한 빛나는 검 of 행운"
    base_name VARCHAR(100) NOT NULL,       -- "빛나는 검"
    grade VARCHAR(5) NOT NULL,             -- "C" | "UC" | "R" | "H" | "L"
    equipment_type VARCHAR(20) NOT NULL,   -- "weapon" | "armor" | "ring" | "neckless" | "belt"
    equipment_level INT NOT NULL,          -- 장비 레벨 (착용 레벨)

    -- 기본 스탯
    base_stat_type VARCHAR(50) NOT NULL,   -- "attack" | "defense" | "hp" | "crit_rate" | "evasion"
    base_stat_value FLOAT NOT NULL,

    -- 수식어 옵션
    prefix_modifier_id INT REFERENCES equipment_modifier_definitions(modifier_id),
    prefix_value FLOAT,
    suffix_modifier_id INT REFERENCES equipment_modifier_definitions(modifier_id),
    suffix_value FLOAT,

    -- 상태
    equipped BOOLEAN DEFAULT FALSE,
    equipped_by UUID REFERENCES heroes(id),

    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
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
INSERT INTO unique_effect_definitions (effect_id, effect_name, effect_category, effect_trigger, value_type, min_value, max_value, duration) VALUES
(1, '강력한 일격', 'offensive', 'FIRST_ATTACK', 'percentage', 0.10, 0.25, 0),
(2, '받는 피해 감소', 'defensive', 'ALWAYS', 'percentage', 0.08, 0.20, -1),
(3, '흡혈', 'defensive', 'ON_HIT', 'percentage', 0.10, 0.25, 0),
(4, '반사 피해', 'defensive', 'ON_DAMAGED', 'percentage', 0.15, 0.35, 0),
(5, '아군 전체 공격력 증가', 'aura', 'COMBAT_START', 'percentage', 0.08, 0.20, -1),
(6, '적 전체 방어력 감소', 'aura', 'ROUND_START', 'percentage', 0.10, 0.25, 3);

-- 장비 수식어 초기 데이터 예시
INSERT INTO equipment_modifier_definitions (modifier_id, name, position, stat, modifier_type, base_ratio) VALUES
-- Prefix (공격형 Flat)
(1, '맹공의', 'prefix', 'attack', 'flat', 0.5),
(2, '날카로운', 'prefix', 'armor_penetration', 'flat', 0.3),
-- Prefix (공격형 Percent)
(10, '강력한', 'prefix', 'attack', 'percent', 0.5),
(11, '치명적인', 'prefix', 'crit_rate', 'percent', 0.5),
(12, '파괴의', 'prefix', 'crit_damage', 'percent', 0.4),
(13, '신속한', 'prefix', 'evasion_pierce', 'percent', 0.3),
-- Prefix (방어형 Flat)
(20, '견고한', 'prefix', 'defense', 'flat', 0.5),
(21, '생명의', 'prefix', 'hp', 'flat', 0.5),
-- Prefix (방어형 Percent)
(30, '튼튼한', 'prefix', 'defense', 'percent', 0.5),
(31, '민첩한', 'prefix', 'evasion', 'percent', 0.4),
-- Suffix (유틸리티)
(100, '행운', 'suffix', 'drop_rate_bonus', 'percent', 0.3),
(101, '번영', 'suffix', 'gold_bonus', 'percent', 0.4),
(102, '성장', 'suffix', 'exp_bonus', 'percent', 0.4);

-- 몬스터 템플릿 초기 데이터 예시
INSERT INTO monster_templates (monster_id, name, type, element, base_hp, base_attack, base_defense, base_growth_rate) VALUES
-- 일반 몬스터
('slime', '슬라임', 'normal', 'water', 50, 8, 3, 20),
('wolf', '늑대', 'normal', 'none', 80, 12, 5, 30),
('bee', '벌', 'normal', 'none', 60, 15, 4, 25),
('goblin', '고블린', 'normal', 'earth', 90, 14, 7, 28),
('orc', '오크', 'normal', 'fire', 120, 18, 10, 35),
-- 보스 몬스터
('boss_slime', '보스 슬라임', 'boss', 'water', 200, 25, 15, 60),
('dragon', '드래곤', 'boss', 'fire', 500, 60, 40, 80);
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
public static bool ValidateParty(List<Hero> party, CombatType combatType = CombatType.Normal)
{
    int maxPartySize = combatType == CombatType.Fortress ? 6 : 4;
    if (party.Count > maxPartySize) return false;
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
