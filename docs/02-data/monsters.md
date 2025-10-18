# 몬스터 데이터

## 스키마

```typescript
interface Monster {
  id: string;
  name: string;

  // 스탯
  level: number;
  stats: {
    hp: number;
    attack: number;
    defense: number;
    critical_rate: number;
    critical_damage: number;
    block_rate: number;
    growth_hp: number;
    growth_attack: number;
    growth_defense: number;
  };
  element: "water" | "fire" | "earth" | "none";

  // 드랍
  dropTable: {
    gold: [number, number];  // [min, max]
    exp: number;
    items: ItemDrop[];
  };

  // 출현
  regions: string[];
  spawnTags: string[];
}

interface ItemDrop {
  itemId: string;
  rate: number;  // 0-100%
  count: [number, number];
}
```

## 몬스터 목록

### 초원 지대

```yaml
slime:
  id: "slime_001"
  name: "슬라임"
  level: 3
  stats: { hp: 30, attack: 8, defense: 2 }
  element: "water"
  drop:
    gold: [5, 10]
    exp: 30
    items:
      - { id: "slime_gel", rate: 30% }
      - { id: "common_sword", rate: 5% }

wolf:
  id: "wolf_001"
  name: "들늑대"
  level: 5
  stats: { hp: 50, attack: 12, defense: 4 }
  element: "none"
  drop:
    gold: [8, 15]
    exp: 50
```

### 검은 숲

```yaml
# 향후 작성
```

### 용의 협곡

```yaml
# 향후 작성
```

## 생성 규칙

```yaml
stat_scaling:
  hp: "level * growth + base"
  attack: "level * growth + base"
  defense: "level * growth + base"

difficulty_tiers:
  normal: 1.0x
  elite: 1.5x
  boss: 3.0x
```

---
**최종 수정**: 2025-10-19
**상태**: ⚪ 미작성 (예시만)
**작성자**: SangHyeok
