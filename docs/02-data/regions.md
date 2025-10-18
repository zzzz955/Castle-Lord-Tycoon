# 지역 데이터

## 스키마

```typescript
interface Region {
  id: string;
  name: string;
  difficulty: number;

  biomes: Biome[];
  towns: Town[];
  fortresses: Fortress[];

  recommendedLevel: [number, number];
}

interface Biome {
  tag: string;  // "초원_풀숲"
  encounterRate: number;
  spawnTable: string;  // spawn table ID
}
```

## 지역 목록

### 초원 지대

```yaml
id: "starter_plains"
name: "초원 지대"
difficulty: 1
recommended_level: [1, 10]

biomes:
  - tag: "초원_풀숲"
    encounter_rate: 20%
    spawns: ["slime", "wolf", "bee"]

  - tag: "초원_도로"
    encounter_rate: 10%
    spawns: ["wolf"]

towns:
  - { id: "town_start", name: "시작 마을", owned: true }
  - { id: "town_plains_01", name: "초원 전초기지" }
  - { id: "town_plains_02", name: "농장 마을" }

fortresses:
  - { id: "fort_plains_01", name: "폐허 요새" }
```

### 검은 숲

```yaml
id: "dark_forest"
name: "검은 숲"
difficulty: 3
recommended_level: [15, 25]

# 향후 작성
```

### 철의 요새

```yaml
# 향후 작성
```

### 용의 협곡

```yaml
# 향후 작성
```

---
**최종 수정**: 2025-10-19
**상태**: ⚪ 미작성 (예시만)
**작성자**: SangHyeok
