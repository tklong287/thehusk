using System;

namespace Husk
{
    public enum ResourceKind { Food, Water, RecyclableMaterial, Wood, Iron }

    // Gameplay state owns quantities; presentation only reads this object.
    public sealed class ResourceState
    {
        private readonly int[] amounts = new int[5];
        public event Action Changed;

        public ResourceState(int startingWood = 100, int startingIron = 100,
            int startingFood = 100, int startingWater = 100, int startingRecyclableMaterial = 100)
        {
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
        }

        public int Get(ResourceKind resource) => amounts[Index(resource)];

        public void Add(ResourceKind resource, int amount)
        {
            int index = Index(resource);
            ValidateAmount(amount);
            if (amount == 0) return;
            amounts[index] = checked(amounts[index] + amount);
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

        private static int Index(ResourceKind resource)
        {
            if (resource < ResourceKind.Food || resource > ResourceKind.Iron)
                throw new ArgumentOutOfRangeException(nameof(resource));
            return (int)resource;
        }

        private static void ValidateAmount(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Resource amounts cannot be negative.");
        }
    }
}
