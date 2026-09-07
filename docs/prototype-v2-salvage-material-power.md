# HUSK — Prototype V2: Salvage, Material & Power

## Status

ACTIVE IMPLEMENTATION SPECIFICATION. Prototype V1 is COMPLETE historical/regression baseline.

Prototype V2 is authorized as **one continuous implementation pass**. Internal milestones exist only to keep implementation/test work ordered; the Implementer does **not** stop for user playtest between milestones. Implement the full V2 scope, test continuously, fix failures as they appear, then return one final implementation report for Manager acceptance.

## Goal

V2 adds the next core management loop on top of V1:

1. one shared Harbor supports multiple small-boat roles;
2. Salvage Boats recover material inputs;
3. recovered material is buffered locally and processed into usable construction material;
4. powered buildings create shared electric demand against available capacity;
5. material shortage and electric shortage reduce factory productivity;
6. local-storage UI exposes both current buffer level and whether that buffer is rising or falling;
7. the integrated prototype creates visible tradeoffs between Fishing, Salvage, processing throughput and electric load.

V2 is still a prototype. It is **not** a final economy-balance pass and **not** the citizen happiness/class/nutrition prototype.

---

## Design Principles

- Reuse working V1 Module/Pipeline/Production/Population foundations where possible.
- Do not rewrite working systems merely to make V2 architecture more generic.
- Prefer one transparent rule reused across factories/consumers rather than bespoke hidden multipliers.
- Gameplay constants remain configurable/provisional unless explicitly confirmed below.
- UI should answer “what is happening?” and “is it getting better or worse?” without exposing implementation internals.
- Correctness and playable iteration speed beat future-proof architecture.
- During the continuous pass, a failing milestone is a bug-fix point, **not** a stop boundary. Fix it, rerun relevant validation, then continue.

---

# 1. Shared Harbor & Small Boats

The existing fishing-only facility becomes a general **Harbor** for small boats.

The Harbor owns the small-boat workflow:

- build small boats;
- retain/identify their role;
- dock/dispatch them;
- receive returned cargo;
- support multiple independent boats concurrently.

V2 boat roles:

- **Fishing Boat** → returns Fish;
- **Salvage Boat** → returns Scrap Metal + Scrap Wood.

The Harbor is not resource-specific. Do not create a separate Fishing Port and Salvage Port.

### Required Harbor behavior

- Existing Fishing Boat loop remains functional.
- Salvage Boat uses the same Harbor workflow/foundation rather than a parallel one-off system.
- Each boat has independent state/timer/cargo; one boat must not overwrite another.
- Cargo is credited exactly once on successful return/unload.
- Fresh/start-reset behavior must recreate deterministic boat/resource state.
- UI/state must make Fishing vs Salvage role legible enough for playtest.

### Provisional boat values

Existing V1/V0 working values may be reused where practical (for example build time/trip interval/cargo cadence). Salvage cargo amount, build cost and exact timing remain configurable/provisional. Do not invent final balance.

---

# 2. Salvage Resources & Processing

New recovered concrete inputs:

- **Scrap Metal**
- **Scrap Wood**

Processed construction outputs:

- Scrap Metal → existing usable metal identity, preferably existing `Iron` unless the current code already has a more appropriate processed-metal resource;
- Scrap Wood → usable construction wood. UI may say `Lumber`, but do not perform a broad code/resource rename merely for wording if existing `Wood` is the compatible usable item.

The player-facing semantic distinction is:

- Scrap Metal / Scrap Wood = recovered raw salvage;
- Iron / Wood-or-Lumber = processed, ready-to-use construction material.

### Processing rule

A processing factory:

1. receives required salvage into local input storage;
2. draws from that local buffer while producing;
3. outputs the processed concrete item through the existing compatible material/storage foundation;
4. is affected by local material availability and power productivity rules below.

Exact conversion ratio, base interval and storage capacity are provisional/configurable.

Do not create unnecessary distinct factory frameworks if the existing Recycler can be extended/instanced/configured cleanly for metal/wood processing.

---

# 3. Local Storage

Local storage is the **buffer immediately available to one consumer/building**, distinct from city/global stock.

The purpose is to create readable delay between city-level supply changes and local consequences.

## 3.1 Core buffer behavior

