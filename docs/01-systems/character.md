# 영웅 시스템

## 개요

```yaml
type: "별(★) 등급 시스템"
grades: [1, 2, 3, 4, 5, 6]
special_grade: "★6 (첫 별 빨간색)"
max_party_size: 4
```

**핵심 가치**: 같은 영웅도 별 등급에 따라 다른 성능 = 수집 동기 부여

## 별(★) 등급 시스템

### 등급 구분

```yaml
star_1:
  name: "1성"
  rarity: "매우 흔함"
  unique_effect: false
  growth_rate: "낮음"

star_2:
  name: "2성"
  rarity: "흔함"
  unique_effect: false
  growth_rate: "약간 낮음"

star_3:
  name: "3성"
  rarity: "보통"
  unique_effect: false
  growth_rate: "보통"

star_4:
  name: "4성"
  rarity: "희귀"
  unique_effect: true
  growth_rate: "높음"

star_5:
  name: "5성"
  rarity: "매우 희귀"
  unique_effect: true
  growth_rate: "매우 높음"

star_6:
  name: "6성 (빨간 별)"
  rarity: "전설"
  unique_effect: true
  growth_rate: "최고"
  visual: "첫 번째 별이 빨간색"
```

### 시각적 표현

```
★☆☆☆☆  - 1성
★★☆☆☆  - 2성
★★★☆☆  - 3성
★★★★☆  - 4성
★★★★★  - 5성
★★★★★  - 6성 (첫 별 빨간색)
```

## 영웅 스탯

### 기본 스탯

```yaml
core_stats:
  hp: "생명력"
  attack: "공격력"
  defense: "방어력"
  element: "속성 (물/불/땅/무)"

derived_stats:
  critical_rate: "크리티컬 확률"
  critical_damage: "크리티컬 데미지"
  block_rate: "회피 확률"
```

### 스탯 성장

```yaml
growth_system:
  components:
    growth_rate: "영웅 성장치 (별 등급별 차이)"
    base_stats: "레벨 1 기준 스탯"
    growth_rates: "레벨당 증가량 (성장치 비례)"

  formula:
    stat_at_level: "기본 스탯 + (성장률 × (레벨 - 1))"
    growth_affected: "hp, attack, defense"
    fixed: "critical_rate, critical_damage, block_rate"

example_comparison:
  star_1:
    growth_rate: 10
    base: "HP 50, 공격 10, 방어 5"
    per_level: "HP +5, 공격 +1, 방어 +0.5"

  star_6:
    growth_rate: 60
    base: "HP 200, 공격 32, 방어 20"
    per_level: "HP +30, 공격 +6, 방어 +3"

  note: "6성은 1성 대비 성장률 6배"
```

## 고유 효과 (★4 이상)

### 효과 카테고리

```yaml
offensive:
  - "확률 전열 공격 : N% 확률로 공격 대상과 같은 열에 있는 모든 적에게 M%의 데미지를 입힘"
  - "확률 전행 공격 : N% 확률로 공격 대상과 같은 행에 있는 모든 적에게 M%의 데미지를 입힘"
  - "전체 공격 : 모든 적에게 M%의 데미지를 입힘"
  - "전체 공격(고급) : M%의 데미지를 살아있는 모든 적에게 균등하게 나누어 데미지를 입힘"
  - "강력한 일격 : 공격 시 M%의 추가 데미지를 입힘"

defensive:
  - "받는 피해 감소 -X%"
  - "라운드 시작 시 HP 회복 +X"
  - "첫 공격 무효화"

utility:
  - "파티 경험치 증가 +X%"
  - "드랍률 증가 +X%"
  - "골드 획득 증가 +X%"
```

### 효과 밸런스
```yaml
star_4_effects:
  count: 1~3
  power: "10-15%"

star_5_effects:
  count: 1~3
  power: "15-25%"

star_6_effects:
  count: 1~3
  power: "25-40%"
```

## 레벨 시스템

### 경험치 곡선

