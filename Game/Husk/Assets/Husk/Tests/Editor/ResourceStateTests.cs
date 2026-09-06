using System;
using NUnit.Framework;

namespace Husk.Tests
{
    public sealed class ResourceStateTests
    {
        [Test]
        public void FreshStateUsesConfirmedResourcesAndConfiguredMaterials()
        {
            var state = new ResourceState(2, 9);
            Assert.That(state.Get(ResourceKind.Food), Is.Zero);
            Assert.That(state.Get(ResourceKind.Water), Is.EqualTo(5));
            Assert.That(state.Get(ResourceKind.RecyclableMaterial), Is.EqualTo(10));
            Assert.That(state.Get(ResourceKind.Wood), Is.EqualTo(2));
            Assert.That(state.Get(ResourceKind.Iron), Is.EqualTo(9));
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
            Assert.That(state.TryRemove(ResourceKind.Water, 6), Is.False);
            Assert.That(state.TryRemove(ResourceKind.Water, 0), Is.True);
            state.Add(ResourceKind.Water, 0);
            Assert.That(state.Get(ResourceKind.Water), Is.EqualTo(5));
            Assert.That(notifications, Is.Zero);
            Assert.That(state.TryRemove(ResourceKind.Water, 5), Is.True);
            Assert.That(state.Get(ResourceKind.Water), Is.Zero);
        }

        [Test]
        public void InvalidAmountsAndOverflowCannotCorruptStock()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(-1, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceState(0, -1));
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
            foreach (var kind in new[] { (ResourceKind)(-1), (ResourceKind)5 })
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => state.Get(kind));
                Assert.Throws<ArgumentOutOfRangeException>(() => state.Add(kind, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => state.TryRemove(kind, 1));
            }
        }

        [Test]
        public void NewRunDoesNotInheritPreviousState()
        {
            var first = new ResourceState(0, 5);
            first.TryRemove(ResourceKind.Water, 5);
            first.Add(ResourceKind.Wood, 8);
            var second = new ResourceState(0, 5);
            Assert.That(second.Get(ResourceKind.Water), Is.EqualTo(5));
            Assert.That(second.Get(ResourceKind.Wood), Is.Zero);
        }
    }
}
