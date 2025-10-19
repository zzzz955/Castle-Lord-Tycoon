# 전투 시스템

## 개요

```yaml
type: "완전 자동 전투"
grid: "3×3"
max_party: 4
element_system: true
element_modifier: "±30%"
round_limit: 10
```

**핵심 가치**: 배치와 상성으로 전략을 만드는 단순하지만 깊이 있는 전투

## 그리드 시스템

### 3×3 배치
```
[7] [4] [1]   우측 상단(1) → 우측 중단(2) → 우측 하단(3)
[8] [5] [2]   중앙 상단(4) → 중앙 중단(5) → 중앙 하단(6)
[9] [6] [3]   좌측 상단(7) → 좌측 중단(8) → 좌측 하단(9)
```

**행동 우선순위**: 번호 순서대로 (1→2→3→4→5→6→7→8→9)

```yaml
placement_rules:
  - 최대 4명까지 배치 가능
  - 빈 칸 허용
  - 배치 순서가 전략의 핵심
  - 적도 동일한 규칙 적용, 단 UI상 플레이어와 좌우 반전되어 보임
```

### 전략적 배치

```yaml
front_positions: [1, 2, 3]  # 적에게 가까움, 먼저 공격받음
mid_positions: [4, 5, 6]    # 균형잡힌 위치
back_positions: [7, 8, 9]   # 안전하지만 늦게 행동
```

## 전투 흐름

### 1. 조우 발생
```
필드 이동 → 타일 태그 체크 → 확률 판정 → 전투 돌입
```

### 2. 전투 시작
```yaml
combat_initialization:
  - 파티 배치 확정
  - 적 배치 생성
  - 라운드 카운터 초기화 (0/10)
```

### 3. 라운드 진행

```yaml
전투 스냅샷 생성:
  - 서버가 현재 영웅 기본 스탯 조회
  - 장비 보너스 적용하여 최종 스탯 계산
    * Base Stats (HP, Attack, Defense)
    * Extended Stats (crit_rate, crit_damage, evasion, etc.)
    * Flat Bonuses: stat + equipment_flat_bonus
    * Percent Bonuses: result × (1 + equipment_percent_bonus)
  - 전투 전용 스냅샷 생성 (불변)
  - 아군 버프/적군 디버프 적용 (Aura 고유 효과)

각 라운드:
  1. 플레이어 선공 후 1턴씩 공격을 주고 받음
  2. 우선순위 순서대로 행동 (1→9)
  3. 각 유닛:
     - 생존 여부 체크 (전투 중 사망한 기물의 턴은 스킵, 스킵된 경우 상대방의 턴으로 넘어감)
     - 타겟 선택 (우선 공격 등 효과가 없을 경우 자신과 같은 행의 맨 앞 기물을 타겟으로 공격, 같은 행에 타겟이 없을 경우 위쪽 행 우선 타격, 위쪽 행에도 기물이 없을 경우 아래쪽 행 타격)
     - 데미지 계산 (속성 상성 + 고유 효과 적용)
     - 데미지 적용
     - 고유 효과 발동 (흡혈, 피해 감소, 반사 피해, 보호막 등)
  4. 양측 전멸 체크
  5. 라운드 증가

스탯 검증:
  - 서버 계산 스탯으로 전투 시뮬레이션 (Server Authoritative)
  - 클라이언트 스탯은 UI 표시용
  - 전투 시작 시 서버-클라이언트 스탯 불일치 검증
  - 불일치 시 서버 값 사용 및 클라이언트 동기화
```

### 4. 전투 종료
```yaml
win_condition: "적 전멸"
lose_condition: "아군 전멸 또는 라운드 10 초과"
draw_condition: "라운드 10에서 양측 생존"

round_11_penalty:
  - 11라운드부터 양측 데미지 증가
  - 매 라운드 10%씩 가속
  - 강제 종료 유도
```

## 속성 시스템

### 4속성 상성

```yaml
elements:
  water: "물"
  fire: "불"
  earth: "땅"
  none: "무속성"

advantage_table:
  water > fire: "+30% damage"
  fire > earth: "+30% damage"
  earth > water: "+30% damage"
  none: "±0% (중립)"

disadvantage_table:
  water < earth: "-30% damage"
  fire < water: "-30% damage"
  earth < fire: "-30% damage"
```

### 데미지 계산

