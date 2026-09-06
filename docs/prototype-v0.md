# HUSK — Playable Prototype V0

## 1. Purpose

V0 trả lời câu hỏi:

> Nếu các system cơ bản của Husk chạy trực tiếp trước mắt, game có bắt đầu trông và cảm thấy giống một game thú vị hay không?

Đây là visual/system prototype nhỏ cho developer/user tự test. Mục tiêu là nhìn thấy và tương tác với các core loops để đánh giá feel, không xây full game hoặc vertical slice lớn.

## 2. What V0 Is Testing

Ưu tiên theo thứ tự:

1. Nhìn thấy game world.
2. Camera và interaction cơ bản.
3. Nhìn thấy building, boat và resource production hoạt động.
4. Autonomous production loop.
5. Visual/readability/feel.
6. Iteration nhanh.

Thứ tự implementation:

World shell → build Fishing Boat → autonomous fishing cycle → Fish production → Water production → Recycler production → integration/feel pass.

Thứ tự này phân chia phase triển khai, không áp đặt tutorial hoặc dependency unlock trong gameplay. Fishing là autonomous production loop đầu tiên; Water và Recycler là independent loops.

## 3. What V0 Is NOT Testing

- Tutorial flow hoặc tutorial system.
- Scarcity, economy balance hoặc intentional resource bottleneck.
- Production dependency puzzle hay progression bị chặn bởi starting shortage.

Balance làm sau V0. V0 success không phụ thuộc economy balance.

## 4. Starting Test State

- Các resource test mặc định bắt đầu ở **100** và phải configurable.
- Food, Water, Recyclable Material, Wood, Iron và Fish khi có trong resource state đều dùng default này.
- Fish là resource riêng, không đồng nhất với Food. Fish production thuộc Phase 4; phase trước không cần tạo trước production behavior.
- Storage unlimited; không dùng starting scarcity để chặn test gameplay.
- Trong V0 tích hợp, Fishing Harbor có sẵn từ fresh run; player không cần xây Harbor. Phase 2 sở hữu việc thêm Harbor.
- Fresh-run state có thể tái lập để kiểm behavior và feel. Costs/rates là provisional test values, không final balance.

## 5. World / Camera Goal

Phase 1 phải cho người dùng nhìn thấy hình thái Husk khi mở Play Mode:

- Prototype scene/entry point rõ ràng.
- Water/environment tối thiểu.
- Husk platform/module placeholder.
- Gameplay camera quan sát usable; control/interaction tối thiểu nếu cần để quan sát.
- Basic HUD hiển thị resource state; state độc lập presentation, values dễ chỉnh.

Visual placeholder được chấp nhận. Không production gameplay trong Phase 1; không cần world generation hoặc camera framework.

## 6. Fishing Harbor

Fishing Harbor tồn tại sẵn ở fresh run từ Phase 2, có thể select/click và cho feedback chọn rõ ràng. Action gameplay đầu tiên là **Build Fishing Boat** tại Harbor.

Không yêu cầu player xây Harbor; không worker assignment hoặc fleet management framework.

## 7. Fishing Boat Construction

Player chọn Harbor → Build Fishing Boat → construction khoảng **5 giây** → Fishing Boat hoàn thành và nhìn thấy được.

Build progress/time và completion phải đọc được. Build time configurable. Không cần balance cost; nếu có cost thì cost provisional/configurable và starting resources đủ để test, không tạo bottleneck.

Phase 2 dừng ở boat completion. Autonomous departure/cycle thuộc Phase 3.

## 8. Fishing Boat Autonomous Cycle

Sau khi boat hoàn thành, loop tự chạy:

At Harbor → Depart → Fishing / Out at sea → Return → Harbor → repeat.

Một complete trip/cycle khoảng **30 giây**, gồm hành trình rời Harbor và quay lại; không coi 30s là thời gian fishing cộng thêm vào một trip chưa được định nghĩa. Các chi tiết phân bổ thời gian/chuyển động là implementation detail nhỏ, tunable.

Boat nhìn thấy được rời Husk, làm việc và quay lại. Có thể dùng waypoint, simple destination hoặc minimal state machine để behavior dễ đọc. Boat tự lặp, không cần player redispatch mỗi trip.

Không world navigation framework, pathfinding architecture không cần thiết, route logistics, fuel, maintenance hoặc fishing-area simulation phức tạp.

## 9. Fish Production

Phase 4 hoàn tất autonomous production loop đầu tiên:

Boat returns → unload **5 Fish** tại Harbor → Fish resource tăng → HUD cập nhật → boat tự rời đi cho cycle tiếp theo.

Cargo per trip configurable. Resource credit đúng một lần cho mỗi lần unload; nhiều consecutive cycles phải hoạt động. Feedback cho thấy rõ lúc hàng được giao.

Không cộng Fish chỉ vì boat được build; Fish production gắn với return/unload. Fish lưu riêng với Food, storage unlimited.

