# UI Documentation (MVP)

## 범위
- 시스템 문서에서 결정된 **최소 UI 흐름**만 정의한다.
- 미구현 기능(교환, 이벤트 조우 등) 관련 UI는 작성하지 않는다.

## 우선 정리 대상
| 문서 | 화면 | 주요 요소 | 관련 시스템 |
| --- | --- | --- | --- |
| `scene-flow.md` | 전체 흐름 | Boot → TitleMenu → GameWorld → CombatOverlay 씬 전환 | core |
| `title-menu.md` | 타이틀 메뉴 | Google Play Games 로그인, 공지/웹뷰, 서버/계정 관리 | auth, announcements |
| `gameworld-hud.md` | 월드 HUD | 이동/깃발 모드, NPC 인터랙션, 청크 스트리밍 | world-exploration, territory |
| `combat-overlay.md` | 전투 오버레이 | 3×3 배치, 로그 재생, 보상 창 | combat |
| (미작성) | 상점 UI | 장비/공용/깃발 탭, 품절 처리, 계급 표시 | shop |
| (미작성) | 술집 UI | 영웅 모집 탭, 무료 타이머, 프리셋 관리 | tavern |
| (미작성) | 대장간 UI | 강화/분해/보석/옵션/합성 탭, 컨펌 모달 | forge |

## 작성 가이드
1. **구조 우선**: 화면 레이아웃, 주요 위젯, 상호작용 흐름을 우선 정의한다.  
2. **컨펌 모달**: 강화/합성/깃발 설치 등 자원 소모 행위에는 컨펌 모달을 요구한다.  
3. **상태 표시**: 계급, 보유 깃발, 무료 모집 가능 여부 등 주요 상태는 UI 상단에 표시한다.  
4. **Stub**: 아직 도입되지 않은 UI 플로우는 Stub로 명시하고, 별도 문서는 작성하지 않는다.

---
**최종 수정**: 2025-10-26  
**상태**: MVP 분해 진행 중  
**작성자**: SangHyeok
