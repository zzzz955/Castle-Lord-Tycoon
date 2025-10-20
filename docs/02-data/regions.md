# Region & Biome Specification

월드 탐험 시스템에서 사용하는 지역/바이옴/정착지 메타데이터를 정의합니다. 모든 값은 스프레드시트에서 유지합니다.

## Region Schema

```typescript
interface RegionTemplate {
  id: string;
  display_name: LocalizedString;
  difficulty: number;               // 1~10 scale for matchmaking
  recommended_level_min: number;
  recommended_level_max: number;
  biome_ids: string[];               // References biome_tags.csv
  town_ids: string[];                // References settlement templates
  fortress_ids: string[];            // References settlement templates
  unlock_conditions: string[];       // e.g. rank requirements, story flags
}
```

## Biome Schema

```typescript
interface BiomeTag {
  id: string;                        // e.g. plains_grass
  encounter_table_id: string;        // Reference to encounter_tables.csv
  movement_modifier: number;         // Multiplier for movement speed
  visibility_modifier: number;       // Fog expansion multiplier
  notes: string;
}
```

## Source Tables

| 파일 | 설명 |
| --- | --- |
| `region_templates.csv` | 지역 기본 정보 및 참조 ID |
| `biome_tags.csv` | 바이옴 속성, 조우 테이블 매핑 |
| `region_settlements.csv` | 지역별 마을/요새 배치 |
| `region_links.csv` | 지역 간 이동/해금 조건 |

## Export Targets

- `meta/world/regions.json`  
- `meta/world/biomes.json`  
- `meta/world/region_links.json`

## Authoring Notes

- 지역/바이옴 ID는 `snake_case`로 작성한다.  
- 정착지 ID는 `settlement.md`에서 정의된 템플릿과 동일해야 한다.  
- 난이도와 추천 레벨은 `balance-formulas.md`의 진행 곡선과 일치하도록 유지한다.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
