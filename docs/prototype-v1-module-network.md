# HUSK — Prototype V1: Module Network & Planning

## 1. Active specification và mục tiêu

**ACTIVE specification — 2026-09-06. V1 implementation chưa bắt đầu.** Tài liệu này thay thế draft lịch sử bằng các quyết định V1 mới nhất của user. V0 là completed history (Phase 0–7 DONE); giữ nguyên spec/checklist/status V0.

V1 kiểm chứng liệu Module + Pipeline Network có khiến placement và expansion thành bài toán quy hoạch thú vị, thay vì các building độc lập trên isometric land grid. Husk phải có cảm giác một megastructure thống nhất: gắn thêm Module, infrastructure chạy xuyên cấu trúc, vị trí ảnh hưởng connectivity, network state nhìn thấy trực tiếp và player suy nghĩ topology/routing. V1 không phải economy balance pass.

Source priority: quyết định explicit mới nhất của user → AGENTS.md → tài liệu này → docs/manager-checklist-v1.md → docs/manager-status-v1.md.

Quy ước:

- **CONFIRMED:** rule đã chốt cho prototype; không tự suy ra final balance full game.
- **PROVISIONAL / TEST VALUE:** cấu hình phục vụ thử nghiệm, dễ chỉnh. Một test value được user xác nhận vẫn là CONFIRMED cho V1.
- **TBD / OPEN QUESTION:** chưa quyết định; không âm thầm chọn thành canon.
- **FUTURE CONSIDERATION:** ý tưởng ngoài implementation V1.

## 2. V0 baseline và reuse

V0 đã có camera/WASD/orbit/zoom, HUD và sáu stock integer, Harbor/nhiều boat độc lập, Water production, Recycler conversion và scene placeholder. Hồ sơ V0 ghi 36 EditMode tests và integrated runtime acceptance; đây là evidence lịch sử, không phải test vừa chạy trong task documentation.

| Baseline | Hướng reuse V1 |
|---|---|
| SampleScene có chín ô sàn trực quan | Reuse/rework hình học phù hợp; chưa có logical Module registry, occupancy, Build Module hoặc graph network |
| FishingHarbor / BoatConstruction / FishingTrip | Giữ independent boats, construction, autonomous cycle và unload không duplicate; integrate M input/F output |
| WaterPlant / WaterProduction | Reuse production timing/feedback; thêm Damaged/Repair và E+M supply gating |
| Recycler / RecyclerProcessing | Reuse conversion Recyclable Material → Wood, atomic debit/credit và input waiting; thêm Damaged/Repair và E gating |
| ResourceState / PrototypeSession / PrototypeHud | Reuse stock validation/HUD; thêm Module Core, Town Hall inspection, network access và fractional consumption |
| Fixed Water/Recycler sites | Rework placement/Module ownership cho V1; Water footprint 1×1 đã được xác nhận |

Runtime nằm dưới `Game/Husk/Assets/Husk/Runtime`, tests ở `Game/Husk/Assets/Husk/Tests/Editor`. V0 chưa có Repair, Town Hall storage, Solar source, House/population hoặc network simulation. Không gọi fixed-site construction V0 là Repair đã tồn tại. Không rewrite working production logic nếu chỉ cần integrate; giữ Unity/meta/engineering rules.

## 3. Module construction — CONFIRMED

Một built cell là một physical Module của Husk. Ngoài Module là biển; player mở rộng cấu trúc bằng Build Module, không unlock land.

- Module mới attach vào edge hiện có của Husk và edge/vị trí mới phải tiếp giáp biển.
- Không xây vào vị trí/cạnh bị Fishing Harbor/port chiếm; không chồng Module hiện có.
- Cost chính xác: **1 Module Core + 10 Wood** từ **global city Item Storage**.
- Không yêu cầu Material pipeline nối tới construction site; không construction logistics trong V1.
- Preview/action phải thể hiện hợp lệ hoặc lý do bị chặn. Invalid/duplicate request không tiêu hao; debit và tạo Module phải nhất quán.
- Exact Module construction time **TBD**, chỉ dùng provisional/configurable nếu cần cho implementation.

Building nằm trên một Module. Không suy art blockout scale hoặc tên ô sàn thành grid/placement contract đã implement.

## 4. Town Hall / Item Storage — CONFIRMED

Fresh V1 có **1 Town Hall**, đảm nhiệm Item Storage. Click Town Hall → hiện danh sách **concrete items** trong storage.

