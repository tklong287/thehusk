# HUSK — Agent Instructions

## Project

Husk là game city-builder / survival management lấy bối cảnh Trái Đất bị ngập nước.

Mục tiêu hiện tại không phải xây full game hoặc vertical slice lớn.

Mục tiêu hiện tại là Prototype V1 — Module Network & Planning: kiểm chứng placement và expansion qua Module, pipeline connectivity, supply và quy hoạch một megastructure thống nhất.

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

## Active Prototype — V1: Module Network & Planning

Prototype V0 đã COMPLETE, Phase 0–7 DONE. V0 là lịch sử đã hoàn thành; không rewrite `docs/prototype-v0.md` hoặc `docs/manager-checklist.md`. `docs/manager-status.md` giữ completion history V0. V1 đang ACTIVE ở mức specification/planning; implementation chưa bắt đầu.

Source priority cho active V1 work:

1. User's latest explicit decision.
2. `AGENTS.md` — project/engineering rules.
3. `docs/prototype-v1-module-network.md` — active design specification.
4. `docs/manager-checklist-v1.md` — phase acceptance.
5. `docs/manager-status-v1.md` — operational state.

Không dùng rule V0 đã được V1 thay thế để chặn network, population hoặc Repair trong scope V1. Không biến status hoặc assumption thành design canon.

## Confirmed V1 Rules

- Một built cell là một physical Module; ngoài Module là biển. Build Module gắn vào edge hiện có tiếp giáp biển, không chiếm edge/vị trí Fishing Harbor/port.
- Mỗi Module tốn 1 Module Core + 10 Wood từ global city Item Storage; không cần Material pipeline tới construction site. Construction time TBD.
- Town Hall là Item Storage: click để xem concrete items. Fresh start có 1 stack 100 Module Core; storage unlimited. Exact starting Wood chưa chốt, có thể dùng current/provisional configurable test stock.
- Đúng bốn pipeline categories: F = Food, W = Water, M = Material, E = Electric. Luôn dùng M cho Material.
- Standard Module support đúng 3/4 categories. Đây là confirmed V1 playtest rule, chưa final balance. Category không phải concrete item: Fish dùng F và trực tiếp đáp ứng generic Food demand; Wood/Iron/Recyclable Material dùng M nhưng giữ item identity.
- Module chứa building phải support toàn bộ required INPUT + OUTPUT; slots còn lại player chọn. Supported pipelines vẫn pass-through; không conversion giữa categories.
- I/O: Fishing Harbor M → F; Water Plant E+M → W; Recycler E → M; Solar không input → E; House F+W → không output. Town Hall I/O TBD, không tự invent.
- Required inputs phải có continuous compatible path và actual supply; thiếu bất kỳ input nào thì Disabled/Unsupplied. Output feed đúng category. Stock tồn tại toàn thành phố không thay thế network path/supply.
- Fresh city: 1 Town Hall, 1 Fishing Harbor, 1 Solar Power Plant, 1 House, 1 Water Plant Damaged, 1 Recycler Damaged. Hai building Damaged phải Repair và sau đó vẫn cần đủ network inputs; repair cost/time TBD.
- Population bắt đầu 100/100 (current population / effective operational housing capacity); mỗi House capacity 100. Tăng compound +10%/minute theo current population, kể cả vượt capacity hoặc capacity = 0.
- Mỗi resident cần 0.05 Food/s và 0.05 Water/s. House thiếu F hoặc W thì Disabled; residents vẫn tồn tại và được coi là reallocated sang Operational Houses. Chỉ cần overall population và effective capacity, không detailed assignment simulation.
- Pipeline floor cross thuộc Module; F/W/M/E phân biệt được, supplied sáng, supported-unsupplied dim, unsupported inactive. Visual phản ánh topology/supply thật và đúng downstream branch. Exact colors TBD.
- Reuse V0 production khi phù hợp: nhiều boat độc lập, build khoảng 5s, trip khoảng 30s, unload 5 Fish và tự lặp. Các timing/rates reuse là configurable/provisional; không rewrite working loops khi chỉ cần integrate network.

## V1 Scope and Phase Discipline

V1 test spatial planning/connectivity và visible infrastructure, không economy balance pass. Chỉ thực hiện phase hiện tại được giao trong đúng bốn phase lớn:

1. Module Construction + Pipeline Network.
2. Networked Production + Starting City.
3. House + Population + Consumption.
4. Integrated Network Planning Playtest.

Manager sở hữu acceptance/progression và dừng sau mỗi phase để user playtest. Không tự implement phase sau; chuyển Current Phase không tự cấp quyền bắt đầu code.

Không Hub trong V1. Không Gas, Network/Data, 2/4 standard Module, throughput, congestion, pressure, voltage, batteries, day/night solar, distance loss, construction logistics, storage capacity hoặc final balance/art. Không Food Processing/class-based diet, advanced citizen simulation, mortality/migration/happiness/health/growth slowdown/security/overcrowding penalties. Không thêm tutorial, Collection, tech tree, combat, exploration, procedural generation, multiplayer, live service hoặc future-scale frameworks.

Hub, Gas/Data, 2/4 và Trị an/food preferences chỉ là future considerations chưa chốt; không chuẩn bị implementation cho chúng.

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
