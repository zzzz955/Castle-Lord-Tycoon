# 월드 탐험 시스템

## 개요

```yaml
type: "오픈월드 자유 탐험"
camera: "쿼터뷰 (아이소메트릭)"
grid_based: true
fog_of_war: true
```

**핵심 가치**: 자유롭게 탐험하되, 누적형 여정으로 긴장감 유지

## 필드 구조

### 타일 기반 월드

```yaml
tile_system:
  size: "64x64 픽셀"
  types:
    - grass: "풀숲"
    - road: "도로"
    - forest: "숲"
    - swamp: "늪"
    - mountain: "산"
    - water: "물"
    - lava: "용암"

movement:
  walkable: [grass, road, forest, swamp]
  blocked: [mountain, water]
  speed_modifier:
    road: 1.2x
    grass: 1.0x
    forest: 0.8x
    swamp: 0.6x
```

### 지형 태그

```yaml
tile_tags:
  purpose: "조우 확률 및 스폰 테이블 결정"
  examples:
    - "초원_풀숲"
    - "검은숲_나무"
    - "늪지_물가"
    - "유적_밤"

usage:
  - 조우 발생 확률
  - 출현 몬스터 테이블
  - 드랍 테이블 가중치
```

## 전장의 안개 (Fog of War)

### 3단계 시야

```yaml
unexplored:
  name: "미탐험"
  visual: "짙은 안개"
  visibility: "지형, 오브젝트 확인 불가"
  color: "검은색 + 안개 효과"

explored_far:
  name: "탐험됨 (현재 위치 아님)"
  visual: "옅은 안개"
  visibility: "대략적 지형 파악 가능"
  color: "어두운 회색 + 반투명"

owned_territory:
  name: "내 영토"
  visual: "투명화 처리된 밝은 형광 연두빛"
  visibility: "완전히 보임"
  color: "투명화 처리된 밝은 녹색 하이라이트"
  purpose: "소유감 강화, 경계선 직관적 인식"
```

### 시야 확장

```yaml
exploration:
  method: "캐릭터 이동"
  range: "캐릭터 중심 반경 N 타일"
  permanent: true  # 한 번 탐험하면 유지

visibility_range:
  current_position: "반경 5 타일"
  explored: "영구 저장"
  owned: "영구 하이라이트"
```

## 야영 (캠핑) 시스템

### 야영 메커닉

```yaml
camping:
  setup:
    - 원하는 위치에서 설치 가능
    - 전투 중 불가
    - 조우 중 불가
    - 기존 야영지에서 수행 시 야영 종료

  effect:
    - HP 서서히 회복
    - 회복량: "시간당 최대 HP의 N%"
    - 조우 발생 없음

  cancel:
    - 캐릭터 이동 시 즉시 해제
    - 수동 해제 가능
```

### 야영 밸런스

```yaml
recovery_rate:
  base: "5% / 10초"
  with_items: "10% / 10초"
  max_duration: "무제한 (이동 시까지)"

strategic_use:
  - "긴 여정 전 준비"
  - "전투 후 회복"
  - "안전한 위치에서 대기"
```

## 이동 시스템

### 이동 방식

```yaml
input:
  mobile_pad: "8방향(↖↗↘↙ 포함)"
  keyboard: "WASD 또는 방향키"

pathfinding:
  algorithm: "A* 또는 단순 직선"
  obstacle: "차단 타일 회피"
  speed: "타일 타입에 따라 조정"
```

### 이동 제한

```yaml
restrictions:
  combat: "전투 중 이동 불가"
  encounter: "조우 발생 중 이동 불가"
  camping: "야영 중 이동 시 야영 해제"

movement_cost:
  concept: "없음 (자유 이동)"
  note: "체력 소모, 피로도 등 없음"
```

## 오브젝트

### 필드 오브젝트

```yaml
towns:
  visual: "마을 타일 태그"
  interaction: "타일 접근 시 입장, 별도 인터렉션 없음"
  owned_indicator: "월드맵 : 깃발 및 색상, 점령하지 못한 경우 연한 핑크색"

fortresses:
  visual: "요새 타일 태그"
  interaction: "타일 접근 시 최초 전투 인터렉션, 해금 후 별도 인터렉션 없음"
  locked_indicator: "월드맵 : 깃발 및 색상, 점령하지 못한 경우 연한 보라색"

flags:
  visual: "깃발 오브젝트"
  color: "플레이어 색상"
  size: "S(3x3), M(5x5), L(7x7)"

resources:
  future: "채집 자원 (향후 확장)"
```

## 데이터 구조

```typescript
interface Tile {
  x: number;
  y: number;
  type: "grass" | "road" | "forest" | "swamp" | "mountain" | "water" | "lava";
  tags: string[];  // ["초원_풀숲", "밤"]
  isWalkable: boolean;
  speedModifier: number;

  fogState: "unexplored" | "explored" | "owned";
  ownedByPlayer: boolean;
}

interface WorldMap {
  width: number;
  height: number;
  tiles: Tile[][];
  playerPosition: { x: number; y: number };
}

interface Camp {
  position: { x: number; y: number };
  startTime: number;
  healingRate: number;
  isActive: boolean;
}
```

## UI 요구사항

### 필드 화면

```yaml
minimap:
  - 전체 맵 축소판
  - 안개 상태 표시
  - 플레이어 위치
  - 마을/요새 표시(마을/요새 전체 타일의 중앙)

main_view:
  - 아이소메트릭 렌더링
  - 안개 효과 (3단계)
  - 영토 하이라이트
  - 오브젝트 표시

hud:
  - 파티 HP 바
  - 현재 위치 (타일 좌표 또는 지역명)
  - 야영 버튼
  - 귀환 버튼
```

## 시스템 연동

### 의존 시스템
- `territory.md` - 영토 시각화
- `settlement.md` - 마을/요새 위치
- `encounter.md` - 조우 발생 체크

### 제공 기능
- 필드 이동
- 시야 관리
- 야영 회복
- 오브젝트 상호작용

## 밸런스 파라미터

```yaml
fog_reveal_radius: 5  # 타일 단위
owned_territory_highlight: "0.3 alpha 녹색"

camping_heal_rate:
  base: 5  # % per 10 seconds
  max_hp_ratio: true

movement_speed:
  base: 5  # tiles per second
  road_bonus: 1.2
  forest_penalty: 0.8
  swamp_penalty: 0.6
```

## 구현 체크리스트

- [ ] 타일 기반 월드 생성
- [ ] 안개 시스템 (3단계)
- [ ] 이동 시스템
- [ ] 경로 탐색
- [ ] 야영 시스템
- [ ] 오브젝트 렌더링
- [ ] 미니맵

---
**최종 수정**: 2025-10-18
**상태**: 🔴 초안
**작성자**: SangHyeok
