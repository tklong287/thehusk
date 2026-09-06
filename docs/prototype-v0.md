# HUSK — Playable Prototype V0

## 1. Purpose

Prototype V0 tồn tại để kiểm chứng một câu hỏi gameplay chính:

> Liệu opening của Husk có tạo được cảm giác người chơi khai thác và sản xuất tài nguyên vì một nhu cầu thực tế của settlement, thay vì chỉ làm theo checklist tutorial hay không?

Prototype này không nhằm chứng minh toàn bộ city builder.

Nó chỉ cần chứng minh một early-game management loop nhỏ, rõ ràng và có thể chơi được.

---

## 2. Core Experience

Opening loop mong muốn:
```text
Settlement có nhu cầu
        ↓
Player nhận ra một resource đang thiếu
        ↓
Player xác định production building cần thiết
        ↓
Player thiếu building material
        ↓
Player sử dụng infrastructure hiện có để giải quyết bottleneck
        ↓
Player xây production building
        ↓
Production bắt đầu
        ↓
Nhu cầu ban đầu được giải quyết
```

---

## 3. Prototype Opening State

Fresh start của Prototype V0 phải có:

- Food = 0.
- Water = 5.
- Recyclable Material = 10.
- Wood: chưa đủ để build Water Plant.
- Iron: đủ để build Water Plant và vẫn còn dư sau construction.
- Recycler có sẵn từ đầu, trong trạng thái Broken.
- Water Plant chưa được xây.

Exact starting Wood/Iron values là provisional và phải configurable. Starting state phải có thể tái lập khi bắt đầu fresh run.

## 4. Water as the Initial Need

Water là initial problem dẫn dắt opening. Player cần feedback đủ rõ để hiểu settlement cần Water và cần một cách cung cấp Water.

Representation của Water problem phải nhỏ, dễ hiểu và configurable. Không cần complex citizen simulation để biểu diễn nhu cầu này. Không chốt thêm consumption rate, timer hoặc hậu quả thiếu Water trong specification này.

## 5. Water Plant

Water Plant là giải pháp trực tiếp cho Water problem.

- Player có thể xem requirement/cost trước khi build.
- Building material cần Wood và Iron.
- Fresh start thiếu Wood, nhưng đủ Iron và có phần dư.
- Attempt build khi thiếu resource phải bị chặn với feedback chỉ rõ Wood bottleneck ở fresh start.
- Khi đủ resource, construction là một action đơn giản: kiểm tra và trừ cost, chuyển Water Plant sang Built/Operational.
- Sau build vẫn còn Iron > 0.
- Water Plant bắt đầu cung cấp/tạo Water, với feedback cho thấy initial problem đang được giải quyết.

Exact build costs, production rate và production/consumption mechanism chưa được chốt; giữ provisional/configurable. Không thêm multi-stage crafting/construction, worker logistics hoặc electricity để xây/vận hành Water Plant trong V0.

## 6. Recycler

Recycler là infrastructure có sẵn từ fresh start, nhưng Broken. Player khám phá Recycler như cách giải quyết Wood bottleneck.

- Broken Recycler không thể process material.
- Player phải thực hiện action Repair để chuyển Recycler sang Operational.
- Repair chỉ tốn một lượng resource nhỏ; cost phải configurable.
- Resource dùng để Repair và exact cost chưa phải canon.
- Repair không được tạo thêm resource chain phức tạp hoặc yêu cầu Collection trước khi player học Recycler.
- State transition Broken → Operational phải có feedback rõ.

Nếu lựa chọn repair resource làm thay đổi đáng kể opening experience, phải hỏi user trước khi quyết định.

## 7. Starter Recyclable Material

Player có sẵn Recyclable Material = 10 ngay từ đầu để học Recycler. Không yêu cầu đi collection trước lần xử lý starter material.

Starter material phải đủ để học mechanic và, qua intended recycling, tạo đủ Wood để vượt Water Plant Wood bottleneck. Các provisional costs/conversion values phải phối hợp để flow này không bị chặn.

## 8. Recycling Result

Operational Recycler tiêu hao Recyclable Material và tạo Wood.

- Material giảm và Wood tăng đúng theo conversion đã cấu hình.
- Resource state/UI cập nhật đúng.
- Feedback thể hiện rõ input → process → output.
- Wood nhận được giúp player đạt Water Plant requirement.

Exact conversion ratio và processing values là provisional/configurable. Không xây production-chain framework tổng quát chỉ để phục vụ conversion này.

## 9. Collection Introduction

Collection chỉ được introduce sau khi player đã học Recycler bằng starter processing và starter Recyclable Material đã cạn, theo `AGENTS.md`.

Khi cần thêm material, một collection action/mechanic đơn giản cho phép tăng Recyclable Material; material mới có thể tiếp tục được Recycler xử lý.

Exact collection mechanic và yield chưa được chốt. V0 chỉ cần kiểm chứng learning order: dùng starter material trước, học acquisition sau. Không tạo fleet system, world exploration framework hoặc procedural scavenging/generation.