## 10. Water Production Loop

Test một building-based production loop độc lập:

Build/select Water Plant → construction/activation đơn giản → Operational → Water tăng theo thời gian.

Player đủ resources để dùng mechanic. Placement/build interaction chỉ ở mức tối thiểu nếu cần. Operational state, production và Water HUD update có feedback rõ.

Production/cost/construction values configurable, provisional. Không dùng Water shortage, Recycler prerequisite, electricity, workers, logistics hoặc construction chain phức tạp.

## 11. Recycler Production Loop

Test resource transformation:

Recyclable Material → Recycler → Wood.

Recycler accessible/buildable theo implementation nhỏ nhất. Processing làm Recyclable Material giảm, Wood tăng theo conversion values đã cấu hình, state/HUD và feedback cập nhật đúng.

Không cần Broken/Repair mechanic. Recycler không unlock Water Plant. Không Collection hoặc generic production-chain framework; conversion/processing values là tunable prototype values.

## 12. Storage Rules

Storage **unlimited trong V0**: không capacity limit, storage upgrade hoặc capacity gate cản production/unload. Đây là scope rule của V0, không phải numeric capacity cần tuning.

Resource state vẫn phải đúng khi add/remove/query. Unlimited storage không có nghĩa bỏ qua input consumption của Recycler.

## 13. Provisional/Test Values

| Setting | V0 default / direction |
|---|---|
| Starting test resources | 100 mỗi resource, configurable |
| Storage | Unlimited; không capacity system trong V0 |
| Fishing Boat build time | ≈ 5s, configurable |
| Complete fishing trip/cycle | ≈ 30s, configurable |
| Cargo per trip | 5 Fish, configurable |
| Water production values | Provisional/configurable; chưa chốt exact rate |
| Recycler conversion/processing values | Provisional/configurable; chưa chốt exact ratio/rate |
| Costs/construction values | Provisional/configurable; không intentional bottleneck |

Các số là tunable prototype values, không final balance. Chưa chốt mechanic/timing chi tiết ngoài direction trên; chỉ chọn implementation detail nhỏ, reversible và ghi rõ assumption. Quyết định gameplay mới ảnh hưởng đáng kể player experience phải hỏi user qua Manager.

## 14. Explicit Non-Goals

- Tutorial, scarcity/economy balancing, production dependency puzzle.
- Collection, storage capacity, worker assignment, fuel, maintenance.
- Tech tree, complex citizen simulation, combat, security/policing, morality system.
- Electricity/logistics networks, fleet management framework.
- World exploration, procedural generation, complex fishing-area simulation.
- Multiplayer, live-service systems, advanced save architecture, optimization framework cho scale tương lai.
- Unnecessary navigation/pathfinding hoặc generic production-chain framework.

Không đổi Unity version/render pipeline hoặc thêm third-party package nếu chưa có yêu cầu rõ ràng. Giữ engineering/repository rules trong `AGENTS.md`.

## 15. Integration / Feel Questions

Fresh-run expected capability:

Husk/world visible → resources available → Fishing Harbor exists → Build Fishing Boat → boat automatically fishes → Fish returns → Water production available → Recycler production available.

Phase 7 review:

- Camera readability và scale Husk/buildings/boats.
- Boat movement có dễ thấy, dễ hiểu không?
- Select/click, construction và production feedback.
- HUD readability, world có cảm giác sống hay không?
- Build khoảng 5s và fishing khoảng 30s có cảm giác thế nào?
- Các production loops có thể hiểu bằng observation không?

Ghi nhận kết quả để user đánh giá feel. Phase 7 chủ yếu integration và fix nhỏ cần để hoạt động; không thêm gameplay system mới hoặc biến thành economy balance pass.

## 16. Definition of V0 Complete

V0 complete khi Phase 0–7 trong `docs/manager-checklist.md` đều DONE theo specification mới, và Manager tự xác nhận:

- World và core loops nhìn thấy được, tương tác được, chạy ổn và readable.
- Player build Fishing Boat tại Harbor; sau completion boat tự depart/return/unload/repeat, không manual dispatch mỗi trip.
- Water và Recycler production hoạt động độc lập với feedback/HUD đúng.
- Fresh run tái lập; các resource có sẵn cho test và storage unlimited.
- Unity compile PASS; không compile/runtime errors từ implementation; warnings được review.
- Source/diff và runtime qua Unity MCP có evidence; không chỉ dựa vào Implementer report.
- Integration/feel review đủ để user đánh giá game; không còn blocker chưa giải quyết.

Success phụ thuộc core loops đủ trực quan và ổn định để đánh giá feel, không phụ thuộc economy balance hoặc kết luận rằng final game đã hấp dẫn. Tôn trọng checkpoint/stop của user; không tự chạy phase kế tiếp khi user yêu cầu dừng.
