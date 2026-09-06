# HUSK — Prototype V0 Manager Checklist

## Purpose

Checklist chuyển `docs/prototype-v0.md` thành các phase cho visual/system prototype: world, interaction, autonomous production và game feel. Balance làm sau V0.

Phase 1–7 bên dưới thay thế checklist cũ. Status từng phase phản ánh review theo direction mới; acceptance trước design reset không tự chuyển sang checklist này.

Manager tự review source/diff, Unity compilation, Console và runtime qua Unity MCP. Report của Implementer không phải acceptance proof.

---

# Phase 0 — Technical Foundation

STATUS: DONE

## Goal

Có development environment đủ để Codex implement và trực tiếp validate prototype trong Unity.

## Scope

Development environment, Unity/URP, Git và CLI/Pipeline/MCP foundation đã được xác nhận. Kết quả bên dưới là checkpoint Phase 0, không phải tuyên bố vừa chạy lại validation trong documentation task.

## Acceptance Criteria

- [x] Repository root hoạt động tại `D:\TheHusk`.
- [x] Unity project tồn tại tại `Game/Husk`.
- [x] Unity version = `6000.5.7f1`.
- [x] URP hoạt động.
- [x] `AGENTS.md` tồn tại.
- [x] `docs/prototype-v0.md` tồn tại.
- [x] Git remote `origin/main` hoạt động.
- [x] Official Unity CLI hoạt động.
- [x] Official Unity Pipeline package hoạt động.
- [x] Codex Unity MCP kết nối được live Editor.
- [x] MCP đọc được scene.
- [x] MCP đọc được Console.
- [x] MCP có thể Enter/Exit Play Mode.
- [x] Baseline Unity Console không có error/warning.
- [x] Repository có clean checkpoint trước gameplay implementation.

## Manager Note

Không rebuild hoặc redesign technical foundation trừ khi một phase sau thực sự phát hiện blocker.

---

# Phase 1 — Playable World Shell

STATUS: DONE

## Goal

Mở Play Mode và nhìn thấy một prototype có hình thái của Husk.

## Scope

Prototype scene/entry point; water/environment tối thiểu; Husk platform/module placeholder; gameplay camera và control tối thiểu nếu cần quan sát; basic HUD/resource state. Các resource test default 100, configurable; storage unlimited.

## Acceptance Criteria

- [x] Prototype scene/entry point rõ ràng và mở Play Mode được.
- [x] Water/environment và Husk platform/module placeholder nhìn thấy được; visual placeholder được chấp nhận.
- [x] Gameplay camera quan sát usable; control tối thiểu nếu cần đủ để quan sát world.
- [x] HUD hiển thị resource state; gameplay state độc lập presentation.
- [x] Các resource test hiện có bắt đầu ở 100 theo default configuration, dễ chỉnh; fresh run tái lập đúng cấu hình.
- [x] Resource state hỗ trợ query/add/remove rõ ràng, không thêm storage capacity limit.
- [x] Không production gameplay hoặc Fishing Boat construction trong Phase 1.
- [x] Manager tự validate world/camera/HUD và fresh-start values qua Unity MCP Play Mode; kiểm trực quan readability.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không tutorial/scarcity/balance; không thêm production của các phase sau. Fish là resource riêng khi được đưa vào state; Fish production thuộc Phase 4.

---

# Phase 2 — Fishing Harbor & Build Fishing Boat

STATUS: DONE

## Goal

Interaction đầu tiên: Fishing Harbor → Build Fishing Boat → khoảng 5s → boat completed.

## Scope

Harbor có sẵn từ fresh run; select/click Harbor; action Build Fishing Boat; build progress/time; visible Fishing Boat và completion feedback.

## Acceptance Criteria

- [x] Fishing Harbor tồn tại từ fresh run; player không cần xây Harbor.
- [x] Select/click Harbor hoạt động và selection/action feedback rõ.
- [x] Action Build Fishing Boat bắt đầu construction với progress/time nhìn thấy được.
- [x] Default build time khoảng 5s, configurable; completion tạo Fishing Boat nhìn thấy được.
- [x] Multi-boat revision: một Harbor xây được thêm nhiều boat; mỗi completion tạo đúng một boat, construction mới không làm gián đoạn boat cũ.
- [x] Nếu có build cost, cost configurable/provisional và starting resources đủ để dùng mechanic, không bottleneck.
- [x] Không fishing autonomous trip hoặc Fish production trong Phase 2.
- [x] Manager tự dùng Unity MCP kiểm fresh Harbor, interaction, timed construction và boat completion trong Play Mode.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Chỉ Harbor + build boat. Không workers, fleet management framework, manual dispatch system hoặc balancing.

---

# Phase 3 — Fishing Boat Autonomous Cycle