For each displayed local input:

- `LocalQuantity` = current amount in that local buffer;
- `LocalCapacity` (or target) = configured maximum/normal buffer amount;
- `LocalRatio = clamp01(LocalQuantity / LocalCapacity)`.

Progress bar displays `LocalRatio`.

If capacity is zero because of invalid configuration, implementation must fail safely and never produce NaN/Infinity.

### Feeding local storage

Local storage must be filled only from valid existing V1 supply/resource paths and concrete available stock/output. It must not create resources from nothing.

Preserve V1 pipeline connectivity semantics: having a global item somewhere is not enough if the building/consumer does not have the compatible required network path/supply.

Use the smallest integration compatible with current architecture. Do not add detailed logistics vehicles/haulers in V2.

### Consuming local storage

Factory and House/Citizen consumption draws from local storage, not directly from an abstract infinite supply flag.

Local stock therefore acts as a buffer:

`upstream shortage -> local buffer drains -> UI trend warns -> consequence appears when/while buffer becomes insufficient`

A temporary upstream interruption should not instantly erase remaining local stock.

---

# 4. Local Storage Trend UI

Every displayed local-input bar has a trend indicator.

Required states:

- **Up**: buffer is increasing;
- **Down**: buffer is decreasing;
- **Neutral**: approximately stable.

Presentation convention:

- Up arrow: green/positive;
- Down arrow: red/negative;
- Neutral: neutral/no alarm.

Exact final palette is not V2 art scope.

Trend should represent a short, stable recent net change rather than instantaneous frame noise. A small rolling/sample window or simple hysteresis is acceptable. Choose the smallest implementation that does not flicker during bursty boat deliveries/consumption.

Trend is diagnostic only; it must not affect simulation.

---

# 5. House / Citizen Input UI in V2

V2 does **not** implement total citizen Satisfaction/Happiness.

For the existing House/Citizen consumption layer, V2 adds only local-buffer feedback for the two current essential inputs:

- Food local-storage progress bar + trend;
- Water local-storage progress bar + trend.

The bars represent local storage ratio, not Happiness and not a nutrition score.

Do not add:

- overall Satisfaction;
- Happiness;
- Calories/Protein/Vitamins;
- Diet Diversity;
- class-specific diet requirements;
- mortality/migration/health/security consequences.

Those belong to a later prototype.

---

# 6. Factory Productivity

Factory UI shows an overall **Productivity %** plus local input bars.

Productivity is the current throughput factor from required bottlenecks.

## 6.1 Material input satisfaction

For each required material input, derive a transparent satisfaction value from its local availability.

V2 default rule:

`InputSatisfaction = LocalRatio`

where `LocalRatio = clamp01(LocalQuantity / LocalCapacity)`.

This intentionally avoids additional hidden modifiers.

If a factory has multiple required material inputs:

`MaterialSatisfaction = min(InputSatisfaction_1, InputSatisfaction_2, ...)`

The most deficient required material input is the material bottleneck.

A factory with no required material input contributes no material penalty (material factor = 1).

## 6.2 Final productivity

After power integration:

`FactoryProductivity = min(MaterialSatisfaction, PowerEfficiency)`

For factories with additional already-existing mandatory operational gates (Damaged, disconnected required pipeline, explicitly Disabled), those gates remain authoritative. V2 productivity must not accidentally bypass V1 operational rules.

Display:

`ProductivityPercent = round/format(FactoryProductivity * 100)`

Exact display rounding is implementation detail.

## 6.3 Interval scaling

Confirmed behavior:

`ActualInterval = DefaultInterval / FactoryProductivity`

Examples:

- 100% productivity → default interval unchanged;
- 80% → interval × 1.25;
- 50% → interval × 2;
- 25% → interval × 4.

If productivity <= 0, production progress stops safely until productivity recovers. Never divide by zero or generate NaN/Infinity.

Do not accumulate duplicate outputs while stopped/recovering.

---

# 7. Electric Capacity + Demand

Power is **capacity**, not a concrete inventory item and not kWh consumed per production cycle.

A powered building has configurable:

- `RatedPowerKW`;
- `Enabled` state.

Demand rule:

`BuildingPowerDemand = Enabled ? RatedPowerKW : 0`

