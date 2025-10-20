# Progression & Rank System

## Intent
- 영토 확장과 정착지 확보를 통해 계급을 올리고, 각 계급은 전투/경제 보너스를 제공한다.
- 계급은 플레이어의 장기 목표이자 컨텐츠 해금 조건으로 활용한다.
- 지표(영토 면적, 마을/요새 수)와 보너스 수치는 메타데이터 테이블로 관리한다.

## Rank Structure
| 계급 ID | 표시명 | 요구 조건 | 주요 보너스 | 데이터 출처 |
| --- | --- | --- | --- | --- |
| castellan | 성주 | 기본 계급 | 없음 | `data/rank_table.csv` |
| baron ~ emperor | 추후 정의 | 면적/마을/요새 요구치 증가 | 경험치, 드랍률, 이동 속도 등 | `data/rank_table.csv` |

> `data/rank_table.csv`는 요구 조건(`required_area`, `required_towns`, `required_fortresses`)과 보너스(`exp_bonus`, `drop_bonus`, 등)를 정의한다.

## Progression Loop
1. 영토를 확장하고 마을/요새를 점령해 조건을 충족한다.  
2. 조건이 만족되면 즉시 계급이 승급되며 보너스가 적용된다.  
3. 신규 계급은 상점/요새/메타 콘텐츠를 해금한다.  
4. 주기적으로 진행 상황을 확인하고 목표를 설정한다.

## Bonus Categories
- **전투 보너스**: 경험치, 드랍률, 골드 획득률.  
- **이동/생활 보너스**: 캠핑 효율, 이동 속도, 깃발 보유량.  
- **경제 보너스**: 상점 할인, 제작 비용 절감.  
- 보너스 합산 규칙은 `rank_bonus_rules.csv`에서 관리한다.

## Technical Boundaries
- 서버는 플레이어 상태(영토 면적, 정착지 수)를 주기적으로 재계산한다.  
- 계급 변동 시 SignalR을 통해 클라이언트에 알리고, 보상(예: 귀속 아이템)을 지급한다.  
- 계급 데이터는 메타 버전 manifest에 포함되며, 클라이언트는 최신 버전을 확인해 UI를 갱신한다.

## Dependencies
- `territory.md` : 영토 면적 계산.  
- `settlement.md` : 마을/요새 점령 수.  
- `economy.md` : 상점/제작 해금 조건.  
- `metadata-pipeline.md` : 계급 메타 데이터 동기화.

## Open Questions
- `rank_count` : MVP에 필요한 계급 수와 단계 구간 확정.  
- `bonus_scaling` : 후반부 보너스가 과도해지지 않도록 상한/감쇠 규칙 필요.  
- `decay_rules` : 계급 하락(미도입) 여부 결정.  
- `prestige_loop` : 엔드게임 전환(프레스티지/시즌제) 설계 필요 여부.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
