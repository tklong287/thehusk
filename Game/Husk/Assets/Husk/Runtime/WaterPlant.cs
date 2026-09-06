using UnityEngine;
using UnityEngine.InputSystem;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class WaterPlant : MonoBehaviour
    {
        [Header("Provisional Water production")]
        [SerializeField, Min(0.1f)] private float buildSeconds = 5f;
        [SerializeField, Min(0.1f)] private float productionInterval = 2f;
        [SerializeField, Min(0)] private int waterPerCycle = 1;
        [SerializeField, Min(0.1f)] private float feedbackSeconds = 4f;
        [Header("Scene references")]
        [SerializeField] private PrototypeSession session;
        [SerializeField] private Camera view;
        [SerializeField] private GameObject selectionMarker;
        [SerializeField] private GameObject constructionVisual;
        [SerializeField] private GameObject operationalVisual;
        [SerializeField] private Transform labelAnchor;
        private FishingHarbor harbor;
        private Recycler recycler;
        private GUIStyle textStyle, titleStyle, buttonStyle;
        private float feedbackRemaining;
        public WaterProduction Production { get; private set; }
        public bool IsSelected { get; private set; }
        public int LastProducedWater { get; private set; }
        public static Rect PanelRect => new Rect(16, 266, 268, 220);
        private float UiScale => Mathf.Min(1f, Screen.width / 800f, Screen.height / 540f);

        private void Awake()
        {
            Production = new WaterProduction(buildSeconds, productionInterval, waterPerCycle);
            harbor = FindAnyObjectByType<FishingHarbor>();
            recycler = FindAnyObjectByType<Recycler>();
            selectionMarker.SetActive(false);
            constructionVisual.SetActive(false);
            operationalVisual.SetActive(false);
        }

        public bool IsScreenPointOverPanel(Vector2 point) => isActiveAndEnabled &&
            PanelRect.Contains(new Vector2(point.x, Screen.height - point.y) / UiScale);

        public void Select(bool selected)
        {
            IsSelected = selected;
            selectionMarker.SetActive(selected);
        }

        public bool TryBuild() => IsSelected && Production.TryStart();

        private void Update()
        {
            AdvanceSimulation(Time.deltaTime);
            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
            Vector2 point = mouse.position.ReadValue();
            Vector2 guiPoint = new Vector2(point.x, Screen.height - point.y) / UiScale;
            if (PanelRect.Contains(guiPoint) || PrototypeHud.ResourcePanelRect.Contains(guiPoint)
                || (harbor != null && harbor.IsScreenPointOverPanel(point))
                || (recycler != null && recycler.IsScreenPointOverPanel(point))) return;
            Select(Physics.Raycast(view.ScreenPointToRay(point), out RaycastHit hit)
                && hit.collider.GetComponentInParent<WaterPlant>() == this);
        }

        public void AdvanceSimulation(float deltaTime)
        {
            int produced = Production.Tick(deltaTime);
            feedbackRemaining = Mathf.Max(0f, feedbackRemaining - deltaTime);
            if (produced > 0)
            {
                session.Resources.Add(ResourceKind.Water, produced);
                LastProducedWater = produced;
                feedbackRemaining = feedbackSeconds;
            }
            if (constructionVisual.activeSelf != Production.IsBuilding) constructionVisual.SetActive(Production.IsBuilding);
            if (operationalVisual.activeSelf != Production.IsOperational) operationalVisual.SetActive(Production.IsOperational);
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
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * UiScale);
            var previousColor = GUI.color;
            GUI.color = new Color(0.035f, 0.08f, 0.11f, 0.94f);
            GUI.DrawTexture(PanelRect, Texture2D.whiteTexture);
            GUI.color = previousColor;
            GUI.Label(new Rect(30, 276, 240, 28), "WATER PLANT", titleStyle);
            if (Production.IsOperational)
            {
                GUI.Label(new Rect(30, 310, 240, 26), "Operational", textStyle);
                GUI.Label(new Rect(30, 340, 240, 26), $"+{Production.WaterPerCycle} Water / {Production.IntervalSeconds:0.#}s", textStyle);
                GUI.Label(new Rect(30, 370, 240, 26), $"Production: {Production.ProductionProgress:P0}", textStyle);
                GUI.Label(new Rect(30, 435, 240, 26), feedbackRemaining > 0 ? $"Produced +{LastProducedWater} Water" : "", textStyle);
            }
            else if (Production.IsBuilding)
            {
                GUI.Label(new Rect(30, 310, 240, 26), "Under construction", textStyle);
                GUI.Label(new Rect(30, 345, 240, 26), $"{Production.ConstructionProgress:P0} - {Production.BuildSeconds * (1 - Production.ConstructionProgress):0.0}s remaining", textStyle);
            }
            else if (IsSelected)
            {
                GUI.Label(new Rect(30, 310, 240, 26), $"Construction: {Production.BuildSeconds:0.#} seconds", textStyle);
                if (GUI.Button(new Rect(30, 350, 240, 40), "Build Water Plant", buttonStyle)) TryBuild();
            }
            else GUI.Label(new Rect(30, 310, 240, 26), "Click the Water Plant site", textStyle);
            if (Production.HasStarted)
            {
                GUI.color = new Color(0.15f, 0.25f, 0.29f);
                GUI.DrawTexture(new Rect(30, 408, 240, 12), Texture2D.whiteTexture);
                GUI.color = new Color(0.3f, 0.85f, 0.95f);
                float progress = (float)(Production.IsOperational ? Production.ProductionProgress : Production.ConstructionProgress);
                GUI.DrawTexture(new Rect(30, 408, 240 * progress, 12), Texture2D.whiteTexture);
                GUI.color = previousColor;
            }
            Vector3 anchor = view.WorldToScreenPoint(labelAnchor.position);
            Rect label = new Rect(anchor.x / UiScale - 85, (Screen.height - anchor.y) / UiScale - 15, 170, 30);
            if (anchor.z > 0 && !label.Overlaps(PanelRect) && !label.Overlaps(PrototypeHud.ResourcePanelRect)
                && (harbor == null || !label.Overlaps(harbor.PanelRect))
                && (recycler == null || !label.Overlaps(recycler.PanelRect)))
                if (GUI.Button(label, IsSelected ? "Water Plant - Selected" : "Water Plant", buttonStyle)) Select(true);
            GUI.matrix = previousMatrix;
        }
    }
}
