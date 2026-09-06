# HUSK — Prototype V1: Module Network & Planning

## 1. Mục đích và trạng thái tài liệu

Kế hoạch design/implementation cho prototype tiếp theo sau V0, lập ngày 2026-09-06.

Câu hỏi trung tâm:

> Liệu hệ thống module + pipeline network có khiến việc placement và mở rộng Husk trở thành một bài toán quy hoạch thú vị, thay vì cảm giác các building độc lập đặt lên một isometric grid hay không?

V1 kiểm chứng Husk như một cấu trúc ghép từ các Module: connectivity, routing, visible infrastructure, population demand và spatial planning. Đây chưa phải economy balance pass.

Tài liệu này chỉ là kế hoạch; không đánh dấu V1 đã implement hoặc thay đổi Manager Loop hiện tại. V0 vẫn COMPLETE theo scope và evidence của V0. Không rewrite `prototype-v0.md`, checklist/status V0 hoặc `AGENTS.md`.

Quy ước:

- **CONFIRMED:** direction được user xác nhận cho V1; không đồng nghĩa final game balance.
- **PROVISIONAL:** giá trị/giải pháp đề nghị để thử, configurable và có thể đổi sau playtest.
- **TBD / OPEN:** chưa được chốt; không được âm thầm chọn thành canon khi implementation cần quyết định.

AGENTS hiện loại electricity/network và population simulation khỏi V0. Yêu cầu V1 mới mở phạm vi kế hoạch cho đúng những mechanic mô tả tại đây, không sửa lịch sử V0. Các quy tắc engineering, Unity/meta, scope discipline vẫn áp dụng. Trước implementation sau này, Manager cần task và checklist dành riêng cho V1; tài liệu này không tự cấp quyền bắt đầu code.

## 2. Audit V0 và hướng reuse

Audit read-only trên working tree hiện tại tại `D:\TheHusk`; Unity project `Game/Husk`. Có nhiều thay đổi V0/art chưa commit, vì vậy đây là audit working tree, không phải cam kết mọi system đã có trên origin/main.

| Phần hiện tại | Evidence trong repository | Hướng V1 |
|---|---|---|
| World/module trực quan | `Assets/Scenes/SampleScene.unity`: sàn 3×3, chín `Deck Module x,z`; bước tâm 6 đơn vị, mặt sàn 5.84×5.84; biển và khung Husk placeholder | Reuse scale/camera/geometry nếu phù hợp; cần Module state và cấu trúc có thể mở rộng thật |
| Grid logic | Runtime hiện không có grid coordinate registry, adjacency graph, occupancy hay Module construction | Không gọi ô sàn trực quan là grid system hoàn chỉnh; thêm phần domain tối thiểu cần cho placement/network |
| Harbor/Boat | `Assets/Husk/Runtime/FishingHarbor.cs`, `BoatConstruction.cs`, `FishingTrip.cs` | Reuse build 5s, nhiều boat có clock riêng, cycle 30s, arrival/unload một lần và feedback; nối output với source có địa chỉ Module |
| Fish/Food | ResourceKind có cả Fish và Food riêng; boat đang cộng Fish5 lúc return→Harbor | Giữ sự phân biệt; Fish↔Food network là OPEN, không tự đổi tên resource hoặc thêm processing |
| Water | `WaterProduction.cs`, `WaterPlant.cs`: build 5s, +1 Water/2s vào session resources | Reuse timing/activation; thay quyền phục vụ global bằng Water source và đường kết nối |
| Recycler | `RecyclerProcessing.cs`, `Recycler.cs`: 2 Recyclable Material→1 Wood/4s sau build 5s | Reuse conversion/wait/feedback; xác định vai trò các item này trong Material network trước tích hợp |
| Resource state | `ResourceState.cs`, `PrototypeSession.cs`: sáu stock integer default 100, unlimited storage; Changed event, TryRecycle atomic | Reuse validation và tính nhất quán; bổ sung Item Storage/Module Core, source ownership và fractional demand theo nhu cầu V1 |
| Building placement | Water/Recycler là fixed site, bật visual sau construction; chưa có lựa chọn cell hoặc occupancy validation | Reuse select/build feedback; cần placement House trên Module tồn tại và kiểm ô bị chiếm |
| Module footprint | Water(-6,0.7,-6), Recycler(6,0.7,-6) nằm gọn trong một ô; Harbor có dock vươn ra biển | Giữ footprint 1×1 đã xác nhận cho Water; attachment của Harbor vào graph cần khai báo, không suy từ dock mesh |
| HUD/camera | `PrototypeHud.cs`: stock chung, IMGUI panels; `PrototypeCamera.cs`: WASD/orbit/zoom/reset; các panel chặn click/zoom xuyên UI | Reuse interaction; thêm inspect Module, network state và House demand; tổng stock không chứng minh supply |
| Scene structure | Camera/light/volume, `Husk Prototype`, environment/platform, module placeholders, Harbor, Water Plant, Recycler | Reuse có chọn lọc; khung hull/rail hiện cố định không được tạo cảm giác biển ngoài grid là đất đã sở hữu |
| Chưa tồn tại | Item Storage riêng, Module Core, bốn graph, Solar, House, Population, consumption theo topology | Đây là công việc V1, không phải tính năng đã có |

