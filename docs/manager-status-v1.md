# HUSK — Prototype V1 Manager Status

## Overall Status

ACTIVE

## Current Phase

V1 Phase 2 — Networked Production + Starting City (TODO; chưa bắt đầu)

## Phase Status

| Phase | Name | Status |
|---|---|---|
| V1 Phase 1 | Module Construction + Pipeline Network | DONE |
| V1 Phase 2 | Networked Production + Starting City | TODO |
| V1 Phase 3 | House + Population + Consumption | TODO |
| V1 Phase 4 | Integrated Network Planning Playtest | TODO |

## Last Completed Task

V1 Phase 1 — Module Construction + Pipeline Network: PASS ngày 2026-09-07 (Asia/Saigon), sau independent Manager review, 55/55 EditMode tests và runtime acceptance.

V0 giữ Overall Status COMPLETE và Phase 0–7 DONE trong docs/manager-status.md; spec/checklist/status V0 là lịch sử giữ nguyên. Không chuyển evidence 36 tests V0 thành acceptance V1.

## Current Phase Goal

Dừng tại checkpoint Phase 1 cho user playtest. Phase 2 chỉ bắt đầu khi user giao task rõ ràng; goal tiếp theo là networked production và starting city theo checklist.

## Current Blockers

None cho Phase 1 đã nghiệm thu. Phase 2 chưa audit/triển khai; startup supply policy vẫn cần làm rõ khi tới integration.

Open questions bên dưới phải được giữ mở; nếu exact workflow/acceptance sau này phụ thuộc câu trả lời, báo Manager/user trước phần việc phụ thuộc. Không đánh dấu phase DONE dựa trên assumption chưa được phép.

## Confirmed V1 Rules / Test Values

| Nội dung | Confirmed V1 value |
|---|---|
| Standard Module | Đúng 3/4 categories |
| Categories | F = Food, W = Water, M = Material, E = Electric |
| Starting Module Core | 1 stack 100 Module Core |
| Module build cost | 1 Module Core + 10 Wood từ global city Item Storage |
| Placement | Attach edge hiện có tiếp giáp biển; cấm occupied Module hoặc Fishing Harbor/port edge/vị trí |
| Town Hall | 1, Item Storage; click xem concrete items |
| Storage | Unlimited; không M construction logistics |
| Fishing Harbor | 1, M input → F output |
| Solar Power Plant | 1, không input → E output |
| House | 1, F+W input → không output |
| Water Plant | 1, DAMAGED; E+M input → W output sau Repair và đủ supply |
| Recycler | 1, DAMAGED; E input → M output sau Repair và đủ supply |
| Population | Fresh 100/100: current population / effective operational housing capacity |
| Base House capacity | 100 |
| Growth | +10% compound/minute theo current population |
| Food demand | 0.05 Food/s/resident, Fish trực tiếp đáp ứng qua F |
| Water demand | 0.05 Water/s/resident |

Module building phải support union required INPUT+OUTPUT, optional slots player chọn và pass-through. Category khác concrete item: Fish thuộc F; Wood/Iron/Recyclable Material thuộc M và giữ identity.

Required input phải có continuous compatible path + actual supply; thiếu bất kỳ input nào → Disabled/Unsupplied. Repair không bypass network. House Disabled giữ residents, coi là reallocated sang Operational Houses; effective capacity giảm nhưng population và compound growth tiếp tục kể cả overcrowded/capacity = 0. Không detailed assignment hoặc penalty.

Visual: floor cross mỗi Module, F/W/M/E phân biệt được; supported-supplied sáng, supported-unsupplied dim, unsupported inactive. Branch failure/alternate source và recovery phản ánh actual topology/supply, không chỉ debug overlay.

## Provisional / Reuse Values

Boat build khoảng 5s, trip khoảng 30s, unload 5 Fish, nhiều boat độc lập và auto repeat là baseline reuse configurable/provisional. Water/Recycler rates và test stocks ngoài Module Core dùng current/provisional values nếu phù hợp, không final balance. Exact starting Wood chưa chốt.

## Pending User Decisions / Open Questions

- Exact Water Plant repair cost/time.
- Exact Recycler repair cost/time.
- Policy reconfigure cho các phase sau/full game: vẫn mở; Phase 1 dùng free/instant provisional theo explicit authorization.
- Exact F/W/M/E colors.
- Module construction time.
- Exact starting Wood.
- Town Hall pipeline requirements.
- Population rounding/display.
- Future overcrowding/Trị an formula.
- Future food preferences.
- Future 2/4 vs 3/4 sau playtest.

Integration cần chứng minh target starting 100/100 với F/W actual supply trong khi Water Plant/Recycler Damaged. Không invent Town Hall I/O hoặc fake supplied; nếu startup source/stock policy cần design decision, hỏi trước acceptance Phase 2–3. Không blocker cho foundation Phase 1.

