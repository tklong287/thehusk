using System;
using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class ModuleNetworkTests
    {
        private const Pipeline FWM = Pipeline.F | Pipeline.W | Pipeline.M;
        private const Pipeline FME = Pipeline.F | Pipeline.M | Pipeline.E;
        private static ModuleLayout Line()
        {
            var layout = new ModuleLayout(new ResourceState());
            layout.AddInitial(Vector2Int.zero, FWM);
            layout.AddInitial(Vector2Int.right, FWM);
            layout.AddInitial(Vector2Int.right * 2, FWM);
            layout.SetTestSupply(Vector2Int.zero, Pipeline.F | Pipeline.W);
            return layout;
        }
        [Test]
        public void OnlyFourExactThreeBitConfigurationsAreStandard()
        {
            int valid = 0;
            for (int bits = 0; bits < 32; bits++)
            {
                bool expected = bits == 7 || bits == 11 || bits == 13 || bits == 14;
                Assert.That(BuildingPipelines.IsStandard((Pipeline)bits), Is.EqualTo(expected));
                if (expected) valid++;
            }
            Assert.That(valid, Is.EqualTo(4));
            Assert.That(BuildingPipelines.IsStandard((Pipeline)(-1)), Is.False);
        }
        [TestCase(ModuleBuilding.FishingHarbor, Pipeline.M, Pipeline.F)]
        [TestCase(ModuleBuilding.WaterPlant, Pipeline.E | Pipeline.M, Pipeline.W)]
        [TestCase(ModuleBuilding.Recycler, Pipeline.E, Pipeline.M)]
        [TestCase(ModuleBuilding.Solar, Pipeline.None, Pipeline.E)]
        [TestCase(ModuleBuilding.House, Pipeline.F | Pipeline.W, Pipeline.None)]
        public void BuildingRequiresBothInputsAndOutputs(ModuleBuilding building, Pipeline input, Pipeline output)
        {
            Assert.That(BuildingPipelines.Inputs(building), Is.EqualTo(input));
            Assert.That(BuildingPipelines.Outputs(building), Is.EqualTo(output));
            foreach (var excluded in ModuleNetwork.Types)
            {
                var support = Pipeline.All & ~excluded;
                Assert.That(BuildingPipelines.Allows(building, support), Is.EqualTo(((input | output) & excluded) == 0));
            }
        }
        [Test]
        public void BuildAtomicallyPublishesStockAndNewIdentityWithoutMaterialSupply()
        {
            var layout = new ModuleLayout(new ResourceState());
            layout.AddInitial(Vector2Int.zero, Pipeline.F | Pipeline.W | Pipeline.E);
            int observed = 0;
            layout.Storage.Changed += () =>
            {
                observed++;
                Assert.That(layout.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(99));
                Assert.That(layout.Storage.Get(ResourceKind.Wood), Is.EqualTo(90));
                Assert.That(layout.Cells.ContainsKey(Vector2Int.right), Is.True);
                Assert.That(layout.TryBuild(Vector2Int.right, FWM, out _), Is.False);
            };
            Assert.That(layout.TryBuild(Vector2Int.right, FWM, out _), Is.True);
            Assert.That(observed, Is.EqualTo(1));
            Assert.That(layout.Storage.Get(ResourceKind.Iron), Is.EqualTo(100));
        }
        [TestCase(0, 100)]
        [TestCase(100, 9)]
        [TestCase(0, 0)]
        public void InsufficientEitherItemCannotPartiallyDebitOrBuild(int cores, int wood)
        {
            var storage = new ResourceState(startingModuleCore: cores, startingWood: wood);
            var layout = new ModuleLayout(storage); layout.AddInitial(Vector2Int.zero, FWM);
            int notifications = 0; storage.Changed += () => notifications++;
            Assert.That(layout.TryBuild(Vector2Int.right, FWM, out string reason), Is.False);
            Assert.That(reason, Is.Not.Empty);
            Assert.That(storage.Get(ResourceKind.ModuleCore), Is.EqualTo(cores));
            Assert.That(storage.Get(ResourceKind.Wood), Is.EqualTo(wood));
            Assert.That(layout.Cells.Count, Is.EqualTo(1)); Assert.That(notifications, Is.Zero);
        }
        [Test]
        public void RejectsOccupiedDiagonalRemoteAndPortFromAnyApproach()
        {
            var layout = new ModuleLayout(new ResourceState()); layout.AddInitial(Vector2Int.zero, FWM);
            layout.ReservePort(Vector2Int.zero, Vector2Int.down);
            layout.AddInitial(new Vector2Int(1, -1), FWM);
            foreach (var target in new[] { Vector2Int.zero, new Vector2Int(-1, 1), new Vector2Int(20, 20), Vector2Int.down })
                Assert.That(layout.TryBuild(target, FWM, out _), Is.False, target.ToString());
            Assert.That(layout.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(100));
            Assert.That(layout.Storage.Get(ResourceKind.Wood), Is.EqualTo(100));
            Assert.That(layout.Cells[Vector2Int.zero].Pipelines, Is.EqualTo(FWM));
        }
        [Test]
        public void InvalidConfigurationCannotSpendAndDuplicateInitialThrows()
        {
            var layout = Line();
            Assert.That(layout.TryBuild(Vector2Int.up, Pipeline.All, out _), Is.False);
            Assert.Throws<ArgumentException>(() => layout.AddInitial(Vector2Int.zero, FME));
            Assert.That(layout.Storage.Get(ResourceKind.Wood), Is.EqualTo(100));
        }
        [Test]
        public void SharedAdjacentCategoriesConnectAndDoNotJumpOrConvert()
        {
            var layout = Line(); var network = new ModuleNetwork(layout);
            Assert.That(layout.AdjacentConnection(Vector2Int.zero, Vector2Int.right, Pipeline.W), Is.True);
            Assert.That(layout.AdjacentConnection(Vector2Int.zero, Vector2Int.right * 2, Pipeline.W), Is.False);
            Assert.That(network.Connected(Vector2Int.zero, Vector2Int.right * 2, Pipeline.W), Is.True);
            Assert.That(network.State(Vector2Int.right, Pipeline.M), Is.EqualTo(PipelineState.Unsupplied));
            Assert.That(network.State(Vector2Int.right, Pipeline.E), Is.EqualTo(PipelineState.Unsupported));
            Assert.That(network.Component(Vector2Int.zero, Pipeline.All), Is.EqualTo(-1));
        }
        [Test]
        public void UnsupportedIntermediateBreaksOnlyItsCategoryAndRestoreRecovers()
        {
            var layout = Line(); var network = new ModuleNetwork(layout);
            Assert.That(layout.TryConfigure(Vector2Int.right, FME, out _), Is.True);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.W), Is.EqualTo(PipelineState.Unsupplied));
            Assert.That(network.Connected(Vector2Int.zero, Vector2Int.right * 2, Pipeline.W), Is.False);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.F), Is.EqualTo(PipelineState.Supplied));
            layout.TryConfigure(Vector2Int.right, FWM, out _);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.W), Is.EqualTo(PipelineState.Supplied));
        }
        [Test]
        public void SourceOffKeepsConnectivityButNotSupplyAndAnotherSourceKeepsComponentLit()
        {
            var layout = Line(); var network = new ModuleNetwork(layout);
            layout.SetTestSupply(Vector2Int.zero, Pipeline.None);
            Assert.That(network.Connected(Vector2Int.zero, Vector2Int.right * 2, Pipeline.W), Is.True);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.W), Is.EqualTo(PipelineState.Unsupplied));
            layout.SetTestSupply(Vector2Int.right * 2, Pipeline.W);
            Assert.That(network.State(Vector2Int.zero, Pipeline.W), Is.EqualTo(PipelineState.Supplied));
            layout.TryConfigure(Vector2Int.right, FME, out _);
            Assert.That(network.State(Vector2Int.zero, Pipeline.W), Is.EqualTo(PipelineState.Unsupplied));
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.W), Is.EqualTo(PipelineState.Supplied));
        }
        [Test]
        public void AlternateRouteKeepsDownstreamSupplyWithoutDoubleCountingStorage()
        {
            var layout = Line(); var network = new ModuleNetwork(layout);
            for (int x = 0; x < 3; x++) layout.AddInitial(new Vector2Int(x, 1), FWM);
            layout.TryConfigure(Vector2Int.right, FME, out _);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.W), Is.EqualTo(PipelineState.Supplied));
            Assert.That(layout.Storage.Get(ResourceKind.Water), Is.EqualTo(100));
        }
        [Test]
        public void BuildingOccupancyEnforcesUnionAndNeverBlocksOptionalPassThrough()
        {
            var layout = Line(); var network = new ModuleNetwork(layout);
            Assert.That(layout.TryOccupy(Vector2Int.right, ModuleBuilding.House, out _), Is.True);
            Assert.That(layout.TryConfigure(Vector2Int.right, FME, out _), Is.False);
            Assert.That(layout.TryOccupy(Vector2Int.right, ModuleBuilding.Solar, out _), Is.False);
            layout.SetTestSupply(Vector2Int.zero, Pipeline.M);
            Assert.That(network.State(Vector2Int.right * 2, Pipeline.M), Is.EqualTo(PipelineState.Supplied));
            Assert.That(layout.Cells[Vector2Int.right].Building, Is.EqualTo(ModuleBuilding.House));
            Assert.That(layout.TryOccupy(Vector2Int.right * 2, ModuleBuilding.WaterPlant, out _), Is.True);
            Assert.That(layout.Cells[Vector2Int.right * 2].Pipelines, Is.EqualTo(Pipeline.W | Pipeline.M | Pipeline.E));
        }
        [Test]
        public void TownHallHasNoInventedIoOrSupplyAndFixtureCannotEmitUnsupportedCategory()
        {
            var layout = ModuleNetworkPrototype.CreateFresh(); var network = new ModuleNetwork(layout);
            var townHall = layout.Cells[new Vector2Int(-1, 0)];
            Assert.That(townHall.TestSupply, Is.EqualTo(Pipeline.None));
            Assert.That(BuildingPipelines.Inputs(ModuleBuilding.TownHall) | BuildingPipelines.Outputs(ModuleBuilding.TownHall), Is.EqualTo(Pipeline.None));
            layout.SetTestSupply(Vector2Int.zero, Pipeline.E);
            Assert.That(network.State(new Vector2Int(0, -1), Pipeline.E), Is.EqualTo(PipelineState.Unsupplied));
        }
        [Test]
        public void FreshPresetRecreatesIdentityConfigurationFixtureAndStockDeterministically()
        {
            var first = ModuleNetworkPrototype.CreateFresh(120);
            first.TryBuild(Vector2Int.right, FME, out _); first.TryConfigure(Vector2Int.zero, FME, out _);
            first.SetTestSupply(Vector2Int.zero, Pipeline.None);
            var second = ModuleNetworkPrototype.CreateFresh(120);
            Assert.That(second.Cells.Count, Is.EqualTo(3));
            Assert.That(second.Storage.Get(ResourceKind.ModuleCore), Is.EqualTo(100));
            Assert.That(second.Storage.Get(ResourceKind.Wood), Is.EqualTo(120));
            Assert.That(second.Cells[Vector2Int.zero].Pipelines, Is.EqualTo(FWM));
            Assert.That(second.Cells[Vector2Int.zero].TestSupply, Is.EqualTo(Pipeline.F | Pipeline.W));
            Assert.That(second.IsPortReserved(new Vector2Int(0, -2)), Is.True);
        }
    }
}