## 10. Intended Full Opening Flow

```text
Fresh start
→ nhận ra Water problem
→ xem Water Plant requirement
→ phát hiện thiếu Wood
→ phát hiện Recycler có sẵn nhưng Broken
→ Repair Recycler
→ xử lý starter Recyclable Material
→ nhận Wood
→ đủ resource và build Water Plant bằng một action
→ Water production bắt đầu
→ starter Recyclable Material cạn, cần thêm material
→ được giới thiệu Collection
→ có material mới để tiếp tục recycling
```

Đây là thứ tự trải nghiệm cần kiểm chứng, không yêu cầu một tutorial framework mới.

## 11. What Prototype V0 Must Demonstrate

- Nhu cầu thực tế dẫn player đến production building cần thiết.
- Requirement của Water Plant làm Wood shortage có ý nghĩa.
- Infrastructure có sẵn giúp giải quyết bottleneck qua Repair và recycling.
- Starter material giúp player học Recycler trước Collection.
- Build Water Plant hoàn tất causal chain và bắt đầu giải quyết Water problem.
- Feedback đủ rõ để player hiểu nguyên nhân/kết quả của từng action.
- Full opening có thể hoàn thành từ fresh run mà không cần developer can thiệp state.

## 12. Explicit Non-Goals

V0 không phải full game hoặc vertical slice lớn. Không tự implement các system ngoài scope được nêu trong `AGENTS.md`:

- Tech tree.
- Complex citizen simulation.
- Combat.
- Security/policing.
- Morality system.
- Electricity network.
- Logistics network.
- Fleet system.
- World exploration.
- Procedural generation.
- Multiplayer.
- Live-service systems.
- Advanced save architecture.
- Optimization framework cho scale tương lai.

Không thêm chuỗi crafting/construction nhiều bước hoặc abstraction/framework chỉ vì có thể cần sau này.

## 13. Prototype Quality Bar

Ưu tiên correctness, playable iteration speed, readability và architecture vừa đủ cho prototype.

- Có entry point rõ ràng và fresh-run workflow dễ lặp lại.
- Resource state độc lập với UI presentation; UI/developer UI đủ để quan sát và hiểu state.
- Intended flow chơi được mà không sửa Inspector, dùng Console command hoặc manually change state giữa run.
- Không soft-lock trong intended flow.
- Unity compilation PASS; Console không có compile/runtime error từ implementation.
- Manager kiểm source/diff và trực tiếp validate Console/runtime qua Unity MCP khi phù hợp; report của Implementer không tự động là evidence.

Không cần polish hoặc system ở quy mô full game để đạt quality bar này.

## 14. Provisional Values

Chỉ các starting resource values sau đã được xác nhận: Food = 0, Water = 5, Recyclable Material = 10.

Các giá trị/lựa chọn sau chưa được specification này chốt thành canon:

- Starting Wood và Iron.
- Water Plant Wood/Iron costs.
- Repair resource và repair cost.
- Recycle conversion ratio và processing values.
- Water production/consumption values và representation cụ thể của Water problem.
- Collection mechanic và yield.

Giữ gameplay values chưa chốt configurable, dễ chỉnh và ghi rõ provisional. Không rải magic number. Mọi tuning phải giữ starting Wood thiếu, Iron đủ và còn dư sau build, repair nhỏ, starter recycling vượt được Wood bottleneck, và Collection đến sau starter learning/exhaustion.

## 15. Scope Decision Rule

Với implementation detail không ảnh hưởng đáng kể player experience, chọn giải pháp nhỏ nhất, dễ thay đổi và phù hợp architecture hiện tại.

Với quyết định game design mới, không tự biến assumption thành canon: ghi rõ assumption/blocker và hỏi user hoặc Manager. Manager phải hỏi user khi quyết định ảnh hưởng đáng kể opening experience hoặc khi requirement mâu thuẫn.

Không tự mở rộng scope, bắt đầu phase kế tiếp hoặc thêm feature để chuẩn bị tương lai. Không đổi Unity version/render pipeline hoặc thêm third-party package khi chưa có yêu cầu rõ ràng. Các non-goals chỉ được xem xét lại khi user yêu cầu scope mới.

## 16. Prototype Success Condition

Prototype V0 thành công khi fresh run chứng minh toàn bộ intended opening flow: Water need dẫn tới Water Plant, Wood bottleneck dẫn tới Repair Recycler và xử lý starter material, Wood thu được cho phép build Water Plant và bắt đầu Water production; Collection chỉ được giới thiệu sau starter learning/exhaustion.

Player hoàn thành flow mà không cần developer intervention, hiểu cause/effect qua feedback, và có thể bắt đầu fresh run để playtest lại.

Manager chỉ xác nhận complete sau khi Phase 0–7 trong `docs/manager-checklist.md` đạt acceptance criteria, implementation phù hợp `AGENTS.md` và specification này, và fresh-run runtime validation qua Unity Editor PASS.
