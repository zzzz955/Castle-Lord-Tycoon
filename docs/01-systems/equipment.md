# 장비 시스템

## 개요

```yaml
grade_system: "색상 기반"
grades: ["C", "UC", "R", "H", "L"]
stat_system: "기본 + 옵션"
set_effects: false
```

**핵심 가치**: 별(★) 대신 색상으로 구분, 직관적 성능 예측

## 등급 시스템

### 색상 등급

```yaml
common:
  code: "C"
  name: "Common"
  color: "흰색"
  drop_rate: 65%

uncommon:
  code: "UC"
  name: "UnCommon"
  color: "연두"
  drop_rate: 20%

rare:
  code: "R"
  name: "Rare"
  color: "노랑"
  drop_rate: 12%

hero:
  code: "H"
  name: "Hero"
  color: "보라"
  drop_rate: 2.5%

legend:
  code: "L"
  name: "Legend"
  color: "연한 주황"
  drop_rate: 0.5%
```

### 등급별 특징

```yaml
c_uc:
  options: 0-1
  power: "낮음"
  use: "초반"

rare:
  options: 1-2
  power: "중간"
  examples: ["행운의 검 - 공격↑ + 드랍률 +5%"]

hero_legend:
  options: 2
  power: "높음"
  examples: ["강력한 필살의 검 - 공격↑ + 추가공격 +15 + 치확 +10%"]
```

## 장비 타입

### 부위별 장비

```yaml
weapon:
  slot: "무기"
  primary_stat: "attack"
  types: ["검", "활", "지팡이"]

armor:
  slot: "방어구"
  primary_stat: "defense"
  types: ["판금 갑옷", "가죽 갑옷", "로브"]

neckless:
  slot: "목걸이"
  primary_stat: "hp"
  types: ["목걸이"]

ring:
  slot: "반지"
  primary_stat: "critical_rate"
  types: ["반지"]
  count: 2  # 2개 장착 가능

belt:
  slot: "벨트"
  primary_stat: "block_rate"
  types: ["벨트"]
```

## 옵션 구조

### 기본 능력치

```yaml
weapon: "공격력 +N (Flat)"
armor: "방어력 +N (Flat)"
neckless: "HP +N (Flat)"
ring: "critical_rate +N% (Percent)"
belt: "evasion +N% (Percent)"  # block_rate → evasion 변경
```

### 장비 수식어 시스템 (Prefix/Suffix)

```yaml
concept: "등급별로 수식어가 장비 이름에 붙어 옵션 표시"

prefix_examples:
  flat_bonuses:
    - "맹공의" → attack +N (Flat)
    - "견고한" → defense +N (Flat)
    - "생명의" → hp +N (Flat)
    - "날카로운" → armor_penetration +N (Flat)

  percent_bonuses:
    - "강력한" → attack +N% (Percent)
    - "튼튼한" → defense +N% (Percent)
    - "치명적인" → crit_rate +N% (Percent)
    - "신속한" → evasion_pierce +N% (Percent)
    - "파괴의" → crit_damage +N% (Percent)

suffix_examples:
  utility:
    - "행운" → drop_rate +N% (Percent)
    - "번영" → gold_bonus +N% (Percent)
    - "성장" → exp_bonus +N% (Percent)

naming_pattern:
  C: "빛나는 검"
  UC: "빛나는 검 of 행운"
  R: "강력한 빛나는 검 of 행운"
  H: "강력한 빛나는 검 of 행운"
  L: "강력한 빛나는 검 of 행운"

note: "수식어 개수는 등급에 따라 결정, 수식어 목록은 확장 가능"
```

### 추가 옵션 (Flat vs Percent 명시)

```yaml
offensive:
  flat:
    - "추가 공격 +N (attack)"
    - "방어도 무시 +N (armor_penetration)"

  percent:
    - "공격력 +N% (attack)"
    - "치명타 확률 +N% (crit_rate)"
    - "치명타 데미지 +N% (crit_damage)"
    - "회피 무시 +N% (evasion_pierce)"

defensive:
  flat:
    - "추가 방어 +N (defense)"
    - "추가 HP +N (hp)"

  percent:
    - "방어력 +N% (defense)"
    - "회피 확률 +N% (evasion)"

utility:
  percent:
    - "드랍률 +N% (drop_rate_bonus)"
    - "경험치 +N% (exp_bonus)"
    - "골드 획득 +N% (gold_bonus)"

note: "HP는 Flat만 존재, Percent로 증가시키지 않음"
```

