"""Edit existing base: neutral geometry + deterministic RGB lane-ID atlas. Blender only."""
import bpy, bmesh, json, math, struct, zlib, traceback
from pathlib import Path
from mathutils import Vector

HERE=Path(__file__).resolve().parent
OUT=HERE.parent/'Previews'/'HuskBaseModule'
TEX=HERE/'Textures'/'HuskBaseModule_PipelineMask.png'
REPORT=OUT/'pipeline-report.json'
report={'stage':'technical-pipeline-integration-awaiting-approval','renders':[]}
WIDTH=.5; SPACING=.8; OFFSETS=(-.8,0,.8); LEVELS=(4.52,4.32,4.12); EDGE_Z=4.75

def bounds(objects):
    ps=[o.matrix_world@v.co for o in objects for v in o.data.vertices]
    return [min(p[i] for p in ps) for i in range(3)],[max(p[i] for p in ps) for i in range(3)]

def reshape(o,c,s):
    lo=[min(v.co[i] for v in o.data.vertices) for i in range(3)]
    hi=[max(v.co[i] for v in o.data.vertices) for i in range(3)]
    for v in o.data.vertices: v.co=[(v.co[i]-(lo[i]+hi[i])*.5)/(hi[i]-lo[i])*s[i]+c[i] for i in range(3)]
    o.data.update()

