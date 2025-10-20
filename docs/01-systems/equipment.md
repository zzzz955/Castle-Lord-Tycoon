# Equipment System

## Intent
- 장비는 영웅 역할을 강화하고 플레이어의 파밍 동기를 부여하는 핵심 시스템이다.
- 희귀도와 옵션 조합을 통해 수평/수직 성장을 동시에 제공한다.
- 모든 수치는 메타데이터 테이블에서 관리하고, 서버는 장비 합성/강화 로직을 검증한다.

## Structure Overview
| 요소 | 설명 | 데이터 출처 |
| --- | --- | --- |
| Slot | 무기, 방어구, 목걸이, 반지(2), 벨트 | `data/equipment_slots.csv` |
| Grade | C/UC/R/H/L 등급, 색상/드랍률/옵션 수 | `data/equipment_grades.csv` |
| Base Stat | 슬롯별 고정 주스탯 | `data/equipment_base_stats.csv` |
| Affix Pool | Prefix/Suffix 옵션, 값 범위 | `data/equipment_affixes.csv` |

## Player Loop
1. 전투/요새/상점/제작에서 장비를 획득한다.  
2. 장비를 착용해 영웅 스탯을 보강하고, 필요 시 옵션을 재롤한다.  
3. 중복 장비는 강화/분해해 재료를 확보한다.  
4. 메타 업데이트 시 새 장비 풀을 다운로드하고 파티 구성을 조정한다.

## Mechanics
> **강화·분해 기획 스텁**  
> 강화, 재련, 분해 규칙은 별도 전용 기획서가 필요합니다. 본 문서는 MVP 범위 확정 전까지 구조만 정의하며, 상세 로직은 후속 문서에서 다룹니다.
- **착용 규칙**: 슬롯, 클래스 제한, 파티 조건 등은 `equipment_slots.csv`와 `equipment_restrictions.csv`에 정의한다.  
- **옵션 롤링**: 희귀도별 옵션 수/값 범위는 `equipment_affix_rules.csv`에서 관리한다.  
- **강화/재련**: 강화 재료 및 확률은 `equipment_upgrade_rules.csv`에 기록한다.  
- **드랍 테이블**: 몬스터/요새/상점별 장비 ID 드랍률은 `loot_tables.csv`와 `shop_inventory.csv`에서 참조한다.

## Technical Constraints
- 착용/해제/강화 API는 `client-server-contract.md`에서 `/heroes/{id}/equipment`, `/equipment/upgrade` 등으로 명세한다.  
- 서버는 장비 변동 시 영웅 스탯을 재계산하고 Dirty Flag를 설정한다.  
- 클라이언트는 메타버전 변경 시 `equipment_manifest.json`을 기준으로 데이터 최신 여부를 확인한다.

## Open Questions
- `equipment_upgrade_rules` : 강화 성공/실패, 파괴 여부에 대한 정책 확정 필요.  
- `set_effects` : 세트 효과 도입 시 데이터 구조 및 UI 설계.  
- `legendary_uniques` : 고유 장비(고정 옵션) 추가 시 밸런스 검토.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