### 등급별 수식어 수

```yaml
modifiers_by_grade:
  C: 0개 (수식어 없음)
  UC: 1개 (Prefix or Suffix)
  R: 2개 (Prefix + Suffix)
  H: 2개 (Prefix + Suffix, 높은 수치)
  L: 2개 (Prefix + Suffix, 최고 수치)

note: "장비 레벨은 기본 스탯에만 영향, 수식어는 등급에만 영향"
```

## 레벨 밴드

### 착용 레벨

```yaml
concept: "몬스터 레벨대 = 드랍 장비 착용 레벨"

level_bands:
  lv1_10: "초반 장비"
  lv11_20: "초중반"
  lv21_35: "중반"
  lv36_50: "중후반"
  lv51_70: "후반"
  lv71_99: "최종"

example:
  monster_lv15: "Lv11-20 착용 장비 드랍"
```

### 장비 네이밍

```yaml
naming_strategy:
  base_names: "반복 사용 가능"
  differentiation: "등급 + 옵션"

examples:
  - "녹슨 검 (C, Lv1)"
  - "빛 바랜 검 (UC, Lv18)"
  - "빛나는 검 (R, Lv37)"
  - "찰나의 검 (H, Lv51)"
  - "전설의 검 (L, Lv70)"

unique_names:
  grade: "H, L"
  purpose: "희소성, 수집 욕구"
```

## 드랍 시스템

### 드랍 확률

```yaml
base_rates:
  C: 65%
  UC: 20%
  R: 12%
  H: 2.5%
  L: 0.5%

modifiers:
  drop_rate_option: "직접 가산"
  rank_bonus: "계급별 보너스"

calculation:
  final_rate = base_rate * (1 + equipment_bonus + rank_bonus)
  desc: 타 등급 아이템 확률 증가 시 C등급 아이템 등장 확률이 줄어드는 개념
```

### 드랍 테이블

```yaml
monster_drop_table:
  monster_id: "slime_001"
  level: 5
  drops:
    - type: "equipment"
      grade_weights: [65, 20, 12, 2.5, 0.5]  # C, UC, R, H, L
      equipment_types: ["weapon", "armor", "ring"]
      level_range: [1, 10]
```

## 데이터 구조

> 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **Equipment**: 장비 정보 (등급, 타입, 착용 레벨, 기본 스탯, 옵션)
- **EquipmentOption**: 추가 옵션 (공격/방어/유틸리티 타입, 스탯, 수치)
- **등급 시스템**: C/UC/R/H/L 색상 기반 등급
- **부위별 장비**: 무기, 방어구, 목걸이, 반지, 벨트

## UI 요구사항

### 인벤토리

```yaml
display:
  - 장비 아이콘
  - 등급 (배경 색상)
  - 이름
  - 착용 레벨
  - 기본 스탯
  - 옵션 목록

sort:
  - 등급
  - 타입
  - 레벨
  - 획득 순서

filter:
  - 타입별
  - 등급별
  - 장착 가능 여부
```

### 장착 화면

```yaml
equipment_slots:
  weapon: 1
  armor: 1
  ring: 2
  neckless: 1
  belt: 1
  total: 6

interaction:
  - 클릭 시 비교 툴팁 노출
  - 장착 버튼 클릭 시 장착/장비 교체
```

## 시스템 연동

### 의존 시스템
- `character.md` - 영웅 장착
- `encounter.md` - 드랍 소스

### 제공 기능
- 스탯 증가
- 특수 효과 (드랍률 등)

## 밸런스 파라미터

### 장비 기본 스탯 공식

```yaml
concept: "장비 레벨과 등급에 따른 기본 스탯 계산"

formula:
  base_stat: "(equipment_level × grade_multiplier) + grade_base"

grade_multipliers:
  C:
    multiplier: 0.8
    base: 5
  UC:
    multiplier: 1.0
    base: 8
  R:
    multiplier: 1.3
    base: 12
  H:
    multiplier: 1.7
    base: 18
  L:
    multiplier: 2.2
    base: 25

examples:
  weapon_attack:
    C_lv1: "(1 × 0.8) + 5 = 6"
    C_lv50: "(50 × 0.8) + 5 = 45"
    R_lv1: "(1 × 1.3) + 12 = 13"
    R_lv50: "(50 × 1.3) + 12 = 77"
    L_lv1: "(1 × 2.2) + 25 = 27"
    L_lv50: "(50 × 2.2) + 25 = 135"
    L_lv100: "(100 × 2.2) + 25 = 245"

  armor_defense:
    C_lv10: "(10 × 0.8) + 5 = 13"
    R_lv20: "(20 × 1.3) + 12 = 38"
    L_lv50: "(50 × 2.2) + 25 = 135"

  neckless_hp:
    C_lv1: "(1 × 0.8) + 5 = 6 → HP +60"
    R_lv30: "(30 × 1.3) + 12 = 51 → HP +510"
    L_lv70: "(70 × 2.2) + 25 = 179 → HP +1790"

note: "HP 장비는 계산값 × 10"
```

