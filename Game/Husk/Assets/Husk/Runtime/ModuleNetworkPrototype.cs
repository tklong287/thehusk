using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class ModuleNetworkPrototype : MonoBehaviour
    {
        [Header("Phase 1 provisional test preset")]
        [SerializeField, Min(0)] private int startingWood = 100;
        [SerializeField, Min(1)] private float moduleSize = 6f;
        [SerializeField] private Camera view;
        [Header("Phase 2 integration (off preserves Phase 1 fixture scene)")]
        [SerializeField] private bool networkedProduction;
        [SerializeField] private NetworkProductionSettings productionSettings = new();
        [Header("Provisional F / W / M / E palette")]
        [SerializeField] private Color foodColor = new(0.35f, 1f, 0.32f);
        [SerializeField] private Color waterColor = new(0.1f, 0.75f, 1f);
        [SerializeField] private Color materialColor = new(1f, 0.45f, 0.12f);
        [SerializeField] private Color electricColor = new(0.95f, 0.35f, 1f);
        [SerializeField, Range(0.05f, 0.8f)] private float unsuppliedBrightness = 0.25f;

        private readonly Dictionary<Vector2Int, Transform> visuals = new();
        private readonly List<Material> ownedMaterials = new();
        private readonly Material[,] laneMaterials = new Material[4, 2];
        private Material deckMaterial, hullMaterial, selectionMaterial, validMaterial, invalidMaterial;
        private Transform cursor;
        private Vector2Int selected;
        private Pipeline pending = Pipeline.F | Pipeline.W | Pipeline.M;
        private string feedback = "Select a Module or adjacent sea. Configure 3/4, then Build.";
        private Vector2 scroll;
        private GUIStyle labelStyle;
        private int visualRevision = -1;
        private Vector2Int viewportSize;
        public ModuleLayout Layout { get; private set; }
        public ModuleNetwork Network { get; private set; }
        public NetworkedCity City { get; private set; }
        public Vector2Int Selected => selected;
        public Pipeline PendingConfiguration => pending;
        public float ModuleSize => moduleSize;
        public float UiScale => Mathf.Min(1f, Screen.width / 800f, Screen.height / 540f);
        public Rect PanelRect => new(10, 10, 262, Screen.height / UiScale - 20);
        public bool IsScreenPointOverPanel(Vector2 point) => isActiveAndEnabled && PanelRect.Contains(new Vector2(point.x, Screen.height - point.y) / UiScale);

        private void Awake()
        {
            if (view == null) view = Camera.main;
            Layout = networkedProduction ? NetworkedCity.CreateLayout(startingWood, productionSettings.startingRecyclableMaterial) : CreateFresh(startingWood);
            if (networkedProduction) City = new NetworkedCity(Layout, productionSettings);
            Network = City != null ? City.Network : new ModuleNetwork(Layout);
            deckMaterial = MakeMaterial(new Color(0.36f, 0.43f, 0.45f), false);
            hullMaterial = MakeMaterial(new Color(0.12f, 0.2f, 0.24f), false);
            selectionMaterial = MakeMaterial(new Color(1f, 0.93f, 0.45f));
            validMaterial = MakeMaterial(new Color(0.2f, 0.75f, 0.55f));
            invalidMaterial = MakeMaterial(new Color(0.9f, 0.2f, 0.2f));
            Color[] palette = { foodColor, waterColor, materialColor, electricColor };
            for (int i = 0; i < 4; i++)
            {
                laneMaterials[i, 0] = MakeMaterial(palette[i] * unsuppliedBrightness);
                laneMaterials[i, 1] = MakeMaterial(palette[i]);
            }
            Cube("Sea", transform, new Vector3(0, -0.6f, 0), new Vector3(600, 0.2f, 600), MakeMaterial(new Color(0.055f, 0.2f, 0.29f)));
            cursor = new GameObject("Selection / build target").transform;
            cursor.SetParent(transform, false);
            for (int i = 0; i < 4; i++)
            {
                bool horizontal = i < 2;
                Cube("Target edge", cursor, horizontal ? new Vector3(0, 0.84f, (i == 0 ? -1 : 1) * moduleSize * 0.48f)
                    : new Vector3((i == 2 ? -1 : 1) * moduleSize * 0.48f, 0.84f, 0),
                    horizontal ? new Vector3(moduleSize, 0.06f, 0.08f) : new Vector3(0.08f, 0.06f, moduleSize), selectionMaterial);
            }
            var port = Cube("PORT clearance - construction blocked", transform, Position(new Vector2Int(0, -2)) + Vector3.up * 0.05f,
                new Vector3(moduleSize * 0.88f, 0.12f, moduleSize * 0.88f), invalidMaterial);
            port.name = "Reserved port sea cell (0,-2)";
            if (City != null) CreateHarbor();
            Select(Vector2Int.zero);
            RefreshVisuals();
        }
        public static ModuleLayout CreateFresh(int wood = 100)
        {
            var layout = new ModuleLayout(new ResourceState(startingWood: wood, startingModuleCore: 100));
            layout.AddInitial(new Vector2Int(-1, 0), Pipeline.F | Pipeline.W | Pipeline.M, ModuleBuilding.TownHall);
            layout.AddInitial(Vector2Int.zero, Pipeline.F | Pipeline.W | Pipeline.M);
            layout.AddInitial(new Vector2Int(0, -1), Pipeline.F | Pipeline.M | Pipeline.E, ModuleBuilding.FishingHarbor);
            layout.ReservePort(new Vector2Int(0, -1), new Vector2Int(0, -2));
            layout.SetTestSupply(Vector2Int.zero, Pipeline.F | Pipeline.W);
            return layout;
        }
        public void Select(Vector2Int position)
        {
            selected = position;
            if (Layout.Cells.TryGetValue(position, out var cell)) pending = cell.Pipelines;
            feedback = Layout.Cells.ContainsKey(position) ? "Inspect or reconfigure this Module." : Layout.BuildFailure(position, pending);
            if (feedback.Length == 0) feedback = "Valid sea target. Build costs 1 Core + 10 Wood.";
            RefreshCursor();
        }
        public void SetPendingConfiguration(Pipeline pipelines)
        {
            pending = pipelines | (Layout.Cells.TryGetValue(selected, out var cell) ? cell.LockedPipelines : Pipeline.None);
            RefreshCursor();
        }
        public bool PlaceSolarSelected()
        {
            if (City == null) return false;
            bool placed = Layout.TryOccupy(selected, ModuleBuilding.Solar, out string reason);
            City.Resolve();
            if (placed) pending = Layout.Cells[selected].Pipelines;
            feedback = placed ? "Solar placed; required E added and locked automatically." : reason;
            RefreshVisuals(); RefreshCursor(); return placed;
        }
        public bool BuildSelected()
        {
            bool built = Layout.TryBuild(selected, pending, out string reason);
            City?.Resolve();
            feedback = built ? "Module built: -1 Core / -10 Wood." : reason;
            RefreshVisuals(); RefreshCursor(); return built;
        }
        public bool ConfigureSelected()
        {
            bool changed = Layout.TryConfigure(selected, pending, out string reason);
            City?.Resolve();
            feedback = changed ? "Configuration applied (provisional free / instant)." : reason;
            RefreshVisuals(); RefreshCursor(); return changed;
        }
        public void SetSelectedTestSupply(Pipeline supply)
        { if (City == null) Layout.SetTestSupply(selected, supply); RefreshVisuals(); }
        public bool RepairSelected()
        {
            bool repaired = City != null && City.TryRepair(selected);
            feedback = repaired ? "Repair started (provisional cost / timer)." : "Cannot repair: check damage / Wood / current job.";
            RefreshVisuals(); return repaired;
        }
        public bool BuildFishingBoat()
        {
            if (City?.Harbor == null || selected != NetworkedCity.HarborPosition) return false;
            City.Resolve(); City.Harbor.Select(true);
            bool built = City.Harbor.TryBuildBoat();
            feedback = built ? "Fishing Boat construction started." : "Requires supplied M and no active construction job.";
            return built;
        }
        public void AdvanceSimulation(float seconds) { City?.Tick(seconds); RefreshVisuals(); }
        private void Update()
        {
            City?.Tick(Time.deltaTime);
            UpdateViewport();
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame && !IsScreenPointOverPanel(mouse.position.ReadValue()))
            {
                Ray ray = view.ScreenPointToRay(mouse.position.ReadValue());
                if (new Plane(Vector3.up, new Vector3(0, 0.7f, 0)).Raycast(ray, out float distance))
                {
                    var hit = ray.GetPoint(distance);
                    Select(new Vector2Int(Mathf.RoundToInt(hit.x / moduleSize), Mathf.RoundToInt(hit.z / moduleSize)));
                }
            }
            RefreshVisuals();
        }
        private void UpdateViewport()
        {
            var size = new Vector2Int(Screen.width, Screen.height);
            if (size == viewportSize) return;
            viewportSize = size;
            float left = 282f * UiScale / Screen.width;
            view.rect = new Rect(left, 0, 1f - left, 1);
        }
        public Vector3 Position(Vector2Int position) => new(position.x * moduleSize, 0, position.y * moduleSize);
        private void RefreshCursor()
        {
            if (cursor == null) return;
            cursor.position = Position(selected);
            var material = Layout.Cells.ContainsKey(selected) ? selectionMaterial : Layout.BuildFailure(selected, pending).Length == 0 ? validMaterial : invalidMaterial;
            foreach (var renderer in cursor.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = material;
        }
        private void RefreshVisuals()
        {
            if (visualRevision == Network.Revision) return;
            foreach (var pair in Layout.Cells)
            {
                if (!visuals.TryGetValue(pair.Key, out var root))
                {
                    root = new GameObject("Module " + pair.Key).transform;
                    root.SetParent(transform, false); root.position = Position(pair.Key);
                    visuals.Add(pair.Key, root);
                    Cube("Hull", root, new Vector3(0, 0, 0), new Vector3(moduleSize * 0.99f, 1.2f, moduleSize * 0.99f), hullMaterial);
                    Cube("Deck", root, new Vector3(0, 0.63f, 0), new Vector3(moduleSize * 0.97f, 0.14f, moduleSize * 0.97f), deckMaterial);
                    for (int i = 0; i < 4; i++)
                    {
                        float offset = (i - 1.5f) * moduleSize * 0.065f;
                        var lane = new GameObject(ModuleNetwork.Types[i] + " floor cross").transform;
                        lane.SetParent(root, false);
                        Cube("East-West", lane, new Vector3(0, 0.74f + i * 0.016f, offset), new Vector3(moduleSize, 0.022f, moduleSize * 0.034f), laneMaterials[i, 0]);
                        Cube("North-South", lane, new Vector3(offset, 0.74f + i * 0.016f, 0), new Vector3(moduleSize * 0.034f, 0.022f, moduleSize), laneMaterials[i, 0]);
                    }
                }
                if (pair.Value.Building != ModuleBuilding.None && root.Find(pair.Value.Building.ToString()) == null)
                    Cube(pair.Value.Building.ToString(), root, new Vector3(-1.7f, 1.35f, 1.7f), new Vector3(1.7f, 1.3f, 1.7f), selectionMaterial);
                for (int i = 0; i < 4; i++)
                {
                    var lane = root.Find(ModuleNetwork.Types[i] + " floor cross");
                    var state = Network.State(pair.Key, ModuleNetwork.Types[i]);
                    lane.gameObject.SetActive(state != PipelineState.Unsupported);
                    foreach (var renderer in lane.GetComponentsInChildren<Renderer>())
                        renderer.sharedMaterial = laneMaterials[i, state == PipelineState.Supplied ? 1 : 0];
                }
            }
            visualRevision = Network.Revision;
        }
        private Material MakeMaterial(Color color, bool unlit = true)
        {
            var material = new Material(Shader.Find(unlit ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit"));
            color.a = 1; material.SetColor("_BaseColor", color);
            ownedMaterials.Add(material); return material;
        }
        private void CreateHarbor()
        {
            var root = new GameObject("Networked Fishing Harbor");
            root.SetActive(false); root.transform.SetParent(transform, false);
            root.transform.localPosition = Position(NetworkedCity.HarborPosition);
            var harbor = root.AddComponent<FishingHarbor>();
            var boat = new GameObject("Boat template"); boat.transform.SetParent(root.transform, false);
            boat.transform.localPosition = new Vector3(0, -0.25f, -moduleSize * 0.7f);
            Cube("Hull", boat.transform, Vector3.zero, new Vector3(0.9f, 0.4f, 1.7f), hullMaterial);
            Cube("Cabin", boat.transform, Vector3.up * 0.4f, new Vector3(0.6f, 0.6f, 0.7f), selectionMaterial);
            var marker = new GameObject("Harbor selection"); marker.transform.SetParent(root.transform, false);
            var construction = Cube("Boat construction", root.transform, boat.transform.localPosition,
                new Vector3(0.9f, 0.5f, 1.7f), validMaterial);
            harbor.BindNetwork(Layout.Storage, boat, marker, construction);
            root.SetActive(true);
            City.BindHarbor(harbor);
        }
        private static GameObject Cube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name; cube.transform.SetParent(parent, false);
            cube.transform.localPosition = position; cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            Destroy(cube.GetComponent<Collider>()); return cube;
        }
        private void OnDestroy() { foreach (var material in ownedMaterials) if (material != null) Destroy(material); }
        private void OnGUI()
        {
            if (Layout == null) return;
            labelStyle ??= new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 13 };
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * UiScale);
            GUI.Box(PanelRect, "");
            GUILayout.BeginArea(new Rect(20, 18, 242, PanelRect.height - 16));
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label(City == null ? "HUSK V1 / PHASE 1" : "HUSK V1 / PHASE 2", GUI.skin.box);
            GUILayout.Label(City == null ? "Module + Network foundation" : "Networked production / starting city", labelStyle);
            GUILayout.Label($"Core {Layout.Storage.Get(ResourceKind.ModuleCore)}   Wood {Layout.Storage.Get(ResourceKind.Wood)}", labelStyle);
            GUILayout.Label("Click sea to select target; click floor / building to inspect.", labelStyle);
            GUILayout.Label($"Selected cell: {selected.x}, {selected.y}", labelStyle);
            bool exists = Layout.Cells.TryGetValue(selected, out var cell);
            GUILayout.Label(exists ? "Occupancy: " + cell.Building : "Sea / non-built", labelStyle);
            GUILayout.Label("Choose exactly 3 pipelines:", labelStyle);
            GUILayout.BeginHorizontal();
            foreach (var type in ModuleNetwork.Types)
            {
                bool supported = (pending & type) != 0;
                GUI.enabled = !exists || (cell.LockedPipelines & type) == 0;
                bool next = GUILayout.Toggle(supported, type.ToString(), "Button", GUILayout.Height(27));
                if (next != supported) SetPendingConfiguration(next ? pending | type : pending & ~type);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (exists && cell.LockedPipelines != Pipeline.None)
                GUILayout.Label("Locked INPUT + OUTPUT: " + cell.LockedPipelines, labelStyle);
            string failure = exists ? (BuildingPipelines.Allows(cell.Building, pending) ? "" : "Keep 3/4 and all building INPUT + OUTPUT.") : Layout.BuildFailure(selected, pending);
            GUILayout.Label(failure.Length == 0 ? (exists ? "Configuration valid" : "Valid build: 1 Core + 10 Wood") : failure, labelStyle);
            GUI.enabled = failure.Length == 0;
            if (GUILayout.Button(exists ? "Apply configuration" : "Build Module", GUILayout.Height(28)))
            { if (exists) ConfigureSelected(); else BuildSelected(); }
            GUI.enabled = true;
            GUILayout.Label(feedback, labelStyle);
            if (exists)
            {
                if (City != null && cell.Building == ModuleBuilding.None)
                {
                    if (GUILayout.Button("Place Solar (test)", GUILayout.Height(28))) PlaceSolarSelected();
                    GUILayout.Label("Provisional free / instant placement; required pipelines configure automatically.", labelStyle);
                }
                if (City != null && cell.Building != ModuleBuilding.None) DrawBuilding(cell);
                if (cell.Building == ModuleBuilding.TownHall)
                {
                    GUILayout.Label("TOWN HALL / unlimited Item Storage", labelStyle);
                    foreach (ResourceKind item in System.Enum.GetValues(typeof(ResourceKind)))
                        GUILayout.Label($"{(item == ResourceKind.ModuleCore ? "Module Core" : item == ResourceKind.RecyclableMaterial ? "Recyclable Material" : item.ToString())}: {Layout.Storage.Get(item)}", labelStyle);
                    GUILayout.Label("Town Hall I/O: TBD; no network source inferred.", labelStyle);
                }
                foreach (var type in ModuleNetwork.Types)
                    GUILayout.Label($"{type}: {Network.State(selected, type)} | component {Network.Component(selected, type)}", labelStyle);
                if (City == null)
                {
                GUILayout.Label("DEV SOURCE FIXTURE (no production)", labelStyle);
                GUILayout.BeginHorizontal();
                foreach (var type in ModuleNetwork.Types)
                {
                    bool active = (cell.TestSupply & type) != 0;
                    bool next = GUILayout.Toggle(active, type.ToString(), "Button", GUILayout.Height(24));
                    if (next != active) SetSelectedTestSupply(next ? cell.TestSupply | type : cell.TestSupply & ~type);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Fixture supplies only supported lanes. No concrete stock output.", labelStyle);
                }
            }
            GUILayout.Label("F green / W cyan / M orange / E violet\nBright: supplied | dim: no supply\nAbsent: unsupported", labelStyle);
            GUILayout.Label("Provisional: instant build; free reconfigure; Wood100; test palette.", labelStyle);
            if (City != null) GUILayout.Label("Sources: actual operational buildings. Availability is binary; item quantities remain separate. No population simulation.", labelStyle);
            GUILayout.Label("WASD move / RMB orbit / wheel zoom / R reset", labelStyle);
            GUILayout.EndScrollView(); GUILayout.EndArea();
            foreach (var pair in Layout.Cells)
            {
                string title = pair.Value.Building == ModuleBuilding.None ? $"{pair.Key.x},{pair.Key.y}" : pair.Value.Building.ToString();
                if (City != null && pair.Value.Building != ModuleBuilding.None)
                    title += "\n" + City.State(pair.Key);
                WorldLabel(Position(pair.Key) + new Vector3(0, 1f, 2.4f), title);
            }
            WorldLabel(Position(new Vector2Int(0, -2)) + Vector3.up, "PORT / BLOCKED");
            GUI.matrix = oldMatrix;
        }
        private void DrawBuilding(ModuleCell cell)
        {
            GUILayout.Label("State: " + City.State(selected), labelStyle);
            if (cell.Building != ModuleBuilding.TownHall)
            {
                GUILayout.Label($"INPUT {BuildingPipelines.Inputs(cell.Building)} -> OUTPUT {BuildingPipelines.Outputs(cell.Building)}", labelStyle);
                GUILayout.Label($"Missing input: {City.MissingInputs(selected)}\nActive output: {City.ActiveOutput(selected)}", labelStyle);
            }
            var repair = City.Repair(cell.Building);
            if (repair != null)
            {
                if (!repair.IsComplete)
                {
                    GUILayout.Label($"Provisional Repair: {City.RepairCost(cell.Building)} Wood / {repair.Duration:0.#}s", labelStyle);
                    GUI.enabled = !repair.IsBuilding && Layout.Storage.Get(ResourceKind.Wood) >= City.RepairCost(cell.Building);
                    if (GUILayout.Button(repair.IsBuilding ? $"Repairing {repair.Progress:P0}" : "Repair " + cell.Building, GUILayout.Height(30))) RepairSelected();
                    GUI.enabled = true;
                }
                else GUILayout.Label("Repaired; required inputs still apply.", labelStyle);
            }
            if (cell.Building == ModuleBuilding.WaterPlant)
                GUILayout.Label($"Water {Layout.Storage.Get(ResourceKind.Water)} | +{City.Water.WaterPerCycle}/{City.Water.IntervalSeconds:0.#}s\nCycle {City.Water.ProductionProgress:P0} (pauses without E/M)", labelStyle);
            if (cell.Building == ModuleBuilding.Recycler)
                GUILayout.Label($"Recyclable Material {Layout.Storage.Get(ResourceKind.RecyclableMaterial)}\n{City.Recycler.InputPerCycle} Recyclable -> {City.Recycler.WoodPerCycle} Wood / {City.Recycler.IntervalSeconds:0.#}s\nCycle {City.Recycler.ProcessingProgress:P0} | completed {City.Recycler.ProcessedCycles}", labelStyle);
            if (cell.Building == ModuleBuilding.House)
                GUILayout.Label("House needs actual F + W supply. Population / consumption belongs to Phase 3.", labelStyle);
            if (cell.Building == ModuleBuilding.FishingHarbor)
            {
                var harbor = City.Harbor;
                GUI.enabled = harbor.NetworkAvailable && !harbor.Construction.IsBuilding;
                if (GUILayout.Button(harbor.Construction.IsBuilding ? $"Building boat {harbor.Construction.Progress:P0}" : "Build Fishing Boat", GUILayout.Height(30))) BuildFishingBoat();
                GUI.enabled = true;
                GUILayout.Label($"Boats {harbor.BoatCount} | Fish {Layout.Storage.Get(ResourceKind.Fish)}\n{harbor.LastDelivery}", labelStyle);
                GUILayout.Label("M enables the F source. Boats deliver concrete Fish at arrival.", labelStyle);
                foreach (var boat in harbor.Boats)
                    GUILayout.Label($"{boat.Visual.name}: {boat.Trip.Stage} {boat.Trip.CycleElapsed:0.0}/{boat.Trip.Duration:0.#}s | unloads {boat.UnloadedTrips}", labelStyle);
                GUILayout.Label("Provisional: M loss pauses all boat / construction clocks; resume without catch-up. Fish credits at arrival, no Food conversion.", labelStyle);
            }
        }
        private void WorldLabel(Vector3 position, string title)
        {
            Vector3 screen = view.WorldToScreenPoint(position);
            var rect = new Rect(screen.x / UiScale - 68, (Screen.height - screen.y) / UiScale - 10, 136, title.Contains('\n') ? 40 : 22);
            if (screen.z > 0 && !rect.Overlaps(PanelRect)) GUI.Box(rect, title);
        }
    }
}
