# 영웅 시스템

## 개요

```yaml
type: "별(★) 등급 시스템"
grades: [1, 2, 3, 4, 5, 6]
special_grade: "★6 (첫 별 빨간색)"
max_party_size: 4
```

**핵심 가치**: 같은 영웅도 별 등급에 따라 다른 성능 = 수집 동기 부여

## 별(★) 등급 시스템

### 등급 구분

```yaml
star_1:
  name: "1성"
  rarity: "매우 흔함"
  unique_effect: false
  growth_rate: "낮음"

star_2:
  name: "2성"
  rarity: "흔함"
  unique_effect: false
  growth_rate: "약간 낮음"

star_3:
  name: "3성"
  rarity: "보통"
  unique_effect: false
  growth_rate: "보통"

star_4:
  name: "4성"
  rarity: "희귀"
  unique_effect: true
  growth_rate: "높음"

star_5:
  name: "5성"
  rarity: "매우 희귀"
  unique_effect: true
  growth_rate: "매우 높음"

star_6:
  name: "6성 (빨간 별)"
  rarity: "전설"
  unique_effect: true
  growth_rate: "최고"
  visual: "첫 번째 별이 빨간색"
```

### 시각적 표현

```
★☆☆☆☆  - 1성
★★☆☆☆  - 2성
★★★☆☆  - 3성
★★★★☆  - 4성
★★★★★  - 5성
★★★★★  - 6성 (첫 별 빨간색)
```

## 영웅 스탯

### 스탯 시스템 구조

```yaml
architecture: "Hybrid + Metadata-Driven"
purpose: "마이그레이션 없이 스탯 추가 가능한 확장성 설계"

core_stats:
  description: "DB 컬럼으로 관리 (쿼리 최적화)"
  stats:
    hp: "생명력"
    attack: "공격력"
    defense: "방어력"

extended_stats:
  description: "JSONB로 관리 (동적 추가)"
  stats:
    critical_rate: "크리티컬 확률"
    critical_damage: "크리티컬 데미지"
    evasion: "회피율"
  future_examples:
    - "lifesteal (흡혈)"
    - "armor_penetration (관통력)"
    - "magic_resistance (마법 저항)"

metadata_system:
  description: "stat_definitions 테이블로 스탯 정의 관리"
  features:
    - "서버-클라이언트 자동 동기화"
    - "다국어 지원 (display_name)"
    - "스탯 타입 분류 (base/combat/special)"
    - "버전 관리"
```

### 기본 스탯 (Core Stats)

```yaml
hp:
  type: "base"
  data_type: "int"
  description: "생명력"
  storage: "DB 컬럼 (base_hp)"
  query_optimized: true

attack:
  type: "base"
  data_type: "int"
  description: "공격력"
  storage: "DB 컬럼 (base_attack)"
  query_optimized: true

defense:
  type: "base"
  data_type: "int"
  description: "방어력"
  storage: "DB 컬럼 (base_defense)"
  query_optimized: true

element:
  type: "base"
  values: ["물", "불", "땅", "무"]
  description: "속성"
  storage: "DB 컬럼 (element)"
```

### 확장 스탯 (Extended Stats)