Các file runtime trong bảng nằm dưới `Game/Husk/Assets/Husk/Runtime`; tests tương ứng nằm trong `Assets/Husk/Tests/Editor`. Hồ sơ V0 ghi 36 tests và integrated validation đã đạt; task documentation này không chạy lại Unity tests.

Technical baseline vẫn là Unity 6000.5.7f1, URP 17.5.0, C#. Không đổi engine/pipeline, thêm package hay framework để lập kế hoạch. Khi reuse, giữ các invariants đã test: không duplicate unload, không partial conversion, fresh run reset, giữ `.meta`.

## 3. Starting state và các giá trị thử nghiệm

| Nội dung | Giá trị/direction | Mức chốt |
|---|---|---|
| Item Storage | Có Item Storage; unlimited capacity | CONFIRMED |
| Module Core | Fresh start 100 | CONFIRMED |
| Module Core cost/module | Configurable, exact cost TBD; không tự gán số | OPEN |
| Solar Power Plant | Fresh prototype hoàn chỉnh có sẵn 1 plant tạo Electric | CONFIRMED |
| House | Fresh prototype hoàn chỉnh có sẵn 1 House | CONFIRMED |
| Residents House cơ bản | 100 residents | CONFIRMED test value, configurable |
| Demand mỗi resident | 0.05 Food/s và 0.05 Water/s | CONFIRMED test values, configurable |
| Demand House 100 residents | 5 Food/s và 5 Water/s | Hệ quả của test values |
| Standard Module | Support3 trong 4 loại pipeline | CONFIRMED active playtest setting; chưa final rule |
| Bốn network | Food / Water / Material / Electric, độc lập | CONFIRMED |
| V1 production/build rates khác | Configurable/provisional; không mặc định rates V0 đủ phục vụ demand V1 | PROVISIONAL / TBD |
| Starting stocks ngoài Module Core | Có thể reuse test stocks V0 theo cấu hình; vị trí source/storage và exact V1 preset chưa chốt | PROVISIONAL / TBD |

Starting Solar được đưa vào từ phase 2; starting House từ phase 3. Đây là lộ trình phát triển, không phải unlock/tutorial trong bản V1 hoàn chỉnh. Không tự xóa Harbor hay đổi vòng boat đã có; fresh layout V1 và vị trí cụ thể cần phục vụ bài test routing.

Lưu ý audit: Water V0 tạo 0.5/s, thấp hơn demand 5/s của một House; một boat V0 giao 5 Fish/30s và Fish chưa được xác nhận là Food. Không giữ nguyên các số này rồi dùng thiếu hụt để đánh giá topology. Chuẩn bị cấu hình supply đủ cho routing test, ghi rõ giá trị thử nghiệm; không tự chốt balance hoặc Fish mapping.

