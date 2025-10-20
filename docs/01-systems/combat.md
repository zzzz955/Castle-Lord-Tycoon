# Combat System

## Intent
- 3×3 자동 전투를 기반으로 “배치 + 조합”을 즐기는 짧은 전투 루프를 제공한다.
- 서버가 전투 결과를 전권으로 판정하고, 클라이언트는 연출과 로그만 표시한다.
- 전투 난이도는 `battle_rules` 메타 데이터와 `monster_templates` 표를 통해 조정한다.

## Player Loop
1. 월드 탐험 중 조우가 발생하면 서버에 전투 시작을 요청한다.
2. 플레이어는 편성 화면에서 출전 영웅을 고르거나 이전 편성을 그대로 사용한다.
3. 전투는 최대 10라운드까지 자동으로 진행되며, 승패/보상 결과를 즉시 확인한다.
4. 전투 로그는 선택적으로 리플레이를 재생하거나 캠핑/마을에서 회복한다.

## Battlefield Layout
| 항목 | 설명 | 데이터 소스 |
| --- | --- | --- |
| Grid | 3×3 고정 | `data/battle_rules.csv: grid_width/grid_height` |
| Slot Priority | 1→9 순번 (전열→후열) | `battle_rules.csv: turn_priority` |
| Party Size | 기본 4명, 요새 보스 6명 | `battle_modes.csv` |

## Combat Flow
1. **초기화**  
   - 서버는 전투 참여 영웅/몬스터의 최종 스탯을 계산한다.  
   - `hero_final_stats.json`과 `monster_stats.json`을 사용하며, 장비/버프는 서버에서 재검증한다.
2. **라운드 진행**  
   - 우선순위에 따라 각 슬롯이 한 번씩 행동한다.  
   - 기본 공격, 속성 보정, 장비/고유 효과, 버프/디버프 해제 순으로 처리한다.  
   - 라운드 종료 후 생존 상태와 라운드 수를 갱신한다.
3. **종료 조건**  
   - 한쪽 전멸 → 즉시 종료.  
   - 10라운드 경과 → 패널티 후 강제 종료(패/무승부 정책은 `battle_rules.csv: round_limit_policy`).  
4. **보상 계산**  
   - 경험치, 골드, 전리품은 `reward_tables.csv`에서 로드한다.  
   - 파티 버프(경험치/드랍/골드)는 살아있는 영웅만 합산한다.

## Status Effects & Skills
- 전투 중 발동하는 모든 효과는 `combat_effects.csv`에 정의된 트리거/대상/지속 규칙을 따른다.
- 현재 MVP 범위에서는 능동 스킬이 없으며, 자동 발동 패시브/장비 효과만 존재한다.
- 향후 능동 스킬 도입 시 `skill_queue_rules` 표와 UI 설계가 필요하다.

## Technical Boundaries
- 전투 계산은 서버 전권이며, 클라이언트는 전투 로그를 통한 UI 재생만 허용된다.
- `client-server-contract.md`의 `/combat/start`, `/combat/resolve` API를 사용한다.
- 전투 로그 리플레이는 `combat_log` 테이블(또는 추후 선택한 로그 저장소)에 보관한다.

## Dependencies
- `world-exploration.md` : 조우 발생 조건 및 전투 진입 트리거.  
- `character.md` : 영웅 스탯 및 고유 효과 정의.  
- `equipment.md` : 장비 옵션과 전투 영향.  
- `progression.md` : 계급 보너스(경험치/드랍/골드).  
- `docs/02-data/` : `battle_rules`, `combat_effects`, `reward_tables`.  
- `metadata-pipeline.md` : 전투 관련 메타 버전 관리 및 업데이트 플로우.

## Open Questions
- `skill_queue_rules` : 능동 스킬 추가 시 행동 우선순위와 입력 방식 정의 필요.  
- `round_limit_policy` : 10라운드 초과 시 패/무승부/보상 변동 정책 확정 필요.  
- `combat_log_retention` : 리플레이 로그 저장 기간 및 압축 규칙 결정 필요.  
- `boss_mode_scaling` : 요새/보스 전투에서 추가 버프 혹은 HP 배수 적용 여부.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
