using System;
using System.Collections.Generic;
using UnityEngine;

namespace Husk
{
    [Serializable]
    public sealed class NetworkProductionSettings
    {
        [Min(0)] public int waterRepairWood = 5, recyclerRepairWood = 5;
        [Min(0.1f)] public float waterRepairSeconds = 5, recyclerRepairSeconds = 5;
        [Min(0.1f)] public float waterInterval = 2, recyclerInterval = 4;
        [Min(1)] public int waterPerCycle = 1, recyclablePerCycle = 2, woodPerCycle = 1;
        [Min(0)] public int startingRecyclableMaterial = 100;
    }

    public enum NetworkBuildingState { Storage, Damaged, Repairing, Disabled, WaitingForMaterial, Operational }

    // Phase 2 adapter: existing clocks own production arithmetic. No population or quantity routing.
    public sealed class NetworkedCity
    {
        public static readonly Vector2Int SolarPosition = new(-2, 1), ElectricBridge = new(-1, 1),
            RecyclerPosition = new(0, 1), MaterialBridge = new(0, 0), WaterPosition = new(1, 0),
            HarborPosition = new(0, -1), WaterBridge = new(1, -1), HousePosition = new(2, -1), TownHallPosition = new(-1, 0);
        public ModuleLayout Layout { get; }
        public ModuleNetwork Network { get; }
        public ResourceState Storage => Layout.Storage;
        public WaterProduction Water { get; }
        public RecyclerProcessing Recycler { get; }
        public BoatConstruction WaterRepair { get; }
        public BoatConstruction RecyclerRepair { get; }
        public FishingHarbor Harbor { get; private set; }
        private readonly NetworkProductionSettings settings;
        private readonly Dictionary<Vector2Int, Pipeline> sources = new();
        private readonly Dictionary<Vector2Int, NetworkBuildingState> states = new();
        private readonly Dictionary<Vector2Int, Pipeline> missingInputs = new();
        private readonly ModuleBuilding[] resolutionOrder = { ModuleBuilding.Solar, ModuleBuilding.Recycler,
            ModuleBuilding.WaterPlant, ModuleBuilding.FishingHarbor, ModuleBuilding.House, ModuleBuilding.TownHall };

        public NetworkedCity(ModuleLayout layout, NetworkProductionSettings settings)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (settings.waterRepairWood < 0 || settings.recyclerRepairWood < 0) throw new ArgumentOutOfRangeException(nameof(settings));
            Network = new ModuleNetwork(layout, useTestSupply: false);
            WaterRepair = new BoatConstruction(settings.waterRepairSeconds);
            RecyclerRepair = new BoatConstruction(settings.recyclerRepairSeconds);
            Water = new WaterProduction(settings.waterRepairSeconds, settings.waterInterval, settings.waterPerCycle);
            Recycler = new RecyclerProcessing(settings.recyclerRepairSeconds, settings.recyclerInterval, settings.recyclablePerCycle, settings.woodPerCycle);
            Resolve();
        }

        public static ModuleLayout CreateLayout(int wood = 100, int recyclableMaterial = 100)
        {
            var layout = new ModuleLayout(new ResourceState(startingWood: wood, startingRecyclableMaterial: recyclableMaterial));
            layout.AddInitial(SolarPosition, Pipeline.F | Pipeline.M | Pipeline.E, ModuleBuilding.Solar);
            layout.AddInitial(ElectricBridge, Pipeline.F | Pipeline.M | Pipeline.E);
            layout.AddInitial(RecyclerPosition, Pipeline.W | Pipeline.M | Pipeline.E, ModuleBuilding.Recycler);
            layout.AddInitial(MaterialBridge, Pipeline.W | Pipeline.M | Pipeline.E);
            layout.AddInitial(WaterPosition, Pipeline.W | Pipeline.M | Pipeline.E, ModuleBuilding.WaterPlant);
            layout.AddInitial(HarborPosition, Pipeline.F | Pipeline.W | Pipeline.M, ModuleBuilding.FishingHarbor);
            layout.AddInitial(WaterBridge, Pipeline.F | Pipeline.W | Pipeline.M);
            layout.AddInitial(HousePosition, Pipeline.F | Pipeline.W | Pipeline.M, ModuleBuilding.House);
            layout.AddInitial(TownHallPosition, Pipeline.F | Pipeline.W | Pipeline.M, ModuleBuilding.TownHall);
            layout.ReservePort(HarborPosition, new Vector2Int(0, -2));
            return layout;
        }
        public void BindHarbor(FishingHarbor harbor) { Harbor = harbor; Resolve(); }
        public NetworkBuildingState State(Vector2Int position) => states.TryGetValue(position, out var state) ? state : NetworkBuildingState.Disabled;
        public Pipeline MissingInputs(Vector2Int position) => missingInputs.TryGetValue(position, out var missing) ? missing : Pipeline.None;
        public Pipeline ActiveOutput(Vector2Int position) => sources.TryGetValue(position, out var output) ? output : Pipeline.None;
        public BoatConstruction Repair(ModuleBuilding building) => building == ModuleBuilding.WaterPlant ? WaterRepair : building == ModuleBuilding.Recycler ? RecyclerRepair : null;
        public int RepairCost(ModuleBuilding building) => building == ModuleBuilding.WaterPlant ? settings.waterRepairWood : settings.recyclerRepairWood;
        public bool TryRepair(Vector2Int position)
        {
            if (!Layout.Cells.TryGetValue(position, out var cell)) return false;
            var repair = Repair(cell.Building);
            if (repair == null || repair.IsBuilding || repair.IsComplete || Storage.Get(ResourceKind.Wood) < RepairCost(cell.Building)) return false;
            // Start before publishing debit so a reentrant observer cannot start or pay twice.
            repair.TryStart();
            Storage.TryRemove(ResourceKind.Wood, RepairCost(cell.Building));
            Resolve(); return true;
        }

