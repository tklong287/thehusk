# Husk — Minimal 3D Art Pipeline

This guide covers model art for Prototype V0. Follow `AGENTS.md` and the current assigned task; pipeline setup does not authorize creating assets or starting another phase.

## Ownership

- The 3D Model Artist owns Blender models, exported model meshes, and model-related texture/material assets.
- The gameplay/technical-art agent owns VFX and shader-support art, including effect textures, effect meshes, shaders, and effect materials.
- Gameplay code, interaction/collision behavior, and scene integration remain with the gameplay implementer. Manager owns phase progression and acceptance.
- Reuse the existing Husk shared material palette where possible. Do not alter shared materials or existing scenes without an explicit task covering those changes. Coordinate overlapping material needs with their owner.

## Folders and stable names

Paths below are relative to `D:/TheHusk`:

| Path | Contents |
|---|---|
| `Art/Blender/` | Editable `.blend` source files, outside Unity `Assets` |
| `Game/Husk/Assets/Husk/Models/` | Exported `.fbx` model files |
| `Game/Husk/Assets/Husk/Materials/` | Existing shared palette; model materials only when needed by an assigned asset task |

Use a matching stable source/export basename, such as `<AssetName>.blend` and `<AssetName>.fbx`. Preserve paths, filenames, hierarchy names, material slot names/order, and Unity `.meta` GUIDs across re-exports. Use Git history rather than renaming each revision. Move an existing Unity asset only with its corresponding `.meta` and an explicit reason.

Add asset-specific or texture folders when an actual asset requires them. `.gitkeep` files retain empty folders; Unity folder metadata stays with the Models folder. The calibration source, generation/verification scripts, and evidence live in `Art/Blender/`; the verification C# is evaluated through Unity MCP, not compiled into the game.

## Initial V0 model rules

- **Scale:** 1 Unity unit = 1 meter. Author at meter scale and verify imported dimensions; do not compensate for export errors with arbitrary scene scaling.
- **Orientation:** after Unity import, +Y is up and +Z is forward. Verify the actual imported result before standardizing exporter settings.
- **Buildings:** bottom-center pivot at the center of the footprint on the supporting surface.
- **Boats:** default root is centered horizontally on the hull at the intended waterline, with the bow facing Unity +Z. Local Y = 0 is the water surface, not the keel. Document each boat's dimensions, waterline, draft, and any mesh offset in an adjacent asset note before handoff. If a different root is needed, agree and document it with the implementer before export.
- Prefer simple geometry and strong silhouettes over small details; review from the gameplay camera.
- Reuse Husk's shared palette where possible. Existing placeholder dimensions and colors are references, not additional approved design requirements.
- Triangle budgets, texture resolutions, and LOD requirements are **TBD until the first assets have been tested**.

## Minimal workflow and validation

1. Read the assigned asset brief and inspect the current project, Unity version, and packages. Confirm dimensions and integration needs without changing gameplay.
2. Keep the editable Blender source outside Unity. Export only intended geometry/hierarchy to FBX; omit unrelated cameras, lights, reference objects, and unused animation.
3. Use the verified static-cube settings below as a starting point. Verify meter scale, +Y up/+Z forward, root transforms, pivot/waterline, normals, and material mapping on each new asset. Animated/skinned models and material/texture transfer have not been calibrated.
4. Check silhouette/readability at gameplay zoom and re-export to the same path to verify references remain intact. Preserve existing `.meta` files.
5. Report source/export paths, dimensions/pivot, validation actually performed, assumptions, and blockers. Hand off scene/gameplay integration to the implementer; Manager reviews acceptance.

## Verified calibration — 2026-09-06

Blender **5.2.1 LTS**, Microsoft Store package **5.2.1.0**, successfully generated and exported the fixture in background mode. Its self-reported executable was:

`C:/Program Files/WindowsApps/BlenderFoundation.Blender_5.2.1.0_x64__ppwjx1n5r4v9t/Blender/blender.exe`

Direct invocation of that executable returned Access denied. The supported Store execution alias successfully ran the script:

```powershell
& "$env:LOCALAPPDATA/Microsoft/WindowsApps/blender-launcher.exe" --background --factory-startup --python-exit-code 1 --python D:/TheHusk/Art/Blender/calibrate_cube.py
```

The launcher returns without console output; inspect the timestamp and `success` in `CalibrationCube_2m.blender.json` and the resulting files, not just the launch exit code. Store updates may change the package executable path. No Blender MCP bridge was needed.

### Source and FBX export settings actually tested

