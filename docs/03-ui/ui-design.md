# UI 설계

## 화면 구성

### 메인 필드 화면

```yaml
layout:
  main_viewport:
    - 아이소메트릭 필드 렌더링
    - 캐릭터/오브젝트 표시
    - 안개 효과 (3단계)
    - 영토 하이라이트

  top_hud:
    left:
      - 계급 아이콘 + 이름
      - 영토 면적 표시
    right:
      - 골드
      - 현재 위치

  bottom_hud:
    left:
      - 파티 영웅 (HP 바)
      - 빠른 상태 확인
    right:
      - 야영 버튼
      - 깃발 버튼
      - 귀환 버튼
      - 메뉴 버튼

  minimap:
    position: "좌측 상단"
    size: "150x150"
    display:
      - 근처 맵 축소판
      - 안개 상태 색상
      - 마을/요새 아이콘
      - 영토 상태 별 색상 노출(이동 불가 지형, 미점령, 마을, 요새, 점령)
```

### 조우 화면
```yaml
layout:
  grid_display:
    - 적군 배치 정보 확인
    - 최초 배치 or 영웅 변경 시 기본적으로 우선순위 1, 2, 3, 4 배치
    - 이전 배치내역이 있다면 영웅별 이전 배치 위치 기억 및 노출
    - 배치 방법은 영웅 클릭 후 빈 셀 클릭 시 해당 셀로 이동, 혹은 이미 배치된 영웅 클릭 시 해당 영웅과 자리 변경

  info_panel:
    top-right(1): "도망치기"
    top-right(2): "전투시작"
```

### 전투 화면

```yaml
layout:
  grid_display:
    - 3×3 그리드 (아군/적군 구분, 아군 좌측, 적군 우측, 적군의 경우 아군과 좌우 반전)
    - 영웅 및 적군 HP 바

  info_panel:
    top: "라운드 카운터"
    top-left: 영웅 초상화 및 속성 아이콘, 상태(사망)
    top-right: 적군 초상화 및 속성 아이콘, 상태(사망)
    side: "현재 행동 순서"

  controls:
    - 스킵 버튼
    - 속도 조절 (1x, 2x, 3x)
```

### 마을 화면

```yaml
menu:
  - 회복 (무료)
  - 파티 편성
  - 인벤토리
  - 상점 (있는 경우)
  - 나가기

quick_info:
  - 마을 이름
  - 지역 정보
```

### 인벤토리

```yaml
layout:
  left_panel:
    - 영웅 목록
    - 필터/정렬

  center_panel:
    - 선택된 영웅 상세
    - 스탯 표시
    - 장착 슬롯 (무기/방어구/악세x2)

  right_panel:
    - 장비 목록
    - 필터/정렬 (타입, 등급)
```

## UI 원칙

### 정보 계층

```yaml
priority_1:
  - HP (생존 관련)
  - 현재 위치
  - 전투 상태

priority_2:
  - 골드
  - 계급
  - 영토

priority_3:
  - 상세 스탯
  - 옵션
```

### 색상 시스템

```yaml
territory:
  owned: "#00FF00"  # 형광 연두
  explored: "#808080"  # 회색
  unexplored: "#000000"  # 검은색

equipment_grades:
  C: "#FFFFFF"  # 흰색
  UC: "#7FFF00"  # 연두
  R: "#FFFF00"  # 노랑
  H: "#9370DB"  # 보라
  L: "#FFA07A"  # 연한 주황

elements:
  water: "#0099FF"  # 파랑
  fire: "#FF4500"  # 빨강
  earth: "#8B4513"  # 갈색
  none: "#CCCCCC"  # 회색
```

### 접근성

```yaml
font_size:
  min: 24  # 최소 가독성
  ui_text: 36
  important: 48

contrast:
  text_bg: "최소 4.5:1"
  important: "최소 7:1"

icons:
  - 색상 + 모양 구분 (색맹 고려)
  - 툴팁 제공
```

## 반응형 디자인

```yaml
mobile:
  resolution: "FHD(2340x1080) 기준"
  scaling: "16:9~20:9 대응"
  input: "터치 중심 UX"
  future: "태블릿/PC 포팅은 MVP 이후 별도 계획 수립"
```

## 구현 우선순위

```yaml
mvp:
  - 필드 화면
  - 전투 화면
  - 마을 화면 (기본)

phase_2:
  - 인벤토리 (전체)
  - 파티 편성
  - 미니맵

phase_3:
  - 계급 화면
  - 요새 UI
  - 설정/옵션
```

---
**최종 수정**: 2025-10-19
**상태**: 🔴 초안
**작성자**: SangHyeok
