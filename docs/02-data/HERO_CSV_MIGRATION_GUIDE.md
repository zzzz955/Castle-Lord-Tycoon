# 영웅 CSV 데이터 개편 완료 보고서

**작성일**: 2025-10-25  
**작성자**: Claude Code (SuperClaude)  
**상태**: ✅ 완료  

---

## 📋 개편 목적

기획서(`docs/01-systems/hero.md`, `docs/01-systems/hero-unique-effects.md`)와 CSV 데이터 간 불일치 해소 및 1-6성 영웅 등급 체계 명확화

---

## ⚠️ 주요 변경사항 요약

### 1. **hero_templates.csv** - 완전 재구성

| 변경 항목 | 기존 | 신규 | 이유 |
|---------|------|------|------|
| 성급 표현 | `base_star` + `rarity` (R/UC/H/L) | `star_rating` (1-6) | ⭐ 가시성 향상 |
| 속성 체계 | light/wind/arcane/void | water/fire/earth/none | 🌊 기획서 일치 |
| 성장 타입 | aggressive/defensive/balanced (영문) | 공격형/방어형/균등형 (한글) | 🎯 일관성 |
| 기본 능력치 | 없음 | attack/defense/hp (6000/4000/5000) | 📊 기획서 명세 |
| 크리티컬 | 없음 | crit_rate_bp, crit_damage_bp | 🎲 전투 시스템 |
| 영웅 분류 | 전체 가상 인물 | 1-3성: 가상, 4-6성: 역사 인물 | 🏛️ 기획서 요구사항 |

**신규 영웅 추가**:
- ⭐ 1성: 로데릭, 엘시
- ⭐⭐ 2성: 가렌, 마렌
- ⭐⭐⭐ 3성: 알리시아, 브론
- ⭐⭐⭐⭐ 4성: 나폴레옹, 잔 다르크
- ⭐⭐⭐⭐⭐ 5성: 카이사르, 레오니다스
- ⭐⭐⭐⭐⭐⭐ 6성: 알렉산드로스 대왕, 세종대왕

---

### 2. **hero_unique_effects_*.csv** - 모듈화 시스템 구축

기획서에 명시된 모듈형 고유 효과 시스템 구현:

| 파일명 | 역할 | 레코드 수 |
|--------|------|-----------|
| `hero_unique_effects_triggers.csv` | 발동 트리거 정의 | 13개 |
| `hero_unique_effects_conditions.csv` | 발동 조건 정의 | 6개 |
| `hero_unique_effects_selectors.csv` | 대상 선택 규칙 | 10개 |
| `hero_unique_effects_actions.csv` | 액션 효과 정의 | 21개 |
| `hero_unique_effects_templates.csv` | 효과 템플릿 조합 | 19개 |
| `hero_effects_mapping.csv` | 영웅별 효과 매핑 | 18개 |

**이전 구조**: 단일 CSV 파일 (hero_unique_effects.csv) → **신규**: 6개 모듈형 CSV

**장점**:
- ✅ 트리거/조건/대상/액션 재사용 가능
- ✅ 신규 효과 추가 시 기존 데이터 수정 불필요
- ✅ 영웅별 파라미터 커스터마이징 가능
- ✅ 로컬라이제이션 지원 구조

---

### 3. **hero_rebirth_rules.csv** - 환생 시스템 개편

| 변경 항목 | 기존 | 신규 |
|---------|------|------|
| 재료 | 아이템 기반 (ITEM_HERO_EMBLEM_I) | 동일 영웅 재료 방식 |
| 성장치 이관 | 없음 | 10%/20%/30%/40%/50% |
| 환생 단계 | 4단계 (Lv30/60/90/120) | 5단계 (Lv30/60/90/120/150) |

**기획서 일치**: "동일한 영웅을 재료로 사용해 성장치를 이관"

---

### 4. **hero_growth.csv** - 환생 가능 레벨 명시

**추가 컬럼**:
- `cumulative_exp`: 누적 경험치
- `max_growth_cap`: 레벨별 최대 성장치 상한
- `rebirth_eligible`: 환생 가능 여부 (Lv 30/60/90/120/150)

---

### 5. **hero_release_rewards.csv** - 파편 시스템 통일

**변경사항**:
- `reward_item_id` → `fragment_item_id`: FRAG_HERO_STAR_X 형식 통일
- 성급별 파편 수량 조정 (5/8/12/18/25/35)
- ⭐ 시각적 표현 추가 (notes_ko)

---

### 6. **hero_drop_tables.csv** - 영웅 ID 갱신

기존 HERO_SELENE, HERO_IRENE, HERO_DARIUS → 역사 인물 기반 영웅으로 교체

---

## 📊 데이터 일관성 검증

### ✅ 기획서 준수 사항

