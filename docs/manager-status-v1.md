# HUSK — Prototype V1 Manager Status

## Overall Status

COMPLETE

## Current Phase

V1 Phase 4 — Integrated Network Planning Playtest (DONE; V1 COMPLETE)

## Phase Status

| Phase | Name | Status |
|---|---|---|
| V1 Phase 1 | Module Construction + Pipeline Network | DONE |
| V1 Phase 2 | Networked Production + Starting City | DONE |
| V1 Phase 3 | House + Population + Consumption | DONE |
| V1 Phase 4 | Integrated Network Planning Playtest | DONE |

## Last Completed Task

V1 Phase 4 — Integrated Network Planning Playtest: PASS ngày 2026-09-07 (Asia/Saigon), sau independent Manager source/diff review,117/117 EditMode tests,runtime/native UI và actual script reload. Last Completed Phase: V1 Phase 4.

V0 giữ Overall Status COMPLETE và Phase 0–7 DONE trong docs/manager-status.md; spec/checklist/status V0 là lịch sử giữ nguyên. Không chuyển evidence 36 tests V0 thành acceptance V1.

## Current Phase Goal

Phase4 đã hoàn tất, V1 COMPLETE. Dừng cho user playtest; không bắt đầu prototype hoặc phase mới.

## Current Blockers

Startup blocker RESOLVED bởi latest explicit user decision: City Hall special4/4, F/W/M storage IN/OUT từ stock thật; E pass-through only. Mọi concrete storable item hiện có bắt đầu100, không Electric item. F/W paths từ City Hall cấp starting House100/100 trong khi Water/RecyclerDamaged. Không còn blocker đã biết; Phase4 implementation/validation đã PASS.

Open questions bên dưới phải được giữ mở; nếu exact workflow/acceptance sau này phụ thuộc câu trả lời, báo Manager/user trước phần việc phụ thuộc. Không đánh dấu phase DONE dựa trên assumption chưa được phép.

## Confirmed V1 Rules / Test Values

| Nội dung | Confirmed V1 value |
|---|---|
| Standard Module | Đúng 3/4 categories |
| Categories | F = Food, W = Water, M = Material, E = Electric |
| Starting Module Core | 1 stack 100 Module Core |
| Module build cost | 1 Module Core + 10 Wood từ global city Item Storage |
| Placement | Attach edge hiện có tiếp giáp biển; cấm occupied Module hoặc Fishing Harbor/port edge/vị trí |
| City Hall | 1 special4/4, F/W/M stock IN/OUT, E pass-through only; mọi concrete item100, không Electric item |
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

Standard Module building phải support và lock union INPUT+OUTPUT; Operational chỉ kiểm Inputs. City Hall là exception special4/4 fixed, E không phải output. Placement tự thêm required và bỏ optional deterministically để giữ 3/4, không popup. Optional slots vẫn editable/pass-through. Category khác concrete item: Fish thuộc F; Wood/Iron/Recyclable Material thuộc M và giữ identity.

Required input phải có continuous compatible path + actual supply; thiếu bất kỳ input nào → Disabled/Unsupplied. Repair không bypass network. House Disabled giữ residents, coi là reallocated sang Operational Houses; effective capacity giảm nhưng population và compound growth tiếp tục kể cả overcrowded/capacity = 0. Không detailed assignment hoặc penalty.

Visual: floor cross mỗi Module, F/W/M/E phân biệt được; supported-supplied sáng, supported-unsupplied dim, unsupported inactive. Branch failure/alternate source và recovery phản ánh actual topology/supply, không chỉ debug overlay.

## Provisional / Reuse Values

Boat build khoảng 5s, trip khoảng 30s, unload 5 Fish, nhiều boat độc lập và auto repeat là baseline reuse configurable/provisional. Water/Recycler rates và test stocks ngoài Module Core dùng current/provisional values nếu phù hợp, không final balance. Fresh stocks đã chốt100 mỗi concrete item hiện có; không Electric item.

## Pending User Decisions / Open Questions

