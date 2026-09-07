# HUSK — Prototype V2 Manager Checklist

## Manager Rules

Source priority for V2:

1. User's latest explicit decision.
2. `AGENTS.md`.
3. `docs/prototype-v2-salvage-material-power.md`.
4. This checklist.
5. `docs/manager-status-v2.md`.

V1 docs are completed history. Do not rewrite V1 acceptance evidence.

Implementer does not own phase progression or acceptance. Manager reviews source/diff, Unity compile/Console/tests/runtime behavior, then updates status/checklist.

Stop after each phase for user playtest. Do not implement the next phase merely because the previous one is complete.

---

## Phase 1 — Harbor + Salvage Loop

### Acceptance

- [ ] Existing fishing-only Harbor presentation/logic is generalized to one shared small-boat Harbor without creating a separate Salvage Port.
- [ ] Fishing Boat still works through the Harbor and continues its existing loop unless intentionally changed by an explicitly configurable V2 value.
- [ ] Salvage Boat can be built/managed through the same Harbor workflow.
- [ ] Salvage Boat returns both Scrap Metal and Scrap Wood as concrete resources.
- [ ] Multiple boats remain independent and do not overwrite each other's state/cargo/timers.
- [ ] Resource accounting is deterministic and does not duplicate/drop cargo on normal dispatch/return cycles.
- [ ] Harbor UI/state makes Fishing vs Salvage role legible enough for playtest.
- [ ] No Phase 2 local-storage/productivity system is implemented beyond minimum compatibility needed for Phase 1.
- [ ] No Phase 3 power-demand system is implemented beyond existing V1 compatibility.
- [ ] Relevant EditMode/runtime tests pass; V0/V1 regressions remain green.
- [ ] Unity compiles with no implementation-caused Console errors.
- [ ] Diff contains no unrelated/generated/IDE files and preserves `.meta` files.

### Playtest questions

- Can the user understand that one Harbor supports multiple small-boat roles?
- Is choosing/building a Fishing Boat vs Salvage Boat clear?
- Does salvage visibly return both scrap resource types?

---

## Phase 2 — Processing + Local Storage

### Acceptance

- [ ] Scrap Metal can be processed into the existing usable metal resource without an unnecessary broad resource rename.
- [ ] Scrap Wood can be processed into usable construction wood; presentation may say Lumber while preserving existing code compatibility where appropriate.
- [ ] Factory required material inputs have local storage/buffer state.
- [ ] Factory material input UI shows progress bars based on local-storage ratio.
- [ ] Each displayed local-storage input shows a stable rising/falling/neutral trend indicator.
- [ ] Factory UI shows current Productivity %.
- [ ] Material shortage reduces factory productivity using the smallest transparent mapping consistent with local-storage availability.
- [ ] Multiple required material inputs use the most deficient required input as the material bottleneck.
- [ ] Productivity 0 safely stops progress without divide-by-zero/NaN behavior.
- [ ] House/Citizen UI shows Food and Water local-storage bars + trends.
- [ ] V2 does NOT add citizen total Satisfaction/Happiness/nutrition/class systems.
- [ ] Local storage works as a buffer: global shortage does not instantly erase remaining local stock.
- [ ] Relevant tests cover fill/drain/trend/bottleneck/zero-state behavior.
- [ ] Unity compile/Console/runtime validation passes with no implementation-caused errors.

### Playtest questions

- Can the user spot a falling local buffer before the factory stops?
- Can the user identify which material is limiting a factory?
- Are Food/Water input trends readable without introducing citizen happiness yet?

---

## Phase 3 — Power Capacity + Demand

### Acceptance

- [ ] Power remains capacity, not a concrete stock item.
- [ ] Powered buildings have configurable rated kW demand.
- [ ] Enabled building contributes rated demand; disabled building contributes zero demand.
- [ ] V2 does not add a separate idle-power state: enabled demand remains counted even when material input is low.
- [ ] `TotalPowerDemand` is the sum of enabled powered-building rated demand.
- [ ] `PowerEfficiency = min(1, TotalPowerCapacity / TotalPowerDemand)` with explicit zero-demand handling.
- [ ] Factory productivity uses the minimum of PowerEfficiency and required material-input satisfaction values.
- [ ] `ActualInterval = DefaultInterval / FactoryProductivity` or an implementation mathematically equivalent to the confirmed behavior.
- [ ] At 100% productivity default interval is unchanged; lower productivity lengthens interval predictably.
- [ ] Factory UI provides enough power information to distinguish power bottleneck from material bottleneck.
- [ ] User can switch a powered building off/on and see city demand/productivity respond predictably.
- [ ] No batteries, voltage, distance loss, priority automation, per-cycle kWh stock or advanced grid simulation is added.
- [ ] Relevant formula/state tests and runtime overload/recovery cases pass.
- [ ] Unity compile/Console validation passes with no implementation-caused errors.

### Playtest questions

- Is overload understandable without opening debug tooling?
- Does switching off a building relieve overload in an obvious way?
- Does the factory clearly show whether power or input is the current bottleneck?

---

## Phase 4 — Integrated V2 Playtest

### Acceptance

- [ ] Fresh V2 scenario exposes Fishing and Salvage as competing/meaningful small-boat choices.
- [ ] Salvage → local storage → processing → usable material loop works end to end.
- [ ] Local-storage trend indicators remain readable during bursty boat deliveries/consumption.
- [ ] Factory productivity correctly follows the most deficient current requirement.
- [ ] Power overload and recovery integrate with material shortages without contradictory UI states.
- [ ] House Food/Water local-storage bars remain read-only informational V2 feedback; no accidental happiness/class logic was introduced.
- [ ] Existing Module/Pipeline/House/Population behavior needed from V1 remains functional.
- [ ] Runtime scenario can demonstrate at least: healthy state, falling input buffer, material bottleneck, power bottleneck, building disable/recovery.
- [ ] Full relevant EditMode suite/regressions pass.
- [ ] Unity compilation and Console are clean of implementation-caused errors.
- [ ] Final diff review finds no unrelated feature expansion or generated files.

### V2 final success questions

- [ ] Player can explain Harbor + boat roles.
- [ ] Player can explain origin of both salvage resources.
- [ ] Player can see whether a local input is rising or falling.
- [ ] Player can identify factory productivity and the active bottleneck.
- [ ] Player can intentionally reduce power demand by disabling a building.
- [ ] Player experiences a meaningful Fishing-vs-Salvage/material-vs-power management decision.
