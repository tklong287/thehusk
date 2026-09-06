"""Historical initial generator. Superseded; do not overwrite the revised source."""
raise SystemExit("Use revise_husk_base_module.py to update the existing 20 m blockout.")
import bpy
import json
import math
import traceback
from pathlib import Path
from mathutils import Vector

HERE = Path(__file__).resolve().parent
PREVIEWS = HERE.parent / "Previews" / "HuskBaseModule"
PREVIEWS.mkdir(parents=True, exist_ok=True)
REPORT = PREVIEWS / "blockout-report.json"
report = {"stage": "blockout-awaiting-approval", "blender": bpy.app.version_string, "renders": []}

def collection(name):
    c = bpy.data.collections.new(name)
    bpy.context.scene.collection.children.link(c)
    return c

def move_to(obj, coll):
    for c in list(obj.users_collection):
        c.objects.unlink(obj)
    coll.objects.link(obj)

try:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.preferences.filepaths.save_version = 0
    scene = bpy.context.scene
    scene.unit_settings.system = "METRIC"
    scene.unit_settings.scale_length = 1
    scene.unit_settings.length_unit = "METERS"
    model = collection("HuskBaseModule_MODEL")
    review = collection("REVIEW_ONLY_Cameras_Guides")
    root = bpy.data.objects.new("HuskBaseModule", None)
    model.objects.link(root)
    root["origin"] = "Center of 6 m grid cell at waterline. Blender Z=0; +Y forward, +Z up."
    root["stage"] = "ROUGH BLOCKOUT - approval required before detail or export"
    groups = {}
    for name in ("Deck", "MainBody", "SideLocks", "UnderwaterBody", "Feet"):
        group = bpy.data.objects.new(name, None)
        model.objects.link(group)
        group.parent = root
        groups[name] = group

    def mat(name, color):
        m = bpy.data.materials.new(name)
        m.diffuse_color = (*color, 1)
        m.use_nodes = True
        shader = m.node_tree.nodes.get("Principled BSDF")
        shader.inputs["Base Color"].default_value = (*color, 1)
        shader.inputs["Roughness"].default_value = .78
        return m

    deck = mat("Husk_Deck_Blockout", (.57, .61, .57))
    body = mat("Husk_StructureGray_Blockout", (.20, .25, .27))
    lock = mat("Husk_Hull_Blockout", (.12, .20, .23))
    orange = mat("Husk_SafetyTrim_Muted_Blockout", (.90, .32, .08))
    underwater = mat("Husk_UnderwaterGray_Blockout", (.13, .23, .27))
    guide = mat("REVIEW_Waterline", (.10, .70, .80))
    textmat = mat("REVIEW_Text", (.78, .87, .88))

    def box(name, center, size, material, group):
        bpy.ops.mesh.primitive_cube_add(size=1)
        obj = bpy.context.object
        obj.name = name
        obj.data.name = name + "_Geometry"
        # Bake dimensions/offset into vertices: every model transform is identity.
        for v in obj.data.vertices:
            v.co = Vector((v.co.x * size[0] + center[0], v.co.y * size[1] + center[1], v.co.z * size[2] + center[2]))
        obj.data.update()
        move_to(obj, model)
        obj.parent = groups[group]
        obj.data.materials.append(material)
        return obj

    box("MainBody_Core", (0, 0, 2.325), (5.64, 5.64, 4.65), body, "MainBody")
    # Four closed corner masses; none project above deck level.
    for x in (-1, 1):
        for y in (-1, 1):
            label = f"{'E' if x>0 else 'W'}{'N' if y>0 else 'S'}"
            box("MainBody_Corner_"+label, (x*2.725, y*2.725, 2.5), (.55,.55,5), lock,"MainBody")
            box("Deck_Panel_"+label, (x*1.335,y*1.335,4.84), (2.63,2.63,.32), deck,"Deck")
            box("Foot_"+label, (x*2.4,y*2.4,-4.7), (.85,.85,.6), lock,"Feet")
    box("UnderwaterBody_Solid", (0,0,-2.2), (5.9,5.9,4.4), underwater,"UnderwaterBody")

    # Each wall occupies its own grid boundary, including the lock frame.
    # Local wall coordinates: u across wall, d outward, h vertical.
    for side, axis, sign in (("North",1,1),("South",1,-1),("East",0,1),("West",0,-1)):
        def wallbox(label,u,d,h,width,depth,height,material,group):
            center = (u,sign*d,h) if axis==1 else (sign*d,u,h)
            size = (width,depth,height) if axis==1 else (depth,width,height)
            return box(f"{group}_{side}_{label}",center,size,material,group)
        for direction in (-1,1):
            suffix = "A" if direction<0 else "B"
            wallbox("Panel"+suffix,direction*1.6,2.91,2.275,1.7,.18,4.55,body,"MainBody")
            wallbox("OrangeEdge"+suffix,direction*1.6,2.84,4.80,1.7,.32,.40,orange,"Deck")
        # Recessed dark pocket and a broad central slider between two guide rails.
        wallbox("Pocket",0,2.84,2.6,1.5,.04,3.5,lock,"SideLocks")
        for direction in (-1,1):
            wallbox("Rail"+("A" if direction<0 else "B"),direction*.63,2.92,2.6,.24,.16,3.5,lock,"SideLocks")
        wallbox("BottomStop",0,2.92,.95,1.02,.16,.20,lock,"SideLocks")
        wallbox("TopStop",0,2.92,4.25,1.02,.16,.20,lock,"SideLocks")
        wallbox("RetractedSlider",0,2.895,2.25,.76,.11,1.8,body,"SideLocks")

    meshes = [o for o in model.objects if o.type == "MESH"]
    points = [o.matrix_world @ v.co for o in meshes for v in o.data.vertices]
    minimum = [min(v[i] for v in points) for i in range(3)]
    maximum = [max(v[i] for v in points) for i in range(3)]
    size = [maximum[i]-minimum[i] for i in range(3)]
    assert all(abs(a-b)<1e-5 for a,b in zip(size,(6,6,10))), size
    assert len([o for o in meshes if o.parent == groups["Feet"]]) == 4
    assert all(tuple(o.location)==(0,0,0) and tuple(o.rotation_euler)==(0,0,0) and tuple(o.scale)==(1,1,1) for o in model.objects)
    assert all(len(o.modifiers)==0 for o in meshes)

    # Review-only waterline witness: no ocean plane obscures the submerged volume.
    curve=bpy.data.curves.new("REVIEW_Waterline_Geometry","CURVE")
    curve.dimensions="3D"
    curve.bevel_depth=.014
    curve.bevel_resolution=0
    spline=curve.splines.new("POLY")
    spline.points.add(3)
    for p,co in zip(spline.points,[(-3.2,-3.2,0,1),(3.2,-3.2,0,1),(3.2,3.2,0,1),(-3.2,3.2,0,1)]):
        p.co=co
    spline.use_cyclic_u=True
    witness=bpy.data.objects.new("REVIEW_Waterline_Z0_NotPartOfAsset",curve)
    review.objects.link(witness)
    curve.materials.append(guide)

    scene.render.engine="BLENDER_WORKBENCH"
    scene.render.resolution_x=1100
    scene.render.resolution_y=1100
    scene.render.resolution_percentage=100
    scene.render.image_settings.file_format="PNG"
    scene.render.film_transparent=False
    scene.display.shading.light="STUDIO"
    scene.display.shading.color_type="MATERIAL"
    # Review labels must not cast shadows onto the asset.
    scene.display.shading.show_shadows=False
    scene.display.shading.show_cavity=True
    scene.display.shading.cavity_type="BOTH"
    scene.display.shading.curvature_ridge_factor=1.2
    scene.display.shading.curvature_valley_factor=1.0
    scene.display.shading.background_type="WORLD"
    scene.world=bpy.data.worlds.new("REVIEW_Background")
    scene.world.color=(.035,.05,.065)
    scene.view_settings.view_transform="Standard"
    scene.display.render_aa="32"

    views = [
        ("01_isometric", (15,-19,16), (0,0,.3), "PERSP", "ISOMETRIC / BLOCKOUT"),
        ("02_front", (0,24,0), (0,0,0), "ORTHO", "FRONT / +Y"),
        ("03_side", (24,0,0), (0,0,0), "ORTHO", "SIDE / +X"),
        ("04_top", (0,0,24), (0,0,0), "ORTHO", "TOP / +Z"),
        ("05_underwater", (15,-19,-14), (0,0,-.4), "PERSP", "LOW ANGLE / UNDERSIDE"),
    ]
    cameras=[]
    overlays=[]
    for filename,location,target,kind,title in views:
        data=bpy.data.cameras.new("REVIEW_"+filename)
        cam=bpy.data.objects.new("REVIEW_"+filename,data)
        review.objects.link(cam)
        cam.location=location
        cam.rotation_euler=(Vector(target)-cam.location).to_track_quat("-Z","Y").to_euler()
        data.type=kind
        data.lens=52
        data.ortho_scale=8.6 if filename=="04_top" else 13.2
        data.clip_end=200
        cameras.append(cam)
        # Camera-facing review labels, separate from the module hierarchy.
        distance=10
        half = data.ortho_scale/2 if kind=="ORTHO" else distance*math.tan(data.angle_x/2)
        for index,(label,y,scale) in enumerate((("HUSK / BASE MODULE",.92,.065),(title,.82,.04),("6 x 6 m | +5 m / -5 m | cyan line = waterline",-.91,.035))):
            font=bpy.data.curves.new("REVIEW_Label","FONT")
            font.body=label
            font.size=half*scale
            font.align_x="LEFT"
            ob=bpy.data.objects.new(f"REVIEW_{filename}_Label{index}",font)
            review.objects.link(ob)
            ob.parent=cam
            ob.location=(-half*.91,half*y,-distance)
            font.materials.append(textmat)
            overlays.append((cam,ob))

    for cam,(filename,*_) in zip(cameras,views):
        scene.camera=cam
        witness.hide_render=filename=="04_top"
        for owner,ob in overlays:
            ob.hide_render=owner!=cam
        scene.render.filepath=str(PREVIEWS/(filename+".png"))
        bpy.ops.render.render(write_still=True)
        report["renders"].append(scene.render.filepath)
        REPORT.write_text(json.dumps(report,indent=2)+"\n",encoding="utf-8")

    scene.camera=cameras[0]
    witness.hide_render=False
    for owner,ob in overlays:
        ob.hide_render=owner!=cameras[0]
    bpy.ops.object.select_all(action="DESELECT")
    root.select_set(True)
    bpy.context.view_layer.objects.active=root
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type=="VIEW_3D":
                area.spaces.active.region_3d.view_distance=19
                area.spaces.active.region_3d.view_location=(0,0,0)
                area.spaces.active.shading.color_type="MATERIAL"
    destination=HERE/"HuskBaseModule.blend"
    bpy.ops.wm.save_as_mainfile(filepath=str(destination))
    report.update(success=True,source=str(destination),bounds_min=minimum,bounds_max=maximum,
                  dimensions=size,above_water_m=5,below_water_m=5,feet_height_m=.6,
                  root="HuskBaseModule",origin="Grid-cell center at waterline (Blender Z=0)",
                  hierarchy={name:[o.name for o in meshes if o.parent==group] for name,group in groups.items()},
                  clean_transforms=True,mesh_objects=len(meshes),modifiers=0,
                  materials={m.name:list(m.diffuse_color) for m in (deck,body,lock,orange,underwater)},
                  footprint_check="All model vertices within x/y +/-3 m; 6 m grid pitch; no adjacent modules created",
                  lock_status="Recessed rails/pocket/retracted slider only; moving mechanism and mating tolerances not validated",
                  unity_touched=False,fbx_exported=False)
except Exception:
    report.update(success=False,error=traceback.format_exc())
    raise
finally:
    REPORT.write_text(json.dumps(report,indent=2)+"\n",encoding="utf-8")
