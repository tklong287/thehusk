# HUSK — Prototype V0 Manager Status

## Overall Status

ACTIVE

## Current Phase

Phase 2 — Fishing Harbor & Build Fishing Boat

## Phase Status

| Phase | Name | Status |
|---|---|---|
| Phase 0 | Technical Foundation | DONE |
| Phase 1 | Playable World Shell | DONE |
| Phase 2 | Fishing Harbor & Build Fishing Boat | TODO |
| Phase 3 | Fishing Boat Autonomous Cycle | TODO |
| Phase 4 | Fish Production & Unload Feedback | TODO |
| Phase 5 | Water Production Loop | TODO |
| Phase 6 | Recycler Production Loop | TODO |
| Phase 7 | Prototype Integration & Feel Pass | TODO |

## Design Reset and Existing Implementation

User đã xác nhận V0 chuyển sang visual/system prototype: world, interaction, autonomous production và game feel. Không tutorial/scarcity/balance hoặc intentional bottleneck.

Phase 1–7 đã được đặt lại TODO ở design reset. Checkpoint gameplay trước reset (`bb46f956e56209e226a0a72c038a45beafcc14f3`) được audit và reuse có chọn lọc; Phase 1 mới đã được implement và review riêng theo checklist hiện tại.

Phase 1 giữ resource query/add/remove/validation/events và test foundation; sửa all-resource starting configuration, HUD và scene; bỏ Water-first message/warning. World shell hiện dùng test defaults 100. Production values của Phase 2–6 bên dưới chưa được implement.

## Last Completed Phase

Phase 1 — Playable World Shell

Manager validation on 2026-09-06:

- All 11 current Phase 1 acceptance criteria PASS after independent source/scene/runtime review.
- Entry: `Game/Husk/Assets/Scenes/SampleScene.unity`; existing scene and resource foundation reused.
- World: static ocean 240 x 240, floating hull 18.6 x 18.6, 3 x 3 deck, three module placeholders; seven simple URP materials.
- Camera: provisional orthographic framing, pitch 48 / yaw 35 / size 15; RMB orbit, scroll zoom 8–24, R reset. Input System package already existed; no new package.
- Manager's queued Input System mouse events through controller Update changed rotation to 51/51 and size to 12.6; R restored 48/35 and size 15.
- Manager's composited Game view capture confirmed world/platform/modules and neutral readable HUD; no Water-first warning/tutorial.
- Food, Water, Recyclable Material, Wood and Iron each default 100; each starting field configurable in PrototypeSession. Fish not introduced yet.
- Manager added 1,000,000 Wood: HUD showed 1,000,100; removed 37 Water: 63; attempted debit 64 was rejected. No gameplay storage capacity; integer overflow validation retained.
- Second fresh Play run restored all five resources to 100 and camera to 48/35, size 15.
- MCP recompile completed, failed=false, errors=[]; Manager inspected actual Unity EditMode results: 8/8 tests passed.
- Console: zero new errors/warnings since precheck cursor 10. Two historical Unity Pipeline timeout errors predate this task; retained, not cleared.
- Runtime Husk behaviours are only PrototypeSession, PrototypeHud and PrototypeCamera. No Harbor/Boat gameplay or production loops.
- Final Play Mode stopped; Editor ready; intended scene/assets saved; scene not dirty. Source/meta/scene review and diff whitespace checks passed.
- Visual geometry/scale/layout/colors and camera framing remain reversible prototype assumptions, not final art/design canon. Ocean is static; closest zoom intentionally crops outer deck for detail inspection.

Retained Phase 0 foundation:

Foundation đã được xác nhận ở checkpoint trước:

- Repository: `D:\TheHusk`; Unity project: `Game/Husk`.
- Unity 6000.5.7f1; URP 17.5.0.
- Git/GitHub origin/main và Official Unity CLI operational.
- Official Unity Pipeline 0.6.0-exp.1; Unity MCP/live Editor communication.
- Scene/Console read và Enter/Exit Play Mode đã được validate.
- Custom `husk_implementer` đã smoke-test với project context và MCP kế thừa.

