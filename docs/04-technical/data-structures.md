# 데이터 구조 정의

## 핵심 인터페이스

### Hero (영웅)

```typescript
interface Hero {
  // 식별
  id: string;              // 고유 ID
  templateId: string;      // 영웅 종류
  type: string; // 영웅 타입(공격, 방어, 지원)

  // 등급
  starGrade: 1 | 2 | 3 | 4 | 5 | 6;

  // 기본 정보
  name: string;
  element: Element;

  // 레벨 & 경험치
  level: number;
  currentExp: number;

  // 스탯
  stats: {
    hp: number;
    maxHp: number;
    attack: number;
    defense: number;
    critical_rate: number;
    critical_damage: number;
    block_rate: number;
  };

  // 성장
  baseStats: Stats;        // 레벨 1 기준
  growthRates: Stats;      // 레벨당 증가량
  rebirths: number; // 환생 횟수, 환생 시 growthRates증가, 기존 레벨당 증가량 격차만큼 능력치 보정

  // 효과 (★4+)
  uniqueEffects?: Effect[];

  // 상태
  isDead: boolean;
  isInParty: boolean;
  equippedItems: {
    weapon?: string;       // equipment ID
    armor?: string;
    accessory1?: string;
    accessory2?: string;
  };
}

type Element = "water" | "fire" | "earth" | "none";

interface Stats { // critical_rate, critical_damage, block_rate는 성장치와 영향 없음
  hp: number;
  attack: number;
  defense: number;
}

interface Effect {
  id: string;
  name: string;
  description: string;
  type: "offensive" | "defensive" | "utility";
  value: number;
}
```

### Equipment (장비)

```typescript
interface Equipment {
  // 식별
  id: string;
  templateId: string;

  // 기본 정보
  name: string;
  grade: Grade;
  type: EquipmentType;
  requiredLevel: number;

  // 스탯
  baseStat: {
    type: "attack" | "defense" | "hp";
    value: number;
  };

  // 옵션
  options: EquipmentOption[];

  // 상태
  equipped: boolean;
  equippedBy?: string;     // hero ID
}

type Grade = "C" | "UC" | "R" | "H" | "L";
type EquipmentType = "weapon" | "armor" | "ring" | "neckless" | "belt";

interface EquipmentOption {
  type: "offensive" | "defensive" | "utility";
  stat: string;            // "추가 공격", "드랍률" 등
  value: number;
}
```

### Combat (전투)

```typescript
interface Combat {
  combatId: string;

  // 참가자
  playerParty: Hero[];
  enemyParty: Enemy[];

  // 배치
  gridPositions: {
    player: (Hero | null)[];   // length 9
    enemy: (Enemy | null)[];   // length 9
  };

  // 진행 상태
  currentRound: number;
  maxRound: number;
  isFinished: boolean;

  // 기록
  log: CombatAction[];
}

interface CombatAction {
  round: number;
  actorId: string;
  targetId: string;
  damage: number;
  elementBonus: number;
  targetDied: boolean;
}

interface CombatResult {
  victory: boolean;
  survivors: Hero[];
  dead: Hero[];
  expGained: Map<string, number>;  // heroId -> exp
  itemsDropped: Equipment[];
  goldGained: number;
  roundsElapsed: number;
}
```

### Territory (영토)

```typescript
interface Territory {
  // 소유 타일
  ownedTiles: Set<string>;   // "x,y" 형식

  // 깃발
  flags: Flag[];

  // 통계
  totalArea: number;
  connectedTowns: string[];  // town IDs
}

interface Flag {
  id: string;
  size: 3 | 5 | 7 | 9;
  position: Position;
  placedAt: number;          // timestamp
}

interface Position {
  x: number;
  y: number;
}
```

### World (월드)

```typescript
interface WorldMap {
  width: number;
  height: number;
  tiles: Tile[][];

  regions: Region[];
  towns: Town[];
  fortresses: Fortress[];
}

interface Tile {
  x: number;
  y: number;
  type: TileType;
  tags: string[];            // ["초원_풀숲", "밤"]

  isWalkable: boolean;
  speedModifier: number;

  fogState: FogState;
  ownedByPlayer: boolean;
}

type TileType = "grass" | "road" | "forest" | "swamp" | "mountain" | "water" | "lava";
type FogState = "unexplored" | "explored" | "owned";
```

### Settlement (정착지)

```typescript
interface Town {
  id: string;
  name: string;
  position: Position;
  size: number;              // 기본 영토 크기

  owned: boolean;
  conqueredAt?: number;

  region: string;
  difficulty: number;

  features: {
    recovery: boolean;       // 회복 기능
    shop?: Shop;
  };
}

interface Fortress {
  id: string;
  name: string;
  position: Position;
  size: number;

  unlocked: boolean;
  unlockedAt?: number;

  firstBattle: {
    enemies: Enemy[];
    rewards: Equipment[];
  };

  features: {
    shop: Shop;
    exchange: Exchange;
    crafting: CraftingStation;
  };
}
```

### Player (플레이어)

```typescript
interface Player {
  // 기본 정보
  name: string;

  // 위치
  position: Position;
  camping: CampingState | null;

  // 소유
  heroes: Hero[];
  equipment: Equipment[];
  gold: number;

  // 파티
  party: Hero[];             // 최대 4명

  // 진행도
  territory: Territory;
  rank: Rank;
  ownedTowns: Set<string>;   // town IDs
  unlockedFortresses: Set<string>;

  // 메타
  playTime: number;          // 초
}

interface CampingState {
  position: Position;
  startTime: number;
  healingRate: number;
  isActive: boolean;
}
```

### Progression (진행)

```typescript
interface Rank {
  id: string;
  name: string;
  order: number;

  requirement: {
    area: number;
    towns?: number;
    fortresses?: number;
  };

  benefits: {
    expBonus?: number;
    dropRateBonus?: number;
    campingEfficiency?: number;
    movementSpeed?: number;
    fortressDiscount?: number;
    upgradeCostReduction?: number;
    bossRewardMultiplier?: number;
    rareDropWeight?: number;
    specialTokens?: number;
  };
}
```

## 헬퍼 타입

```typescript
// 유틸리티
type Optional<T> = T | null | undefined;
type ReadOnly<T> = { readonly [K in keyof T]: T[K] };

// 이벤트
type GameEvent =
  | { type: "COMBAT_START"; data: { enemies: Enemy[] } }
  | { type: "COMBAT_END"; data: { result: CombatResult } }
  | { type: "TOWN_CONQUERED"; data: { town: Town } }
  | { type: "RANK_UP"; data: { newRank: Rank } }
  | { type: "LEVEL_UP"; data: { hero: Hero } }
  | { type: "FLAG_PLACED"; data: { flag: Flag } };
```

## 데이터 검증

```typescript
// 예시: 영웅 생성 검증
function validateHero(hero: Hero): boolean {
  if (hero.starGrade < 1 || hero.starGrade > 6) return false;
  if (hero.level < 1) return false;
  if (hero.stats.hp < 0) return false;
  return true;
}

// 파티 검증
function validateParty(party: Hero[]): boolean {
  if (party.length > 4) return false;
  const uniqueIds = new Set(party.map(h => h.id));
  if (uniqueIds.size !== party.length) return false;  // 중복 불가
  return true;
}
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