```yaml
exp_formula:
  formula: "100 × level^1.5"

examples:
  Lv1→2: 100
  Lv2→3: 282
  Lv3→4: 519
```

### 레벨 상한

```yaml
level_cap:
  initial: 200
```

## 파티 편성

### 파티 규칙

```yaml
party_composition:
  min_size: 1
  max_size: 4
  duplicate: true  # 같은 영웅 중복 가능

element_effect:
  - 동일 속성 영웅끼리만 파티 편성 시 전체 파티원(동일 속성) 공/방/체 10% 보너스(단, 4인 파티에 한함, 사망시에도 유효)
  - 특정 영웅 조합끼리 파티 편성 시 해당 영웅들 공/방/체 N% 보너스(ex:로미오, 줄리엣)

restrictions:
  - 전투 중 편성 변경 불가
  - 마을에서만 편성 가능
```

### 배치 전략

```yaml
tank_build:
  front: "고방어 영웅"
  mid: "균형형 영웅"
  back: "공격형 영웅"

dps_build:
  front: "빠른 딜러"
  mid: "빠른 딜러"
  back: "빠른 딜러"

balanced_build:
  positions: "상황에 따라 혼합"
```

## 획득 방법

```yaml
acquisition:
  starter_heroes:
    count: 4
    grade: [1, 2, 3]
    location: "시작 마을"
    desc: "grade별 랜덤 출현 확률"

  fortress_rewards:
    grades: [4, 5, 6]
    source: "요새 클리어 시 보급형 고밸류 영웅 수집(확률)"
    guaranteed: true

  shop:
    grades: [1, 2, 3, 4, 5, 6]
    cost: "골드"
    desc: "grade별 랜덤 출현 확률"
```

## 데이터 구조

> 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **Hero**: 영웅 정보 (별 등급, 레벨, 스탯, 고유 효과)
- **별 등급 시스템**: 1~6성 (성장률, 고유효과 여부)
- **스탯**: HP, 공격, 방어, 치명타율/데미지, 회피율
- **고유 효과**: ★4 이상 전용 (공격/방어/유틸리티 효과)
- **속성**: 물/불/땅/무

## UI 요구사항

### 영웅 목록 화면

```yaml
display:
  - 영웅 초상화
  - 이름
  - 별 등급 (★ 시각화)
  - 레벨
  - 속성 아이콘

sort_options:
  - 별 등급 (기본)
  - 레벨
  - 공격력
  - 획득 순서

filter_options:
  - 속성별
  - 별 등급별
  - 파티 포함 여부
```

### 영웅 상세 화면

```yaml
information:
  - 모든 스탯 표시
  - 경험치 바 (현재/필요)
  - 고유 효과 (★4+)
  - 장착 장비

actions:
  - 파티 편입/해제
  - 장비 장착
  - (미래) 강화, 진화
```

## 시스템 연동

### 의존 시스템
- `equipment.md` - 장비 장착으로 스탯 증가
- `combat.md` - 전투에서 사용, 경험치 획득

### 제공 기능
- 전투 유닛
- 파티 편성 정보
- 고유 효과 제공

## 밸런스 파라미터

```yaml
star_grade_power:
  star_1: 1.0x  # 기준
  star_2: 1.3x
  star_3: 1.6x
  star_4: 2.0x
  star_5: 2.5x
  star_6: 3.2x

drop_rate:
  star_1: 40%
  star_2: 30%
  star_3: 20%
  star_4: 8%
  star_5: 1.8%
  star_6: 0.2%
```

## 구현 체크리스트

- [ ] 영웅 데이터 구조
- [ ] 별 등급 시스템
- [ ] 레벨/경험치 시스템
- [ ] 파티 편성 UI
- [ ] 고유 효과 구현 (★4+)
- [ ] 영웅 목록 화면
- [ ] 영웅 상세 화면
- [ ] 획득 시스템

---
**최종 수정**: 2025-10-18
**상태**: 🔴 초안
**작성자**: SangHyeok
