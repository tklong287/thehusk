# HUSK — Prototype V0 Manager Status

## Overall Status

ACTIVE

## Current Phase

Phase 1 — Playable World Shell

## Phase Status

| Phase | Name | Status |
|---|---|---|
| Phase 0 | Technical Foundation | DONE |
| Phase 1 | Playable World Shell | TODO |
| Phase 2 | Fishing Harbor & Build Fishing Boat | TODO |
| Phase 3 | Fishing Boat Autonomous Cycle | TODO |
| Phase 4 | Fish Production & Unload Feedback | TODO |
| Phase 5 | Water Production Loop | TODO |
| Phase 6 | Recycler Production Loop | TODO |
| Phase 7 | Prototype Integration & Feel Pass | TODO |

## Design Reset and Existing Implementation

User đã xác nhận V0 chuyển sang visual/system prototype: world, interaction, autonomous production và game feel. Không tutorial/scarcity/balance hoặc intentional bottleneck.

Phase 1–7 được đặt lại TODO theo specification/checklist mới. Checkpoint gameplay trước reset (`bb46f956e56209e226a0a72c038a45beafcc14f3`) vẫn được giữ trong repository; không được coi là PASS cho Phase 1 mới.

Đợt cập nhật này chỉ sửa documentation/agent instructions. Unity project và gameplay hiện có chưa được migrate sang direction mới. Manager phải audit implementation đó khi user giao Phase 1 mới; không coi test defaults dưới đây là runtime values đã được triển khai.

## Last Completed Phase

Phase 0 — Technical Foundation

Foundation đã được xác nhận ở checkpoint trước:

- Repository: `D:\TheHusk`; Unity project: `Game/Husk`.
- Unity 6000.5.7f1; URP 17.5.0.
- Git/GitHub origin/main và Official Unity CLI operational.
- Official Unity Pipeline 0.6.0-exp.1; Unity MCP/live Editor communication.
- Scene/Console read và Enter/Exit Play Mode đã được validate.
- Custom `husk_implementer` đã smoke-test với project context và MCP kế thừa.

Đây là foundation history, không phải Unity validation mới trong documentation task.

## Current Phase Goal

Phase 1 — Playable World Shell phải cho người dùng nhìn thấy hình thái Husk khi mở Play Mode:

- Scene/entry point rõ ràng.
- Water/environment tối thiểu và Husk platform/module placeholder.
- Gameplay camera usable; control tối thiểu nếu cần quan sát.
- Basic HUD/resource display; state độc lập presentation.
- Resource test default 100, configurable; storage unlimited.
- Visual placeholders được chấp nhận; chưa production gameplay.

## Current Blockers

None.

## Pending User Decisions

None blocking Phase 1.

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

Documentation reset complete; chưa bắt đầu Phase 1 mới.

Next action: chờ explicit user command trước implementation. Khi được giao Phase 1, Manager audit project hiện tại, giao đúng một `husk_implementer` chỉ Phase 1, rồi tự review evidence theo checklist mới.

Không tự spawn để code hoặc bắt đầu gameplay trong documentation task. Tôn trọng checkpoint/stop để user playtest.

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
