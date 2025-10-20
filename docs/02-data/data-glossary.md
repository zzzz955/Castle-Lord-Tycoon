# Data Glossary

메타데이터 파이프라인에서 사용되는 주요 테이블/파일과 스프레드시트 탭을 정리합니다. 각 항목은 `tools/build-meta`가 참조하는 원본 스프레드시트와 변환된 JSON 경로를 함께 명시합니다.

> **진행 상태 안내**  
> 현재 MVP 기획서 확정이 우선 과제이므로, 아래 테이블은 설계 전 단계(`TODO`)로 유지합니다. 기획 검토가 마무리되는 즉시 상세 스키마 정의와 시트 설계를 다음 작업으로 진행합니다.

| ID | 스프레드시트 탭 | 출력 파일 | 설명 | 상태 |
| --- | --- | --- | --- | --- |
| `battle_rules` | `combat-battle-rules` | `meta/combat/battle_rules.json` | 전투 그리드, 라운드 제한, 행동 우선순위 | TODO |
| `combat_effects` | `combat-effects` | `meta/combat/effects.json` | 전투 효과 트리거/대상/지속 | TODO |
| `hero_templates` | `heroes-templates` | `meta/heroes/templates.json` | 영웅 기본 정보(속성, 역할, 기본 스탯) | TODO |
| `hero_growth` | `heroes-growth` | `meta/heroes/growth.json` | 레벨 경험치/스탯 곡선 | TODO |
| `hero_unique_effects` | `heroes-unique-effects` | `meta/heroes/effects.json` | 고유 패시브 정의 | TODO |
| `equipment_slots` | `equipment-slots` | `meta/equipment/slots.json` | 장비 슬롯/착용 제한 | TODO |
| `equipment_grades` | `equipment-grades` | `meta/equipment/grades.json` | 희귀도, 색상, 옵션 수 | TODO |
| `equipment_affixes` | `equipment-affixes` | `meta/equipment/affixes.json` | 옵션 ID, 카테고리, 값 범위 | TODO |
| `flag_types` | `territory-flags` | `meta/territory/flags.json` | 깃발 크기, 비용, 계급 요구 | TODO |
| `territory_rules` | `territory-rules` | `meta/territory/rules.json` | 연결 규칙, 회수 규칙 | TODO |
| `rank_table` | `progression-ranks` | `meta/progression/ranks.json` | 계급 조건 및 보너스 | TODO |
| `shop_inventory` | `economy-shop-inventory` | `meta/economy/shop_inventory.json` | 상점 재고 목록 | TODO |
| `shop_rotation` | `economy-shop-rotation` | `meta/economy/shop_rotation.json` | 상점 리셋 스케줄 | TODO |
| `exchange_rules` | `economy-exchange` | `meta/economy/exchange.json` | 교환 비율 및 제한 | TODO |
| `crafting_recipes` | `economy-crafting` | `meta/economy/crafting_recipes.json` | 제작 레시피 | TODO |
| `encounter_tables` | `encounter-tables` | `meta/encounters/tables.json` | 바이옴별 조우 확률 | TODO |
| `biome_tags` | `world-biomes` | `meta/world/biomes.json` | 바이옴 속성, 조우 계수 | TODO |
| `camping_rules` | `world-camping` | `meta/world/camping.json` | 캠핑 회복/설치 요건 | TODO |

> **상태 컬럼**은 메타 테이블이 구현되어 있는지 추적하기 위해 사용합니다. 스프레드시트 탭/출력 파일 이름은 `metadata-pipeline.md`에서 정의한 빌드 스크립트를 따라야 합니다.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
