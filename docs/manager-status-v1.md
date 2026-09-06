# HUSK — Prototype V1 Manager Status

## Overall Status

ACTIVE

## Current Phase

V1 Phase 3 — House + Population + Consumption (TODO, chưa bắt đầu)

## Phase Status

| Phase | Name | Status |
|---|---|---|
| V1 Phase 1 | Module Construction + Pipeline Network | DONE |
| V1 Phase 2 | Networked Production + Starting City | DONE |
| V1 Phase 3 | House + Population + Consumption | TODO |
| V1 Phase 4 | Integrated Network Planning Playtest | TODO |

## Last Completed Task

V1 Phase 2 — Networked Production + Starting City, gồm corrective building/pipeline contract: PASS ngày 2026-09-07 (Asia/Saigon), sau independent Manager review, 84/84 EditMode tests và runtime acceptance.

V0 giữ Overall Status COMPLETE và Phase 0–7 DONE trong docs/manager-status.md; spec/checklist/status V0 là lịch sử giữ nguyên. Không chuyển evidence 36 tests V0 thành acceptance V1.

## Current Phase Goal

Phase 2 hoàn tất theo latest corrective task, dừng cho user playtest. Current Phase 3 vẫn TODO; chưa triển khai Build House, population hoặc consumption.

## Current Blockers

Không còn blocker cho corrective contract/Phase 2 checkpoint. Startup audit đã xác nhận House Disabled khi Water/Recycler Damaged; criterion Phase 2 về kiểm tra và báo rủi ro startup đã thực hiện. Chưa chứng minh target population 100/100 và không suy diễn user đã chốt cách cấp F/W ban đầu. Source/stock policy để đạt target đó vẫn là quyết định cần giải quyết trước phần population Phase 3.

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

Module building phải support và lock union INPUT+OUTPUT; Operational chỉ kiểm Inputs. Placement tự thêm required và bỏ optional deterministically để giữ 3/4, không popup. Optional slots vẫn editable/pass-through. Category khác concrete item: Fish thuộc F; Wood/Iron/Recyclable Material thuộc M và giữ identity.

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

Phase 3 integration cần chứng minh target starting 100/100 với F/W actual supply trong khi Water Plant/Recycler Damaged. Không invent Town Hall I/O hoặc fake supplied; startup source/stock policy vẫn mở, cần quyết định trước phần implementation phụ thuộc. Phase 2 chỉ nghiệm thu House supply state thực tế và báo giới hạn này.

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

V1 Phase 2 — Networked Production + Starting City — DONE / PASS, gồm corrective contract.

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

## Phase 2 Initial Audit — 2026-09-07

User đã playtest/chấp nhận Phase 1 và explicit giao riêng Phase 2. Precheck: working tree clean, main = origin/main tại 3c04e36c481d1ddfc953550d9d238365f20a51b6; docs ACTIVE / Phase 1 DONE / Phase 2 TODO trước khi chuyển IN_PROGRESS. Unity 6000.5.7f1, URP 17.5.0, V1Phase1 scene saved/clean, Editor ready/stopped, scriptCompilationFailed=false. Console baseline 4944; warning 4939 thuộc task trước, không clear.

| Existing system | Audit / direction |
|---|---|
| ModuleLayout / BuildingPipelines | REUSE explicit identity, occupancy, 3/4, required INPUT+OUTPUT, coast/port, atomic payment |
| ModuleNetwork | MODIFY nguồn fixture thành operational-source input cho Phase 2, giữ independent same-category components và Phase 1 fixture behavior |
| ModuleNetworkPrototype | REUSE floor lanes/selection/configuration/inspection, MODIFY để hiển thị real source/building state trong Phase 2 |
| ResourceState / Town Hall | REUSE concrete seven-item storage, Core 100, global construction payment; không thêm Town Hall source |
| FishingHarbor / BoatConstruction / FishingTrip | REUSE independent boats, clocks, arrival credit; MODIFY binding/gating để mất M không reset/duplicate unload |
| WaterProduction / RecyclerProcessing | REUSE timing và atomic Recyclable Material → Wood; thêm network gating và Damaged/Repair adapter. V0 không có Repair mechanic |
| V0 WaterPlant / Recycler panels / fixed sites | Presentation reference; không kéo fixed-site placement thành contract V1 hoặc duplicate production arithmetic |
| PrototypeSession / Camera / tests | REUSE storage baseline, camera and regression; thêm integration tests cho actual sources/failures/recovery |
| Art / Blender / engine / packages / V0 scene / V1Phase1 scene | Preserve, không nằm trong requested mutation |

