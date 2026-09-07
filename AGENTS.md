# HUSK — Agent Instructions

## Project

Husk là game city-builder / survival management lấy bối cảnh Trái Đất bị ngập nước.

Mục tiêu hiện tại không phải full game hoặc vertical slice lớn.

Prototype V0 và V1 đã COMPLETE. Active work hiện tại là **Prototype V2 — Salvage, Material & Power**: shared Harbor/small boats, salvage material loop, local storage readability, factory productivity và shared electric capacity/demand.

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
- cấu trúc project hiện tại.

Không giả định package/system tồn tại nếu chưa kiểm tra.

## Unity Repository Rules

Không sửa hoặc commit Unity-generated/IDE-generated:
- Library/
- Temp/
- Logs/
- obj/
- UserSettings/
- .vs/
- generated .csproj
- generated .sln / .slnx

Không xóa/làm mất `.meta` file. Khi move Unity asset phải giữ/move `.meta` tương ứng.

Runtime code không đặt trong `Editor`. Editor-only code phải nằm trong `Editor`.

Không thêm package hoặc đổi render pipeline để giải quyết vấn đề có thể làm bằng project hiện tại.

## Engineering Principles

Ưu tiên:

1. Correctness
2. Playable iteration speed
3. Readability
4. Architecture vừa đủ cho prototype

Không over-engineer.

Không tạo abstraction/framework/service layer/extensibility chỉ vì có thể dùng trong tương lai.

Không refactor unrelated code trong khi làm V2.

Không mở rộng scope bằng feature “tiện thể”.

Gameplay constants có khả năng thay đổi phải dễ chỉnh/configurable, không rải magic number.

Tránh scene-wide searches trong hot path, allocation không cần thiết mỗi frame, và repeated GetComponent/Find nếu có thể cache.

---

# Active Prototype — V2: Salvage, Material & Power

V0/V1 là completed history và regression baseline. Không rewrite historical spec/checklist/status nếu user/Manager không explicit yêu cầu.

Source priority:

1. User's latest explicit decision.
2. `AGENTS.md`.
3. `docs/prototype-v2-salvage-material-power.md`.
4. `docs/manager-checklist-v2.md`.
5. `docs/manager-status-v2.md`.

V1 systems là nền để reuse/integrate, không phải mục tiêu rewrite.

## V2 Continuous-Pass Authorization

V2 hiện được giao **làm liền một mạch**.

Implementer được phép thực hiện toàn bộ Milestone A → B → C → D trong một task, không cần dừng chờ user playtest giữa milestones.

Workflow bắt buộc:

1. implement milestone hiện tại;
2. test/compile/runtime validate phần vừa làm;
3. sửa implementation-caused errors;
4. chạy lại validation liên quan;
5. tiếp tục milestone kế tiếp;
6. sau Milestone D chạy final full relevant validation;
7. trả một implementation report hoàn chỉnh;
8. dừng cho Manager review/acceptance.

Milestone là checkpoint nội bộ, không phải phase permission gate.

Implementer vẫn **không** được tự tuyên bố V2 PASS/DONE, tự sửa Manager checklist/status, tự commit/push/merge hoặc mở rộng ngoài V2 spec.

---

# Confirmed V2 Rules

## Harbor / boats

- Một shared Harbor phục vụ small boats; không tách Fishing Port và Salvage Port.
- Fishing Boat và Salvage Boat dùng chung Harbor workflow/build/dock/dispatch/receive foundation.
- Fishing Boat tiếp tục trả Fish.
- Salvage Boat trả Scrap Metal + Scrap Wood.
- Multiple boats độc lập về role/state/timer/cargo.
- Cargo chỉ credit một lần mỗi return/unload.

## Material / local storage

- Scrap Metal và Scrap Wood là raw salvage concrete items.
- Processing tạo usable construction materials; ưu tiên existing Iron/Wood identity nếu phù hợp, không broad rename chỉ vì wording.
- Local storage là buffer riêng của consumer/factory, không phải alias của global stock.
- Local storage chỉ được fill từ valid existing resource/network supply, không tự sinh resource.
- Progress bar = `LocalQuantity / LocalCapacity` clamp 0..1.
- Arrow = xu hướng local buffer Up/Down/Neutral trong short stable window.
- House/Citizen V2 chỉ hiển thị Food + Water local-storage bars/trends; chưa có total Satisfaction/Happiness.

## Factory productivity

- Factory hiển thị Productivity % + local material bars/trends.
- `InputSatisfaction = LocalRatio` cho V2.
- Multiple required material inputs lấy minimum làm material bottleneck.
- Power là thêm một bottleneck factor.
- `FactoryProductivity = min(MaterialSatisfaction, PowerEfficiency)` trong khi giữ nguyên V1 operational/damaged/network gates.
- `ActualInterval = DefaultInterval / FactoryProductivity`.
- Productivity <= 0 phải stop an toàn, không divide-by-zero/NaN/duplicate output.

