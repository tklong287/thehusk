using System;

namespace Husk
{
    public enum FishingTripStage { Depart, Fishing, Return, Harbor }

    // Complete trip includes travel and the harbor stop. No production here.
    public sealed class FishingTrip
    {
        private readonly double departEnd, fishingEnd, returnEnd;
        private double elapsed;
        public double Duration { get; }
        public double CycleElapsed { get; private set; }
        public long CompletedCycles { get; private set; }
        public long Arrivals { get; private set; }
        public FishingTripStage Stage { get; private set; }
        public float StageProgress { get; private set; }

        public FishingTrip(float duration, float departWeight, float fishingWeight, float returnWeight, float harborWeight)
        {
            foreach (float value in new[] { duration, departWeight, fishingWeight, returnWeight, harborWeight })
                if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(duration), "Timing values must be finite and positive.");
            Duration = duration;
            double total = (double)departWeight + fishingWeight + returnWeight + harborWeight;
            departEnd = (double)duration * departWeight / total;
            fishingEnd = departEnd + (double)duration * fishingWeight / total;
            returnEnd = fishingEnd + (double)duration * returnWeight / total;
        }

        public void Tick(float deltaTime)
        {
            if (float.IsNaN(deltaTime) || float.IsInfinity(deltaTime) || deltaTime < 0)
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            elapsed += deltaTime;
            CompletedCycles = (long)Math.Floor(elapsed / Duration);
            CycleElapsed = elapsed % Duration;
            Arrivals = CompletedCycles + (CycleElapsed >= returnEnd ? 1L : 0L);
            double start, end;
            if (CycleElapsed < departEnd) { Stage = FishingTripStage.Depart; start = 0; end = departEnd; }
            else if (CycleElapsed < fishingEnd) { Stage = FishingTripStage.Fishing; start = departEnd; end = fishingEnd; }
            else if (CycleElapsed < returnEnd) { Stage = FishingTripStage.Return; start = fishingEnd; end = returnEnd; }
            else { Stage = FishingTripStage.Harbor; start = returnEnd; end = Duration; }
            StageProgress = (float)((CycleElapsed - start) / (end - start));
        }
    }
}