Đúng một husk_implementer được giao Phase 2. Manager sở hữu docs, review/acceptance/Git. Chưa có Phase 2 acceptance PASS.

Startup question đã gửi user: House honest Disabled lúc fresh do Water/Recycler Damaged; 100/100 population target thuộc Phase 3 cần chốt lại khi tích hợp. Chưa thêm nguồn giả, Town Hall I/O, population hoặc consumption để né câu hỏi.

## Phase 2 Manager Review trước corrective — 2026-09-07 (historical)

Evidence trước latest corrective task: 10/11 criteria có evidence PASS, criterion startup được giữ pending lúc đó; chưa checkpoint. Bảng lịch sử dưới đây giữ nguyên hành vi trước fix, được thay bằng final corrective acceptance bên dưới. Manager đã chạy Unity EditMode: **77 total / 77 passed / 0 failed / 0 skipped**, 0.78s (55 regression + 22 Phase 2). Test Runner post-build cleanup hoàn tất tại Console cursor 4968. Console warn/error từ baseline 4944 đến 4968 rỗng.

| Phase 2 criterion | Manager evidence / assessment |
|---|---|
| Six starting buildings / storage | Runtime fresh 9 cells, đúng 1 TownHall/Harbor/Solar/House/Water/Recycler. Core100, Wood100; Water/Recycler Damaged; TownHall Storage/no output. PASS |
| Damaged / Repair | Native Repair Water trước: Wood100→95, timer5s, sau đó Disabled missing M, Water100. Native Repair Recycler: Wood95→90 rồi production. PASS |
| I/O / Module constraints | Source review + tests đủ MF, EMW, EM, E, FW; all9 configs hợp lệ3/4. PASS |
| Compatible path + real supply | Manager ngắt E bridge: Recycler/Water/Harbor Disabled, output M/W/F None, E downstream Unsupplied nhưng Solar E vẫn Supplied. Restore phục hồi. PASS |
| Concrete identity | Fish riêng/F binary availability; Wood/Iron/Recyclable không gộp. Runtime 2 boats unload → Fish110, Food100. PASS |
| Independent fishing | Native Build boat5s → House Operational, Fish chưa tăng trước arrival. MCP tạo boat thứ2, chạy2s rồi mấtM20s: boat1 giữ elapsed10.780238, construction giữ2s, Fish100. Restore3s → 2boats clocks13.780238/0; thêm30s → mỗi boat1 unload, Fish110; Tick0 không duplicate. PASS |
| Production / inputs | Native Recycler repaired+E → M/W supplied; snapshot Recyclable94/Wood93/Water107. Tests conversion/exhaustion/replenishment, no global-stock/fixture bypass, long delta vs partition PASS. |
| Downstream loss/recovery / alternatives | Native W bridge FWM→FME: House Disabled Missing W; F vẫn Supplied, W Unsupplied và lane dim. Restore → Operational. MCP M bridge WME→FWE: Water missing M/E Supplied, Recycler Operational, Harbor Disabled. Tests real alternate Solar/path giữ nhánh được cấp PASS. |
| House compatibility only | House states F+W → Operational, missingF/missingW → Disabled; no population/consumption/BuildHouse code. PASS |
| Startup100/100 vs damaged producers | **PENDING USER DECISION**. Fresh House Disabled trung thực, chưa có nguồn F/W. Không tuyên bố target100/100 đã đạt, không tự thêm source TownHall/storage bypass. |
| Manager runtime/compile/scene review | Manager independently77tests + native Repair/Build/configure + MCP assertions A–F, Console clean. Framing rework đã kiểm lại native tại994×708: đủ6buildings/port, Solar ngoài panel. Camera focus(0,0.5,0), size23. Final Editor ready/stopped, scene saved/dirty=false, scriptCompilationFailed=false, missing scripts0; Console4944→4968 không warning/error mới. PASS |

## Phase 2 Implementation / Provisional Values