## Electric

- Power là capacity/demand, không phải concrete inventory hoặc kWh consumed per interval.
- Generator operational đóng góp rated kW capacity.
- Powered building bật => demand = rated kW; tắt => demand = 0.
- Không separate idle-power state: bật thì vẫn count rated demand dù input material thấp.
- Existing V1 Electric connectivity vẫn authoritative; disconnected consumer không được dùng remote capacity.
- Trong valid Electric component/grid:
  - `TotalPowerCapacity = sum(operational generator capacity)`;
  - `TotalPowerDemand = sum(enabled powered-building rated demand)`;
  - demand <= 0 => efficiency 1;
  - otherwise `PowerEfficiency = min(1, Capacity / Demand)`.
- Khi thiếu capacity, powered factories cùng grid/component bị cùng PowerEfficiency slowdown.
- Player có thể disable powered processing building để giảm demand và phục hồi grid.
- Không power priority automation trong V2.

## Configurable/provisional

Exact salvage amounts, boat timings/costs, processor conversion ratios, local capacities, base intervals, generator capacities và rated kW vẫn configurable/provisional nếu user chưa chốt.

---

# V2 Milestones

## A — Shared Harbor + Salvage

- generalize Harbor;
- preserve Fishing Boat;
- add Salvage Boat;
- Scrap Metal + Scrap Wood accounting;
- multi-boat independence;
- role UI/state;
- targeted tests/runtime validation.

Sau khi pass targeted validation, tiếp tục B ngay.

## B — Processing + Local Storage

- salvage processing;
- factory local buffers;
- House Food/Water local buffers;
- progress bars + stable trend arrows;
- material shortage → Productivity;
- safe interval scaling/zero state;
- tests/runtime validation.

Sau khi pass targeted validation, tiếp tục C ngay.

## C — Power Capacity + Demand

- generator capacity;
- powered building rated demand;
- on/off state/control;
- V1 Electric connectivity integration;
- PowerEfficiency;
- combine power/material bottleneck;
- power diagnostic UI;
- overload/recovery tests.

Sau khi pass targeted validation, tiếp tục D ngay.

## D — Integrated V2 Hardening

Playtest/harden full loop:

Fishing + Salvage → local buffers → processing → material shortage / power overload → productivity → building disable/recovery.

House Food/Water bars remain informational only.

Run full relevant V0/V1/V2 tests plus Unity compile/Console/Play Mode and final diff/status inspection.

---

# V2 Out of Scope

Không thêm nếu chưa explicit reopen:

- citizen total Satisfaction/Happiness;
- Calories/Protein/Vitamins;
- Diet Diversity;
- Worker/Technician/Upper Class food expectations;
- luxury food;
- mortality/migration/health/security/politics;
- detailed citizen assignment;
- detailed logistics vehicles/haulers;
- batteries/voltage/distance loss/power priority automation;
- final economy balance/art;
- tech tree/tutorial/combat/exploration;
- future-scale generic frameworks không cần cho V2 acceptance.

---

# Decision Rules

Nếu là implementation detail không ảnh hưởng đáng kể player experience:
- chọn giải pháp nhỏ nhất;
- reversible;
- phù hợp architecture hiện tại;
- tiếp tục không cần hỏi.

Nếu thiếu một design decision thật sự thay đổi player experience/dependency loop:
- không tự invent canon;
- ghi blocker rõ ràng;
- báo Manager/user.

Nếu gameplay value chưa chốt:
- giữ configurable/provisional;
- không mô tả là final balance.

---

# Validation

Sau mỗi milestone:

1. inspect diff/status liên quan;
2. Unity refresh/recompile;
3. kiểm compilation state;
4. inspect Console;
5. chạy targeted tests;
6. runtime validation nếu phù hợp;
7. sửa lỗi implementation;
8. rerun validation;
9. nếu sạch thì tiếp tục milestone kế tiếp.

Trước final report:

- run full relevant regression suite;
- run integrated Play Mode V2 scenario;
- verify no implementation-caused errors;
- inspect final diff/status;
- verify no unrelated/generated/IDE files;
- preserve `.meta` integrity.

Không báo hoàn thành chỉ vì code đã viết xong.

---

# Git Discipline

Implementer được sửa files cần thiết cho V2 nhưng không được:

- commit;
- push;
- merge;
- reset;
- force checkout;
- discard unrelated user changes.

Manager sở hữu final acceptance/checklist/status/commit checkpoint.
