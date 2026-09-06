using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Husk.Tests
{
    public sealed class FishingHarborTests
    {
        private GameObject root;
        private FishingHarbor harbor;
        private PrototypeSession session;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Test Harbor");
            root.SetActive(false);
            harbor = root.AddComponent<FishingHarbor>();
            session = root.AddComponent<PrototypeSession>();
            SetReference("session", session);
            SetReference("selectionMarker", Child("Selection"));
            SetReference("constructionVisual", Child("Construction"));
            var template = Child("Boat Template");
            template.transform.localPosition = new Vector3(3.4f, -0.48f, -3.6f);
            SetReference("fishingBoat", template);
            typeof(FishingHarbor).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(harbor, null);
            harbor.Select(true);
        }

        private GameObject Child(string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(root.transform);
            return child;
        }

        private void SetReference(string name, Object value)
        {
            typeof(FishingHarbor).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(harbor, value);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        [Test]
        public void CompletionCreatesExactlyOneInstanceAndBuildCanRepeat()
        {
            Assert.That(harbor.TryBuildBoat(), Is.True);
            Assert.That(harbor.TryBuildBoat(), Is.False);
            harbor.AdvanceSimulation(4);
            Assert.That(harbor.BoatCount, Is.Zero);
            harbor.AdvanceSimulation(1);
            Assert.That(harbor.BoatCount, Is.EqualTo(1));
            Assert.That(harbor.BoatTemplate.activeSelf, Is.False);
            Assert.That(harbor.Boats[0].Visual.activeSelf, Is.True);
            harbor.AdvanceSimulation(20);
            Assert.That(harbor.BoatCount, Is.EqualTo(1));
            Assert.That(harbor.TryBuildBoat(), Is.True);
            harbor.AdvanceSimulation(5);
            Assert.That(harbor.BoatCount, Is.EqualTo(2));
            Assert.That(harbor.Boats[0].Visual, Is.Not.SameAs(harbor.Boats[1].Visual));
        }

        [Test]
        public void StaggeredBoatsKeepIndependentClocksDuringLaterConstruction()
        {
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(7);
            var first = harbor.Boats[0];
            Assert.That(first.Trip.CycleElapsed, Is.EqualTo(2));
            Assert.That(harbor.TryBuildBoat(), Is.True);
            Assert.That(first.Trip.CycleElapsed, Is.EqualTo(2));
            harbor.AdvanceSimulation(7);
            var second = harbor.Boats[1];
            Assert.That(first.Trip, Is.Not.SameAs(second.Trip));
            Assert.That(first.Trip.CycleElapsed, Is.EqualTo(9));
            Assert.That(second.Trip.CycleElapsed, Is.EqualTo(2));
            Assert.That(first.Trip.Stage, Is.EqualTo(FishingTripStage.Fishing));
            Assert.That(second.Trip.Stage, Is.EqualTo(FishingTripStage.Depart));
            Assert.That(first.Destination, Is.Not.EqualTo(second.Destination));
            harbor.AdvanceSimulation(23);
            Assert.That(first.Trip.CompletedCycles, Is.EqualTo(1));
            Assert.That(second.Trip.CompletedCycles, Is.Zero);
        }

        [Test]
        public void ManySequentialBuildsHaveNoBoatCapOrRepeatedCompletion()
        {
            for (int i = 0; i < 12; i++)
            {
                Assert.That(harbor.TryBuildBoat(), Is.True);
                harbor.AdvanceSimulation(5);
                Assert.That(harbor.BoatCount, Is.EqualTo(i + 1));
                harbor.AdvanceSimulation(0);
                Assert.That(harbor.BoatCount, Is.EqualTo(i + 1));
            }
            Assert.That(harbor.Boats[0].Trip.CompletedCycles, Is.GreaterThan(0));
        }

        [Test]
        public void FreshHarborDoesNotKeepBoatsFromPreviousRun()
        {
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(10);
            TearDown();
            SetUp();
            Assert.That(harbor.BoatCount, Is.Zero);
            Assert.That(harbor.Construction.IsBuilding, Is.False);
            Assert.That(harbor.BoatTemplate.activeSelf, Is.False);
            Assert.That(harbor.TryBuildBoat(), Is.True);
        }
        [Test]
        public void FishCreditsAtArrivalNotBuildAndNeverTwiceAtSameArrival()
        {
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(5);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(100));
            harbor.AdvanceSimulation(27.5f);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(100));
            harbor.AdvanceSimulation(0.5f);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(105));
            Assert.That(harbor.Boats[0].Trip.Stage, Is.EqualTo(FishingTripStage.Harbor));
            Assert.That(harbor.Boats[0].UnloadedTrips, Is.EqualTo(1));
            Assert.That(harbor.LastDelivery, Does.Contain("+5 Fish"));
            harbor.AdvanceSimulation(0);
            harbor.AdvanceSimulation(2);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(105));
            harbor.AdvanceSimulation(28);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(110));
            Assert.That(session.Resources.Get(ResourceKind.Food), Is.EqualTo(100));
        }

        [Test]
        public void CargoConfigurationAndOvershootCreditEveryArrivalIncludingNewBoatRemainder()
        {
            typeof(FishingHarbor).GetField("fishPerTrip", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(harbor, 7);
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(94); // build 5, then trip 89: arrivals at 28,58,88.
            Assert.That(harbor.Boats[0].UnloadedTrips, Is.EqualTo(3));
            Assert.That(harbor.Boats[0].CargoPerTrip, Is.EqualTo(7));
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(121));
            harbor.AdvanceSimulation(0);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(121));
        }

        [Test]
        public void StaggeredBoatDeliveriesAccumulateAndFreshRunResetsFish()
        {
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(5);
            harbor.TryBuildBoat();
            harbor.AdvanceSimulation(5);
            harbor.AdvanceSimulation(23); // first arrives, second has 23s.
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(105));
            harbor.AdvanceSimulation(5);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(110));
            harbor.AdvanceSimulation(60);
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(130));
            Assert.That(harbor.Boats[0].UnloadedTrips, Is.EqualTo(3));
            Assert.That(harbor.Boats[1].UnloadedTrips, Is.EqualTo(3));
            TearDown();
            SetUp();
            Assert.That(session.Resources.Get(ResourceKind.Fish), Is.EqualTo(100));
            Assert.That(session.Resources.Get(ResourceKind.Food), Is.EqualTo(100));
            Assert.That(harbor.TotalUnloads, Is.Zero);
        }
    }
}
