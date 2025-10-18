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
weapon: "공격력 +N"
armor: "방어력 +N"
neckless: "HP +N"
ring: "critical_rate +N%"
belt: "block_rate +N%"
```

### 추가 옵션

```yaml
offensive:
  - "추가 공격 +N"
  - "데미지 +N%"
  - "치명타 확률 +N%"
  - "치명타 데미지 +N%"
  - "속성 피해 +N%"

defensive:
  - "추가 방어 +N"
  - "피해 감소 +N%"
  - "치명타 피해 감소 +N%"
  - "회피 확률 +N%"
  - "HP +N"

utility:
  - "드랍률 +N%"
  - "경험치 +N%"
  - "골드 획득 +N%"
```

### 등급별 옵션 수

```typescript
interface EquipmentOptions {
  C: 0;   // 0-1개
  UC: 1;  // 0-1개
  R: 2;   // 1-2개
  H: 2;   // 2개 확정
  L: 2;   // 2개 확정, 높은 수치
}
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

```typescript
interface Equipment {
  id: string;
  templateId: string;

  // 기본 정보
  name: string;
  grade: "C" | "UC" | "R" | "H" | "L";
  type: "weapon" | "armor" | "ring" | "neckless" | "belt";
  requiredLevel: number;

  // 스탯
  baseStat: {
    type: "attack" | "defense" | "hp" | "critical_rate" | "block_rate";
    value: number;
  };

  // 옵션
  options: EquipmentOption[];

  // 메타
  equipped: boolean;
  equippedBy?: string;  // hero ID
}

interface EquipmentOption {
  type: "offensive" | "defensive" | "utility";
  stat: string;  // "추가 공격", "드랍률" 등
  value: number;
}
```

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

```yaml
stat_scaling:
  weapon_attack:
    C_lv1: 5
    C_lv50: 50
    L_lv1: 15
    L_lv50: 200

option_values:
  drop_rate:
    R: "3-5%"
    H: "5-10%"
    L: "10-20%"

  bonus_attack:
    R: "5-10"
    H: "10-20"
    L: "20-40"
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
