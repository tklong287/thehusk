# HUSK — Agent Instructions

## Project

Husk là game city-builder / survival management lấy bối cảnh Trái Đất bị ngập nước.

Mục tiêu hiện tại không phải xây full game hoặc vertical slice lớn.

Mục tiêu hiện tại là xây visual/system prototype nhỏ nhất để nhìn thấy các core production loops hoạt động và đánh giá game feel.

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

## Prototype Design Principle

V0 trả lời câu hỏi: nếu các system cơ bản của Husk chạy trực tiếp trước mắt, game có bắt đầu trông và cảm thấy giống một game thú vị hay không?

Ưu tiên V0:

1. Nhìn thấy game world.
2. Camera và interaction cơ bản.
3. Nhìn thấy building, boat và resource production hoạt động.
4. Autonomous production loop.
5. Visual/readability/feel.
6. Iteration nhanh.

V0 không test tutorial flow, scarcity, economy balance, resource bottleneck hoặc production dependency puzzle. Balance làm sau V0. Không tạo shortage để ép progression; không cần tutorial system cho developer/user tự test.

Thứ tự triển khai:

World shell
→ build Fishing Boat
→ autonomous fishing cycle
→ Fish production
→ Water production
→ Recycler production
→ integration/feel pass.

Đây là thứ tự phase triển khai, không phải tutorial ordering hoặc dependency bắt player unlock từng loop.

## Starting Test State and Storage

- Các resource test mặc định bắt đầu ở 100, configurable.
- Food, Water, Recyclable Material, Wood, Iron và Fish khi có trong resource state đều theo cùng default 100; Fish là resource riêng.
- Storage unlimited trong V0: không capacity limit hoặc nâng cấp storage.
- Costs/rates/timing là test/provisional values, dễ chỉnh, không phải final balance.
- Starting resources phải cho phép test mechanic; không tạo intentional bottleneck.

## Fishing — Confirmed V0 Direction

- Fishing Harbor có sẵn từ fresh run khi triển khai Phase 2; player không cần xây Harbor.
- Interaction đầu tiên: chọn Harbor → Build Fishing Boat → construction khoảng 5 giây → boat hoàn thành.
- Boat tự rời Harbor, thực hiện trip khoảng 30 giây rồi quay về.
- Khi Fish production được triển khai ở Phase 4, mỗi lần về unload 5 Fish, resource/HUD cập nhật.
- Một Harbor có thể xây nhiều Fishing Boat; mỗi boat giữ trạng thái/chuyến đi riêng và hoạt động độc lập. Xây thêm không reset hoặc dừng boat cũ.
- Boat tự bắt đầu cycle tiếp theo, không manual redispatch.
- Build time ≈ 5s, complete trip/cycle ≈ 30s và cargo 5 Fish là tunable prototype values.
- Không worker assignment, fuel, maintenance, route logistics, fleet management framework hoặc fishing-area simulation phức tạp.

## Water and Recycler — Independent Production Loops

- Water Plant dùng build/select và construction/activation đơn giản; khi Operational, Water tăng theo thời gian với feedback rõ.
- Recycler chuyển Recyclable Material thành Wood với processing feedback và configurable conversion values.
- Water và Recycler là independent production loops trong V0; Recycler không là prerequisite để dùng Water Plant.
- Không cần Recycler Broken/Repair mechanic.
- Player có resources để test; không Water shortage, construction chain phức tạp hoặc economy balancing.

## Current Prototype Scope

Các mục sau không thuộc Prototype V0; chỉ thay scope khi user yêu cầu rõ ràng:

- tutorial system
- scarcity/economy balance hoặc production dependency puzzle
- Collection
- storage capacity system
- tech tree
- complex citizen simulation
- combat
- security/policing
- morality system
- electricity network
- logistics network
- fleet management framework
- world exploration
- procedural generation
- multiplayer
- live-service systems
- advanced save architecture
- optimization framework cho scale tương lai

Các ý tưởng này có thể thuộc full game sau này nhưng không thuộc playable prototype hiện tại.

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
