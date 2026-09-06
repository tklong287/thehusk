# HUSK — Prototype V1 Manager Checklist

## Purpose và nguồn thiết kế

Active prototype: **Prototype V1 — Module Network & Planning**. V0 Phase 0–7 COMPLETE là lịch sử, không chuyển checklist V0 thành V1. Phase 1 đã DONE và user chấp nhận; Phase 2 DONE sau corrective contract validation; Phase 3 DONE; Phase 4 DONE; V1 COMPLETE.

Đọc theo priority: latest explicit user decision → AGENTS.md → docs/prototype-v1-module-network.md → checklist này → docs/manager-status-v1.md. Đúng bốn phase lớn; mỗi phase là một task coherent, không chia micro-phase. Checkbox đã đánh dấu có evidence nghiệm thu trong manager-status-v1.md; checkbox còn trống chưa được nghiệm thu. Phase1–2 giữ acceptance history tại thời điểm checkpoint; rule City Hall/starting stock mới thay thế các ghi chú TBD lịch sử, được kiểm ở Phase3.

## V1 Phase 1 — Module Construction + Pipeline Network

STATUS: DONE

**Goal:** player gắn Module vào Husk và thấy pipeline connectivity/supply foundation tác động trực tiếp lên cấu trúc.

**Scope:** Module abstraction, coastline attachment, port blocking, Town Hall Item Storage foundation, construction payment/interaction, standard 3/4, building constraints foundation, F/W/M/E graphs, continuity/pass-through, cross-floor visuals và configure/inspect UI. Reuse/rework V0 visual grid khi phù hợp; không giả định đã có logical grid.

**Acceptance Criteria:**

- [x] Built cell là physical Module, bên ngoài là biển. Build Module attach vào edge hiện có tiếp giáp biển; occupied cell và Fishing Harbor/port blocked edge/vị trí bị từ chối.
- [x] Town Hall Item Storage foundation cho inspect concrete items; 1 stack 100 Module Core ở fresh start, unlimited storage. Không slot/weight/capacity/logistics system.
- [x] Build Module debit đúng 1 Module Core + 10 Wood từ global city storage; không yêu cầu M tới construction site. Invalid/duplicate/không đủ stock không debit một phần hoặc tạo Module trùng.
- [x] Exact starting Wood và construction time giữ TBD/provisional configurable; ghi preset đã thử, không biến số chưa chốt thành canon.
- [x] Standard Module support đúng 3/4 F/W/M/E; M = Material. UI cho configure/inspect lựa chọn và lý do invalid.
- [x] Building constraints foundation kiểm cả required INPUT + OUTPUT: Harbor M/F, Water E/M/W, Recycler E/M, Solar E, House F/W; optional slots player chọn. Town Hall I/O để TBD.
- [x] Bốn network độc lập: chỉ truyền cùng category trên continuous compatible path; không nhảy qua biển/unsupported Module hoặc convert type. Building không chặn supported pass-through.
- [x] Phân biệt connectivity và actual supply foundation: source mất supply làm vùng liên quan unsupplied dù path còn; alternate path/source giữ đúng vùng còn cấp.
- [x] Mỗi Module có floor cross/lane, bright supplied / dim supported-unsupplied / inactive unsupported phân biệt được. F/W/M/E đọc được; exact colors TBD, palette thử được ghi provisional.
- [x] Configure/inspect UI và pipeline floor visual là gameplay feedback; world placement và nguyên nhân mất path/supply đọc được ở camera chơi.
- [x] Tests phù hợp kiểm placement/port blocking/payment/3-of-4/building requirements/path/pass-through/alternate supply và fresh reset. Manager trực tiếp kiểm Unity compile, Console, Play/Stop; intended scene không unrelated dirty state.
- [x] Manager review source/diff/metas và từng criterion, ghi evidence cùng limits/provisional values.

**Guardrails:** Không full production/starting-city integration của Phase 2 hoặc population Phase 3. Source fixture có thể chứng minh foundation nhưng phải ghi rõ fixture, không giả là production thật. Latest explicit user task cho phép minimum reversible post-build configuration: Phase 1 dùng free/instant reconfigure provisional để thử ngắt/khôi phục path; chưa chốt policy cho phase sau hoặc full game.

**Stop:** Sau acceptance, Manager cập nhật DONE và dừng cho user playtest. Không tự bắt đầu Phase 2.

## V1 Phase 2 — Networked Production + Starting City

STATUS: DONE

**Goal:** starting city và bốn mạng vận hành bằng actual production/supply, có hậu quả khi mất input.

**Scope:** 1 Town Hall, 1 Fishing Harbor, 1 Solar Power Plant, 1 House, 1 Water Plant Damaged, 1 Recycler Damaged; Repair; I/O gating/output; actual supply propagation và visual downstream.

**Acceptance Criteria:**

