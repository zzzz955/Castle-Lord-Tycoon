# World Exploration

## Intent
- 준 오픈월드(타일 기반)에서 탐험·조우·캠핑을 반복하며 영토를 확장한다.
- 플레이어는 시야 정보와 위험도를 기반으로 루트를 결정한다.
- 월드는 메타데이터로 정의되어, 서버/클라이언트가 동일한 타일 맵과 태그를 공유한다.

## World Model
| 항목 | 설명 | 데이터 출처 |
| --- | --- | --- |
| Tile Grid | 64×64 픽셀 타일, 워프 좌표 | `data/world_tileset.csv` |
| Biome Tags | 조우/보상/시야 값 결정 | `data/biome_tags.csv` |
| Fog State | 미탐색/탐색/영토 소유 3단계 | `data/fog_rules.csv` |

## Player Loop
1. 월드 맵을 이동하며 탐색되지 않은 타일을 밝힌다.  
2. 조우 주사위가 발동하면 전투/이벤트를 처리한다.  
3. 필요 시 캠핑으로 HP를 회복하거나 마을로 귀환한다.  
4. 깃발을 배치해 영토를 확보하고, 마을/요새를 자동 점령한다.

## Mechanics
- **이동**: 8방향 이동, 지형별 이동 속도는 `movement_modifiers.csv`.  
- **조우**: 바이옴 태그 기반 확률(`encounter_tables.csv`). 전투 이외의 이벤트 추가 예정.  
- **캠핑**: 설정된 안전 지역 또는 소모 아이템 사용 시 설치. 회복률과 조건은 `camping_rules.csv`.  
- **귀환**: 마을포탈/아이템 사용으로 즉시 귀환. 쿨다운은 `return_rules.csv`.

## Technical Boundaries
- 서버는 플레이어 위치를 5~10초 간격으로 수신하며, 월드 이벤트는 서버에서 검증한다.  
- 월드 타일 데이터는 클라이언트가 캐시하지만, 소유 상태/조우 결과는 서버 authoritative.  
- API/SignalR 이벤트는 `client-server-contract.md`와 `metadata-pipeline.md`에 정의된 manifest 버전에 의존한다.

## Dependencies
- `encounter.md` : 조우 타입과 전투 진입 로직.  
- `territory.md` : 깃발 배치, 영토 하이라이트.  
- `settlement.md` : 마을/요새 위치, 상호작용.  
- `metadata-pipeline.md` : 월드 타일/바이옴 데이터 동기화.

## Open Questions
- `camping_rules` : 안전 지역 판정과 회복 수치 확정 필요.  
- `event_types` : 전투 외 이벤트(보물, NPC) 추가 여부.  
- `fast_travel` : 마을 간 순간 이동 시스템 도입 시 조건/비용.  
- `world_size_scaling` : MVP에서 필요한 월드 크기와 타일 세트 범위 확정.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
