using System;
using UnityEngine;

namespace Husk
{
    [Serializable]
    public sealed class PopulationSettings
    {
        [Min(0)] public double startingPopulation = 100;
        [Min(1)] public int houseCapacity = 100;
        [Min(0.01f)] public double growthInterval = 60;
        [Min(0)] public double growthFraction = 0.1;
        [Min(0)] public double foodPerResidentSecond = 0.05, waterPerResidentSecond = 0.05;
        [Range(0, 3)] public int displayDecimals = 1; // Presentation only; never rounds simulation.
        [Min(0.001f)] public double simulationQuantum = 0.05; // Provisional deterministic city tick.
    }

    // Overall population only. Capacity never limits growth or removes residents.
    public sealed class CityPopulation
    {
        private readonly PopulationSettings settings;
        public double Current { get; private set; }
        public double GrowthElapsed { get; private set; }
        public int HouseCount { get; private set; }
        public int OperationalHouses { get; private set; }
        public int EffectiveCapacity => OperationalHouses * settings.houseCapacity;
        public double FoodDemand => Current * settings.foodPerResidentSecond;
        public double WaterDemand => Current * settings.waterPerResidentSecond;
        public string Display => Current.ToString("F" + settings.displayDecimals) + " / " + EffectiveCapacity;

        public CityPopulation(PopulationSettings settings)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            foreach (double value in new[] { settings.startingPopulation, settings.growthFraction,
                settings.foodPerResidentSecond, settings.waterPerResidentSecond })
                if (double.IsNaN(value) || double.IsInfinity(value) || value < 0) throw new ArgumentOutOfRangeException(nameof(settings));
            if (!(settings.growthInterval > 0) || double.IsInfinity(settings.growthInterval) ||
                !(settings.simulationQuantum > 0) || double.IsInfinity(settings.simulationQuantum) ||
                settings.simulationQuantum > settings.growthInterval || settings.houseCapacity <= 0 ||
                settings.displayDecimals < 0 || settings.displayDecimals > 3) throw new ArgumentOutOfRangeException(nameof(settings));
            Current = settings.startingPopulation;
        }
        internal void ResolveHousing(ModuleLayout layout, NetworkedCity city)
        {
            HouseCount = 0; OperationalHouses = 0;
            foreach (var cell in layout.Cells.Values)
                if (cell.Building == ModuleBuilding.House)
                { HouseCount++; if (city.State(cell.Position) == NetworkBuildingState.Operational) OperationalHouses++; }
        }
        public void Tick(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            GrowthElapsed += seconds;
            while (GrowthElapsed + 1e-9 >= settings.growthInterval)
            { Current *= 1 + settings.growthFraction; GrowthElapsed = Math.Max(0, GrowthElapsed - settings.growthInterval); }
        }
    }
}
