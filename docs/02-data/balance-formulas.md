# Balance Formulas

전투, 성장, 보상, 계급 진행에 사용되는 주요 공식을 정의하고 필요한 파라미터를 명시합니다. 실제 수치는 `data-glossary.md`에 언급된 테이블에서 관리합니다.

## Combat

| 항목 | 설명 | 파라미터 소스 |
| --- | --- | --- |
| 기본 피해 | `max(attacker_attack - defender_defense, min_damage)` | `battle_rules.json:min_damage` |
| 속성 보정 | 속성 상성 테이블에 따른 ±배수 | `combat_effects.json:element_matrix` |
| 치명타 | `damage × (1 + crit_bonus)` | `combat_effects.json:crit_multiplier`, 장비/영웅 보정 |
| 회피 | `1 - min(1, defender_evasion - attacker_evasion_pierce)` | `combat_effects.json` |

> 속성, 치명타, 회피 등은 모두 메타 테이블에서 조정하며, 공식은 서버에서 하드코딩된 구조만 공유합니다.

## Growth

| 항목 | 공식 | 파라미터 |
| --- | --- | --- |
| 경험치 곡선 | `base_exp × level^exp_exponent` | `hero_growth.json:base_exp`, `exp_exponent` |
| 스탯 성장 | `base + growth_rate × (level - 1)` | `hero_growth.json` |
| 장비 강화 | TBD (강화 정책 확정 후 작성) | `equipment_upgrade_rules.json` |

## Rewards

| 항목 | 공식 | 파라미터 소스 |
| --- | --- | --- |
| 드랍률 | `base_rate × (1 + equipment_bonus + rank_bonus)` (상한 적용) | `loot_tables.json`, `rank_table.json` |
| 경험치 분배 | `total_exp / alive_members` | `reward_tables.json`, 파티 상태 |
| 골드 보너스 | `base_gold × (1 + gold_bonus)` | 동일 |

상한/하한 값(`max_drop_rate`, `min_gold_reward`)은 `reward_rules.json`에서 관리합니다.

## Rank Progress

| 항목 | 공식 | 파라미터 |
| --- | --- | --- |
| 진행도 | `Σ(metric / requirement × weight)` | `rank_table.json`, `rank_progress_weights.json` |
| 요구치 | 계급별 영토/마을/요새 수 | `rank_table.json:requirements` |

> 진행도 계산은 UI 표시용으로만 사용하며, 실제 승급은 요구치를 모두 충족해야 발생합니다.

## Tuning Parameters

| 그룹 | 파라미터 | 기본값 | 관리 위치 |
| --- | --- | --- | --- |
| Combat | `min_damage`, `round_limit`, `boss_hp_multiplier` | TBD | `battle_rules.json` |
| Growth | `exp_exponent`, `growth_multiplier` | TBD | `hero_growth.json` |
| Rewards | `max_drop_rate`, `rank_bonus_cap` | TBD | `reward_rules.json` |
| Progression | `area_weight`, `town_weight`, `fortress_weight` | TBD | `rank_progress_weights.json` |

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
