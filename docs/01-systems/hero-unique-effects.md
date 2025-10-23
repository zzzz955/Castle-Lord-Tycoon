# 영웅 고유 효과 기획서

## 목적
- 영웅이 보유한 고유 효과(Unique Effect)를 정의하고, 전투 시스템이 일관된 방식으로 처리할 수 있도록 템플릿 구조와 데이터 스키마를 명세한다.
- 고유 효과는 영웅 정보 패널 등에서는 참고용으로 노출되지만, 실제 수치 적용은 전투 중 서버 로직이 담당한다.
- 공격형/방어형/유틸형 등 다양한 유형을 확장 가능하게 설계하며, 향후 새로운 트리거·대상·액션이 추가되어도 기존 데이터를 수정하지 않고 확장할 수 있어야 한다.
- 모든 설명 문자열은 기본 언어(한글)로 작성하되, 인덱스 기반 로컬라이제이션을 적용할 수 있도록 설계한다.

## 핵심 개념
고유 효과는 다음 구성요소의 조합으로 정의한다.
1. **트리거(Trigger)**: 언제 발동할 것인가?  
2. **조건(Condition, 선택)**: 추가적인 발동 조건이 있는가?  
3. **대상 선택(Target Selector)**: 누구에게 영향을 미치는가?  
4. **액션(Action)**: 어떤 효과가 적용되는가?  
5. **파라미터(Parameter)**: 영웅별로 다른 수치, 횟수, 지속시간 등을 채울 값.

각 구성요소는 별도의 테이블로 분리된 뒤, 최종 템플릿과 영웅 매핑에서 조합된다.

## 데이터 스키마

### 1. 효과 템플릿
| 컬럼 | 설명 |
| --- | --- |
| `effect_id` | 고유 효과 템플릿 ID (고유값) |
| `name_token_ko` | 효과 이름(기본 한글). 로컬라이즈 키(예: `effect.name.001`)를 함께 기재 |
| `description_template_ko` | 효과 설명 템플릿(기본 한글, `{value1}` 플레이스홀더 사용). 로컬라이즈 키 병행 |
| `trigger_id` | 발동 트리거 참조 |
| `condition_id` | 조건 참조 (없으면 `NULL` 또는 0) |
| `target_selector_id` | 대상 선택 규칙 참조 |
| `action_id` | 액션 정의 참조 |
| `param_count` | 액션 실행 시 요구되는 파라미터 개수 |
| `category` | 공격형/방어형/유틸형 등 분류 태그 |
| `notes` | 설계 주석 |

### 2. 트리거
| 컬럼 | 설명 |
| --- | --- |
| `trigger_id` | 트리거 ID |
| `trigger_type` | `battle_start`, `round_start`, `on_attack`, `on_hit`, `on_death`, `on_hp_below`, 등 |
| `options_json` | 트리거 옵션(JSON 또는 key-value). 예: `{ "max_rounds": 3, "probability": 1500 }` ← 1500 = 15.00% |
| `description_token_ko` | UI 표기용 기본 설명 (필요 시) |

### 3. 조건 (선택)
| 컬럼 | 설명 |
| --- | --- |
| `condition_id` | 조건 ID |
| `condition_type` | `stat_compare`, `ally_dead`, `with_hero`, `enemy_attribute`, 등 |
| `options_json` | 비교할 값, 특정 영웅 ID 등 |

조건이 없다면 `condition_id = 0` 혹은 `NULL`.

### 4. 대상 선택
| 컬럼 | 설명 |
| --- | --- |
| `selector_id` | 선택기 ID |
| `target_type` | `self`, `ally_all`, `ally_random`, `enemy_front`, `enemy_random`, `enemy_column`, `ally_lowest_hp`, 등 |
| `options_json` | 선택 수(`count`), 우선 순위(`stat=min_hp`), 특정 열/행 정보 등 |
| `description_token_ko` | 필요 시 UI에서 대상 설명에 사용할 기본 문자열 |

### 5. 액션
| 컬럼 | 설명 |
| --- | --- |
| `action_id` | 액션 ID |
| `action_type` | `deal_damage`, `multi_hit`, `apply_debuff`, `apply_buff`, `revive`, `shield`, `lifesteal`, `resource_bonus`, 등 |
| `param_schema` | 파라미터 의미 정의(JSON). 예: `[{ "key": "damage_percent", "type": "int", "scale": 100 }]` |
| `duration_type` | 즉시/지속/스택/턴 단위 등 |
| `description_token_ko` | 액션 자체 설명(선택) |

### 6. 영웅-효과 매핑
| 컬럼 | 설명 |
| --- | --- |
| `hero_id` | 영웅 ID |
| `effect_id` | 템플릿 참조 |
| `param_slot_1..n` | 템플릿에서 요구하는 파라미터 값을 채움(고정소수 정수) |
| `priority` | 발동 순서(동일 시점에 여러 효과가 있을 때) |
| `is_active` | 잠금/해금 여부 제어 |

파라미터는 템플릿의 `param_count`와 `param_schema`에 맞춰 채운다. 예: `damage_percent=15000` → 150.00%.

## 트리거 / 조건 / 대상 / 액션 예시

