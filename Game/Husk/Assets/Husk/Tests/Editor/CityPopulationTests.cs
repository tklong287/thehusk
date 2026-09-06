using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class CityPopulationTests
    {
        private const Pipeline FWM = Pipeline.F | Pipeline.W | Pipeline.M;
        private const Pipeline FME = Pipeline.F | Pipeline.M | Pipeline.E;
        private const Pipeline WME = Pipeline.W | Pipeline.M | Pipeline.E;
        private const Pipeline FWE = Pipeline.F | Pipeline.W | Pipeline.E;
        private static NetworkedCity Fresh() => new(NetworkedCity.CreateLayout(cityHallStorage: true), new NetworkProductionSettings(), new PopulationSettings());
        private static void Configure(NetworkedCity city, Vector2Int position, Pipeline value)
        { Assert.That(city.Layout.TryConfigure(position, value, out _), Is.True); city.Resolve(); }
        private static void AssertNear(double actual, double expected) => Assert.That(actual, Is.EqualTo(expected).Within(1e-7));

        [Test] public void FreshHasRealHallPathsAnd100OfEveryConcreteItemWithDamagedProducers()
        {
            var city = Fresh();
            AssertNear(city.Population.Current, 100);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            Assert.That(city.Population.HouseCount, Is.EqualTo(1));
            foreach (ResourceKind item in Enum.GetValues(typeof(ResourceKind))) AssertNear(city.Storage.GetExact(item), 100);
            Assert.That(Enum.GetNames(typeof(ResourceKind)), Does.Not.Contain("Electric"));
            foreach (var category in new[] { Pipeline.F, Pipeline.W })
            {
                Assert.That(city.Network.Connected(NetworkedCity.TownHallPosition, NetworkedCity.HousePosition, category), Is.True);
                Assert.That(city.Network.State(NetworkedCity.HousePosition, category), Is.EqualTo(PipelineState.Supplied));
            }
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Damaged));
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Damaged));
            Assert.That(city.Layout.Cells.Values.Count(c => c.Building != ModuleBuilding.None), Is.EqualTo(6));
            Assert.That(city.Layout.Cells.Values.All(c => c.TestSupply == Pipeline.None), Is.True);
        }
        [Test] public void CityHallIsFixedSpecialFourAndAllOrdinaryModulesRemainThree()
        {
            var city = Fresh(); var hall = city.Layout.Cells[NetworkedCity.TownHallPosition];
            Assert.That(hall.Pipelines, Is.EqualTo(Pipeline.All));
            Assert.That(hall.LockedPipelines, Is.EqualTo(Pipeline.All));
            foreach (var value in new[] { FWM, FME, WME, FWE })
                Assert.That(city.Layout.TryConfigure(hall.Position, value, out _), Is.False);
            Assert.That(city.Layout.TryConfigure(hall.Position, Pipeline.All, out _), Is.True);
            foreach (var cell in city.Layout.Cells.Values.Where(c => c != hall))
            {
                Assert.That(BuildingPipelines.IsStandard(cell.Pipelines), Is.True);
                Assert.That(city.Layout.TryConfigure(cell.Position, Pipeline.All, out _), Is.False);
            }
            Assert.That(city.Layout.TryBuild(new Vector2Int(-2, 0), Pipeline.All, out _), Is.False);
        }
        [Test] public void HallPassesElectricButCannotSupplyElectricEvenWithFullStock()
        {
            var layout = new ModuleLayout(new ResourceState());
            layout.AddInitial(NetworkedCity.TownHallPosition, Pipeline.All, ModuleBuilding.TownHall);
            layout.AddInitial(new Vector2Int(-2, 0), FME, ModuleBuilding.Solar);
            layout.AddInitial(Vector2Int.zero, FME);
            var city = new NetworkedCity(layout, new NetworkProductionSettings(), new PopulationSettings());
            Assert.That(city.Network.State(Vector2Int.zero, Pipeline.E), Is.EqualTo(PipelineState.Supplied));
            Assert.That(city.ActiveOutput(NetworkedCity.TownHallPosition) & Pipeline.E, Is.EqualTo(Pipeline.None));
            var alone = new ModuleLayout(new ResourceState());
            alone.AddInitial(NetworkedCity.TownHallPosition, Pipeline.All, ModuleBuilding.TownHall);
            var noSolar = new NetworkedCity(alone, new NetworkProductionSettings(), new PopulationSettings());
            Assert.That(noSolar.Network.State(NetworkedCity.TownHallPosition, Pipeline.E), Is.EqualTo(PipelineState.Unsupplied));
        }
        [TestCase(60, 110)] [TestCase(120, 121)] [TestCase(180, 133.1)]
        public void CompoundGrowthIgnoresCapacityAndDepletion(float seconds, double expected)
        {
            var city = Fresh(); city.Tick(seconds);
            AssertNear(city.Population.Current, expected);
            Assert.That(city.Population.EffectiveCapacity, Is.Zero);
            AssertNear(city.Storage.GetExact(ResourceKind.Water), 0);
        }
        [Test] public void GrowthIsNotCappedWhenHouseRemainsSupplied()
        {
            var city = Fresh(); city.Storage.Add(ResourceKind.Fish, 2000); city.Storage.Add(ResourceKind.Water, 2000);
            city.Tick(120);
            AssertNear(city.Population.Current, 121); Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            AssertNear(city.Population.FoodDemand, 6.05); AssertNear(city.Population.WaterDemand, 6.05);
        }
        [TestCase(0.25f, 480)] [TestCase(0.01f, 12000)] [TestCase(0.001f, 120000)]
        public void LargeTickMatchesFramePartitionsIncludingTinyFrames(float frame, int count)
        {
            var large = Fresh(); var frames = Fresh(); large.Tick(frame * count);
            for (int i = 0; i < count; i++) frames.Tick(frame);
            AssertNear(frames.Population.Current, large.Population.Current);
            AssertNear(frames.Population.GrowthElapsed, large.Population.GrowthElapsed);
            foreach (ResourceKind kind in Enum.GetValues(typeof(ResourceKind))) AssertNear(frames.Storage.GetExact(kind), large.Storage.GetExact(kind));
        }
        [Test] public void GrowthPreservesFractionalPopulationAndRoundingAffectsDisplayOnly()
        {
            var p = new CityPopulation(new PopulationSettings { displayDecimals = 0 }); p.Tick(180);
            AssertNear(p.Current, 133.1); Assert.That(p.Display, Is.EqualTo("133 / 0"));
            p.Tick(0); AssertNear(p.Current, 133.1);
        }
        [Test] public void IndependentInputLossKeepsOtherCategorySupplied()
        {
            foreach (var sample in new[] { (FME, Pipeline.W, Pipeline.F), (WME, Pipeline.F, Pipeline.W) })
            {
                var city = Fresh(); Configure(city, NetworkedCity.WaterBridge, sample.Item1);
                Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(sample.Item2));
                Assert.That(city.Network.State(NetworkedCity.HousePosition, sample.Item3), Is.EqualTo(PipelineState.Supplied));
                Assert.That(city.Population.EffectiveCapacity, Is.Zero); AssertNear(city.Population.Current, 100);
                Configure(city, NetworkedCity.WaterBridge, FWM);
                Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100)); AssertNear(city.Population.Current, 100);
            }
        }
        [Test] public void TwoHousesReallocateWithoutAddingOrDeletingResidentsOrDoubleDemand()
        {
            var city = Fresh();
            Assert.That(city.TryBuildHouse(new Vector2Int(-1, -1), out _), Is.True);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200)); AssertNear(city.Population.Current, 100);
            city.Tick(1); AssertNear(city.Storage.GetExact(ResourceKind.Fish), 95); AssertNear(city.Storage.GetExact(ResourceKind.Water), 95);
            Configure(city, NetworkedCity.WaterBridge, FME);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100)); AssertNear(city.Population.Current, 100);
            Configure(city, NetworkedCity.WaterBridge, FWM);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200));
        }
        [TestCase(FME)] [TestCase(WME)] [TestCase(FWM)] [TestCase(FWE)]
        public void BuildHouseAutoConfiguresAndLocksRequiredWithoutChangingOtherModules(Pipeline initial)
        {
            var city = Fresh(); var spot = NetworkedCity.WaterBridge;
            Configure(city, spot, initial);
            var before = city.Layout.Cells.Where(p => p.Key != spot).ToDictionary(p => p.Key, p => p.Value.Pipelines);
            Assert.That(city.TryBuildHouse(spot, out _), Is.True);
            var cell = city.Layout.Cells[spot]; Assert.That(cell.LockedPipelines, Is.EqualTo(Pipeline.F | Pipeline.W));
            Assert.That(BuildingPipelines.IsStandard(cell.Pipelines), Is.True);
            Assert.That(city.Layout.TryConfigure(spot, FME, out _), Is.False);
            Assert.That(city.Layout.TryConfigure(spot, WME, out _), Is.False);
            Assert.That(city.Layout.TryConfigure(spot, FWE, out _), Is.True);
            foreach (var pair in before) Assert.That(city.Layout.Cells[pair.Key].Pipelines, Is.EqualTo(pair.Value));
            AssertNear(city.Population.Current, 100);
            Assert.That(city.TryBuildHouse(spot, out _), Is.False);
            Assert.That(city.TryBuildHouse(new Vector2Int(99, 99), out _), Is.False);
        }
        [Test] public void FractionalConsumptionIsAtomicFishFirstThenLegacyFoodAndNeverNegative()
        {
            var storage = new ResourceState(startingFish: 1, startingFood: 2, startingWater: 1);
            int notifications = 0;
            storage.Changed += () => { notifications++; AssertNear(storage.FoodAvailable, 1.75); AssertNear(storage.GetExact(ResourceKind.Water), 0.75); };
            storage.ConsumeFoodAndWater(1.25, 0.25);
            Assert.That(notifications, Is.EqualTo(1)); AssertNear(storage.GetExact(ResourceKind.Fish), 0); AssertNear(storage.GetExact(ResourceKind.Food), 1.75);
            var depleted = new ResourceState(startingFish: 1, startingFood: 2, startingWater: 1);
            depleted.ConsumeFoodAndWater(500, 500); AssertNear(depleted.FoodAvailable, 0); AssertNear(depleted.GetExact(ResourceKind.Water), 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => depleted.ConsumeFoodAndWater(double.NaN, 1));
        }
        [Test] public void DemandUsesActualPopulationAndNeverTruncatesFractionPerFrame()
        {
            var city = Fresh(); city.Tick(0.05f);
            AssertNear(city.Storage.GetExact(ResourceKind.Fish), 99.75); AssertNear(city.Storage.GetExact(ResourceKind.Water), 99.75);
            AssertNear(city.Population.FoodDemand, 5); AssertNear(city.Population.WaterDemand, 5);
            city.Tick(59.95f); AssertNear(city.Population.Current, 110); AssertNear(city.Population.FoodDemand, 5.5);
        }
        [Test] public void ExhaustedFoodDisablesFDespiteOperationalHarborAndRestoresFromActualStock()
        {
            var city = Fresh(); city.Storage.ConsumeFoodAndWater(200, 0); city.Resolve();
            Assert.That(city.State(NetworkedCity.HarborPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.F));
            Assert.That(city.ActiveOutput(NetworkedCity.HarborPosition), Is.EqualTo(Pipeline.None));
            city.Storage.Add(ResourceKind.Fish, 1); city.Resolve();
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            city.Tick(0.05f); AssertNear(city.Storage.GetExact(ResourceKind.Fish), 0.75);
            Assert.That(city.Network.State(NetworkedCity.HousePosition, Pipeline.F), Is.EqualTo(PipelineState.Supplied));
        }
        [Test] public void NoHousingMeansNoConsumptionBacklogButGrowthContinues()
        {
            var city = Fresh(); Configure(city, NetworkedCity.WaterBridge, FME); city.Tick(120);
            AssertNear(city.Population.Current, 121); AssertNear(city.Storage.GetExact(ResourceKind.Fish), 100); AssertNear(city.Storage.GetExact(ResourceKind.Water), 100);
            Configure(city, NetworkedCity.WaterBridge, FWM); city.Tick(1);
            AssertNear(city.Storage.GetExact(ResourceKind.Fish), 93.95); AssertNear(city.Storage.GetExact(ResourceKind.Water), 93.95);
        }
        [Test] public void DepletedWaterDoesNotDisableProducerAndProductionRestoresReachableSupply()
        {
            var city = Fresh(); city.TryRepair(NetworkedCity.WaterPosition); city.Tick(5);
            city.Storage.ConsumeFoodAndWater(0, 500); city.Resolve();
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.W));
            city.Tick(2);
            Assert.That(city.Storage.GetExact(ResourceKind.Water), Is.GreaterThan(0));
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
        }
        [Test] public void StorageSupplyRequiresRealPathsAndMaterialIdentity()
        {
            var city = Fresh();
            foreach (var item in new[] { ResourceKind.Wood, ResourceKind.Iron, ResourceKind.RecyclableMaterial }) city.Storage.TryRemove(item, 100);
            city.Resolve(); Assert.That(city.ActiveOutput(NetworkedCity.TownHallPosition) & Pipeline.M, Is.EqualTo(Pipeline.None));
            city.Storage.Add(ResourceKind.Iron, 1); city.Resolve();
            Assert.That(city.ActiveOutput(NetworkedCity.TownHallPosition) & Pipeline.M, Is.EqualTo(Pipeline.M));
            AssertNear(city.Storage.GetExact(ResourceKind.Wood), 0); AssertNear(city.Storage.GetExact(ResourceKind.RecyclableMaterial), 0);
            Configure(city, NetworkedCity.WaterBridge, WME);
            Assert.That(city.MissingInputs(NetworkedCity.HousePosition), Is.EqualTo(Pipeline.F));
        }
        [Test] public void ProductionPartitionMatchesAndNoStateLeaksToFreshCity()
        {
            var one = Fresh(); var many = Fresh();
            foreach (var city in new[] { one, many }) { city.TryRepair(NetworkedCity.WaterPosition); city.TryRepair(NetworkedCity.RecyclerPosition); }
            one.Tick(120); for (int i = 0; i < 480; i++) many.Tick(0.25f);
            foreach (ResourceKind kind in Enum.GetValues(typeof(ResourceKind))) AssertNear(one.Storage.GetExact(kind), many.Storage.GetExact(kind));
            AssertNear(one.Population.Current, many.Population.Current);
            AssertNear(Fresh().Storage.GetExact(ResourceKind.Water), 100); Assert.That(Fresh().Population.EffectiveCapacity, Is.EqualTo(100));
        }
        [Test] public void WaterDepositPathLossPausesProgressWithoutFreeCreditOrCatchUp()
        {
            var city = Fresh(); city.TryRepair(NetworkedCity.WaterPosition); city.Tick(5.5f);
            Configure(city, NetworkedCity.MaterialBridge, FME); Configure(city, NetworkedCity.WaterBridge, FME);
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.CanDeposit(NetworkedCity.WaterPosition, Pipeline.W), Is.False);
            double water = city.Storage.GetExact(ResourceKind.Water), progress = city.Water.ProductionProgress;
            city.Tick(10); AssertNear(city.Storage.GetExact(ResourceKind.Water), water); AssertNear(city.Water.ProductionProgress, progress);
            Configure(city, NetworkedCity.MaterialBridge, WME);
            city.Tick(1.5f); AssertNear(city.Storage.GetExact(ResourceKind.Water), water + 1);
        }
        [Test] public void RecyclerDepositPathLossPreservesConcreteInputUntilConnected()
        {
            var city = Fresh(); city.TryRepair(NetworkedCity.RecyclerPosition); city.Tick(6);
            Configure(city, NetworkedCity.ElectricBridge, FWE); Configure(city, NetworkedCity.MaterialBridge, FWE);
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.CanDeposit(NetworkedCity.RecyclerPosition, Pipeline.M), Is.False);
            double input = city.Storage.GetExact(ResourceKind.RecyclableMaterial), progress = city.Recycler.ProcessingProgress;
            city.Tick(10); AssertNear(city.Storage.GetExact(ResourceKind.RecyclableMaterial), input); AssertNear(city.Recycler.ProcessingProgress, progress);
            Configure(city, NetworkedCity.MaterialBridge, WME); city.Tick(3);
            AssertNear(city.Storage.GetExact(ResourceKind.RecyclableMaterial), input - 2);
        }
        [Test] public void BoatDepositRoutePausesIndependentBoatsAndConstructionThenCreditsOnce()
        {
            var city = Fresh(); var root = new GameObject("Phase3 boat deposit test"); root.SetActive(false);
            try
            {
                var harbor = root.AddComponent<FishingHarbor>();
                GameObject Child(string name) { var go = new GameObject(name); go.transform.SetParent(root.transform); return go; }
                harbor.BindNetwork(city.Storage, Child("Boat template"), Child("Marker"), Child("Construction"));
                typeof(FishingHarbor).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(harbor, null);
                harbor.Select(true); city.BindHarbor(harbor);
                Assert.That(harbor.TryBuildBoat(), Is.True); city.Tick(5);
                Assert.That(harbor.TryBuildBoat(), Is.True); city.Tick(2);
                double elapsed = harbor.Boats[0].Trip.CycleElapsed;
                Configure(city, new Vector2Int(-1, -1), WME);
                Assert.That(city.State(NetworkedCity.HarborPosition), Is.EqualTo(NetworkBuildingState.Operational));
                Assert.That(harbor.NetworkAvailable, Is.False);
                city.Tick(10); AssertNear(harbor.Boats[0].Trip.CycleElapsed, elapsed); Assert.That(harbor.Construction.Elapsed, Is.EqualTo(2).Within(1e-5));
                Configure(city, new Vector2Int(-1, -1), FWM); city.Tick(3);
                Assert.That(harbor.BoatCount, Is.EqualTo(2));
                // Disable housing only, so exact concrete arrival credits can be inspected without consumption.
                Configure(city, NetworkedCity.WaterBridge, FME);
                double fish = city.Storage.GetExact(ResourceKind.Fish);
                city.Tick(30); AssertNear(city.Storage.GetExact(ResourceKind.Fish), fish + 10);
                Assert.That(harbor.Boats.All(b => b.UnloadedTrips == 1), Is.True);
                city.Tick(0); AssertNear(city.Storage.GetExact(ResourceKind.Fish), fish + 10);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }
        [Test] public void PopulationAndFractionalStockRejectInvalidValuesWithoutMutation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CityPopulation(new PopulationSettings { growthInterval = 0 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CityPopulation(new PopulationSettings { startingPopulation = double.NaN }));
            var city = Fresh();
            Assert.Throws<ArgumentOutOfRangeException>(() => city.Tick(float.PositiveInfinity));
            Assert.Throws<ArgumentOutOfRangeException>(() => city.Population.Tick(-1));
            AssertNear(city.Population.Current, 100); AssertNear(city.Storage.GetExact(ResourceKind.Fish), 100);
        }
    }
}