Module Core bắt đầu **1 stack, 100 Module Core**. Storage unlimited; không warehouse capacity, slot limit, weight hoặc logistics-to-storage.

Wood, Iron, Recyclable Material vẫn là concrete items. Không gộp chúng thành một item tên Material. Exact starting Wood **TBD**; có thể giữ current test stock hoặc dùng provisional configurable stock đủ thử Build Module, không biến exact lượng Wood thành canon.

Town Hall pipeline input/output requirement **TBD**. Vai trò global construction storage không tự chứng minh Town Hall là source của một network nào. Không invent requirement hoặc supply endpoint chỉ để giải dependency.

## 5. Pipeline categories và standard Module — CONFIRMED

| Ký hiệu | Category | Concrete items / ý nghĩa |
|---|---|---|
| F | Food | Fish hiện dùng trực tiếp đáp ứng generic Food consumption; Meat/Fruit/future foods có thể thuộc category sau này |
| W | Water | Water supply |
| M | Material | Wood, Iron, Recyclable Material giữ identity riêng |
| E | Electric | Electric supply |

Có đúng bốn categories **F/W/M/E**. Luôn dùng **M = Material**; pipeline category không phải concrete item. Không thêm Food Processing hoặc class-based diet. Không thêm Meat/Fruit gameplay chỉ để chứng minh khả năng mở rộng category.

Standard Module support **đúng 3 trong 4 categories** do cấu hình player lựa chọn, chịu ràng buộc building. **3/4 là confirmed active test rule**, chưa final full-game balance.

Resource chỉ truyền trên continuous path mà mọi Module đều support cùng category; Module cho pipeline đi xuyên qua nó. Không conversion giữa network types. Building consume/produce không chặn other supported pass-through pipelines.

## 6. Building INPUT / OUTPUT và Module constraint — CONFIRMED

| Building | Required INPUT | Produced OUTPUT | Module bắt buộc support | Slots player còn chọn |
|---|---|---|---|---|
| Fishing Harbor | M | F | M + F | W hoặc E |
| Water Plant | E + M | W | E + M + W | Không còn |
| Recycler | E | M | E + M | F hoặc W |
| Solar Power Plant | Không | E | E | Hai trong F/W/M |
| House | F + W | Không | F + W | M hoặc E |
| Town Hall | TBD | TBD | TBD | Chưa chốt requirement |

Module chứa building bắt buộc support toàn bộ INPUT + OUTPUT của building. Phân biệt required input, produced output và optional pass-through; output không phải nhu cầu tiêu thụ. Town Hall storage role không thay thế quyết định I/O còn mở.

## 7. Connectivity, actual supply và Operational

Connectivity đóng vai trò infrastructure connection tương tự road/port connection trong Anno. Building tồn tại trên map chưa đủ để hoạt động.

Đối với mỗi required input, phải có cả continuous compatible path và actual supply. Thiếu bất kỳ input nào → **Disabled / Unsupplied**. Damaged chưa Repair cũng chưa thể Operational; Repair xong vẫn cần đủ input. Có network supply không bỏ qua concrete input availability của production loop (ví dụ Recycler vẫn cần Recyclable Material để tạo Wood).

- House: F supplied + W supplied → Operational; thiếu một loại → Disabled.
- Recycler: thiếu E → Disabled; đủ E mới có thể Operational và processing theo concrete input hiện có.
- Water Plant: E + M supplied mới có thể Operational; thiếu một loại → Disabled.
- Fishing Harbor cần M; output Fish đi vào F. Solar không input, cấp E.

Output phải feed đúng category trong bảng I/O. Không dùng global stock tồn tại ở nơi khác để giả báo endpoint được supply khi không có đường hợp lệ.

| State | Ý nghĩa |
|---|---|
| CONNECTED / NOT CONNECTED | Có/không continuous compatible path |
| SUPPLIED / NOT SUPPLIED | Qua path đó có/không actual supply khả dụng |
| OPERATIONAL / DISABLED | Building đủ/thiếu điều kiện hoạt động |

Khi path/source mất supply, cập nhật downstream thực sự bị ảnh hưởng; alternate path/source hợp lệ giữ vùng còn được cấp. Khi khôi phục, state và visual phục hồi đồng bộ. Không double-count stock do nhiều đường nối. Flowing ở đây biểu diễn supply state, không đo throughput vật lý.

## 8. Starting city và Repair — CONFIRMED

