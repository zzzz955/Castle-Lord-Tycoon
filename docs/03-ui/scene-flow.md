# Scene & Flow Blueprint (MVP)

## 1. 목적
- **부팅 → 탐험 → 전투 → 보상** 루프를 하나의 오픈월드 씬 안에서 구현하기 위한 장면 구성 원칙을 정의한다.
- 자동 로그인, 메타데이터 버전 검증, UI 오버레이를 포함한 공통 진입 동작을 명확히 문서화해 클라이언트/서버 작업을 동기화한다.
- 청크 기반 월드 스트리밍과 UI 기반 전투 표현을 병행할 때 발생할 씬·오브젝트 책임 범위를 정리한다.

## 2. 씬 인벤토리
| 씬/오버레이 | 타입 | 역할 | 비고 |
| --- | --- | --- | --- |
| `Boot` | Single | 앱 시작점. 메타 버전 체크, Remote Config 로드, 자동 로그인 시도 후 다음 씬 결정. | 처리 완료 후 `TitleMenu` 로 전환. |
| `TitleMenu` | Single | 인증 UI, 공지, 서버 선택, 계정 전환 등 허브 화면. | 자동 로그인 성공 시 “이어하기” 강조. |
| `GameWorld` | Single | 마을 중심부와 주변 필드를 포함한 오픈월드. 이동/조우/영토 확장 구현. | Tilemap 64×64 청크 단위 스트리밍. |
| `CombatOverlay` | Additive | 전투 UI. 서버 로그 재생, 속도 조절, 결과 패널 제공. | `GameWorld`를 일시정지한 상태에서 오버레이. |
| `UI.ModalLayer` | Additive | 컨펌 모달, 보상 창 등 전역 팝업. | `Boot` 시점에 로드, 루트 오브젝트는 `DontDestroyOnLoad`. |
| `UI.ToastLayer` | Additive | 토스트, 알림 메시지. | 동일하게 `DontDestroyOnLoad` 로 유지. |
| `UI.LoadingOverlay` | Additive | 씬 전환/네트워크 대기 애니메이션. | Overlay On/Off만 제어. |
| `DebugPanel` | Additive | QA/개발용 툴(위치 이동, 전투 강제 호출). | 개발 빌드 전용. |

> **원칙**  
> - Single 씬은 한 번에 하나만 활성화한다. Boot → TitleMenu → GameWorld 순으로 `SceneManager.LoadScene` 사용.  
> - Additive 오버레이는 Boot에서 미리 로드하고 루트 객체를 `DontDestroyOnLoad` 처리한 뒤, 윈도우 단위로 활성/비활성만 관리한다.  
> - 런타임 서비스(`HttpClient`, `AuthService`, `PlayerDataStore`, `ChunkStreamer`, `AnnouncementManager`)는 Boot 씬에서만 생성하고 이후 씬 전환 간 파괴되지 않도록 한다. `DebugPanel`은 컴파일 플래그/화이트리스트 기반으로만 로드한다.

## 3. 부팅 & 로그인 플로우
1. **Boot**  
   - Splash/Loading 일러스트 + Progress Indicator 노출.  
   - Remote Config/환경설정 로드 → `ApiConfig.SetBaseUrl` 호출.  
   - `/metadata/version`(추가 예정) 호출로 클라이언트 번들과 서버 버전을 비교. 불일치 시 업데이트 유도 팝업.  
   - `AuthService.Initialize()` → `TryAutoLogin()` 실행. *(우선순위: SecureStorage Refresh Token → 미존재 시 Google Play Games Sign-In 시도 → 실패 시 미로그인)*  
   - GlobalServices(GameObject) 생성 → 네트워크/데이터/오버레이 매니저를 `DontDestroyOnLoad`로 유지.  