## Manager Operating Rules

Manager đọc latest user decision → AGENTS.md → docs/prototype-v1-module-network.md → docs/manager-checklist-v1.md → tài liệu này. Implementer đọc active V1 docs, chỉ làm phase được giao; Manager sở hữu acceptance/status/checklist/progression và Git checkpoint.

Phase bắt đầu mới đổi IN_PROGRESS; lỗi acceptance giữ REWORK; blocker ghi nguyên nhân cụ thể. Mỗi phase chỉ DONE sau independent source/diff/metas + Unity compile/Console/tests/runtime review. Chuyển Current Phase không tự cấp quyền bắt đầu phase tiếp theo; dừng sau từng phase cho user playtest.

Không Hub/Gas/Data/2/4/throughput/congestion/pressure/network capacity hoặc economy balance trong V1. Không advanced citizen mechanics, security/overcrowding penalties, Food Processing hoặc class-based diet. Scope đầy đủ và 11 playtest questions nằm trong spec/checklist.

## Phase 1 Initial Audit — 2026-09-06

Initial working tree clean on main at 398aa1055ce035ba2a81a822705c88fb7a8a0984. Unity live Editor đúng D:/TheHusk/Game/Husk, 6000.5.7f1 / URP 17.5.0, stopped/ready, SampleScene không dirty. Console baseline 4930; retained historical tool errors không bị clear.

| Existing part | Classification | Phase 1 direction |
|---|---|---|
| ResourceState validation/items, fresh session | REUSE / MODIFY | Giữ concrete item identity và validation; thêm Core/payment foundation tối thiểu |
| PrototypeCamera | REUSE / MODIFY | Giữ WASD/orbit/zoom/reset; hỗ trợ UI guard cho Phase 1 |
| V0 visual 3×3 floor, fixed building sites | REPLACE cho V1 representation | Module/grid/occupancy/adjacency/network domain thật; V0 scene giữ nguyên để regression |
| PrototypeHud / production panels | REUSE reference, presentation V1 riêng | Town Hall itemlist và Module configuration/supply inspection |
| Harbor/boat, Water/Recycler production | OUT-OF-SCOPE integration | Giữ working V0 loops; không networked production hoặc population trước Phase 2–3 |
| Existing tests | REUSE | Regression V0; thêm tests Module/payment/constraints/network |
| Blender base module/mask | OUT-OF-SCOPE import hiện tại | Source 20m/three lane IDs chưa Unity export; placeholder category lanes để cấu hình khác nhau vẫn khớp endpoints, không sửa art |

Audit này không phải acceptance; implementation/validation evidence được ghi sau khi Manager review.

## Last Completed Phase

V1 Phase 1 — Module Construction + Pipeline Network — DONE / PASS.

## Phase 1 Acceptance — 2026-09-07

Manager tự review source/diff, scene/metas và kiểm Unity; không dùng report Implementer thay acceptance. Đúng một husk_implementer thực hiện implementation. Tất cả 12 criteria Phase 1 PASS:

| Criterion | Evidence / result |
|---|---|
| Physical Module / coast / port | Native click chọn sea (1,0), (2,0), xây hai physical Modules gắn cạnh. Port (0,-2) hiện đỏ/lý do reserved, Build disabled; runtime API reject port và duplicate. Tests reject diagonal, remote, occupied và port từ hướng khác. PASS |
| Town Hall storage | Native click TownHall (-1,0) thấy từng concrete item và Module Core 100; global ResourceState unlimited, không storage slots/capacity. PASS |
| Atomic payment | Native builds: 100/100 Core/Wood → 99/90 → 98/80, 3 → 4 → 5 cells. Port/duplicate không đổi stock. Tests thiếu Core/Wood/both và observer atomicity PASS; không yêu cầu M tới site. |
| Provisional stock/time | Inspector startingWood=100 configurable; build immediate. Không coi đây là final balance. PASS |
| Exactly 3/4 + UI | Toggle F/W/M/E; 2/4 bị disable, 3/4 mới hợp lệ. Hai Module thử FWM và FWE; cell lưu explicit mask. PASS |
| Required INPUT + OUTPUT | Pure data/API kiểm union Harbor MF, Water EMW, Recycler EM, Solar E, House FW; tests từng building và optional pass-through PASS. Town Hall không suy diễn I/O/source. |
| Independent continuity | Bốn connected-component maps, orthogonal same-category only. Tests adjacency/sea/unsupported/no conversion và building pass-through PASS. |
| Connectivity vs supply | Manager runtime restore W path rồi tắt source: Connected=true nhưng W Unsupplied, F vẫn Supplied. Native bật W fixture phục hồi sáng. Tests alternate source/path và branch isolation PASS. |
| Cross-floor visual | Category lanes khớp endpoints, unlit bright supplied, 25% brightness unsupplied, absent unsupported. Native scene F/W sáng, E dim, M absent ở FWE. PASS |
| Configure / inspect feedback | Native đổi middle (1,0) FWM → FME: W lane tại middle biến mất, downstream W dim, F vẫn sáng; UI component/state/occupancy và lý do invalid đọc được. PASS |
| Tests / Editor / runtime | Manager chạy final EditMode 55 total / 55 passed / 0 failed / 0 skipped (36 regression + 19 Phase 1 cases), 0.71s. Play/Stop rồi Play lại: 100 Core, 100 Wood, 3 cells, source F/W và FWM preset phục hồi. PASS |
| Review / limits | Scene V1Phase1 saved, dirty=false, missing scripts=0; Editor ready/stopped, scriptCompilationFailed=false. 77 protected file hashes không đổi (Art/V0 scene/docs/config/packages/settings); git diff --check PASS. PASS |

