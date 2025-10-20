# Territory System

## Intent
- 깃발 배치를 통해 탐험한 지역을 영구 점유하고, 마을·요새와 연결해 경제/계급 보너스를 얻는다.
- 플레이어는 비용과 경로 최적화를 고려해 확장을 계획하게 된다.
- 서버는 깃발 배치/제거를 검증하고, 메타데이터는 깃발 크기·비용·보너스를 제공한다.

## Player Loop
1. 탐험을 통해 미탐색 지역을 발견한다.  
2. 깃발 모드를 활성화하고 대상 타일에 S/M/L 깃발을 배치한다.  
3. 깃발을 통해 새로운 마을/요새와 연결되면 자동으로 점령한다.  
4. 불필요한 깃발은 회수하여 자원을 회복하거나 재배치한다.

## Territory Assets
| 항목 | 설명 | 데이터 출처 |
| --- | --- | --- |
| Flag Types | S(3×3), M(5×5), L(7×7) 크기, 비용, 계급 요구조건 | `data/flag_types.csv` |
| Inventory | 플레이어가 보유한 깃발 수 | `player_flags` DB 테이블 |
| Connectivity | 마을/요새 연결 규칙(직접 인접만 허용) | `territory_rules.csv` |
| Bonuses | 계급 보너스 외 자원/효율 버프 | `territory_bonuses.csv` |

## Mechanics
- **배치 제한**: 전투 중, 조우 중, 다른 플레이어 영역(추후)에는 배치 불가.  
- **겹침 처리**: 기존 깃발과 중첩될 경우 비용만 소모하고 영역은 덮어쓴다.  
- **회수**: 무료로 회수 가능하나 연결이 끊기면 마을/요새 점령이 해제된다.  
- **자동 점령**: 연결된 중립 마을은 즉시 점령되며 보상은 `settlement_rewards.csv` 기준으로 지급된다.

## Data Hooks
| 표 | 용도 |
| --- | --- |
| `data/flag_types.csv` | 깃발 ID, 크기, 비용, 계급 요구 |
| `data/territory_rules.csv` | 연결 기준, 대각선 허용 여부, 회수 패널티 |
| `data/territory_bonuses.csv` | 특정 면적/조합 달성 시 보너스 |

## Technical Boundaries
- 서버 `TerritoryService`가 배치 요청을 검증하고, 깃발 배치 시 영역을 다시 계산한다.  
- 클라이언트는 미리보기(미탐색, 탐색, 소유)를 렌더링하며, 계산은 서버 응답을 기반으로 한다.  
- API: `/territory/flags` (배치/회수), `/territory/state` (조회).

## Dependencies
- `world-exploration.md` : 탐험/시야 시스템과 좌표 체계 공유.  
- `settlement.md` : 마을/요새 자동 점령 및 보상.  
- `progression.md` : 계급 요구조건 및 보너스 연동.  
- `metadata-pipeline.md` : 깃발 메타 버전 관리.

## Open Questions
- `diagonal_connectivity` : 대각선 연결 허용 여부.  
- `maintenance_cost` : 장기 점령 시 유지비 부과 여부.  
- `territory_events` : 소유지에 발생하는 이벤트/침략 컨텐츠 도입 여부.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