- New NetworkedCity adapter reuse WaterProduction, RecyclerProcessing và BoatConstruction repair timer; dependency order E→M→W/F→House, không giữ output cũ để tự cấp nguồn. Supply binary chỉ trong compatible components; concrete quantity arithmetic vẫn ở existing production systems.
- FishingHarbor thêm external storage/network gating, giữ V0 independent boats/arrival logic; M loss pause clocks/construction rồi resume không catch-up. Sau corrective, M supplied kích hoạt F ngay cả khi chưa có boat; Fish quantity chỉ credit tại arrival, không chuyển Food stock.
- ModuleNetwork thêm operational endpoints riêng, Phase 2 ignore TestSupply. ModuleNetworkPrototype opt-in Phase 2, reuse build/configure/inspect/floor palette, bổ sung building feedback/repair/fishing actions. Editor setup tạo V1Phase2 scene; V0 và V1Phase1 scene giữ nguyên.
- Repair Water/Recycler riêng: 5 Wood/5s configurable; Water +1/2s; Recycler 2 Recyclable Material→1 Wood/4s. Boat build5s/trip30s/cargo5, Wood100/Recyclable100, instant Module build/free reconfigure, palette Phase1 đều provisional. Không balance hoặc production quantity allocation.
- Settings chưa final: repair cost/time, palette, lâu dài reconfigure, Module construction time, starting Wood, TownHall I/O và Phase3 population display/startup policy.
- Scene placeholder, chưa standalone player build. Stocks có thể cạn trong playtest; hết Recyclable thì WaitingForMaterial và nguồnM dừng đúng, Stop/Play reset.

## Phase 2 Layout / Playtest Draft

| Cell | Content | Pipelines |
|---|---|---|
| (-2,1) | Solar | FME |
| (-1,1) | E routing Module | FME |
| (0,1) | Recycler Damaged | WME |
| (0,0) | M routing Module | WME |
| (1,0) | Water Plant Damaged | WME |
| (0,-1) | Fishing Harbor | FWM |
| (1,-1) | W routing Module | FWM |
| (2,-1) | House | FWM |
| (-1,0) | Town Hall | FWM |

Port reserved (0,-2). Mở D:/TheHusk/Game/Husk bằng Unity6000.5.7f1, mở Assets/Scenes/V1Phase2.unity và Play. Click sàn Module để inspect. Repair Water trước để thấy thiếu M; Repair Recycler để cấp M/W/F và House Operational ngay cả khi chưa có boat. Chọn Harbor Build Fishing Boat để tạo concrete Fish tại arrival. Ngắt E ở(-1,1) FME→FWM, M ở(0,0) WME→FWE, W tới House ở(1,-1) FWM→FME; restore config để khôi phục. Để thử placement, chọn Module trống FWM rồi Place Solar (test): tự FWE, E locked; không popup. WASD/orbit/wheel/R; Stop/Play reset.

## Concurrent Working Tree Changes

Art là concurrent work ngoài Phase 2. Task này không ghi/stage/commit/discard Art. Trong corrective run, hash của một số untracked Art thay đổi bên ngoài task và có file mới xuất hiện; không tuyên bố toàn bộ Art byte-identical. 81 protected tracked files vẫn giữ nguyên hash, bao gồm tracked Art, V0/V1Phase1 scenes, V0 docs, engine/packages/settings và agent configuration.

Untracked Art tại final review: Art/Blender/HarborModule.blend, HarborModule.md, blockout_harbor_module.py, revise_harbor_flow.py; Art/Previews/HarborModule/{01_isometric.png,02_top.png,03_dock_front.png,04_side.png,05_rear.png,06_low_angle.png,07_adjacent.png,08_cargo_flow.png,09_waterside_hook.png,blockout-report.json,functional-flow-report.json}. Các file này được loại hoàn toàn khỏi checkpoint.

## Phase 2 Final Corrective Acceptance — 2026-09-07

**PASS / DONE.** Latest user task yêu cầu hoàn thiện current Phase 2 và checkpoint toàn bộ baseline + corrective, không triển khai Phase 3. Đúng một husk_implementer thực hiện corrective; Manager tự review toàn bộ runtime/editor/test/scene/meta diff và tự kiểm Editor. Root cause thực tế: resolver vốn đã dùng Inputs; gate completed boat chặn F, UI cho untick pending required và TryOccupy từ chối config thiếu required. Không tìm thấy union bị dùng làm operational supply check trong baseline đã audit.

