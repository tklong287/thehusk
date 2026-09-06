using System;
using NUnit.Framework;

namespace Husk.Tests
{
    public sealed class ResourceStateTests
    {
        [Test]
        public void FreshStateDefaultsEveryResourceTo100()
        {
            var state = new ResourceState();
            foreach (ResourceKind kind in Enum.GetValues(typeof(ResourceKind)))
                Assert.That(state.Get(kind), Is.EqualTo(100), kind.ToString());
        }

        [Test]
        public void EveryStartingResourceIsConfigurable()
        {
            var state = new ResourceState(2, 9, 17, 23, 41, 57, 83);
            Assert.That(state.Get(ResourceKind.Food), Is.EqualTo(17));
            Assert.That(state.Get(ResourceKind.Water), Is.EqualTo(23));
            Assert.That(state.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(41));
            Assert.That(state.Get(ResourceKind.Wood), Is.EqualTo(2));
            Assert.That(state.Get(ResourceKind.Iron), Is.EqualTo(9));
            Assert.That(state.Get(ResourceKind.Fish), Is.EqualTo(57));
            Assert.That(state.Get(ResourceKind.ModuleCore), Is.EqualTo(83));
        }

        [Test]
        public void AddAndRemoveUpdateOnlyTargetResourceAndNotifyPresentation()
        {
            var state = new ResourceState(0, 5);
            int notifications = 0;
            state.Changed += () => notifications++;
            state.Add(ResourceKind.Wood, 3);
            Assert.That(state.TryRemove(ResourceKind.Wood, 2), Is.True);
            Assert.That(state.Get(ResourceKind.Wood), Is.EqualTo(1));
            Assert.That(state.Get(ResourceKind.Iron), Is.EqualTo(5));
            Assert.That(notifications, Is.EqualTo(2));
        }

        [Test]
        public void InsufficientAndZeroDebitsDoNotMutateOrNotify()
        {
            var state = new ResourceState(0, 5);
            int notifications = 0;
            state.Changed += () => notifications++;
            Assert.That(state.TryRemove(ResourceKind.Water, 101), Is.False);
            Assert.That(state.TryRemove(ResourceKind.Water, 0), Is.True);
            state.Add(ResourceKind.Water, 0);
            Assert.That(state.Get(ResourceKind.Water), Is.EqualTo(100));
            Assert.That(notifications, Is.Zero);
            Assert.That(state.TryRemove(ResourceKind.Water, 100), Is.True);
            Assert.That(state.Get(ResourceKind.Water), Is.Zero);
        }

        [Test]
        public void InvalidAmountsAndOverflowCannotCorruptStock()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(-1, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(startingFood: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(startingWater: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(startingRecyclableMaterial: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(startingFish: -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(startingModuleCore: -1));
            var state = new ResourceState(int.MaxValue, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => state.Add(ResourceKind.Wood, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => state.TryRemove(ResourceKind.Wood, -1));
            Assert.Throws<OverflowException>(() => state.Add(ResourceKind.Wood, 1));
            Assert.That(state.Get(ResourceKind.Wood), Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void UnknownResourceIsRejectedByEveryOperation()
        {
            var state = new ResourceState(0, 5);
            foreach (var kind in new[] { (ResourceKind)(-1), (ResourceKind)7 })
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => state.Get(kind));
                Assert.Throws<ArgumentOutOfRangeException>(() => state.Add(kind, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => state.TryRemove(kind, 1));
            }
        }

        [Test]
        public void NewRunDoesNotInheritPreviousState()
        {
            var first = new ResourceState();
            first.TryRemove(ResourceKind.Water, 100);
            first.Add(ResourceKind.Wood, 8);
            var second = new ResourceState();
            Assert.That(second.Get(ResourceKind.Water), Is.EqualTo(100));
            Assert.That(second.Get(ResourceKind.Wood), Is.EqualTo(100));
        }

        [Test]
        public void StorageHasNoGameplayCapacityLimit()
        {
            var state = new ResourceState();
            foreach (ResourceKind kind in Enum.GetValues(typeof(ResourceKind)))
            {
                state.Add(kind, 1000000);
                Assert.That(state.Get(kind), Is.EqualTo(1000100));
                Assert.That(state.TryRemove(kind, 1000000), Is.True);
                Assert.That(state.Get(kind), Is.EqualTo(100));
            }
        }
    }
}