```yaml
critical_rate:
  type: "combat"
  data_type: "float"
  description: "크리티컬 확률"
  default_value: 0.10
  max_value: 1.0
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "치명타 발생 시 피해량 * 크리티컬 데미지를 추가로 입힘"

critical_damage:
  type: "combat"
  data_type: "float"
  description: "크리티컬 데미지 배율"
  default_value: 1.0
  max_value: null
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "기본 공격력 * 해당 수치"

evasion:
  type: "combat"
  data_type: "float"
  description: "회피 확률"
  default_value: 0.05
  max_value: 1.0
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "적의 공격을 회피하여 데미지를 입지 않음, Miss 처리"

armor_penetration:
  type: "special"
  data_type: "float"
  description: "방어도 무시"
  default_value: 0.0
  max_value: 1.0
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "데미지 연산 전에 공격 대상의 방어력을 N%만큼 무시"

evasion_pierce:
  type: "special"
  data_type: "float"
  description: "회피 무시"
  default_value: 0.0
  max_value: 1.0
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "내 공격이 회피 판정을 받았을 때, N% 확률로 회피 판정을 무시"

exp_bonus:
  type: "utility"
  data_type: "float"
  description: "경험치 획득률"
  default_value: 0.0
  max_value: null
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "파티 전체 합산, 전투 승리 시 적용"

gold_bonus:
  type: "utility"
  data_type: "float"
  description: "골드 획득률"
  default_value: 0.0
  max_value: null
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "파티 전체 합산, 전투 승리 시 적용"

drop_rate_bonus:
  type: "utility"
  data_type: "float"
  description: "아이템 드랍률"
  default_value: 0.0
  max_value: null
  storage: "JSONB (extended_stats)"
  growth_affected: false
  note: "파티 전체 합산, 전투 승리 시 적용"

note: "damage_reduction, lifesteal은 고유 효과로 이전됨"
```

### 스탯 성장

```yaml
growth_system:
  components:
    growth_rate: "영웅 성장치 (별 등급별 차이)"
    base_stats: "레벨 1 기준 스탯"
    growth_rates: "레벨당 증가량 (성장치 비례)"

  formula:
    stat_at_level: "기본 스탯 + (성장률 × (레벨 - 1))"
    growth_affected: "hp, attack, defense"
    fixed: "critical_rate, critical_damage, block_rate"

example_comparison:
  star_1:
    growth_rate: 10
    base: "HP 50, 공격 10, 방어 5"
    per_level: "HP +5, 공격 +1, 방어 +0.5"

  star_6:
    growth_rate: 60
    base: "HP 200, 공격 32, 방어 20"
    per_level: "HP +30, 공격 +6, 방어 +3"

  note: "6성은 1성 대비 성장률 6배"
```

## 고유 효과 (★4 이상)

### 효과 카테고리

```yaml
offensive:
  - "확률 전열 공격 : N% 확률로 공격 대상과 같은 열에 있는 모든 적에게 M%의 데미지를 입힘"
  - "확률 전행 공격 : N% 확률로 공격 대상과 같은 행에 있는 모든 적에게 M%의 데미지를 입힘"
  - "전체 공격 : 모든 적에게 M%의 데미지를 입힘"
  - "전체 공격(고급) : M%의 데미지를 살아있는 모든 적에게 균등하게 나누어 데미지를 입힘"
  - "강력한 일격 : 공격 시 M%의 추가 데미지를 입힘"
  - "추가 피해 : 최종 데미지에 +N%"
  - "범위 공격 : N% 확률로 인접 적들에게 M% 피해 전파"
  - "연속 공격 : N% 확률로 즉시 추가 공격 (1회)"
  - "최후의 일격 : 적 HP 30% 이하 시 데미지 +N%"
  - "첫 타격 보너스 : 전투 시작 첫 공격 시 데미지 +N%"
  - "치명타 증폭 : 크리티컬 데미지에 추가로 +N%"

defensive:
  - "받는 피해 감소 -N% (피해 감소율)"
  - "라운드 시작 시 HP 회복 +N"
  - "첫 공격 무효화"
  - "흡혈 : 데미지의 N%만큼 체력 회복"
  - "반사 피해 : 받은 피해의 N%를 공격자에게 반사"
  - "보호막 : 전투 시작 시 최대 HP의 N%만큼 보호막 생성"
  - "블록 : N% 확률로 받는 피해 50% 감소"
  - "피해 무효화 : N 이하의 피해를 받지 않음"
  - "부활 : N% 확률로 HP 50%로 부활 (전투당 1회)"

utility:
  - "파티 경험치 증가 +N%"
  - "드랍률 증가 +N%"
  - "골드 획득 증가 +N%"
  - "희귀템 드랍률 증가 +N%"
  - "치명타 저항 : 적의 치명타 확률 -N%"
  - "영혼 흡수 : 적 처치 시 적 최대 HP의 N%만큼 회복"

debuff:
  - "공격력 감소 : N% 확률로 적 공격력 -M% (3라운드)"
  - "방어력 감소 : N% 확률로 적 방어력 -M% (3라운드)"
  - "치유 감소 : N% 확률로 적 회복 효과 -M% (3라운드)"
  - "둔화 : N% 확률로 적 공격 속도 -M% (2라운드)" # 공격 속도 시스템 도입 시

special:
  - "생존 보너스 : 5라운드마다 공/방 +N% (누적)"
  - "파티 보너스 : 파티원당 스탯 +N%"
```

