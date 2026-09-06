using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class WaterProductionTests
    {
        [Test]
        public void IdleAndConstructionProduceNothingThenFirstIntervalCredits()
        {
            var model = new WaterProduction(5, 2, 1);
            Assert.That(model.Tick(100), Is.Zero);
            Assert.That(model.TryStart(), Is.True);
            Assert.That(model.Tick(4), Is.Zero);
            Assert.That(model.IsBuilding, Is.True);
            Assert.That(model.Tick(1), Is.Zero);
            Assert.That(model.IsOperational, Is.True);
            Assert.That(model.Tick(1.5f), Is.Zero);
            Assert.That(model.Tick(0.5f), Is.EqualTo(1));
            Assert.That(model.Tick(0), Is.Zero);
            Assert.That(model.Tick(2), Is.EqualTo(1));
        }

        [Test]
        public void OvershootConfigurationAndFreshStateAreDeterministic()
        {
            var model = new WaterProduction(3, 0.5f, 7);
            model.TryStart();
            Assert.That(model.Tick(5.25f), Is.EqualTo(28));
            Assert.That(model.ProducedCycles, Is.EqualTo(4));
            Assert.That(model.ProductionProgress, Is.EqualTo(0.5));
            Assert.That(model.TryStart(), Is.False);
            Assert.That(model.Tick(0.25f), Is.EqualTo(7));
            var fresh = new WaterProduction(3, 0.5f, 7);
            Assert.That(fresh.ProducedCycles, Is.Zero);
            Assert.That(fresh.HasStarted, Is.False);
        }

        [Test]
        public void RepeatedStartDoesNotRestartConstruction()
        {
            var model = new WaterProduction(5, 2, 1);
            model.TryStart();
            model.Tick(4);
            Assert.That(model.TryStart(), Is.False);
            model.Tick(1);
            Assert.That(model.IsOperational, Is.True);
        }

        [Test]
        public void InvalidValuesAreRejectedWithoutProgress()
        {
            foreach (float value in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new WaterProduction(value, 2, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => new WaterProduction(5, value, 1));
            }
            Assert.Throws<ArgumentOutOfRangeException>(() => new WaterProduction(5, 2, -1));
            var model = new WaterProduction(5, 2, 1);
            model.TryStart();
            foreach (float value in new[] { -1f, float.NaN, float.PositiveInfinity })
                Assert.Throws<ArgumentOutOfRangeException>(() => model.Tick(value));
            Assert.That(model.ConstructionProgress, Is.Zero);
        }

        [Test]
        public void SceneComponentCreditsOnlyWaterWithoutFishingOrRecycler()
        {
            var root = new GameObject("Water test");
            root.SetActive(false);
            try
            {
                var session = root.AddComponent<PrototypeSession>();
                var plant = root.AddComponent<WaterPlant>();
                void Set(string field, UnityEngine.Object value) => typeof(WaterPlant)
                    .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(plant, value);
                GameObject Child(string name)
                {
                    var child = new GameObject(name);
                    child.transform.SetParent(root.transform);
                    return child;
                }
                Set("session", session);
                Set("selectionMarker", Child("Marker"));
                Set("constructionVisual", Child("Frame"));
                var operational = Child("Operational");
                Set("operationalVisual", operational);
                typeof(WaterPlant).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(plant, null);
                Assert.That(plant.TryBuild(), Is.False);
                plant.Select(true);
                Assert.That(plant.TryBuild(), Is.True);
                plant.AdvanceSimulation(5);
                Assert.That(operational.activeSelf, Is.True);
                Assert.That(session.Resources.Get(ResourceKind.Water), Is.EqualTo(100));
                plant.AdvanceSimulation(6);
                Assert.That(session.Resources.Get(ResourceKind.Water), Is.EqualTo(103));
                Assert.That(plant.LastProducedWater, Is.EqualTo(3));
                Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(100));
                Assert.That(session.Resources.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(100));
                Assert.That(plant.TryBuild(), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }
    }
}