- Exact Water Plant repair cost/time.
- Exact Recycler repair cost/time.
- Policy reconfigure cho các phase sau/full game: vẫn mở; Phase 1 dùng free/instant provisional theo explicit authorization.
- Exact F/W/M/E colors.
- Module construction time.
- Starting stocks đã chốt100 mỗi concrete storable item; không còn open question.
- City Hall đã chốt special4/4; F/W/M storage IN/OUT, E pass-through only.
- Population rounding/display.
- Future overcrowding/Trị an formula.
- Future food preferences.
- Future 2/4 vs 3/4 sau playtest.

Phase3 phải kiểm fresh100/100 bằng City Hall stock-backed F/W và actual topology; Water/Recycler vẫn Damaged. Các ghi chú startup pending trong Phase2 bên dưới là historical và đã được quyết định mới thay thế.

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

## Phase 3 Initial Audit / Design Resolution — 2026-09-07

Baseline main=origin/main tại36d549aef2e6cf707ef904a22d85887125aae055, không tracked changes trước task; known untracked Art được latest instruction tiếp tục từ hiện trạng cho phép giữ ngoài checkpoint. Không stash/reset/stage Art. Snapshot Art56files được lưu ngoài repository để kiểm preservation.

Đã reuse đúng một husk_implementer từ read-only startup audit. Manager independently xác nhận Unity MCP baseline HouseDisabled/missingF,W trong khi Water/RecyclerDamaged. User sau đó explicit quyết định City Hall special4/4 + starter stock-backed supply, giải quyết blocker; active spec được cập nhật trước implementation. Latest decision có priority trên câu Town Hall TBD/starting Wood TBD còn trong AGENTS hoặc acceptance history Phase1–2.

Audit: ResourceState có7 concrete items (Food,Water,RecyclableMaterial,Wood,Iron,Fish,ModuleCore), không Electric. Giữ item identity và default100 cho cả7. ModuleLayout đã có atomic occupancy/auto-config/locked union; NetworkedCity đã có real producer endpoints, Damaged/Repair và input-only gating; House mới là supply state chưa capacity/population/BuildHouse UI. Reuse những phần này, bổ sung Phase3 có opt-in để giữ baseline scenes/tests. Engine6000.5.7f1/URP17.5.0 giữ nguyên.

Phạm vi: special City Hall và stock-backed F/W/M, E pass-through; House placement/capacity; city-level double population/growth; fractional consumption once-per-city có network gating; UI và scene/test evidence. Không Phase4, battery/Electric storage, Hub, citizen assignment, penalties, quantity routing hay final balance. Đây là audit đầu task; evidence nghiệm thu cuối cùng nằm bên dưới.
## Phase 3 Final Manager Acceptance — 2026-09-07

**PASS / DONE.** Đúng một husk_implementer được reuse và bàn giao; Manager tự review implementation, scene/metas, chạy lại suite và kiểm runtime/UI. Active spec đã được cập nhật trước implementation theo quyết định City Hall mới. Không bắt đầu Phase4.