### 효과 밸런스 및 관리

```yaml
효과 관리 방식:
  - 고유 효과는 인덱스로 관리
  - 효과 정의 테이블에 고정 수치 저장
  - 영웅에게 효과 ID를 할당
  - 수치 변경 불가 (밸런싱은 정의 테이블에서)

star_4_effects:
  count: 1~2
  power: "8-12%"
  example: "강력한 일격 +10%"

star_5_effects:
  count: 2~3
  power: "12-18%"
  example: "아군 전체 공격력 +15%"

star_6_effects:
  count: 3~4
  power: "18-25%"
  example: "적 전체 방어력 -20%"

aura_stacking:
  rule: "동일 효과 가산"
  example: "Hero1 공격+10% + Hero2 공격+8% = 총 +18%"
```

## 레벨 시스템

### 경험치 곡선

```yaml
exp_formula:
  formula: "100 × level^1.5"

examples:
  Lv1→2: 100
  Lv2→3: 282
  Lv3→4: 519
```

### 레벨 상한

```yaml
level_cap:
  initial: 200
```

## 파티 편성

### 파티 규칙

```yaml
party_composition:
  min_size: 1
  max_size: 4
  duplicate: true  # 같은 영웅 중복 가능

element_effect:
  - 동일 속성 영웅끼리만 파티 편성 시 전체 파티원(동일 속성) 공/방/체 10% 보너스(단, 4인 파티에 한함, 사망시에도 유효)
  - 특정 영웅 조합끼리 파티 편성 시 해당 영웅들 공/방/체 N% 보너스(ex:로미오, 줄리엣)

restrictions:
  - 전투 중 편성 변경 불가
  - 마을에서만 편성 가능
```

### 배치 전략

```yaml
tank_build:
  front: "고방어 영웅"
  mid: "균형형 영웅"
  back: "공격형 영웅"

dps_build:
  front: "빠른 딜러"
  mid: "빠른 딜러"
  back: "빠른 딜러"

balanced_build:
  positions: "상황에 따라 혼합"
```

## 획득 방법

```yaml
acquisition:
  starter_heroes:
    count: 4
    grade: [1, 2, 3]
    location: "시작 마을"
    desc: "grade별 랜덤 출현 확률"

  fortress_rewards:
    grades: [4, 5, 6]
    source: "요새 클리어 시 보급형 고밸류 영웅 수집(확률)"
    guaranteed: true

  shop:
    grades: [1, 2, 3, 4, 5, 6]
    cost: "골드"
    desc: "grade별 랜덤 출현 확률"
```

## 스탯 확장 워크플로우

### 새로운 스탯 추가 절차

```yaml
step_1_server_db:
  description: "스탯 정의 추가"
  action: |
    INSERT INTO stat_definitions VALUES
    ('lifesteal', '{"en":"Life Steal","ko":"흡혈"}', 'combat', 'percent', 0.1);

step_2_hero_data:
  description: "영웅 데이터에 스탯 추가 (선택적)"
  action: |
    UPDATE heroes
    SET extended_stats = extended_stats || '{"lifesteal": 0.15}'::jsonb
    WHERE star_grade >= 5;

step_3_server_deploy:
  description: "서버 배포"
  note: "코드 수정 불필요"

step_4_client_sync:
  description: "클라이언트 자동 동기화"
  action: "앱 시작 시 /api/stats/definitions 호출"
  result: "새 스탯 즉시 사용 가능"

step_5_game_logic:
  description: "게임 로직에서 사용"
  code: |
    float lifesteal = heroStats.GetStat("lifesteal");
    // 즉시 사용 가능!
```

