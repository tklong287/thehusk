# HUSK — Prototype V2 Manager Checklist

## Manager Rules

Source priority for V2:

1. User's latest explicit decision.
2. `AGENTS.md`.
3. `docs/prototype-v2-salvage-material-power.md`.
4. This checklist.
5. `docs/manager-status-v2.md`.

V0/V1 docs are completed history/regression evidence and should not be rewritten during normal V2 implementation.

Prototype V2 is authorized as **one continuous implementation pass**. Milestones A–D are implementation checkpoints only. Implementer must test/fix continuously and proceed through all milestones without waiting for user playtest between them.

Manager performs one final acceptance review after the full V2 implementation report. Implementer does not mark V2 PASS/DONE, update this checklist, or update manager status unless explicitly told to do so.

---

# Milestone A — Shared Harbor + Salvage

- [ ] Existing fishing-only facility is generalized to one shared Harbor without creating a separate Salvage Port.
- [ ] Existing Fishing Boat still works through the Harbor.
- [ ] Salvage Boat can be built/managed through the same Harbor workflow/foundation.
- [ ] Salvage Boat returns both Scrap Metal and Scrap Wood as concrete resources.
- [ ] Multiple boats maintain independent role/state/timer/cargo.
- [ ] Cargo is credited exactly once per successful return/unload.
- [ ] Fresh/reset behavior is deterministic.
- [ ] Harbor UI/state makes Fishing vs Salvage roles legible for playtest.
- [ ] Relevant targeted tests/compile/runtime checks pass before moving on.

# Milestone B — Processing + Local Storage

- [ ] Scrap Metal processes into existing usable metal identity without unnecessary broad rename.
- [ ] Scrap Wood processes into usable construction wood; UI may say Lumber while preserving code compatibility where appropriate.
- [ ] Factory required material inputs have local storage/buffer state.
- [ ] Local buffers are fed only by valid compatible V1 network/resource supply; they do not generate resources from nothing.
- [ ] Local storage respects capacity, never goes below zero, and survives upstream shortage until locally consumed.
- [ ] Factory material input UI shows progress bars from local storage ratio.
- [ ] Each displayed local-storage input shows stable Up/Down/Neutral trend.
- [ ] Trend is diagnostic only and does not affect simulation.
- [ ] Factory UI shows current Productivity %.
- [ ] `InputSatisfaction = LocalRatio` or mathematically equivalent transparent mapping.
- [ ] Multiple required material inputs use the minimum satisfaction as material bottleneck.
- [ ] Productivity 0 stops progress safely without NaN/divide-by-zero or duplicate output on recovery.
- [ ] House/Citizen UI shows Food and Water local-storage bars + trends only.
- [ ] No citizen total Satisfaction/Happiness/nutrition/class system was introduced.
- [ ] Relevant targeted + regression tests pass before moving on.

# Milestone C — Power Capacity + Demand

- [ ] Power remains capacity, not a concrete stock item.
- [ ] Operational generators contribute configurable rated capacity in kW.
- [ ] Powered buildings have configurable rated kW demand.
- [ ] Enabled powered building contributes rated demand; disabled building contributes zero.
- [ ] Enabled demand remains counted even if local material input is low; no separate idle-power state added.
- [ ] Existing V1 Electric connectivity remains authoritative; disconnected/unsupplied consumers cannot use remote capacity.
- [ ] For each valid Electric grid/component, total capacity and total enabled demand are calculated without leaking across disconnected grids.
- [ ] Zero-demand grid => 100% PowerEfficiency.
- [ ] Capacity >= demand => 100% PowerEfficiency.
- [ ] Demand > capacity => `PowerEfficiency = Capacity / Demand`.
- [ ] Factory productivity is `min(MaterialSatisfaction, PowerEfficiency)` while preserving V1 operational/damaged/connectivity gates.
- [ ] `ActualInterval = DefaultInterval / Productivity` or mathematically equivalent confirmed behavior.
- [ ] 100%, 80%, 50%, 25% productivity scale interval predictably.
- [ ] 0% productivity stops safely.
- [ ] Factory UI provides enough power information to distinguish power bottleneck from material bottleneck.
- [ ] Player can switch relevant powered processing buildings off/on.
- [ ] Turning one load off can visibly reduce demand and recover remaining factory productivity.
- [ ] No battery/voltage/distance-loss/power-priority automation is added.
- [ ] Formula/state/runtime overload/recovery tests pass before final hardening.

# Milestone D — Integrated V2 Hardening

- [ ] One V2 playtest scene/scenario demonstrates Fishing Boat functioning.
- [ ] Same scenario demonstrates Salvage Boat returning Scrap Metal + Scrap Wood.
- [ ] Salvage local buffers fill and drain correctly.
- [ ] Salvage processes into usable construction materials end to end.
- [ ] A falling local input buffer shows Down trend before/while shortage develops.
- [ ] Material shortage visibly reduces Productivity.
- [ ] Electric overload visibly reduces Productivity.
- [ ] Player can disable one powered factory and observe grid/productivity recovery.
- [ ] Re-enable restores demand and expected slowdown when overloaded.
- [ ] Local-storage trend indicators remain readable during bursty deliveries/consumption.
- [ ] House Food/Water bars/trends remain informational only; no accidental happiness/class logic.
- [ ] Existing Module/Pipeline/House/Population behavior needed from V1 remains functional.
- [ ] Relevant full EditMode/regression suite passes.
- [ ] Unity compilation has no implementation-caused errors.
- [ ] Unity Console has no unresolved implementation-caused errors.
- [ ] Play Mode integrated scenario passes.
- [ ] Final diff/status contains no unrelated generated/IDE files and preserves `.meta` files.

---

# Final V2 Acceptance Questions

- [ ] Player understands one Harbor supports Fishing + Salvage small boats.
- [ ] Player can explain origin of Scrap Metal and Scrap Wood.
- [ ] Player can explain salvage → processed construction material.
- [ ] Player can see how full each important local input buffer is.
- [ ] Player can see whether a local buffer is rising/falling/stable.
- [ ] Player can read selected factory Productivity.
- [ ] Player can identify whether a material input is the bottleneck.
- [ ] Player can identify whether power is the bottleneck.
- [ ] Player can intentionally reduce electric demand by disabling a building.
- [ ] Fishing-vs-Salvage and processing-vs-electric-capacity tradeoffs are observable.

Manager marks V2 COMPLETE only after independent source/diff/metas + Unity compile/Console/tests/runtime review of the full continuous pass.
