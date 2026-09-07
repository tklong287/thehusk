# HUSK — Prototype V2: Salvage, Material & Power

## Status

ACTIVE SPECIFICATION. V1 is COMPLETE historical baseline.

## Goal

Prototype V2 tests the next core management loop on top of V1:

1. Small-boat Harbor supports multiple boat roles instead of being a fishing-only extractor.
2. Salvage creates recoverable material inputs.
3. Factories process salvage through local input storage.
4. Power is a shared capacity/demand system; enabled buildings consume rated kW.
5. Shortage is legible before failure through local-storage bars and trend arrows.
6. Factory productivity falls when power or required material inputs are insufficient.

V2 is still a prototype. It is not an economy-balance pass and not the citizen happiness/class/nutrition prototype.

## Design Principles

- Reuse working V1 systems where possible; do not rewrite working network/production loops without need.
- Prefer one readable rule reused across consumers/factories rather than bespoke systems.
- Gameplay values remain configurable/provisional unless explicitly confirmed.
- UI should explain current state and direction of change without requiring the player to inspect formulas.

## 1. Harbor & Small Boats

The existing fishing harbor becomes a general small-boat Harbor.

The Harbor owns the small-boat workflow:

- build small boats;
- dock/dispatch small boats;
- receive returned cargo.

V2 boat roles:

- Fishing Boat → Fish;
- Salvage Boat → Scrap Metal + Scrap Wood.

The Harbor is not dedicated to one resource category. Do not create separate Fishing Port and Salvage Port buildings for V2.

Existing boat timing/build behavior may be reused as provisional values unless V2 acceptance needs a change.

## 2. Salvage Resources & Processing

New recovered inputs:

- Scrap Metal
- Scrap Wood

Processed construction materials:

- Scrap Metal → existing usable metal resource (prefer existing `Iron` identity unless a later explicit rename is requested);
- Scrap Wood → usable construction wood. Keep existing code/resource compatibility; UI may use `Lumber` when appropriate, but do not perform a broad rename migration merely for wording.

Recycler/processing buildings receive salvage into local input storage and convert it to usable material.

Exact conversion ratios, local-storage capacity and cycle interval are provisional/configurable.

## 3. Local Storage

Local storage is the buffer immediately available to a consumer/building.

Core visual rule for every displayed local input:

- progress bar = current local storage level / local storage target or capacity;
- up arrow = local storage trending upward;
- down arrow = local storage trending downward;
- neutral state = approximately stable.

Trend exists to communicate direction, not only current quantity. Exact averaging/smoothing window is an implementation detail; keep it simple and stable enough to avoid noisy frame-by-frame flicker.

### Citizen in V2

Citizen/House input UI shows only the two V2 essential input buffers already relevant to V1:

- Food local storage bar + trend;
- Water local storage bar + trend.

Do NOT add total citizen Satisfaction, Happiness, nutrition, class diet, penalties, mortality or other advanced citizen consequences in V2.

### Factory in V2

Factory UI shows:

- Productivity %;
- one local-storage progress bar + trend per required material input;
- power status/efficiency where useful for diagnosing productivity.

The player should be able to see both the bottleneck outcome (Productivity) and the local input buffers causing it.

## 4. Power Model

Power is capacity, not a stock item consumed from storage.

Each enabled powered building has a rated power demand in kW.

`BuildingPowerDemand = RatedPowerKW * Enabled`

where `Enabled` is 1 when the building is switched on and 0 when switched off.

A powered building consumes its rated demand while enabled, even if its local input storage is currently low. V2 does not need a separate idle-power state.

City totals:

`TotalPowerDemand = sum(EnabledBuildingRatedPowerKW)`

`PowerEfficiency = min(1, TotalPowerCapacity / TotalPowerDemand)`

When TotalPowerDemand is 0, PowerEfficiency is 1.

Example provisional building value discussed for Recycler: 2 kW while enabled. Keep exact rated values configurable.