### 마이그레이션 비교

```yaml
traditional_approach:
  steps:
    - DB 스키마 마이그레이션 (ALTER TABLE)
    - 서버 모델/DTO 수정
    - 클라이언트 모델 수정
    - 서버 & 클라이언트 동시 배포
  risk: "높음 (동기화 실패 위험)"
  downtime: "있음"

metadata_driven_approach:
  steps:
    - DB INSERT (stat_definitions)
    - 서버 배포 (옵션)
    - 클라이언트 자동 동기화
  risk: "낮음 (자동 동기화)"
  downtime: "없음"
```

## 데이터 구조

> 관련 데이터 구조는 `docs/04-technical/data-structures.md`를 참조하세요.

주요 개념:
- **Hero**: 영웅 정보 (별 등급, 레벨, 스탯, 고유 효과)
- **HeroStats**: Hybrid 구조 (핵심 스탯 + 확장 스탯)
- **StatDefinition**: 스탯 메타데이터 (다국어, 타입, 버전)
- **별 등급 시스템**: 1~6성 (성장률, 고유효과 여부)
- **핵심 스탯**: HP, 공격, 방어 (DB 컬럼)
- **확장 스탯**: 크리티컬, 회피, 흡혈 등 (JSONB)
- **고유 효과**: ★4 이상 전용 (공격/방어/유틸리티 효과)
- **속성**: 물/불/땅/무

### 데이터베이스 스키마

```sql
-- 스탯 정의
CREATE TABLE stat_definitions (
    id VARCHAR(50) PRIMARY KEY,
    display_name JSONB NOT NULL,
    stat_type VARCHAR(20),
    data_type VARCHAR(20),
    version INT DEFAULT 1
);

-- 영웅
CREATE TABLE heroes (
    id UUID PRIMARY KEY,
    -- 핵심 스탯 (컬럼)
    base_hp INT NOT NULL,
    base_attack INT NOT NULL,
    base_defense INT NOT NULL,
    -- 확장 스탯 (JSONB)
    extended_stats JSONB
);
```

## UI 요구사항

### 영웅 목록 화면

```yaml
display:
  - 영웅 초상화
  - 이름
  - 별 등급 (★ 시각화)
  - 레벨
  - 속성 아이콘

sort_options:
  - 별 등급 (기본)
  - 레벨
  - 공격력
  - 획득 순서

filter_options:
  - 속성별
  - 별 등급별
  - 파티 포함 여부
```

### 영웅 상세 화면

```yaml
information:
  - 모든 스탯 표시
  - 경험치 바 (현재/필요)
  - 고유 효과 (★4+)
  - 장착 장비

actions:
  - 파티 편입/해제
  - 장비 장착
  - (미래) 강화, 진화
```

## 시스템 연동

### 의존 시스템
- `equipment.md` - 장비 장착으로 스탯 증가
- `combat.md` - 전투에서 사용, 경험치 획득

### 제공 기능
- 전투 유닛
- 파티 편성 정보
- 고유 효과 제공

## 밸런스 파라미터

```yaml
star_grade_power:
  star_1: 1.0x  # 기준
  star_2: 1.3x
  star_3: 1.6x
  star_4: 2.0x
  star_5: 2.5x
  star_6: 3.2x

drop_rate:
  star_1: 40%
  star_2: 30%
  star_3: 20%
  star_4: 8%
  star_5: 1.8%
  star_6: 0.2%
```

## 구현 체크리스트

- [ ] 영웅 데이터 구조
- [ ] 별 등급 시스템
- [ ] 레벨/경험치 시스템
- [ ] 파티 편성 UI
- [ ] 고유 효과 구현 (★4+)
- [ ] 영웅 목록 화면
- [ ] 영웅 상세 화면
- [ ] 획득 시스템

---
**최종 수정**: 2025-10-18
**상태**: 🔴 초안
**작성자**: SangHyeok
