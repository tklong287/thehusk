using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class IntegratedPlanningTests
    {
        private const Pipeline FWM = Pipeline.F | Pipeline.W | Pipeline.M;
        private const Pipeline FME = Pipeline.F | Pipeline.M | Pipeline.E;
        private const Pipeline FWE = Pipeline.F | Pipeline.W | Pipeline.E;
        private static NetworkedCity Fresh() => new(NetworkedCity.CreateLayout(cityHallStorage: true),
            new NetworkProductionSettings { waterPerCycle = 20 }, new PopulationSettings());
        private static void Configure(NetworkedCity city, Vector2Int position, Pipeline lanes)
        { Assert.That(city.Layout.TryConfigure(position, lanes, out _), Is.True); city.Resolve(); }

        [Test] public void CoastExpansionAndHousingPayOnlyModuleCostAndReservePort()
        {
            var city = Fresh(); var branch = new Vector2Int(2, -2);
            Assert.That(city.Layout.TryBuild(branch, FWM, out _), Is.True);
            Assert.That(city.TryBuildHouse(branch, out _), Is.True);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200));
            Assert.That(city.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(99));
            Assert.That(city.Storage.Get(ResourceKind.Wood), Is.EqualTo(90));
            Assert.That(city.Layout.TryBuild(branch, FWM, out _), Is.False);
            Assert.That(city.Layout.TryBuild(new Vector2Int(0, -2), FWM, out _), Is.False);
            Assert.That(city.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(99));
            Assert.That(city.Network.State(branch, Pipeline.M), Is.EqualTo(PipelineState.Supplied));
        }
        [Test] public void AlternateWaterRouteKeepsOnlyReachableHousesOperational()
        {
            var city = Fresh();
            Assert.That(city.TryBuildHouse(new Vector2Int(-1, -1), out _), Is.True);
            Configure(city, NetworkedCity.WaterBridge, FME);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            Assert.That(city.Network.State(NetworkedCity.HousePosition, Pipeline.F), Is.EqualTo(PipelineState.Supplied));
            Assert.That(city.Layout.TryBuild(new Vector2Int(2, 0), FWM, out _), Is.True);
            city.Resolve();
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200));
            Assert.That(city.Network.State(NetworkedCity.HousePosition, Pipeline.W), Is.EqualTo(PipelineState.Supplied));
            Configure(city, new Vector2Int(2, 0), FME);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            Configure(city, NetworkedCity.WaterBridge, FWM);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200));
        }
        [Test] public void RepairWithoutElectricWaitsThenRealSourceRestorationProduces()
        {
            var city = Fresh(); Configure(city, NetworkedCity.ElectricBridge, FWM);
            Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.True);
            Assert.That(city.TryRepair(NetworkedCity.RecyclerPosition), Is.True);
            city.Tick(5);
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Disabled));
            Assert.That(city.Water.ProducedCycles, Is.Zero);
            // Stored W remains legitimate supply when its independent producer loses E.
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            Configure(city, NetworkedCity.ElectricBridge, FME); city.Tick(4);
            Assert.That(city.Water.ProducedCycles, Is.EqualTo(2));
            Assert.That(city.Recycler.ProcessedCycles, Is.EqualTo(1));
            Assert.That(city.Storage.GetExact(ResourceKind.Water), Is.EqualTo(95).Within(1e-5));
            Assert.That(city.Storage.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(98));
        }
        [Test] public void AlternateElectricProducerKeepsRepairAndDownstreamProductionWorking()
        {
            var city = Fresh();
            Assert.That(city.Layout.TryBuild(new Vector2Int(1, 1), FWE, out _), Is.True);
            Assert.That(city.Layout.TryOccupy(new Vector2Int(1, 1), ModuleBuilding.Solar, out _), Is.True);
            Configure(city, NetworkedCity.ElectricBridge, FWM);
            city.TryRepair(NetworkedCity.WaterPosition); city.TryRepair(NetworkedCity.RecyclerPosition); city.Tick(9);
            Assert.That(city.State(NetworkedCity.WaterPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.State(NetworkedCity.RecyclerPosition), Is.EqualTo(NetworkBuildingState.Operational));
            Assert.That(city.Water.ProducedCycles, Is.EqualTo(2));
        }
        [Test] public void ExhaustedHousingCanRepairAndRestoreWithoutResetOrInjectedStock()
        {
            var city = Fresh(); city.Tick(25);
            Assert.That(city.Population.EffectiveCapacity, Is.Zero);
            Assert.That(city.Storage.GetExact(ResourceKind.Water), Is.Zero);
            Assert.That(city.TryRepair(NetworkedCity.WaterPosition), Is.True); city.Tick(7);
            Assert.That(city.Water.ProducedCycles, Is.EqualTo(1));
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(100));
            Assert.That(city.Storage.GetExact(ResourceKind.Water), Is.EqualTo(20).Within(1e-5));
            Assert.That(city.Layout.TryBuild(new Vector2Int(2, -2), FWM, out _), Is.True);
            Assert.That(city.TryBuildHouse(new Vector2Int(2, -2), out _), Is.True);
            Assert.That(city.Population.EffectiveCapacity, Is.EqualTo(200));
        }
    }
}
