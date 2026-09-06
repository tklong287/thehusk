"""Generate only the Husk 2 m pipeline fixture in an isolated background Blender."""
import bpy
import json
from pathlib import Path

source_dir = Path(__file__).resolve().parent
repo = source_dir.parent.parent
name = "CalibrationCube_2m"
report_path = source_dir / (name + ".blender.json")
report = {"version": bpy.app.version_string, "executable": bpy.app.binary_path}
try:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.preferences.filepaths.save_version = 0
    scene = bpy.context.scene
    scene.unit_settings.system = "METRIC"
    scene.unit_settings.scale_length = 1.0
    scene.unit_settings.length_unit = "METERS"
    root = bpy.data.objects.new(name, None)
    scene.collection.objects.link(root)
    bpy.ops.mesh.primitive_cube_add(size=2, location=(0, 0, 0))
    obj = bpy.context.object
    obj.name = name + "_Mesh"
    obj.data.name = name + "_Geometry"
    # Shift geometry, not the transform: both origins remain bottom-center.
    for vertex in obj.data.vertices:
        vertex.co.z += 1
    obj.data.update()
    obj.parent = root
    # A cube alone cannot prove forward direction. Per-face diagnostic data:
    # red = Blender +Y (forward), green = +Z (up), blue = +X (right).
    colors = obj.data.color_attributes.new(name="CalibrationAxes", type="BYTE_COLOR", domain="CORNER")
    for face in obj.data.polygons:
        n = face.normal
        color = ((1, 0, 0, 1) if n.y > 0.9 else
                 (0, 1, 0, 1) if n.z > 0.9 else
                 (0, 0, 1, 1) if n.x > 0.9 else (0, 0, 0, 1))
        for loop in face.loop_indices:
            colors.data[loop].color = color
    obj.data.color_attributes.active_color = colors
    bpy.context.view_layer.update()
    assert tuple(obj.dimensions) == (2.0, 2.0, 2.0)
    for item in (root, obj):
        assert tuple(item.location) == (0.0, 0.0, 0.0)
        assert tuple(item.rotation_euler) == (0.0, 0.0, 0.0)
        assert tuple(item.scale) == (1.0, 1.0, 1.0)
        item.select_set(True)
    export_path = repo / "Game/Husk/Assets/Husk/Models" / (name + ".fbx")
    settings = dict(
        use_selection=True, object_types={"EMPTY", "MESH"},
        global_scale=1.0, apply_unit_scale=True, apply_scale_options="FBX_SCALE_UNITS",
        axis_forward="Z", axis_up="Y", use_space_transform=True,
        bake_space_transform=True, use_mesh_modifiers=True, mesh_smooth_type="FACE",
        use_triangles=True, use_tspace=False, colors_type="LINEAR",
        prioritize_active_color=True, use_custom_props=False,
        bake_anim=False, add_leaf_bones=False, path_mode="AUTO", embed_textures=False,
    )
    bpy.ops.wm.save_as_mainfile(filepath=str(source_dir / (name + ".blend")))
    bpy.ops.export_scene.fbx(filepath=str(export_path), **settings)
    report.update({"success": True, "source": str(source_dir / (name + ".blend")),
                   "export": str(export_path), "dimensions_m": list(obj.dimensions),
                   "root": root.name, "mesh": obj.name,
                   "transforms": "Both: position/rotation zero, scale one",
                   "source_bounds": [[-1, -1, 0], [1, 1, 2]],
                   "axis_colors": {"red": "Blender +Y forward", "green": "Blender +Z up", "blue": "Blender +X right"},
                   "export_settings": {k: sorted(v) if isinstance(v, set) else v for k, v in settings.items()}})
except Exception as exc:
    report.update({"success": False, "error": repr(exc)})
    raise
finally:
    report_path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
