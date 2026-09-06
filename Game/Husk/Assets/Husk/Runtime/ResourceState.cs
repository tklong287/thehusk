using System;

namespace Husk
{
    public enum ResourceKind { Food, Water, RecyclableMaterial, Wood, Iron, Fish, ModuleCore }

    // Gameplay state owns quantities; presentation only reads this object.
    public sealed class ResourceState
    {
        private readonly double[] amounts = new double[7];
        public event Action Changed;

        public ResourceState(int startingWood = 100, int startingIron = 100,
            int startingFood = 100, int startingWater = 100, int startingRecyclableMaterial = 100, int startingFish = 100, int startingModuleCore = 100)
        {
            ValidateAmount(startingFish);
            ValidateAmount(startingModuleCore);
            ValidateAmount(startingWood);
            ValidateAmount(startingIron);
            ValidateAmount(startingFood);
            ValidateAmount(startingWater);
            ValidateAmount(startingRecyclableMaterial);
            amounts[(int)ResourceKind.Food] = startingFood;
            amounts[(int)ResourceKind.Water] = startingWater;
            amounts[(int)ResourceKind.RecyclableMaterial] = startingRecyclableMaterial;
            amounts[(int)ResourceKind.Wood] = startingWood;
            amounts[(int)ResourceKind.Iron] = startingIron;
            amounts[(int)ResourceKind.Fish] = startingFish;
            amounts[(int)ResourceKind.ModuleCore] = startingModuleCore;
        }

        // Existing production/construction APIs use whole items; consumption keeps exact fractions.
        public int Get(ResourceKind resource) => (int)Math.Floor(GetExact(resource));
        public double GetExact(ResourceKind resource) => amounts[Index(resource)];
        public double FoodAvailable => GetExact(ResourceKind.Fish) + GetExact(ResourceKind.Food);

        // One city demand debit, regardless of House count. Fish first, then legacy Food (provisional).
        public void ConsumeFoodAndWater(double food, double water)
        {
            if (double.IsNaN(food) || double.IsInfinity(food) || food < 0 ||
                double.IsNaN(water) || double.IsInfinity(water) || water < 0)
                throw new ArgumentOutOfRangeException(nameof(food));
            double fish = Math.Min(food, amounts[(int)ResourceKind.Fish]);
            double legacy = Math.Min(food - fish, amounts[(int)ResourceKind.Food]);
            double wet = Math.Min(water, amounts[(int)ResourceKind.Water]);
            amounts[(int)ResourceKind.Fish] -= fish;
            amounts[(int)ResourceKind.Food] -= legacy;
            amounts[(int)ResourceKind.Water] -= wet;
            if (fish + legacy + wet > 0) Changed?.Invoke();
        }

        public void Add(ResourceKind resource, int amount)
        {
            int index = Index(resource);
            ValidateAmount(amount);
            if (amount == 0) return;
            double next = amounts[index] + amount;
            if (next > int.MaxValue) throw new OverflowException();
            amounts[index] = next;
            Changed?.Invoke();
        }

        // Insufficient stock is an expected gameplay outcome, with no partial debit.
        public bool TryRemove(ResourceKind resource, int amount)
        {
            int index = Index(resource);
            ValidateAmount(amount);
            if (amounts[index] < amount) return false;
            if (amount == 0) return true;
            amounts[index] -= amount;
            Changed?.Invoke();
            return true;
        }

        // Recycler conversion is atomic: subscribers see both updated quantities together.
        public bool TryRecycle(int recyclableMaterial, int wood)
        {
            if (recyclableMaterial <= 0) throw new ArgumentOutOfRangeException(nameof(recyclableMaterial));
            if (wood <= 0) throw new ArgumentOutOfRangeException(nameof(wood));
            int input = (int)ResourceKind.RecyclableMaterial;
            int output = (int)ResourceKind.Wood;
            if (amounts[input] < recyclableMaterial) return false;
            double nextWood = amounts[output] + wood;
            if (nextWood > int.MaxValue) throw new OverflowException();
            amounts[input] -= recyclableMaterial;
            amounts[output] = nextWood;
            Changed?.Invoke();
            return true;
        }

        // ModuleLayout validates stock first and publishes only after adding the Module.
        internal void DebitModuleConstruction()
        {
            if (Get(ResourceKind.ModuleCore) < ModuleLayout.CoreCost || Get(ResourceKind.Wood) < ModuleLayout.WoodCost)
                throw new InvalidOperationException("Insufficient Module construction stock.");
            amounts[(int)ResourceKind.ModuleCore] -= ModuleLayout.CoreCost;
            amounts[(int)ResourceKind.Wood] -= ModuleLayout.WoodCost;
        }
        internal void PublishConstructionPayment() => Changed?.Invoke();

        private static int Index(ResourceKind resource)
        {
            if (resource < ResourceKind.Food || resource > ResourceKind.ModuleCore)
                throw new ArgumentOutOfRangeException(nameof(resource));
            return (int)resource;
        }

        private static void ValidateAmount(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Resource amounts cannot be negative.");
        }
    }
}