An enabled powered building continues to count its rated demand even if its local material buffer is low. V2 does not add a separate idle-power state.

Example discussed for a Recycler/processor: `2 kW` enabled demand. Treat exact rated values as configurable/provisional unless already confirmed by existing data.

## 7.1 Capacity

Power-producing buildings contribute configurable capacity in kW while operational.

Use the existing V1 Electric pipeline/topology rather than inventing a separate invisible grid.

A powered consumer that has no valid supplied Electric path receives:

`PowerEfficiency = 0`

For a valid supplied Electric connected component/grid, calculate:

`TotalPowerCapacity = sum(operational generator capacity in that connected Electric component)`

`TotalPowerDemand = sum(enabled powered-building rated demand in that connected Electric component)`

Then:

- if `TotalPowerDemand <= 0`, `PowerEfficiency = 1`;
- otherwise `PowerEfficiency = min(1, TotalPowerCapacity / TotalPowerDemand)`.

If current architecture makes per-E-component aggregation disproportionately invasive, the Implementer may use the smallest equivalent integration that still preserves V1 Electric connectivity and does not let disconnected buildings consume remote capacity. Report the chosen implementation explicitly.

## 7.2 Shared slowdown behavior

If capacity >= demand, powered factories run at 100% power factor.

If demand > capacity, all enabled powered factories sharing that grid/component receive the same `PowerEfficiency` factor.

Example:

- capacity = 100 kW;
- demand = 150 kW;
- PowerEfficiency = 100 / 150 = 0.6667.

A factory otherwise fully supplied therefore runs at ~66.7% productivity and its interval becomes ~1.5× default.

## 7.3 Enable/disable management

The player must be able to switch relevant powered processing buildings on/off.

Turning a building off:

- its demand becomes 0;
- it does not produce;
- city/grid demand recalculates;
- remaining enabled factories may recover power efficiency.

Turning it back on restores rated demand and normal operational checks.

No power priority automation in V2.

---

# 8. Factory UI Contract

When inspecting/selecting a processing factory, V2 UI must expose at minimum:

- building name;
- operational/Enabled state;
- Enabled on/off control where applicable;
- **Productivity %**;
- one progress bar per required material local storage;
- percentage or sufficiently clear fill state for each bar;
- Up/Down/Neutral trend indicator per local storage;
- power information sufficient to diagnose whether power is the bottleneck.

Example conceptual display:

```text
METAL RECYCLER
Enabled: ON
Productivity: 62%

Scrap Metal  [████████████--------] 62%  ↓
Power        80%
```

If material is 62% and power 80%, productivity is 62%.

If material is 90% and power 55%, productivity is 55%.

Do not make the player infer the bottleneck from hidden debug values only.

---

# 9. V2 Integrated Scenario / Player Decisions

The playable V2 scenario should make these decisions observable:

- build/use Fishing Boats to protect Food supply;
- build/use Salvage Boats to obtain Scrap Metal + Scrap Wood;
- notice which salvage input/local buffer is falling;
- process salvage into usable construction materials;
- recognize material shortage through falling bars before complete stoppage;
- recognize power overload through reduced Productivity;
- switch a powered factory off to relieve demand;
- observe another factory recover productivity after load shedding;
- choose between additional production and available electric capacity.

Do not require a final polished tutorial; UI state and playtest setup only need to make the loop understandable.

---

# 10. Continuous Implementation Milestones

These are implementation order/checkpoints, **not stop gates**.

## Milestone A — Harbor + Salvage

- generalize Fishing Harbor to shared Harbor;
- preserve Fishing Boat;
- add Salvage Boat;
- add Scrap Metal/Scrap Wood accounting;
- validate multi-boat independence and reset behavior.

Run targeted tests/compile/runtime checks. Fix failures, then continue directly.

## Milestone B — Processing + Local Buffers

- add/configure salvage processors;
- local input storage foundation;
- refill/consume semantics through valid existing network/resource paths;
- local progress bars + stable trend arrows;
- House Food/Water local bars/trends only;
- material-driven Productivity + safe interval behavior.

Run targeted + regression tests. Fix failures, then continue directly.

## Milestone C — Power

- rated generation capacity;
- rated enabled-building demand;
- enable/disable control;
- PowerEfficiency;
- integrate power into factory Productivity and interval scaling;
- expose diagnostic power UI.