| Criterion | Independent Manager evidence / result |
|---|---|
| City Hall / storage / topology | Fresh special All (4/4 locked), 7 concrete items đều100, không Electric item. Hall chỉ F/W/M source từ stock; E supplied qua Solar. MCP cắt E bridge: Hall E Supplied→Unsupplied, Solar vẫn Supplied. Tests actual deposit paths, stock depletion, identity, no free output/catch-up PASS. |
| Build House | Runtime xây hai Modules (2,-2) FWM và (2,-3) FME: Core100→98, Wood100→80. Native Build House ở(2,-3) tự FWM, F/W locked; native click W không bỏ được. Capacity100→200, current121 không đổi. Tests bốn configs/occupied/sea/third pass-through PASS. |
| Fresh population / capacity | Independent MCP fresh model và live scene:100.0/100, sáu buildings trên10Modules, Water/RecyclerDamaged, starting HouseOperational. Tất cả7 stocks=100 trong fresh model; initial scene còn100population. |
| Compound growth | Live Advance60→110.0/100, thêm60→121.0/100; sau all-housing path loss thêm60→133.1/0. Đã thêm10000Fish/Water trong Play test để cô lập growth khỏi depletion; không lưu debug stock vào scene/defaults. |
| Uncapped growth | 121 vượt capacity100 vẫn tăng. Khi cap0,133.1 tiếp tục146.41 (display146.4); không penalty, không mất residents hoặc thêm dân khi xây House. Tests180s=133.1 và overshoot PASS. |
| Fractional demand | Hai Houses Operational, population133.1: advance1s trừ6.65499999999 Food và Water, đúng demand6.655 mỗi loại một lần toàn city. Fish-first rồi legacy Food; tests fractional/tiny frames/atomic/no-negative PASS. |
| Independent inputs | Router(2,-2) FWM→FME: chỉ House mới Disabled/missingW; House gốc Operational, capacity200→100, population121 giữ nguyên. Restore FWM→200. Tests mất riêng F/W vẫn giữ state loại còn lại PASS. |
| Reallocation | MCP model starting180, hai Houses:180/200; cắt W một House→180/100. Model cả hai mấtWater→180/0, Tick60→198/0. Live all-House route loss cũng giữ current và tiếp tục growth. |
| UI / resource consistency | Native Game View Build House/Hall/HUD đã inspect: current/effective capacity,2/2Houses,demand,stock,required locks và F/W state nhất quán. Khi stockF/W cạn Hall chỉ outputM, cap0; restock100Fish/Water→cap200, stock không âm. |
| Tests / debit policy | Manager Unity MCP EditMode112/112 PASS,0failed/skipped/inconclusive,1.74s; gồm84 regression +28Phase3 cases. Large/tiny tick partitions, fresh reset, production deposit pause/resume và independent boats đều PASS. |
| Review / Unity | Full runtime/editor/test/scene/meta review; Unity6000.5.7f1, compileFailed=false, missing scripts0, scene saved/dirty=false; Play→Stop thành công. Console warn/error mới từ4997→5005 rỗng; Implementer interval4981→4997 cũng rỗng, không clear Console. Whitespace diff check PASS. |

### Implementation / provisional choices / limits

CityPopulation giữ double current và growth; ResourceState giữ exact double quantities, legacy integer API dùng whole items. Quantum0.05s giữ remainder, tolerance1e-7s cho float boundary; display1decimal configurable, không round simulation. House capacity100; growth10%/60s; demand0.05 mỗi loại/resident/s. Build House free/instant là provisional test workflow.

Demand được tính từ toàn bộ population, debit một lần khi có ít nhất một Operational House; cap không giới hạn residents. Khi không House nào đủ cảF/W, không debit và không tích debt; live60s all-disabled giữ nguyên Food/Water. Consumption trước production mỗi quantum; delivery dùng được từ quantum kế tiếp. Fish ưu tiên rồi legacy Food (item đã tồn tại), không tạo generic Material/Electric. Khi thiếu output route về Hall, producer/boat clocks pause, giữ progress và resume không catch-up. Output stock không là activation prerequisite của producer.

Scene V1Phase3 opt-in, thêm routerFWM(-1,-1) cho continuous starterF/W từHall. Historical V0/V1Phase1/V1Phase2 scene và behavior regression được giữ. Scene vẫn placeholder; chưa standalone player build. Với stock100/rates hiện tại, Water có thể cạn sau khoảng20s nếu chưa có replenishment; không rebalance/penalty hoặc free resources để kéo dài playtest. Không battery/Hub/quantity logistics/advanced citizens/Phase4.

### Playtest handoff

Mở Game/Husk/Assets/Scenes/V1Phase3.unity và Play. Fresh100/100; click Hall xem special4/4 và7stocks. Chọn Module trống rồi Build House: requiredF/W tự thêm/lock. Xây nhánh(2,-2) rồi(2,-3), đặt House ở cuối; đổi router(2,-2) sangFME để cắt riêngW rồi restoreFWM. Đổi bridge(1,-1) sangFME để cắtW cả nhánh, quan sát capacity0/population vẫn tăng; restore không tạo lại residents. Native UI và MCP simulation advancement đã được Manager dùng để kiểm nhanh các phút growth. Stop/Play reset toàn bộ runtime test changes.

### Checkpoint scope / preservation

11 implementation files: ModuleLayout.cs, ModuleNetworkPrototype.cs, NetworkedCity.cs, ResourceState.cs, CityPopulation.cs+.meta, CityPopulationTests.cs+.meta, ModuleNetworkSceneSetup.cs, V1Phase3.unity+.meta; cộng3 activeV1docs. Stage explicit14paths, inspect staged diff và git diff --cached --check trước commit. Commit message: Complete V1 Phase 3 population and housing; push origin/main, không force. Hash/push được báo sau transaction.

