# HUSK — Prototype V2 Manager Status

## Overall Status

ACTIVE

## Current Phase

V2 Phase 1 — Harbor + Salvage Loop (READY; implementation not started)

## Phase Status

| Phase | Name | Status |
|---|---|---|
| V2 Phase 1 | Harbor + Salvage Loop | READY |
| V2 Phase 2 | Processing + Local Storage | TODO |
| V2 Phase 3 | Power Capacity + Demand | TODO |
| V2 Phase 4 | Integrated V2 Playtest | TODO |

## Last Completed Prototype

Prototype V1 — Module Network & Planning: COMPLETE. Preserve V1 spec/checklist/status as historical acceptance evidence.

## Current Phase Goal

Generalize the existing small-boat facility into a shared Harbor and implement the minimum end-to-end Salvage Boat loop producing Scrap Metal and Scrap Wood, while preserving Fishing Boat behavior.

Stop after Phase 1 implementation/report for Manager review and user playtest. Do not start Phase 2 without explicit authorization.

## Current Blockers

None known for Phase 1.

Exact salvage amounts, boat timing/build cost and final labels are provisional/configurable unless already safely reusable from the current implementation.

## Confirmed V2 Rules

- One shared Harbor supports small boats; do not create resource-specific Fishing/Salvage ports.
- Fishing Boat and Salvage Boat use the same Harbor workflow.
- Salvage Boat returns Scrap Metal + Scrap Wood.
- Processing later converts salvage into usable construction materials.
- Local storage is a buffer immediately available to each consumer/factory.
- Local-storage UI uses progress bars plus up/down/neutral trend.
- House/Citizen V2 feedback is only Food and Water local-storage bars/trends; no total Satisfaction/Happiness yet.
- Factory V2 feedback includes Productivity and per-input local-storage bars/trends.
- Power is shared capacity/demand, not a stock item.
- Enabled powered buildings consume rated kW; disabled buildings consume 0 kW.
- `PowerEfficiency = min(1, TotalPowerCapacity / TotalPowerDemand)` with zero-demand = 100%.
- Factory productivity is determined by the most deficient required factor: power or required material input.
- Lower productivity increases production interval; 0 productivity stops progress safely.

## Out of Scope V2

- citizen Satisfaction/Happiness score;
- Calories/Protein/Vitamins, Diet Diversity or class food expectations;
- luxury-food systems;
- mortality/migration/health/security/politics;
- batteries/voltage/distance loss/power priorities/advanced grid simulation;
- detailed logistics vehicle simulation;
- final economy balance or production-quality art;
- unrelated future-proof frameworks.

## Source Priority

1. User's latest explicit decision.
2. `AGENTS.md`.
3. `docs/prototype-v2-salvage-material-power.md`.
4. `docs/manager-checklist-v2.md`.
5. This file.

## Phase 1 Task Boundary

Implement only:

- shared Harbor identity/workflow;
- Fishing Boat preserved through Harbor;
- Salvage Boat build/dispatch/return flow;
- Scrap Metal + Scrap Wood concrete cargo/resource accounting;
- minimum UI/state needed to distinguish boat roles;
- tests/validation for the above.

Do NOT implement Phase 2 local-storage/productivity UI or Phase 3 power-demand mechanics yet, except minimal compatibility strictly required to keep current code compiling.

## Manager Acceptance Rule

Phase 1 becomes DONE only after independent Manager review of source/diff/metas plus appropriate Unity compile/Console/tests/runtime behavior. Implementer report is evidence, not acceptance.