STATUS: DONE

## Goal

Boat nhìn thấy được rời Husk, làm việc và quay lại tự động.

## Scope

At Harbor → Depart → Fishing / Out at sea → Return → Harbor → repeat. Complete trip/cycle khoảng 30s, configurable. Waypoint/simple destination/minimal state machine được chấp nhận.

## Acceptance Criteria

- [x] Boat hoàn thành construction tự bắt đầu cycle; không cần player dispatch.
- [x] Depart, hoạt động ngoài biển và Return về Harbor nhìn thấy được, dễ phân biệt.
- [x] Complete trip khoảng 30s ở default configuration; timing dễ chỉnh.
- [x] Boat tự bắt đầu cycle tiếp theo và chạy nhiều consecutive cycles không player redispatch.
- [x] Multi-boat revision: nhiều boat giữ clock/state riêng, chạy độc lập qua nhiều cycles và có feedback phân biệt từng boat.
- [x] Không Fish production trước Phase 4, trừ interface tối thiểu thực sự cần; không credit resource trong phase này.
- [x] Manager tự quan sát nhiều cycles qua Unity MCP Play Mode, kiểm state/timing và movement readability.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không world navigation framework, fleet manager lớn, pathfinding architecture không cần thiết, route logistics, fuel, maintenance hoặc fishing-area simulation phức tạp.

---

# Phase 4 — Fish Production & Unload Feedback

STATUS: DONE

## Goal

Hoàn tất autonomous production loop đầu tiên: boat về → unload → Fish tăng → tự rời đi tiếp.

## Scope

Fish resource riêng; cargo default 5 Fish/trip configurable; unload tại Harbor; storage unlimited; resource/HUD và delivery feedback.

## Acceptance Criteria

- [x] Fish là resource riêng với Food; starting Fish default 100 và configurable như các resource test khác.
- [x] Boat returns rồi unload 5 Fish/trip theo default cargo configurable.
- [x] Mỗi lần unload credit đúng một lần; Fish state và HUD tăng đúng cargo, delivery feedback rõ.
- [x] Không storage capacity limit chặn unload/production.
- [x] Boat tự rời Harbor cho cycle kế tiếp; nhiều consecutive cycles tiếp tục tăng Fish đúng.
- [x] Manager tự dùng Unity MCP kiểm Fish trước/sau unload và qua nhiều cycles, đối chiếu cargo, HUD và feedback.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không tự bắt đầu Water loop; không workers, fuel, maintenance, storage capacity hoặc fleet management framework.

---

# Phase 5 — Water Production Loop

STATUS: DONE

## Goal

Test building-based production loop độc lập, có Water tăng theo thời gian.

## Scope

Build/select Water Plant → construction/activation đơn giản → Operational → Water tăng. Placement/build interaction tối thiểu nếu cần; operational/production feedback và HUD.

## Acceptance Criteria

- [x] Player đủ resource để sử dụng Water Plant mechanic; không starting shortage hoặc Recycler prerequisite.
- [x] Build/select và construction/activation đơn giản hoạt động; placement tối thiểu nếu cần.
- [x] Operational state nhìn thấy được; Water tăng theo production values đã cấu hình.
- [x] Water HUD và production feedback cập nhật đúng.
- [x] Costs/construction/production values provisional, configurable.
- [x] Manager tự dùng Unity MCP kiểm interaction, operational state và Water tăng qua thời gian trong Play Mode.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không electricity, workers, logistics, construction chain phức tạp, economy bottleneck hoặc tự implement Recycler.

---

# Phase 6 — Recycler Production Loop

STATUS: DONE

## Goal

Test transformation Recyclable Material → Recycler → Wood.

## Scope

Recycler accessible/buildable theo implementation nhỏ nhất; processing input/output và feedback; configurable conversion/processing values.

## Acceptance Criteria

- [x] Recycler accessible/buildable để player test bằng resources có sẵn.
- [x] Processing tiêu hao Recyclable Material và tạo Wood đúng configured conversion; không resource credit/debit sai.
- [x] Processing state/feedback và resource HUD cập nhật rõ input/output.
- [x] Conversion/processing và costs nếu có là provisional/configurable; storage unlimited.
- [x] Recycler hoạt động độc lập, không prerequisite để dùng Water Plant; không Broken/Repair mechanic, Collection hoặc balance/dependency systems.
- [x] Manager tự dùng Unity MCP kiểm input giảm, Wood tăng và processing feedback trong Play Mode.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không generic production-chain framework nếu không cần; không economy balancing hoặc mở rộng phase khác.

---

# Phase 7 — Prototype Integration & Feel Pass

STATUS: DONE

## Goal

Ghép các system V0 để user đánh giá liệu game đã bắt đầu trông và cảm thấy như một game thú vị.

