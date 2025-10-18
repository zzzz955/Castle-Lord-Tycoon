# 조우 시스템

## 개요

```yaml
type: "타일 태그 기반"
trigger: "필드 이동"
skip_conditions: ["야영 중"]
```

**핵심 가치**: 지역마다 다른 적, 고정 난이도로 도전 의미 부여

## 조우 발생 메커닉

### 발생 조건

```yaml
trigger:
  - 플레이어가 타일 이동
  - 야영 중 아님
  - 전투 중 아님

check:
  - 타일 태그 확인
  - 조우 테이블 조회
  - 확률 판정
```

### 타일 태그 시스템

```yaml
tag_format: "지역_바이옴_시간"

examples:
  - "초원_풀숲"
  - "검은숲_나무_밤"
  - "늪지_물가"
  - "유적_돌바닥"

usage:
  - 조우 확률 결정
  - 스폰 테이블 선택
  - 드랍 테이블 가중치
```

## 스폰 테이블

### 테이블 구조

```yaml
spawn_table_example:
  tag: "초원_풀숲"
  encounter_rate: 10%  # 이동 시 30% 확률

  monsters:
    - id: "slime"
      weight: 45
      level_range: [1, 5]

    - id: "wolf"
      weight: 30
      level_range: [3, 7]

    - id: "bee"
      weight: 20
      level_range: [2, 6]

    - id: "boss_slime"
      weight: 5
      level_range: [8, 10]
```

### 가중치 시스템

```typescript
function selectMonster(table: SpawnTable): Monster {
  const totalWeight = table.monsters.reduce((sum, m) => sum + m.weight, 0);
  const roll = random(0, totalWeight);

  let current = 0;
  for (const entry of table.monsters) {
    current += entry.weight;
    if (roll <= current) {
      return createMonster(entry.id, randomLevel(entry.level_range));
    }
  }
}
```

## 지역 난이도

### 고정 난이도 시스템

```yaml
concept: "플레이어 레벨에 맞춰지지 않음"
design: "지역마다 고정된 난이도"

regions:
  starter_plains:
    difficulty: 1
    monster_levels: [1, 10]
    description: "입문 지역"

  dark_forest:
    difficulty: 3
    monster_levels: [15, 25]
    description: "중수 지역"

  iron_fortress:
    difficulty: 5
    monster_levels: [30, 40]
    description: "중상급 지역"

  dragon_valley:
    difficulty: 8
    monster_levels: [50, 70]
    description: "상급 지역"
```

### 난이도 표시

```yaml
visual_indicators:
  - 없음

warning_system:
  - "과도한 경고 지양 (모험성 우선)"
```

## 조우 구성

### 적 그룹 생성

```yaml
group_size:
  min: 3
  max: 4
  rule: "플레이어 파티 크기와 무관"

composition:
  - 같은 종류 반복 가능
  - 레벨 범위 내 랜덤

배치:
  - 적도 3×3 그리드 사용
  - 우선순위 순서 적용
```

## 데이터 구조

```typescript
interface SpawnTable {
  tag: string;
  encounterRate: number;  // 0-100%
  monsters: MonsterEntry[];
}

interface MonsterEntry {
  id: string;
  weight: number;
  levelRange: [number, number];
  maxCount?: number;  // 최대 동시 출현 수
}

interface EncounterResult {
  enemies: Monster[];
  gridPositions: (Monster | null)[];  // length 9
}

interface Region {
  id: string;
  name: string;
  difficulty: number;
  spawnTables: Map<string, SpawnTable>;  // tag -> table
}
```

## 조우 확률 조정

### 확률 변경 요인

```yaml
base_rate: "타일 태그별 기본 확률"

modifiers:
  - 계급 보너스 (확률 감소 가능)
  - 특수 아이템 (조우 증가/감소)
  - 이벤트 (한시적 변경)

calculation:
  final_rate = base_rate * (1 + modifiers)
  cap: [5%, 80%]  # 최소/최대 제한
```

## UI 요구사항

### 조우 발생 시

```yaml
transition:
  - 화면 효과 (페이드, 플래시)
  - 전투 화면으로 전환
  - 적 정보 미리보기 (옵션)

pre_battle:
  display:
    - 적 종류 및 수
    - 적 전체 전투력
```

### 지역 정보

```yaml
region_panel:
  - 지역 이름
  - 난이도 (★로 표시)
  - 출현 몬스터 목록
  - 권장 레벨
```

## 시스템 연동

### 의존 시스템
- `world-exploration.md` - 타일 태그
- `combat.md` - 전투 시작

### 제공 기능
- 조우 발생
- 적 그룹 생성
- 난이도 정보

## 밸런스 파라미터

```yaml
encounter_rates:
  safe_road: 10%
  normal_field: 30%
  dangerous_area: 50%
  boss_zone: 80%

difficulty_scaling:
  - 지역별 고정
  - 플레이어 성장과 무관
  - 보상은 난이도 비례
```

## 확장 가능성

```yaml
future_features:
  - 선제공격 (먼저 공격)
  - 기습 (적이 먼저)
  - 희귀 몬스터 출현
  - 날씨/시간 조우 변경
```

## 구현 체크리스트

- [ ] 타일 태그 시스템
- [ ] 스폰 테이블 데이터
- [ ] 조우 확률 판정
- [ ] 적 그룹 생성
- [ ] 지역 난이도 시스템
- [ ] 조우 트랜지션
- [ ] 지역 정보 UI

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
