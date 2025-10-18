# 01-systems - 핵심 시스템

## 목적
게임의 핵심 메커닉과 시스템을 정의하는 영역

## 시스템 목록

### world-exploration.md
필드 탐험, 전장의 안개, 야영 시스템

### territory.md
깃발 설치, 영토 편입 메커닉

### settlement.md
마을, 요새, 자동 점령 시스템

### combat.md
3×3 그리드 자동 전투, 속성 상성, 보상 분배

### character.md
영웅 시스템, 별(★) 등급, 성장, 파티 편성

### equipment.md
장비 등급, 옵션 구조, 드랍 시스템

### encounter.md
조우 발생, 스폰 테이블, 지역 난이도

### progression.md
계급(작위) 시스템, 혜택, 성장 곡선

## 시스템 연관도

```
World Exploration → Encounter → Combat
                                   ↓
Character + Equipment → Combat Result → Progression
                                   ↓
                            Territory Expansion
                                   ↓
                            Settlement Conquest
```

## 작성 가이드

각 시스템 문서는:
1. **독립적으로 이해 가능**해야 함
2. **다른 시스템과의 연동** 명시
3. **구현 가능한 수준의 구체성** 유지
4. **데이터 구조 예시** 포함