## Scope

Fresh run: world visible → resources available → Harbor exists → build boat → autonomous fishing → Fish returns → Water production available → Recycler production available. Integration fixes nhỏ và feel review.

## Acceptance Criteria

- [x] Fresh run có world/Husk visible, resources default 100/configurable, storage unlimited và Harbor có sẵn.
- [x] Player build boat; boat tự fishing/return/unload/repeat qua nhiều cycles mà không manual redispatch.
- [x] Water production và Recycler transformation sử dụng được độc lập, resource state/HUD cập nhật đúng.
- [x] Manager review camera readability và scale của Husk/buildings/boats.
- [x] Manager review movement, select/click, construction và production feedback cùng HUD readability.
- [x] Manager ghi nhận feel của khoảng 5s build, khoảng 30s fishing, world có cảm giác sống và loops có hiểu được qua observation không.
- [x] Fresh run/restart tái lập và integrated loops chạy ổn, không cần developer sửa state giữa run.
- [x] Manager tự validate integrated run qua Unity MCP và ghi evidence đủ cho user playtest/feel review.
- [x] Manager review toàn bộ implementation diff; phù hợp AGENTS/spec và không feature expansion hoặc economy balance pass.
- [x] Unity compilation PASS; Editor ready.
- [x] Console không có compile/runtime error từ implementation; mọi warning mới được review.
- [x] Sau validation đã Exit Play Mode; intended scene/assets đã save, không còn unrelated dirty state.

## Guardrails

Không thêm gameplay system mới ngoài fix nhỏ cần cho integration. Feel review ghi kết quả, không tự biến sở thích/tuning thành final balance hoặc mở rộng scope.

---

# Phase Execution Rules

1. Đọc AGENTS/spec/checklist/status và exact task user giao; kiểm repository trước khi làm.
2. Xử lý Phase 1 → 2 → 3 → 4 → 5 → 6 → 7. Không bắt đầu phase sau khi phase hiện tại chưa PASS toàn bộ criteria.
3. Cho implementation, spawn đúng một custom agent `husk_implementer`, giao chỉ phase hiện tại. Không dùng agent khác làm workaround nếu custom agent chưa load; không hơn một spawned Implementer đồng thời.
4. Manager sở hữu checklist/status/acceptance và Git checkpoint; Implementer không tự commit/push hoặc đổi phase.
5. Manager tự inspect toàn bộ source/diff, scope và .meta; trực tiếp kiểm compilation/Console/runtime qua Unity MCP. Không chỉ dùng CLI hoặc Implementer report làm bằng chứng.
6. Đánh giá từng criterion PASS/FAIL theo evidence; không clear Console để che lỗi.
7. Nếu FAIL, giữ cùng phase, cập nhật REWORK khi phù hợp, gửi feedback cụ thể và tiếp tục cùng Implementer thread nếu còn khả dụng. Nếu phải spawn lại, chỉ dùng `husk_implementer` và giữ concurrency một Implementer.
8. Sau rework, Manager review lại diff và validate compile/Console/runtime liên quan. Không mark DONE khi còn criterion FAIL.
9. Nếu tất cả PASS, mark phase DONE, cập nhật Last Completed Phase và Current Phase sang phase kế tiếp nhưng giữ phase đó TODO. Checkpoint/commit/push theo user authorization.
10. Nếu user yêu cầu checkpoint hoặc dừng để playtest, DỪNG sau checkpoint; đổi Current Phase không phải quyền tự bắt đầu phase mới.

# Stop Conditions

Dừng và hỏi user khi cần:

- Quyết định gameplay mới ảnh hưởng đáng kể player experience hoặc thay đổi confirmed direction.
- Giải quyết requirement mâu thuẫn giữa documents.
- Thêm third-party package hoặc đổi Unity version/render pipeline.
- Architecture/scope vượt Prototype V0.
- Technical blocker không giải quyết được bằng implementation nhỏ.
- Quyết định mà AGENTS/spec không cho Manager tự chọn.

Khi bị chặn: Overall Status = BLOCKED, Current Phase giữ nguyên, phase giữ IN_PROGRESS/REWORK, ghi exact blocker. Không tự discard partial work hoặc chuyển phase khác.

Implementation detail nhỏ, reversible, configurable và không đổi design intent có thể tự chọn. Không biến test values thành final balance.

# Definition of Prototype V0 Complete

Phase 0–7 đều DONE theo checklist mới; Manager có source/compile/Console và fresh-run runtime MCP evidence. Core loops nhìn thấy được, tương tác được, chạy ổn, readable và đủ để user đánh giá feel. Không còn unresolved blocker.

Không yêu cầu economy balance, tutorial hoặc progression puzzle. Collection không thuộc V0.
