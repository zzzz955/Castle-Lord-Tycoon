# Castle Lord Tycoon – Documentation Guide

이 저장소의 문서는 1인 개발 환경을 기준으로 **컨셉 → 시스템 정의 → 데이터/기술 명세** 세 계층으로 나뉩니다. 각 계층은 서로를 참조하지만 내용을 중복 기록하지 않습니다.

## 0. 문서 계층

| 계층 | 위치 | 목적 | 대표 문서 |
| --- | --- | --- | --- |
| Concept | `docs/00-overview/` | 게임 방향성과 판타지, 핵심 루프 정의 | `game-concept.md` |
| Systems | `docs/01-systems/` | 플레이어가 체감하는 규칙, 입력/출력, 시스템 간 의존성 | `combat.md`, `world-exploration.md` |
| Data Specs | `docs/02-data/` | 실제 키/필드 명세, 수치 테이블, 스프레드시트 참조 | `data-glossary.md`, `balance-formulas.md` |
| UX | `docs/03-ui/` | UI 흐름, 온보딩 시나리오, 정보 구조 | `ui-design.md`, `onboarding.md` |
| Technical | `docs/04-technical/` | 아키텍처, API 계약, 메타 파이프라인, 배포 | `architecture.md`, `metadata-pipeline.md` |

> **핵심 원칙**  
> 1. **시스템 문서**는 구조·흐름·디자인 의도를 설명하고, 실제 변수/필드명은 `docs/02-data/`에서 정의된 항목을 인용합니다.  
> 2. 모든 수치·확률·레벨 곡선은 **스프레드시트 원본 + 변환된 JSON**(메타데이터)에서 관리합니다. 문서에는 “어떤 표에 저장되는지”만 서술합니다.  
> 3. 구현 세부 제약(API 필드, DB 스키마 등)은 기술 문서에 기록하고, 시스템 문서에서는 해당 문서를 링크합니다.

## 1. 작성 규칙

- **포맷**: Markdown, 필요 시 `yaml`/`json` 코드블록은 구조 예시용으로만 사용합니다.  
- **상태 블록**: 모든 문서 하단에 `최종 수정 / 상태 / 작성자` 메타 정보를 유지합니다.  
- **명명법**: 데이터 키는 `snake_case`, 스프레드시트 탭은 `kebab-case`로 통일합니다.  
- **Open Questions**: 결정되지 않은 항목은 각 문서 말미에 bullet로 정리해 추후 해소합니다.  
- **Changelog**: 실제 수치/스키마가 바뀌면 `docs/02-data/CHANGELOG.md` 또는 관련 기술 문서의 변경 이력을 함께 갱신합니다.

## 2. 작업 흐름

> **우선 순위 노트**  
> 현재는 MVP 범위 기획 완성과 시스템 합의가 최우선입니다. 테이블/스키마 설계 및 메타 파이프라인 구현은 기획서 검토가 끝난 뒤 순차적으로 진행합니다.

1. **기획 수정** → 시스템 문서 업데이트 → 관련 `Open Questions`/`Changelog` 갱신.  
2. **데이터 수정** → 스프레드시트 업데이트 → `tools/build-meta` 실행 → 생성된 JSON/manifest를 커밋.  
3. **기술 수정** → API/스키마 문서 업데이트 → 메타 버전이 바뀌면 manifest 버전을 증가시키고 서버 배포 절차에 반영.

## 3. 새롭게 추가된 문서

- `docs/01-systems/economy.md` – 상점, 교환, 제작을 하나의 서브시스템으로 정의.  
- `docs/02-data/data-glossary.md` – 메타데이터 키와 스프레드시트 위치, 타입, 버전 관리.  
- `docs/04-technical/metadata-pipeline.md` – 구글 시트 → JSON → 서버 배포 자동화 및 버전 관리 정책.

## 4. 현재 우선순위

1. `docs/02-data/` 내 미작성 테이블(`equipment_pool`, `monsters`, `regions`)을 스프레드시트 기반으로 채우고 파이프라인 규칙에 맞춰 JSON을 생성합니다.  
2. 각 시스템 문서의 `Open Questions` 항목을 확인하고 수치/필드 요구사항을 데이터·기술 문서로 이전합니다.  
3. 메타 버전 manifest와 서버 알림 흐름을 `metadata-pipeline.md` 및 `client-server-contract.md`와 연동합니다.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
