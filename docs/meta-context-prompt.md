# Castle Lord Tycoon – Context Prompt (2025-10-24)

Use this prompt to rehydrate the current project state before continuing work on Castle Lord Tycoon. Paste it at the start of a new session so the assistant understands the existing implementation, data assets, and next actions.

```
Project: Castle Lord Tycoon (mobile-first mid-core idle RPG with territory expansion loop)
Current Goal: MVP gameplay loop (필드 탐험 → 전투 → 보상 → 깃발 설치 → 거점 관리)

Repo facts:
- Primary docs live under docs/00-overview..04-technical.
- MVP 설계: 00-overview/game-concept.md, 00-overview/game-flow.md.
- Systems: 탐험, 조우, 깃발, 상점, 술집, 대장간, 영웅/장비/보석, 계급, 경제 등 전면 정리됨.
- Data/UI/Technical README는 MVP 범위에 맞춰 새로 작성됨(구버전 파일 삭제 완료).

Core Systems (MVP):
- World Exploration: 타일 이동(WebSocket 이벤트) → 서버 인메모리 저장 → 5초 DB flush → 10% 전투 판정.
- Encounter: 전투 조우만 활성화(이벤트/자원 Stub).
- Territory: 3×3~9×9 깃발, 겹침 허용, 제거 시 아이템 반환. 영토는 청크 기반 비트맵으로 저장.
- Progression: 영토 수치로 10단계 계급 달성(성주~황제). 누적 보너스 적용.
- Shop: 장비(마을별), 공용(소비재), 깃발 탭으로 구성. 6시간 갱신. 옵션 변경권 판매 추가.
- Tavern: 일반/고급/최고급 모집, 영웅 리스트, 파티 프리셋.
- Forge: 강화/분해/보석 각인/옵션 변경/장비 합성.
- Economy: 상점/대장간/술집 중심 루프만 문서화. 교환/제작은 Stub.

Data (02-data):
- CSV는 MVP에 필요한 파일만 생성 예정 (enemy_encounters, shop pools, forge rules, tavern tiers, flag_templates, rank_requirements 등).
- 미구현 기능 관련 CSV는 미작성 상태 유지.

UI (03-ui):
- 필수 화면: 필드 HUD, 상점(3탭), 술집, 대장간, 전투 로그/리플레이 등.
- 컨펌 모달(강화/합성/깃발 설치) 규칙 명시.

Technical (04-technical):
- WebSocket 기반 이동/조우 처리. 단일 서버 기준 인메모리 + 5초 DB flush. 추후 확장 시 Redis 검토.
- 메타데이터 파이프라인/CI는 Stub. CSV 버전 관리와 동기화 필요.

TODO (short-term):
1. MVP CSV 작성 (조우, 상점, 대장간, 술집, 깃발/계급).  
2. 핵심 UI 와이어프레임 정리(필드 HUD, 상점, 술집, 대장간).  
3. 이동/조우/상점/대장간 API 세부 스펙 작성 (client-server-contract).  
4. 클라이언트·서버 MVP 구현 시작.  
5. 이후 테스트 피드백 기반 밸런스 조정 및 Stub 항목 확장.

Security/Infra reminders:
- 외부 스프레드시트/CI 연동은 추후 도입. 현재는 로컬 CSV 기준.  
- 민감한 키/토큰은 repo에 포함하지 않는다.
```

---
**최종 수정**: 2025-10-24  
**상태**: 최신  
**작성자**: SangHyeok
