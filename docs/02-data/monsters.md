# Monster Template Specification

몬스터 메타데이터는 전투 시스템과 보상 시스템에서 사용됩니다. 모든 값은 스프레드시트에서 관리하며, 여기서는 스키마와 작성 규칙만 정의합니다.

## Schema

```typescript
interface MonsterTemplate {
  id: string;
  display_name: LocalizedString;
  faction_id: string | null;
  level_band_id: string;          // References region/level range
  element_id: string;             // water, fire, earth, none
  base_stats: {
    hp: number;
    attack: number;
    defense: number;
  };
  scaling: {
    hp_growth: number;
    attack_growth: number;
    defense_growth: number;
  };
  behavior_profile_id: string;    // AI / pattern reference
  loot_table_id: string;          // References loot_tables.csv
  tags: string[];                 // biome, boss, elite, etc.
}
```

## Source Tables

| 파일 | 설명 |
| --- | --- |
| `monster_templates.csv` | 위 스키마에 해당하는 기본 데이터 |
| `monster_level_bands.csv` | 지역/난이도별 레벨 범위, 체력 배수 |
| `monster_behavior_profiles.csv` | AI 패턴, 스킬 세트 |
| `loot_tables.csv` | 몬스터 드랍 목록 |
| `monster_spawn_rules.csv` | 지역/바이옴별 출현 확률 |

## Authoring Notes

- **Faction/Tag**: 스폰 규칙, 이벤트, 업적 등에서 필터링할 수 있도록 태그를 적극 활용합니다.  
- **Scaling**: 성장 파라미터는 `balance-formulas.md`의 성장 공식과 일치해야 합니다.  
- **Behavior Profiles**: 전투 패턴(순차 공격, 버프, 광역 등)은 `monster_behavior_profiles.csv`에서 정의합니다.  
- **Localization**: `display_name`은 다국어 대응을 위해 JSON 객체로 관리합니다.

## Export Targets

- `meta/monsters/templates.json`  
- `meta/monsters/spawn_rules.json`  
- `meta/loot/loot_tables.json`

## Validation Checklist

- ID 중복, 잘못된 참조(레벨 밴드, 행동 프로필, 루트 테이블).  
- 성장 파라미터가 음수가 아닌지, 드랍률 합계가 1.0 이하인지 확인.  
- 보스/엘리트 태그에 맞춰 HP/공격 배수가 적용되었는지 검증.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
