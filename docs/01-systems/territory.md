# 영토 시스템

## 개요

```yaml
type: "깃발 기반 영토 확장"
flag_sizes: [3, 5, 7]  # S, M, L
expansion_method: "깃발 설치"
visualization: "투명화가 적용된 형광 연두빛"
```

**핵심 가치**: 깃발만 꽂으면 자동으로 영토 확장 = 직관적 성취감

## 깃발 시스템

### 깃발 크기

```yaml
flag_s:
  name: "소형 깃발"
  size: "3x3"
  cost: 10  # 골드 (예시)
  unlock: "시작부터"

flag_m:
  name: "중형 깃발"
  size: "5x5"
  cost: 30
  unlock: "계급 남작"

flag_l:
  name: "대형 깃발"
  size: "7x7"
  cost: 60
  unlock: "계급 후작"
```

### 설치 규칙

```yaml
placement:
  toggle:
    - 깃발 설치 모드로 변경
    - 현재 위치 주변의 N*N타일에 배치 시 편입될 영토 미리보기 노출

  allowed:
    - 마을/요새가 아닌 타일
    - 탐험된 타일
    - 비전투 상태
    - 기존 깃발이 없는 타일("owned"상태이나 깃발이 없는 타일은 허용)

  forbidden:
    - 전투 중
    - 조우 중
    - 야영 중
    - 이미 깃발이 있는 타일
    - 차단 타일 (산, 물)

  effect:
    - NxN 타일이 즉시 영토로 편입
    - 영토 하이라이트 적용
    - 전장의 안개 "owned" 상태로 변경
```

### 깃발 제거

```yaml
removal:
  method: "우클릭 또는 전용 UI"
  cost: "없음 (무료 제거)"
  restriction:
    - 마을 연결에 필수적인 깃발은 경고
    - 제거 시 영토 상실

  use_case:
    - 잘못된 위치에 설치
    - 더 큰 깃발로 교체
    - 계획 변경
```

## 영토 편입

### 영토 정의

```yaml
territory:
  definition: "깃발로 지정된 NxN 타일 영역"
  ownership: "플레이어 소유"
  visual: "형광 연두빛 하이라이트"
  alpha: 0.3  # 반투명

effects:
  - 전장의 안개 완전 제거
  - 마을 자동 점령 조건 충족
  - 시각적 만족감
```

### 영토 연결

```yaml
connectivity:
  rule: "타일이 인접해야 연결됨"
  diagonal: false  # 대각선 연결 불가
  path_finding: "마을-마을 영토 경로 추적"

  example:
    scenario: "마을A → 영토 → 영토 → 마을B"
    result: "마을B 자동 점령"
```

## 영토 시각화

### 하이라이트 효과

```yaml
highlight:
  color: "형광 연두 (#00FF00)"
  alpha: 0.3
  animation: "옵션 - 펄스 효과"

border:
  display: true
  color: "밝은 녹색"
  thickness: 2  # 픽셀
```

### 미니맵 표시

```yaml
minimap:
  owned_territory: "밝은 녹색"
  explored: "회색"
  unexplored: "검은색"
  towns: "연한 핑크색"
  fortresses: "연한 보라색"
  flags: "작은 점"
```

## 전략적 요소

### 확장 전략

```yaml
strategies:
  aggressive:
    - 빠르게 넓은 영토 확보
    - 대형 깃발 위주
    - 비용 부담 큼

  conservative:
    - 마을 연결 최소 경로
    - 소형 깃발 위주
    - 비용 절약

  balanced:
    - 중요 지점에 중/대형
    - 연결 부분에 소형
```

### 비용 관리

```yaml
cost_optimization:
  - 소형으로 경로 연결
  - 중요 거점에 대형 투자
  - 불필요한 영토 최소화

future_income:
  - 영토 크기 → 계급 상승
  - 계급 → 보상 증가
  - 투자 대비 수익
```

## 데이터 구조

```typescript
interface Flag {
  id: string;
  size: 3 | 5 | 7;
  position: { x: number; y: number };
  placedAt: number;  // timestamp
}

interface Territory {
  tiles: Set<string>;  // "x,y" 형식
  flags: Flag[];
  totalArea: number;
  connectedTowns: string[];  // town IDs
}

interface FlagPlacement {
  validate(x: number, y: number, size: number): boolean;
  place(x: number, y: number, size: number): boolean;
  remove(flagId: string): boolean;
  getOwnedTiles(): Set<string>;
}
```

## UI 요구사항

### 깃발 설치 UI

```yaml
placement_mode:
  trigger: "깃발 설치 모드 토글"
  display:
    - 설치 가능 타일 하이라이트
    - 깃발 크기 선택 (S/M/L, 단 해당 깃발 보유 시)
    - 영역 프리뷰 (NxN 표시)
    - 비용 표시

  feedback:
    - 설치 성공: "확장 효과"
    - 설치 실패: "경고 메시지"
```

### 영토 관리 UI

```yaml
territory_panel:
  statistics:
    - 총 영토 면적
    - 설치된 깃발 수
    - 점령된 마을 수
    - 현재 계급

  actions:
    - 깃발 제거 모드
    - 영토 통계 보기
    - 확장 비용 계산기
```

## 시스템 연동

### 의존 시스템
- `world-exploration.md` - 필드 타일 정보
- `settlement.md` - 마을 자동 점령

### 제공 기능
- 영토 소유권 정보
- 마을 연결 상태
- 계급 계산 기준

## 밸런스 파라미터

```yaml
flag_costs:
  small: 100  # 골드
  medium: 500
  large: 2000

available_flags:
  default: 20  # 계급에 상관 없이 기본값
  expansion: 5  # 계급 증가 시 마다 깃발 배치 가능 개수 증가

area_coverage:
  small: 9  # 3x3
  medium: 25  # 5x5
  large: 49  # 7x7

cost_per_tile:
  small: 11.11  # 10/9
  medium: 20  # 30/25
  large: 40.82  # 60/49
  note: "대형이 타일당 비용 높음 (균형), 깃발 배치 가능 개수가 정해져있으므로 추후 large배치 필수"
```

## 확장 가능성

```yaml
future_features:
  - 영토 내 특수 건물 설치
  - 영토 보너스 (자원 생산 등)
```

## 구현 체크리스트

- [ ] 깃발 데이터 구조
- [ ] 깃발 설치/제거 로직
- [ ] 영토 타일 추적
- [ ] 영토 연결 판정
- [ ] 하이라이트 렌더링
- [ ] 깃발 설치 UI
- [ ] 미니맵 영토 표시

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
