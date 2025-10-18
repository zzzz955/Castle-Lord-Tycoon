# Castle Lord Tycoon - 게임 기획 문서

## 📋 문서 구조

### 00-overview/ - 프로젝트 개요
- `game-concept.md` - 게임 핵심 컨셉 및 비전

### 01-systems/ - 핵심 시스템
- `world-exploration.md` - 필드, 전장의 안개, 야영
- `territory.md` - 깃발, 영토 편입
- `settlement.md` - 마을, 요새, 자동 점령
- `combat.md` - 전투 시스템 (3×3 그리드, 자동 전투)
- `character.md` - 영웅, 성장, 파티 편성
- `equipment.md` - 장비, 등급, 옵션
- `encounter.md` - 조우 시스템, 지역 난이도
- `progression.md` - 계급(작위), 혜택

### 02-data/ - 게임 데이터
- `monsters.md` - 몬스터 데이터베이스
- `equipment-pool.md` - 장비 풀
- `regions.md` - 지역 데이터
- `balance-formulas.md` - 밸런스 계산식

### 03-ui/ - UI/UX 설계
- `ui-design.md` - UI 설계 및 화면 구성
- `onboarding.md` - 온보딩 및 튜토리얼

### 04-technical/ - 기술 사양
- `architecture.md` - 시스템 아키텍처
- `data-structures.md` - 데이터 구조 정의

## 🎯 문서 작성 원칙

### AI 친화적 문서화
- **구조화된 데이터**: YAML/테이블 형식 사용
- **명확한 계층**: 섹션 구분 명확히
- **최소한의 설명**: 토큰 효율적인 서술
- **타입 명시**: 구현 시 참조 가능하도록

### 문서 상태 태그
- 🟢 **확정** - 구현 가능한 최종 사양
- 🟡 **검토중** - 검토 및 조정 필요
- 🔴 **초안** - 아이디어 단계
- ⚪ **미작성** - 향후 작성 예정

## 📖 문서 읽는 순서

### 신규 참여자
1. `00-overview/game-concept.md` - 게임 전체 이해
2. `01-systems/combat.md` - 핵심 전투 메커닉
3. `01-systems/character.md` - 영웅 시스템
4. `03-ui/onboarding.md` - 플레이어 경험

### 시스템 구현 시
1. 해당 시스템 문서 읽기
2. `04-technical/data-structures.md` 참조
3. `02-data/` 관련 데이터 확인
4. 시스템 연동 섹션 확인

### 밸런싱 작업 시
1. `02-data/balance-formulas.md` 참조
2. 각 시스템의 밸런스 섹션 확인
3. 수치 조정 및 문서 업데이트

## 🔧 문서 관리

### 파일 명명 규칙
- 소문자 + 하이픈: `file-name.md`
- 한글 사용: 내용은 한글로 작성
- 코드/데이터: 영문 사용

### 문서 업데이트
각 문서 하단에 업데이트 이력 기록:
```markdown
---
**최종 수정**: 2025-01-15
**상태**: 🔴 초안
**작성자**: SangHyeok
```

## 🚀 빠른 참조

### 핵심 게임 루프
탐험 → 조우/전투 → 전리품/성장 → 깃발 설치 → 영토 확장 → 마을 점령 → 더 어려운 지역

### 주요 시스템 연관도
```
World Exploration (필드 탐험)
    ↓
Encounter (조우)
    ↓
Combat (전투) ← Character (영웅) + Equipment (장비)
    ↓
Progression (성장) → Territory (영토 확장)
    ↓
Settlement (마을/요새 점령)
    ↓
Rank System (계급 상승)
```

## 📝 작성 팁

### AI에게 질문할 때
- 관련 시스템 문서 경로 제시
- 구체적인 섹션 참조
- 데이터 구조 변경 시 연관 시스템 확인 요청

### 새 기능 추가 시
1. 관련 시스템 문서 확인
2. 시스템 연동 섹션 업데이트
3. 데이터 구조 추가/변경
4. 밸런스 영향도 검토