try:
    bpy.ops.wm.open_mainfile(filepath=str(HERE/'HuskBaseModule.blend'))
    bpy.context.preferences.filepaths.save_version=0
    scene=bpy.context.scene; model=bpy.data.collections['HuskBaseModule_MODEL']
    review=bpy.data.collections['REVIEW_ONLY_Cameras_Guides']; root=bpy.data.objects['HuskBaseModule']
    for o in list(review.objects): bpy.data.objects.remove(o,do_unlink=True)
    groups={o.name:o for o in root.children if o.type=='EMPTY'}
    for name in ['PipelineHousing','PipelineSurface']:
        if name in groups:
            for o in list(groups[name].children): bpy.data.objects.remove(o,do_unlink=True)
        else:
            o=bpy.data.objects.new(name,None); model.objects.link(o); o.parent=root; groups[name]=o
    def mesh(name,vs,fs,mat,group):
        data=bpy.data.meshes.new(name+'_Geometry'); data.from_pydata(vs,[],fs); data.update()
        bm=bmesh.new(); bm.from_mesh(data); bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces)); bm.to_mesh(data); bm.free()
        o=bpy.data.objects.new(name,data); model.objects.link(o); o.parent=groups[group]; data.materials.append(mat); return o
    def box(name,c,s,mat,group):
        x,y,z=c; a,b,h=[v/2 for v in s]
        vs=[(x-a,y-b,z-h),(x+a,y-b,z-h),(x+a,y+b,z-h),(x-a,y+b,z-h),
            (x-a,y-b,z+h),(x+a,y-b,z+h),(x+a,y+b,z+h),(x-a,y+b,z+h)]
        return mesh(name,vs,[(3,2,1,0),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)],mat,group)
    dark=bpy.data.materials['Husk_Hull_Blockout']; structure=bpy.data.materials['Husk_StructureGray_Blockout']
    # Edit existing deck panels: four usable quadrants, central 3.4 m service trench.
    for o in model.objects:
        if o.name.startswith('Deck_Panel_'):
            lo,hi=bounds([o]); sx=1 if lo[0]+hi[0]>0 else -1; sy=1 if lo[1]+hi[1]>0 else -1
            outerx=max(abs(lo[0]),abs(hi[0])); outery=max(abs(lo[1]),abs(hi[1]))
            reshape(o,(sx*(outerx+1.7)/2,sy*(outery+1.7)/2,4.84),(outerx-1.7,outery-1.7,.32))
    # Lower the existing sealed core ceiling; refill only under the deck quadrants.
    core=bpy.data.objects['MainBody_Core']; lo,hi=bounds([core])
    reshape(core,(0,0,(lo[2]+3.85)/2),(hi[0]-lo[0],hi[1]-lo[1],3.85-lo[2]))
    for sx in (-1,1):
        for sy in (-1,1):
            box(f'Pipeline_DeckSupport_{sx}_{sy}',(sx*5.55,sy*5.55,4.25),(7.7,7.7,.8),structure,'PipelineHousing')
    box('Pipeline_TrenchBed_NS',(0,0,3.925),(3.4,20,.15),dark,'PipelineHousing')
    for sign in (-1,1):
        box(f'Pipeline_TrenchBed_EW_{sign}',(sign*5.85,0,3.925),(8.3,3.4,.15),dark,'PipelineHousing')
    # Four arms, raised solid bed below the visible strips, stopping at center hub.
    for axis in (0,1):
        for sign in (-1,1):
            c=[0,0,4.29]; s=[3.4,3.4,.58]; c[axis]=sign*6.05; s[axis]=7.9
            box(f'Pipeline_ArmBed_{axis}_{sign}',c,s,dark,'PipelineHousing')
            for rail in (-1,1):
                c=[0,0,4.81]; s=[.22,.22,.38]; c[axis]=sign*6; c[1-axis]=rail*1.48; s[axis]=8
                box(f'Pipeline_TrenchRim_{axis}_{sign}_{rail}',c,s,structure,'PipelineHousing')
    lid=box('Pipeline_CenterCover',(0,0,4.925),(2.85,2.85,.15),structure,'PipelineHousing')
    lid['review']='Hide for inspection of three vertically separated continuous crossings; production cover remains attached.'
    # Lossless ID atlas: three disjoint islands, black unused texels, alpha reserved=0.
    # Constant-ID regions permit a compact mask without edge aliasing in visible strips.
    TEX.parent.mkdir(parents=True,exist_ok=True)
    raw=bytearray(256*256*4); counts=[0,0,0]; rectangles=[]
    for lane in range(3):
        x0=16+80*lane; x1=x0+64; rectangles.append([x0,16,x1,240])
        for y in range(16,240):
            for x in range(x0,x1): raw[(y*256+x)*4+lane]=255; counts[lane]+=1
    def chunk(t,d): return struct.pack('>I',len(d))+t+d+struct.pack('>I',zlib.crc32(t+d)&0xffffffff)
    rows=b''.join(b'\0'+raw[y*1024:(y+1)*1024] for y in reversed(range(256)))
    TEX.write_bytes(b'\x89PNG\r\n\x1a\n'+chunk(b'IHDR',struct.pack('>IIBBBBB',256,256,8,6,0,0,0))+chunk(b'IDAT',zlib.compress(rows))+chunk(b'IEND',b''))
    image=bpy.data.images.load(str(TEX),check_existing=True); image.reload(); image.colorspace_settings.name='Non-Color'; image.alpha_mode='CHANNEL_PACKED'
    image.filepath='//Textures/HuskBaseModule_PipelineMask.png'
    mat=bpy.data.materials.get('Husk_Pipeline_Neutral') or bpy.data.materials.new('Husk_Pipeline_Neutral')
    mat.use_nodes=True; mat.diffuse_color=(.18,.21,.23,1); mat.node_tree.nodes.clear()
    nt=mat.node_tree; output=nt.nodes.new('ShaderNodeOutputMaterial'); shader=nt.nodes.new('ShaderNodeBsdfPrincipled')
    shader.inputs['Base Color'].default_value=mat.diffuse_color; shader.inputs['Roughness'].default_value=.72; shader.inputs['Emission Strength'].default_value=0
    nt.links.new(shader.outputs['BSDF'],output.inputs['Surface'])
    uvnode=nt.nodes.new('ShaderNodeUVMap'); uvnode.uv_map='PipelineMaskUV'; uvnode.label='UV0 / Unity TEXCOORD0'
    texnode=nt.nodes.new('ShaderNodeTexImage'); texnode.image=image; texnode.interpolation='Closest'; texnode.extension='EXTEND'; texnode.label='DATA ONLY: RGB lane IDs, no resource colors'
    nt.links.new(uvnode.outputs['UV'],texnode.inputs['Vector']); texnode.location=(-300,-250); uvnode.location=(-550,-250)
    lanes=[]
    for lane,(offset,level) in enumerate(zip(OFFSETS,LEVELS)):
        edges=sorted(set([-10,-2.1,-1.4,offset-WIDTH/2,offset+WIDTH/2,1.4,2.1,10]))
        verts=[]; faces=[]; lookup={}
        def vertex(x,y):
            key=(x,y)
            if key not in lookup:
                t=min(1,max(0,(max(abs(x),abs(y))-1.4)/.7))
                lookup[key]=len(verts); verts.append((x,y,level+(EDGE_Z-level)*t))
            return lookup[key]
        for a,b in zip(edges,edges[1:]):
            for c,d in zip(edges,edges[1:]):
                if abs((a+b)/2-offset)<WIDTH/2-1e-6 or abs((c+d)/2-offset)<WIDTH/2-1e-6:
                    faces.append((vertex(a,c),vertex(b,c),vertex(b,d),vertex(a,d)))
        # Add 8 cm thickness, sharing boundary edges; one connected watertight mesh per lane.
        n=len(verts); verts+= [(x,y,z-.08) for x,y,z in verts[:]]
        topfaces=faces[:]; faces += [tuple(v+n for v in reversed(f)) for f in topfaces]
        edge_counts={}
        for f in topfaces:
            for a,b in zip(f,f[1:]+f[:1]):
                key=tuple(sorted((a,b))); edge_counts[key]=edge_counts.get(key,0)+1
        for f in topfaces:
            for a,b in zip(f,f[1:]+f[:1]):
                if edge_counts[tuple(sorted((a,b)))]==1: faces.append((a,a+n,b+n,b))
        o=mesh(f'Lane_{lane+1:02}',verts,faces,mat,'PipelineSurface'); lanes.append(o)
        uv=o.data.uv_layers.new(name='PipelineMaskUV')
        # Stable affine mapping, inside an 8px safe margin in the assigned mask island.
        for loop in o.data.loops:
            p=o.data.vertices[loop.vertex_index].co
            uv.data[loop.index].uv=((24+80*lane+(p.x+10)/20*48)/256,(24+(p.y+10)/20*208)/256)
        o['mask_channel']='RGB'[lane]; o['mask_uv']='UV0 / PipelineMaskUV'; o['resource_assignment']='NONE; runtime only'
    bpy.context.view_layer.update()
    meshes=[o for o in model.objects if o.type=='MESH']
    # All non-pipeline surfaces sample the black sentinel in UV0, even after a future mesh merge.
    for o in meshes:
        if o in lanes: continue
        uv=o.data.uv_layers[0] if o.data.uv_layers else o.data.uv_layers.new(name='PipelineMaskUV')
        uv.name='PipelineMaskUV'
        for v in uv.data: v.uv=(4/256,4/256)
    lo,hi=bounds([o for o in meshes if o.parent!=groups['Feet']]); dims=[hi[i]-lo[i] for i in range(3)]
    assert all(abs(a-b)<1e-5 for a,b in zip(dims,(20,20,8))),dims
    assert all(tuple(o.location)==(0,0,0) and tuple(o.rotation_euler)==(0,0,0) and tuple(o.scale)==(1,1,1) for o in model.objects)
    # Validate saved PNG values, and sample every UV vertex and every polygon centroid.
    pixels=list(image.pixels); overlap=sum(sum(pixels[i+j]>.5 for j in range(3))>1 for i in range(0,len(pixels),4))
    assert overlap==0 and all(pixels[i]==0 for i in range(3,len(pixels),4))
    sampled=0; edges_report={}
    for k,o in enumerate(lanes):
        uvs=o.data.uv_layers[0].data
        points=[v.uv.copy() for v in uvs]
        for p in o.data.polygons: points.append(sum((uvs[i].uv for i in p.loop_indices),Vector((0,0)))/p.loop_total)
        for uv in points:
            index=(min(255,int(uv.y*256))*256+min(255,int(uv.x*256)))*4
            assert all(abs(pixels[index+j]-(1 if j==k else 0))<1e-6 for j in range(3)); sampled+=1
        bm=bmesh.new(); bm.from_mesh(o.data); assert all(e.is_manifold for e in bm.edges); bm.free()
        for axis,label in [(0,'EastWest'),(1,'NorthSouth')]:
            ends=[]
            for sign in (-1,1):
                vv=[v.co for v in o.data.vertices if abs(v.co[axis]-sign*10)<1e-5]
                ends.append(sorted((round(v[1-axis],5),round(v.z,5)) for v in vv))
            assert ends[0]==ends[1]
            edges_report[f'Lane{k+1}_{label}']={'opposite_edge_profiles_identical':True,'pitch_m':20,'gap_m':0,'profile':ends[0]}
    # All crossings fit in |x|,|y|<=1.05, where separate levels give 12cm clearance.
    assert min(abs(a-b) for a,b in zip(LEVELS,LEVELS[1:]))-.08>.1199
    root['stage']='Three neutral physical lanes + RGB mask, UV0; technical blockout awaiting approval'
    root['lane_order']='N/S increasing world X: 01,02,03; E/W increasing world Y: 01,02,03. Translation-only 20m grid; rotation remapping not implemented.'
    # Workbench preview colors are assigned only while rendering, not saved on production objects.
    original_colors={o.name:tuple(o.color) for o in meshes}
    for o in meshes: o.color=o.data.materials[0].diffuse_color
    neutral_colors={o.name:tuple(o.color) for o in meshes}
    temp=[(.1,.65,1,1),(.62,.19,.9,1),(1,.62,.12,1)]
    temporary_materials=[]
    for o,c in zip(lanes,temp):
        o.color=c
        pm=bpy.data.materials.new('REVIEW_ONLY_TemporaryLaneColor'); pm.diffuse_color=c; temporary_materials.append(pm)
        o.data.materials[0]=pm
    instance=bpy.data.objects.new('REVIEW_ONLY_Adjacent20m_Instance',None); review.objects.link(instance)
    instance.instance_type='COLLECTION'; instance.instance_collection=model; instance.location=(20,0,0)
    scene.render.engine='BLENDER_WORKBENCH'; scene.render.resolution_x=1400; scene.render.resolution_y=1100; scene.render.resolution_percentage=100
    scene.display.render_aa='32'; sh=scene.display.shading; sh.light='STUDIO'; sh.color_type='OBJECT'; sh.show_shadows=False; sh.show_cavity=True; sh.cavity_type='BOTH'
    scene.render.image_settings.file_format='PNG'; scene.view_settings.view_transform='Standard'
    textmat=bpy.data.materials['REVIEW_Text']
    def label(body,cam,x,y,size):
        data=bpy.data.curves.new('REVIEW_Label','FONT'); data.body=body; data.size=size
        o=bpy.data.objects.new('REVIEW_Label',data); review.objects.link(o); o.parent=cam; o.location=(x,y,-10); o.color=textmat.diffuse_color; data.materials.append(textmat); return o
    views=[('pipeline_01_isometric',(32,-40,34),(0,0,1.4),37,'THREE LANES / TEMPORARY ID COLORS'),
        ('pipeline_02_top',(0,0,45),(0,0,1),34,'TOP / 20 m GRID / LANE IDs ONLY'),
        ('pipeline_03_lanes',(10,-15,18),(0,-5.5,4.7),11,'INSET LANES / WIDTH 0.50 / PITCH 0.80 m'),
        ('pipeline_04_center',(8,-11,14),(0,0,4.3),7.5,'CENTER / COVER HIDDEN / THREE ROUTING LEVELS'),
        ('pipeline_05_adjacent',(38,-50,51),(10,0,1.5),59,'IDENTICAL INSTANCES / 20 m PITCH / NO GAP'),
        ('pipeline_06_mask_diagnostic',(0,0,60),(0,0,4.5),75,'MASK DIAGNOSTIC / UV0 / CHANNEL ISOLATION')]
    cams=[]; texts=[]
    for filename,loc,target,scale,title in views:
        data=bpy.data.cameras.new('REVIEW_'+filename); cam=bpy.data.objects.new('REVIEW_'+filename,data); review.objects.link(cam)
        cam.location=loc; cam.rotation_euler=(Vector(target)-cam.location).to_track_quat('-Z','Y').to_euler(); data.type='ORTHO'; data.ortho_scale=scale; data.clip_end=300
        cams.append(cam); half=scale/2; vertical=half*1100/1400
        for txt,y,size in [('HUSK / BASE MODULE PIPELINES',vertical*.91,half*.05),(title,vertical*.81,half*.029),('TECHNICAL BLOCKOUT | NEUTRAL PRODUCTION ASSET | NO RESOURCE ASSIGNMENT',-vertical*.92,half*.022)]:
            texts.append((cam,label(txt,cam,-half*.93,y,size)))
        if filename in ('pipeline_03_lanes','pipeline_04_center'):
            background=bpy.data.materials.get('REVIEW_CaptionBackground') or bpy.data.materials.new('REVIEW_CaptionBackground')
            background.diffuse_color=(.035,.05,.065,1)
            for bottom,top in [(vertical*.75,vertical),(-vertical,-vertical*.865)]:
                dat=bpy.data.meshes.new('REVIEW_CaptionBar'); dat.from_pydata([(-half,bottom,0),(half,bottom,0),(half,top,0),(-half,top,0)],[],[(0,1,2,3)])
                bar=bpy.data.objects.new('REVIEW_CaptionBar',dat); review.objects.link(bar); bar.parent=cam; bar.location=(0,0,-10.1); dat.materials.append(background); texts.append((cam,bar))
    # Diagnostic copies are linked to the actual UV'd lane meshes; color comes from saved mask samples.
    diagnostic=[]; diaglabels=[]
    for k,o in enumerate(lanes):
        clone=bpy.data.objects.new(f'REVIEW_ONLY_Mask_{k+1}',o.data); review.objects.link(clone); clone.location.x=(k-1)*24
        uv=o.data.uv_layers[0].data[0].uv; idx=(int(uv.y*256)*256+int(uv.x*256))*4
        clone.color=(*pixels[idx:idx+3],1); diagnostic.append(clone)
        diaglabels.append(label(f'{"RGB"[k]} / LANE {k+1:02}',cams[-1],(k-1)*24-8,13,1.25))
    for cam,(filename,*_) in zip(cams,views):
        scene.camera=cam; diagnostic_mode=filename=='pipeline_06_mask_diagnostic'
        sh.color_type='OBJECT' if diagnostic_mode else 'MATERIAL'
        instance.hide_render=filename!='pipeline_05_adjacent'
        for owner,o in texts: o.hide_render=owner!=cam
        for o in diagnostic+diaglabels: o.hide_render=not diagnostic_mode
        for o in meshes: o.hide_render=diagnostic_mode
        lid.hide_render=diagnostic_mode or filename=='pipeline_04_center'
        scene.render.filepath=str(OUT/(filename+'.png')); bpy.ops.render.render(write_still=True); report['renders'].append(scene.render.filepath)
    for o in meshes: o.hide_render=False; o.color=neutral_colors[o.name]
    for o in lanes: o.data.materials[0]=mat
    for pm in temporary_materials: bpy.data.materials.remove(pm)
    for o in diagnostic+diaglabels: o.hide_render=True; o.hide_set(True)
    instance.hide_render=True; instance.hide_set(True)
    for owner,o in texts: o.hide_render=True; o.hide_set(True)
    scene.camera=cams[0]; sh.color_type='MATERIAL'
    for screen in bpy.data.screens:
        for area in screen.areas:
            if area.type=='VIEW_3D':
                r=area.spaces.active.region_3d; r.view_distance=38; r.view_location=(0,0,1); r.view_rotation=cams[0].rotation_euler.to_quaternion(); area.spaces.active.shading.color_type='MATERIAL'
    bpy.ops.wm.save_as_mainfile(filepath=str(HERE/'HuskBaseModule.blend'))
    report.update(success=True,blender=bpy.app.version_string,mask_path=str(TEX),mask_resolution=[256,256],mask_type='RGBA8 linear-data lane-ID atlas; NOT top-down projection',mask_alpha=0,
        mask_rectangles_pixels=rectangles,channel_white_pixel_counts=counts,overlapping_channel_pixels=overlap,uv_samples_verified=sampled,uv_channel='UV0 / PipelineMaskUV / Unity TEXCOORD0',non_pipeline_uv0_black=True,
        dimensions=dims,main_bounds_min=lo,main_bounds_max=hi,waterline_z=0,feet_count=4,feet_height=.45,clean_transforms=True,lane_width_m=WIDTH,lane_center_pitch_m=SPACING,lane_clear_gap_m=SPACING-WIDTH,
        lane_offsets_m=OFFSETS,edge_surface_z=EDGE_Z,center_surface_z=LEVELS,center_thickness_m=.08,center_clearance_m=.12,opposite_edges=edges_report,
        hierarchy={name:[o.name for o in meshes if o.parent==g] for name,g in groups.items()},source=str(HERE/'HuskBaseModule.blend'),production_resource_colors=False,unity_touched=False,fbx_exported=False)
except Exception:
    report.update(success=False,error=traceback.format_exc()); raise
finally:
    REPORT.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
