# HUSK — Prototype V0 Manager Status

## Overall Status

ACTIVE

## Current Phase

Phase 2 — Water Plant Requirement & Wood Bottleneck

## Phase Status

| Phase | Name | Status |
|---|---|---|
| Phase 0 | Technical Foundation | DONE |
| Phase 1 | Prototype Shell & Resource State | DONE |
| Phase 2 | Water Plant Requirement & Wood Bottleneck | TODO |
| Phase 3 | Broken Recycler & Repair | TODO |
| Phase 4 | Recycle Starter Material Into Wood | TODO |
| Phase 5 | Build Water Plant & Begin Water Production | TODO |
| Phase 6 | Starter Material Exhaustion & Collection Introduction | TODO |
| Phase 7 | Full Fresh-Run Validation | TODO |

## Last Completed Phase

Phase 1 — Prototype Shell & Resource State

Manager validation on 2026-09-06:

- All 14 Phase 1 acceptance criteria reviewed and passed.
- Entry point: `Game/Husk/Assets/Scenes/SampleScene.unity`, root `Husk Prototype`.
- `ResourceState` is independent of UI; `Get`, `Add`, and `TryRemove` validate resource operations.
- Unity MCP compilation result: completed, failed=false, errors=[].
- Unity EditMode Test Runner: 6/6 resource API tests passed; Manager inspected the actual MCP test result.
- Manager independently entered Play Mode twice: Food=0, Water=5, Recyclable Material=10, Wood=0, Iron=5 on both fresh starts.
- Manager added Wood=7 and removed Water=5; HUD updated to Wood=7/Water=0 and rejected a further Water debit. Re-entering Play Mode restored the fresh state.
- Composited Game view screenshot confirmed all five resources and the Water need feedback were visible.
- Wood=0 and Iron=5 are provisional Inspector configuration, not design canon. Water need uses a configurable message; no consumption timer or citizen simulation.
- No Water Plant requirements/construction, Recycler behavior, or Collection implemented.
- Console retained two Unity Pipeline main-thread timeout errors from Editor handoff; no implementation-caused errors or warnings. Communication recovered after restoring the Unity window; no Console clearing.
- Final Editor ready, Play Mode stopped, SampleScene saved and not dirty.
- Unity scene save migrated template Camera/Light/Lightmap serialization; no intentional lighting or camera tuning.

Phase 0 foundation retained:

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

Phase 2 is the next phase: communicate Water Plant requirements and the fresh-start Wood bottleneck, according to `docs/manager-checklist.md`.

Phase 2 remains TODO and has not begun. This session stops after the Phase 1 checkpoint for user playtesting.

## Current Blockers

None.

## Pending User Decisions

None currently blocking the next phase. Await user playtest feedback and an explicit command before starting Phase 2.

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

1. User playtests Phase 1 in `Assets/Scenes/SampleScene.unity`.
2. Await user feedback and an explicit next command.
3. Do not start Phase 2 automatically.

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
