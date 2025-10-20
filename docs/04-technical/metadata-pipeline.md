# Metadata Pipeline

구글 스프레드시트에서 메타 데이터를 작성하고, JSON 산출물을 생성해 서버/클라이언트가 공유하는 파이프라인을 정의합니다.

## Overview

```
Google Sheet (source)
    ↓ export (CSV)
tools/build-meta (local or CI)
    ↓ validate / transform
meta/*.json + manifest.json
    ↓ commit & deploy
Server/Client consume via manifest version
```

## Components

| 단계 | 설명 | 도구 |
| --- | --- | --- |
| Sheet Export | `https://docs.google.com/spreadsheets/d/<ID>/gviz/tq?tqx=out:csv&sheet=<TAB>` | Curl/Node fetch |
| Validation | 스키마 검증, ID 중복 체크, 참조 무결성 확인 | `tools/build-meta` |
| Transformation | CSV → JSON, manifest 업데이트 | 동일 |
| Distribution | Git 커밋 또는 아티팩트 업로드 | GitHub Actions |

## Manifest

`meta/manifest.json` 예시:

```json
{
  "meta_version": 12,
  "generated_at": "2025-10-21T13:00:00Z",
  "tables": {
    "battle_rules": { "hash": "..." },
    "hero_templates": { "hash": "..." }
  }
}
```

- `meta_version`은 업데이트 시 자동 증가한다.  
- 서버는 manifest를 읽어 클라이언트 버전과 비교하고, 차이가 있으면 SignalR로 “메타 업데이트” 이벤트를 보낸다.

## CI Workflow (예시)

1. `main` 브랜치에 변경사항 머지 → GitHub Actions 트리거.  
2. `tools/build-meta` 실행 → CSV 다운로드, 검증, JSON 생성.  
3. 변경된 산출물이 있다면 `meta_version`+1 후 커밋(`meta: bump vXX`).  
4. 필요 시 서버 배포 파이프라인에서 최신 메타를 포함해 배포한다.  
5. 클라이언트는 로그인 시 manifest 버전을 확인하고, mismatch 발생 시 메타 파일을 다운로드한다.

## Failure Handling

- 검증 실패 시 CI는 작업을 중단하고 결과를 Slack/메일로 알린다.  
- 수동으로 시트를 수정한 경우, 다시 `build-meta`를 실행하고 재커밋한다.  
- manifest 업데이트 없이 JSON만 변경되는 것을 방지하기 위해 CI에 “변경 발견 → manifest 증가” 체크를 넣는다.

## APIs & Contracts

- `client-server-contract.md`에 `/meta/manifest` (GET)와 `/meta/download/{table}` (GET) 엔드포인트를 추가한다.  
- 서버는 manifest 버전을 캐싱하고, 클라이언트는 로그인 시점과 포그라운드 복귀 시점에 버전을 비교한다.

## TODO

- `tools/build-meta` 초기 구현.  
- manifest 파일 형식 확정 및 버전 충돌 대응 정책(강제 로그아웃/자동 재시도) 정의.  
- 스프레드시트 ID와 탭 이름을 `.env` 또는 설정 파일로 분리.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