## 4. Module construction và placement

**CONFIRMED:** mỗi cell đã xây là một Module vật lý của Husk; ngoài Module là biển. Player không unlock land.

```text
Existing Husk
  → chọn vị trí gắn Module mới
  → Build Module / tiêu hao Module Core
  → Module vật lý xuất hiện, gắn vào cấu trúc
  → configure pipeline support
  → cell có thể chứa building
```

Hợp đồng implementation cần có:

- Phân biệt cell trống trên biển, Module đã xây và occupancy của building.
- Build chỉ hợp lệ nếu vị trí chưa có Module và có attachment hợp lệ vào Husk hiện hữu.
- Dự kiến dùng adjacency chung cạnh trên grid vuông vì phù hợp sàn và dấu cộng; **PROVISIONAL implementation proposal**, không tự coi diagonal/corner attachment là rule đã được user chốt.
- Preview phải hiển thị Module thật, vị trí nối và lý do invalid; không tạo ô đất sử dụng được trước khi xây.
- Cost đọc từ cấu hình sau khi được chốt/cho phép thử; request invalid/duplicate/cancel trước commit không được trừ Core. Tạo Module và debit phải nhất quán.
- Footprint building phải nằm trên Module phù hợp; House không đặt trên biển hoặc chồng building khác. Exact House footprint/capacity và cách xử lý module có building là OPEN nếu cần quyết định thêm.
- Module vẫn là hạ tầng truyền dẫn khi có building; occupancy không được tự xóa network passage.
- Chưa đưa demolition, refund, di chuyển building hoặc structural destruction vào scope; không dựa vào chúng để giải bài test.

Item Storage chứa Module Core không được mặc nhiên biến thành một source toàn bản đồ cho Food/Water. Không thêm item logistics hay capacity limit.

## 5. Network model và continuity

**CONFIRMED:** Food, Water, Material, Electric là bốn graph độc lập. Network type không chuyển đổi qua Module: Water vẫn là Water, Food vẫn là Food. Recycler transformation giữa stock/item không phải phép đổi Food network thành Material network.

Standard Module dùng active test 3/4. UI cho thấy ba loại được hỗ trợ và loại bị loại ra; không có slot thứ tư ngầm. Việc được chọn ít hơn ba loại và policy thay đổi cấu hình sau xây vẫn cần làm rõ nếu implementation/UI phụ thuộc vào đó.

Mỗi graph xét Module hỗ trợ đúng type và những attachment hợp lệ giữa chúng. Với đề nghị adjacency chung cạnh, hai Module chỉ có edge cho type T nếu cả hai support T. Không nhảy qua biển, ô không support hoặc nối chéo ngầm.

```text
Water Source ─ [Water] ─ [Water + House] ─ [Water]   : continuous path
Water Source ─ [Water] ─ [WITHOUT Water] ─ [House]  : bị ngắt
```

Các lane Food/Material/Electric còn lại vẫn được xét riêng khi Water bị ngắt. Thay đổi support của một type không được làm tắt type khác nếu topology/supply của chúng còn hợp lệ.

Building requirement và network transmission tách biệt:

```text
Module support: Food + Water + Electric
Building: House cần Food + Water
Electric: vẫn được đi xuyên Module đến phía sau
```

Không tự suy House phải tiêu thụ Electric, hoặc Water Plant phải cần Electric, chỉ vì các network tồn tại. Requirement cụ thể ngoài House Food+Water còn OPEN.

## 6. Resource tồn tại, path tồn tại và supplied

Ba khái niệm phải có state/UI riêng:

| Khái niệm | Câu hỏi | Không được suy ra |
|---|---|---|
| Resource exists somewhere | Có stock/output ở một source/storage nào đó? | Không chứng minh House dùng được |
| Network path exists | Có continuous path đúng type từ endpoint đến source? | Không chứng minh source đang có supply |
| House actually supplied | Qua path đó có supply khả dụng đáp ứng nhu cầu xét tại tick? | Không được dựa riêng vào global HUD hoặc lane support |