2. **TitleMenu**  
   - Boot 결과(자동 로그인 성공 여부)를 인자로 받아 초기 상태 결정.  
   - 자동 로그인 성공 시 기본 상태를 “Continue” 강조로 설정하고, 공지 배너/웹뷰를 즉시 로드.  
   - 실패 시 Google Play Games Sign-In UI를 기본 탭으로 노출.  
   - 정보 탭: 서버 목록(멀티 서버 시), 계정 전환(Play Games 계정 전환 플로우 연결), 보안 알림(동시 접속 감지) 등 확장 가능하도록 설계.  
   - 기본 버튼: `Continue / Start Adventure`, `Announcements`, `Options`, `Exit Game`.  
   - 플레이어가 GameWorld에서 로그아웃/타이틀 복귀를 선택하면 TitleMenu 씬을 재로드해 동일 허브를 사용한다.  
3. **에러 처리**  
   - 네트워크 실패 → `UI.ToastLayer`, 재시도 버튼 제공.  
   - 패치 필요 → 업데이트 유도 팝업에서 마켓 이동.  
   - 계정/토큰 오류 → TitleMenu에서 계정 재선택 안내.

부팅 중에는 `UI.LoadingOverlay`로 입력을 차단하고, TitleMenu에서 GameWorld 로 전환할 때도 동일 오버레이를 사용한다.

## 4. GameWorld 구성
- **범위**: 마을 중심부(거점 기능) + 주변 필드 + 요새 진입구까지 하나의 타일맵으로 포함.  
- **허브 구역**  
  - 상점, 술집, 대장간, 여관, 깃발 관리소 등은 각 위치에 `InteractionTrigger`(Collider + Prompt)를 배치.  
  - 플레이어가 반경 내로 들어오면 상호작용 UI(버튼/버블)를 활성화하고, 터치/키 입력 시 대응 UI 패널을 `UI.ModalLayer`에 오픈.  
  - NPC는 MVP에서도 배치하며, 깃발 구매는 상점 NPC에서 이루어진다.  
  - 영웅 배치/파티 편성 UI는 GameWorld 내 HUD 버튼으로 열고, 전투 요청 시 최신 배치를 사용한다.  
  - NPC/건물 내부 진입은 MVP에서 생략(포켓몬 스타일). 필요한 경우 짧은 컷씬/ 패널 전환만 사용.
- **스폰 & 복귀**  
  - TitleMenu에서 GameWorld 진입 직전 서버 `/player/profile` 또는 `/world/state` 응답을 통해 마지막 좌표, 시야, 진행 퀘스트를 수신.  
  - 저장된 좌표가 유효하지 않으면 마을 중심 안전 지점으로 폴백.  
  - 월드 내 캠핑/귀환/사망 처리 시 좌표를 즉시 서버에 업데이트해 다음 로그인 시 재사용.
- **청크 스트리밍**  
  - Tilemap 또는 Addressables Prefab을 64×64 타일 기준 청크로 분할.  
  - `ChunkStreamer`가 플레이어 위치를 기준으로 가시 반경 내 청크만 활성화, 외부 청크는 비활성화 또는 언로드.  
  - 마을 중심 청크는 항상 활성화. 요새/특수 구역은 접근 시 로딩.  
  - 타일 충돌체도 청크 단위로 Enable/Disable.  
- **전투 & 조우**  
  - 이동 입력 시 좌표 이동 이벤트 → 서버 `/world/move` 또는 SignalR 전송 → 조우 발생 시 CombatOverlay 호출.  
  - 전투 중 `GameWorld` 씬의 시간 스케일을 0 또는 입력 금지 상태로 전환.  
  - 전투 종료 후 서버가 내려준 위치/상태를 갱신하고 보상 패널 노출.  
- **영토 & 깃발**  
  - 깃발 설치 모드 토글 → 설치 가능한 위치 하이라이트 → 설치 요청 시 서버 `/territory/flag`.  
  - 소유 영토는 타일맵 레이어/Shader로 색상 강조.  
  - 계급 변화 시 HUD 갱신 및 토스트 알림.