- [x] Fresh city đúng sáu building/số lượng/trạng thái trên. Town Hall dùng storage foundation Phase 1, click xem concrete items/100 Module Core; không tự gán Town Hall I/O.
- [x] Water Plant/Recycler bắt đầu Damaged; Repair trước, sau Repair vẫn kiểm required inputs. Exact repair cost/time từng loại TBD/provisional/configurable, không final balance.
- [x] I/O đúng bảng: Harbor M → F; Water E+M → W; Recycler E → M; Solar không input → E; House F+W → không output. Module support và lock union INPUT+OUTPUT trong UI/domain; optional pass-through editable. Placement tự thêm required pipelines, deterministic optional drop, giữ đúng 3/4 và tối đa config cũ; không popup/manual prerequisite.
- [x] Operational chỉ kiểm INPUT có compatible continuous path + actual supply, cộng Damaged/Repair/concrete conditions hiện có. OUTPUT không là prerequisite: Harbor có M tự cấp F, Solar tự cấp E, Recycler repaired+có E tự cấp M, Water repaired+có E/M tự cấp W. Thiếu input → Disabled và output dừng; Repair không bypass network.
- [x] Fish giữ concrete item identity, feed F và trực tiếp đáp ứng generic Food trong V1; không Food Processing/class diet. M giữ Wood/Iron/Recyclable Material riêng.
- [x] Reuse nhiều boat độc lập/build khoảng 5s/trip khoảng 30s/unload 5 Fish/repeat; không reset boat cũ hoặc duplicate unload khi integrate. Behavior phụ thuộc M phải được kiểm, không mở fleet framework.
- [x] Water/Recycler reuse production/conversion; concrete input không bị bỏ qua, debit/credit đúng. Output feed đúng W/M; Solar source E thật. Không global stock bypass path.
- [x] Source/path mất và hồi phục cập nhật đúng downstream building và bright/dim lanes. Branch còn alternate source/path vẫn hoạt động; không tắt toàn map sai hoặc double-count stock.
- [x] House F/W supply state sẵn sàng cho Phase 3; minimum compatibility không trở thành population simulation triển khai sớm.
- [x] Kiểm startup supply setup cho target Population 100/100 của Phase 3 đồng thời Water/Recycler Damaged; không fake supplied hoặc invent Town Hall I/O. Quyết định gameplay thiếu phải được báo trước acceptance liên quan.
- [x] Manager chạy regression tests phù hợp và Play/Stop thực tế trên intended scene: production/output/input-loss/repair/recovery, compile PASS, Console không implementation errors, scene không unrelated dirty state; review source/diff/metas.

**Guardrails:** Không economy balance; không throughput/congestion/capacity/pressure, voltage/battery/day-night solar. Không population simulation ngoài minimum compatibility. Ghi provisional test stock/rates đủ thử routing.

**Stop:** Manager nghiệm thu và dừng cho user playtest, không tự Phase 3.

## V1 Phase 3 — House + Population + Consumption

STATUS: DONE

**Goal:** House demand và population khiến network placement có hậu quả quan sát được.

**Scope:** Build House, F/W required + slot thứ ba, starting population, capacity/growth/consumption, Disabled/reallocation/overcrowding và UI.

**Acceptance Criteria:**

- [x] City Hall special4/4; F/W/M storage IN/OUT từ concrete stock, E pass-through only/no source/no storage. Fresh mọi concrete item hiện có100; starter F/W qua topology cấp House trong khi Water/RecyclerDamaged. Stock depletion/path loss và recovery phản ánh đúng network.

- [x] Build House trên Module hợp lệ support F + W, slot thứ ba M hoặc E do player chọn; supported third pipeline vẫn pass-through. Không đặt trên biển/occupied site.
- [x] Starting Population 100/100; format current population / effective operational housing capacity. Base House capacity 100; effective capacity chỉ tính Operational Houses.
- [x] +10% compound/minute theo current population: 100 → 110 → 121 → 133.1 trước rounding/display. Rounding/display TBD; giữ fractional precision, không tăng theo capacity.
- [x] Growth không hard-cap hoặc slowdown khi vượt capacity, House Disabled hay capacity = 0. Không tự thêm residents bằng nominal capacity khi Build House.
- [x] Demand mỗi resident 0.05 Food/s và 0.05 Water/s; 100 residents = 5/s mỗi loại. Fish đáp ứng Food qua F; consumption dùng actual reachable supply, không debit trùng/âm hoặc truncate fractional rate mỗi frame.
- [x] House Operational chỉ với F + W supplied; thiếu một loại → Disabled. UI chỉ đúng input thiếu, không giả báo loại còn supply cũng mất.
- [x] Residents được giữ và coi là reallocated sang Operational Houses còn lại, không detailed assignment AI. 180 residents/hai House → một Disabled → 180/100; tất cả Disabled → 180/0 và growth vẫn tiếp tục.
- [x] UI population/resource/building supply/effective capacity/demand cập nhật nhất quán. Overcrowding là valid state, không security/mortality/happiness penalty.
- [x] Tests phù hợp kiểm phút liên tiếp/overshoot/frame partition, fractional consumption, nhiều House không double-spend, supply loss/recovery, capacity 0 và fresh reset. Policy stock allocation/debit thiếu nguồn được ghi rõ; hỏi nếu cần design mới.
- [x] Manager review source/diff/metas, compile/Console và Play/Stop trên scene thực; không unrelated dirty state, ghi evidence từng criterion.