Đề nghị implementation tối thiểu: mỗi source gắn với Module/endpoint rõ ràng; nguồn khả dụng làm active tập Module reachable theo đúng type. Graph connectivity có thể còn nguyên khi source hết nước, nhưng supply state phải cập nhật thành dim. Một counter toàn cục có thể dùng để tổng hợp HUD; không phải quyền truy cập mọi stock.

Ví dụ bắt buộc:

- Source/storage Water ngừng cung cấp → những Module chỉ được nguồn đó cấp không còn sáng, dù path vật lý vẫn tồn tại.
- Một branch mất Water path → chỉ phần không còn đường tới nguồn khả dụng bị dim; branch khác còn kết nối vẫn sáng.
- Nếu có đường vòng hợp lệ hoặc nguồn khả dụng khác, không tắt downstream một cách máy móc theo một parent tree cũ.
- Khôi phục path/supply → trạng thái được tính lại, không cần reload hay sửa resource bằng tay để cập nhật visual.

Đây là supply/reachability, không mô phỏng packet, throughput hoặc hydraulic flow. Khi dùng từ “flowing”, visual biểu diễn network đang được cấp, không khẳng định đang đo lưu lượng vật lý.

Vị trí lưu stock, phạm vi storage theo source/component, phân phối khi nhiều House cùng yêu cầu vượt supply và policy supply một phần là OPEN. Kế hoạch không chọn global stock + một bool connectivity làm giải pháp hoàn chỉnh: nguồn bị cô lập không được phục vụ House qua stock của nguồn khác không liên quan. Không đếm cùng stock hai lần khi có nhiều path.

Thứ tự cập nhật đề nghị: thay đổi Module/support/source → cập nhật connectivity/supply → tính consumption hợp lệ → cập nhật lượng stock và availability → HUD/visual cùng trạng thái nhất quán. Chi tiết cấu trúc dữ liệu để implementation chọn giải pháp nhỏ, tránh rebuild graph mỗi frame nếu không đổi.

## 7. Pipeline visual là gameplay feedback

Mỗi Module có network marking dạng chữ thập trên mặt sàn, thuộc Module chứ không thuộc building:

```text
        │
        │
────────┼────────
        │
        │
```

Food/Water/Material/Electric có lane và màu phân biệt; exact colors **TBD**, không gán palette canon trong kế hoạch. Nên bổ sung icon/tên type và khác biệt độ sáng để UI không chỉ dựa vào màu.

| State | Visual cần có |
|---|---|
| Supported + supplied/flowing | Lane sáng, active state nhìn thấy rõ; chuyển động nhẹ chỉ là lựa chọn trình bày |
| Supported nhưng không supply | Lane vẫn hiện, tối/dim; người chơi thấy hạ tầng có tồn tại |
| Unsupported | Không hiện như active pipeline; lane không có hoặc dấu inactive phân biệt với supported-dim |

Dấu cộng phải nối tới cạnh Module và khớp lane của hàng xóm, không tạo nối giả qua type khác. Building không được che hết thông tin tuyến; giữ phần lane đọc được quanh footprint và tại các cạnh. Hình thức giữ lane visible là lựa chọn implementation/art thử nghiệm.

Gameplay view mặc định phải đọc được trạng thái; inspect/overlay có thể bổ sung nguyên nhân nhưng không thay thế marking trên sàn. Khi branch mất nguồn/path, visual downstream tắt đúng phạm vi. Mục tiêu art identity: những “mạch” hạ tầng chạy xuyên thành phố, cho thấy nơi network hoạt động, bị đứt hoặc mất supply.

## 8. Tích hợp bốn category

