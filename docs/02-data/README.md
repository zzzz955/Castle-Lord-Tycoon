# Data Documentation (MVP)

## 범위
- 현재 시스템 문서(01-systems)를 기준으로 **MVP에 직접 사용되는 CSV**만 관리한다.
- 아직 도입되지 않은 기능(교환, 제작, 이벤트 조우 등)은 CSV를 생성하지 않는다.

## 핵심 파일
| 파일 | 용도 | 관련 시스템 |
| --- | --- | --- |
| `enemy_encounters.csv` | 필드별 전투 조우 테이블 | encounter, enemy |
| `shop_equipment_pool.csv` | 마을별 장비 슬롯 풀 | shop |
| `shop_shared_pool.csv` | 공용 슬롯 출현 품목/확률 | shop, economy |
| `shop_flag_inventory.csv` | 깃발 판매/환매 정보 | shop, territory |
| `forge_upgrade_rules.csv` | 강화 확률, 보호석 사용 | forge, equipment |
| `forge_synthesis_rules.csv` | 장비 합성 확률/골드 | forge |
| `tavern_recruitment_tiers.csv` | 영웅 모집 쿨타임/성급 분포 | tavern, character |
| `flag_templates.csv` | 깃발 크기/가격/계급 조건 | territory, progression |
| `rank_requirements.csv` | 계급 요구 영토, 누적 보너스 | progression |

## 작성 가이드
1. **구조 우선**: 컬럼 이름, 데이터 타입, 제약 조건을 먼저 정의하고 수치는 추후 조정한다.  
2. **팁**: 확률은 basis point(10000 단위), 퍼센트는 정수 고정소수(×100)로 저장한다.  
3. **버전 관리**: CSV 변경 시 `metadata-pipeline.md` 절차에 따라 manifest를 갱신한다.  
4. **Stub 관리**: 미구현 기능에 대한 CSV는 생성하지 않고, Stub 표기만 남긴다.

---
**최종 수정**: 2025-10-24  
**상태**: MVP 범위 정리  
**작성자**: SangHyeok