Run formula/runtime overload/recovery tests. Fix failures, then continue directly.

## Milestone D — Full Integration / Hardening

Demonstrate and validate in one V2 scene/scenario:

1. Fishing Boat functioning;
2. Salvage Boat returning both scrap types;
3. salvage local storage filling/draining;
4. processing into usable material;
5. falling input buffer + down trend;
6. material bottleneck productivity reduction;
7. electric overload productivity reduction;
8. disabling one powered factory reduces demand and recovers another;
9. re-enabling restores demand;
10. House Food/Water bars/trends remain informational only;
11. V0/V1 regression suite remains green.

Fix implementation-caused failures until final validation passes or a genuine design blocker is encountered.

---

# 11. Required Tests / Validation

Implementer must use tests where current project architecture supports them. At minimum cover the behaviors below either through automated tests or explicit runtime validation when automation is impractical.

### Harbor / boat

- Fishing Boat still unloads Fish exactly once per return;
- Salvage Boat unloads Scrap Metal and Scrap Wood exactly once;
- multiple boats maintain independent role/state/timer/cargo;
- reset/fresh session deterministic.

### Local storage

- buffer never becomes NaN/Infinity;
- fill respects capacity;
- consume does not go below zero;
- global/upstream shortage drains local buffer rather than deleting it instantly;
- trend reports rising/falling/stable correctly with noise tolerance;
- material satisfaction maps transparently to local ratio.

### Productivity

- multiple required material inputs use minimum factor;
- 100%, 80%, 50%, 25% values scale interval predictably;
- 0% stops safely;
- recovery does not duplicate production.

### Power

- disabled powered building contributes zero demand;
- enabled building contributes rated demand;
- zero demand => 100% power efficiency;
- capacity >= demand => 100%;
- demand > capacity => capacity/demand;
- disconnected/unsupplied Electric path cannot use remote capacity;
- power bottleneck combines with material bottleneck through `min()`;
- disabling load can recover power efficiency.

### Regression

- relevant V0/V1 EditMode tests remain passing;
- Module/Pipeline connectivity required by V1 still functions;
- existing House/Population baseline remains compatible;
- no unrelated generated/IDE files are introduced.

### Unity validation

Before final report:

1. inspect `ProjectVersion.txt`, `Packages/manifest.json`, current project structure;
2. inspect git status/diff before and after implementation;
3. allow Unity refresh/recompile;
4. check compilation state;
5. inspect Console and distinguish pre-existing historical messages from new implementation-caused errors;
6. run relevant automated test suite;
7. enter Play Mode and execute integrated V2 scenarios;
8. exit Play Mode cleanly;
9. inspect final diff/status;
10. do not claim success while implementation-caused compile/test/runtime errors remain.

---

# 12. Explicitly Out of Scope for V2

Do not add unless user explicitly reopens scope:

- total citizen Satisfaction/Happiness;
- Calories / Protein / Vitamins;
- Diet Diversity;
- Worker / Technician / Upper Class dietary expectations;
- luxury food;
- mortality, migration, health, security, politics;
- detailed citizen assignment simulation;
- detailed logistics vehicles/haulers between storages;
- batteries;
- voltage;
- power distance loss;
- automated power priority/load shedding;
- fuel simulation unless already strictly required by an existing power building;
- final economy balance;
- production-quality final art;
- tech tree/tutorial/combat/exploration;
- generic future-scale factory/logistics framework not necessary for this prototype.

---

# 13. Final V2 Acceptance Questions

V2 is successful if a player can answer by inspecting/playing the prototype:

1. Is this Harbor shared by Fishing and Salvage boats?
2. Where do Scrap Metal and Scrap Wood come from?
3. How does salvage become usable construction material?
4. How full is each important local input buffer?
5. Is that buffer currently rising, falling or stable?
6. What is a selected factory's current Productivity?
7. Which material input is currently limiting it?
8. Is electric shortage limiting it instead?
9. Does switching a powered building off reduce demand and improve the remaining grid predictably?
10. Can the player perceive the core tradeoff between Fishing, Salvage, processing capacity and electric capacity?

If these are legible and the relevant regression/Unity validation is clean, V2 is ready for Manager/user playtest and acceptance.
