# Economy Systems (Shop / Exchange / Crafting)

## Intent
- 전투 외 플레이 루프를 지탱하는 재화 흐름과 소비처를 정의한다.
- 상점, 교환, 제작은 서로 보완적으로 작동하며, 메타 업데이트를 통해 주기적으로 변화를 준다.
- 모든 가격, 재고, 재료 배율은 메타데이터로 관리한다.

## System Overview
| 시스템 | 목적 | 주요 재화 | 데이터 출처 |
| --- | --- | --- | --- |
| Shop | 즉각적인 장비/소모품 구매 | Gold, Tokens | `data/shop_inventory.csv`, `data/shop_rotation.csv` |
| Exchange | 자원 전환, 토큰 ↔ 재료 | Exchange Tokens, Resource Items | `data/exchange_rules.csv` |
| Crafting | 장비/소모품 제작 | Craft Materials, Gold | `data/crafting_recipes.csv`, `data/crafting_facilities.csv` |

## Player Loop
1. 전투/퀘스트에서 골드·재료·토큰을 획득한다.  
2. 마을/요새에서 상점에 들러 필요한 장비/소모품을 구매한다.  
3. 부족한 재료는 교환을 통해 변환하거나, 제작 시설에서 장비를 제작한다.  
4. 주간 리셋 시 상점/교환 재고가 갱신되어 반복 플레이 동기를 제공한다.

## Shops
- **분류**: 일반 상점, 요새 상점(고급 재화), 주간 상점.  
- **재고**: `shop_inventory.csv`에서 아이템 ID, 가격, 상점 등급을 관리한다.  
- **리셋**: `shop_rotation.csv`에서 주기/계급 요구조건 정의.  
- **할인**: 계급 보너스(`rank_table.csv`)를 통해 가격 변동을 적용한다.

## Exchange
- **사용처**: 잉여 재료를 토큰으로 전환하거나 특정 재료/깃발을 구입.  
- **비율**: `exchange_rules.csv`에 입력/출력 아이템, 수수료, 일일 제한을 기록한다.  
- **제한**: NPC 친밀도, 계급 등 잠금 조건은 `exchange_unlocks.csv`.

## Crafting
- **시설**: 요새/특수 마을에서 해금되는 제작소(`crafting_facilities.csv`).  
- **레시피**: `crafting_recipes.csv`에 재료, 제작 시간, 결과물 ID 기록.  
- **제작 큐**: 실시간 제작 대기열이 필요한 경우 `crafting_queue_rules.csv`에서 정의한다.

## Technical Boundaries
- 모든 거래는 서버 검증을 거치며, 로그(`economy_transactions` 테이블)를 남긴다.  
- 클라이언트는 메타 버전 변화에 따라 상점/교환/제작 UI를 재동기화한다.  
- 주요 API: `/shop/purchase`, `/exchange/trade`, `/crafting/start`, `/crafting/collect`.

## Dependencies
- `settlement.md` : 각 시설이 어느 정착지에서 해금되는지 정의.  
- `progression.md` : 계급 요구조건 및 보너스 영향.  
- `equipment.md` : 제작 결과물/장비 ID 참조.  
- `metadata-pipeline.md` : 경제 관련 테이블 버전 관리.

## Open Questions
- `weekly_rotation_size` : 주간 상점 재고 수량과 희귀도 구성.  
- `crafting_time_model` : 제작 시간/즉시 완성 비용 정책.  
- `exchange_cap` : 일일/주간 교환 제한치.  
- `premium_currency` : 유료 재화 도입 여부와 사용처.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
