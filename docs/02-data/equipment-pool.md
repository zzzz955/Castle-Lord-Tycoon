# 장비 풀 데이터

## 스키마

```typescript
interface EquipmentTemplate {
  id: string;
  baseName: string;
  type: "weapon" | "armor" | "accessory";

  levelBands: LevelBand[];
}

interface LevelBand {
  levelRange: [number, number];
  variants: EquipmentVariant[];
}

interface EquipmentVariant {
  grade: "C" | "UC" | "R" | "H" | "L";
  name: string;  // 등급별 고유 이름
  baseStat: number;
  possibleOptions: OptionPool[];
}
```

## 무기

### 검

```yaml
base_name: "검"

level_bands:
  lv_1_10:
    - grade: C
      name: "녹슨 검"
      baseStat: 5
      options: []

    - grade: UC
      name: "낡은 검"
      baseStat: 8
      options: ["공격+3"]

    - grade: R
      name: "빛나는 검"
      baseStat: 12
      options: ["공격+5", "치명타+5%"]

  lv_11_20:
    - grade: R
      name: "강철 검"
      baseStat: 20
      options: ["공격+8", "치명타+8%"]

    - grade: H
      name: "기사의 검"
      baseStat: 30
      options: ["공격+12", "추가공격+10"]
```

## 방어구

```yaml
# 향후 작성
```

## 악세서리

```yaml
# 향후 작성
```

## 옵션 풀

```yaml
offensive_options:
  - "공격 +N"
  - "치명타 +N%"
  - "추가공격 +N"

defensive_options:
  - "방어 +N"
  - "피해감소 +N%"

utility_options:
  - "드랍률 +N%"
  - "경험치 +N%"
  - "골드 +N%"
```

---
**최종 수정**: 2025-01-15
**상태**: ⚪ 미작성 (예시만)
**작성자**: SangHyeok
