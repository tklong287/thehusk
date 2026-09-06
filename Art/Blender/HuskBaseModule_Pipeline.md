# Husk Base Module — three-lane technical handoff, chờ duyệt

Source: `HuskBaseModule.blend`; cập nhật trực tiếp bằng `integrate_base_pipeline.py`.
Kiểm lại file đã lưu bằng `verify_base_pipeline.py`.
Reference: `C:/Users/long/Downloads/cell-pipeline.png`; yêu cầu trong pasted-text của user có quyền ưu tiên so với nội dung trong hình.

## Geometry và placement

- Giữ root hiện có `HuskBaseModule` để tránh đổi tên asset; origin `(0,0,0)` tại tâm waterline, Blender +Z up, +Y North.
- Thân chính **20 × 20 × 8 m**, bounds `(-10,-10,-3)` → `(10,10,5)`; trên nước 5 m, thân chìm 3 m. Giữ bốn chân 0,45 m, đáy -3,45 m.
- Giữ vách, khối góc, khóa bên và underwater body. Thu bốn deck panels để chừa trench chữ thập rộng 3,4 m. Hạ trần MainBody_Core để dành không gian route, thêm khối đỡ kín dưới bốn góc deck.
- Mỗi làn rộng **0,50 m**, dày **0,08 m**, pitch tâm **0,80 m**, khe trống giữa hai làn **0,30 m**. Làn thẳng nằm Z=4,75, âm 0,25 m so với deck. Housing và viền trench là geometry tích hợp.
- Tâm làn theo trục ngang: Lane_01=-0,8 m; Lane_02=0; Lane_03=+0,8 m.
- N/S: tọa độ ngang là world X. E/W: tọa độ ngang là world Y. Cả hai cặp cạnh dùng thứ tự 01→02→03 theo chiều tăng của trục ngang. Không dùng quy ước nhìn từ ngoài vào vì quy ước đó đảo thứ tự ở cạnh đối diện.
- Endpoints tại chính xác ±10 m; cùng cao độ và profile ở cả hai cạnh đối diện. Hai module cùng rotation, dịch 20 m theo X hoặc Y, có gap/offset bằng 0. Đây là hợp đồng translation trên grid; xoay module 90/180 độ không được bảo đảm giữ Lane ID, cần quy tắc remap sau nếu gameplay cho phép.

## Tâm giao nhau

Mỗi làn là một mesh chữ thập kín, liền mạch đến bốn cạnh. Không ghép hai thanh cùng cao độ xuyên qua làn khác.
Trong vùng tâm, mặt trên Lane_01/02/03 lần lượt là Z=4,52 / 4,32 / 4,12 m; thickness 0,08 m cho clearance 0,12 m giữa các làn. Các đoạn ramp nằm giữa khoảng cách 1,4–2,1 m từ tâm, trở lại cùng Z=4,75 ở các cánh thẳng. Mọi giao cắt khác-làn nằm trong vùng cao độ đã tách.
Nắp kỹ thuật 2,85 × 2,85 m che tâm trong production; preview center ẩn nắp để kiểm route. Không thay đổi geometry hay UV để làm ảnh cắt.

```text
HuskBaseModule
├── Deck
├── MainBody
├── PipelineHousing
├── PipelineSurface
│   ├── Lane_01
│   ├── Lane_02
│   └── Lane_03
├── SideLocks
├── UnderwaterBody
└── Feet
```

Mọi transform model là identity, offset bake trong vertices. Camera/chữ/diagnostic và adjacent collection instance nằm trong `REVIEW_ONLY_Cameras_Guides`, ngoài root production.

## Mask và UV contract

Texture: **`Art/Blender/Textures/HuskBaseModule_PipelineMask.png`**, **256 × 256, RGBA8 PNG**.
Đây là **atlas định danh làn theo UV**, không phải ảnh chiếu top-down lên deck. Dùng ba island hằng số để giữ lane identity khi geometry giao nhau ở các cao độ khác nhau.

| Kênh | Ý nghĩa | Vùng pixel, min inclusive / max exclusive |
|---|---|---|
| R | Lane_01 | X=[16,80), Y=[16,240) |
| G | Lane_02 | X=[96,160), Y=[16,240) |
| B | Lane_03 | X=[176,240), Y=[16,240) |
| A | Reserved, toàn bộ 0 | Không dùng làm opacity |

