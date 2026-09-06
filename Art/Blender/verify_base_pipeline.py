"""Read-only Blender verification of saved source and mask; writes evidence only."""
import bpy, bmesh, json, traceback
from pathlib import Path
from mathutils import Vector

HERE=Path(__file__).resolve().parent
OUT=HERE.parent/'Previews'/'HuskBaseModule'/'pipeline-verification.json'
result={}
try:
    bpy.ops.wm.open_mainfile(filepath=str(HERE/'HuskBaseModule.blend'))
    model=bpy.data.collections['HuskBaseModule_MODEL']; root=bpy.data.objects['HuskBaseModule']
    image=bpy.data.images.load(str(HERE/'Textures'/'HuskBaseModule_PipelineMask.png'),check_existing=False)
    image.colorspace_settings.name='Non-Color'; image.alpha_mode='CHANNEL_PACKED'
    pixels=list(image.pixels)
    assert set(pixels)<={0.0,1.0}
    assert all(sum(pixels[i:i+3])<=1 and pixels[i+3]==0 for i in range(0,len(pixels),4))
    counts=[sum(pixels[i+j]==1 for i in range(0,len(pixels),4)) for j in range(3)]
    assert counts==[14336]*3
    checks={}; samples=0
    for o in model.objects:
        if o.type!='MESH': continue
        assert o.parent and o.parent.parent==root
        assert o.data.uv_layers[0].name=='PipelineMaskUV'
        uv=o.data.uv_layers[0].data
        lane=int(o.name[-2:])-1 if o.name.startswith('Lane_') else None
        target=[0,0,0]
        if lane is not None: target[lane]=1
        for p in o.data.polygons:
            # UVs are affine inside a single constant-ID rectangle. Include centroid and corners.
            coords=[uv[i].uv for i in p.loop_indices]
            coords.append(sum(coords,Vector((0,0)))/len(coords))
            for v in coords:
                i=(int(v.y*256)*256+int(v.x*256))*4
                assert pixels[i:i+3]==target,(o.name,v,pixels[i:i+3]); samples+=1
        if lane is not None:
            assert o.data.materials[0].name=='Husk_Pipeline_Neutral'
            assert all(abs(a-b)<1e-6 for a,b in zip(o.color,(.18,.21,.23,1)))
            bm=bmesh.new(); bm.from_mesh(o.data)
            assert all(e.is_manifold for e in bm.edges)
            bm.verts.ensure_lookup_table()
            seen=set(); stack=[bm.verts[0]]
            while stack:
                v=stack.pop()
                if v in seen: continue
                seen.add(v); stack.extend(e.other_vert(v) for e in v.link_edges)
            assert len(seen)==len(bm.verts)
            vol=bm.calc_volume(signed=True); assert vol>0
            checks[o.name]={'connected':True,'watertight':True,'outward_winding_signed_volume_m3':vol}; bm.free()
    mat=bpy.data.materials['Husk_Pipeline_Neutral']
    shader=next(n for n in mat.node_tree.nodes if n.type=='BSDF_PRINCIPLED')
    assert shader.inputs['Emission Strength'].default_value==0
    assert not any(m.name.startswith('REVIEW_ONLY_TemporaryLaneColor') for m in bpy.data.materials)
    result.update(success=True,source_reopened=True,png_binary_values_only=True,alpha_zero=True,channel_overlap_pixels=0,white_pixels_per_channel=counts,
        all_model_uv_samples_checked=samples,non_pipeline_samples_black=True,lanes=checks,production_material_neutral=True,production_emission_zero=True,temporary_lane_materials_removed=True)
except Exception:
    result.update(success=False,error=traceback.format_exc()); raise
finally:
    OUT.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