## 5. Factory Productivity

Factories use one bottleneck rule.

For every required material input, calculate an input satisfaction value from its local storage availability. Power contributes another satisfaction value through `PowerEfficiency`.

`FactoryProductivity = min(PowerEfficiency, RequiredInputSatisfaction...)`

The most deficient required input determines current productivity.

Production speed:

`ActualInterval = DefaultInterval / FactoryProductivity`

At 100% productivity, interval is unchanged. At 50%, the interval doubles.

If productivity reaches 0, production cannot progress until the bottleneck recovers. Avoid divide-by-zero in implementation.

Exact mapping from local-storage level to InputSatisfaction should be the smallest readable implementation consistent with the displayed local-storage ratio; do not add extra hidden multipliers in V2.

## 6. Player Decisions V2 Must Expose

The prototype should create understandable decisions such as:

- send Harbor capacity toward Fishing or Salvage;
- salvage more Scrap Metal vs Scrap Wood according to bottleneck;
- process recovered material into usable construction resources;
- switch buildings off to reduce grid demand;
- recognize a future shortage from a falling local-storage bar before productivity collapses;
- identify whether low factory productivity is caused by power or material input.

## 7. V2 UI Feedback

Required prototype feedback:

### Local input

`Resource Name  [progress bar]  %  trend-arrow`

Direction convention:

- up: positive/increasing (green in final presentation; exact palette remains provisional);
- down: negative/decreasing (red in final presentation; exact palette remains provisional);
- neutral: stable.

### Factory

At minimum:

- building name/state;
- Enabled on/off control/state;
- Productivity %;
- material local-storage bars and trends;
- power information sufficient to diagnose grid shortage.

### Citizen/House

At minimum:

- Food local-storage bar + trend;
- Water local-storage bar + trend.

No total Satisfaction/Happiness metric in V2.

## 8. Out of Scope

Do not add in V2 unless explicitly reopened by user:

- citizen Happiness/Satisfaction score;
- class-based nutrition;
- Calories / Protein / Vitamins;
- Diet Diversity;
- Worker/Technician/Upper Class expectations;
- mortality, migration, health, security or political systems;
- luxury food requirements;
- detailed logistics vehicles between global and local storage;
- batteries, voltage, distance loss, power priority automation or advanced grid simulation;
- production-quality art/final balance;
- new future-proof frameworks unrelated to V2 acceptance.

## 9. Phase Plan

### Phase 1 — Harbor + Salvage Loop

Implement/rework the Harbor as the shared small-boat facility and add Salvage Boat flow producing Scrap Metal and Scrap Wood. Preserve Fishing Boat behavior through the same Harbor.

Stop for playtest.

### Phase 2 — Processing + Local Storage

Add salvage processing, local input storage, shortage-driven factory productivity foundation, and local-storage UI bars/trends. Add Food/Water local-storage bars/trends for House/Citizen without total satisfaction.

Stop for playtest.

### Phase 3 — Power Capacity + Demand

Add rated kW demand for enabled powered buildings, total capacity/demand, PowerEfficiency, enabled on/off behavior, and integrate power as a factory-productivity bottleneck affecting interval.

Stop for playtest.

### Phase 4 — Integrated V2 Playtest

Harden and validate the combined loop: Fishing vs Salvage, material processing, local-storage warnings, building enable/disable, overload and bottleneck readability. No major feature expansion.

Stop for user acceptance.

## 10. V2 Success Questions

V2 succeeds if a player can answer, by looking at the prototype:

1. What is the Harbor doing and which small boats exist?
2. Where do Scrap Metal and Scrap Wood come from?
3. Which processed material is currently limiting construction/production?
4. Is a local input buffer rising or falling?
5. Which factory is underperforming and by how much?
6. Is that productivity loss caused by power or by a material input?
7. Can switching off a building relieve power shortage in a predictable way?
8. Does the prototype create a clear Fishing-versus-Salvage allocation decision?
