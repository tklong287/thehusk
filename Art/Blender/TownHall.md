# Town Hall — major-form identity blockout, chờ duyệt

Nguồn ổn định: `TownHall.blend`. Script chỉnh bản hiện có: `revise_town_hall.py`.
`blockout_town_hall.py` là generator bản đầu, không dùng để ghi đè bản sửa hiện tại.
Reference kiến trúc: `C:/Users/long/Downloads/0ac44ed8-37aa-42ce-960f-05d31a40f053.png`.

## Kích thước và placement

- Blender X/Y/Z: **32 × 32 × 18,9 m**, gồm crown. Bounds `(-16,-16,0)` → `(16,16,18.9)` (sai số float dưới 0,000001 m).
- Footprint giữ nguyên; nằm trong envelope 40 × 40 m, khoảng lùi tối thiểu 4 m.
- Root `TownHall_Root` tại `(0,0,0)`, bottom-center; Z=0 là mặt đặt building. Blender +Y là mặt cửa chính; +Z hướng lên; Metric, unit scale 1.
- Giữ trực tiếp 10 mesh cũ: enclosure, storm shoulder và 8 facade frames. Thay các tầng trên/cửa bằng major forms mới; không dựng lại thân dưới từ đầu.
- Các kích thước kiến trúc là lựa chọn blockout chờ duyệt, không chốt floor plan hoặc chức năng gameplay.

## Phân tầng kiến trúc

| Khối | Cao độ Z (m) | Vai trò thị giác |
|---|---|---|
| Lower Operations / Research | 0–5,7 | Thân rộng, kín, vai tường nghiêng chịu thời tiết |
| Observation / Command Ring | 5,82–9,45 | Kính tối bao quanh tám mặt, mái bảo vệ rộng |
| Upper Command Section | 9,45–11,65 | Tầng trung gian bậc thang, rộng 17 × 15,6 m ở chân |
| Central Command Core | 11,65–15,75 | Lõi tám mặt, chân 10,8 × 10,8 m, kính riêng và sống cam phía trước |
| Sensor / Communications Crown | 15,75–18,9 | Chảo 12 cạnh đường kính 6,7 m, pedestal và hai pylon ngắn |

Hai housing kỹ thuật lớn hai bên lõi cao tới Z=12,65 m; chỉ biểu diễn các khối environmental/service equipment. Cửa chính có jamb nghiêng và lintel sâu, cửa lùi sau mặt ngoài khung khoảng 1,7 m. Recess được thể hiện bằng chênh lệch chiều sâu và màu tối, chưa dựng interior hoặc hốc xuyên tường.

## Hierarchy

```text
TownHall_Root
├── PrimaryShell
├── ObservationBand
├── UpperCommandSection
├── CommandCore
├── SensorCrown
├── TechnicalVolumes
├── Entrance
├── StructuralFrames
└── Accents
```

Toàn bộ mesh model thuộc root này; vị trí/kích thước được bake trong vertices, object/group/root position và rotation zero, scale one. `REVIEW_ONLY_Cameras_Envelope` đứng ngoài root, chứa camera/chữ và đường viền 40 m chỉ hiện ở top preview. Không có guide plane hay foundation.

## Materials và phạm vi

Giữ nguyên năm material Blender: `TownHall_Shell_LightGray`, `TownHall_StructureGray`, `TownHall_Hull_Dark`, `TownHall_ObservationGlass`, `TownHall_SafetyOrange`. Màu phẳng; roughness 0,72. Kính là mặt màu tối trên backing, chưa làm shader kính hoặc thickness production. Không đổi shared material Unity.

Không có base modules, water, connectors, nền, landscape hoặc props trang trí. Không thêm bolts, panel seams, pipes, vents, decal hoặc texture. Radar và housing là major silhouette forms theo yêu cầu lần này.

## Review và đối chiếu concept

- Giữ các dấu hiệu kiến trúc chính: shell trắng vát, kính quan sát nghiêng bao quanh, tầng mái giật cấp, command tower trung tâm, sống cam và radar đỉnh.
- So với concept, crown dùng pylon ngắn thay các antenna mảnh; technical volumes giảm còn hai housing lớn. Không sao chép platform hoặc chi tiết bề mặt trong ảnh.
- Command core mới rộng hơn và có kính tám mặt; radar tạo điểm nhận diện ở góc gameplay. Nhận diện cuối cùng vẫn chờ người dùng duyệt và kiểm trong game ở bước được cho phép sau này.

Previews dưới `Art/Previews/TownHall/`: `01_isometric.png`, `02_front.png`, `03_side.png`, `04_top.png`, `05_low_angle.png`.
Đã xem trực tiếp cả năm ảnh Workbench; kiểm số đo, envelope và transform bằng Blender. Report chi tiết: `blockout-report.json`.
Không mở Unity, không xuất FBX. Dừng ở major-form blockout, chờ duyệt trước secondary detail.
