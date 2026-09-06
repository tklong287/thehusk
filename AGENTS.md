# HUSK — Agent Instructions

## Project

Husk là game city-builder / survival management lấy bối cảnh Trái Đất bị ngập nước.

Mục tiêu hiện tại không phải xây full game hoặc vertical slice lớn.

Mục tiêu hiện tại là xây playable prototype nhỏ nhất để kiểm chứng early-game management loop.

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

Gameplay opening phải đi từ nhu cầu của cư dân đến lý do khai thác/sản xuất tài nguyên.

Không thiết kế tutorial theo kiểu:
"thu thập resource vì tutorial bảo phải thu thập".

Chuỗi ý tưởng cốt lõi hiện tại:

Need
→ thiếu resource
→ cần production building
→ thiếu building material
→ dùng hệ thống hiện có để giải quyết bottleneck
→ xây production building
→ giải quyết need

## Confirmed Prototype Starting State

Khi bắt đầu prototype:

- Food = 0
- Water = 5
- Recyclable Material = 10

Building Material ban đầu gồm:
- Wood
- Iron

Confirmed design intent:
- Starting Iron phải đủ để xây Water Plant và vẫn còn dư một ít.
- Starting Wood không đủ để xây Water Plant.
- Player phải tạo thêm Wood thông qua Recycler.

Exact numerical values cho Wood, Iron và building costs chưa được coi là canon nếu chưa được ghi rõ trong prototype specification.

## Recycler — Confirmed Intent

- Recycler đã tồn tại từ đầu game.
- Recycler bắt đầu trong trạng thái hỏng.
- Player phải Repair Recycler.
- Repair chỉ nên tốn một lượng resource nhỏ.
- Starter Recyclable Material đã có sẵn ngay từ đầu.
- Player dùng starter material này để học Recycler trước.
- Recycler tạo ra Wood.
- Player không cần đi collection trước khi hiểu Recycler.

Chỉ sau khi starter Recyclable Material cạn mới giới thiệu collection.

Exact repair cost và recycle conversion ratio chưa phải canon nếu chưa được specification xác nhận.

## Water Plant — Confirmed Intent

- Player cần Water Plant để giải quyết Water problem.
- Water Plant cần Wood và Iron.
- Starting Wood không đủ.
- Recycler giải quyết Wood bottleneck.
- Việc xây Water Plant trong prototype phải là một construction action đơn giản.
- Không thêm chuỗi crafting/construction nhiều bước chỉ để xây Water Plant.

Exact building cost và production rate chưa phải canon nếu chưa được specification xác nhận.

## Current Prototype Scope

Không tự implement các system sau nếu task hiện tại không yêu cầu:

- tech tree
- complex citizen simulation
- combat
- security/policing
- morality system
- electricity network
- logistics network
- fleet system
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
