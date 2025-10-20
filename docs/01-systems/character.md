# Hero System

## Intent
- 플레이어가 수집·육성·편성의 세 축을 순환하며 파티의 정체성을 만든다.
- 희귀도에 따라 성장 효율과 고유 효과가 달라지고, 파티 합성으로 메타를 조정한다.
- 모든 스탯·효과 정의는 메타데이터 테이블과 서버 검증 로직을 공유한다.

## Pillars
| Pillar | 설명 | 관련 문서/데이터 |
| --- | --- | --- |
| Collection | 영웅 획득 경로(전투 드랍, 요새 보상, 상점) | `economy.md`, `data/hero_sources.csv` |
| Growth | 레벨·진화·재련 등 성장 요소 | `progression.md`, `data/hero_growth.csv` |
| Identity | 속성, 역할, 고유 패시브 | `data/hero_templates.csv`, `combat_effects.csv` |

## Player Loop
1. 전투/퀘스트/상점에서 영웅을 획득한다.  
2. 경험치를 투자해 레벨을 올리고, 필요 시 재료로 승급한다.  
3. 장비와 파티 배치를 조정해 전투/계급 목표를 공략한다.  
4. 일정 주기로 메타 데이터를 동기화해 새로운 영웅/균형 패치를 수신한다.

## Progression Tracks
- **Level**: 영웅별 `level_cap`, `exp_curve`는 `hero_growth.csv`에서 관리한다.  
- **Rank (Star Grade)**: 1~6성 등급, 승급 조건/비용은 `hero_rank_requirements.csv`.  
- **Rebirth (Future)**: 동일 영웅을 중복 획득해 강화하는 시스템, 현재는 예약 상태.  
- **Skill Unlocks**: 4성 이상 고유 패시브/장비 슬롯 확장은 `combat_effects.csv`, `equipment_slots.csv`를 참조한다.

## Party Management
- 파티 슬롯은 기본 4명, 계급/시설 업그레이드로 확장 가능 (`progression.md`, `data/party_rules.csv`).  
- 파티 버프 합산 규칙: 같은 타입 중복 허용 여부는 `party_modifiers.csv`에서 관리한다.  
- 자동 편성 규칙(권장 전투력, 역할 균형)은 클라이언트 편의 기능으로 추후 정의한다.

## Data Hooks
| 표/파일 | 용도 |
| --- | --- |
| `data/hero_templates.csv` | 영웅 ID, 이름, 속성, 역할, 기본 스탯 |
| `data/hero_growth.csv` | 레벨별 필요 경험치, 스탯 성장 곡선 |
| `data/hero_rank_requirements.csv` | 등급 승급 비용/재료 |
| `data/hero_unique_effects.csv` | 고유 패시브 정의(트리거, 효과, 지속) |
| `data/hero_sources.csv` | 획득 경로(드랍 테이블 레퍼런스, 상점 재고 ID 등) |

## Technical Boundaries
- 서버의 `HeroService`가 스탯 계산을 전권으로 담당하며, 클라이언트는 캐시된 값을 UI용으로만 사용한다.  
- API: `client-server-contract.md`의 `/heroes`, `/party` 관련 엔드포인트.  
- 메타 데이터 버전은 `metadata-pipeline.md`의 manifest를 통해 동기화된다.

## Open Questions
- `rebirth_rules` : 재생/중복 강화 시스템 도입 시 성장 한계와 보상 설계 필요.  
- `auto_party_builder` : 추천 편성 알고리즘 적용 여부 및 우선순위 기준.  
- `skill_active_support` : 능동 스킬 추가 시 입력/쿨다운/에너지 시스템 정의.  
- `hero_pool_diversity` : 1인 개발 범위 내에서 MVP에 필요한 최소 영웅 수 확정.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
