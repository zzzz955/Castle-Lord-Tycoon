# 시스템 아키텍처

## 전체 구조

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│   (UI, Rendering, Input Handling)   │
└─────────────┬───────────────────────┘
              │
┌─────────────▼───────────────────────┐
│          Game Logic Layer           │
│   (Systems, Rules, State Machine)   │
└─────────────┬───────────────────────┘
              │
┌─────────────▼───────────────────────┐
│           Data Layer                │
│   (Save/Load, Data Management)      │
└─────────────────────────────────────┘
```

## 모듈 구성

### Core Systems

```yaml
world_system:
  responsibility: "필드 관리, 이동, 안개"
  dependencies: [data_loader]
  provides: [tile_info, player_position]

combat_system:
  responsibility: "전투 로직, 데미지 계산"
  dependencies: [character_system, equipment_system]
  provides: [combat_result]

territory_system:
  responsibility: "깃발, 영토, 자동 점령"
  dependencies: [world_system, settlement_system]
  provides: [owned_tiles, conquest_events]

character_system:
  responsibility: "영웅 관리, 성장, 파티"
  dependencies: [data_loader]
  provides: [party_info, hero_stats]

equipment_system:
  responsibility: "장비, 인벤토리, 드랍"
  dependencies: [data_loader]
  provides: [equipment_bonuses]
```

### Data Flow

```
User Input
    ↓
Input Handler → Game State
    ↓
System Update (Combat, Territory, etc.)
    ↓
State Change → Event Dispatcher
    ↓
UI Update + Renderer
```

## 상태 관리

### Game State

```typescript
interface GameState {
  world: WorldState;
  player: PlayerState;
  combat: CombatState | null;
  ui: UIState;
}

interface PlayerState {
  position: { x: number; y: number };
  party: Hero[];
  inventory: Equipment[];
  territory: TerritoryState;
  rank: Rank;
  camping: CampingState | null;
}
```

### Event System

```typescript
type GameEvent =
  | { type: "COMBAT_START"; enemies: Enemy[] }
  | { type: "COMBAT_END"; result: CombatResult }
  | { type: "TOWN_CONQUERED"; town: Town }
  | { type: "RANK_UP"; newRank: Rank }
  | { type: "FLAG_PLACED"; position: Position };

interface EventDispatcher {
  subscribe(eventType: string, handler: EventHandler): void;
  emit(event: GameEvent): void;
}
```

## 성능 고려사항

### 렌더링

```yaml
optimization:
  - 타일 기반 청킹 (Chunk System)
  - 화면 밖 오브젝트 컬링
  - 안개 상태별 레이어 캐싱

target:
  fps: 60
  tile_count: "최대 15000 타일"
  visible_area: "20×20 타일"
```

### 데이터 로딩

```yaml
strategy:
  - 레이지 로딩 (필요 시 로드)
  - 지역별 데이터 청크
  - 몬스터/장비 템플릿 캐싱

startup_time:
  target: "< 3초"
  preload: ["core systems", "starter region"]
```

## 저장/로드

### 세이브 데이터

```typescript
interface SaveData {
  version: string;
  timestamp: number;

  player: {
    position: Position;
    rank: string;
    gold: number;
  };

  heroes: SavedHero[];
  equipment: SavedEquipment[];
  territory: {
    flags: Flag[];
    ownedTiles: string[];  // "x,y"
  };
  towns: {
    owned: string[];  // town IDs
  };
  fortresses: {
    unlocked: string[];  // fortress IDs
  };

  meta: {
    playTime: number;
    version: string;
  };
}
```

### 저장 전략

```yaml
triggers:
  - 서버 및 DB기반
```

## 확장성

```yaml
modular_design:
  - 각 시스템 독립적
  - 인터페이스 기반 통신
  - 이벤트 기반 느슨한 결합

future_expansion:
  - 플러그인 시스템
  - 멀티플레이어 (서버 분리)
  - 모드 지원
```

## 개발 도구

```yaml
debug_mode:
  - 안개 끄기
  - 무적 모드
  - 아이템 스폰
  - 텔레포트

logging:
  - 전투 로그
  - 시스템 이벤트
  - 성능 프로파일링
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