**Guardrails:** Không advanced citizen mechanics, death/migration/happiness/health/growth slowdown hoặc Trị an. Không tự giải open questions thành final design.

**Stop:** Manager nghiệm thu và dừng cho user playtest, không tự Phase 4.

## V1 Phase 4 — Integrated Network Planning Playtest

STATUS: DONE

**Goal:** đánh giá Husk có cảm giác quy hoạch/lắp ráp connected megastructure thay vì đặt các building độc lập trên đất không.

**Scope:** integration/hardening Module expansion, Core+Wood payment, 3/4, Houses/F/W/M/E, production/consumption, damaged/repair, population, visible flow và routing failure/recovery. Không major new subsystem.

**Required Scenarios / Acceptance:**

- [x] Fresh run đúng city/100 Core/100 population và state; xây coastline branch, trả 1 Core + 10 Wood mỗi Module, port block đúng; chọn 3/4 configuration.
- [x] Build supplied House trên F/W Module, kiểm third pipeline pass-through, demand và capacity.
- [x] Chủ động ngắt Water route: House Disabled, Water lanes downstream tắt/dim đúng branch, branch còn nguồn khác không bị tắt sai.
- [x] Restore/reroute Water: House supplied lại, lane sáng đúng. Chỉ dùng configuration workflow được chốt; không tự thêm free reconfiguration/demolition/refund để làm scenario.
- [x] Ngắt Food: House Disabled đúng lý do, Water state độc lập. Khôi phục Food phục hồi supply đúng.
- [x] Ngắt Electric tới production: dependent building Disabled, downstream output supply/visual phản ánh nguồn thực tế; alternate source/path được tôn trọng.
- [x] Repair Water Plant/Recycler; chứng minh thiếu network input vẫn chưa Operational, đủ input mới hoạt động.
- [x] Thêm Houses, quan sát +10% compound growth và consumption; disable House làm effective capacity giảm, population không mất và có thể vượt capacity; tất cả Disabled không dừng growth.
- [x] Kiểm no soft-lock với preset đã ghi; routing/placement có consequence nhìn thấy được. Không ép scarcity hoặc cân bằng economy để đạt tiêu chí.
- [x] Tích hợp production/resource reconciliation, restart/fresh state, camera/readability/interaction ổn; tests/Unity compile/Console/Play/Stop phù hợp đạt, scene không unrelated dirty state.
- [x] Manager trả lời đủ 11 playtest questions bên dưới với evidence, ghi limits/provisional values và quyết định còn mở; review source/diff/metas. Không tự implement future system từ kết quả playtest.

**Stop:** Hoàn tất report và dừng. Không tự bắt đầu prototype mới.

## Playtest questions — giữ đủ 11 câu

1. Placement có consequence không?
2. Player có nghĩ pipeline nào mỗi Module phải giữ không?
3. 3/4 có quá dễ không?
4. Grid routing có khiến failure quá dễ bypass không?
5. Có nên thử 2/4 ở prototype sau không?
6. F/W/M/E visuals có readable không?
7. Có chẩn đoán broken supply bằng mắt không?
8. Topology có tạo districts tự nhiên không?
9. Husk có bớt FarmVille/isometric land grid không?
10. Build Module có cảm giác lắp ráp megastructure không?
11. House/population demand có làm routing meaningful mà chưa cần balance không?

## Manager execution rules

1. Chỉ bắt đầu phase được user giao; implementation dùng đúng một `husk_implementer` tại một thời điểm, ưu tiên tiếp tục Implementer hiện có. Task implementation Phase 1 đã dùng một Implementer; không tự spawn phase sau.
2. Implementer chỉ implementation; Manager sở hữu acceptance, checklist/status/progression và Git checkpoint. Không lấy report Implementer thay independent review.
3. Manager inspect source/diff/metas; kiểm Unity compilation, Console, relevant tests và runtime qua Unity MCP. Không clear Console để che lỗi; không báo DONE chỉ vì code viết xong.
4. Criterion FAIL giữ phase REWORK, gửi feedback và kiểm lại; design mới cần user quyết định, không tự canonize assumption.
5. Khi mọi criterion PASS, mark DONE và ghi evidence/Last Completed Phase; Current Phase có thể chuyển sang phase tiếp theo vẫn TODO nhưng phải dừng cho user playtest. Không auto-start phase mới.
6. Commit/push theo authorization của user; preserve unrelated work, không generated/IDE files, không đổi engine/URP/package hoặc mở scope.

## Open questions và non-goals

Giữ mở: repair cost/time riêng Water Plant và Recycler; có reconfigure sau build không và cost/time; exact F/W/M/E colors; Module construction time; population rounding/display; future Trị an/overcrowding formula, food preferences và 2/4 vs 3/4.

Không Hub trong V1; không Gas/Data/2/4, throughput/congestion/pressure/network capacity/voltage/battery/day-night solar/distance loss, construction logistics hoặc storage capacity. Không economy balance/final art, Food Processing/class diet hoặc advanced citizen penalties. Các future considerations không tạo task chuẩn bị.
