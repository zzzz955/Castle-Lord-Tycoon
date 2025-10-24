# 영웅 시스템 추가 개편 완료 보고서 (v2)

**작성일**: 2025-10-25  
**작성자**: Claude Code (SuperClaude)  
**상태**: ✅ 완료  

---

## 📋 개편 내용 요약

### 1. CSV 데이터 업데이트 (6개 파일)

#### hero_rebirth_rules.csv
- ❌ 골드 비용 제거 (`gold_cost` 컬럼 삭제)
- ✅ 환생 확률 100% (실패 없음)
- ✅ 동급 영웅만 제물로 사용

#### hero_release_rewards.csv
- ❌ 골드 지급 제거 (`gold_reward` 컬럼 제거)
- ✅ 파편 수량 대폭 증가: 10/30/120/500/2400/9600 (1~6성)
- ℹ️ MVP 단계에서 파편 사용처 없음 (추후 영웅 모집 기능 예정)

#### hero_pool_expansion_rules.csv
- ✅ 초기 슬롯: 12개 (기존 20개에서 변경)
- ✅ 최대 슬롯: 200개 (기존 60개에서 대폭 확장)
- ✅ 4단계 확장 비용 체계:
  - 13~30: 100골드/슬롯
  - 31~100: 1,000골드/슬롯
  - 101~150: 2,000골드/슬롯
  - 151~200: 5,000골드/슬롯

#### hero_growth.csv
- ✅ 최대 레벨 확장: 150 → 200
- ✅ 기존 패턴 유지하여 자연스러운 확장
- ✅ 레벨 180~200 구간 추가

#### hero_templates.csv
- ✅ MVP 영웅 10명으로 완전 재구성
- ✅ 1~6성 균형 분배:
  - 1성: 테오도라
  - 2성: 뮬란
  - 3성: 이사벨라, 잔다르크
  - 4성: 샤를마뉴, 살라딘
  - 5성: 예카테리나 1세, 오다 노부나가
  - 6성: 측천무후, 나폴레옹

#### hero_drop_tables.csv
- ✅ 동일 등급 동일 확률 (모든 weight = 1000)
- ✅ 술집 모집 티어별 영웅 분배:
  - TIER_BASIC: 1~4성
  - TIER_ADVANCED: 2~6성
  - TIER_SUPREME: 3~6성

---

### 2. 신규 고유 효과 추가

#### hero_unique_effects_templates.csv
14개 신규 효과 추가:
- UE_SWIFT_STRIKE (뮬란)
- UE_ROYAL_DECREE (이사벨라)
- UE_IMPERIAL_COMMAND (샤를마뉴)
- UE_FRANKISH_FURY (샤를마뉴)
- UE_DESERT_FORTRESS (살라딘)
- UE_CHIVALRY (살라딘)
- UE_DIPLOMATIC_MASTERY (예카테리나)
- UE_WINTER_RESILIENCE (예카테리나)
- UE_UNIFICATION_AMBITION (오다 노부나가)
- UE_TANEGASHIMA (오다 노부나가)
- UE_EMPRESS_AUTHORITY (측천무후)
- UE_POLITICAL_INTRIGUE (측천무후)
- UE_CULTURAL_PATRONAGE (측천무후)
- UE_GRAND_ARMEE (나폴레옹)

#### hero_effects_mapping.csv
- ✅ MVP 영웅 10명에 대한 효과 매핑 완료
- ✅ 1성은 효과 없음, 2~3성은 1개, 4~6성은 2~3개

---

### 3. 기획서 업데이트 (2개 문서)

#### hero.md
- ✅ 최대 레벨 200 명시
- ✅ 환생 시스템 세부사항 추가:
  - 골드 비용 없음
  - 100% 성공률
  - 동급 영웅 제물
  - 5단계 성장치 이관: 10%/20%/30%/40%/50%
- ✅ 방출 시스템 업데이트:
  - 골드 지급 없음
  - MVP 단계에서 파편 사용처 없음

#### tavern.md
- ✅ 영웅 보유 슬롯 세부사항 추가:
  - 초기 12개
  - 최대 200개
  - 4단계 확장 비용 체계

---

## 📊 MVP 영웅 상세 정보

