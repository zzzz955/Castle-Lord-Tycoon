# 데이터 구조 정의

## 핵심 인터페이스

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
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float Attack { get; set; }
    public float Defense { get; set; }
    public float CriticalRate { get; set; }
    public float CriticalDamage { get; set; }
    public float BlockRate { get; set; }
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

public class Effect
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EffectType Type { get; set; }
    public float Value { get; set; }
}

public enum EffectType
{
    Offensive,
    Defensive,
    Utility
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
    if (hero.Stats.Hp < 0) return false;
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
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
