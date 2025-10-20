# Equipment Template Specification

장비 관련 메타데이터는 아래 구조를 따른다. 모든 값은 스프레드시트에서 관리하며, 샘플 값은 문서에 직접 기재하지 않는다.

## Schema Overview

```typescript
interface EquipmentTemplate {
  id: string;                // Unique equipment identifier
  slot_id: string;           // e.g. weapon, armor, necklace, ring, belt
  level_band_id: string;     // References level bracket (e.g. lv_1_10)
  grade_id: string;          // References rarity (C, UC, R, H, L)
  display_name: LocalizedString;
  base_stat_id: string;      // Primary stat (attack, defense, hp, etc.)
  base_stat_value: number;   // Retrieved from grade/base tables
  affix_pool_ids: string[];  // References entries in equipment_affixes.csv
  tags: string[];            // Optional tag list for filters (elemental, faction)
}
```

## Source Tables

| 파일 | 설명 |
| --- | --- |
| `equipment_slots.csv` | 슬롯 정의(착용 제한, 중복 허용 여부, 아이콘) |
| `equipment_grades.csv` | 희귀도, 색상, 옵션 수, 기본 배율 |
| `equipment_level_bands.csv` | 레벨 범위별 추천 스탯 값/드랍 지역 |
| `equipment_templates.csv` | 장비 인스턴스(위 스키마) |
| `equipment_affixes.csv` | 옵션 ID, 카테고리, 값 범위 |
| `equipment_affix_rules.csv` | 희귀도별 옵션 개수, 범위 배수 |

## Authoring Guidelines

- **Base Stat**: `equipment_base_stats.csv`에서 슬롯·등급 조합마다 기본값을 계산하고, 템플릿에는 해당 ID를 참조합니다.  
- **Affix Pool**: 옵션을 직접 문자열로 적지 않고 `equipment_affixes.csv`의 ID를 배열로 넣습니다.  
- **Level Bands**: 장비 레벨 범위는 `equipment_level_bands.csv`에서 정의하고, 드랍 테이블이 이를 참조합니다.  
- **Localization**: `display_name`은 다국어 지원을 위해 JSON 객체(`{"ko": "...", "en": "..."}`)로 관리합니다.

## Export Targets

- `meta/equipment/slots.json`  
- `meta/equipment/grades.json`  
- `meta/equipment/templates.json`  
- `meta/equipment/affixes.json`  
- `meta/equipment/manifest.json` (버전/해시/생성 시각)

## Validation

- `tools/build-meta` 실행 시 중복 ID, 잘못된 참조, 옵션 수 초과 여부를 검증합니다.  
- 각 산출물에는 `meta_version`이 포함되어 서버가 업데이트를 통지할 수 있게 합니다.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
