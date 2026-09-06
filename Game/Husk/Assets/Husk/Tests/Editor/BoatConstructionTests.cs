using System;
using NUnit.Framework;

namespace Husk.Tests
{
    public sealed class BoatConstructionTests
    {
        [Test]
        public void FreshBerthWaitsForBuildAction()
        {
            var build = new BoatConstruction(5f);
            build.Tick(20f);
            Assert.That(build.Progress, Is.Zero);
            Assert.That(build.IsBuilding, Is.False);
            Assert.That(build.IsComplete, Is.False);
        }

        [Test]
        public void TimedConstructionCompletesAtConfiguredDuration()
        {
            var build = new BoatConstruction(5f);
            Assert.That(build.TryStart(), Is.True);
            build.Tick(2f);
            Assert.That(build.Progress, Is.EqualTo(0.4f).Within(0.0001));
            Assert.That(build.IsComplete, Is.False);
            build.Tick(3f);
            Assert.That(build.IsComplete, Is.True);
            Assert.That(build.IsBuilding, Is.False);
            Assert.That(build.Progress, Is.EqualTo(1f));
        }

        [Test]
        public void DuplicateStartCannotResetOrRepeatTheSameConstructionJob()
        {
            var build = new BoatConstruction(2f);
            build.TryStart();
            build.Tick(1f);
            Assert.That(build.TryStart(), Is.False);
            Assert.That(build.Elapsed, Is.EqualTo(1f));
            build.Tick(8f);
            Assert.That(build.TryStart(), Is.False);
            build.Tick(8f);
            Assert.That(build.Elapsed, Is.EqualTo(2f));
        }

        [Test]
        public void NewRunStartsEmptyAfterPreviousBoatCompletion()
        {
            var first = new BoatConstruction(0.5f);
            first.TryStart();
            first.Tick(0.5f);
            var fresh = new BoatConstruction(0.5f);
            Assert.That(fresh.IsComplete, Is.False);
            Assert.That(fresh.TryStart(), Is.True);
        }

        [Test]
        public void InvalidTimingCannotCorruptProgress()
        {
            foreach (float value in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
                Assert.Throws<ArgumentOutOfRangeException>(() => new BoatConstruction(value));
            var build = new BoatConstruction(5f);
            build.TryStart();
            foreach (float value in new[] { -1f, float.NaN, float.PositiveInfinity })
                Assert.Throws<ArgumentOutOfRangeException>(() => build.Tick(value));
            Assert.That(build.Progress, Is.Zero);
        }
    }
}
