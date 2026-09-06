# HUSK — Prototype V0 Manager Status

## Overall Status

ACTIVE

## Current Phase

Phase 1 — Prototype Shell & Resource State

## Phase Status

| Phase | Name | Status |
|---|---|---|
| Phase 0 | Technical Foundation | DONE |
| Phase 1 | Prototype Shell & Resource State | TODO |
| Phase 2 | Water Plant Requirement & Wood Bottleneck | TODO |
| Phase 3 | Broken Recycler & Repair | TODO |
| Phase 4 | Recycle Starter Material Into Wood | TODO |
| Phase 5 | Build Water Plant & Begin Water Production | TODO |
| Phase 6 | Starter Material Exhaustion & Collection Introduction | TODO |
| Phase 7 | Full Fresh-Run Validation | TODO |

## Last Completed Phase

Phase 0 — Technical Foundation

Validated foundation:

- Unity 6000.5.7f1
- URP 17.5.0
- Git/GitHub origin/main operational
- AGENTS.md present
- Prototype V0 specification present
- Official Unity CLI operational
- Official Unity Pipeline operational
- Codex Unity MCP operational
- Live Editor communication PASS
- Scene read PASS
- Console read PASS
- Enter Play Mode PASS
- Exit Play Mode PASS
- Clean Git checkpoint established before gameplay implementation

## Current Phase Goal

Phase 1 must create the smallest playable shell needed to represent:

- resource state;
- correct fresh-start resources;
- initial Water problem;
- enough developer/player feedback to inspect that state in Play Mode.

Phase 1 does NOT own:

- Water Plant requirement/build behavior;
- Recycler repair behavior;
- Recycler conversion;
- Collection.

Those belong to later phases according to `docs/manager-checklist.md`.

## Current Blockers

None.

## Pending User Decisions

None currently blocking Phase 1.

## Provisional Gameplay Values

The following values are intentionally NOT canon yet:

- Starting Wood
- Starting Iron
- Recycler repair resource
- Recycler repair cost
- Recycler conversion ratio
- Water Plant Wood cost
- Water Plant Iron cost
- Water production rate
- Water consumption rate
- Collection yield
- Collection timing
- Construction timing

Implementer may use temporary values only when required by the active phase.

Any temporary value must be:

- configurable;
- documented as provisional;
- easy to change;
- consistent with prototype constraints.

## Manager Operating State

Next action:

1. Inspect current repository and Unity project.
2. Read Phase 1 acceptance criteria from `docs/manager-checklist.md`.
3. Spawn the Husk Implementer once the Manager Loop agent configuration exists.
4. Assign ONLY Phase 1.
5. Review evidence before marking Phase 1 PASS.

## Status Update Rules

Manager updates this file only when operational state changes.

Examples:

- phase begins;
- phase passes;
- phase enters rework;
- blocker appears;
- blocker is resolved;
- user makes a design decision relevant to the current phase.

When a phase begins:

- Current Phase remains that phase.
- Phase Status may become `IN_PROGRESS`.

When a phase fails review but remains workable:

- mark it `REWORK`.

When all acceptance criteria PASS:

- mark phase `DONE`;
- update Last Completed Phase;
- set Current Phase to the next phase;
- keep the next phase `TODO` until implementation begins.

If progress is blocked by a user decision:

- set Overall Status to `BLOCKED`;
- record the exact blocker under Current Blockers;
- do not continue implementation.

When the blocker is resolved:

- restore Overall Status to `ACTIVE`;
- record the resolved decision if it affects prototype implementation.

## Source of Truth Priority

If documents appear inconsistent, use this priority:

1. User's explicit latest decision.
2. `AGENTS.md` for project/engineering rules.
3. `docs/prototype-v0.md` for design intent and prototype scope.
4. `docs/manager-checklist.md` for phase acceptance criteria.
5. `docs/manager-status.md` for current operational progress.

`manager-status.md` must never silently override design intent or acceptance criteria.

## Completion State

Overall Status may become `COMPLETE` only when:

- Phase 0 through Phase 7 are all DONE;
- final fresh-run validation has PASS evidence;
- Unity Console has no implementation-caused errors;
- Manager confirms no unresolved blocker remains.