| 항목 | 기획서 요구사항 | 구현 상태 |
|------|----------------|-----------|
| 성급 체계 | 1-6성 명시 | ✅ star_rating 컬럼 |
| 속성 시스템 | 물/불/땅/무 | ✅ water/fire/earth/none |
| 기본 능력치 | 공격 60, 방어 40, 체력 50 | ✅ 6000/4000/5000 (고정소수 *100) |
| 크리티컬 | 확률 10%, 데미지 100% | ✅ 1000bp, 10000bp |
| 역사 인물 | 4-6성 실존 인물 | ✅ historical_figure=true |
| 환생 시스템 | 동일 영웅 재료 | ✅ material_hero_min_level |
| 환생 단계 | 5회 (30/60/90/120/150) | ✅ 5단계 정의 |
| 고유 효과 | 모듈형 시스템 | ✅ 6개 테이블 분리 |

### 📈 고정소수 표현 (Fixed Point)

모든 수치는 `값 × 100` 정수 저장 방식:
- 공격력 60 → 6000
- 성장치 10% → 1000
- 크리티컬 확률 10% → 1000bp (basis point)
- 피해 150% → 15000

---

## 🗂️ 파일 구조 변경

### 신규 파일
```
docs/02-data/
├── hero_unique_effects_triggers.csv      (신규)
├── hero_unique_effects_conditions.csv    (신규)
├── hero_unique_effects_selectors.csv     (신규)
├── hero_unique_effects_actions.csv       (신규)
├── hero_unique_effects_templates.csv     (신규)
└── hero_effects_mapping.csv              (신규)
```

### 백업 파일
```
docs/02-data/
├── hero_templates.csv.backup
├── hero_growth.csv.backup
├── hero_rebirth_rules.csv.backup
├── hero_release_rewards.csv.backup
├── hero_drop_tables.csv.backup
└── hero_unique_effects.csv.deprecated    (구 단일 파일)
```

---

## 🔄 마이그레이션 체크리스트

### ✅ 완료된 작업
- [x] hero_templates.csv 재구성 (17개 컬럼 → 완전 개편)
- [x] hero_unique_effects 6개 테이블로 분리
- [x] hero_rebirth_rules.csv 환생 시스템 개편
- [x] hero_growth.csv 환생 레벨 추가
- [x] hero_release_rewards.csv 파편 시스템 통일
- [x] hero_drop_tables.csv 영웅 ID 갱신
- [x] 기획서 검증 및 일관성 확인
- [x] 백업 파일 생성
- [x] ⭐ 성급 시각화 추가

### ⚠️ 추가 작업 필요
- [ ] 코드베이스에서 구 컬럼명 참조 검색 및 수정
  - `base_star` → `star_rating`
  - `rarity` → 제거
  - `growth_type` → `growth_type_ko`
  - `role` → 제거 (전투 타입과 혼동)
- [ ] 고유 효과 시스템 파서 구현
- [ ] UI에서 ⭐ 시각화 렌더링
- [ ] 환생 시스템 로직 업데이트 (아이템 → 영웅 재료)

---

## 📝 개발자 참고사항

### 1. CSV 파싱 시 주의사항

**고정소수 변환**:
```typescript
// 예: attack_power = 6000 → 60.00
const actualValue = csvValue / 100;

// 예: growth_transfer_pct = 1000 → 10%
const percentage = csvValue / 100; // 10.00%
```

**속성 매핑**:
```typescript
const ATTRIBUTE_MAP = {
  water: "물",
  fire: "불",
  earth: "땅",
  none: "무"
};
```

### 2. 고유 효과 로드 순서

```
1. hero_unique_effects_triggers.csv
2. hero_unique_effects_conditions.csv
3. hero_unique_effects_selectors.csv
4. hero_unique_effects_actions.csv
5. hero_unique_effects_templates.csv (위 4개 참조)
6. hero_effects_mapping.csv (템플릿 + 영웅 매핑)
```

### 3. 환생 검증 로직

```typescript
// 환생 가능 여부 확인
const canRebirth = (heroLevel: number, rebirthStage: number) => {
  const requiredLevels = [30, 60, 90, 120, 150];
  return heroLevel >= requiredLevels[rebirthStage - 1];
};

// 성장치 이관 계산
const transferGrowth = (materialGrowth: number, stage: number) => {
  const transferPct = stage * 1000; // 1000 = 10%
  return Math.floor(materialGrowth * transferPct / 10000);
};
```

---

## 🎯 결론

✅ **기획서 기준 100% 일치**  
✅ **1-6성 명확한 시각화**  
✅ **모듈형 고유 효과 시스템 구축**  
✅ **확장 가능한 데이터 구조**  

---

**최종 수정**: 2025-10-25  
**검수 상태**: ✅ 완료  
**다음 단계**: 코드베이스 통합 및 테스트
