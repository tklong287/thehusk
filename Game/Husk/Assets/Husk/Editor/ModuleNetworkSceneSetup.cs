using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Husk.Editor
{
    public static class ModuleNetworkSceneSetup
    {
        public const string ScenePath = "Assets/Scenes/V1Phase1.unity";
        [MenuItem("Husk/Create Phase 1 Network Scene")]
        public static void CreateScene() => Create(false);
        [MenuItem("Husk/Create Phase 2 Production Scene")]
        public static void CreatePhase2Scene() => Create(true);
        private static void Create(bool production)
        {
            if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Exit Play before scene setup.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new System.InvalidOperationException("Current scene is dirty; preserve changes before scene setup.");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(PrototypeCamera));
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.2f, 0.29f);
            camera.rect = new Rect(0.34f, 0, 0.66f, 1);
            camera.farClipPlane = 500;
            var serializedCamera = new SerializedObject(cameraObject.GetComponent<PrototypeCamera>());
            serializedCamera.FindProperty("focus").vector3Value = production ? new Vector3(0, 0.5f, 0) : new Vector3(2, 0.5f, -1);
            serializedCamera.FindProperty("viewSize").floatValue = production ? 23 : 13;
            serializedCamera.FindProperty("maxViewSize").floatValue = 60;
            serializedCamera.FindProperty("zoomSensitivity").floatValue = 0.025f;
            serializedCamera.ApplyModifiedPropertiesWithoutUndo();
            var lightObject = new GameObject("Directional Light", typeof(Light));
            lightObject.transform.rotation = Quaternion.Euler(50, -30, 0);
            var light = lightObject.GetComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.4f;
            RenderSettings.ambientLight = new Color(0.55f, 0.6f, 0.65f);
            var prototype = new GameObject(production ? "V1 Phase 2 Networked City" : "V1 Phase 1 Module Network", typeof(ModuleNetworkPrototype));
            var serialized = new SerializedObject(prototype.GetComponent<ModuleNetworkPrototype>());
            serialized.FindProperty("view").objectReferenceValue = camera;
            serialized.FindProperty("networkedProduction").boolValue = production;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.SaveScene(scene, production ? "Assets/Scenes/V1Phase2.unity" : ScenePath);
        }
    }
}