Art56files và81protected tracked files giữ nguyênSHA256 so với snapshot trước task. Không stage/commit/untrack/discard Art;15untracked Art files vẫn ngoài checkpoint. Không engine/package/ProjectSettings/generated/IDE changes. OverallACTIVE; LastCompletedPhase3DONE; CurrentPhase4TODO chưa bắt đầu. Dừng cho user playtest.

## Post-Phase 3 tuning — Water Plant

Theo yêu cầu user, active V1Phase3 Water Plant tạo20 Water mỗi2s (10/s,600/phút). Scene preset và menu tạo lại Phase3 dùng cùng giá trị; Phase4 vẫn TODO, chưa bắt đầu. Các mốc1 Water/2s ở acceptance history phía trên là rate trước thay đổi này.

Validation: Unity MCP dùng rate đọc từ scene, sau construction hai chu kỳ2s đều credit20; compileFailed=false. Console trước validation có lỗi NullReferenceException tại ModuleNetworkPrototype.RefreshVisuals:190 từ phiên play trước; không thuộc thay đổi rate này. Không chạy lại full suite cho config-only tuning.

## Phase 4 Initial Audit

User đã giaoPhase4 từ current state. Baseline05edfeb63459c5a4d4fe2c91f522c9d1922bb3ac;3tracked modifications là Water20/2s tuning đã được user giao (scene,scene factory,status), được giữ trong checkpoint này. Art56files snapshot riêng ngoài repository, không stage/discard. Reuse đúng một husk_implementer; Manager owns docs/Git/acceptance. Kiểm lifecycle RefreshVisuals NullReference history và integrated scenarios, không giả đạt bằng code-only.

## Phase 4 Manager Playtest Evidence

Manager dùng V1Phase4 Play Mode, Time.timeScale=0 và AdvanceSimulation để kiểm các bước chính xác; không tăng starter stock trong các scenarios dưới đây. Native UI qua computer-use được dùng cho Repair Water, Build House, Restart và camera scroll. MCP dùng cho configuration/build/advance/assertions; không coi ảnh capture bị stale là evidence trạng thái domain.

| Scenario | Independent evidence |
|---|---|
| Fresh / stock / port / payment | Native Restart rồi MCP:100/100,10Modules,sáu buildings,7concrete stocks100,Water20/2. Build ởport(0,-2) bị từ chối; hai Modules(2,-2),(2,-3) debit2Core/20Wood. |
| Repair / arithmetic | Native Repair Water debit5Wood; RepairRecycler debit5Wood. Advance5: cả haiOperational,Wood90/Water75. Thêm4s:2Watercycles +40Water, consumption20Water→95;Food155,Recycler1cycle→Recyclable98/Wood91. |
| House / pass-through | Native Build House(2,-3) từFME tựFWM+FWlocked, population100 không đổi,capacity100→200; optionalM Supplied. |
| Water branch failure | Router(2,-2) FWM→FME:House mớiDisabled/missingW,House gốcOperational,capacity100;F vẫnSupplied. W floor material dimRGBA(.025,.188,.25,1), bằng25% palette. RestoreFWM→capacity200. |
| Water alternate path | Bridge(1,-1) mấtW→cả haiHousecap0. Xây ModuleFWM(2,0) từWaterPlanttớiHouse→capacity200 mà bridgevẫnthiếuW;debit1Core/10Wood. |
| Independent Food | Bridge(1,-1) đổiWME:F tớiHouseUnsupplied,W vẫnSupplied từalternate path,cap0. RestoreFWM→cap200. |
| Electric / storage distinction | CắtEbridge(-1,1):Water/RecyclerDisabled,WateroutputNone,HallEUnsupplied,SolarvẫnOperational. HallW vẫnSupplied từstock thật. PlaceSolar(2,0) tựFWE→Ealternate cấp lạiWater/Recycler/Hall. |
| Real boats / growth / depletion | Build5s hai boatstaggered; qua120simulationseconds có6unloads,2boats độc lập;pop100→110→121 dùFoodcạn/cap0. Tại120s:Water1009.1,Food0,Wood88,Recyclable44. Không reset hoặc injectstock để giữsupply. |
| Food recovery / no hard lock | TừFood0/cap0, advance từngquantum tớiarrival12s:unloadthứ7 credit5Fish,Food5,capacity0→200. Boat vẫn chạy khihousingdisabled. Stockdepletion không khóa permanently build/repair/reconfigure. |
| Restart | NativeRestart từ13Modules/2Houses/2boats/121population phục hồi10Modules/1House/0boats/100population/stock100,hai producersDamaged. Lifecycle vàcamera final verification được ghi khi rework kết thúc. |

