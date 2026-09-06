"""Revise existing blockout meshes, then render review-only helpers. No Unity/FBX."""
import bpy, json, traceback
from pathlib import Path
from mathutils import Vector

HERE=Path(__file__).resolve().parent
SOURCE=HERE/'HuskBaseModule.blend'
OUT=HERE.parent/'Previews'/'HuskBaseModule'
REPORT=OUT/'blockout-report.json'
report={'stage':'corrected-blockout-awaiting-approval','renders':[]}

def bounds(objects):
    pts=[o.matrix_world@v.co for o in objects for v in o.data.vertices]
    return [min(p[i] for p in pts) for i in range(3)],[max(p[i] for p in pts) for i in range(3)]

def reshape(o,c,s):
    lo=[min(v.co[i] for v in o.data.vertices) for i in range(3)]
    hi=[max(v.co[i] for v in o.data.vertices) for i in range(3)]
    for v in o.data.vertices:
        v.co=[(v.co[i]-(lo[i]+hi[i])*.5)/(hi[i]-lo[i])*s[i]+c[i] for i in range(3)]
    o.data.update()

try:
    bpy.ops.wm.open_mainfile(filepath=str(SOURCE))
    bpy.context.preferences.filepaths.save_version=0
    scene=bpy.context.scene
    scene.unit_settings.system='METRIC'
    scene.unit_settings.scale_length=1
    scene.unit_settings.length_unit='METERS'
    model=bpy.data.collections['HuskBaseModule_MODEL']
    root=bpy.data.objects['HuskBaseModule']
    meshes=[o for o in model.objects if o.type=='MESH']
    original={o.name:(o.data.name,len(o.data.vertices),len(o.data.polygons)) for o in meshes}
    lo,hi=bounds(meshes)
    scale=20/(hi[0]-lo[0])
    for o in meshes:
        for v in o.data.vertices: v.co.x*=scale; v.co.y*=scale
        o.data.update()
    reshape(bpy.data.objects['UnderwaterBody_Solid'],(0,0,-1.5),(20,20,3))
    for o in meshes:
        lo,hi=bounds([o]); x=(lo[0]+hi[0])*.5; y=(lo[1]+hi[1])*.5
        if o.parent.name=='Feet': reshape(o,(x,y,-3.225),(1.8,1.8,.45))
        elif o.name.startswith('Deck_Panel_'): reshape(o,(x,y,4.84),(abs(x)*2-.06,abs(y)*2-.06,.32))
    for side,axis,sign in [('North',1,1),('South',1,-1),('East',0,1),('West',0,-1)]:
        def wall(name,u,d,z,w,depth,h):
            reshape(bpy.data.objects[name],(u,sign*d,z) if axis==1 else (sign*d,u,z),(w,depth,h) if axis==1 else (depth,w,h))
        for suffix,direction in [('A',-1),('B',1)]:
            wall(f'MainBody_{side}_Panel{suffix}',direction*4.9,9.7,2.275,6.5,.6,4.55)
            wall(f'SideLocks_{side}_Rail{suffix}',direction*1.38,9.74,2.6,.44,.52,3.5)
        wall(f'SideLocks_{side}_Pocket',0,9.43,2.6,3.2,.1,3.5)
        wall(f'SideLocks_{side}_BottomStop',0,9.74,.95,2.32,.52,.2)
        wall(f'SideLocks_{side}_TopStop',0,9.74,4.25,2.32,.52,.2)
        wall(f'SideLocks_{side}_RetractedSlider',0,9.64,2.25,1.65,.28,1.8)
    root['origin']='Center of 20 m square grid cell at waterline Z=0; +Y forward, +Z up'
    root['stage']='BLOCKOUT: main body -3..+5 m; feet to -3.45 m; awaiting approval'
    bpy.context.view_layer.update()
    lo,hi=bounds([o for o in meshes if o.parent.name!='Feet'])
    flo,fhi=bounds(meshes)
    dims=[hi[i]-lo[i] for i in range(3)]
    assert all(abs(a-b)<1e-5 for a,b in zip(dims,(20,20,8))),dims
    assert abs(lo[2]+3)<1e-5 and abs(hi[2]-5)<1e-5
    assert len([o for o in meshes if o.parent.name=='Feet'])==4
    assert all(tuple(o.location)==(0,0,0) and tuple(o.rotation_euler)==(0,0,0) and tuple(o.scale)==(1,1,1) for o in model.objects)
    assert original=={o.name:(o.data.name,len(o.data.vertices),len(o.data.polygons)) for o in meshes}
    # All model meshes are axis-aligned cuboids; box-volume tests are exact here.
    boxes=[bounds([o]) for o in meshes]; adjacency={}
    for axis in (0,1):
        overlaps=0
        for amin,amax in boxes:
            for bmin,bmax in boxes:
                overlap=[min(amax[i],bmax[i]+(20 if i==axis else 0))-max(amin[i],bmin[i]+(20 if i==axis else 0)) for i in range(3)]
                overlaps+=int(all(v>1e-5 for v in overlap))
        gap=flo[axis]+20-fhi[axis]
        adjacency['XY'[axis]]={'pitch_m':20,'boundary_gap_m':gap,'volume_overlap_pairs':overlaps}
        assert overlaps==0 and abs(gap)<1e-5
    report.update(blender=bpy.app.version_string,updated_existing_source=True,mesh_objects_preserved=len(meshes),topology_preserved=True,
        main_dimensions=dims,main_bounds_min=lo,main_bounds_max=hi,overall_dimensions=[fhi[i]-flo[i] for i in range(3)],
        overall_bounds_min=flo,overall_bounds_max=fhi,waterline_z=0,above_water_m=5,main_body_below_water_m=3,
        feet_height_m=.45,feet_count=4,clean_transforms=True,adjacency=adjacency,
        hierarchy={g.name:[o.name for o in meshes if o.parent==g] for g in root.children})
    review=bpy.data.collections['REVIEW_ONLY_Cameras_Guides']
    for o in list(review.objects): bpy.data.objects.remove(o,do_unlink=True)
    guide=bpy.data.materials['REVIEW_Waterline']; textmat=bpy.data.materials['REVIEW_Text']
    def line(name,pts,parent=None,closed=False):
        data=bpy.data.curves.new(name,'CURVE'); data.dimensions='3D'; data.bevel_depth=.025; data.bevel_resolution=0
        s=data.splines.new('POLY'); s.points.add(len(pts)-1)
        for p,co in zip(s.points,pts): p.co=(*co,1)
        s.use_cyclic_u=closed
        o=bpy.data.objects.new(name,data); review.objects.link(o); o.parent=parent; data.materials.append(guide)
        return o
    def label(name,body,x,y,size,cam):
        data=bpy.data.curves.new(name,'FONT'); data.body=body; data.size=size
        o=bpy.data.objects.new(name,data); review.objects.link(o); o.parent=cam; o.location=(x,y,-10)
        data.materials.append(textmat)
        return o
    witness=line('REVIEW_Waterline_Z0_NotModel',[(-10.15,-10.15,0),(10.15,-10.15,0),(10.15,10.15,0),(-10.15,10.15,0)],closed=True)
    instance=bpy.data.objects.new('REVIEW_AdjacentModule_Instance_Only',None); review.objects.link(instance)
    instance.instance_type='COLLECTION'; instance.instance_collection=model; instance.location=(20,0,0)
    scene.render.engine='BLENDER_WORKBENCH'; scene.render.resolution_x=1400; scene.render.resolution_y=1100
    scene.render.resolution_percentage=100; scene.display.shading.show_shadows=False; scene.display.render_aa='32'
    views=[
        ('01_isometric',(32,-40,32),(0,0,1),34,'ISOMETRIC / 20 m SQUARE'),
        ('04_top',(0,0,45),(0,0,1),36,'TOP / +Z'),
        ('02_front',(0,45,1),(0,0,1),28,'FRONT / +Y'),
        ('03_side',(45,0,1),(0,0,1),28,'SIDE / +X'),
        ('05_underwater',(32,-40,-18),(0,0,.4),33,'LOW ANGLE / SOLID UNDERSIDE'),
        ('06_waterline',(0,45,1),(0,0,1),31,'WATERLINE / MEASURED BODY PROFILE'),
        ('07_adjacent',(42,-50,44),(10,0,1),55,'TWO IDENTICAL INSTANCES / 20 m PITCH')]
    cameras=[]; overlays=[]; dimensions=[]
    for filename,location,target,scale,title in views:
        data=bpy.data.cameras.new('REVIEW_'+filename); cam=bpy.data.objects.new('REVIEW_'+filename,data); review.objects.link(cam)
        cam.location=location; cam.rotation_euler=(Vector(target)-cam.location).to_track_quat('-Z','Y').to_euler()
        data.type='ORTHO'; data.ortho_scale=scale; data.clip_end=300; cameras.append(cam)
        half=scale/2; vertical=half*scene.render.resolution_y/scene.render.resolution_x
        for j,(body,y,size) in enumerate([('HUSK / BASE MODULE',vertical*.91,half*.057),(title,vertical*.80,half*.033),
            ('BODY 20 x 20 x 8 m | WATER Z=0 | +5 / -3 m | FEET 0.45 m',-vertical*.92,half*.026)]):
            overlays.append((cam,label(f'REVIEW_{filename}_Label{j}',body,-half*.93,y,size,cam)))
        if filename=='06_waterline':
            for start,end,txt in [(-1,4,'5 m'),(-4,-1,'3 m')]:
                dimensions.append(line('REVIEW_Bracket',[(11.1,start,-10),(11.1,end,-10)],cam))
                for z in (start,end): dimensions.append(line('REVIEW_Tick',[(10.5,z,-10),(11.5,z,-10)],cam))
                dimensions.append(label('REVIEW_Measure',txt,11.65,(start+end)*.5-.25,.65,cam))
            dimensions.append(label('REVIEW_DeckZ','DECK +5',-10,4.65,.55,cam))
            dimensions.append(label('REVIEW_WaterZ','WATER 0',-10,-.6,.55,cam))
            dimensions.append(label('REVIEW_BodyZ','BODY -3 / FEET -3.45',-10,-5.1,.5,cam))
    for cam,(filename,*_) in zip(cameras,views):
        scene.camera=cam; instance.hide_render=filename!='07_adjacent'; witness.hide_render=filename in ('04_top','07_adjacent')
        for owner,o in overlays: o.hide_render=owner!=cam
        for o in dimensions: o.hide_render=filename!='06_waterline'
        scene.render.filepath=str(OUT/(filename+'.png')); bpy.ops.render.render(write_still=True)
        report['renders'].append(scene.render.filepath); REPORT.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    instance.hide_render=True; instance.hide_set(True); witness.hide_render=False
    for o in dimensions: o.hide_render=True; o.hide_set(True)
    for owner,o in overlays: o.hide_render=owner!=cameras[0]; o.hide_set(True)
    scene.camera=cameras[0]; bpy.ops.object.select_all(action='DESELECT'); root.select_set(True); bpy.context.view_layer.objects.active=root
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type=='VIEW_3D':
                r=area.spaces.active.region_3d; r.view_distance=38; r.view_location=(0,0,1); r.view_rotation=cameras[0].rotation_euler.to_quaternion()
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE))
    report.update(success=True,source=str(SOURCE),instance_only=True,unity_touched=False,fbx_exported=False)
except Exception:
    report.update(success=False,error=traceback.format_exc()); raise
finally:
    REPORT.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
