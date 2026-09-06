"""Revise existing building only: major-form identity pass. No Unity/FBX."""
import bpy, bmesh, json, math, traceback
from pathlib import Path
from mathutils import Vector

HERE=Path(__file__).resolve().parent
OUT=HERE.parent/'Previews'/'TownHall'
report={'stage':'major-form-identity-pass-awaiting-approval','renders':[]}
try:
    bpy.ops.wm.open_mainfile(filepath=str(HERE/'TownHall.blend'))
    bpy.context.preferences.filepaths.save_version=0
    scene=bpy.context.scene
    model=bpy.data.collections['TownHall_MODEL']
    review=bpy.data.collections['REVIEW_ONLY_Cameras_Envelope']
    root=bpy.data.objects['TownHall_Root']
    # Retain the original lower architecture, facade bracing and root identity.
    retained=[]
    for o in list(model.objects):
        keep=o.name in ('TownHall_Root','Shell_LowerEnclosure','Shell_StormShoulder') or o.name.startswith('Frame_') or o.type=='EMPTY'
        if keep:
            if o.type=='MESH': retained.append(o.name)
        else: bpy.data.objects.remove(o,do_unlink=True)
    for o in list(review.objects): bpy.data.objects.remove(o,do_unlink=True)
    groups={o.name:o for o in model.objects if o.type=='EMPTY' and o!=root}
    for name in ['UpperCommandSection','TechnicalVolumes','SensorCrown']:
        if name not in groups:
            o=bpy.data.objects.new(name,None); model.objects.link(o); o.parent=root; groups[name]=o
    white=bpy.data.materials['TownHall_Shell_LightGray']
    structural=bpy.data.materials['TownHall_StructureGray']
    dark=bpy.data.materials['TownHall_Hull_Dark']
    glass=bpy.data.materials['TownHall_ObservationGlass']
    orange=bpy.data.materials['TownHall_SafetyOrange']
    guide=bpy.data.materials['REVIEW_Envelope']; textmat=bpy.data.materials['REVIEW_Text']
    def mesh(name,verts,faces,mat,group):
        data=bpy.data.meshes.new(name+'_Geometry'); data.from_pydata(verts,[],faces); data.update()
        bm=bmesh.new(); bm.from_mesh(data); bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces)); bm.to_mesh(data); bm.free()
        o=bpy.data.objects.new(name,data); model.objects.link(o); o.parent=groups[group]; data.materials.append(mat); return o
    def octagon(w,d,c,z,center=(0,-1)):
        x=w/2; y=d/2
        pts=[(x-c,-y),(x,-y+c),(x,y-c),(x-c,y),(-x+c,y),(-x,y-c),(-x,-y+c),(-x+c,-y)]
        return [(a+center[0],b+center[1],z) for a,b in pts]
    def volume(name,lo,hi,mat,group):
        n=len(lo)
        return mesh(name,lo+hi,[tuple(reversed(range(n))),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)],mat,group)
    def box(name,c,s,mat,group):
        x,y,z=c; a,b,h=[v/2 for v in s]
        lo=[(x-a,y-b,z-h),(x+a,y-b,z-h),(x+a,y+b,z-h),(x-a,y+b,z-h)]
        return volume(name,lo,[(u,v,z+h) for u,v,_ in lo],mat,group)
    def band(name,lo,hi,group,count=3):
        volume(name+'_StructuralBacking',lo,hi,dark,group)
        for i in range(8):
            a,b=Vector(lo[i]),Vector(lo[(i+1)%8]); c,d=Vector(hi[i]),Vector(hi[(i+1)%8])
            normal=(b-a).cross(c-a).normalized()
            splits=count if (b-a).length>5 else 1
            for j in range(splits):
                s=(j+.065)/splits; e=(j+.935)/splits
                p,q=a.lerp(b,s),a.lerp(b,e); r,t=c.lerp(d,s),c.lerp(d,e)
                vs=[p.lerp(r,.09),q.lerp(t,.09),q.lerp(t,.91),p.lerp(r,.91)]
                mesh(f'{name}_Glass_{i:02}_{j:02}',[v+normal*.035 for v in vs],[(0,1,2,3)],glass,group)
    # Wider, taller ocean-facing bridge band; all eight facets carry observation glazing.
    volume('Observation_OrangeSill',octagon(28.1,26.1,3,5.55),octagon(27.9,25.9,3,5.85),orange,'Accents')
    band('Observation',octagon(27.8,25.8,3,5.82),octagon(24.7,22.7,3,8.8),'ObservationBand')
    volume('Shell_UpperRoofCap',octagon(25.4,23.4,3,8.8),octagon(24.2,22.2,3,9.45),white,'PrimaryShell')
    # Broad stepped upper command section, distinct from the central bridge.
    volume('UpperCommand_ArmoredTerrace',octagon(17,15.6,2.4,9.45,(0,-1.5)),octagon(15.8,14.4,2.4,10.1,(0,-1.5)),structural,'UpperCommandSection')
    volume('UpperCommand_Shell',octagon(15.8,14.4,2.4,10.1,(0,-1.5)),octagon(14,12.6,2.4,11.65,(0,-1.5)),white,'UpperCommandSection')
    # Broad central control bridge; substantially larger than the previous roof cabin.
    volume('Command_CoreEnclosure',octagon(10.8,10.8,1.9,11.65,(0,-1.5)),octagon(10.8,10.8,1.9,13,(0,-1.5)),white,'CommandCore')
    volume('Command_OrangeSill',octagon(10.95,10.95,1.9,12.8,(0,-1.5)),octagon(10.95,10.95,1.9,13.05,(0,-1.5)),orange,'Accents')
    band('Command',octagon(10.8,10.8,1.9,13.05,(0,-1.5)),octagon(9.4,9.4,1.8,15.1,(0,-1.5)),'CommandCore',2)
    volume('Command_ArmoredCrown',octagon(10.2,10.2,1.8,15.1,(0,-1.5)),octagon(9.3,9.3,1.8,15.75,(0,-1.5)),white,'CommandCore')
    # One strong vertical structural spine ties the bridge to the terraced section.
    box('Command_FrontSpine',(0,4.02,12.65),(1.8,.8,4.6),structural,'CommandCore')
    box('Command_OrangeSpine',(0,4.435,12.55),(.55,.04,3.5),orange,'Accents')
    # Two large protected environmental/research housings flanking the command terrace.
    for sign in (-1,1):
        c=(sign*9,-3.5)
        volume(f'Technical_ServiceHousing_{sign}',octagon(4.4,6.8,.55,9.3,c),octagon(3.7,5.8,.55,12.25,c),structural,'TechnicalVolumes')
        volume(f'Technical_ArmoredCap_{sign}',octagon(4.5,6.7,.55,12,c),octagon(3.6,5.7,.55,12.65,c),white,'TechnicalVolumes')
        box(f'Technical_FrontInset_{sign}',(sign*9,.0,10.8),(2.7,.09,1.6),dark,'TechnicalVolumes')
    # Protected communications pedestal, 12-sided bowl and stout sensor pylons.
    volume('Sensor_Pedestal',octagon(3.8,3.8,.65,15.75,(0,-1.5)),octagon(3.1,3.1,.6,17.05,(0,-1.5)),structural,'SensorCrown')
    n=12; verts=[]
    # Closed thick bowl: outer bottom -> outer rim -> inner rim -> inner basin.
    for radius,z in [(1.1,17),(3.35,18.45),(3.1,18.65),(.85,17.4)]:
        for i in range(n):
            a=2*math.pi*i/n; verts.append((radius*math.cos(a),-1.5+radius*math.sin(a),z))
    faces=[tuple(reversed(range(n))),tuple(range(3*n,4*n))]
    for ring in range(3):
        for i in range(n): faces.append((ring*n+i,ring*n+(i+1)%n,(ring+1)*n+(i+1)%n,(ring+1)*n+i))
    mesh('Sensor_RadarBowl',verts,faces,white,'SensorCrown')
    box('Sensor_CentralFeed',(0,-1.5,18.15),(.7,.7,1.5),structural,'SensorCrown')
    for sign in (-1,1):
        volume(f'Sensor_ProtectedPylon_{sign}',octagon(1.4,2,.25,15.75,(sign*3.9,-2.2)),octagon(.95,1.5,.2,18.05,(sign*3.9,-2.2)),structural,'SensorCrown')
    # Door behind a deep substantial portal: no decorative entrance props.
    box('Entrance_Recess',(0,14,2.25),(6.1,.3,4.5),dark,'Entrance')
    for sign in (-1,1):
        volume(f'Entrance_ArmoredJamb_{sign}',[(sign*3.5-.75,13.8,0),(sign*3.5+.75,13.8,0),(sign*3.5+.75,16,0),(sign*3.5-.75,16,0)],
            [(sign*3.0-.65,13.8,4.9),(sign*3.0+.65,13.8,4.9),(sign*3.0+.65,15.8,4.9),(sign*3.0-.65,15.8,4.9)],white,'Entrance')
        box(f'Entrance_DoorLeaf_{sign}',(sign*1.03,14.22,1.9),(2,.16,3.8),structural,'Entrance')
        box(f'Entrance_OrangeEdge_{sign}',(sign*2.3,14.25,2.2),(.22,.12,3.4),orange,'Accents')
    box('Entrance_StormLintel',(0,14.9,4.7),(7.4,2.1,.7),structural,'Entrance')
    bpy.context.view_layer.update()
    meshes=[o for o in model.objects if o.type=='MESH']
    pts=[o.matrix_world@v.co for o in meshes for v in o.data.vertices]
    lo=[min(v[i] for v in pts) for i in range(3)]; hi=[max(v[i] for v in pts) for i in range(3)]
    dims=[hi[i]-lo[i] for i in range(3)]
    assert all(abs(a-b)<1e-4 for a,b in zip(dims,(32,32,18.9))),dims
    assert all(-20<=v.x<=20 and -20<=v.y<=20 and v.z>=0 for v in pts)
    assert all(tuple(o.location)==(0,0,0) and tuple(o.rotation_euler)==(0,0,0) and tuple(o.scale)==(1,1,1) for o in model.objects)
    assert scene.unit_settings.scale_length==1
    root['stage']='Major-form architectural identity pass; awaiting approval'
    data=bpy.data.curves.new('REVIEW_40mEnvelope','CURVE'); data.dimensions='3D'; data.bevel_depth=.025; data.bevel_resolution=0
    s=data.splines.new('POLY'); s.points.add(3)
    for p,co in zip(s.points,[(-20,-20,0,1),(20,-20,0,1),(20,20,0,1),(-20,20,0,1)]): p.co=co
    s.use_cyclic_u=True
    envelope=bpy.data.objects.new('REVIEW_ONLY_40x40m_Envelope_NotBuilding',data); review.objects.link(envelope); data.materials.append(guide)
    scene.render.engine='BLENDER_WORKBENCH'; scene.render.resolution_x=1400; scene.render.resolution_y=1100; scene.render.resolution_percentage=100
    scene.render.image_settings.file_format='PNG'; scene.display.render_aa='32'
    sh=scene.display.shading; sh.light='STUDIO'; sh.color_type='MATERIAL'; sh.show_shadows=False; sh.show_cavity=True; sh.cavity_type='BOTH'
    sh.curvature_ridge_factor=1.15; sh.curvature_valley_factor=1; sh.background_type='WORLD'
    scene.world.color=(.035,.05,.065); scene.view_settings.view_transform='Standard'
    views=[('01_isometric',(45,55,45),(0,0,7),64,'ISOMETRIC / COMMAND + RESEARCH'),
        ('02_front',(0,65,9),(0,0,9),48,'FRONT / +Y'),('03_side',(65,0,9),(0,0,9),48,'SIDE / +X'),
        ('04_top',(0,0,70),(0,0,0),66,'TOP / 40 m PLACEMENT ENVELOPE'),
        ('05_low_angle',(38,60,23),(0,0,9),52,'LOW ANGLE / COMMAND CORE + SENSOR CROWN')]
    cameras=[]; captions=[]
    for filename,loc,target,scale,title in views:
        data=bpy.data.cameras.new('REVIEW_'+filename); cam=bpy.data.objects.new('REVIEW_'+filename,data); review.objects.link(cam)
        cam.location=loc; cam.rotation_euler=(Vector(target)-cam.location).to_track_quat('-Z','Y').to_euler(); data.type='ORTHO'; data.ortho_scale=scale; data.clip_end=300
        cameras.append(cam); half=scale/2; vertical=half*1100/1400
        for j,(txt,y,size) in enumerate([('HUSK / TOWN HALL',vertical*.91,half*.055),(title,vertical*.80,half*.032),
            ('IDENTITY BLOCKOUT | 32 x 32 x 18.9 m | BUILDING ONLY',-vertical*.92,half*.027)]):
            font=bpy.data.curves.new('REVIEW_Label','FONT'); font.body=txt; font.size=size
            ob=bpy.data.objects.new(f'REVIEW_{filename}_Label{j}',font); review.objects.link(ob); ob.parent=cam; ob.location=(-half*.93,y,-10); font.materials.append(textmat)
            captions.append((cam,ob))
    for cam,(filename,*_) in zip(cameras,views):
        scene.camera=cam; envelope.hide_render=filename!='04_top'
        for owner,ob in captions: ob.hide_render=owner!=cam
        scene.render.filepath=str(OUT/(filename+'.png')); bpy.ops.render.render(write_still=True); report['renders'].append(scene.render.filepath)
    scene.camera=cameras[0]; envelope.hide_render=True
    for owner,ob in captions: ob.hide_render=owner!=cameras[0]; ob.hide_set(True)
    root.select_set(True); bpy.context.view_layer.objects.active=root
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type=='VIEW_3D':
                r=area.spaces.active.region_3d; r.view_distance=60; r.view_location=(0,0,8); r.view_rotation=cameras[0].rotation_euler.to_quaternion()
                area.spaces.active.shading.color_type='MATERIAL'
    bpy.ops.wm.save_as_mainfile(filepath=str(HERE/'TownHall.blend'))
    report.update(success=True,blender=bpy.app.version_string,dimensions=dims,bounds_min=lo,bounds_max=hi,clean_transforms=True,
        retained_meshes=retained,root=root.name,root_position=list(root.location),placement_envelope=[40,40],fits_envelope=True,
        hierarchy={name:[o.name for o in meshes if o.parent==g] for name,g in groups.items()},mesh_count=len(meshes),
        materials={m.name:list(m.diffuse_color) for m in (white,structural,dark,glass,orange)},
        source=str(HERE/'TownHall.blend'),building_only=True,foundation_geometry=False,guide_plane=False,unity_touched=False,fbx_exported=False)
except Exception:
    report.update(success=False,error=traceback.format_exc()); raise
finally:
    (OUT/'blockout-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
