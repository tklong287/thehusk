using System;

namespace Husk
{
    public sealed class RecyclerProcessing
    {
        private double constructionElapsed, processingElapsed;
        public double BuildSeconds { get; }
        public double IntervalSeconds { get; }
        public int InputPerCycle { get; }
        public int WoodPerCycle { get; }
        public bool HasStarted { get; private set; }
        public bool IsOperational => HasStarted && constructionElapsed >= BuildSeconds;
        public bool IsBuilding => HasStarted && !IsOperational;
        public bool IsWaiting { get; private set; }
        public double ConstructionProgress => constructionElapsed / BuildSeconds;
        public double ProcessingProgress => processingElapsed / IntervalSeconds;
        public long ProcessedCycles { get; private set; }

        public RecyclerProcessing(float buildSeconds, float intervalSeconds, int inputPerCycle, int woodPerCycle)
        {
            if (float.IsNaN(buildSeconds) || float.IsInfinity(buildSeconds) || buildSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(buildSeconds));
            if (float.IsNaN(intervalSeconds) || float.IsInfinity(intervalSeconds) || intervalSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(intervalSeconds));
            if (inputPerCycle <= 0) throw new ArgumentOutOfRangeException(nameof(inputPerCycle));
            if (woodPerCycle <= 0) throw new ArgumentOutOfRangeException(nameof(woodPerCycle));
            BuildSeconds = buildSeconds;
            IntervalSeconds = intervalSeconds;
            InputPerCycle = inputPerCycle;
            WoodPerCycle = woodPerCycle;
        }

        public bool TryStart()
        {
            if (HasStarted) return false;
            HasStarted = true;
            return true;
        }

        public int Tick(float deltaTime, ResourceState resources)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0) throw new ArgumentOutOfRangeException(nameof(deltaTime));
            if (resources == null) throw new ArgumentNullException(nameof(resources));
            if (!HasStarted) return 0;
            double remaining = deltaTime;
            if (!IsOperational)
            {
                double constructionTime = Math.Min(remaining, BuildSeconds - constructionElapsed);
                constructionElapsed += constructionTime;
                remaining -= constructionTime;
                if (!IsOperational) return 0;
            }
            int availableCycles = resources.Get(ResourceKind.RecyclableMaterial) / InputPerCycle;
            if (availableCycles == 0)
            {
                IsWaiting = true;
                processingElapsed = 0;
                return 0;
            }
            double nextElapsed = processingElapsed + remaining;
            int cycles = (int)Math.Min(availableCycles, Math.Floor(nextElapsed / IntervalSeconds));
            if (cycles > 0)
            {
                int consumed = checked(cycles * InputPerCycle);
                int produced = checked(cycles * WoodPerCycle);
                if (!resources.TryRecycle(consumed, produced)) return 0;
                ProcessedCycles += cycles;
            }
            IsWaiting = resources.Get(ResourceKind.RecyclableMaterial) < InputPerCycle;
            processingElapsed = IsWaiting ? 0 : nextElapsed - cycles * IntervalSeconds;
            return cycles;
        }
    }
}
