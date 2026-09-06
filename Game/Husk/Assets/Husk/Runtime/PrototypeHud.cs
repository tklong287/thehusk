using UnityEngine;

namespace Husk
{
    [DisallowMultipleComponent, RequireComponent(typeof(PrototypeSession))]
    public sealed class PrototypeHud : MonoBehaviour
    {
        private PrototypeSession session;
        private ResourceState resources;
        private readonly string[] rows = new string[5];
        private GUIStyle titleStyle;
        private GUIStyle resourceStyle;
        private GUIStyle needStyle;

        public string ResourceSummary => string.Join(" | ", rows);

        private void OnEnable()
        {
            session = GetComponent<PrototypeSession>();
            resources = session.Resources;
            resources.Changed += RefreshRows;
            RefreshRows();
        }

        private void OnDisable()
        {
            if (resources != null) resources.Changed -= RefreshRows;
        }

        private void RefreshRows()
        {
            rows[0] = $"Food: {resources.Get(ResourceKind.Food)}";
            rows[1] = $"Water: {resources.Get(ResourceKind.Water)}";
            rows[2] = $"Recyclable Material: {resources.Get(ResourceKind.RecyclableMaterial)}";
            rows[3] = $"Wood: {resources.Get(ResourceKind.Wood)}";
            rows[4] = $"Iron: {resources.Get(ResourceKind.Iron)}";
        }

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
                resourceStyle = new GUIStyle(GUI.skin.label) { fontSize = 20 };
                needStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
                needStyle.normal.textColor = new Color(1f, 0.8f, 0.35f);
            }

            // Scale the small developer panel to fit the Game view.
            var previousMatrix = GUI.matrix;
            float scale = Mathf.Min(1f, Screen.width / 600f, Screen.height / 440f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            GUI.Box(new Rect(16, 16, 568, 408), GUIContent.none);
            GUI.Label(new Rect(36, 30, 520, 36), "HUSK | Settlement", titleStyle);
            GUI.Label(new Rect(36, 78, 520, 28), "WATER SUPPLY NEEDED", needStyle);
            GUI.Label(new Rect(36, 110, 520, 70), session.WaterNeedMessage, needStyle);
            for (int i = 0; i < rows.Length; i++)
                GUI.Label(new Rect(36, 196 + i * 36, 520, 32), rows[i], resourceStyle);
            GUI.matrix = previousMatrix;
        }
    }
}