```yaml
formula:
  step_1_armor_penetration: "방어력 × (1 - 방어도 무시)"
  step_2_base_damage: "공격력 - 감소된 방어력 (최소 1)"
  step_3_element_bonus: "±30% (속성 상성)"
  step_4_crit_check: "크리티컬 판정"
  step_5_evasion_check: "회피 판정 (회피 무시 고려)"
  step_6_unique_effects: "고유 효과 적용"
  step_7_final_damage: "최종 피해"

detailed_calculation:
  1. 방어력 감소: defense × (1 - armor_penetration)
  2. 기본 피해: max(1, attack - reduced_defense)
  3. 속성 보너스: base_damage × (1 + element_modifier)
  4. 크리티컬:
     - 확률: crit_rate
     - 피해: damage × (1 + crit_damage)
  5. 회피:
     - 확률: evasion × (1 - evasion_pierce)
     - 성공 시: damage = 0 (Miss)
  6. 고유 효과:
     - 추가 피해, 피해 증가 등
  7. 최종 피해:
     - 받는 피해 감소: damage × (1 - damage_reduction)
     - 블록: damage × 0.5 (확률 판정)

example:
  attacker: "공격력 100, 불 속성, 방어도 무시 20%"
  defender: "방어력 30, 땅 속성, 받는 피해 감소 10%"
  calculation:
    step1: "30 × (1 - 0.2) = 24 (감소된 방어력)"
    step2: "100 - 24 = 76"
    step3: "76 × 1.3 = 98.8 (속성 보너스)"
    step4: "크리티컬 미발동"
    step5: "회피 실패"
    step6: "고유 효과 없음"
    step7: "98.8 × (1 - 0.1) = 88.92 (최종)"
```

## 보상 시스템

### 경험치 분배

```yaml
exp_distribution:
  rule: "생존자만 균등 분배"
  calculation: "총 경험치 / 생존자 수"
  dead_heroes: "0 경험치"

example:
  total_exp: 360
  survivors: 3
  dead: 1
  result: "각 생존자 120 경험치, 사망자 0"
```

### 드랍 보상

```yaml
drop_system:
  rule: "생존 여부 무관"
  factors:
    - 몬스터 드랍 테이블
    - 파티 드랍률 합산 (drop_rate_bonus)
    - 계급 보너스

party_bonus_calculation:
  exp: "파티 전체 exp_bonus 합산"
  gold: "파티 전체 gold_bonus 합산"
  drop_rate: "파티 전체 drop_rate_bonus 합산"

  example:
    hero1: "exp +10%, gold +5%"
    hero2: "exp +15%, drop +8%"
    hero3: "gold +12%"
    total: "exp +25%, gold +17%, drop +8%"

  application:
    base_exp: 100
    final_exp: "100 × (1 + 0.25) = 125"
```

## 전투 결과 누적

### 필드 누적 규칙

```yaml
accumulated:
  - HP 손실 (누적)
  - 영웅 사망 상태 (누적)

reset_on_combat_end:
  - 디버프/버프
  - 스킬 쿨다운
  - 전투 임시 상태

recovery_methods:
  camping: "야영으로 HP 회복"
  town: "마을 귀환 시 완전 회복 (HP + 부활)"
```

## UI 요구사항

### 전투 화면

```yaml
must_display:
  - 3×3 그리드 (아군/적군)
  - 각 영웅 HP 바
  - 영웅 초상화 및 속성 아이콘
  - 영웅 기물(공격형, 방어형, 유틸형)
  - 공격 애니메이션(공격형-원거리 & 활, 방어형-근거리 & 검, 방패, 유틸형-원거리 & 지팡이)
  - 라운드 카운터

actions:
  - 스킵 (결과만 보기)
  - 리플레이 (전투 재생)
  - 속도 조절 (1x, 2x, 3x)
```

### 전투 결과

```yaml
result_screen:
  summary:
    - 승리/패배/무승부
    - 생존자 목록
    - 사망자 목록

  rewards:
    - 획득 경험치 (영웅별)
    - 드랍 아이템 목록
    - 골드 획득

  stats:
    - 총 데미지
    - 라운드 수
    - MVP (최다 데미지)
```

## 시스템 연동

### 의존 시스템
- `character.md` - 영웅 스탯, 속성
- `equipment.md` - 장비 효과 적용
- `encounter.md` - 전투 진입 조건

### 제공 기능
- 전투 결과 (승/패)
- 경험치 보상
- 드랍 아이템
- 영웅 상태 변화 (HP, 사망)

## 데이터 구조

> 전투 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **Combat**: 전투 세션 정보 (참가자, 그리드 배치, 라운드)
- **CombatAction**: 각 행동 로그 (공격자, 대상, 데미지)
- **CombatResult**: 전투 결과 (승패, 생존자, 보상)

## 밸런스 파라미터

```yaml
damage_formula:
  base: "attack - defense"
  min_damage: 1
  element_bonus: 0.3  # ±30%

round_limit:
  normal: 10
  penalty_start: 11
  penalty_increase: 0.1  # 10% per round

exp_scaling:
  base_exp: "monster_level * 10"
  party_penalty: false  # 분배만, 총량 감소 없음
```

## 확장 가능성

```yaml
future_features:
  - 스킬 시스템 (패시브만)
  - 상태 효과 (화상, 빙결 등)
  - 포지션 특화 (전열, 후열 보너스)
  - 콤보 시스템
```

## 구현 체크리스트

- [ ] 3×3 그리드 렌더링
- [ ] 전투 로직 구현
- [ ] 속성 상성 계산
- [ ] 경험치 분배
- [ ] 드랍 시스템
- [ ] 전투 로그 기록
- [ ] 리플레이 기능
- [ ] 스킵 기능

---
**최종 수정**: 2025-10-18
**상태**: 🔴 초안
**작성자**: SangHyeok
