"""Building-only Town Hall blockout. No base modules, scene imports, Unity or FBX."""
import bpy, bmesh, json, traceback
from pathlib import Path
from mathutils import Vector

HERE=Path(__file__).resolve().parent
OUT=HERE.parent/'Previews'/'TownHall'
OUT.mkdir(parents=True,exist_ok=True)
report={'stage':'architectural-blockout-awaiting-approval','renders':[]}

try:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.preferences.filepaths.save_version=0
    scene=bpy.context.scene
    scene.unit_settings.system='METRIC'; scene.unit_settings.scale_length=1; scene.unit_settings.length_unit='METERS'
    model=bpy.data.collections.new('TownHall_MODEL'); scene.collection.children.link(model)
    review=bpy.data.collections.new('REVIEW_ONLY_Cameras_Envelope'); scene.collection.children.link(review)
    root=bpy.data.objects.new('TownHall_Root',None); model.objects.link(root)
    root['origin']='Bottom-center of building bounds at deck contact Z=0. +Y front; +Z up.'
    root['scope']='BUILDING ONLY; 40x40 m is placement envelope, not geometry'
    groups={}
    for name in ['PrimaryShell','ObservationBand','CommandCore','Entrance','StructuralFrames','Accents']:
        o=bpy.data.objects.new(name,None); model.objects.link(o); o.parent=root; groups[name]=o
    def material(name,color):
        m=bpy.data.materials.new(name); m.diffuse_color=(*color,1); m.use_nodes=True
        shader=m.node_tree.nodes.get('Principled BSDF'); shader.inputs['Base Color'].default_value=(*color,1)
        shader.inputs['Roughness'].default_value=.72
        return m
    white=material('TownHall_Shell_LightGray',(.78,.80,.78))
    structural=material('TownHall_StructureGray',(.20,.25,.27))
    dark=material('TownHall_Hull_Dark',(.12,.20,.23))
    glass=material('TownHall_ObservationGlass',(.045,.10,.135))
    orange=material('TownHall_SafetyOrange',(.90,.32,.08))
    guide=material('REVIEW_Envelope',(.15,.48,.54)); textmat=material('REVIEW_Text',(.80,.87,.89))
    def mesh(name,vertices,faces,mat,group):
        data=bpy.data.meshes.new(name+'_Geometry'); data.from_pydata(vertices,[],faces); data.update()
        bm=bmesh.new(); bm.from_mesh(data); bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces)); bm.to_mesh(data); bm.free()
        o=bpy.data.objects.new(name,data); model.objects.link(o); o.parent=groups[group]; data.materials.append(mat)
        return o
    def octagon(w,d,c,z,center=(0,-1)):
        x=w/2; y=d/2
        pts=[(x-c,-y),(x,-y+c),(x,y-c),(x-c,y),(-x+c,y),(-x,y-c),(-x,-y+c),(-x+c,-y)]
        return [(a+center[0],b+center[1],z) for a,b in pts]
    def volume(name,lower,upper,mat,group):
        n=len(lower); verts=lower+upper
        faces=[tuple(reversed(range(n))),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
        return mesh(name,verts,faces,mat,group)
    def box(name,c,s,mat,group):
        x,y,z=c; a,b,h=[v/2 for v in s]
        lo=[(x-a,y-b,z-h),(x+a,y-b,z-h),(x+a,y+b,z-h),(x-a,y+b,z-h)]
        return volume(name,lo,[(u,v,z+h) for u,v,_ in lo],mat,group)
    # The lower volume is enclosed building shell, not a platform or foundation slab.
    volume('Shell_LowerEnclosure',octagon(31.5,29.5,3,0),octagon(31.5,29.5,3,4.3),white,'PrimaryShell')
    volume('Shell_StormShoulder',octagon(32,30,3,4.3),octagon(28,26,3,5.7),white,'PrimaryShell')
    volume('Observation_OrangeSill',octagon(28.1,26.1,3,5.55),octagon(27.9,25.9,3,5.82),orange,'Accents')
    low=octagon(27.8,25.8,3,5.82); high=octagon(24.2,22.2,3,8.15)
    volume('Observation_StructuralBacking',low,high,dark,'ObservationBand')
    # Large glass panels separated by structural mullions, no texture detailing.
    for i in range(8):
        a,b=Vector(low[i]),Vector(low[(i+1)%8]); c,d=Vector(high[i]),Vector(high[(i+1)%8])
        normal=(b-a).cross(c-a).normalized()
        count=3 if (b-a).length>8 else 1
        for j in range(count):
            start=(j+.055)/count; end=(j+.945)/count
            bottom0=a.lerp(b,start); bottom1=a.lerp(b,end); top0=c.lerp(d,start); top1=c.lerp(d,end)
            verts=[bottom0.lerp(top0,.08),bottom1.lerp(top1,.08),bottom1.lerp(top1,.92),bottom0.lerp(top0,.92)]
            mesh(f'Observation_Glass_{i:02}_{j:02}',[v+normal*.025 for v in verts],[(0,1,2,3)],glass,'ObservationBand')
    volume('Shell_UpperRoofCap',octagon(24.8,22.8,3,8.15),octagon(23.8,21.8,3,8.8),white,'PrimaryShell')
    # Short protected core replaces the reference's fragile mast/dish silhouette.
    volume('Command_CoreEnclosure',octagon(7.8,8.2,1,8.8,(0,-2)),octagon(7.8,8.2,1,12.35,(0,-2)),white,'CommandCore')
    volume('Command_ArmoredCrown',octagon(8.1,8.5,1,12.35,(0,-2)),octagon(7.1,7.5,1,13,(0,-2)),structural,'CommandCore')
    box('Command_FrontRecess',(0,2.13,10.7),(2.4,.08,3),dark,'CommandCore')
    box('Command_OrangeSpine',(0,2.185,10.7),(.65,.05,2.7),orange,'Accents')
    box('Command_LeftObservation',(-3.925,-2,11.55),(.05,4.8,.65),glass,'CommandCore')
    box('Command_RightObservation',(3.925,-2,11.55),(.05,4.8,.65),glass,'CommandCore')
    # Storm bracing belongs to the facade; no free-standing props or platform edges.
    for sign in (-1,1):
        for offset in (-9.6,9.6):
            box(f'Frame_Facade_{sign}_{offset}',(offset,sign*14.6-1,2.25),(1.3,.8,4.5),structural,'StructuralFrames')
        for offset in (-8.0,8.0):
            box(f'Frame_Side_{sign}_{offset}',(sign*15.65,offset-1,2.25),(.7,1.3,4.5),structural,'StructuralFrames')
    box('Entrance_Recess',(0,14.10,2.0),(5.2,.2,4),dark,'Entrance')
    for sign in (-1,1):
        volume(f'Entrance_ArmoredJamb_{sign}',
            [(sign*3.0-.65,13.8,0),(sign*3.0+.65,13.8,0),(sign*3.0+.65,16,0),(sign*3.0-.65,16,0)],
            [(sign*2.7-.55,13.8,4.6),(sign*2.7+.55,13.8,4.6),(sign*2.7+.55,15.5,4.6),(sign*2.7-.55,15.5,4.6)],white,'Entrance')
        box(f'Entrance_DoorLeaf_{sign}',(sign*.91,14.24,1.7),(1.76,.12,3.4),structural,'Entrance')
        box(f'Entrance_OrangeEdge_{sign}',(sign*2.1,14.27,1.9),(.18,.08,2.8),orange,'Accents')
    box('Entrance_StormLintel',(0,14.7,4.55),(6.8,2.0,.65),structural,'Entrance')
    bpy.context.view_layer.update()
    meshes=[o for o in model.objects if o.type=='MESH']
    pts=[o.matrix_world@v.co for o in meshes for v in o.data.vertices]
    lo=[min(v[i] for v in pts) for i in range(3)]; hi=[max(v[i] for v in pts) for i in range(3)]
    dims=[hi[i]-lo[i] for i in range(3)]
    assert all(abs(a-b)<1e-5 for a,b in zip(dims,(32,32,13))),dims
    assert lo[2]==0 and all(-20<=v.x<=20 and -20<=v.y<=20 for v in pts)
    assert all(tuple(o.location)==(0,0,0) and tuple(o.rotation_euler)==(0,0,0) and tuple(o.scale)==(1,1,1) for o in model.objects)
    # Only a thin outline is needed to communicate the envelope; no guide plane/base.
    data=bpy.data.curves.new('REVIEW_40mEnvelope','CURVE'); data.dimensions='3D'; data.bevel_depth=.025; data.bevel_resolution=0
    s=data.splines.new('POLY'); s.points.add(3)
    for p,co in zip(s.points,[(-20,-20,0,1),(20,-20,0,1),(20,20,0,1),(-20,20,0,1)]): p.co=co
    s.use_cyclic_u=True
    envelope=bpy.data.objects.new('REVIEW_ONLY_40x40m_Envelope_NotBuilding',data); review.objects.link(envelope); data.materials.append(guide)
    scene.render.engine='BLENDER_WORKBENCH'; scene.render.resolution_x=1400; scene.render.resolution_y=1100; scene.render.resolution_percentage=100
    scene.render.image_settings.file_format='PNG'; scene.display.render_aa='32'
    sh=scene.display.shading; sh.light='STUDIO'; sh.color_type='MATERIAL'; sh.show_shadows=False; sh.show_cavity=True; sh.cavity_type='BOTH'
    sh.curvature_ridge_factor=1.15; sh.curvature_valley_factor=1; sh.background_type='WORLD'
    scene.world=bpy.data.worlds.new('REVIEW_Background'); scene.world.color=(.035,.05,.065); scene.view_settings.view_transform='Standard'
    views=[('01_isometric',(45,55,42),(0,0,4),62,'ISOMETRIC / BUILDING ONLY'),
        ('02_front',(0,65,6),(0,0,6),46,'FRONT / +Y'),('03_side',(65,0,6),(0,0,6),46,'SIDE / +X'),
        ('04_top',(0,0,70),(0,0,0),66,'TOP / 40 m PLACEMENT ENVELOPE')]
    cameras=[]; captions=[]
    for filename,loc,target,scale,title in views:
        data=bpy.data.cameras.new('REVIEW_'+filename); cam=bpy.data.objects.new('REVIEW_'+filename,data); review.objects.link(cam)
        cam.location=loc; cam.rotation_euler=(Vector(target)-cam.location).to_track_quat('-Z','Y').to_euler(); data.type='ORTHO'; data.ortho_scale=scale; data.clip_end=300
        cameras.append(cam); half=scale/2; vertical=half*1100/1400
        for j,(txt,y,size) in enumerate([('HUSK / TOWN HALL',vertical*.91,half*.055),(title,vertical*.80,half*.032),
            ('BLOCKOUT 32 x 32 x 13 m | CONTACT Z=0 | NO FOUNDATION',-vertical*.92,half*.027)]):
            font=bpy.data.curves.new('REVIEW_Label','FONT'); font.body=txt; font.size=size
            ob=bpy.data.objects.new(f'REVIEW_{filename}_Label{j}',font); review.objects.link(ob); ob.parent=cam; ob.location=(-half*.93,y,-10); font.materials.append(textmat)
            captions.append((cam,ob))
    for cam,(filename,*_) in zip(cameras,views):
        scene.camera=cam; envelope.hide_render=filename in ('02_front','03_side')
        for owner,ob in captions: ob.hide_render=owner!=cam
        scene.render.filepath=str(OUT/(filename+'.png')); bpy.ops.render.render(write_still=True)
        report['renders'].append(scene.render.filepath)
    scene.camera=cameras[0]; envelope.hide_render=False
    for owner,ob in captions: ob.hide_render=owner!=cameras[0]; ob.hide_set(True)
    root.select_set(True); bpy.context.view_layer.objects.active=root
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type=='VIEW_3D':
                r=area.spaces.active.region_3d; r.view_distance=58; r.view_location=(0,0,5); r.view_rotation=cameras[0].rotation_euler.to_quaternion()
                area.spaces.active.shading.color_type='MATERIAL'
    bpy.ops.wm.save_as_mainfile(filepath=str(HERE/'TownHall.blend'))
    report.update(success=True,blender=bpy.app.version_string,dimensions=dims,bounds_min=lo,bounds_max=hi,clean_transforms=True,
        root=root.name,root_position=list(root.location),placement_envelope=[40,40],fits_envelope=True,
        hierarchy={name:[o.name for o in meshes if o.parent==g] for name,g in groups.items()},mesh_count=len(meshes),
        materials={m.name:list(m.diffuse_color) for m in (white,structural,dark,glass,orange)},
        source=str(HERE/'TownHall.blend'),building_only=True,foundation_geometry=False,guide_plane=False,unity_touched=False,fbx_exported=False)
except Exception:
    report.update(success=False,error=traceback.format_exc()); raise
finally:
    (OUT/'blockout-report.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
