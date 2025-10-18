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

```typescript
interface RankRequirement {
  area: number;          // 영토 타일 수
  towns?: number;        // 점령 마을 수
  fortresses?: number;   // 해금 요새 수
}

function checkRankUp(player: Player): Rank | null {
  for (const rank of RANKS_DESC) {  // 높은 순서부터
    if (player.territory.area >= rank.requirement.area &&
        player.towns.owned.length >= (rank.requirement.towns || 0) &&
        player.fortresses.unlocked.length >= (rank.requirement.fortresses || 0)) {
      return rank;
    }
  }
  return null;
}
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

```typescript
interface Rank {
  id: string;
  name: string;
  order: number;  // 순서 (낮을수록 높은 계급)

  requirement: RankRequirement;

  benefits: {
    expBonus?: number;       // %
    dropRateBonus?: number;  // %
    campingEfficiency?: number;
    movementSpeed?: number;
    upgradeCostReduction?: number;
  };
}

interface PlayerProgression {
  currentRank: Rank;
  territory: {
    area: number;
    towns: number;
    fortresses: number;
  };

  checkPromotion(): Rank | null;
  promote(newRank: Rank): void;
  getActiveBenefits(): Benefits;
}
```

## 혜택 적용

### 경험치 보너스

```typescript
function calculateExp(baseExp: number, rank: Rank): number {
  const bonus = rank.benefits.expBonus || 0;
  return Math.floor(baseExp * (1 + bonus / 100));
}
```

### 드랍률 보너스

```typescript
function calculateDropRate(baseRate: number, rank: Rank): number {
  const bonus = rank.benefits.dropRateBonus || 0;
  return Math.min(baseRate + bonus, 100);  // 100% 상한
}
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