| Category | Foundation / việc cần làm | Ranh giới chưa chốt |
|---|---|---|
| Food | Reuse Harbor, boat construction, autonomous trip/unload/repeat; output cần tham gia Food supply qua endpoint | Fish đi thẳng vào Food hay intermediate? **OPEN; Manager dừng hỏi khi cần quyết định**, không thêm Food Processing |
| Water | Reuse Water Plant timing/output; đăng ký Water source ở Module; House chỉ lấy nước qua path | Stock storage/source ownership, rates V1, requirement đầu vào của plant chưa chốt |
| Material | Reuse Recycler processing và atomic debit/credit; cung cấp Material network thật | Recyclable Material/Wood tương ứng với hàng hóa nào được truyền trên Material? Input Recycler đến từ đâu theo topology? OPEN; không tự tạo generic Material resource thay thế |
| Electric | Thêm Solar Power Plant, fresh start 1; source cấp Electric qua graph Electric | Electric consumer cụ thể, cách biểu diễn quantity/availability và rate TBD |

Không cần day/night efficiency, weather efficiency, battery simulation, voltage, power-grid capacity hay sophisticated electric balance. Electric phải có source, path và active/dim thật; chỉ vẽ dây màu mà không có supply state là chưa đủ.

Output producer phải đi vào supply có topology. Một building có input requirement chỉ dùng nguồn reachable tương ứng; không tự truy cập session global stock bất kể vị trí. Không tự thêm requirement để buộc player xây theo một production chain.

Đối với Material/Electric chưa có consumer cụ thể được chốt, có thể inspect một endpoint/module để chứng minh reachability và supply; không tự thêm công trình tiêu thụ mới. Cần giải quyết semantics Material trước khi tuyên bố tích hợp output/input Recycler hoàn chỉnh.

## 9. House, Population và consumption

**CONFIRMED:** V1 hoàn chỉnh khởi đầu với 1 House, House cơ bản100 residents; thêm Population state tối thiểu và Build House.

```text
Food demand/s  = residents × 0.05
Water demand/s = residents × 0.05
House 100 residents → 5 Food/s + 5 Water/s
```

Các số configurable/test values, chưa final balance. Với N House mỗi House thực sự có 100 residents, tổng demand là 5N Food/s và5N Water/s. Không suy capacity bằng actual population nếu policy occupancy của House mới chưa chốt.

Expected interaction:

```text
Build Module → configure 3/4 pipelines → Build House trên Module
  → thêm housing/population capacity → tạo Food/Water demand
  → quan sát path và supply riêng cho từng nhu cầu
```

House cần continuous Food và Water path tới supply khả dụng. Nếu chỉ một loại được cấp, UI phải thể hiện loại còn thiếu; aggregate Supplied chỉ khi cả hai nhu cầu được đáp ứng theo consumption policy đã chốt. Có stock ở nơi khác không đủ.

UI cần tối thiểu: residents/capacity, demand Food/s và Water/s, trạng thái connected/supplied từng loại, aggregate Supplied/Unsupplied và lý do như thiếu pipeline hoặc source không cấp. Không tự áp mortality/happiness penalty.

ResourceState V0 dùng integer; không được làm tròn 0.05 về 0 mỗi frame. Implementation cần accumulator/fixed-point hoặc cách tính tương đương đảm bảo demand không phụ thuộc frame rate, không trừ âm, không credit/debit hai lần. Đây là engineering requirement, không quyết định balance.

OPEN: House mới có ngay100 residents hay thêm capacity rồi được phân bổ thế nào; xử lý Food có mà Water thiếu; cấp một phần khi supply thiếu; stock allocation giữa nhiều House. Phải chốt trước acceptance consumption tương ứng, không tự thêm immigration simulation.

## 10. Bốn implementation phases lớn

Các phase dưới đây là kế hoạch V1, không thay thế phase/checklist V0. Chỉ bắt đầu sau task implementation riêng. Không chia thêm micro-phase hoặc thêm Hub vào chuỗi.

### Phase 1 — Module Construction + Pipeline Network

**Phạm vi:** Module abstraction/coordinates/attachment/occupancy tối thiểu; Item Storage với 100 Module Core; Build Module; cấu hình standard 3/4; bốn graph độc lập; continuity và propagation; cross-lane visual ba states; UI configure/inspect; reuse sàn/camera phù hợp.

