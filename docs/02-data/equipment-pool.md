# 장비 풀 데이터

## 스키마

```typescript
interface EquipmentTemplate {
  id: string;
  baseName: string;
  type: "weapon" | "armor" | "ring" | "neckless" | "belt";

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

---
**최종 수정**: 2025-10-19
**상태**: ⚪ 미작성 (예시만)
**작성자**: SangHyeok
