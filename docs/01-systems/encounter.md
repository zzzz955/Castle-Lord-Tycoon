# Encounter System

## Intent
- 탐험 중 바이옴/시간/위험도에 따라 전투·이벤트가 발생하도록 해 월드에 변주를 준다.
- 전투 이외에도 자원 이벤트, NPC, 던전 입구 등 확장 가능성을 열어둔다.
- 트리거 확률과 결과는 메타데이터 테이블에서 관리하여 조정 가능하게 한다.

## Encounter Types
| 타입 | 설명 | 기본 결과 | 데이터 출처 |
| --- | --- | --- | --- |
| Battle | 몬스터 전투로 진입 | 전투 결과에 따른 보상 | `data/encounter_tables.csv` |
| Event | 스토리/선택형 이벤트 | 보상/패널티/플래그 | `data/event_scripts.csv` |
| Resource | 채집/보급 이벤트 | 자원 획득/소모 | `data/resource_events.csv` |

## Trigger Logic
- 기본 확률은 바이옴 태그와 플레이어 위험도 기반으로 계산한다 (`encounter_tables.csv`).  
- 연속 조우 방지를 위한 완충 규칙은 `encounter_cooldown.csv`.  
- 시간/날씨(추후 도입) 변수를 사용하려면 `world_state.md`와 연동한다.

## Flow
1. 플레이어가 이동할 때마다 서버가 조우 판정을 수행한다.  
2. 조우 발생 시 타입을 결정하고, 필요하면 추가 데이터(이벤트 스크립트)를 로드한다.  
3. 전투 타입이면 `combat.md`로 흐름을 위임하고, 이벤트 타입이면 선택지를 표시한다.  
4. 결과에 따라 보상/패널티를 적용하고 로그에 기록한다.

## Technical Boundaries
- 서버 authoritative: 조우 판정과 결과 적용은 서버에서 실행.  
- 클라이언트는 연출/UI만 담당하며, 조우 데이터 버전은 manifest와 함께 동기화한다.  
- API: `/world/encounter/resolve` (이벤트 선택 결과 보고용).

## Dependencies
- `world-exploration.md` : 바이옴 태그, 이동 주기.  
- `combat.md` : 전투 조우 처리.  
- `economy.md` : 자원/보상 타입 정의.  
- `metadata-pipeline.md` : 조우/이벤트 테이블 동기화.

## Open Questions
- `non-combat_events` : MVP에서 포함할 이벤트 종류와 보상 범위.  
- `encounter_spike_control` : 고난도 지역에서 연속 전투 방지 규칙.  
- `event_scripting` : 이벤트 스크립트 포맷(JSON vs 노드 그래프) 결정.  
- `co-op_scaling` : 멀티 확장 시 조우 난이도 조정 방식(미정).

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
