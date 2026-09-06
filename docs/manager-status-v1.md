# HUSK — Prototype V1 Manager Status

## Overall Status

ACTIVE

## Current Phase

V1 Phase 1 — Module Construction + Pipeline Network

## Phase Status

| Phase | Name | Status |
|---|---|---|
| V1 Phase 1 | Module Construction + Pipeline Network | TODO |
| V1 Phase 2 | Networked Production + Starting City | TODO |
| V1 Phase 3 | House + Population + Consumption | TODO |
| V1 Phase 4 | Integrated Network Planning Playtest | TODO |

## Last Completed Task

Documentation transition — 2026-09-06: V1 specification chuẩn hóa từ draft theo latest explicit user decisions; V1 checklist/status và Implementer instructions chuyển sang active V1. Không phase implementation V1 nào đã hoàn thành hoặc bắt đầu.

V0 giữ Overall Status COMPLETE và Phase 0–7 DONE trong docs/manager-status.md; spec/checklist/status V0 là lịch sử giữ nguyên. Không chuyển evidence 36 tests V0 thành acceptance V1.

## Current Phase Goal

Khi có task implementation riêng: Module construction + pipeline network foundation như một task coherent; sau Phase 1 nghiệm thu, dừng cho user playtest. Chưa tự chạy Implementer hoặc Unity implementation trong task documentation.

## Current Blockers

None blocking Phase 1.

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
- Có đổi pipeline configuration sau build không; cost/time thay đổi.
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

## Current Validation Boundary

Task hiện tại chỉ documentation/Manager state/Implementer instructions; không đổi Unity/gameplay/scene/assets/packages, không spawn Implementer, không sửa .codex/config.toml. Các phase và runtime acceptance V1 đều TODO. Không chạy lại Unity tests để chứng minh documentation transition.
