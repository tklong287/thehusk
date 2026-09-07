# HUSK — Prototype V2 Manager Status

## Overall Status

ACTIVE — FULL CONTINUOUS PASS AUTHORIZED

## Current Work

Prototype V2 — Salvage, Material & Power: implement Milestones A–D continuously, test/fix as work proceeds, then stop for one final Manager/user acceptance review.

There are no user-playtest stop gates between milestones.

## Milestone Status

| Milestone | Name | Status |
|---|---|---|
| A | Shared Harbor + Salvage | READY |
| B | Processing + Local Storage | READY AFTER A |
| C | Power Capacity + Demand | READY AFTER B |
| D | Integrated V2 Hardening | READY AFTER C |

Milestones are implementation order only. Implementer is explicitly authorized to proceed A → B → C → D in one task without waiting for phase progression approval, provided it remains inside the active V2 specification.

## Last Completed Prototype

Prototype V1 — Module Network & Planning: COMPLETE. Preserve V1 spec/checklist/status as historical acceptance evidence and regression baseline.

## Current Goal

Deliver one playable, integrated V2 implementation that proves:

- one Harbor serves Fishing Boat + Salvage Boat;
- Salvage Boat returns Scrap Metal + Scrap Wood;
- salvage is processed into usable construction material;
- factories and House/Citizen inputs use local buffers;
- progress bars show local buffer ratio and arrows show rising/falling/stable trend;
- factory Productivity reflects the worst required material/power factor;
- enabled powered buildings add rated kW demand, disabled buildings add zero;
- insufficient electric capacity slows powered factories predictably;
- the player can relieve overload by switching a powered factory off;
- V0/V1 regression behavior remains intact.

## Continuous Work Rule

For this V2 task:

1. Implement Milestone A.
2. Run targeted tests/Unity validation.
3. Fix implementation-caused failures.
4. Continue directly to Milestone B.
5. Repeat through C and D.
6. Run final full relevant validation.
7. Return one complete implementation report.
8. Stop for Manager acceptance.

A milestone failure is not a user stop point. Fix it and continue unless there is a genuine design blocker that cannot be resolved from existing confirmed rules.

## Current Blockers

None known.

If an implementation detail is not gameplay-significant, choose the smallest reversible option and continue. If a missing decision would materially change player experience, report it as a blocker rather than inventing canon.

## Confirmed V2 Rules

- One shared Harbor supports small boats; no separate Fishing/Salvage ports.
- Fishing Boat and Salvage Boat share Harbor workflow/foundation.
- Salvage Boat returns Scrap Metal + Scrap Wood.
- Processing converts salvage into existing compatible usable construction materials; avoid broad renames.
- Local storage is a consumer/factory buffer, distinct from city/global stock.
- Progress bar = local storage ratio.
- Arrow = short-term local storage trend: Up/Down/Neutral.
- House/Citizen V2 UI shows Food + Water local-storage bars/trends only; no total Satisfaction/Happiness.
- Factory UI shows Productivity + required material local bars/trends + enough power information to diagnose bottleneck.
- `InputSatisfaction = LocalRatio` for V2.
- Factory material bottleneck = minimum required input satisfaction.
- Power is capacity/demand, not stock.
- Powered building enabled => rated kW demand; disabled => 0 kW.
- No separate idle-power state; an enabled building still counts demand while material-starved.
- Existing Electric network connectivity remains authoritative; disconnected consumers cannot use remote capacity.
- `PowerEfficiency = min(1, Capacity / Demand)`, zero demand => 1.
- `FactoryProductivity = min(MaterialSatisfaction, PowerEfficiency)` while retaining V1 operational gates.
- `ActualInterval = DefaultInterval / FactoryProductivity`; zero productivity stops safely.
- Exact rates/capacities/timings/conversion ratios/rated kW remain configurable/provisional unless already confirmed.

## Out of Scope V2

- citizen total Satisfaction/Happiness;
- Calories/Protein/Vitamins;
- Diet Diversity;
- class food expectations;
- luxury food;
- mortality/migration/health/security/politics;
- detailed logistics vehicles;
- battery/voltage/distance loss/power-priority automation;
- final economy balance/final art;
- unrelated future-proof frameworks.

## Source Priority

1. User's latest explicit decision.
2. `AGENTS.md`.
3. `docs/prototype-v2-salvage-material-power.md`.
4. `docs/manager-checklist-v2.md`.
5. This file.

## Final Acceptance Rule

Implementer may complete the entire V2 scope continuously but does not declare V2 PASS/DONE and does not update Manager checklist/status.

Manager accepts V2 only after independent review of source/diff/metas plus Unity compilation, Console, relevant automated tests and integrated Play Mode behavior.