### 11 playtest questions — Manager assessment

1. **Placement có consequence không?** Có. Router(2,-2) thiếuW chỉ tắtHouse cuối nhánh; bridge(1,-1) ảnh hưởng cả nhóm. Cần đường liên tục, globalstock không bypass.
2. **Player có phải nghĩ pipeline nào giữ không?** Có. House khóaFW,slotM/E còn lại quyết định pass-through; Solar tại(2,0) giữFWE cung cấp alternateE nhưng bỏM. Quyết định có hậu quả kiểm được.
3. **3/4 có quá dễ không?** Chưa đủ evidence kết luận. Trong preset nhỏ, thiếuW có thể sửa bằngmộtModule(2,0),nhưngrequired building vàlimited3slots vẫn tạo tradeoff. Không đổi rule từmộtplaytest.
4. **Grid routing có khiến failure dễ bypass không?** Có tronglayoutnày: một đường vòngW hoặc Solar test bổ sung bypass được điểmngắt. Đây là quan sátpreset/freeplacementtest, chưa chứng minh mọifailuretronglayoutlớn đều dễ.
5. **Có nên thử2/4 ở prototype sau không?** Có thể là thí nghiệm sau khiuserchốt,mụcđíchso độkhó/routing space. Phase4 chưa cung cấp đủlýdo thay3/4 và không triểnkhai2/4.
6. **F/W/M/E có readable không?** Màu vàbright/dim phân biệt đượcởcamera play;Wdim trênHouse mấtnguồn được kiểm cảmaterialstate. Cầnzoom/inspect khi nhãn nhỏ hoặcnhánh dài; exactpalette/art vẫnprovisional. UIoverflowtitlephát hiện trongManagerreview và được rework riêng.
7. **Chẩn đoán broken supply bằng mắt được không?** Có với vị trí nhánh bịngắt: lane mất/dim vàHouseDisabled đi cùng nhau. Cầninspect MissingInput vàstockđểphânbiệtstockcạn vớiđứtpath; màuđơnlẻ không mô tả nguyênnhân đầyđủ.
8. **Topology tạo districts tự nhiên không?** Bắtđầu có nhómproductionE/M vànhánhHouseFW khác nhau. Mới10–13Modules, chưa đủ đểkếtluận districts tự nhiênởscale lớn.
9. **Có bớt FarmVille/isometric land grid không?** Vềsystemcó: vị trí thayđổikhả nănghoạtđộng vàinfrastructure xuyênModule. Visualplaceholder vẫn làcubestrêngrid; chưa chứng minh art/feelmegaship hoànchỉnh.
10. **Build Module có cảm giác lắp ráp megastructure không?** Edgeattachment,cost vàlanesnốithêm cótínhiệu đó. Instantplacement vàcubeplaceholder còntrừutượng; không tựthêmanimation/finalart trongphaseintegration.
11. **House/population demand làm routing meaningful chưa cầnbalance không?** Có: supplyloss giảmcapacity nhưnggiữdân/demand;reconnect/boatarrival phục hồi. Foodrate5Fish/30s thấp hơn demand100dân,runtime dài bịdepletionchi phối. Ghi nhận limitation thay vì tựrebalance;Water20/2 làlatestuserdecision.
## Phase 4 Final Acceptance — PASS / DONE

Toàn bộ11criteria được nghiệm thu dựa trên bảng runtime và11playtest answers ở trên. Phase4 đã kết thúc; OverallV1 COMPLETE, cả4phases DONE. Không có phase tiếp theo được bắt đầu.

