# 진행 시스템 (계급/작위)

## 개요

```yaml
type: "영토 기반 계급 시스템"
progression: "영토 면적 + 마을/요새 수"
rewards: "가속형 혜택"
```

**핵심 가치**: 영토 확장 → 계급 상승 → 성장 가속 = 장기 동기 부여

## 계급 시스템

### 계급 목록

```yaml
ranks:
  - id: "castellan"
    name: "성주"
    requirement: { area: 0, towns: 1, fortresses: 0 }

  - id: "baron"
    name: "남작"
    requirement: { area: 150, towns: 2, fortresses: 0 }

  - id: "viscount"
    name: "자작"
    requirement: { area: 300, towns: 3, fortresses: 0 }

  - id: "earl"
    name: "백작"
    requirement: { area: 1000, towns: 4, fortresses: 0 }

  - id: "marquis"
    name: "후작"
    requirement: { area: 1500, towns: 5, fortresses: 5 }

  - id: "duke"
    name: "공작"
    requirement: { area: 2500, towns: 6, fortresses: 6 }

  - id: "viceroy"
    name: "대신"
    requirement: { area: 3500, towns: 7, fortresses: 7 }

  - id: "justiciar"
    name: "대공"
    requirement: { area: 5000, towns: 8, fortresses: 8 }

  - id: "king"
    name: "국왕"
    requirement: { area: 7000, towns: 10, fortresses: 15 }

  - id: "emperor"
    name: "황제"
    requirement: { area: 10000, towns: 15, fortresses: 20 }
```

## 승급 조건

### 조건 판정

```yaml
requirement_check:
  fields:
    - area: "영토 타일 수"
    - towns: "점령 마을 수"
    - fortresses: "해금 요새 수"

  logic:
    - 높은 계급부터 순차 확인
    - 모든 조건 충족 시 승급
    - 즉시 적용
```

### 승급 시점

```yaml
check_timing:
  - 마을 자동 점령 시
  - 요새 해금 및 점령 시
  - 깃발 설치 시

auto_promotion:
  instant: true
  notification: "계급 승급 축하 메시지"
  rewards: "즉시 적용"
```

## 계급 혜택

### 혜택 카테고리

```yaml
combat_rewards:
  - "전투 경험치 +N%"
  - "드랍률 +N%"

efficiency:
  - "야영 회복 효율 증가"
  - "이동 속도 증가"

economy:
  - "요새 교환소 할인"
  - "장비 강화 비용 절감"

rewards:
  - "보스/요새 보상 가중"
  - "희귀 드랍 가중"
  - "보장 토큰"
```

### 계급별 혜택 예시

```yaml
castellan:
  exp_bonus: 0%
  drop_rate: 0%

baron:
  exp_bonus: 5%
  drop_rate: 2%

viscount:
  exp_bonus: 10%
  drop_rate: 5%
  camping_efficiency: 10%

earl:
  exp_bonus: 20%
  drop_rate: 10%
  camping_efficiency: 10%
  movementSpeed: 10%

marquis:
  exp_bonus: 30%
  drop_rate: 15%
  camping_efficiency: 20%
  movementSpeed: 10%
  upgradeCostReduction: 10%

duke:
  exp_bonus: 40%
  drop_rate: 20%
  camping_efficiency: 20%
  movementSpeed: 10%
  upgradeCostReduction: 10%

viceroy:
  exp_bonus: 50%
  drop_rate: 25%
  camping_efficiency: 30%
  movementSpeed: 20%
  upgradeCostReduction: 10%

justiciar:
  exp_bonus: 65%
  drop_rate: 32.5%
  camping_efficiency: 30%
  movementSpeed: 20%
  upgradeCostReduction: 10%

king:
  exp_bonus: 80%
  drop_rate: 40%
  camping_efficiency: 40%
  movementSpeed: 20%
  upgradeCostReduction: 20%

emperor:
  exp_bonus: 100%
  drop_rate: 50%
  camping_efficiency: 50%
  movementSpeed: 30%
  upgradeCostReduction: 30%
```

## 데이터 구조

> 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **Rank**: 계급 정보 (이름, 승급 조건, 혜택)
- **RankRequirement**: 승급 조건 (영토 면적, 마을 수, 요새 수)
- **Benefits**: 계급 혜택 (경험치, 드랍률, 효율 보너스)
- **PlayerProgression**: 플레이어 진행 상황 (현재 계급, 영토 통계)

## 혜택 적용

### 경험치 보너스

```yaml
calculation:
  formula: "기본 경험치 × (1 + 보너스%)"
  example: "100 exp × 1.5 (50% 보너스) = 150 exp"
```

### 드랍률 보너스

```yaml
calculation:
  formula: "기본 확률 + 보너스%"
  cap: "100% 상한"
  example: "10% + 40% 보너스 = 50%"
```

## UI 요구사항

### 계급 정보 화면

```yaml
rank_panel:
  current_rank:
    - 계급 아이콘
    - 계급 이름
    - 현재 혜택 목록

  next_rank:
    - 다음 계급 이름
    - 필요 조건 (진행도 바)
    - 추가 혜택 미리보기

  statistics:
    - 영토 면적: "300 / 600"
    - 점령 마을: "5 / 8"
    - 해금 요새: "2 / 3"
```

