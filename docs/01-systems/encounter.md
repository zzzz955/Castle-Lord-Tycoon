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

```yaml
weighted_random:
  algorithm: "가중치 기반 확률 선택"

  process:
    1: "전체 가중치 합 계산"
    2: "0 ~ 전체 가중치 범위에서 난수 생성"
    3: "누적 가중치로 몬스터 선택"

  example:
    monsters:
      - id: "goblin", weight: 70
      - id: "orc", weight: 20
      - id: "troll", weight: 10

    probability:
      goblin: "70/100 = 70%"
      orc: "20/100 = 20%"
      troll: "10/100 = 10%"
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

> 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **SpawnTable**: 타일 태그별 몬스터 출현 테이블 (조우율, 몬스터 목록, 가중치)
- **MonsterEntry**: 몬스터 출현 정보 (ID, 가중치, 레벨 범위)
- **EncounterResult**: 조우 결과 (적 목록, 그리드 배치)
- **Region**: 지역 정보 (난이도, 스폰 테이블 매핑)

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

## 몬스터 스탯 생성 시스템

### 몬스터 성장치 시스템

```yaml
concept: "몬스터도 영웅처럼 성장치를 가지며, 레벨에 따라 스탯 증가"

growth_rate_system:
  base_growth_rate: "몬스터 타입별 기본 성장치"
  variance: "조우 시 ±10% 랜덤 변동"

  examples:
    slime:
      base_growth_rate: 20
      encounter_variance: "18~22 (±10%)"

    wolf:
      base_growth_rate: 30
      encounter_variance: "27~33"

    boss_slime:
      base_growth_rate: 60
      encounter_variance: "54~66"
```

### 몬스터 스탯 계산 공식

```yaml
formula:
  stat_calculation: "base_stat + (level × (100 + growth_rate) / 100)"

examples:
  slime_lv5:
    base_hp: 50
    base_attack: 8
    base_defense: 3
    growth_rate: 20 (랜덤: 18~22, 평균 20)

    calculated_stats:
      hp: "50 + (5 × 1.20) = 56"
      attack: "8 + (5 × 1.20) = 14"
      defense: "3 + (5 × 1.20) = 7"

  wolf_lv10:
    base_hp: 80
    base_attack: 12
    base_defense: 5
    growth_rate: 30

    calculated_stats:
      hp: "80 + (10 × 1.30) = 93"
      attack: "12 + (10 × 1.30) = 25"
      defense: "5 + (10 × 1.30) = 12"

  boss_slime_lv15:
    base_hp: 200
    base_attack: 25
    base_defense: 15
    growth_rate: 60

    calculated_stats:
      hp: "200 + (15 × 1.60) = 224"
      attack: "25 + (15 × 1.60) = 49"
      defense: "15 + (15 × 1.60) = 39"
```

### 몬스터 타입별 기본 스탯

```yaml
normal_monsters:
  slime:
    type: "약함"
    base_hp: 50
    base_attack: 8
    base_defense: 3
    growth_rate: 20
    element: "water"

  wolf:
    type: "보통"
    base_hp: 80
    base_attack: 12
    base_defense: 5
    growth_rate: 30
    element: "none"

  bee:
    type: "빠름"
    base_hp: 60
    base_attack: 15
    base_defense: 4
    growth_rate: 25
    element: "none"

  goblin:
    type: "균형"
    base_hp: 90
    base_attack: 14
    base_defense: 7
    growth_rate: 28
    element: "earth"

  orc:
    type: "강함"
    base_hp: 120
    base_attack: 18
    base_defense: 10
    growth_rate: 35
    element: "fire"

boss_monsters:
  boss_slime:
    type: "보스 (요새)"
    base_hp: 200
    base_attack: 25
    base_defense: 15
    growth_rate: 60
    element: "water"
    note: "일반 슬라임의 4배 기본 스탯 + 3배 성장치"

  dragon:
    type: "보스 (최종)"
    base_hp: 500
    base_attack: 60
    base_defense: 40
    growth_rate: 80
    element: "fire"
    note: "최상급 보스"
```

### 경험치 및 골드 보상

```yaml
exp_formula:
  base_exp: "monster_level × 10"
  growth_rate_bonus: "base_exp × (growth_rate / 100)"
  final_exp: "base_exp + growth_rate_bonus"

  examples:
    slime_lv5_growth20:
      base: "5 × 10 = 50"
      bonus: "50 × 0.20 = 10"
      total: "60 exp"

    wolf_lv10_growth30:
      base: "10 × 10 = 100"
      bonus: "100 × 0.30 = 30"
      total: "130 exp"

    boss_slime_lv15_growth60:
      base: "15 × 10 = 150"
      bonus: "150 × 0.60 = 90"
      total: "240 exp"

gold_formula:
  base_gold: "monster_level × 5"
  growth_rate_bonus: "base_gold × (growth_rate / 100)"
  final_gold: "base_gold + growth_rate_bonus"

  examples:
    slime_lv5: "(5 × 5) + (25 × 0.20) = 30 gold"
    wolf_lv10: "(10 × 5) + (50 × 0.30) = 65 gold"
    boss_slime_lv15: "(15 × 5) + (75 × 0.60) = 120 gold"
```

## 밸런스 파라미터

```yaml
encounter_rates:
  safe_road: 10%
  normal_field: 30%
  dangerous_area: 50%
  boss_zone: 100%  # 요새는 반드시 보스 조우

difficulty_scaling:
  - 지역별 고정 (플레이어 레벨 무관)
  - 몬스터 레벨은 지역 level_range 내 랜덤
  - 보상은 몬스터 레벨 + 성장치에 비례

monster_level_determination:
  rule: "지역별 level_range 내에서 랜덤 생성"
  player_level: "고려하지 않음"
  example:
    region: "초원 (level_range: 1-10)"
    player_lv50: "여전히 Lv1~10 몬스터 출현"
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