        private bool Available(Vector2Int position, Pipeline type)
        {
            foreach (var source in sources)
                if ((source.Value & type) != 0 && Network.Connected(source.Key, position, type)) return true;
            return false;
        }
        public void Resolve()
        {
            // This phase's dependency order is E -> M -> W/F -> House. Rebuild from zero:
            // old output never sustains a source whose upstream input has disappeared.
            sources.Clear(); states.Clear(); missingInputs.Clear();
            foreach (var building in resolutionOrder)
                foreach (var pair in Layout.Cells)
                {
                    if (pair.Value.Building != building) continue;
                    Pipeline missing = Pipeline.None;
                    foreach (var type in ModuleNetwork.Types)
                        if ((BuildingPipelines.Inputs(building) & type) != 0 && !Available(pair.Key, type)) missing |= type;
                    missingInputs.Add(pair.Key, missing);
                    var repair = Repair(building);
                    var state = building == ModuleBuilding.TownHall ? NetworkBuildingState.Storage
                        : repair != null && !repair.IsComplete ? (repair.IsBuilding ? NetworkBuildingState.Repairing : NetworkBuildingState.Damaged)
                        : missing != Pipeline.None ? NetworkBuildingState.Disabled
                        : building == ModuleBuilding.Recycler && Storage.Get(ResourceKind.RecyclableMaterial) < Recycler.InputPerCycle ? NetworkBuildingState.WaitingForMaterial
                        : NetworkBuildingState.Operational;
                    states.Add(pair.Key, state);
                    var output = state == NetworkBuildingState.Operational ? BuildingPipelines.Outputs(building) : Pipeline.None;
                    // Output availability follows operational inputs; concrete Fish credits only at boat arrival.
                    if (output != Pipeline.None) sources.Add(pair.Key, output);
                }
            Network.SetOperationalSources(sources);
            if (Harbor != null) Harbor.SetNetworkAvailable(State(HarborPosition) == NetworkBuildingState.Operational);
        }

        public void Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            Resolve();
            double remaining = deltaTime;
            while (remaining > 0)
            {
                double step = remaining;
                if (WaterRepair.IsBuilding) step = Math.Min(step, WaterRepair.Duration - WaterRepair.Elapsed);
                if (RecyclerRepair.IsBuilding) step = Math.Min(step, RecyclerRepair.Duration - RecyclerRepair.Elapsed);
                // Split at concrete input exhaustion boundaries so a long frame cannot produce downstream
                // after Recycler consumed its last batch. The original conversion remains atomic.
                bool recycling = State(RecyclerPosition) == NetworkBuildingState.Operational;
                if (recycling) step = Math.Min(step, Recycler.IntervalSeconds * (1 - Recycler.ProcessingProgress));
                float seconds = (float)step;
                if (State(WaterPosition) == NetworkBuildingState.Operational) Storage.Add(ResourceKind.Water, Water.Tick(seconds));
                if (recycling) Recycler.Tick(seconds, Storage);
                else if (Recycler.IsOperational && State(RecyclerPosition) == NetworkBuildingState.WaitingForMaterial) Recycler.Tick(0, Storage);
                if (Harbor != null) Harbor.AdvanceSimulation(seconds);
                WaterRepair.Tick(seconds); RecyclerRepair.Tick(seconds);
                // Skip only the existing construction portion: repair already paid/timed it.
                if (WaterRepair.IsComplete && !Water.HasStarted) { Water.TryStart(); Water.Tick((float)Water.BuildSeconds); }
                if (RecyclerRepair.IsComplete && !Recycler.HasStarted) { Recycler.TryStart(); Recycler.Tick((float)Recycler.BuildSeconds, Storage); }
                remaining -= step;
                Resolve();
            }
        }
    }
}