Source scene: Metric, Unit Scale **1.0**, Length **Meters**. Author forward as Blender **+Y**, up as **+Z**, right as **+X** for this preset. One 2 m cube mesh under the empty root `CalibrationCube_2m`; child object `CalibrationCube_2m_Mesh`, source mesh data `CalibrationCube_2m_Geometry`. Geometry spans `(-1,-1,0)` to `(1,1,2)` in Blender. Both objects have zero location/rotation and unit scale; geometry is offset above their bottom-center origins.

Exact explicit `bpy.ops.export_scene.fbx` arguments used by `calibrate_cube.py`:

| Setting | Verified value |
|---|---|
| `use_selection`, `object_types` | `True`, `{'EMPTY', 'MESH'}` |
| `global_scale`, `apply_unit_scale` | `1.0`, `True` |
| `apply_scale_options` | `FBX_SCALE_UNITS` |
| `axis_forward`, `axis_up` | **`Z`**, **`Y`** |
| `use_space_transform`, `bake_space_transform` | `True`, `True` |
| `use_mesh_modifiers`, `mesh_smooth_type` | `True`, `FACE` |
| `use_triangles`, `use_tspace` | `True`, `False` |
| `colors_type`, `prioritize_active_color` | `LINEAR`, `True` |
| `use_custom_props`, `bake_anim`, `add_leaf_bones` | `False`, `False`, `False` |
| `path_mode`, `embed_textures` | `AUTO`, `False` |

Color data is a diagnostic only: red marks Blender +Y, green +Z, blue +X; it adds no geometry or material assets. Unity imported those faces at +Z, +Y, and +X respectively. This proves orientation despite cube symmetry. The exporter uses **Z Forward**, not -Z Forward, with this source convention and the Unity settings below.

### Unity import settings actually tested

Verified through the live **Unity 6000.5.7f1** Editor, directly on the imported model asset:

| Setting | Verified value |
|---|---|
| Scale Factor / `globalScale` | `1` |
| Convert Units / `useFileScale` | `True`; reported `fileScale = 1` |
| Bake Axis Conversion / `bakeAxisConversion` | **`True`** (removes the root rotation introduced with it disabled) |
| Preserve Hierarchy | `False`; named root and one mesh child retained |
| Normals / Mesh Compression | `Import` / `Off` |
| Read/Write / Weld Vertices | `False` / `True` |
| Mesh Optimization / Tangents | `Everything` / `CalculateMikk` |
| Import Animation / Animation Type | `False` / `None` |
| Import Cameras / Import Lights | `False` / `False` |
| Material Import Mode | `None` (no materials needed for calibration) |

The asset's `.fbx.meta` preserves the full importer configuration. These are verified settings for this static fixture, not an animation/rig export guarantee.

### Results and repeatability

- Source: `Art/Blender/CalibrationCube_2m.blend`; export: `Game/Husk/Assets/Husk/Models/CalibrationCube_2m.fbx`.
- Unity bounds: min **(-1, 0, -1)**, max **(1, 2, 1)**, size **(2, 2, 2)**. Exact float equality passed; measured size and bottom-center errors were **0** (general check tolerance `0.00001`).
- Root and mesh child both have position `(0,0,0)`, identity rotation, scale `(1,1,1)`. The two-object hierarchy and bottom-center pivot passed.
- Imported diagnostic face positions/normals confirm Blender +Y -> Unity +Z, Blender +Z -> Unity +Y, Blender +X -> Unity +X. All normals point outward; triangle winding agrees with normals. The imported mesh has 24 split vertices and 12 triangles; these are fixture measurements, not budgets.
- A further same-path Blender export and synchronous Unity reimport passed again. All seven imported object GUID/file-ID pairs and the checked importer settings stayed identical.
- `verify_cube.cs` reads the asset without instantiating it in a gameplay scene. `SampleScene.unity` remained loaded and not dirty. No existing scene, gameplay code, or shared material was changed by calibration.
- Evidence: `Art/Blender/CalibrationCube_2m.blender.json` and `Art/Blender/CalibrationCube_2m.unity.json`. Repeat Unity verification with MCP `eval_file` on `D:/TheHusk/Art/Blender/verify_cube.cs` after synchronously reimporting the FBX.
- Unity's generated asset preview was saved and visually inspected as `Art/Blender/CalibrationCube_2m.unity.png`; it shows the cube with default shading. Diagnostic vertex colors were checked numerically, not through a new shader/material.
- One Unity worker warning reported a source-file timestamp mismatch during re-export. A final synchronous forced reimport completed, all numeric checks still passed, and no further warnings/errors appeared after Console cursor 27. The Console was not cleared. This task did not enter/exit Play Mode; other project work was active in the shared Editor.

No blocker remains for this static calibration. Production silhouettes, textured material transfer, rigs/animation, and boat water placement remain untested. Triangle budgets, texture resolutions, and LOD requirements remain TBD. No Harbor or Fishing Boat art was created.
