# HUSK — Prototype V0 Manager Checklist

## Purpose

File này biến `docs/prototype-v0.md` thành các implementation phase có acceptance criteria cụ thể.

Manager phải xử lý từng phase theo thứ tự.

Không được bắt đầu phase kế tiếp trước khi phase hiện tại PASS toàn bộ acceptance criteria.

Report của Implementer không tự động được coi là evidence.

Manager phải tự kiểm:
- source/diff;
- Unity compilation;
- Unity Console;
- runtime behavior qua Unity MCP khi phù hợp.

Nếu implementation làm phát sinh quyết định game design chưa được specification xác nhận:
Manager phải dừng và hỏi user thay vì tự biến assumption thành canon.

---

# Phase 0 — Technical Foundation

STATUS: DONE

## Goal

Có development environment đủ để Codex implement và trực tiếp validate prototype trong Unity.

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

# Phase 1 — Prototype Shell & Resource State

STATUS: TODO

## Goal

Tạo foundation gameplay nhỏ nhất để fresh run biểu diễn đúng starting state và Water problem.

## Required State

Fresh prototype start phải có:

- Food = 0
- Water = 5
- Recyclable Material = 10
- Wood = provisional configurable value
- Iron = provisional configurable value

Ngoài ra:

- Recycler tồn tại nhưng Broken.
- Water Plant chưa được xây.

## Acceptance Criteria

- [ ] Có một prototype scene/entry point rõ ràng để test V0.
- [ ] Resource state tồn tại độc lập với UI presentation.
- [ ] Có thể add/remove/query resource bằng gameplay code rõ ràng.
- [ ] Starting Food đúng 0.
- [ ] Starting Water đúng 5.
- [ ] Starting Recyclable Material đúng 10.
- [ ] Wood và Iron initial values dễ chỉnh.
- [ ] UI/developer UI đủ để nhìn thấy resource state trong Play Mode.
- [ ] Player có feedback đủ để nhận ra Water là initial problem.
- [ ] Không implement Recycler processing ở phase này.
- [ ] Không implement Collection ở phase này.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh từ implementation.
- [ ] Fresh Play Mode state deterministic đủ để test phase.

## Design Guardrail

Không xây citizen simulation phức tạp chỉ để tạo Water need.

Nếu cần một prototype representation đơn giản cho Water problem thì dùng giải pháp nhỏ nhất và configurable.

---

# Phase 2 — Water Plant Requirement & Wood Bottleneck

STATUS: TODO

## Goal

Player hiểu:

Water problem
→ cần Water Plant
→ hiện chưa đủ Wood.

Chưa cần giải quyết bottleneck trong phase này.

## Acceptance Criteria

- [ ] Water Plant xuất hiện như giải pháp trực tiếp cho Water problem.
- [ ] Player có thể xem requirement/cost trước khi build.
- [ ] Water Plant cần Wood và Iron.
- [ ] Starting Wood < Water Plant Wood cost.
- [ ] Starting Iron > Water Plant Iron cost.
- [ ] Sau hypothetical construction vẫn phải còn Iron dư.
- [ ] Attempt build khi thiếu Wood bị chặn rõ ràng.
- [ ] Feedback cho player chỉ ra Wood là bottleneck.
- [ ] Exact Wood/Iron values vẫn configurable/provisional.
- [ ] Không có multi-stage construction.
- [ ] Không implement Recycler conversion ngoài interface tối thiểu nếu thực sự cần.
- [ ] Không implement Collection.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh.
- [ ] Runtime validation qua MCP chứng minh Water Plant chưa thể build ở fresh state.

---

# Phase 3 — Broken Recycler & Repair

STATUS: TODO

## Goal

Player phát hiện infrastructure có sẵn có thể giúp giải quyết Wood bottleneck nhưng Recycler đang Broken.

## Acceptance Criteria

- [ ] Recycler tồn tại trong prototype từ fresh start.
- [ ] Initial Recycler state = Broken.
- [ ] Broken Recycler không thể process material.
- [ ] Player có action Repair rõ ràng.
- [ ] Repair có chi phí nhỏ và configurable.
- [ ] Repair cost không tạo thêm resource chain phức tạp.
- [ ] Repair thành công chuyển Recycler sang Operational.
- [ ] State transition có feedback rõ.
- [ ] Không yêu cầu Collection để Repair hoặc học Recycler.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh.
- [ ] MCP Play Mode validation chứng minh Broken → Repair → Operational hoạt động.

## Design Guardrail

Nếu resource dùng để Repair chưa được design xác nhận:
- chọn prototype assumption nhỏ nhất;
- giữ configurable;
- document rõ assumption;
- không ghi nó thành canon.

Nếu lựa chọn resource làm thay đổi đáng kể trải nghiệm opening:
Manager phải hỏi user.

---

# Phase 4 — Recycle Starter Material Into Wood

STATUS: TODO

## Goal

Player dùng material có sẵn để hiểu Recycler và vượt Wood bottleneck.

## Acceptance Criteria

- [ ] Operational Recycler nhận Recyclable Material.
- [ ] Recyclable Material được tiêu hao đúng.
- [ ] Recycling tạo Wood.
- [ ] Conversion ratio configurable.
- [ ] Starter Recyclable Material = 10 vẫn đủ để người chơi học mechanic.
- [ ] Intended use của starter material tạo đủ Wood để vượt Water Plant Wood bottleneck.
- [ ] Resource UI/state cập nhật đúng sau recycle.
- [ ] Player nhận feedback rõ input → process → output.
- [ ] Không yêu cầu Collection trước lần recycle đầu.
- [ ] Không tự thêm production-chain framework tổng quát nếu không cần.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh.
- [ ] MCP runtime validation chứng minh:
      Broken/Operational rules đúng,
      material giảm,
      Wood tăng.

