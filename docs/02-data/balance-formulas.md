# 밸런스 공식

## 전투 계산

### 데미지

```typescript
function calculateDamage(attacker: Unit, defender: Unit): number {
  // 기본 데미지
  let baseDamage = attacker.attack - defender.defense;
  if (baseDamage < 1) baseDamage = 1;

  // 속성 상성 (±30%)
  const elementBonus = getElementBonus(attacker.element, defender.element);
  const finalDamage = baseDamage * (1 + elementBonus);

  return Math.floor(finalDamage);
}

function getElementBonus(atk: Element, def: Element): number {
  const advantages = {
    "water>fire": 0.3,
    "fire>earth": 0.3,
    "earth>water": 0.3,
    "water<earth": -0.3,
    "fire<water": -0.3,
    "earth<fire": -0.3,
  };
  const key = `${atk}>${def}` || `${atk}<${def}`;
  return advantages[key] || 0;
}
```

## 성장 곡선

### 경험치

```typescript
function getRequiredExp(level: number): number {
  return Math.floor(100 * Math.pow(level, 1.2));
}
```

### 스탯 성장

```typescript
function getHeroStat(base: number, growth: number, level: number): number {
  return Math.floor(base + growth * (level - 1));
}

// 예시
// ★1 영웅: base=50, growth=5 → Lv10 = 50+5*9 = 95
// ★6 영웅: base=120, growth=15 → Lv10 = 120+15*9 = 255
```

## 드랍 확률

```typescript
function calculateDropRate(
  baseRate: number,
  equipmentBonus: number,
  rankBonus: number
): number {
  const total = baseRate * (1 + equipmentBonus + rankBonus);
  return total;  // 증가가 아닌 기본 확률에 (1 + total)만큼의 값 곱연산, 총 합 100%가 넘는 수치의 경우 C등급 무기 확률에서 차감
}
```

## 계급 진행

```typescript
function calculateRankProgress(
  area: number,
  towns: number,
  fortresses: number
): number {
  // 가중치 조합
  const areaWeight = 0.5;
  const townWeight = 0.3;
  const fortressWeight = 0.2;

  // 정규화 (예시)
  const maxArea = 10000;
  const maxTowns = 15;
  const maxFortresses = 20;

  const areaProgress = (area / maxArea) * areaWeight;
  const townProgress = (towns / maxTowns) * townWeight;
  const fortressProgress = (fortresses / maxFortresses) * fortressWeight;

  return areaProgress + townProgress + fortressProgress;
}
```

## 조정 가능 파라미터

```yaml
combat:
  element_bonus: 0.3  # ±30%
  min_damage: 1

growth:
  exp_curve_exponent: 1.2
  stat_growth_multiplier: 1.0

drop:
  max_drop_rate: 1.0  # 100%

progression:
  area_weight: 0.5
  town_weight: 0.3
  fortress_weight: 0.2
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
