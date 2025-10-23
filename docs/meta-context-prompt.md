# Castle Lord Tycoon – Context Prompt (2025-10-21)

Use this prompt to rehydrate the current project state before continuing work on Castle Lord Tycoon. Paste it at the start of a new session so the assistant understands the existing implementation, data assets, and next actions.

```
Project: Castle Lord Tycoon (mobile-first mid-core idle RPG with territory expansion loop)
Repo facts:
- Primary docs live under docs/00-overview..04-technical (Korean).
- MVP focus = docs/00-overview/game-concept.md Phase 1 checklist.

Data & metadata:
- Static gameplay tables live in Google Sheet ID 1DoEdD_b9IO6eV2FZQviCe7YBVWTn3gcGFuXNrzozDX4 (imported from local CSV seeds in data/csv).
- CSV directory mirrors future tools/build-meta input (battle_rules, hero templates/growth, equipment templates/affixes, territory & ranks, economy, encounters, world tiles, settlement services, etc.).
- Plan: build CI job that pulls sheets -> CSV -> JSON -> updates meta/manifest.json (hash + meta_version). Service-account access required; keep credentials out of repo.

Systems status:
- Combat MVP rules finalized (3x3 auto battles, 10 round limit, log retention). No open questions remain in combat.md.
- Equipment doc keeps post-MVP open items (set effects, legendaries) flagged as future work; reinforcement/merge is intentionally stubbed.
- Progression doc: rank table drafted (castellan → marquis) with decay enabled; plan to enforce via metadata table.
- UI doc is mobile-first (FHD baseline, touch UX). Desktop/PC deferred.
- Metadata pipeline doc outlines Sheet->JSON flow; README notes MVP-first before schema automation.

Next actions:
1. Finalize spreadsheet schemas (owners, validation rules, column descriptions) based on CSV seeds.
2. Implement tools/build-meta: download Sheet tabs, validate, emit JSON + manifest, integrate hashes.
3. Wire CI/CD so meta changes auto-bump meta_version and provide artifacts for client/server.
4. Lock down Google Sheet sharing (service account only) to avoid leaking balance data.
5. When reinforcement/advanced equipment features enter MVP scope, author separate spec before extending tables.
```

---
**최종 수정**: 2025-10-21  
**상태**: 참고용 프롬프트  
**작성자**: Codex Assistant  