- **Source / diff / assets:** Manager review code lifecycle, owned root/material cleanup, scene factory,5integration tests và scene/metas. Runtime giữ graph/production hiện có; scenePhase4 bật integrated UI, stocks100 vàWater20/2. Water tuning Phase3 theo user được bao gồm trong checkpoint. Không thêm hệ thống mới.
- **Tests:** Manager Unity MCP EditMode117/117 PASS,0failed/skipped/inconclusive,1.93s; Implementer117/117,2.4s. Rework cuối chỉ rút gọn title UI; không đổi logic simulation hoặc camera nên không chạy lặp toàn suite.
- **Actual reload:** Manager tự tạo100/200 trong Play rồi RequestScriptReload: về100/100,10Modules,1runtimeRoot,1Harbor,0boats,14ownedMaterials. UI báo rõ scripts reloaded/fresh restart. Implementer trước đó cũng kiểm reload và restart lặp không duplicate. Đây là development reset, không persistence/save feature.
- **UI rework:** Title dài tạo horizontal scrollbar đã được Manager phát hiện và Implementer sửa thành HUSK V1 / PHASE 4. Manager xem lại native screenshot sau reload: không còn scrollbar ngang. PORT / KEEP CLEAR mô tả vùng cấm xây; consumption active/paused phân biệt nhu cầu với debit thực tế.
- **Camera:** Native scroll được Manager nhận: size23→23.05. Native Sky key R không làm Unity nhận reset trong lần kiểm; không báo native R PASS. Implementer kiểm chính PrototypeCamera.Update bằng synthetic Unity Input System events: R23.05→23, W dịch0.05210828, RMB/mouse delta orbit4.471968°, R phục hồi vị trí(-19.19,37.66,-27.41). Không thấy lỗi logic camera, không thay code camera.
- **Compile / Console:** Final compileFailed=false, missing scripts0. Console retained historyNRE tới9052 và tooling transition timeout9062; interval9062→9067 không warning/error mới, Manager final reload cũng không thêm lỗi. Không clear Console hoặc gọi history sạch. Editor cuốiSTOP, V1Phase4saved/dirtyfalse, prototype component children0 ngoàiPlay; timeScale1/runInBackgroundfalse được trả lại.
- **Preservation:** Art56files SHA256 không đổi;81protected tracked files không đổi. V0/V1Phase1/V1Phase2 scenes, V0 docs, engine/packages/settings giữ nguyên.Final review có26untracked Art files:15Harbor baseline và11House files do concurrent work tạo trongtask.56Artfiles baseline vẫn nguyênhash; tất cảArt giữ ngoài checkpoint. Không stage generated/IDE files.

### Final handoff / limits

Mở Game/Husk/Assets/Scenes/V1Phase4.unity và Play. Repair Water/Recycler (5Wood/5s mỗi loại), xây Modules/House, thử ngắt và nối lại F/W/E. Nhánh mẫu(2,-2)→(2,-3), alternateW qua(2,0). Dùng Restart fresh run để bắt đầu lại toàn bộ preset khi playtest; script reload cũng reset và thông báo rõ.

Water20/2s là quyết định user; boat5Fish/30s, repair5Wood/5s, freeinstant reconfigure/House/Solar test placement, display1decimal vàquantum0.05s vẫn là preset/provisional ngoài các giá trị đãconfirmed. Dân vẫn tăng khi cap0; demand không mất, debit dừng khi không có HouseOperational. Food có thể cạn nhanh hơn boats cấp; boat arrival vẫn phục hồi housing tạm thời, không hard-lock. Không dùng developer reset để thay bằng chứng repair/production recovery. Layout nhỏ chưa chứng minh final balance, art, districts hoặc game feel ởscale lớn; chưa standalone player build.

Checkpoint gồm10files: ModuleNetworkPrototype.cs, ModuleNetworkSceneSetup.cs, IntegratedPlanningTests.cs+.meta, V1Phase4.unity+.meta, V1Phase3.unity (Water tuning), và3activeV1docs. Stage explicit paths, review staged content và git diff --cached --check trước commit. Commit: Complete V1 Phase 4 integrated network playtest; push origin/main khôngforce. Hash/push verification được báo sau transaction. DỪNG.