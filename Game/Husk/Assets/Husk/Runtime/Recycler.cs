using UnityEngine;
using UnityEngine.InputSystem;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class Recycler : MonoBehaviour
    {
        [Header("Provisional Recycler values")]
        [SerializeField, Min(0.1f)] private float buildSeconds = 5f;
        [SerializeField, Min(0.1f)] private float processingInterval = 4f;
        [SerializeField, Min(1)] private int inputPerCycle = 2;
        [SerializeField, Min(1)] private int woodPerCycle = 1;
        [SerializeField, Min(0.1f)] private float feedbackSeconds = 4f;
        [Header("Scene references")]
        [SerializeField] private PrototypeSession session;
        [SerializeField] private Camera view;
        [SerializeField] private GameObject selectionMarker;
        [SerializeField] private GameObject constructionVisual;
        [SerializeField] private GameObject operationalVisual;
        [SerializeField] private Transform labelAnchor;
        private FishingHarbor harbor;
        private WaterPlant waterPlant;
        private GUIStyle textStyle, titleStyle, buttonStyle;
        private float feedbackRemaining;
        public RecyclerProcessing Processing { get; private set; }
        public bool IsSelected { get; private set; }
        public string LastConversion { get; private set; } = "";
        private float UiScale => Mathf.Min(1f, Screen.width / 800f, Screen.height / 540f);
        public Rect PanelRect => new Rect(Screen.width / UiScale - 310, 392, 294, 140);

        private void Awake()
        {
            Processing = new RecyclerProcessing(buildSeconds, processingInterval, inputPerCycle, woodPerCycle);
            harbor = FindAnyObjectByType<FishingHarbor>();
            waterPlant = FindAnyObjectByType<WaterPlant>();
            selectionMarker.SetActive(false);
            constructionVisual.SetActive(false);
            operationalVisual.SetActive(false);
        }
        public bool IsScreenPointOverPanel(Vector2 point) => isActiveAndEnabled && PanelRect.Contains(new Vector2(point.x, Screen.height - point.y) / UiScale);
        public void Select(bool selected) { IsSelected = selected; selectionMarker.SetActive(selected); }
        public bool TryBuild() => IsSelected && Processing.TryStart();
        private void Update()
        {
            AdvanceSimulation(Time.deltaTime);
            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
            Vector2 point = mouse.position.ReadValue();
            Vector2 guiPoint = new Vector2(point.x, Screen.height - point.y) / UiScale;
            if (PanelRect.Contains(guiPoint) || PrototypeHud.ResourcePanelRect.Contains(guiPoint)
                || (harbor != null && harbor.IsScreenPointOverPanel(point))
                || (waterPlant != null && waterPlant.IsScreenPointOverPanel(point))) return;
            Select(Physics.Raycast(view.ScreenPointToRay(point), out RaycastHit hit) && hit.collider.GetComponentInParent<Recycler>() == this);
        }
        public void AdvanceSimulation(float deltaTime)
        {
            int cycles = Processing.Tick(deltaTime, session.Resources);
            feedbackRemaining = Mathf.Max(0, feedbackRemaining - deltaTime);
            if (cycles > 0)
            {
                LastConversion = $"-{cycles * Processing.InputPerCycle} Recyclable  +{cycles * Processing.WoodPerCycle} Wood";
                feedbackRemaining = feedbackSeconds;
            }
            if (constructionVisual.activeSelf != Processing.IsBuilding) constructionVisual.SetActive(Processing.IsBuilding);
            if (operationalVisual.activeSelf != Processing.IsOperational) operationalVisual.SetActive(Processing.IsOperational);
        }
        private void OnGUI()
        {
            if (textStyle == null)
            {
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 15 };
                titleStyle = new GUIStyle(textStyle) { fontSize = 19, fontStyle = FontStyle.Bold };
                buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 16 };
                textStyle.normal.textColor = titleStyle.normal.textColor = Color.white;
            }
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * UiScale);
            var oldColor = GUI.color;
            GUI.color = new Color(0.035f, 0.08f, 0.11f, 0.94f);
            GUI.DrawTexture(PanelRect, Texture2D.whiteTexture);
            GUI.color = oldColor;
            float x = PanelRect.x + 14;
            GUI.Label(new Rect(x, 396, 266, 26), "RECYCLER", titleStyle);
            if (Processing.IsOperational)
            {
                GUI.Label(new Rect(x, 422, 266, 24), Processing.IsWaiting ? "Waiting for Recyclable Material" : $"Processing {Processing.ProcessingProgress:P0}", textStyle);
                GUI.Label(new Rect(x, 447, 266, 24), $"{Processing.InputPerCycle} Recyclable -> {Processing.WoodPerCycle} Wood / {Processing.IntervalSeconds:0.#}s", textStyle);
                GUI.Label(new Rect(x, 494, 266, 26), feedbackRemaining > 0 ? LastConversion : "", textStyle);
            }
            else if (Processing.IsBuilding)
                GUI.Label(new Rect(x, 427, 266, 26), $"Building {Processing.ConstructionProgress:P0} - {Processing.BuildSeconds * (1 - Processing.ConstructionProgress):0.0}s", textStyle);
            else if (IsSelected)
            {
                GUI.Label(new Rect(x, 422, 266, 24), $"Construction: {Processing.BuildSeconds:0.#} seconds", textStyle);
                if (GUI.Button(new Rect(x, 454, 266, 34), "Build Recycler", buttonStyle)) TryBuild();
            }
            else GUI.Label(new Rect(x, 427, 266, 26), "Click the Recycler site", textStyle);
            if (Processing.HasStarted)
            {
                GUI.color = new Color(0.15f, 0.25f, 0.29f);
                GUI.DrawTexture(new Rect(x, 477, 266, 10), Texture2D.whiteTexture);
                GUI.color = new Color(0.85f, 0.7f, 0.3f);
                float progress = (float)(Processing.IsOperational ? Processing.ProcessingProgress : Processing.ConstructionProgress);
                GUI.DrawTexture(new Rect(x, 477, 266 * progress, 10), Texture2D.whiteTexture);
                GUI.color = oldColor;
            }
            Vector3 anchor = view.WorldToScreenPoint(labelAnchor.position);
            Rect label = new Rect(anchor.x / UiScale - 80, (Screen.height - anchor.y) / UiScale - 15, 160, 30);
            if (anchor.z > 0 && !label.Overlaps(PanelRect) && !label.Overlaps(PrototypeHud.ResourcePanelRect)
                && (harbor == null || !label.Overlaps(harbor.PanelRect)) && (waterPlant == null || !label.Overlaps(WaterPlant.PanelRect)))
                if (GUI.Button(label, IsSelected ? "Recycler - Selected" : "Recycler", buttonStyle)) Select(true);
            GUI.matrix = oldMatrix;
        }
    }
}