**Đầu ra quan sát được:** mở rộng cấu trúc ra biển bằng Module thật; tuyến mỗi type nối hoặc ngắt theo support; inspect cho biết supported/connected/source available. Có thể dùng source fixture bật/tắt supply để kiểm nền tảng, ghi rõ fixture không phải production hoàn chỉnh của phase 2.

**Acceptance/test đề nghị:**

- Build hợp lệ debit đúng cost được chốt/configure, tạo đúng một Module; invalid/duplicate không debit. Empty sea không chứa building.
- Capacity active 3/4 được kiểm; thay đổi type không làm biến đổi type khác. Graph không vượt unsupported cell/biển.
- Connected và supplied khác nhau; source off dim đúng vùng; alternate path/source giữ vùng còn được cấp.
- Mỗi Module có dấu cộng/lanes; supported-dim khác unsupported; UI không chỉ là debug-only.
- Fresh restart tái lập Core 100 và test layout; inspect được route/branch, Unity compile/test/Console đạt.

**Decision gates:** exact Core cost chưa chốt; adjacency corner nếu có; workflow/policy reconfiguration. Không chọn free reconfiguration ngầm. Fixture có thể thay support để test thuật toán, không chứng minh player được đổi miễn phí.

### Phase 2 — Networked Production + Four Resource Types

**Phạm vi:** reuse Fishing/Food foundation, Water Plant và Recycler/Material; thêm Solar tạo Electric với starting 1 plant; output gắn network source; input/supply theo topology; visuals phản ánh nguồn khả dụng và mất kết nối downstream.

**Đầu ra quan sát được:** đủ bốn network có source thực, quantity/availability và trạng thái active/dim đúng; không dùng global stock để bypass đường đi.

**Acceptance/test đề nghị:**

- Boat vẫn build~5s, trip~30s, unload/repeat đúng một lần; bridge Food chỉ theo quyết định user đã xác nhận.
- Water không phục vụ endpoint bị ngắt dù tổng stock còn; ngừng source/storage supply thì lane liên quan dim.
- Recycler giữ conversion nhất quán, input/output Material theo semantics đã chốt; không tạo Wood miễn phí.
- Solar có sẵn và cấp Electric thật; cô lập source làm vùng mất đường cấp tắt, vùng có nguồn khác vẫn hoạt động.
- Mất/khôi phục branch được cập nhật trên graph, HUD và floor lanes; regression V0 giữ production timing/credit đúng.

**Decision gates:** Fish↔Food unresolved thì Manager dừng hỏi user trước implementation bắt buộc chọn; không tự xử lý Food Processing. Material mapping, source/storage ownership và Electric semantics phải rõ trước nghiệm thu liên quan.

### Phase 3 — House + Population + Consumption

**Phạm vi:** starting 1 House/100 residents; Population state; demand 0.05 Food/s và 0.05 Water/s/resident; Build House trên Module; housing/capacity; Food/Water connectivity và actual supply; UI demand/Unsupplied; thêm House tăng demand theo actual population.

**Đầu ra quan sát được:** vị trí House và lựa chọn ba pipeline có hậu quả; một House vẫn chuyển tiếp loại thứ ba dù không consume nó.

**Acceptance/test đề nghị:**

- House ban đầu tạo 5 Food/s +5 Water/s; cấu hình residents/rates đổi demand đúng, không phụ thuộc FPS.
- Resource exists + path missing không supplied; path exists + source unavailable không supplied; đủ cả hai điều kiện cùng lượng supply thì consumption đúng.
- Build House không đặt trên biển/ô occupied; House mới tăng capacity/population/demand đúng policy được chốt.
- Ngắt Food không giả báo Water mất nếu Water còn; UI chỉ ra nhu cầu thiếu. Không phát minh citizen penalty.
- Nhiều House không double-spend stock; tests depletion/refill/partial supply theo policy đã chốt và restart reset.

