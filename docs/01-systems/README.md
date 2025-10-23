# Systems Documentation

각 시스템 문서는 핵심 의사결정과 기술 경계를 정리하며, 세부 수치/열 이름은 `docs/02-data/`에서 관리한다.

## Index
| 문서 | 요약 |
| --- | --- |
| `world-exploration.md` | 필드 탐험, 이동 동기화, 캠핑 |
| `encounter.md` | 전투 조우 트리거(10% 독립 확률) |
| `territory.md` | 깃발 설치/제거, 영토 데이터 구조 |
| `settlement.md` | 마을/요새 NPC 서비스 개요 |
| `shop.md` | 상점 탭(장비/공용/깃발) 규칙 |
| `tavern.md` | 영웅 모집, 파티 프리셋 관리 |
| `forge.md` | 강화/분해/보석/옵션/합성 |
| `character.md` | 영웅 타입, 성장치 공식 |
| `equipment.md` | 장비 등급, 부가 능력치, 보석 슬롯 |
| `hero-unique-effects.md` | 고유 효과 템플릿 구조 |
| `enemy.md` | 적 템플릿, 난이도 보정, 드랍 |
| `gems.md` | 보석 종류, 조합 규칙 |
| `progression.md` | 계급 요구량, 누적 혜택 |
| `economy.md` | 상점/대장간/술집 중심 경제 루프 |

## 작성 가이드
1. **구조 우선**: 구조와 결정 이유를 서술하고, 세부 수치는 02-data 문서로 이동한다.  
2. **참조 명시**: 다른 시스템과 연동되는 부분은 관련 문서를 링크하거나 파일명을 명시한다.  
3. **Stub 관리**: 미구현 항목은 Stub으로 표기하고 CSV를 생성하지 않는다.  
4. **교차 검토**: 변경 시 연관 문서(예: shop ↔ forge ↔ economy)를 함께 확인한다.

---
**최종 수정**: 2025-10-24  
**상태**: 개편  
**작성자**: SangHyeok
