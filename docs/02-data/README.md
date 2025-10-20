# Data Specifications

이 디렉터리는 스프레드시트 기반 메타 데이터를 정의하고, `tools/build-meta` 스크립트가 생성하는 JSON 산출물과의 매핑을 설명합니다. 시스템 문서는 여기서 정의된 키와 파일을 참조합니다.

## 작업 절차

1. 스프레드시트에서 테이블을 수정하고 `data-glossary.md`에 있는 탭 규칙을 따른다.  
2. 로컬에서 `tools/build-meta`를 실행해 CSV/JSON을 생성한다.  
3. 산출물과 `meta/manifest.json`을 커밋하고, `CHANGELOG.md`에 변경사항을 기록한다.  
4. CI가 메타 버전을 확인해 서버/클라이언트 배포에 반영한다.

## 문서 목록

| 문서 | 내용 |
| --- | --- |
| `data-glossary.md` | 테이블/출력 파일 목록 및 상태 |
| `balance-formulas.md` | 전투/성장 공식과 파라미터 정의 |
| `monsters.md` | 몬스터 템플릿 스키마와 작성 규칙 |
| `equipment-pool.md` | 장비 템플릿/옵션 스키마 및 샘플 |
| `regions.md` | 월드 지역/바이옴 템플릿 스키마 |

## 포맷 규칙

- **CSV**: 기본 저장 포맷. 첫 행은 필드명, snake_case 사용.  
- **JSON**: 배포용 포맷, `tools/build-meta`가 생성.  
- **샘플 코드**: 문서에서는 TypeScript/C# 인터페이스로 스키마 예시만 제공한다.  
- **버전 필드**: 각 산출물에는 `meta_version`을 포함시켜 서버가 클라이언트 업데이트를 통지할 수 있도록 한다.

---
**최종 수정**: 2025-10-21  
**상태**: 개편중  
**작성자**: SangHyeok  
