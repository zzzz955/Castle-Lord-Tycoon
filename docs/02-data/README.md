# 02-data - 게임 데이터

## 목적
게임 콘텐츠의 실제 데이터를 정의하는 영역

## 데이터 문서 목록

### monsters.md
몬스터 종류, 스탯, 스킬, 드랍 테이블

### equipment-pool.md
장비 이름, 등급, 옵션, 레벨 밴드

### regions.md
지역 정보, 난이도, 바이옴 속성, 스폰 테이블

### balance-formulas.md
데미지 계산, 경험치 곡선, 드랍률 등 밸런스 공식

## 데이터 형식

### 테이블 형식
```markdown
| ID | Name | Stat1 | Stat2 |
|----|------|-------|-------|
| 001| Item | 10    | 20    |
```

### YAML 형식
```yaml
monster_001:
  name: "슬라임"
  stats:
    hp: 50
    atk: 10
```

### JSON/TypeScript 스키마
```typescript
interface Monster {
  id: string;
  name: string;
  stats: Stats;
}
```

## 작성 가이드

- **CSV/JSON 변환 가능**한 구조 유지
- **스키마 먼저 정의**, 데이터는 나중에 채움
- **프로시저럴 생성 규칙** 명시 (수동 입력 최소화)
- **밸런스 공식** 명확히 문서화