| 분류 | 예시 값 | 옵션 |
| --- | --- | --- |
| 트리거 | `battle_start`, `round_start`, `on_first_attack`, `on_first_hit`, `on_attack`, `on_hit`, `on_ally_death`, `on_enemy_death`, `on_self_death`, `on_hp_threshold`, `random_chance` | `max_rounds`, `max_hits`, `probability (basis point)`, `hp_threshold`, `enemy_type` |
| 조건 | `stat_compare`, `with_hero`, `enemy_attribute`, `ally_count`, `self_buff_state` | 비교 스탯, 대상 영웅 ID, 속성 태그 등 |
| 대상 | `self`, `ally_all`, `ally_random`, `ally_lowest_hp`, `enemy_all`, `enemy_random`, `enemy_column`, `target_chain`, `{hero_id}_synergy` | `count`, `stat=hp_min`, `line=row`, `include_dead=true` 등 |
| 액션 | `deal_damage`, `multi_hit`, `split_damage`, `reduce_attack`, `reduce_defense`, `ignore_defense`, `extra_hit`, `guaranteed_hit`, `stun`, `silence`, `poison`, `damage_reduction`, `revive`, `reduce_crit`, `buff_attack`, `buff_defense`, `heal_percent`, `increase_probability`, `exp_bonus`, `gold_bonus`, `drop_bonus`, `shield`, `force_attack_self`, `lifesteal`, `dodge_chance` | 파라미터 예: `damage_percent`, `hit_count`, `duration_rounds`, `amount_percent`, `max_stacks`, `dodge_percent` 등 |

새로운 트리거/조건/대상/액션이 필요하면 각 테이블에 행을 추가한 뒤 템플릿에서 참조만 하면 된다.

## 로컬라이제이션 원칙
- 모든 텍스트 컬럼에 기본 한글 문구와 함께 로컬라이즈 키(예: `effect.desc.001`)를 기록한다.
- UI 렌더링 시에는 로컬라이즈 키를 사용하고, 한글 문구는 기본 언어 혹은 개발 중 참고용으로 활용한다.
- 영웅 이름, 효과 이름, 설명 텍스트 등 모든 테이블에서 동일한 정책을 적용한다.

## 고정소수 규칙
- 모든 수치는 정수 기반 고정소수(예: 값 × 100 혹은 × 10,000)로 저장한다.
- 예: `damage_percent=15000`은 150.00%, `probability=2500`은 25.00%.
- UI 노출 시 `docs/04-technical/data-format.md`에 정의된 반올림/표시 규칙을 따른다.

## 예시
### 1) 템플릿
| effect_id | name | trigger_id | condition_id | target_selector_id | action_id | param_count | description_template |
| --- | --- | --- | --- | --- | --- | --- | --- |
| UE0001 | effect.name.001 / "경갑 돌격" | TRIG_ON_FIRST_ATTACK | COND_NONE | TSEL_TARGET | ACT_DEAL_DAMAGE_MULTI | 2 | effect.desc.001 / "첫 공격 시 {value1}% 데미지로 {value2}회 추가 공격" |

### 2) 영웅 매핑
| hero_id | effect_id | param1 | param2 |
| --- | --- | --- | --- |
| HERO_NAPOLEON | UE0001 | 15000 | 2 |
| HERO_JOAN | UE0001 | 18000 | 1 |

→ 나폴레옹: 첫 공격 시 150% 데미지로 2회 추가 공격  
→ 잔 다르크: 첫 공격 시 180% 데미지로 1회 추가 공격

### 3) 회피형 액션 예시
| effect_id | name | trigger_id | target_selector_id | action_id | param_count | description_template |
| --- | --- | --- | --- | --- | --- | --- |
| UE0105 | effect.name.105 / "기민한 회피" | TRIG_ON_HIT | TSEL_SELF | ACT_DODGE_CHANCE | 1 | effect.desc.105 / "피격 시 {value1}% 확률로 공격을 회피한다" |

| hero_id | effect_id | param1 |
| --- | --- | --- |
| HERO_FIELD_BOSS | UE0105 | 2500 |

→ 해당 보스는 피격 시 25% 확률로 공격을 회피한다(25.00%).

## 구현 참고
1. 전투 엔진은 영웅 진입 시 `hero_effects`를 불러와 템플릿과 병합하여 효과 객체를 만든다.
2. 각 트리거 시점마다 조건 검증 → 대상 선택 → 액션 실행 순서로 처리한다.
3. 액션은 스택/지속 여부를 추적하고, 종료 조건(라운드 수, 횟수 등)을 관리한다.
4. UI에서는 `description_template`와 파라미터를 조합해 최종 설명을 렌더한다.

## 향후 과제 (Stub)
1. 구체적인 트리거/대상/액션 목록 확정 및 옵션 키 정의.
2. 전투 로그/리플레이에서 고유 효과 발동을 표시하는 규칙 결정.
3. 특정 효과 간 우선순위/충돌 처리 기준 정의.
4. 보호 장치(중복 발동 제한, 밸런스 검증 툴) 기획.
5. 개발용 Validator 스크립트 제작(CSV → JSON → 검증).

---
**최종 수정**: 2025-10-23  
**상태**: 초안  
**작성자**: SangHyeok  
