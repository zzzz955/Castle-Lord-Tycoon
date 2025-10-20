# Client–Server Contract (MVP)

본 문서는 Unity 클라이언트와 ASP.NET Core 서버 사이의 REST/SignalR 계약을 정의합니다. 모든 응답은 `application/json`을 사용하며, 성공 시 `{"success": true, "data": ...}` 형식을 따른다.

## 1. Authentication

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `POST` | `/api/auth/register` | 신규 계정 생성 (최소 정보) |
| `POST` | `/api/auth/login` | 로그인 + Access/Refresh 토큰 발급 |
| `POST` | `/api/auth/refresh` | Refresh 토큰으로 Access 토큰 재발급 |

> 모든 보호된 엔드포인트는 `Authorization: Bearer <access_token>` 헤더가 필요하다.

## 2. Metadata Sync

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `GET` | `/api/meta/manifest` | 현재 메타 버전 및 테이블 해시 제공 |
| `GET` | `/api/meta/download/{tableId}` | 지정 테이블(JSON) 다운로드 |

### SignalR 이벤트

| Event | Payload | 설명 |
| --- | --- | --- |
| `MetaUpdated` | `{ "metaVersion": number }` | 서버가 새로운 메타를 배포했음을 알림 |

클라이언트는 로그인 및 `MetaUpdated` 수신 시 manifest를 비교하고 필요한 테이블만 갱신한다.

## 3. Player Profile

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `GET` | `/api/player` | 프로필(닉네임, 계급, 골드, 메타 버전) |
| `PUT` | `/api/player/settings` | 옵션(언어, 사운드 등) 저장 |
| `GET` | `/api/player/progression` | 영토 면적, 마을/요새 수, 계급 진행 |

## 4. Heroes & Party

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `GET` | `/api/heroes` | 플레이어 보유 영웅 목록 |
| `POST` | `/api/heroes/{heroId}/equip` | 장비 착용/해제 |
| `POST` | `/api/heroes/{heroId}/level-up` | 경험치 소비 후 레벨 업 |
| `GET` | `/api/party` | 현재 출전 파티 |
| `PUT` | `/api/party` | 파티 편성 변경 |

> 모든 장비/영웅 검증은 서버 권한으로 수행한다.

## 5. Combat

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `POST` | `/api/combat/start` | 조우 정보와 파티 구성으로 전투 시작 |
| `POST` | `/api/combat/resolve` | 서버 계산 후 결과(승패, 보상, 로그) |
| `GET` | `/api/combat/logs/{combatId}` | 선택적 리플레이 데이터 |

### SignalR 이벤트

| Event | Payload | 설명 |
| --- | --- | --- |
| `CombatAnnouncement` | `{ encounterId, seed }` | 서버가 실시간 조우를 푸시 (옵션) |

## 6. Territory & Settlements

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `GET` | `/api/territory` | 보유 깃발, 영역, 연결 상태 |
| `POST` | `/api/territory/flags` | 깃발 배치/회수 요청 |
| `GET` | `/api/settlements` | 점령된/발견한 정착지 목록 |
| `POST` | `/api/settlements/claim` | 요새 전투 완료 후 개방 |

## 7. Economy

| Method | Endpoint | 설명 |
| --- | --- | --- |
| `GET` | `/api/shop/{shopId}` | 상점 재고(메타 버전 포함) |
| `POST` | `/api/shop/{shopId}/purchase` | 아이템 구매 |
| `GET` | `/api/exchange` | 교환 가능 목록 |
| `POST` | `/api/exchange` | 재화 교환 |
| `GET` | `/api/crafting` | 제작 큐 / 레시피 정보 |
| `POST` | `/api/crafting/start` | 제작 시작 |
| `POST` | `/api/crafting/collect` | 제작 완료물 수령 |

## 8. SignalR Connection Lifecycle

- **Connect**: 로그인 후 `/hub/game` 연결, JWT 토큰 사용.  
- **Heartbeat**: 클라이언트 → 서버 `SendHeartbeat()` (30초 간격).  
- **Server Events**: `MetaUpdated`, `RankUp`, `SettlementClaimed`, `EconomyRefresh`.  
- **Reconnect**: 연결 끊김 시 백오프 기반 재시도.

## Common Response Schema

```json
{
  "success": true,
  "data": {},
  "metaVersion": 12   // 선택: 응답 시 최신 메타 버전을 알려 갱신 여부 판단
}
```

오류 응답은 `{"success": false, "error": {"code": "...", "message": "..."} }` 형식을 사용한다.

---
**최종 수정**: 2025-10-21  
**상태**: 초안  
**작성자**: SangHyeok  
