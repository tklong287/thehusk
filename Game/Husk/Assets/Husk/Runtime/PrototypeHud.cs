using UnityEngine;

namespace Husk
{
    [DisallowMultipleComponent, RequireComponent(typeof(PrototypeSession))]
    public sealed class PrototypeHud : MonoBehaviour
    {
        private PrototypeSession session;
        private ResourceState resources;
        private readonly string[] rows = new string[6];
        private GUIStyle titleStyle;
        private GUIStyle resourceStyle;
        private GUIStyle controlsStyle;

        public static Rect ResourcePanelRect => new Rect(16, 16, 268, 234);
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
            rows[5] = $"Fish: {resources.Get(ResourceKind.Fish)}";
        }

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 21, fontStyle = FontStyle.Bold };
                resourceStyle = new GUIStyle(GUI.skin.label) { fontSize = 16 };
                titleStyle.normal.textColor = new Color(0.75f, 0.93f, 0.94f);
                resourceStyle.normal.textColor = Color.white;
                controlsStyle = new GUIStyle(resourceStyle);
            }

            // Scale the small developer panel to fit the Game view.
            var previousMatrix = GUI.matrix;
            float scale = Mathf.Min(1f, Screen.width / 800f, Screen.height / 540f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            Color previousColor = GUI.color;
            GUI.color = new Color(0.035f, 0.08f, 0.11f, 0.92f);
            GUI.DrawTexture(ResourcePanelRect, Texture2D.whiteTexture);
            GUI.color = previousColor;
            GUI.Label(new Rect(30, 24, 240, 32), "HUSK", titleStyle);
            for (int i = 0; i < rows.Length; i++)
                GUI.Label(new Rect(30, 64 + i * 28, 240, 26), rows[i], resourceStyle);
            float controlsWidth = Mathf.Min(620f, Screen.width / scale - 346f);
            controlsStyle.fontSize = Mathf.Min(16, Mathf.FloorToInt(16f * controlsWidth / 620f));
            GUI.Label(new Rect(20, Screen.height / scale - 32, controlsWidth, 28),
                "WASD: move    RMB drag: orbit    Scroll: zoom    R: reset view", controlsStyle);
            GUI.matrix = previousMatrix;
        }
    }
}