### 수식어 옵션 값 범위

```yaml
concept: "수식어 값은 기본 스탯의 50% 평균, ±25% 범위"

formula:
  average_value: "base_stat × 0.5"
  min_value: "average_value × 0.75"
  max_value: "average_value × 1.25"
  variance: "±25%"

examples:
  강력한_검_R_lv30:
    base_attack: "(30 × 1.3) + 12 = 51"
    average_bonus: "51 × 0.5 = 25.5"
    min_bonus: "25.5 × 0.75 = 19"
    max_bonus: "25.5 × 1.25 = 32"
    result: "공격력 +19~32%"

  맹공의_검_L_lv50:
    base_attack: "(50 × 2.2) + 25 = 135"
    average_bonus: "135 × 0.5 = 67.5"
    min_bonus: "67.5 × 0.75 = 51"
    max_bonus: "67.5 × 1.25 = 84"
    result: "공격력 +51~84 (Flat)"

  행운의_반지_H_lv40:
    base_crit: "기본 10%"
    average_bonus: "10 × 0.5 = 5%"
    min_bonus: "5 × 0.75 = 3.75%"
    max_bonus: "5 × 1.25 = 6.25%"
    result: "드랍률 +3.75~6.25%"
```

### 등급별 수식어 값 보정

```yaml
grade_bonus_multiplier:
  C: 1.0  # 수식어 없음
  UC: 1.0
  R: 1.0
  H: 1.3  # 수식어 값 30% 증가
  L: 1.6  # 수식어 값 60% 증가

examples:
  R등급_강력한:
    base: "공격력 +25%"
    multiplier: 1.0
    final: "+25%"

  H등급_강력한:
    base: "공격력 +25%"
    multiplier: 1.3
    final: "+32.5%"

  L등급_강력한:
    base: "공격력 +25%"
    multiplier: 1.6
    final: "+40%"
```

### 수식어 테이블 예시

```yaml
prefix_modifiers:
  offensive_flat:
    - id: 1, name: "맹공의", stat: "attack", type: "flat", base_ratio: 0.5
    - id: 2, name: "날카로운", stat: "armor_penetration", type: "flat", base_ratio: 0.3

  offensive_percent:
    - id: 10, name: "강력한", stat: "attack", type: "percent", base_ratio: 0.5
    - id: 11, name: "치명적인", stat: "crit_rate", type: "percent", base_ratio: 0.5
    - id: 12, name: "파괴의", stat: "crit_damage", type: "percent", base_ratio: 0.4
    - id: 13, name: "신속한", stat: "evasion_pierce", type: "percent", base_ratio: 0.3

  defensive_flat:
    - id: 20, name: "견고한", stat: "defense", type: "flat", base_ratio: 0.5
    - id: 21, name: "생명의", stat: "hp", type: "flat", base_ratio: 0.5

  defensive_percent:
    - id: 30, name: "튼튼한", stat: "defense", type: "percent", base_ratio: 0.5
    - id: 31, name: "민첩한", stat: "evasion", type: "percent", base_ratio: 0.4

suffix_modifiers:
  utility:
    - id: 100, name: "행운", stat: "drop_rate_bonus", type: "percent", base_ratio: 0.3
    - id: 101, name: "번영", stat: "gold_bonus", type: "percent", base_ratio: 0.4
    - id: 102, name: "성장", stat: "exp_bonus", type: "percent", base_ratio: 0.4

note: "base_ratio는 장비 기본 스탯 대비 옵션 평균값 비율"
```

## 구현 체크리스트

- [ ] 장비 데이터 구조
- [ ] 등급 시스템
- [ ] 옵션 생성 로직
- [ ] 드랍 시스템
- [ ] 인벤토리 UI
- [ ] 장착 시스템
- [ ] 스탯 적용

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
