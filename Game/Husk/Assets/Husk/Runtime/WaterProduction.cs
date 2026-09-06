using System;

namespace Husk
{
    public sealed class WaterProduction
    {
        private double elapsed;
        public double BuildSeconds { get; }
        public double IntervalSeconds { get; }
        public int WaterPerCycle { get; }
        public bool HasStarted { get; private set; }
        public bool IsOperational => HasStarted && elapsed >= BuildSeconds;
        public bool IsBuilding => HasStarted && !IsOperational;
        public double ConstructionProgress => Math.Min(1, elapsed / BuildSeconds);
        public double ProductionProgress => IsOperational ? ((elapsed - BuildSeconds) % IntervalSeconds) / IntervalSeconds : 0;
        public long ProducedCycles { get; private set; }

        public WaterProduction(float buildSeconds, float intervalSeconds, int waterPerCycle)
        {
            if (float.IsNaN(buildSeconds) || float.IsInfinity(buildSeconds) || buildSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(buildSeconds));
            if (float.IsNaN(intervalSeconds) || float.IsInfinity(intervalSeconds) || intervalSeconds <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalSeconds));
            if (waterPerCycle < 0) throw new ArgumentOutOfRangeException(nameof(waterPerCycle));
            BuildSeconds = buildSeconds;
            IntervalSeconds = intervalSeconds;
            WaterPerCycle = waterPerCycle;
        }

        public bool TryStart()
        {
            if (HasStarted) return false;
            HasStarted = true;
            return true;
        }

        public int Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (!HasStarted) return 0;
            double nextElapsed = elapsed + deltaTime;
            long nextCycles = nextElapsed < BuildSeconds ? 0 : checked((long)Math.Floor((nextElapsed - BuildSeconds) / IntervalSeconds));
            int amount = checked((int)((nextCycles - ProducedCycles) * WaterPerCycle));
            elapsed = nextElapsed;
            ProducedCycles = nextCycles;
            return amount;
        }
    }
}