### 승급 알림

```yaml
promotion_notification:
  display:
    - "계급 승급!" 대형 팝업
    - 이전 계급 → 새 계급
    - 새로운 혜택 목록
    - 축하 효과

  timing:
    - 조건 충족 즉시
    - 전투 중이면 전투 후
```

## 시스템 연동

### 의존 시스템
- `territory.md` - 영토 면적
- `settlement.md` - 마을/요새 수

### 제공 기능
- 경험치 배율
- 드랍률 증가
- 각종 효율 보너스

## 경제 밸런싱

### 골드 획득 vs 소비

```yaml
gold_sources:
  monster_drops:
    formula: "(monster_level × 5) × (1 + growth_rate/100)"
    examples:
      lv5_slime_growth20: "30 gold"
      lv10_wolf_growth30: "65 gold"
      lv15_boss_growth60: "120 gold"

  rank_bonus:
    apply: "gold_bonus from rank benefits"
    examples:
      baron: "+2%"
      earl: "+10%"
      emperor: "+50%"

  hero_bonus:
    apply: "party gold_bonus sum"
    example: "hero1 +5% + hero2 +12% = +17%"

gold_sinks:
  flag_placement:
    3x3: 100
    5x5: 500
    7x7: 2000
    note: "영토 확장의 주요 골드 소비처"

  hero_recruitment:
    gacha_cost: "300~1000 gold/pull"
    rate_up: "이벤트 시 할인"
    note: "영웅 수집의 주요 골드 소비처"

  equipment_purchase:
    town_shop: "level × grade_multiplier"
    fortress_shop: "premium items"

balancing_target:
  early_game:
    lv1_10: "깃발 3x3 위주, 영웅 1~2회 모집"
    income: "전투당 평균 50 gold"
    expense: "깃발 2~3개 (300 gold), 가챠 1회 (500 gold)"

  mid_game:
    lv11_50: "깃발 5x5 주력, 영웅 수집 가속"
    income: "전투당 평균 200 gold"
    expense: "깃발 5~10개 (2500~5000 gold), 가챠 5~10회"

  late_game:
    lv51_200: "깃발 7x7 필수, 고성능 영웅 수집"
    income: "전투당 평균 800 gold"
    expense: "깃발 10~20개 (20000~40000 gold), 가챠 20회+"
```

### 경험치 밸런싱

```yaml
exp_sources:
  monster_drops:
    formula: "(monster_level × 10) × (1 + growth_rate/100)"
    분배: "생존자 균등 분배"
    examples:
      lv5_slime_growth20:
        total: "60 exp"
        party_4_all_alive: "15 exp/hero"
        party_4_two_alive: "30 exp/hero"

  rank_bonus:
    apply: "exp_bonus from rank benefits"
    examples:
      castellan: "+0%"
      earl: "+20%"
      emperor: "+100%"

  hero_bonus:
    apply: "party exp_bonus sum"
    example: "hero1 +10% + hero2 +15% = +25%"

leveling_curve_validation:
  target: "캐주얼 플레이 기준 레벨 100 도달 시간"

  assumptions:
    daily_playtime: "1~2시간"
    battles_per_hour: "10~15 전투"
    average_exp_per_battle: "파티 생존자 기준 50~200 exp"

  early_game_lv1_20:
    exp_required: "누적 약 10000 exp"
    battles_needed: "100~200 전투"
    estimated_time: "7~10 시간"

  mid_game_lv21_50:
    exp_required: "누적 약 100000 exp"
    battles_needed: "500~1000 전투"
    estimated_time: "50~100 시간"

  late_game_lv51_100:
    exp_required: "누적 약 1000000 exp"
    battles_needed: "3000~5000 전투"
    estimated_time: "300~500 시간"

  balancing_note:
    - "계급 보너스로 후반 가속"
    - "파티 보너스 영웅으로 효율 증가"
    - "요새 클리어 보상으로 점프 가능"
```

## 밸런스 파라미터

```yaml
progression_curve:
  early_game: "빠른 승급 (동기 부여)"
  mid_game: "안정적 성장"
  late_game: "천천히 (장기 목표)"

benefit_scaling:
  linear: ["exp_bonus", "drop_rate"]
  exponential: ["rare_drop_weight"]
  threshold: ["special_tokens"]

economy_balance:
  gold_income_vs_expense: "1:1.2 비율 (약간의 부족)"
  gacha_frequency: "하루 1~2회 (적극적 플레이 시)"
  flag_expansion: "계급당 5~10개"
```

## 확장 가능성

```yaml
future_features:
  - 계급 전용 스킬
  - 계급 전용 장비
  - 계급 퀘스트
  - 명성 시스템
  - 타이틀 커스터마이징
```

## 구현 체크리스트

- [ ] 계급 데이터 정의
- [ ] 승급 조건 판정
- [ ] 혜택 시스템
- [ ] 경험치/드랍 보너스 적용
- [ ] 계급 UI
- [ ] 승급 알림
- [ ] 진행도 추적

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