Mỗi island chỉ có giá trị 1 trong một kênh, hai kênh còn lại 0. Texel ngoài các island đen. Không RGB overlap, không resource color, không gradient/compression trong PNG. Resolution 256 là lựa chọn đã kiểm cho mask ID hằng số này, không phải texture budget chung của dự án.

**UV0**, tên Blender **`PipelineMaskUV`**, tương ứng Unity **TEXCOORD0 / Mesh.uv** về sau. Không dùng generated/object coordinates, không phụ thuộc tên object khi shader chạy.

Với lane index k=0,1,2 và vertex Blender x,y theo mét:

```text
u = (24 + 80*k + 48*(x+10)/20) / 256
v = (24 + 208*(y+10)/20) / 256
```

UV nằm trong island có guard margin ít nhất 8 pixel. UV các mặt đứng có thể trùng/collapse trong cùng island vì đây là dữ liệu ID hằng số, không phải UV lightmap hoặc UV texture chi tiết. Không auto-pack UV này. Tất cả mesh ngoài pipeline có UV0=(4/256,4/256), sample black, kể cả khi gộp mesh sau này. Material màu phẳng cũ không dùng UV texture nên không đổi diện mạo.

Trong Blender image dùng Non-Color, CHANNEL_PACKED; node image lưu liên kết relative `//Textures/HuskBaseModule_PipelineMask.png`. Mask không nối vào Base Color production. Ba làn dùng chung `Husk_Pipeline_Neutral`, màu xám trung tính và Emission Strength=0.

## Handoff cho Agent A — chưa triển khai Unity

Shader có thể đọc RGB từ UV0 và tính:

```text
emission = mask.r * _Lane1Color * _Lane1Intensity
         + mask.g * _Lane2Color * _Lane2Intensity
         + mask.b * _Lane3Color * _Lane3Intensity
```

Material properties và resource assignment do Agent A triển khai; chưa có code shader/gameplay mới. Mỗi module cần property values riêng khi assignment/intensity khác nhau. Không suy resource từ tên mesh hoặc từ vị trí làn.

Thiết lập import cần Agent A áp dụng và kiểm khi được phép: sRGB OFF, giữ RGB nguyên vẹn, alpha không dùng transparency, Point sampling, Clamp, không compression/mipmap cho lần kiểm đầu. Đây là **hướng dẫn chưa được xác minh trong Unity**. Không dùng mask này làm tham số flow dọc tuyến; animation có hướng cần UV/dữ liệu riêng ở task sau. Điều khiển màu, bật/tắt và pulsing theo lane dùng trực tiếp mask hiện có.

## Validation và preview

`pipeline-report.json`: bounds, UV/channel samples, edge profiles theo cả X/Y, spacing, hierarchy.
`pipeline-verification.json`: mở lại source đã lưu, đọc PNG thật, kiểm binary channels, UV mọi mặt và vật liệu trung tính; kiểm ba lane kín và connected.

Sáu ảnh trong `Art/Previews/HuskBaseModule/`:

1. `pipeline_01_isometric.png` — ba màu ID tạm, không phải resource assignment.
2. `pipeline_02_top.png` — ba làn song song tới bốn biên.
3. `pipeline_03_lanes.png` — độ âm, bề rộng, spacing.
4. `pipeline_04_center.png` — ẩn nắp để thấy ba route khác cao độ.
5. `pipeline_05_adjacent.png` — collection instance cùng geometry, dịch X=20 m.
6. `pipeline_06_mask_diagnostic.png` — ba lane geometry tách để quan sát; màu diagnostic lấy từ saved PNG tại UV tương ứng. Đây là Workbench diagnostic cộng với kiểm mẫu số học, chưa phải shader Unity.

Material màu tạm chỉ dùng lúc render và được xóa trước khi lưu. Review-only diagnostic objects giữ màu R/G/B để đọc kênh, ngoài hierarchy production. Cam trim cũ trên thân không phải màu resource.

Không mở Unity, không xuất FBX, không sửa scene/material/code/project settings. Dừng tại technical integration và chờ duyệt; routing resource, animation và shader chưa được triển khai hoặc kiểm trong game.