Console từ baseline 4930: 0 implementation errors. Một warning 4939 “Files generated by test without cleanup” do tạo V1Phase1 scene khi Test Runner còn post-build cleanup. Đã inspect, khôi phục scene đúng và chạy lại suite; final cleanup hoàn tất (cursor 4944), không warning/error mới. Không clear Console để che evidence.

## Phase 1 Implementation / Files

- Reuse ResourceState concrete items và validation, PrototypeCamera WASD/orbit/zoom/reset, toàn bộ V0 production code/regression suite.
- Modify ResourceState thêm ModuleCore và atomic Module payment; PrototypeCamera thêm cached Phase 1 panel guard; ResourceStateTests cập nhật enum bounds/Core validation.
- New ModuleLayout (identity/occupancy/3-of-4/build/port), ModuleNetwork (independent revision-cached components/supply), ModuleNetworkPrototype (input/UI/runtime floor geometry/materials), ModuleNetworkTests, Editor/ModuleNetworkSceneSetup và metas.
- New Assets/Scenes/V1Phase1.unity + meta. V1 dùng logical Modules thay visual-only V0 grid; SampleScene/V0 production giữ nguyên. Không đổi Blender/art, engine/URP/packages hay generated files.
- Manager cập nhật duy nhất hai V1 checklist/status docs. AGENTS/spec giữ nguyên; câu planning-era “implementation chưa bắt đầu” trong AGENTS là lịch sử trước task này, operational status hiện tại nằm ở đây.

## Provisional Preset / Validation Limits

- Module size 6, Wood 100 configurable, Core 100 confirmed; build instant, post-build reconfigure free/instant theo explicit user authorization. Chưa chốt policy lâu dài.
- Palette configurable: F green RGB (0.35,1,0.32), W cyan (0.1,0.75,1), M orange (1,0.45,0.12), E violet (0.95,0.35,1); dim multiplier 0.25. Đây là placeholder, không final art.
- Fresh foundation chỉ có TownHall (-1,0), empty source Module (0,0), Harbor placeholder (0,-1), reserved port (0,-2). Không phải six-building starting city Phase 2.
- DEV SOURCE FIXTURE cấp boolean network state trên supported lanes, không tạo/debit concrete resources; production/repair/population/consumption chưa integrate. Town Hall I/O vẫn TBD.
- Building I/O foundation và alternate path/source được kiểm bằng EditMode tests; runtime Manager kiểm native construction/configuration/inspection/port, dùng MCP API bổ sung restore/source-off/duplicate/fresh-state assertions. Chưa build standalone player hoặc chạy Phase 2–4 acceptance. UI/geometry là dev prototype; fresh run reset, không thêm save architecture.

## User Playtest / Stop Boundary

1. Mở Unity 6000.5.7f1 project D:/TheHusk/Game/Husk, mở Assets/Scenes/V1Phase1.unity và Play.
2. Click TownHall để xem storage. Click sea sát Module (0,0) ở hướng (1,0), chọn F/W/M, Build Module; kiểm 99 Core / 90 Wood.
3. Xây tiếp (2,0) với F/W/E; kiểm 98 Core / 80 Wood. M absent, E dim, F/W sáng.
4. Chọn (1,0), bỏ W/chọn E, Apply configuration; W downstream dim, F vẫn sáng. Đổi về F/W/M để phục hồi.
5. Chọn (0,0), dùng hàng DEV SOURCE FIXTURE tắt/bật W để phân biệt mất nguồn và mất đường. Thử vùng PORT đỏ (0,-2): không xây được.
6. WASD di chuyển, giữ chuột phải orbit, wheel zoom, R reset camera. Stop/Play để reset layout/resources.

Overall ACTIVE; Phase 1 DONE; Current Phase 2 TODO. Dừng cho user playtest, chưa cấp quyền triển khai Phase 2.