## 5. CombatOverlay 플로우
1. `CombatController.Open(encounterPayload)` 호출 → CombatOverlay 씬 Additive 로드.  
2. 배치 단계: 아군 3×3 슬롯 UI + 도망/전투 시작 버튼(`docs/01-systems/combat.md:24`).  
3. 서버 `/combat/start` 호출 → 로그 수신 → Timeline 재생(속도 1×/2×/4×/8×, 스킵).  
4. 전투 종료 → 결과 창(UI.ModalLayer)에서 경험치/골드/드랍 표시 → `Confirm` 시 씬 언로드.  
5. 실패/무승부 시 강제 귀환 로직 호출(마을 중심 좌표 Teleport).  
6. CombatOverlay 언로드 후 `GameWorld` 시간/입력 재개.

UI는 Canvas 기반, 전투 기물은 2D Sprite 또는 UI Image로 표현해 초기 구현 부담을 줄인다.

## 6. 공통 오버레이 & 서비스
- `UI.LoadingOverlay`: 네트워크 대기, 씬 전환 시 활성화/비활성화.  
- `UI.ToastLayer`: 짧은 메시지 큐 처리. 자동 로그인, 깃발 설치 결과 등 공통 피드백.  
- `UI.ModalLayer`: 컨펌/보상/상점/술집/대장간/파티 편성 패널을 모두 여기서 열고 닫는다.  
- `DebugPanel`: 개발 빌드에서만 Additive 로드해 청크 강제 로드, 전투 스폰, 플레이어 위치 이동 기능 제공. 릴리스 빌드에서는 컴파일 상수 혹은 서버 제공 화이트리스트를 통해 접근을 제한한다.

Boot 씬에서 싱글턴 GameObject(`GlobalServices`)를 생성해 HttpClient, AuthService, PlayerDataStore, ChunkStreamer, AudioManager 등을 초기화하고 `DontDestroyOnLoad`로 유지한다.

## 7. 데이터 & 의존성
- **메타데이터**: Boot에서 서버 버전 확인 → 필요 시 로컬 캐시 갱신(향후 Addressables 교체).  
- **플레이어 상태**: `PlayerDataStore`가 파티, 인벤토리, 영토, 진행 퀘스트를 보관. GameWorld/Combat UI는 모두 동일 스토어를 참조. 로그인 성공 후 DB의 마지막 좌표/채널을 로드해 스폰 위치를 결정.  
- **네트워크**: HttpClient 싱글턴 + SignalR(WebSocket) 준비. 이동 이벤트는 WebSocket, 나머지는 REST API 기준(`docs/01-systems/world-exploration.md:29`). WebSocket 통해 실시간 공지, 월드 이벤트, 친구 접속 알림 등 푸시를 확장할 수 있도록 메시지 루팅 설계.  
- **저장소**: SecureStorage에 RefreshToken, UserId, Username, 마지막 월드 좌표를 저장해 자동 로그인을 지원.

## 8. QA & 테스트 플로우
- `Boot` → TitleMenu(자동 로그인 성공 시 Continue, 실패 시 로그인) → `GameWorld` → 이동/조우/전투 → 보상 → 깃발 설치 순으로 일일 테스트.  
- QA 전용 메뉴(디버그 패널)에서 드랍 강제, 전투 로그 스킵, 영토 리셋 기능을 제공해 반복 검증 시간 단축.  
- 청크 스트리밍 테스트: 에디터에서 Gizmo로 로드된 청크 목록을 표시하고, 플레이어 이동 시 KPI(Loaded Tiles, Draw Calls)를 HUD에 표시.

## 9. Stub / 남은 과제
1. `/metadata/version` 및 클라이언트 버전 캐시 구조 정의.  
2. ChunkStreamer 구현 상세(로드 반경, 언로드 기준, Addressables vs SubScene) 결정.  
3. CombatOverlay UI 레이아웃 와이어프레임 작성 및 애니메이션 정책 확정.  
4. 월드 내 상호작용 Prompt UX(키 가이드, 모바일 터치 대응) 설계.  
5. DebugPanel 권한 제어 및 릴리스 빌드에서 완전 제거 방법 명세.  
6. Google Play Games 계정 전환 UX(로그아웃→재로그인) 가이드 문서화.

---
**최종 수정**: 2025-10-26  
**상태**: 초안  
**작성자**: SangHyeok  
