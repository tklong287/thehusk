using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class NetworkedCityTests
    {
        private NetworkedCity city;
        private GameObject root;
        private FishingHarbor harbor;
        private const Pipeline FWM = Pipeline.F | Pipeline.W | Pipeline.M;
        private const Pipeline FME = Pipeline.F | Pipeline.M | Pipeline.E;
        private const Pipeline WME = Pipeline.W | Pipeline.M | Pipeline.E;
        private const Pipeline FWE = Pipeline.F | Pipeline.W | Pipeline.E;

        [SetUp]
        public void Setup()
        {
            city = NewCity();
            root = new GameObject("Network integration harbor"); root.SetActive(false);
            harbor = root.AddComponent<FishingHarbor>();
            harbor.BindNetwork(city.Storage, Child("Boat template"), Child("Marker"), Child("Construction"));
            typeof(FishingHarbor).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(harbor, null);
            harbor.Select(true); city.BindHarbor(harbor);
        }
        private static NetworkedCity NewCity(int wood = 100, int recyclable = 100) =>
            new(NetworkedCity.CreateLayout(wood, recyclable), new NetworkProductionSettings());
        private GameObject Child(string name)
        { var child = new GameObject(name); child.transform.SetParent(root.transform); return child; }
        [TearDown] public void Cleanup() => Object.DestroyImmediate(root);
        private void Configure(Vector2Int position, Pipeline pipelines)
        { Assert.That(city.Layout.TryConfigure(position, pipelines, out _), Is.True); city.Resolve(); }
        private void RepairBoth()
        {
            Assert.That(city.TryRepair(NetworkedCity.RecyclerPosition), Is.True);
            Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.True);
            city.Tick(5);
        }
        private void FullProduction()
        { RepairBoth(); Assert.That(harbor.TryBuildBoat(), Is.True); city.Tick(5); }
        private void Supplied(Vector2Int position, Pipeline type, bool expected = true) =>
            Assert.That(city.Network.State(position, type), Is.EqualTo(expected ? PipelineState.Supplied : PipelineState.Unsupplied));

        [TestCase(ModuleBuilding.FishingHarbor, Pipeline.M, Pipeline.F)]
        [TestCase(ModuleBuilding.WaterPlant, Pipeline.E | Pipeline.M, Pipeline.W)]
        [TestCase(ModuleBuilding.Recycler, Pipeline.E, Pipeline.M)]
        [TestCase(ModuleBuilding.Solar, Pipeline.None, Pipeline.E)]
        [TestCase(ModuleBuilding.House, Pipeline.F | Pipeline.W, Pipeline.None)]
        public void PlacementPreservesThreeSlotsAndLocksExactlyInputsAndOutputs(ModuleBuilding building, Pipeline inputs, Pipeline outputs)
        {
            Assert.That(BuildingPipelines.Inputs(building), Is.EqualTo(inputs));
            Assert.That(BuildingPipelines.Outputs(building), Is.EqualTo(outputs));
            Assert.That(BuildingPipelines.RequiredModule(building), Is.EqualTo(inputs | outputs));
            foreach (var original in new[] { FWM, FME, WME, FWE })
            {
                var layout = new ModuleLayout(new ResourceState());
                layout.AddInitial(Vector2Int.zero, original);
                int notifications = 0;
                layout.Changed += () => {
                    notifications++;
                    Assert.That(BuildingPipelines.Allows(building, layout.Cells[Vector2Int.zero].Pipelines), Is.True);
                    Assert.That(layout.Cells[Vector2Int.zero].Building, Is.EqualTo(building));
                };
                Assert.That(layout.TryOccupy(Vector2Int.zero, building, out _), Is.True);
                Assert.That(notifications, Is.EqualTo(1));
                var cell = layout.Cells[Vector2Int.zero];
                Assert.That(BuildingPipelines.IsStandard(cell.Pipelines), Is.True);
                Assert.That(cell.LockedPipelines, Is.EqualTo(inputs | outputs));
                if ((original & cell.LockedPipelines) == cell.LockedPipelines)
                    Assert.That(cell.Pipelines, Is.EqualTo(original), "Compatible configuration must be retained.");
                var repeat = new ModuleLayout(new ResourceState()); repeat.AddInitial(Vector2Int.zero, original);
                Assert.That(repeat.TryOccupy(Vector2Int.zero, building, out _), Is.True);
                Assert.That(repeat.Cells[Vector2Int.zero].Pipelines, Is.EqualTo(cell.Pipelines));
                foreach (var candidate in new[] { FWM, FME, WME, FWE })
                {
                    var before = cell.Pipelines;
                    bool legal = (candidate & cell.LockedPipelines) == cell.LockedPipelines;
                    Assert.That(layout.TryConfigure(Vector2Int.zero, candidate, out _), Is.EqualTo(legal));
                    Assert.That(cell.Pipelines, Is.EqualTo(legal ? candidate : before));
                }
            }
        }
        [Test] public void MissingRequiredPlacementDropsOnlyTheStableLowestPriorityOptional()
        {
            Assert.That(BuildingPipelines.ForPlacement(ModuleBuilding.House, FME), Is.EqualTo(FWM));
            Assert.That(BuildingPipelines.ForPlacement(ModuleBuilding.Solar, FWM), Is.EqualTo(FWE));
            Assert.That(BuildingPipelines.ForPlacement(ModuleBuilding.Recycler, FWM), Is.EqualTo(FME));
            Assert.That(BuildingPipelines.ForPlacement(ModuleBuilding.WaterPlant, FWM), Is.EqualTo(WME));
        }
        [Test] public void SolarIsOperationalWithoutExternalElectricAndProducersDoNotNeedTheirOutputs()
        {
            Assert.That(city.State(NetworkedCity.SolarPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.MissingInputs(NetworkedCity.SolarPosition), Is.EqualTo(Pipeline.None));
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.M, false);
            Supplied(NetworkedCity.WaterPosition, Pipeline.W, false);
            RepairBoth();
            foreach (var pair in new[] { (NetworkedCity.SolarPosition, Pipeline.E), (NetworkedCity.RecyclerPosition, Pipeline.M),
                (NetworkedCity.WaterPosition, Pipeline.W), (NetworkedCity.HarborPosition, Pipeline.F) })
            {
                Assert.That(city.State(pair.Item1), Is.EqualTo(NetworkBuildingState.Operational));
                Assert.That(city.MissingInputs(pair.Item1), Is.EqualTo(Pipeline.None));
                Assert.That(city.ActiveOutput(pair.Item1), Is.EqualTo(pair.Item2));
            }
            Configure(NetworkedCity.ElectricBridge, FWM);
            foreach (var position in new[] { NetworkedCity.RecyclerPosition, NetworkedCity.WaterPosition, NetworkedCity.HarborPosition })
            {
                Assert.That(city.State(position), Is.EqualTo(NetworkBuildingState.Disabled));
                Assert.That(city.ActiveOutput(position), Is.EqualTo(Pipeline.None));
            }
        }
        [Test] public void FreshCityHasSixValidBuildingsThreeRoutersAndHonestSources()
        {
            Assert.That(city.Layout.Cells.Count, Is.EqualTo(9));
            foreach (var building in new[] { ModuleBuilding.TownHall, ModuleBuilding.Solar, ModuleBuilding.Recycler,
                ModuleBuilding.WaterPlant, ModuleBuilding.FishingHarbor, ModuleBuilding.House })
                Assert.That(city.Layout.Cells.Values.Count(c => c.Building == building), Is.EqualTo(1));
            Assert.That(city.Layout.Cells.Values.All(c => BuildingPipelines.Allows(c.Building, c.Pipelines)), Is.True);
            Assert.That(city.Layout.Cells.Values.All(c => c.TestSupply == Pipeline.None), Is.True);
            Assert.That(city.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(100));
            Assert.That(city.ActiveOutput(NetworkedCity.TownHallPosition), Is.EqualTo(Pipeline.None));
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.F | Pipeline.W));
        }
        [Test] public void SolarSuppliesElectricConnectedComponentOnly()
        {
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.E);
            Assert.That(city.ActiveOutput(NetworkedCity.SolarPosition), Is.EqualTo(Pipeline.E));
            Configure(NetworkedCity.ElectricBridge, FWM);
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.E, false);
            Supplied(NetworkedCity.SolarPosition, Pipeline.E);
            Configure(NetworkedCity.ElectricBridge, FME);
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.E);
        }
        [Test] public void DamagedBuildingsNeverOutputEvenWithConnectedPipelines()
        {
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Damaged));
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Damaged));
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.M, false);
            Supplied(NetworkedCity.WaterPosition, Pipeline.W, false);
            city.Tick(30);
            Assert.That(city.Water.ProducedCycles, Is.Zero);
            Assert.That(city.Recycler.ProcessedCycles, Is.Zero);
        }
        [Test] public void RepairWaterFirstStillRequiresMaterial()
        {
            city.TryRepair(NetworkedCity.WaterPosition); city.Tick(5);
            Assert.That(city.WaterRepair.IsComplete, Is.True);
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Assert.That(city.MissingInputs(NetworkedCity.WaterPosition), Is.EqualTo(Pipeline.M));
            Supplied(NetworkedCity.WaterPosition, Pipeline.E);
            Supplied(NetworkedCity.WaterPosition, Pipeline.W, false);
        }
        [Test] public void RepairedRecyclerRequiresElectricThenProvidesMaterial()
        {
            Configure(NetworkedCity.ElectricBridge, FWM);
            city.TryRepair(NetworkedCity.RecyclerPosition); city.Tick(5);
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Supplied(NetworkedCity.MaterialBridge, Pipeline.M, false);
            Configure(NetworkedCity.ElectricBridge, FME);
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Supplied(NetworkedCity.MaterialBridge, Pipeline.M);
        }
        [Test] public void WaterWithBothInputsSuppliesWaterAndProducesConcreteItem()
        {
            RepairBoth();
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Supplied(NetworkedCity.HousePosition, Pipeline.W);
            city.Tick(2);
            Assert.That(city.Storage.Get(ResourceKind.Water), Is.EqualTo(101));
            Assert.That(city.Storage.Get(ResourceKind.Food), Is.EqualTo(100));
        }
        [Test] public void MissingMaterialDisablesWaterWhileElectricRemains()
        {
            RepairBoth(); Configure(NetworkedCity.MaterialBridge, FWE);
            Assert.That(city.MissingInputs(NetworkedCity.WaterPosition), Is.EqualTo(Pipeline.M));
            Supplied(NetworkedCity.WaterPosition, Pipeline.E);
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
            city.Tick(2); Assert.That(city.Storage.Get(ResourceKind.Water), Is.EqualTo(100));
            Configure(NetworkedCity.MaterialBridge, WME);
            Supplied(NetworkedCity.HousePosition, Pipeline.W);
        }
        [Test] public void MissingElectricStopsRecyclerAndCascadesThroughBothOutputs()
        {
            FullProduction(); Configure(NetworkedCity.ElectricBridge, FWM);
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Supplied(NetworkedCity.HousePosition, Pipeline.F, false);
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
            Supplied(NetworkedCity.HarborPosition, Pipeline.M, false);
            Configure(NetworkedCity.ElectricBridge, FME);
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Operational));
        }
        [Test] public void WaterLosesElectricIndependentlyWhileMaterialSourceStaysOperational()
        {
            RepairBoth(); Configure(NetworkedCity.MaterialBridge, FWM);
            Supplied(NetworkedCity.WaterPosition, Pipeline.M);
            Supplied(NetworkedCity.WaterPosition, Pipeline.E, false);
            Assert.That(city.MissingInputs(NetworkedCity.WaterPosition), Is.EqualTo(Pipeline.E));
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
        }
        [Test] public void AnotherRealSolarKeepsProductionSuppliedAfterOriginalRouteBreaks()
        {
            RepairBoth();
            city.Layout.AddInitial(new Vector2Int(1, 1), FME, ModuleBuilding.Solar);
            Configure(NetworkedCity.ElectricBridge, FWM);
            Assert.That(city.ActiveOutput(new Vector2Int(1, 1)), Is.EqualTo(Pipeline.E));
            Supplied(NetworkedCity.RecyclerPosition, Pipeline.E);
            Supplied(NetworkedCity.HousePosition, Pipeline.W);
        }
        [Test] public void HarborNeedsOnlyMaterialAndProvidesFoodWithoutPreexistingFoodOrBoat()
        {
            Assert.That(harbor.TryBuildBoat(), Is.False);
            Supplied(NetworkedCity.HarborPosition, Pipeline.F, false);
            city.TryRepair(NetworkedCity.RecyclerPosition); city.Tick(5);
            Assert.That(city.State(NetworkedCity.HarborPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(harbor.BoatCount, Is.Zero);
            Assert.That(city.ActiveOutput(NetworkedCity.HarborPosition), Is.EqualTo(Pipeline.F));
            Supplied(NetworkedCity.HousePosition, Pipeline.F);
            Assert.That(harbor.TryBuildBoat(), Is.True); city.Tick(5);
            Supplied(NetworkedCity.HousePosition, Pipeline.F);
            Assert.That(city.Storage.Get(ResourceKind.Fish), Is.EqualTo(100));
        }
        [Test] public void HouseRequiresBothFoodAndWaterAndHasNoOutput()
        {
            FullProduction();
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.ActiveOutput(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.None));
        }
        [Test] public void FoodWithoutWaterLeavesHouseDisabledAndFoodUnaffected()
        {
            FullProduction(); Configure(NetworkedCity.WaterBridge, FME);
            Supplied(NetworkedCity.HousePosition, Pipeline.F);
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.W));
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Configure(NetworkedCity.WaterBridge, FWM);
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Operational));
        }
        [Test] public void WaterWithoutFoodLeavesHouseDisabled()
        {
            RepairBoth(); Configure(NetworkedCity.WaterBridge, WME);
            Supplied(NetworkedCity.HousePosition, Pipeline.W);
            Supplied(NetworkedCity.HousePosition, Pipeline.F, false);
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.F));
        }
        [Test] public void GlobalStockAndPhase1FixturesCannotBypassDisconnectedOutput()
        {
            FullProduction(); Configure(NetworkedCity.WaterBridge, FME);
            city.Layout.SetTestSupply(NetworkedCity.HousePosition, Pipeline.All);
            city.Storage.Add(ResourceKind.Water, 1000); city.Resolve();
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
            Assert.That(city.State(NetworkedCity.HousePosition), Is.EqualTo(NetworkBuildingState.Disabled));
        }
        [Test] public void RepairDebitsOnlyOnceAndInsufficientRequestDoesNothing()
        {
            int observed = 0;
            city.Storage.Changed += () => { observed++; Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.False); };
            Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.True);
            Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.False);
            Assert.That(city.Storage.Get(ResourceKind.Wood), Is.EqualTo(95));
            Assert.That(observed, Is.EqualTo(1));
            var poor = NewCity(4);
            Assert.That(poor.TryRepair(NetworkedCity.WaterPosition), Is.False);
            Assert.That(poor.WaterRepair.IsBuilding, Is.False);
            Assert.That(poor.Storage.Get(ResourceKind.Wood), Is.EqualTo(4));
        }
        [Test] public void RecyclerPreservesConcreteIdentityAndAtomicConversion()
        {
            RepairBoth(); city.Tick(4);
            Assert.That(city.Storage.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(98));
            Assert.That(city.Storage.Get(ResourceKind.Wood), Is.EqualTo(91));
            Assert.That(city.Storage.Get(ResourceKind.Iron), Is.EqualTo(100));
            Assert.That(city.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(100));
        }
        [Test] public void InputExhaustionStopsMaterialAndDownstreamUntilReplenished()
        {
            RepairBoth(); city.Storage.TryRemove(ResourceKind.RecyclableMaterial, 98);
            city.Tick(8);
            Assert.That(city.Recycler.ProcessedCycles, Is.EqualTo(1));
            Assert.That(city.Storage.Get(ResourceKind.Water), Is.EqualTo(102));
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.WaitingForMaterial));
            Supplied(NetworkedCity.HousePosition, Pipeline.W, false);
            city.Storage.Add(ResourceKind.RecyclableMaterial, 2); city.Tick(0);
            Supplied(NetworkedCity.HousePosition, Pipeline.W);
        }
        [Test] public void RepairOvershootAndLongTicksMatchFramePartition()
        {
            var once = NewCity(); var split = NewCity();
            foreach (var value in new[] { once, split })
            { value.TryRepair(NetworkedCity.WaterPosition); value.TryRepair(NetworkedCity.RecyclerPosition); }
            once.Tick(240);
            for (int i = 0; i < 960; i++) split.Tick(0.25f);
            foreach (ResourceKind kind in System.Enum.GetValues(typeof(ResourceKind)))
                Assert.That(once.Storage.Get(kind), Is.EqualTo(split.Storage.Get(kind)), kind.ToString());
            Assert.That(once.Water.ProducedCycles, Is.EqualTo(100));
            Assert.That(once.Recycler.ProcessedCycles, Is.EqualTo(50));
        }
        [Test] public void MaterialLossPausesStaggeredBoatsAndConstructionWithoutDuplicateUnload()
        {
            FullProduction(); city.Tick(2); Assert.That(harbor.TryBuildBoat(), Is.True); city.Tick(2);
            var first = harbor.Boats[0]; double elapsed = first.Trip.CycleElapsed;
            Configure(NetworkedCity.MaterialBridge, FWE); city.Tick(20);
            Assert.That(first.Trip.CycleElapsed, Is.EqualTo(elapsed));
            Assert.That(harbor.Construction.Elapsed, Is.EqualTo(2));
            Assert.That(city.Storage.Get(ResourceKind.Fish), Is.EqualTo(100));
            Configure(NetworkedCity.MaterialBridge, WME); city.Tick(3);
            Assert.That(harbor.BoatCount, Is.EqualTo(2));
            Assert.That(first.Trip.CycleElapsed, Is.EqualTo(7));
            Assert.That(harbor.Boats[1].Trip.CycleElapsed, Is.Zero);
            city.Tick(21);
            Assert.That(city.Storage.Get(ResourceKind.Fish), Is.EqualTo(105));
            city.Tick(0);
            Assert.That(city.Storage.Get(ResourceKind.Fish), Is.EqualTo(105));
            Assert.That(city.Storage.Get(ResourceKind.Food), Is.EqualTo(100));
        }
        [Test] public void OperationalSourcesRespectAlternatePathsAndMultipleSources()
        {
            var layout = new ModuleLayout(new ResourceState());
            foreach (var p in new[] { Vector2Int.zero, Vector2Int.right, Vector2Int.up, Vector2Int.one }) layout.AddInitial(p, FWE);
            var network = new ModuleNetwork(layout, false);
            var sources = new Dictionary<Vector2Int, Pipeline> { [Vector2Int.zero] = Pipeline.E, [Vector2Int.one] = Pipeline.E };
            network.SetOperationalSources(sources);
            layout.TryConfigure(Vector2Int.right, FWM, out _);
            Assert.That(network.State(Vector2Int.one, Pipeline.E), Is.EqualTo(PipelineState.Supplied));
            sources.Remove(Vector2Int.zero); network.SetOperationalSources(sources);
            Assert.That(network.State(Vector2Int.up, Pipeline.E), Is.EqualTo(PipelineState.Supplied));
            sources.Clear(); network.SetOperationalSources(sources);
            Assert.That(network.Connected(Vector2Int.zero, Vector2Int.one, Pipeline.E), Is.True);
            Assert.That(network.State(Vector2Int.up, Pipeline.E), Is.EqualTo(PipelineState.Unsupplied));
        }
        [Test] public void UnchangedResolutionDoesNotInvalidateNetworkVisualRevision()
        {
            int revision = city.Network.Revision;
            for (int i = 0; i < 10; i++) city.Tick(1);
            Assert.That(city.Network.Revision, Is.EqualTo(revision));
            var fresh = NewCity();
            Assert.That(fresh.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Damaged));
            Assert.That(fresh.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(100));
        }
    }
}
