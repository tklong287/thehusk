using System;
using NUnit.Framework;

namespace Husk.Tests
{
    public sealed class RecyclerProcessingTests
    {
        [Test]
        public void ExactBoundaryConsumesOnlyAfterConstructionAndFullInterval()
        {
            var stock = new ResourceState();
            var model = new RecyclerProcessing(5, 4, 2, 1);
            Assert.That(model.Tick(100, stock), Is.Zero);
            Assert.That(model.TryStart(), Is.True);
            Assert.That(model.TryStart(), Is.False);
            Assert.That(model.Tick(5, stock), Is.Zero);
            Assert.That(model.IsOperational, Is.True);
            Assert.That(model.Tick(3.5f, stock), Is.Zero);
            Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(100));
            Assert.That(model.Tick(0.5f, stock), Is.EqualTo(1));
            Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(98));
            Assert.That(stock.Get(ResourceKind.Wood), Is.EqualTo(101));
            Assert.That(model.Tick(0, stock), Is.Zero);
            Assert.That(model.Tick(4, stock), Is.EqualTo(1));
            Assert.That(stock.Get(ResourceKind.Water), Is.EqualTo(100));
            Assert.That(stock.Get(ResourceKind.Fish), Is.EqualTo(100));
        }

        [Test]
        public void OvershootCapsToInputAndWaitingDoesNotAccumulateBacklog()
        {
            var stock = new ResourceState(startingRecyclableMaterial: 5);
            var model = new RecyclerProcessing(5, 4, 2, 1);
            model.TryStart();
            Assert.That(model.Tick(100, stock), Is.EqualTo(2));
            Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(1));
            Assert.That(model.IsWaiting, Is.True);
            Assert.That(model.Tick(1000, stock), Is.Zero);
            stock.Add(ResourceKind.RecyclableMaterial, 1);
            Assert.That(model.Tick(3, stock), Is.Zero);
            Assert.That(model.Tick(1, stock), Is.EqualTo(1));
            Assert.That(stock.Get(ResourceKind.Wood), Is.EqualTo(103));
        }

        [Test]
        public void AtomicNotificationAndOverflowCannotPartiallyMutateResources()
        {
            var stock = new ResourceState();
            int events = 0;
            stock.Changed += () => { events++; Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(98)); Assert.That(stock.Get(ResourceKind.Wood), Is.EqualTo(101)); };
            Assert.That(stock.TryRecycle(2, 1), Is.True);
            Assert.That(events, Is.EqualTo(1));
            var full = new ResourceState(startingWood: int.MaxValue);
            int fullEvents = 0;
            full.Changed += () => fullEvents++;
            Assert.Throws<OverflowException>(() => full.TryRecycle(2, 1));
            Assert.That(full.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(100));
            Assert.That(fullEvents, Is.Zero);
            var empty = new ResourceState(startingRecyclableMaterial: 0);
            Assert.That(empty.TryRecycle(2, 1), Is.False);
            Assert.That(empty.Get(ResourceKind.Wood), Is.EqualTo(100));
        }

        [Test]
        public void ConfiguredConversionAndFreshRunDoNotDependOnOtherLoops()
        {
            var stock = new ResourceState();
            var model = new RecyclerProcessing(1, 0.5f, 3, 7);
            model.TryStart();
            Assert.That(model.Tick(2.25f, stock), Is.EqualTo(2));
            Assert.That(stock.Get(ResourceKind.Wood), Is.EqualTo(114));
            Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(94));
            Assert.That(model.ProcessingProgress, Is.EqualTo(0.5));
            var fresh = new RecyclerProcessing(1, 0.5f, 3, 7);
            Assert.That(fresh.ProcessedCycles, Is.Zero);
            Assert.That(fresh.HasStarted, Is.False);
        }

        [Test]
        public void InvalidValuesAndMultiplicationOverflowLeaveStockUntouched()
        {
            foreach(float value in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new RecyclerProcessing(value, 4, 2, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => new RecyclerProcessing(5, value, 2, 1));
            }
            Assert.Throws<ArgumentOutOfRangeException>(() => new RecyclerProcessing(5, 4, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RecyclerProcessing(5, 4, 2, -1));
            var stock = new ResourceState();
            var model = new RecyclerProcessing(1, 1, 2, int.MaxValue);
            model.TryStart();
            Assert.Throws<OverflowException>(() => model.Tick(3, stock));
            Assert.That(stock.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(100));
            Assert.That(stock.Get(ResourceKind.Wood), Is.EqualTo(100));
            Assert.That(model.ProcessedCycles, Is.Zero);
            Assert.Throws<ArgumentOutOfRangeException>(() => model.Tick(float.NaN, stock));
            Assert.Throws<ArgumentOutOfRangeException>(() => stock.TryRecycle(0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stock.TryRecycle(1, -1));
        }
    }
}
