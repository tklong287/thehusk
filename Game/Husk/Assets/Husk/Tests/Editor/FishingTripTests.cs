using System;
using NUnit.Framework;

namespace Husk.Tests
{
    public sealed class FishingTripTests
    {
        private static FishingTrip DefaultTrip() => new FishingTrip(30, 6, 16, 6, 2);

        [Test]
        public void CompleteCycleIncludesTravelFishingAndHarbor()
        {
            var trip = DefaultTrip();
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Depart));
            trip.Tick(3);
            Assert.That(trip.StageProgress, Is.EqualTo(0.5f));
            trip.Tick(3);
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Fishing));
            trip.Tick(16);
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Return));
            trip.Tick(6);
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Harbor));
            Assert.That(trip.CompletedCycles, Is.Zero);
            trip.Tick(2);
            Assert.That(trip.CompletedCycles, Is.EqualTo(1));
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Depart));
            Assert.That(trip.CycleElapsed, Is.Zero);
        }

        [Test]
        public void OvershootCarriesAcrossMultipleCyclesWithoutRedispatch()
        {
            var trip = DefaultTrip();
            trip.Tick(91);
            Assert.That(trip.CompletedCycles, Is.EqualTo(3));
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Depart));
            Assert.That(trip.CycleElapsed, Is.EqualTo(1));
            trip.Tick(29);
            Assert.That(trip.CompletedCycles, Is.EqualTo(4));
        }

        [Test]
        public void ConfiguredTotalScalesEveryStageAndFreshTripResets()
        {
            var trip = new FishingTrip(12, 1, 1, 1, 1);
            trip.Tick(9);
            Assert.That(trip.Stage, Is.EqualTo(FishingTripStage.Harbor));
            trip.Tick(3);
            Assert.That(trip.CompletedCycles, Is.EqualTo(1));
            var fresh = new FishingTrip(12, 1, 1, 1, 1);
            Assert.That(fresh.CompletedCycles, Is.Zero);
            Assert.That(fresh.CycleElapsed, Is.Zero);
        }

        [Test]
        public void FramePartitionDoesNotChangeCycleOrStage()
        {
            var large = DefaultTrip();
            var small = DefaultTrip();
            large.Tick(95.5f);
            for (int i = 0; i < 382; i++) small.Tick(0.25f);
            Assert.That(small.CompletedCycles, Is.EqualTo(large.CompletedCycles));
            Assert.That(small.CycleElapsed, Is.EqualTo(large.CycleElapsed));
            Assert.That(small.StageProgress, Is.EqualTo(large.StageProgress));
        }

        [Test]
        public void InvalidTimingIsRejectedWithoutStateMutation()
        {
            foreach (float value in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new FishingTrip(value, 6, 16, 6, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => new FishingTrip(30, value, 16, 6, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => new FishingTrip(30, 6, value, 6, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => new FishingTrip(30, 6, 16, value, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() => new FishingTrip(30, 6, 16, 6, value));
            }
            var trip = DefaultTrip();
            foreach (float value in new[] { -1f, float.NaN, float.PositiveInfinity })
                Assert.Throws<ArgumentOutOfRangeException>(() => trip.Tick(value));
            Assert.That(trip.CycleElapsed, Is.Zero);
        }
        [Test]
        public void ArrivalsCountReturnBoundaryEvenWhenTickSkipsHarborStage()
        {
            var trip = DefaultTrip();
            trip.Tick(27.5f);
            Assert.That(trip.Arrivals, Is.Zero);
            trip.Tick(0.5f);
            Assert.That(trip.Arrivals, Is.EqualTo(1));
            trip.Tick(2);
            Assert.That(trip.Arrivals, Is.EqualTo(1));
            trip.Tick(91);
            Assert.That(trip.Arrivals, Is.EqualTo(4));
            trip.Tick(0);
            Assert.That(trip.Arrivals, Is.EqualTo(4));
        }
    }
}