**Decision gates:** occupancy của House mới, allocation khi thiếu supply, partial consumption và hậu quả Unsupplied còn OPEN. Trước mắt có thể đề nghị chỉ hiển thị Unsupplied; đây chưa là quyền thêm mortality/happiness.

### Phase 4 — Integrated Network Planning Playtest

**Phạm vi:** không subsystem lớn; tích hợp Module Core expansion, cấu hình pipeline, Build House, cả bốn network, production/consumption, visible flow, failure và recovery/rerouting. Đánh giá mechanic, không balance economy.

**Scenarios và evidence cần ghi:**

| Scenario | Kết quả cần quan sát |
|---|---|
| Xây branch mới, đặt House cuối branch | Core giảm đúng, Module gắn thật, House tạo demand; chọn pipeline có ảnh hưởng path |
| Branch thiếu Water | House mất Water supply; lane unsupported và downstream-dim chỉ đúng điểm đứt |
| Reroute/khôi phục Water | Dùng tuyến Module mới hoặc reconfiguration theo policy đã chốt; House supplied trở lại |
| Cắt Food path | Food downstream tắt, Water còn nếu đường Water hợp lệ; không giả xóa global Food stock |
| Cô lập Solar khỏi Electric path | Electric dim đúng phần mất nguồn; không bắt buộc tắt toàn bản đồ có nguồn khác |
| Source/storage hết supply rồi có lại | Path vẫn tồn tại nhưng lanes dim, sau đó sáng; House feedback theo supply thực |
| Thêm House | Demand tăng theo actual residents; tránh kết luận routing sai chỉ vì test rates quá thấp |
| Module chứa House truyền type thứ ba | Chứng minh building consumption không chặn transmission |
| Đường vòng và nhiều branch/source | Chỉ vùng thật sự mất mọi đường cấp tắt; đánh giá3/4 có tạo quyết định đáng kể không |

Scenario “cắt path” dùng cấu hình/harness đã được chấp thuận, không tự thêm demolition hoặc free pipeline editing. Lập preset supply đủ trước khi đánh giá spatial planning; ghi config, topology trước/sau, stocks, demand và ảnh game view để tái hiện.

**Acceptance cuối:** fresh run có 100 Module Core/1 Solar/1 House; cả bốn network hoạt động; có construction/consumption thật; lỗi path/supply và recovery nhìn thấy trên floor lanes; restart tái lập; Unity tests/compile/Console và review diff đạt. Manager ghi câu trả lời playtest và hạn chế thay vì tự kết luận cần đổi capacity.

## 11. Playtest questions

1. Placement có thực sự có consequence không?
2. Player có nghĩ “Module này đặt ở đây sẽ block network nào?”, hay vẫn đặt gần như tùy ý?
3. Active test 3/4 có quá dễ không?
4. Grid có khiến player dễ dàng route vòng mọi vấn đề không?
5. Có cần test 2/4 ở prototype sau không? Đây là câu hỏi sau playtest, không current implementation.
6. Pipeline visual có readable ở camera chơi thực tế không?
7. Khi Food/Water/Material/Electric bị mất, player có nhìn ra nguyên nhân bằng mắt không?
8. Network có tạo zoning/district tự nhiên không?
9. Thành phố có bớt cảm giác FarmVille/isometric land grid không?
10. Mở rộng có cảm giác “lắp ráp một megastructure” hơn là “mở đất rồi đặt nhà” không?

Ghi tình huống và hành vi/nhận xét quan sát được cho từng câu; tách lỗi readability, lỗi topology và thiếu supply do preset. Không dùng số House sống sót hay economy bottleneck làm tiêu chí thành công chính.

## 12. Open Design Questions và thời điểm cần quyết định

Tất cả mục dưới đây vẫn **UNRESOLVED**; kế hoạch không trả lời thay user.