| Fresh building | Số lượng | Starting state / vai trò |
|---|---:|---|
| Town Hall | 1 | Item Storage |
| Fishing Harbor | 1 | Có sẵn; cần M input để hoạt động |
| Solar Power Plant | 1 | E source |
| House | 1 | Capacity 100; cần F + W |
| Water Plant | 1 | DAMAGED |
| Recycler | 1 | DAMAGED |

Water Plant/Recycler: **Damaged → Repair → kiểm network inputs → có thể Operational**. Exact repair cost/time từng building **TBD**; reuse provisional construction/activation mechanics nếu phù hợp nhưng không gọi đó là final repair balance.

Starting Population target là **100/100**. Trong integration phải chứng minh starting supply thật phù hợp House F/W và hai producer Damaged; không hardcode Operational hoặc invent Town Hall I/O để làm đẹp HUD. Nếu setup nguồn/stock cần quyết định gameplay chưa chốt, Manager hỏi trước acceptance phần liên quan ở Phase 2–3. Điều này không chặn foundation Phase 1.

Starting city được tích hợp ở Phase 2; population/consumption đầy đủ ở Phase 3. Đây là thứ tự implementation, không tutorial/unlock progression.

## 9. Population, House và consumption — CONFIRMED TEST VALUES

- Fresh Population **100/100**: current population / effective operational housing capacity.
- Base House capacity **100**. Build House thêm 100 capacity khi House Operational; không tự thêm 100 residents theo mỗi lần xây.
- Population tăng **+10% compound mỗi phút theo current population**: 100 → 110 → 121 → 133.1 → …; rounding/display **TBD**.
- Population không bị hard-cap: 110/100, 121/100 là valid. Growth tiếp tục khi vượt capacity, House Disabled hoặc effective capacity = 0.
- Mỗi resident tạo demand **0.05 Food/s + 0.05 Water/s**; 100 residents = **5 Food/s + 5 Water/s**. Fish là concrete Food item đáp ứng demand này trong V1.
- House chỉ Operational khi có F + W actual supply; thiếu một loại thì Disabled. Residents không biến mất: được coi là dồn/reallocated sang Operational Houses còn lại.
- Không cần detailed resident assignment simulation; overall population và effective operational capacity phải đúng. Population 180, hai House capacity 200; một Disabled → 180/100. Tất cả Disabled → 180/0, population vẫn tồn tại và tăng.

Demand tính theo actual population, không theo nominal capacity; không bỏ residents overcrowded khỏi demand. Fractional demand/growth phải không mất do làm tròn mỗi frame, không stock âm hoặc debit trùng. UI hiển thị population/effective capacity, demand, resource và input thiếu. Chi tiết phân bổ/debit khi nguồn không đủ cần được ghi rõ và báo Manager nếu ảnh hưởng player experience; không tự thêm penalty.

Không death, migration, happiness, health, growth slowdown, security hoặc overcrowding penalty. Trị an tương lai không thuộc V1.

## 10. Production reuse và provisional values

Fishing giữ approximate V0 test loop: build boat khoảng **5s**, complete trip khoảng **30s**, return/unload **5 Fish**, auto repeat. Một Harbor có nhiều boat hoạt động độc lập. Integrate M input và F output; không mở rộng fleet framework.

Water Plant E+M → W, Recycler E → M, Solar → E. Reuse Water timing/output và Recycler Recyclable Material → Wood conversion khi hợp lý. Mapping network không biến conversion thành việc tạo Material item chung.

Rates/build/repair timing ngoài giá trị confirmed phải configurable/provisional. V0 Water 0.5/s và boat 5 Fish/30s không tự đủ đáp ứng demand 5/s của 100 residents. Chuẩn bị test stock/supply phù hợp để đánh giá routing, ghi preset và không biến thiếu supply do preset thành kết luận topology sai hoặc thành economy balance task.

## 11. Pipeline visual — CONFIRMED

Mỗi Module có cross marking trên floor, thuộc infrastructure của Module:

```text
      │
──────┼──────
      │
```

F/W/M/E visually distinguishable; exact colors **TBD**. Có ít nhất ba states:

| State | Visual |
|---|---|
| SUPPORTED + SUPPLIED/FLOWING | Line sáng |
| SUPPORTED nhưng không supply | Line còn visible nhưng dim/dark |
| UNSUPPORTED | Lane inactive/not active, phân biệt được với supported-dim |

Visual phải phản ánh actual topology/supply. Water source mất supply → Water lanes downstream tắt/dim; chỉ một branch mất Water thì chỉ branch bị ảnh hưởng tắt. Vùng có alternate supply vẫn sáng. Khi phục hồi, lane và building state cùng cập nhật. Không màu sáng giả chỉ vì Module support type.

