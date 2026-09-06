# Husk Base Module — technical pipeline blockout chờ duyệt

**Cập nhật hiện tại:** đã tích hợp ba làn pipeline, UV0 và RGB mask. Handoff authoritative cho source hiện tại: [HuskBaseModule_Pipeline.md](HuskBaseModule_Pipeline.md). Chạy `integrate_base_pipeline.py` để chỉnh và `verify_base_pipeline.py` để kiểm. Preview hiện tại có tiền tố `pipeline_`; các ảnh/report không có tiền tố là bằng chứng của lần duyệt tỷ lệ trước.

Phần bên dưới ghi lại baseline trước pipeline: các mô tả 54 mesh, topology/UV giữ nguyên và không texture không còn áp dụng cho source hiện tại. Kích thước +5/-3 m, chân 0,45 m, origin và hình thức khóa bên vẫn giữ. Không chạy script sửa tỷ lệ cũ lên source đã tích hợp pipeline.

Nguồn: `HuskBaseModule.blend`. Script cập nhật trực tiếp nguồn hiện có: `revise_husk_base_module.py`.
Reference mới nhất: `C:/Users/long/Downloads/0b48238b-cae0-4f08-b5f1-1a8e0fe09ca7.png`.
Đã giữ nguyên 54 mesh objects, tên mesh, topology, hierarchy và material của blockout cũ; chỉ chỉnh tọa độ vertex và thay bộ helper duyệt. Generator cũ `blockout_husk_base_module.py` đã được chặn chạy để tránh ghi đè lại tỷ lệ cũ.
Ưu tiên yêu cầu trực tiếp của người dùng khi hình minh họa hoặc chữ trong concept không nhất quán.

## Kích thước và origin

- Một module vuông duy nhất: **20 × 20 m**; thân chính cao **8 m**, tính cả chân là **8,45 m**. Toàn bộ kích thước concept cũ đã được thay thế.
- Blender: 1 unit = 1 m; +Z lên trên, +Y hướng trước.
- Root `HuskBaseModule` tại world `(0,0,0)`: tâm ô lưới trên đường nước. Đây là origin riêng cho nền nổi, không dùng quy tắc pivot đáy của building.
- Bounds thân chính đo trong Blender: `(-10,-10,-3)` đến `(10,10,5)`, X/Y/Z = **20/20/8 m**.
- Waterline chính xác **Z = 0**. Mặt deck **Z = +5 m**; đáy thân chìm đặc **Z = -3 m**. Thân chìm là một khối kín 20 × 20 × 3 m.
- Đúng bốn chân ngắn, mỗi chân 1,8 × 1,8 × 0,45 m, tâm ngang khoảng `(±8,±8)`. Đáy chân Z ≈ -3,45 m; chân nằm ngoài số đo chiều cao 8 m của thân chính. Sai số float của số đo toàn bộ là khoảng 0,00000005 m.
- Bước đặt lưới **20 m** trên X/Y Blender (X/Z Unity về sau). Toàn bộ model nằm trong ±10 m ngang.
- Đã kiểm tra đặt bản sao cách 20 m theo cả X và Y: khoảng cách biên **0 m**, **0 cặp mesh giao nhau có thể tích dương** ở tolerance 0,00001 m. Các mesh hiện là cuboid thẳng trục nên phép kiểm tra AABB áp dụng trực tiếp. Các mặt tiếp xúc trùng mặt phẳng biên không được tính là xuyên hình học.
- Root, nhóm và mesh đều có location/rotation zero, scale one. Offset/kích thước được bake vào mesh. Không bevel modifier, không texture.

## Hierarchy

```text
HuskBaseModule
├── Deck             # 4 tấm lớn và 8 đoạn viền cam
├── MainBody         # lõi kín, vách lớn, khối góc không nhô lên
├── SideLocks        # 4 cụm pocket / rails / stops / retracted slider
├── UnderwaterBody   # một khối đặc lớn
└── Feet             # đúng 4 chân ngắn
```

Collection `REVIEW_ONLY_Cameras_Guides` chứa 7 camera, chữ chú thích, đường cyan Z=0, thước đo 5/3 m và một collection instance tại `(20,0,0)` tham chiếu chính collection model. Không nhân bản mesh hoặc gộp thành asset khác. Instance được ẩn khi lưu góc làm việc một module; nó chỉ hiện trong ảnh ghép.
Các helper này không thuộc model, không dùng cho gameplay hay xuất asset. Ảnh waterline là hình chiếu có thước đo để thấy toàn bộ thân kín; không cắt/xóa hình học hoặc dùng mặt nước che khuất thân chìm.

## Vật liệu

Năm material màu phẳng trong Blender, roughness 0,78, không texture:

- `Husk_Deck_Blockout`: lấy màu Deck hiện có `(0.57,0.61,0.57)`.
- `Husk_Hull_Blockout`: lấy màu Hull hiện có `(0.12,0.20,0.23)` cho góc/khóa/chân.
- `Husk_StructureGray_Blockout`: xám thân `(0.20,0.25,0.27)`.
- `Husk_SafetyTrim_Muted_Blockout`: biến thể cam trầm `(0.90,0.32,0.08)` theo hướng SafetyTrim/concept.
- `Husk_UnderwaterGray_Blockout`: xám xanh phần chìm `(0.13,0.23,0.27)`.

Đây là material riêng trong `.blend`, không sửa hoặc tạo material Unity. Màu và roughness còn ở mức blockout.

## Biểu diễn khóa và điểm chưa rõ

- Mỗi cạnh có một vùng khóa tối rộng khoảng 3,2 m nằm âm trong vách: hai ray dọc, rãnh giữa, chặn trên/dưới và khối trượt đang thu vào. Mặt ngoài khung khóa dừng ở đúng biên 20 m, không có chốt trắng nhô ở góc hay mảnh nối rời.
- Đây là biểu diễn hình khối của khóa trượt; chưa chốt male/female, hướng di chuyển cuối cùng, hành trình hoặc dung sai lắp ghép. Concept cho các mặt kết nối khá giống nhau nên không đủ dữ kiện để xác nhận cơ cấu thực.
- Hai module nằm sát nhau ở mặt biên, khung ray đối diện nhau. Những rãnh tối nhìn thấy gần mối ghép là phần lõm của khóa, không phải khoảng hở tách hai thân chính. Chưa mô phỏng hành trình khóa hoặc chứng minh interlock cơ khí thực.
- Kích thước authoritative 20 × 20 m và +5/-3 m được ưu tiên tuyệt đối. Bốn chân phụ cao 0,45 m là lựa chọn blockout chờ duyệt; không cộng vào độ chìm 3 m của thân chính.
- Các chi tiết nhỏ trên deck, chốt góc, bevel và phụ kiện trong concept được bỏ ở giai đoạn này theo yêu cầu.

## Ảnh và trạng thái

Ảnh Workbench được tạo từ chính model Blender tại `Art/Previews/HuskBaseModule/`:
`01_isometric.png`, `04_top.png`, `02_front.png`, `03_side.png`, `05_underwater.png`, `06_waterline.png`, `07_adjacent.png`.
Số đo, hierarchy chi tiết và kiểm tra transform lưu trong `blockout-report.json` cùng thư mục.

Chỉ blockout để duyệt hình khối. Không mở/gọi Unity; không FBX; không thay scene, prefab, material, code hay project settings. Dừng và chờ duyệt trước khi thêm chi tiết hoặc xuất asset.