| Câu hỏi | Khi nào cần chốt |
|---|---|
| Fish đi thẳng vào Food network hay là intermediate resource? | Phase 2 trước bridge Fishing/Food; Manager dừng hỏi nếu bắt buộc chọn |
| Exact Module Core cost cho một Module? | Trước dùng cost cho Build Module acceptance; không hardcode canon |
| Có được đổi pipeline configuration sau xây? Miễn phí, có thời gian hay cost? | Phase 1 UI/workflow và phase 4 reroute; fixture test không phải gameplay permission |
| House thiếu Food/Water có consequence gì? Chỉ Unsupplied có đủ không? | Phase 3; không tự thêm mortality/happiness penalty |
| Exact colors bốn network? | Visual prototype dùng palette thử nếu được phép; final colors chưa chốt |
| 3/4 đủ constraint hay cần 2/4 sau playtest? | Sau phase 4; active prototype vẫn3/4 |
| Recyclable Material/Wood đi qua Material network thế nào? | Phase 2; không tự đồng nhất cả hai stock thành một resource mới |
| Source/storage nằm ở Module nào, sở hữu stock thế nào, output buffer xử lý thế nào? | Phase 2 trước nối production/consumption; unlimited capacity không đồng nghĩa global access |
| Nhiều House thiếu supply: allocation, partial supply, Food/Water debit cùng nhau hay độc lập? | Phase 3 trước test consumption; tránh allocation vô tình do thứ tự Update |
| House mới có ngay100 residents hay thêm capacity rồi được phân bổ? Exact footprint/capacity? | Phase 3 Build House và demand scaling |
| Electric availability biểu diễn ra sao, building nào consume Electric? | Phase 2; không tự thêm battery, công suất lưới hoặc dependency chain |
| Adjacency chỉ chung cạnh? Cần đúng 3 type hay có thể ít hơn? Default support/layout fresh? | Phase 1 trước interaction/topology acceptance |

Các câu hỏi không ngăn việc viết tài liệu này. Khi một phase implementation phụ thuộc câu trả lời, Manager giữ phần liên quan chưa nghiệm thu và hỏi user; không che blocker bằng assumption hoặc tiếp tục phase sau với rule tự đặt.

## 13. Explicit non-goals và future considerations

Không implement trong V1: Hub; Gas; Network/Data; economy balance pass; storage capacity; pipeline throughput; congestion; distance penalties/loss; network pressure; bandwidth; advanced logistics; citizen AI; worker assignment; happiness; jobs; mortality; detailed demographics; families/immigration simulation phức tạp; health/education; final art/UI; tech tree/research; fleet expansion ngoài reuse V0; world exploration; combat; morality; security/policing.

Cũng không thêm electricity weather/day-night efficiency, battery, voltage, grid capacity; không procedural world, multiplayer, live service hoặc advanced save architecture.

**Future considerations, không active scope:** Hub có thể là Module infrastructure cho nhiều/all network đi qua nhưng không chứa building; chưa chốt, không nằm trong bốn phase. Gas và Network/Data chỉ minh họa khả năng progression 3/4→3/5→3/6 với capacity 3 giữ nguyên. Không implement các type này. Test2/4 chỉ được cân nhắc sau evidence 3/4 và một quyết định mới.

## 14. Validation kế hoạch và handoff

Kế hoạch có đúng bốn phase lớn; active test 3/4, đủ Food/Water/Material/Electric, starting 100 Module Core/1 Solar/1 House, House 100 residents và 0.05 Food/s+0.05 Water/s mỗi resident. Hub chỉ là future consideration. Open questions vẫn unresolved.

V0 là baseline reuse và lịch sử được giữ nguyên. V1 cần topology và placement thực, không được báo đã tồn tại dựa vào tên các ô sàn. Chưa có code/asset/scene/package mới trong task này; không thay AGENTS, Manager Loop hoặc custom agent; không spawn Implementer.

Khi có task implementation riêng, dùng bốn phase này để tạo checklist V1 với acceptance và decision gates, giữ scope ở connectivity/supply và spatial planning. Chưa tự bắt đầu implementation hoặc điều chỉnh V0 operational status từ tài liệu này.