| 영웅 | 성급 | 타입 | 속성 | 고유 효과 수 | 역사적 배경 |
|------|------|------|------|------------|----------|
| 테오도라 | ⭐ 1성 | 균등형 | none | 0 | 동로마 제국 황후 |
| 뮬란 | ⭐⭐ 2성 | 공격형 | fire | 1 | 중국 전설의 여전사 |
| 이사벨라 | ⭐⭐⭐ 3성 | 균등형 | water | 1 | 스페인 여왕, 레콩키스타 |
| 잔다르크 | ⭐⭐⭐ 3성 | 방어형 | fire | 1 | 프랑스 구국의 성녀 |
| 샤를마뉴 | ⭐⭐⭐⭐ 4성 | 공격형 | earth | 2 | 프랑크 왕국 황제 |
| 살라딘 | ⭐⭐⭐⭐ 4성 | 방어형 | none | 2 | 이슬람 영웅, 십자군 전쟁 |
| 예카테리나 | ⭐⭐⭐⭐⭐ 5성 | 균등형 | water | 2 | 러시아 제국 황제 |
| 오다 노부나가 | ⭐⭐⭐⭐⭐ 5성 | 공격형 | fire | 2 | 일본 전국시대 통일자 |
| 측천무후 | ⭐⭐⭐⭐⭐⭐ 6성 | 균등형 | none | 3 | 중국 유일의 여제 |
| 나폴레옹 | ⭐⭐⭐⭐⭐⭐ 6성 | 공격형 | fire | 3 | 프랑스 황제, 전술의 천재 |

---

## 🎯 주요 변경사항 비교

| 항목 | 기존 | 신규 | 변경 사유 |
|------|------|------|----------|
| 환생 골드 비용 | 5K~100K | 0 | 접근성 향상 |
| 환생 성공률 | 미정 | 100% | 확실성 보장 |
| 방출 골드 | 100~3K | 0 | 파편 중심 경제 |
| 방출 파편 | 5~35 | 10~9600 | 가치 상향 |
| 초기 슬롯 | 20 | 12 | 초기 난이도 조정 |
| 최대 슬롯 | 60 | 200 | 확장성 대폭 증가 |
| 최대 레벨 | 150 | 200 | 성장 한계 확장 |
| 영웅 수 | 12 (테스트) | 10 (MVP) | 실제 리소스 기반 |

---

## 📁 수정된 파일 목록

### CSV 데이터 (6개)
```
docs/02-data/
├── hero_rebirth_rules.csv          (골드 제거)
├── hero_release_rewards.csv        (파편 수량, 골드 제거)
├── hero_pool_expansion_rules.csv   (12→200, 4단계 비용)
├── hero_growth.csv                 (레벨 200)
├── hero_templates.csv              (MVP 10명)
├── hero_effects_mapping.csv        (신규 매핑)
├── hero_drop_tables.csv            (동일 weight)
└── hero_unique_effects_templates.csv (14개 효과 추가)
```

### 기획서 (2개)
```
docs/01-systems/
├── hero.md    (최대 레벨, 환생, 방출)
└── tavern.md  (슬롯 확장 비용)
```

---

## ✅ 검증 체크리스트

- [x] hero_rebirth_rules.csv: gold_cost 컬럼 제거 확인
- [x] hero_release_rewards.csv: 파편 수량 10/30/120/500/2400/9600
- [x] hero_pool_expansion_rules.csv: 12~200 슬롯, 4단계 비용
- [x] hero_growth.csv: 레벨 200까지 확장
- [x] hero_templates.csv: MVP 10명 등록
- [x] hero_effects_mapping.csv: 10명 매핑
- [x] hero_drop_tables.csv: 모든 weight = 1000
- [x] hero_unique_effects_templates.csv: 14개 효과 추가
- [x] hero.md: 최대 레벨 200, 환생/방출 세부사항
- [x] tavern.md: 슬롯 확장 비용 명시

---

## 🔄 다음 단계 작업

### 코드 통합
1. CSV 파서 업데이트
   - `hero_pool_expansion_rules.csv` 새로운 구조 반영
   - `hero_growth.csv` 레벨 200 지원
2. UI 업데이트
   - 슬롯 확장 UI: 4단계 비용 표시
   - 환생 UI: 골드 비용 제거
   - 방출 UI: 파편만 표시

### 밸런스 조정
1. 파편 경제 시스템 설계
2. 슬롯 확장 비용 테스트
3. 레벨 200 경험치 곡선 밸런싱

### 테스트
1. MVP 영웅 10명 드랍 테스트
2. 술집 모집 확률 검증
3. 환생 시스템 통합 테스트

---

**최종 수정**: 2025-10-25  
**검수 상태**: ✅ 완료  
**다음 단계**: 코드 통합 및 테스트
