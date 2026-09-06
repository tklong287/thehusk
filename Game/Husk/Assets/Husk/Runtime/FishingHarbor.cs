using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class FishingHarbor : MonoBehaviour
    {
        [Header("Provisional construction timing")]
        [SerializeField, Min(0.1f)] private float boatBuildSeconds = 5f;
        [Header("Provisional complete trip and stage weights")]
        [SerializeField, Min(0.1f)] private float tripSeconds = 30f;
        [SerializeField, Min(0.1f)] private float departWeight = 6f;
        [SerializeField, Min(0.1f)] private float fishingWeight = 16f;
        [SerializeField, Min(0.1f)] private float returnWeight = 6f;
        [SerializeField, Min(0.1f)] private float harborWeight = 2f;
        [Header("Provisional route and boat feel")]
        [SerializeField] private Vector3 fishingOffset = new Vector3(5f, 0f, -3f);
        [SerializeField, Min(0.1f)] private float fishingSpotSpacing = 4f;
        [SerializeField, Min(1)] private int fishingSpotColumns = 2;
        [SerializeField, Min(0f)] private float turnDegreesPerSecond = 100f;
        [SerializeField, Min(0f)] private float fishingBobHeight = 0.12f;
        [SerializeField, Min(0f)] private float fishingRockDegrees = 4f;
        [SerializeField, Min(0.1f)] private float fishingBobPeriod = 2f;
        [Header("Provisional Fish delivery")]
        [SerializeField, Min(0)] private int fishPerTrip = 5;
        [SerializeField, Min(0.1f)] private float unloadFeedbackSeconds = 4f;
        [Header("Scene references")]
        [SerializeField] private PrototypeSession session;
        [SerializeField] private Camera view;
        [SerializeField] private GameObject selectionMarker;
        [SerializeField] private GameObject constructionVisual;
        [SerializeField] private GameObject fishingBoat;
        [SerializeField] private Transform labelAnchor;

        private WaterPlant waterPlant;
        private Recycler recycler;
        private BoatConstruction construction;
        public sealed class BoatInstance
        {
            public GameObject Visual { get; }
            public FishingTrip Trip { get; }
            public Vector3 Destination { get; }
            public int CargoPerTrip { get; }
            public long UnloadedTrips { get; internal set; }
            public int LastDeliveredFish { get; internal set; }
            public float UnloadFeedbackRemaining { get; internal set; }
            internal Quaternion Heading;
            internal BoatInstance(GameObject visual, FishingTrip trip, Vector3 destination, int cargoPerTrip)
            {
                CargoPerTrip = cargoPerTrip;
                Visual = visual;
                Trip = trip;
                Destination = destination;
                Heading = visual.transform.localRotation;
            }
        }

        private readonly List<BoatInstance> boats = new List<BoatInstance>();
        private Vector3 berthPosition;
        private Vector2 boatListScroll;
        private float unloadFeedbackRemaining;
        public string LastDelivery { get; private set; } = "";
        public long TotalUnloads { get; private set; }
        public IReadOnlyList<BoatInstance> Boats => boats;
        public int BoatCount => boats.Count;
        private static string Status(FishingTrip trip) => trip.Stage switch
        {
            FishingTripStage.Depart => "Departing",
            FishingTripStage.Fishing => "Fishing",
            FishingTripStage.Return => "Returning",
            _ => "At Harbor"
        };
        private GUIStyle titleStyle;
        private GUIStyle textStyle;
        private GUIStyle buttonStyle;
        public bool IsSelected { get; private set; }
        public BoatConstruction Construction => construction;
        public GameObject BoatTemplate => fishingBoat;
        private float UiScale => Mathf.Min(1f, Screen.width / 800f, Screen.height / 540f);
        public Rect PanelRect => Panel;
        private Rect Panel => new Rect(Screen.width / UiScale - 310, 16, 294, 360);

        public bool IsScreenPointOverPanel(Vector2 screenPoint)
        {
            Vector2 guiPoint = new Vector2(screenPoint.x, Screen.height - screenPoint.y) / UiScale;
            return isActiveAndEnabled && Panel.Contains(guiPoint);
        }
        private void Awake()
        {
            waterPlant = FindAnyObjectByType<WaterPlant>();
            recycler = FindAnyObjectByType<Recycler>();
            construction = new BoatConstruction(boatBuildSeconds);
            selectionMarker.SetActive(false);
            constructionVisual.SetActive(false);
            fishingBoat.SetActive(false);
            berthPosition = fishingBoat.transform.localPosition;

        }

        private void Update()
        {
            AdvanceSimulation(Time.deltaTime);
            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
            Vector2 screen = mouse.position.ReadValue();
            Vector2 gui = new Vector2(screen.x, Screen.height - screen.y) / UiScale;
            // Do not let resource/action panel clicks select the world behind them.
            if (PrototypeHud.ResourcePanelRect.Contains(gui) || Panel.Contains(gui)
                || (waterPlant != null && waterPlant.IsScreenPointOverPanel(screen))
                || (recycler != null && recycler.IsScreenPointOverPanel(screen))) return;
            bool hitHarbor = Physics.Raycast(view.ScreenPointToRay(screen), out RaycastHit hit)
                && hit.collider.GetComponentInParent<FishingHarbor>() == this;
            Select(hitHarbor);
        }

        // The same simulation step is used by Update and deterministic integration tests.
        public void AdvanceSimulation(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            unloadFeedbackRemaining = Mathf.Max(0f, unloadFeedbackRemaining - deltaTime);
            foreach (BoatInstance boat in boats)
            {
                AdvanceBoat(boat, deltaTime);
                UpdateBoatPose(boat, deltaTime);
            }
            bool wasBuilding = construction.IsBuilding;
            float remaining = construction.Duration - construction.Elapsed;
            construction.Tick(deltaTime);
            if (wasBuilding && construction.IsComplete)
            {
                int index = boats.Count;
                GameObject visual = Instantiate(fishingBoat, fishingBoat.transform.parent);
                visual.name = $"Fishing Boat {index + 1}";
                Vector3 destination = berthPosition + fishingOffset + new Vector3(
                    index % Mathf.Max(1, fishingSpotColumns) * fishingSpotSpacing, 0f,
                    -(index / Mathf.Max(1, fishingSpotColumns)) * fishingSpotSpacing);
                var boat = new BoatInstance(visual,
                    new FishingTrip(tripSeconds, departWeight, fishingWeight, returnWeight, harborWeight), destination, fishPerTrip);
                boats.Add(boat);
                visual.SetActive(true);
                float remainder = Mathf.Max(0f, deltaTime - remaining);
                AdvanceBoat(boat, remainder);
                UpdateBoatPose(boat, remainder);
            }
            if (constructionVisual.activeSelf != construction.IsBuilding)
                constructionVisual.SetActive(construction.IsBuilding);
        }

        private void AdvanceBoat(BoatInstance boat, float deltaTime)
        {
            boat.UnloadFeedbackRemaining = Mathf.Max(0f, boat.UnloadFeedbackRemaining - deltaTime);
            boat.Trip.Tick(deltaTime);
            long arrivals = boat.Trip.Arrivals - boat.UnloadedTrips;
            if (arrivals == 0) return;
            int amount = checked((int)(arrivals * boat.CargoPerTrip));
            session.Resources.Add(ResourceKind.Fish, amount);
            boat.UnloadedTrips = boat.Trip.Arrivals;
            boat.LastDeliveredFish = amount;
            boat.UnloadFeedbackRemaining = unloadFeedbackSeconds;
            TotalUnloads += arrivals;
            LastDelivery = $"{boat.Visual.name}: +{amount} Fish";
            unloadFeedbackRemaining = unloadFeedbackSeconds;
        }

        private void UpdateBoatPose(BoatInstance boat, float deltaTime)
        {
            FishingTrip trip = boat.Trip;
            Vector3 destination = boat.Destination;
            Vector3 position;
            Vector3 direction = destination - berthPosition;
            float rock = 0f;
            switch (trip.Stage)
            {
                case FishingTripStage.Depart:
                    position = Vector3.Lerp(berthPosition, destination, trip.StageProgress);
                    break;
                case FishingTripStage.Fishing:
                    position = destination;
                    float wave = Mathf.Sin((float)trip.CycleElapsed * Mathf.PI * 2f / fishingBobPeriod);
                    position.y += wave * fishingBobHeight;
                    rock = wave * fishingRockDegrees;
                    break;
                case FishingTripStage.Return:
                    position = Vector3.Lerp(destination, berthPosition, trip.StageProgress);
                    direction = berthPosition - destination;
                    break;
                default:
                    position = berthPosition;
                    direction = berthPosition - destination;
                    break;
            }
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
                boat.Heading = Quaternion.RotateTowards(boat.Heading, Quaternion.LookRotation(direction), turnDegreesPerSecond * deltaTime);
            boat.Visual.transform.localPosition = position;
            boat.Visual.transform.localRotation = boat.Heading * Quaternion.Euler(0f, 0f, rock);
        }
        public void Select(bool selected)
        {
            IsSelected = selected;
            selectionMarker.SetActive(selected);
        }

        public bool TryBuildBoat()
        {
            if (!IsSelected || construction.IsBuilding) return false;
            if (construction.IsComplete) construction = new BoatConstruction(boatBuildSeconds);
            return construction.TryStart();
        }

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 19, fontStyle = FontStyle.Bold };
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 15 };
                buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 16 };
                titleStyle.normal.textColor = textStyle.normal.textColor = Color.white;
            }
            Matrix4x4 previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * UiScale);
            Vector3 anchor = view.WorldToScreenPoint(labelAnchor.position);
            if (anchor.z > 0f)
            {
                if (unloadFeedbackRemaining > 0f)
                {
                    Rect deliveryLabel = new Rect(anchor.x / UiScale - 125, (Screen.height - anchor.y) / UiScale - 47, 250, 28);
                    if (!deliveryLabel.Overlaps(Panel) && !deliveryLabel.Overlaps(PrototypeHud.ResourcePanelRect)
                        && (waterPlant == null || !deliveryLabel.Overlaps(WaterPlant.PanelRect))
                        && (recycler == null || !deliveryLabel.Overlaps(recycler.PanelRect)))
                        GUI.Box(deliveryLabel, LastDelivery, buttonStyle);
                }
                Rect label = new Rect(anchor.x / UiScale - 82, (Screen.height - anchor.y) / UiScale - 15, 164, 30);
                if (!label.Overlaps(Panel) && !label.Overlaps(PrototypeHud.ResourcePanelRect)
                    && (waterPlant == null || !label.Overlaps(WaterPlant.PanelRect))
                    && (recycler == null || !label.Overlaps(recycler.PanelRect)))
                    if (GUI.Button(label, IsSelected ? "Harbor - Selected" : "Fishing Harbor", buttonStyle)) Select(true);
            }
            Rect panel = Panel;
            Color previousColor = GUI.color;
            GUI.color = new Color(0.035f, 0.08f, 0.11f, 0.94f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture);
            GUI.color = previousColor;
            float x = panel.x + 14;
            GUI.Label(new Rect(x, 26, 270, 28), "FISHING HARBOR", titleStyle);
            if (!IsSelected)
                GUI.Label(new Rect(x, 66, 270, 28), "Click the Harbor to select", textStyle);
            else if (construction.IsBuilding)
            {
                GUI.Label(new Rect(x, 65, 270, 28), "Building Fishing Boat", textStyle);
                GUI.Label(new Rect(x, 96, 270, 28), $"{construction.Progress:P0}   {Mathf.Max(0, construction.Duration - construction.Elapsed):0.0}s remaining", textStyle);
                GUI.color = new Color(0.15f, 0.25f, 0.29f);
                GUI.DrawTexture(new Rect(x, 137, 266, 14), Texture2D.whiteTexture);
                GUI.color = new Color(0.3f, 0.85f, 0.75f);
                GUI.DrawTexture(new Rect(x, 137, 266 * construction.Progress, 14), Texture2D.whiteTexture);
                GUI.color = previousColor;
            }
            else
            {
                GUI.Label(new Rect(x, 62, 270, 28), $"Construction: {construction.Duration:0.#} seconds", textStyle);
                if (GUI.Button(new Rect(x, 104, 266, 40), "Build Fishing Boat", buttonStyle)) TryBuildBoat();
            }
            GUI.Label(new Rect(x, 174, 266, 26), $"Boats: {boats.Count}", textStyle);
            GUI.Label(new Rect(x, 201, 266, 26), unloadFeedbackRemaining > 0f ? LastDelivery : "", textStyle);
            boatListScroll = GUI.BeginScrollView(new Rect(x, 234, 266, 123), boatListScroll,
                new Rect(0, 0, 246, Mathf.Max(123, boats.Count * 54)));
            for (int i = 0; i < boats.Count; i++)
            {
                var trip = boats[i].Trip;
                GUI.Label(new Rect(0, i * 54, 246, 25), $"Boat {i + 1} - {Status(trip)}", textStyle);
                GUI.Label(new Rect(0, i * 54 + 24, 246, 24), boats[i].UnloadFeedbackRemaining > 0f ? $"Unloaded +{boats[i].LastDeliveredFish} Fish" :
                    $"Cycle {trip.CompletedCycles + 1}   {trip.CycleElapsed:0.0} / {trip.Duration:0.#}s", textStyle);
            }
            GUI.EndScrollView();
            GUI.matrix = previousMatrix;
        }
    }
}
