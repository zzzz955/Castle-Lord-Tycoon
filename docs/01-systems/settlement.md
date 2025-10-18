# 정착지 시스템 (마을 & 요새)

## 개요

```yaml
types: ["town", "fortress"]
auto_conquest: true  # 마을 자동 점령
maintenance: false   # 유지비 없음
```

**핵심 가치**: 복잡한 관리 없이, 연결만 하면 자동 점령 = 스트레스 없는 확장

## 마을 시스템

### 마을 역할

```yaml
recovery:
  hp: "무료 완전 회복"
  resurrection: "사망 영웅 부활"
  debuff: "모든 디버프 제거"
  cost: "없음"

management:
  party: "파티 재구성"
  equipment: "장비 구매/판매/제작"
  inventory: "아이템 관리/전리품 판매"

hub:
  spawn_point: "전멸 시 귀환 지점"
  fast_travel: "마을 간 이동 (향후 확장)"
```

### 자동 점령 규칙

```yaml
conquest_condition:
  rule: "내 마을 → 내 영토만으로 연결 → 새 마을"
  requirements:
    - 하나 이상의 소유 마을 존재
    - 영토 타일로 연속 연결
    - 대상 마을이 중립 상태

  example:
    scenario: "마을A(소유) → 깃발 → 깃발 → 마을B(중립)"
    result: "마을B 자동 점령!"

  instant: true  # 즉시 점령, 추가 비용 없음
```

### 마을 특성

```yaml
starting_town:
  name: "시작 마을"
  owned: true
  territory: "마을 크기만큼 기본 영토"
  special: "첫 거점"

neutral_town:
  state: "중립"
  requirement: "영토 연결"
  reward: "점령 시 일회성 보상 (골드, 아이템), 마을의 영토 크기만큼 내 영토로 편입"

regional_towns:
  difficulty: "주변 필드 난이도"
  spawn_table: "지역별 다름"
  strategic_value: "루트 선택 기준점"
```

## 요새 시스템

### 요새 특징

```yaml
type: "특별 거점"
size: "마을보다 소규모"
entrance: "없음 (필드에서 직접 상호작용)"
```

### 해금 메커닉

```yaml
first_interaction:
  trigger: "요새 타일 접근 (최초)"
  event: "1회 전투 발생"
  difficulty: "중상급"

  victory:
    - 요새 해금
    - 중규모 보상
    - 확정 드랍 (도면/보석/재료)
    - 전용 시설 이용 가능
    - 마을과 깃발 연결 시 본인 소유 영토("owned")로 편입

  defeat:
    - 요새 잠금 유지
    - 재도전 가능 (무한)
    - 페널티 없음
```

### 해금 후 기능

```yaml
unlocked_features:
  shop:
    - 전용 상점
    - 주간 재고 (일주일마다 리셋)
    - 희귀 아이템

  exchange:
    - 재료 교환소
    - 전용 교환표 (요새별 다름)

  crafting:
    - 제작대
    - 특수 장비 제작
    - 요새 전용 레시피

strategic_value:
  - 지역별 전용 보상
  - 성장 가속화
  - 수집 목표
```

## 정착지 배치

### 월드 디자인

```yaml
town_placement:
  density: "중간 (각 지역 3-5개)"
  spacing: "적절한 거리 유지"
  connectivity: "다양한 루트 가능"

fortress_placement:
  density: "낮음 (마을과 마을 사이 1개씩 배치)"
  location: "다음 마을에 접근하기 위한 중요 거점, 도전적 위치"
  access: "난이도 높은 경로"
```

## 데이터 구조

```typescript
interface Town {
  id: string;
  name: string;
  position: { x: number; y: number };
  size: number;  // 기본 영토 크기

  owned: boolean;
  conqueredAt?: number;  // timestamp

  region: string;
  difficulty: number;

  features: {
    recovery: boolean;
    shop?: Shop;
    storage?: boolean;
  };
}

interface Fortress {
  id: string;
  name: string;
  position: { x: number; y: number };
  size: number; // 기본 영토 크기

  unlocked: boolean;
  unlockedAt?: number;

  firstBattle: Battle;  // 해금 전투
  rewards: Item[];      // 확정 드랍

  features: {
    shop: Shop;
    exchange: Exchange;
    crafting: CraftingStation;
  };
}

interface Settlement {
  towns: Map<string, Town>;
  fortresses: Map<string, Fortress>;

  checkAutoConquest(): Town[];
  unlockFortress(fortressId: string): boolean;
}
```

## UI 요구사항

### 마을 진입 화면

```yaml
town_ui:
  main_menu:
    - "회복 (무료)" - 즉시 전체 회복
    - "파티 편성"
    - "장비 관리"
    - "상점" (있는 경우)
    - "나가기"

  info_panel:
    - 마을 이름
    - 지역 정보
    - 주변 난이도
```

### 요새 상호작용

```yaml
locked_fortress:
  display:
    - 자물쇠 아이콘
    - "도전하기" 버튼
    - 예상 난이도 표시

unlocked_fortress:
  display:
    - "상점"
    - "교환소"
    - "제작대"

  special_indicator:
    - 주간 재고 리셋까지 시간
    - 미획득 아이템 알림
```

### 자동 점령 피드백

```yaml
conquest_notification:
  trigger: "영토 연결 완성 시"
  display:
    - "새 마을 점령!" 팝업
    - 마을 이름
    - 일회성 보상 표시
    - 지도에 소유권 표시
```

## 시스템 연동

### 의존 시스템
- `territory.md` - 영토 연결 판정
- `combat.md` - 요새 해금 전투

### 제공 기능
- 회복/부활
- 파티 관리
- 상점/교환/제작
- 귀환 지점

## 밸런스 파라미터

```yaml
town_rewards:
  first_conquest:
    gold: 100-500  # 마을 크기/난이도 비례
    items: 1-3

fortress_rewards:
  unlock:
    gold: 500-2000
    guaranteed_drop: 1  # 도면/보석/재료
    bonus_items: 2-5

  weekly_shop:
    reset_cycle: 7  # 일
    unique_items: 3-5
```

## 확장 가능성

```yaml
future_features:
  towns:
    - 마을 업그레이드
    - 마을별 특화 (교역, 제작 등)
    - NPC 퀘스트

  fortresses:
    - 난이도 변형 (하드 모드)
    - 반복 보상 (일일/주간)
    - 요새 간 연계 퀘스트
```

## 구현 체크리스트

- [ ] 마을 데이터 구조
- [ ] 자동 점령 로직
- [ ] 마을 UI (회복, 편성 등)
- [ ] 요새 데이터 구조
- [ ] 요새 해금 전투
- [ ] 요새 상점/교환/제작
- [ ] 점령 알림 UI

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
