# TitleMenu Scene Spec (MVP)

## 1. 목적
- 자동 로그인 성공 여부와 무관하게 플레이어가 **공지, 서버 상태, 계정 관리**를 확인한 뒤 월드로 진입하도록 돕는 허브 화면을 정의한다.
- Google Play Games 기반 인증만 지원하는 MVP 기준을 명확히 하고, 추후 다중 서버·동시 접속 처리·추가 공지 채널을 확장할 수 있는 UI 구조를 마련한다.

## 2. 진입/이탈 플로우
| 상황 | 동작 |
| --- | --- |
| Boot → TitleMenu | Boot에서 Remote Config, 메타 버전, 자동 로그인을 모두 처리한 뒤 결과(자동 로그인 성공 여부, 필요한 공지 ID)를 파라미터로 전달한다. |
| GameWorld → TitleMenu | 플레이어가 메뉴에서 `로그아웃` 또는 `타이틀로 돌아가기`를 선택하면 GameWorld 상태를 저장하고 TitleMenu를 재로드한다. |
| TitleMenu → GameWorld | `Continue / Start Adventure` 버튼 클릭 → `/player/profile`(마지막 좌표) 요청 → 성공 시 GameWorld 로딩. 실패 시 안전 지점 폴백. |
| 종료 선택 | `Exit Game` 클릭 시 앱 종료 확인 모달 → 승인 시 `Application.Quit()` 또는 플랫폼 종료 루틴 호출. |

## 3. UI 레이아웃
| 구역 | 요소 | 설명 |
| --- | --- | --- |
| 헤더 | 게임 로고, 서버 시간, 버전 표시 | 버전은 서버 메타/클라이언트 빌드 번호 동시 표기. |
| 메인 버튼 영역 | `Continue / Start Adventure`, `Announcements`, `Options`, `Exit Game` | 자동 로그인 성공 시 `Continue`에 기본 포커스. |
| 서브 탭(우측 또는 하단) | `Server Status`, `Account`, `Support` | 초기에는 단일 탭만 활성화, Stub로 구조만 유지. |
| 배너 영역 | 최신 공지/이벤트 배너, 클릭 시 WebView | 공지 API에서 받은 우선순위 배너 1~3개 회전. |
| 계정 패널 | Google Play Games 프로필, `Sign Out`, `Switch Account` | `Sign Out` → RefreshToken 삭제 → 다시 Sign-In 요청. |
| 메시지 영역 | 토스트/모달 연결 상태 | Boot에서 전달된 오류/경고 메시지를 노출. |

## 4. 상호작용 플로우
### 4.1 Continue / Start Adventure
1. 버튼 클릭 → `UI.LoadingOverlay` 활성화.  
2. `/player/profile` 요청으로 마지막 좌표/채널 수신.  
3. `/world/state`(추가 예정)로 필수 월드 캐시 데이터 요청.  
4. 응답 성공 → GameWorld 로드 → LoadingOverlay 비활성화.  
5. 실패 → Toast + 다시 TitleMenu. (권한 문제일 경우 `TryAutoLogin` 재시도)

### 4.2 Google Play Games Sign-In
- Boot 단계에서 RefreshToken이 없거나 만료되면 `Sign In with Google Play Games` 버튼이 메인으로 노출된다.  
- 성공 시 AccessToken/RefreshToken 저장 → UI 상태를 `Continue` 강조로 업데이트.  
- `Switch Account` 시 `PlayGamesPlatform.Instance.SignOut()` 후 재로그인 UI 호출. 성공 시 기존 세션 정리.

### 4.3 Announcements/WebView
- `Announcements` 버튼: 공지 리스트 모달 → 항목 선택 시 WebView 오버레이 열기.  
- 강제 확인 공지는 Boot에서 전달된 ID 기준으로 TitleMenu 진입 직후 모달로 띄운다(닫아야만 다른 동작 가능).

### 4.4 Server Status / Queue (Stub)
- 다중 서버 도입 시 `Server Status` 탭에 서버 목록, 인원, 대기열 표기.  
- 서버 선택 변경 시 `/auth/select-server` 호출 후 `Continue` 버튼 상태 갱신.  
- 현재는 Stub로 UI 골조만 유지.

## 5. 데이터 의존성
| API/데이터 | 용도 |
| --- | --- |
| `/metadata/version` | Boot에서 호출, TitleMenu 헤더에 표시. |
| `/announcements/latest` | 공지 배너/리스트. |
| `/auth/refresh` | Boot 자동 로그인. 실패 시 TitleMenu에서 Sign-In 노출. |
| Google Play Games SDK | Sign-In/Out/계정 전환. |
| `/player/profile` | 마지막 좌표, 계급, 재화 등 GameWorld 진입 전 준비. |
| `/world/state` (TBD) | 월드 기본 데이터 캐시. |

## 6. 오류 및 엣지 케이스
- 메타 버전 불일치 → 업데이트 유도 모달 → 확인 시 앱 종료 또는 마켓 이동.  
- 동시 접속/중복 계정: 서버에서 409 반환 시 경고 모달 → `계정 강제 종료` 또는 `취소` 선택. 강제 종료 선택 시 기존 세션 종료 API 호출.  
- Google 인증 실패: 오류 코드에 따라 재시도, 계정 선택 모달 노출, 네트워크 오류 토스트.  
- 공지 WebView 로딩 실패: 재시도 버튼 제공, 로그 전송.

## 7. Stub / 향후 과제
1. 다중 서버 선택/대기열 UI 상세 설계.  
2. 고객센터/문의 링크를 포함한 Support 탭 구성.  
3. A/B 테스트 배너 또는 개인화 공지 노출 정책.  
4. 장기 세션/푸시 연결 상태 표시 여부.  
5. Localization(다국어) 문자열 테이블 정의.

---
**최종 수정**: 2025-10-26  
**상태**: 초안  
**작성자**: SangHyeok  
