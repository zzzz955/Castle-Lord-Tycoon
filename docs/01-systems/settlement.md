# Settlements (Towns & Fortresses)

## Intent
- 탐험과 영토 확장을 통해 마을/요새를 확보하고, 회복·상점·제작 등 핵심 허브 기능을 제공한다.
- 요새는 전투형 도전 콘텐츠이며, 마을은 편의 기능 중심이다.
- 점령/해방 조건과 보상은 메타데이터로 관리되어 밸런스 조정이 용이하도록 한다.

## Settlement Types
| 유형 | 설명 | 주요 기능 | 데이터 출처 |
| --- | --- | --- | --- |
| Town | 기본 거점, 자동 점령 | 회복, 파티 관리, 상점(조건부), 워프 | `data/town_templates.csv` |
| Fortress | 보스 콘텐츠, 전투 후 개방 | 강화 재료, 고급 상점, 제작 | `data/fortress_templates.csv` |

## Player Loop
1. 영토를 연결해 중립 마을을 자동 점령하거나 요새를 발견한다.  
2. 요새는 최초 진입 시 전투를 클리어하면 개방된다.  
3. 개방된 마을/요새는 회복·상점·교환·제작 기능을 제공한다.  
4. 주기적으로 주간 상점/교환 재고가 갱신된다.

## Core Features
- **회복**: 마을에서 무료 회복, 요새에서 유료/포션 회복. 규칙은 `settlement_services.csv`.  
- **상점**: 일반/주간/계급 제한 상점. 재고와 가격은 `shop_inventory.csv`, `shop_rotation.csv`.  
- **교환**: 재료↔토큰 교환. 규칙은 `exchange_rules.csv`.  
- **제작**: 장비/소모품 제작. 레시피는 `crafting_recipes.csv`.  
- **퀘스트 게시판(미정)**: 반복 콘텐츠 추가를 고려.

## Technical Boundaries
- 점령 판정과 보상 지급은 서버에서 처리하며, 영토 연결 여부를 지속적으로 검증한다.  
- 정착지 데이터는 메타 버전 manifest에 포함되고, 클라이언트는 업데이트 시 재동기화한다.  
- 주요 API: `/settlements/state`, `/settlements/claim`, `/shops/purchase`, `/crafting/craft`.

## Dependencies
- `territory.md` : 자동 점령 조건과 연결 규칙.  
- `economy.md` : 상점/교환/제작 상세 기획.  
- `progression.md` : 계급 보너스, 상점 해금 조건.  
- `metadata-pipeline.md` : 정착지 템플릿 업데이트.

## Open Questions
- `fortress_difficulty_scaling` : 요새 보스 난이도 곡선 확정 필요.  
- `service_unlocks` : 각 기능(상점/제작) 해금 계급 및 비용.  
- `weekly_reset_schedule` : 주간 상점/교환 리셋 시각 및 보상 구조.  
- `quest_board` : 반복 퀘스트 시스템 도입 여부와 보상.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
