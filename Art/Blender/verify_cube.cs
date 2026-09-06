var path = "Assets/Husk/Models/CalibrationCube_2m.fbx";
var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);
if (!asset) throw new System.Exception("Calibration asset missing");
var importer = (UnityEditor.ModelImporter)UnityEditor.AssetImporter.GetAtPath(path);
var transforms = asset.GetComponentsInChildren<UnityEngine.Transform>(true);
var filters = asset.GetComponentsInChildren<UnityEngine.MeshFilter>(true);
if (filters.Length != 1) throw new System.Exception("Expected exactly one mesh");
var filter = filters[0];
var mesh = filter.sharedMesh;
var matrix = asset.transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
var vertices = mesh.vertices.Select(v => matrix.MultiplyPoint3x4(v)).ToArray();
var normals = mesh.normals.Select(n => matrix.inverse.transpose.MultiplyVector(n).normalized).ToArray();
var colors = mesh.colors;
var indices = mesh.triangles;
var bounds = new UnityEngine.Bounds(vertices[0], UnityEngine.Vector3.zero);
foreach(var v in vertices) bounds.Encapsulate(v);
var eps = 0.00001f;
var sizeError = (bounds.size - UnityEngine.Vector3.one * 2).magnitude;
var bottomError = (new UnityEngine.Vector3(bounds.center.x, bounds.min.y, bounds.center.z)).magnitude;
var normalCountOK = normals.Length == vertices.Length;
var outward = normalCountOK && Enumerable.Range(0, vertices.Length).All(j => UnityEngine.Vector3.Dot(normals[j], vertices[j] - bounds.center) > 0.99f);
var winding = true;
for(int j=0;j<indices.Length;j+=3) {
 var a=indices[j]; var b=indices[j+1]; var c=indices[j+2];
 winding &= UnityEngine.Vector3.Dot(UnityEngine.Vector3.Cross(vertices[b]-vertices[a], vertices[c]-vertices[a]).normalized, normals[a]) > 0.99f;
}
var red = Enumerable.Range(0, colors.Length).Where(j => colors[j].r > .9f && colors[j].g < .1f && colors[j].b < .1f).ToArray();
var green = Enumerable.Range(0, colors.Length).Where(j => colors[j].g > .9f && colors[j].r < .1f && colors[j].b < .1f).ToArray();
var blue = Enumerable.Range(0, colors.Length).Where(j => colors[j].b > .9f && colors[j].r < .1f && colors[j].g < .1f).ToArray();
var forwardOK = red.Length > 0 && red.All(j => UnityEngine.Vector3.Dot(normals[j], UnityEngine.Vector3.forward) > .999f && System.Math.Abs(vertices[j].z-1) < eps);
var upOK = green.Length > 0 && green.All(j => UnityEngine.Vector3.Dot(normals[j], UnityEngine.Vector3.up) > .999f && System.Math.Abs(vertices[j].y-2) < eps);
var rightOK = blue.Length > 0 && blue.All(j => UnityEngine.Vector3.Dot(normals[j], UnityEngine.Vector3.right) > .999f && System.Math.Abs(vertices[j].x-1) < eps);
var clean = transforms.All(t => t.localPosition.magnitude < eps && UnityEngine.Quaternion.Angle(t.localRotation,UnityEngine.Quaternion.identity) < eps && (t.localScale-UnityEngine.Vector3.one).magnitude < eps);
var hierarchyOK = transforms.Length==2 && asset.name=="CalibrationCube_2m" && filter.name=="CalibrationCube_2m_Mesh" && filter.transform.parent==asset.transform;
var refs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path).Select(a => { string guid; long id; UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(a, out guid, out id); return new { a.name, type=a.GetType().Name, guid, fileID=id.ToString() }; }).ToArray();
return new {
 unityVersion=UnityEngine.Application.unityVersion, assetPath=path, tolerance=eps,
 bounds=new { min=new[]{bounds.min.x,bounds.min.y,bounds.min.z}, max=new[]{bounds.max.x,bounds.max.y,bounds.max.z}, size=new[]{bounds.size.x,bounds.size.y,bounds.size.z} },
 sizeError, bottomError, exactSize=(bounds.size.x==2 && bounds.size.y==2 && bounds.size.z==2),
 cleanTransforms=clean, hierarchyOK, outwardNormals=outward, triangleWinding=winding,
 axisChecks=new { blenderPlusYToUnityPlusZ=forwardOK, blenderPlusZToUnityPlusY=upOK, blenderPlusXToUnityPlusX=rightOK, redVertices=red.Length, greenVertices=green.Length, blueVertices=blue.Length },
 vertexCount=mesh.vertexCount, triangleCount=indices.Length/3, references=refs,
 hierarchy=transforms.Select(t=>new{t.name, position=t.localPosition.ToString("R"), rotation=t.localRotation.ToString("R"), scale=t.localScale.ToString("R")}).ToArray(),
 importSettings=new {importer.globalScale, importer.useFileScale, importer.fileScale, importer.bakeAxisConversion, importer.preserveHierarchy, normals=importer.importNormals.ToString(), compression=importer.meshCompression.ToString(), importer.isReadable, importer.weldVertices, optimization=importer.meshOptimizationFlags.ToString(), tangents=importer.importTangents.ToString(), importer.importAnimation, animationType=importer.animationType.ToString(), importer.importCameras, importer.importLights, materials=importer.materialImportMode.ToString()},
 scenes=Enumerable.Range(0,UnityEngine.SceneManagement.SceneManager.sceneCount).Select(j=>{var s=UnityEngine.SceneManagement.SceneManager.GetSceneAt(j); return new{s.path,s.isDirty};}).ToArray(),
 passed=sizeError<eps && bottomError<eps && clean && hierarchyOK && outward && winding && forwardOK && upOK && rightOK
};
