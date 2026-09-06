using System;

namespace Husk
{
    // One construction job; Harbor creates another job for the next boat. Durations are provisional.
    public sealed class BoatConstruction
    {
        public float Duration { get; }
        public float Elapsed { get; private set; }
        public bool IsBuilding { get; private set; }
        public bool IsComplete { get; private set; }
        public float Progress => IsComplete ? 1f : Elapsed / Duration;

        public BoatConstruction(float duration)
        {
            if (float.IsNaN(duration) || float.IsInfinity(duration) || duration <= 0f)
                throw new ArgumentOutOfRangeException(nameof(duration));
            Duration = duration;
        }

        public bool TryStart()
        {
            if (IsBuilding || IsComplete) return false;
            IsBuilding = true;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (!IsBuilding) return;
            Elapsed = Math.Min(Duration, Elapsed + deltaTime);
            if (Elapsed < Duration) return;
            IsBuilding = false;
            IsComplete = true;
        }
    }
}