Foundation history retained; current Phase 1 validation is recorded above.

## Current Phase Goal

Next phase is Phase 2 — Fishing Harbor & Build Fishing Boat: Harbor available from fresh run, select/click, Build Fishing Boat and configurable construction around 5s. Autonomous fishing belongs to Phase 3.

Phase 2 remains TODO. It has not begun; this session stops at the Phase 1 checkpoint for user playtesting.

## Current Blockers

None.

## Pending User Decisions

None blocking the next phase. Await user playtest feedback and explicit command before starting Phase 2.

## Provisional / Test Values

| Setting | Current V0 direction |
|---|---|
| Test resource starting value | 100 mỗi resource, configurable |
| Storage | Unlimited for V0; không capacity limit |
| Fishing Boat build time | ≈ 5s, configurable |
| Fishing trip/cycle | ≈ 30s cho complete trip, configurable |
| Fish cargo | 5 Fish/trip, configurable |
| Water production values | Provisional/configurable; chưa chốt exact rate |
| Recycler conversion/processing values | Provisional/configurable; chưa chốt exact ratio/rate |
| Costs/construction values | Provisional/configurable; resources đủ để test, không bottleneck |

Các số là tunable prototype values, không final balance. Unlimited storage là scope rule V0. Các resource test gồm Food, Water, Recyclable Material, Wood, Iron và Fish khi được đưa vào resource state; Fish là resource riêng.

Fishing Harbor có sẵn từ Phase 2; boat tự lặp trip, Phase 4 thêm unload/Fish credit. Water và Recycler là independent production loops.

## Manager Operating State

Phase 1 complete and independently reviewed. Await user playtest.

Playtest: open `Assets/Scenes/SampleScene.unity`, enter Play Mode, inspect world/HUD, hold RMB and drag to orbit, scroll to zoom, press R to reset. Stop/re-enter for a fresh run. Starting resource fields are on `Husk Prototype / PrototypeSession`; observation settings are on `Main Camera / PrototypeCamera`.

Do not spawn a Phase 2 Implementer or start Phase 2 automatically. Wait for the user's next explicit command.

## Status Update Rules

- Khi phase bắt đầu: giữ Current Phase, đổi phase sang IN_PROGRESS.
- Khi phase cần rework: đổi phase sang REWORK và giữ cùng phase.
- Chỉ khi mọi criterion PASS theo evidence Manager: mark DONE, cập nhật Last Completed Phase, Current Phase sang phase kế tiếp; phase kế tiếp vẫn TODO.
- Nếu user yêu cầu checkpoint/dừng, không bắt đầu phase kế tiếp dù Current Phase đã đổi.
- Khi bị chặn: Overall Status = BLOCKED; giữ Current Phase, phase IN_PROGRESS/REWORK; ghi exact blocker và hỏi user.
- Khi blocker được giải quyết: khôi phục ACTIVE và ghi quyết định nếu ảnh hưởng implementation.
- Chỉ Manager cập nhật operational status; Implementer không tự quyết định acceptance/progression.

## Source of Truth Priority

1. User's explicit latest decision.
2. `AGENTS.md` for project/engineering rules.
3. `docs/prototype-v0.md` for design intent and scope.
4. `docs/manager-checklist.md` for phase acceptance criteria.
5. `docs/manager-status.md` for current operational progress.

Status không được âm thầm override design intent hoặc acceptance criteria.

## Completion State

Overall Status chỉ COMPLETE khi Phase 0–7 của direction mới đều DONE, Manager đã validate fresh-run integration qua Unity MCP, không có implementation-caused errors hoặc unresolved blockers, và core loops visible/interactive/stable/readable đủ cho user đánh giá feel.

Không yêu cầu economy balance.
