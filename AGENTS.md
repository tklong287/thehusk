# HUSK — Agent Instructions

## Project

Husk là game city-builder / survival management lấy bối cảnh Trái Đất bị ngập nước.

Mục tiêu hiện tại không phải xây full game hoặc vertical slice lớn.

Prototype V0 và V1 đã COMPLETE. Active work hiện tại là Prototype V2 — Salvage, Material & Power: kiểm chứng shared Harbor/small boats, salvage material loop, local storage readability, factory productivity và shared power capacity/demand.

Repository root:
`D:\TheHusk`

Unity project root:
`Game/Husk`

## Technical Baseline

- Engine: Unity 6000.5.7f1
- Render pipeline: Universal Render Pipeline (URP) 17.5.0
- Application/editor code: C#
- Không tự đổi Unity version.
- Không tự đổi render pipeline.
- Không thêm third-party package nếu chưa có yêu cầu rõ ràng.

Trước khi sửa Unity code hoặc asset configuration, kiểm tra:
- `Game/Husk/ProjectSettings/ProjectVersion.txt`
- `Game/Husk/Packages/manifest.json`
- cấu trúc project hiện tại

Không giả định package hoặc system tồn tại nếu chưa kiểm tra.

## Unity Repository Rules

Không sửa hoặc commit các thư mục/file Unity-generated như:
- Library/
- Temp/
- Logs/
- obj/
- UserSettings/
- .vs/
- generated .csproj
- generated .sln / .slnx

Không sửa file IDE-generated.

Không xóa hoặc làm mất `.meta` file.

Khi di chuyển Unity asset:
- phải giữ/move `.meta` tương ứng.

Runtime code không được đặt trong thư mục `Editor`.

Code chỉ dùng trong Unity Editor phải nằm trong thư mục `Editor`.

Không thêm package hoặc thay đổi render pipeline chỉ để giải quyết vấn đề có thể xử lý bằng Unity/package hiện tại.

## Engineering Principles

Ưu tiên theo thứ tự:

1. Correctness
2. Playable iteration speed
3. Readability
4. Architecture vừa đủ cho prototype

Không over-engineer.

Không tạo abstraction, framework, service layer hoặc extensibility chỉ vì có thể cần trong tương lai.

Không refactor unrelated code trong khi đang thực hiện một task cụ thể.

Không mở rộng scope bằng các feature "tiện thể".

Gameplay constants có khả năng thay đổi phải dễ chỉnh và không được rải magic number tùy tiện khắp code.

Tránh:
- scene-wide searches trong hot path;
- allocation không cần thiết mỗi frame;
- GetComponent/Find lặp lại mỗi frame nếu có thể cache reference.

## Active Prototype — V2: Salvage, Material & Power

V0 và V1 là completed history. Không rewrite `docs/prototype-v0.md`, `docs/manager-checklist.md`, `docs/manager-status.md`, `docs/prototype-v1-module-network.md`, `docs/manager-checklist-v1.md`, hoặc `docs/manager-status-v1.md` trừ khi user/Manager explicit yêu cầu sửa historical docs.

Source priority cho active V2 work:

1. User's latest explicit decision.
2. `AGENTS.md` — project/engineering rules.
3. `docs/prototype-v2-salvage-material-power.md` — active design specification.
4. `docs/manager-checklist-v2.md` — phase acceptance.
5. `docs/manager-status-v2.md` — operational state.

V1 systems là baseline để reuse/integrate, không phải scope để rewrite.

## Confirmed V2 Rules

- Một shared Harbor phục vụ small boats; không tách Fishing Port và Salvage Port theo resource.
- Fishing Boat và Salvage Boat dùng cùng Harbor workflow/build/dock/dispatch/receive foundation.
- Salvage Boat trả về Scrap Metal + Scrap Wood.
- Processing dùng salvage để tạo usable construction materials; ưu tiên compatibility với existing Iron/Wood identities, không broad rename nếu không cần.
- Local storage là buffer gần consumer/factory; progress bar biểu diễn local storage ratio, arrow biểu diễn xu hướng up/down/neutral.
- House/Citizen trong V2 chỉ hiển thị Food + Water local-storage bars/trends. Không total Satisfaction/Happiness trong V2.
- Factory hiển thị Productivity % + local-storage bars/trends cho required material inputs.
- Power là capacity/demand, không phải concrete stock item.
- Powered building bật thì demand = rated kW; tắt thì demand = 0. Không cần separate idle-power state trong V2.
- `TotalPowerDemand = sum(enabled rated kW)`.
- `PowerEfficiency = min(1, TotalPowerCapacity / TotalPowerDemand)`, zero demand = 100%.
- Factory productivity lấy bottleneck thấp nhất giữa PowerEfficiency và required material input satisfaction.
- Production interval tăng khi productivity giảm; 0 productivity phải stop an toàn, không NaN/divide-by-zero.
- Exact rates/capacities/timings/conversion ratios/rated kW là configurable/provisional nếu user chưa chốt.

## V2 Scope and Phase Discipline

Chỉ thực hiện phase hiện tại được giao trong đúng bốn phase lớn:

1. Harbor + Salvage Loop.
2. Processing + Local Storage.
3. Power Capacity + Demand.
4. Integrated V2 Playtest.

Manager sở hữu acceptance/progression và dừng sau mỗi phase để user playtest. Không tự implement phase sau; chuyển Current Phase không tự cấp quyền bắt đầu code.

Không thêm trong V2 nếu chưa explicit reopen:
- citizen Satisfaction/Happiness score;
- Calories/Protein/Vitamins;
- Diet Diversity hoặc class food expectations;
- luxury food;
- mortality/migration/health/security/politics;
- detailed logistics vehicles;
- batteries/voltage/distance loss/power-priority automation;
- final economy balance/art;
- future-scale frameworks không cần cho acceptance hiện tại.

## V1 Baseline to Preserve

- Unity/network/module foundation và V1 accepted behavior đang là regression baseline.
- V1 complete status nằm ở `docs/manager-status-v1.md`.
- Không dùng rule planning-era đã được latest V1/V2 decision thay thế để chặn current work.
- Reuse working production/boat/resource/network/UI code khi phù hợp; integrate nhỏ nhất thay vì rewrite.

## Decision Rules

Nếu vấn đề là implementation detail không ảnh hưởng đáng kể đến player experience:
- chọn giải pháp nhỏ nhất;
- dễ thay đổi;
- phù hợp architecture hiện tại.

Nếu vấn đề yêu cầu quyết định game design mới:
- không tự biến assumption thành canon;
- báo rõ assumption/blocker;
- hỏi người dùng hoặc Manager.

Nếu một gameplay value chưa được chốt:
- giữ configurable;
- ghi rõ nó là provisional.

## Validation

Sau mỗi implementation task:

1. Kiểm tra diff.
2. Chạy test/validation phù hợp nếu project hiện hỗ trợ.
3. Kiểm tra Unity compile errors nếu có thể.
4. Không báo hoàn thành chỉ vì code đã được viết.
5. Báo rõ:
   - files changed;
   - validation đã chạy;
   - kết quả;
   - assumptions;
   - blockers.

## Scope Discipline

Agent chỉ được thực hiện task hiện tại.

Không tự bắt đầu phase tiếp theo.

Không tự thêm feature để "chuẩn bị cho tương lai".

Một implementation nhỏ, rõ, chơi được và dễ sửa tốt hơn một architecture lớn chưa được chứng minh cần thiết.