---

# Phase 5 — Build Water Plant & Begin Water Production

STATUS: TODO

## Goal

Hoàn tất causal chain đầu tiên:

Water problem
→ Water Plant
→ Wood shortage
→ Recycler
→ đủ Wood
→ Water Plant được xây
→ Water production bắt đầu.

## Acceptance Criteria

- [ ] Sau recycling, player có thể đạt đủ Water Plant requirement.
- [ ] Build action kiểm tra cost.
- [ ] Build action trừ Wood đúng.
- [ ] Build action trừ Iron đúng.
- [ ] Sau build vẫn còn Iron > 0.
- [ ] Water Plant chuyển từ Not Built sang Built/Operational.
- [ ] Construction là một action đơn giản.
- [ ] Water Plant bắt đầu cung cấp/tạo Water theo prototype mechanism dễ hiểu.
- [ ] Player nhận feedback rằng initial Water problem đang được giải quyết.
- [ ] Production/consumption values configurable.
- [ ] Không thêm worker logistics/electricity/construction chain.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh.
- [ ] MCP runtime validation chứng minh full chain tới Water production hoạt động.

---

# Phase 6 — Starter Material Exhaustion & Collection Introduction

STATUS: TODO

## Goal

Chỉ sau khi player đã hiểu Recycler mới giới thiệu cách acquisition thêm Recyclable Material.

## Acceptance Criteria

- [ ] Starter Recyclable Material có thể bị tiêu thụ/cạn.
- [ ] Collection không phải prerequisite của Recycler tutorial đầu tiên.
- [ ] Khi cần thêm Recyclable Material, game giới thiệu collection.
- [ ] Prototype có một collection action/mechanic đơn giản.
- [ ] Collection làm tăng Recyclable Material.
- [ ] Material mới có thể tiếp tục được Recycler xử lý.
- [ ] Không tạo fleet system.
- [ ] Không tạo world exploration framework.
- [ ] Không tạo procedural scavenging.
- [ ] Collection mechanic đủ nhỏ để chỉ test learning order.
- [ ] Unity compile PASS.
- [ ] Console không có error phát sinh.
- [ ] MCP runtime validation chứng minh:
      starter processing trước,
      acquisition sau.

---

# Phase 7 — Full Fresh-Run Validation

STATUS: TODO

## Goal

Một fresh run hoàn thành toàn bộ Prototype V0 mà không cần developer intervention.

## Required Flow

Start
→ understand Water problem
→ inspect Water Plant
→ discover Wood shortage
→ discover Broken Recycler
→ Repair Recycler
→ recycle starter material
→ gain Wood
→ build Water Plant
→ Water production begins
→ later require more recyclable material
→ discover Collection

## Acceptance Criteria

- [ ] Fresh run bắt đầu đúng starting state.
- [ ] Intended sequence hoàn thành được từ đầu tới cuối.
- [ ] Không cần sửa Inspector giữa run.
- [ ] Không cần Console command.
- [ ] Không cần developer manually change state.
- [ ] Không soft-lock.
- [ ] Không có required tutorial step ngoài core loop mà specification không yêu cầu.
- [ ] Player-facing/developer feedback đủ rõ để hiểu cause/effect.
- [ ] Restart/fresh-run workflow đủ đơn giản để playtest lặp lại.
- [ ] Unity Console không có compile/runtime error.
- [ ] Không có unrelated feature creep.
- [ ] Manager review toàn bộ diff từ prototype implementation.
- [ ] Manager xác nhận implementation vẫn phù hợp `AGENTS.md`.
- [ ] Manager xác nhận implementation vẫn phù hợp `docs/prototype-v0.md`.

---

# Phase Execution Rules

Manager phải xử lý phase theo thứ tự:

Phase 1
→ Phase 2
→ Phase 3
→ Phase 4
→ Phase 5
→ Phase 6
→ Phase 7

Với mỗi phase:

1. Đọc acceptance criteria.
2. Giao CHỈ phase hiện tại cho Implementer.
3. Chờ Implementer report.
4. Manager inspect code/diff.
5. Manager dùng Unity MCP để kiểm compilation/Console/runtime nếu phù hợp.
6. Đánh dấu từng acceptance criterion dựa trên evidence.
7. Nếu FAIL:
   - gửi feedback cụ thể cho Implementer;
   - rework cùng phase.
8. Nếu PASS:
   - update checklist;
   - update manager-status;
   - commit checkpoint phù hợp;
   - chuyển phase tiếp theo.

Manager không được đánh dấu phase PASS chỉ vì Implementer nói "done".

---

# Stop Conditions

Manager phải dừng và hỏi user nếu gặp:

- quyết định game design mới có ảnh hưởng đáng kể đến player experience;
- requirement mâu thuẫn với prototype-v0.md;
- cần thêm third-party package;
- cần thay đổi Unity version/render pipeline;
- cần mở rộng scope ngoài Prototype V0;
- technical blocker không thể giải quyết bằng implementation nhỏ.

Manager KHÔNG cần hỏi user cho implementation detail nhỏ nếu có thể chọn phương án:
- đơn giản;
- reversible;
- configurable;
- không thay đổi design intent.

---

# Definition of Prototype V0 Complete

Prototype V0 chỉ được coi là COMPLETE khi:

Phase 0 = DONE
Phase 1 = DONE
Phase 2 = DONE
Phase 3 = DONE
Phase 4 = DONE
Phase 5 = DONE
Phase 6 = DONE
Phase 7 = DONE

và fresh-run runtime validation qua Unity Editor đã PASS.