| Phase 2 criterion | Final Manager evidence / result |
|---|---|
| 1. Fresh city / Town Hall | Fresh Play: 9 Modules, đúng sáu buildings; Core100/Wood100, TownHall Storage/no source/no lock requirement invented. TownHall inspection đã native-test trong baseline; ResourceState seven-item tests PASS. |
| 2. Damaged / Repair | Water/Recycler fresh Damaged/output None. Native Repair Water: Wood100→95, MCP advance5s → repaired nhưng Disabled missing M. Repair Recycler thêm5s: Wood90, cả hai hoạt động với actual inputs. PASS. |
| 3. I/O / union / lock / placement | Required/Locked MF, EMW, EM, E, FW đúng bảng. Tests năm buildings × bốn starting configs, legal/illegal optional edits, deterministic result và atomic notification PASS. Native F trên Harbor bị khóa không đổi pending/current FWM. Native Place Solar trên(1,-1) FWM→FWE, drop M, E locked, visual xuất hiện, không popup. PASS. |
| 4. Input-only operational | Sau Repair: Solar E, Recycler M, Water W, Harbor F đều active; House Operational, Harbor0boats/Fish100. Không producer nào cần pre-existing output supply. Ngắt E bridge làm Recycler/Water/Harbor Disabled và M/W/F output None; Solar vẫn E supplied. PASS. |
| 5. Concrete identity | Hai boat mỗi chiếc unload1: Fish100→110, Tick0 vẫn110, Food100. Wood/Recyclable/Water giữ riêng; runtime sau các bước có Water121/Recyclable70/Wood105. PASS. |
| 6. Independent fishing | Native Build boat rồi MCP advance5s; stagger boat thứ2. M loss20s giữ boat1 elapsed4s và construction2s/Fish100. Restore3s+30s: hai boats elapsed7/0, mỗi chiếc1 unload; không reset/catch-up/duplicate. PASS. |
| 7. Water / Recycler / Solar | Water repaired chưa M không tạo Water; E+M mới W active. Recycler repaired+E mới M active, vẫn cần concrete Recyclable. Tests conversion, exhaustion/recovery, atomic debit/credit, long240s vs960×0.25s, stock/fixture không bypass path PASS. |
| 8. Downstream / recovery | M bridge FWE: Harbor outputNone, Water missingM/outputNone, Recycler vẫnOperational. M bridge FWM: Water missingE/Msupplied, Recycler vẫnOperational. House branch FME chỉ mấtW; WME chỉ mấtF. Restore phục hồi. Native optional Solar FWE→WME: F lane House dim, W vẫn sáng, HouseDisabled. Tests alternate actual Solar/path PASS. |
| 9. House compatibility | Fresh missingF/W; đủ F/W Operational; mất riêng F hoặc W Disabled đúng nguyên nhân; outputNone. Không population/consumption/BuildHouse implementation. PASS. |
| 10. Startup audit / handoff | Đã kiểm và báo mâu thuẫn target100/100 với fresh Damaged producers. Không fake source hoặc invent TownHall I/O. PASS cho criterion kiểm tra/báo rủi ro Phase 2; **không phải PASS cho target100/100 Phase 3**, startup policy vẫn mở trước phần population phụ thuộc. |
| 11. Independent validation | Manager EditMode **84/84 passed, 0 failed/skipped/inconclusive**, duration0.16s (55 regression +29 Phase2 cases); post-build cleanup4981. Play/native UI + MCP assertions như trên; final stopped/ready, compileFailed=false, scene saved/dirty=false, missing scripts0. Full source/diff/metas review và whitespace check PASS. |

Console: MCP captured warn/error từ4968→4981 rỗng, không clear Console. Unity UI có **một tooling error** `Failed to handle /api/exec request: Thread was being aborted` khi eval gửi sát Play/domain transition; request trả network error, retry sau khi Editor sẵn sàng thành công và toàn bộ runtime tiếp tục. Lỗi này không xuất hiện trong captured cursor buffer nên không gọi Console hoàn toàn rỗng. Implementation-caused errors **0**, warnings mới **0**; warning4939 là history Phase1.

Final contract: RequiredModule=Inputs∪Outputs, Locked=RequiredModule từ current occupancy, Operational=Inputs-only cộng damage/explicit concrete conditions. Auto-config giữ config hợp lệ cũ; khi cần slot, giữ optional theo F/W/M/E, bỏ optional ưu tiên thấp từ E/M/W/F, không bỏ required. Exact priority là implementation detail. Optional edits vẫn phải đúng3/4; pending UI/API không bỏ required, domain reject vi phạm.

Checkpoint scope: ModuleNetworkSceneSetup.cs, FishingHarbor.cs, ModuleLayout.cs, ModuleNetwork.cs, ModuleNetworkPrototype.cs, NetworkedCity.cs+.meta, ModuleNetworkTests.cs, NetworkedCityTests.cs+.meta, V1Phase2.unity+.meta, prototype-v1-module-network.md, manager-checklist-v1.md, manager-status-v1.md. Đây là toàn bộ Phase2 + corrective chưa commit trước đó; không generated junk, Art hoặc Phase3. Commit message: `Complete V1 Phase 2 networked production`; destination origin/main, no force. Hash/push verification được report sau Git transaction.

Limits: scene vẫn placeholder; Solar placement là test surface free/instant, không building construction economy. Chưa standalone player build; chưa population quantity allocation. Phase2 DONE; Current Phase3 TODO, chưa bắt đầu. Dừng cho user playtest.