Floor cross là gameplay UX và visual identity, không chỉ debug overlay. Building không được che hết thông tin routing; inspect UI bổ sung type và nguyên nhân mất supply. Art blockouts là reference/handoff riêng, chưa chứng minh network hoặc shader đã implement.

## 12. Đúng bốn implementation phases lớn

| Phase | Phạm vi coherent | Điểm dừng |
|---|---|---|
| V1 Phase 1 — Module Construction + Pipeline Network | Module/edge/port blocking, Town Hall storage foundation/100 Core, 1 Core + 10 Wood payment, Build Module, 3/4/building constraints, F/W/M/E continuity/pass-through/supply foundation, cross visuals và configure/inspect UI | Manager nghiệm thu rồi dừng cho user playtest |
| V1 Phase 2 — Networked Production + Starting City | Đủ starting buildings, Damaged/Repair, confirmed I/O, actual sources/supply/output propagation và downstream failures | Dừng cho user playtest |
| V1 Phase 3 — House + Population + Consumption | Build House, F/W và slot thứ ba, 100/100, capacity/growth/demand, Disabled/reallocation, overcrowding và UI | Dừng cho user playtest |
| V1 Phase 4 — Integrated Network Planning Playtest | Tích hợp/harden expansion/payment/configuration/production/consumption/repair/population/visuals/failure/recovery; không subsystem lớn | Dừng sau playtest report |

Acceptance/scenarios chi tiết trong `docs/manager-checklist-v1.md`; operational state trong `docs/manager-status-v1.md`. Không micro-phase, không implement phase sau sớm.

## 13. Playtest questions

1. Placement có consequence không?
2. Player có nghĩ pipeline nào mỗi Module cần giữ không?
3. 3/4 có quá dễ không?
4. Grid routing có khiến mọi failure quá dễ bypass không?
5. Có nên thử 2/4 ở prototype sau không?
6. Visual F/W/M/E có readable không?
7. Có chẩn đoán broken supply bằng mắt không?
8. Topology có tạo districts tự nhiên không?
9. Husk có bớt cảm giác FarmVille/isometric land grid không?
10. Build Module có cảm giác lắp ráp megastructure không?
11. House/population demand có làm routing meaningful mà chưa cần balance không?

Ghi evidence và nhận xét từng câu; không tự kết luận cần đổi 3/4 sau một cảm nhận đơn lẻ.

## 14. TBD / Open Questions

- Exact Water Plant repair cost/time.
- Exact Recycler repair cost/time.
- Có đổi pipeline configuration sau build không; cost/time reconfiguration.
- Exact F/W/M/E colors.
- Module construction time.
- Exact starting Wood.
- Town Hall pipeline requirements.
- Population rounding/display.
- Future overcrowding/Trị an formula.
- Future food preferences.
- Future 2/4 vs 3/4 sau playtest.

Không giữ các câu hỏi draft đã được user giải quyết: Fish trực tiếp đáp ứng Food, cost Module đã chốt, building I/O đã chốt ngoài Town Hall, House thiếu supply bị Disabled và population vẫn tăng. Cấu hình trước khi build không tự cấp quyền free reconfiguration sau build. Scenario rerouting phải theo workflow được chốt; không tự thêm demolition/refund.

## 15. Non-goals và Future Considerations

Không implement Hub, Gas, Network/Data hoặc 2/4 trong V1. Không throughput/network capacity/congestion/pressure/voltage/battery/day-night solar/distance loss, road/logistics network, construction logistics qua M, storage capacity, final economy balance hoặc final art. Không Food Processing, class-based diet, advanced citizen simulation, death/migration/happiness/health/growth slowdown/security/overcrowding penalties.

**FUTURE CONSIDERATION, chưa chốt:** Hub có thể là special routing Module hỗ trợ nhiều/all pipelines và hy sinh building slot. Gas/Data có thể đưa capacity cố định 3 thành 3/5 hoặc 3/6; 2/4 chỉ cân nhắc sau playtest và quyết định mới. Không tạo implementation chuẩn bị cho các mục này.

## 16. Validation và handoff

Task hiện tại chỉ chuẩn hóa specification/Manager state/Implementer instructions. Không Unity/gameplay/scene/asset/package changes, không spawn Implementer, không V1 implementation. Khi có task phase riêng, Manager tự review source/diff/metas, Unity compile/Console/runtime/tests và từng criterion; không dùng report Implementer làm acceptance duy nhất. Giữ scope đúng phase và dừng cho user playtest.
