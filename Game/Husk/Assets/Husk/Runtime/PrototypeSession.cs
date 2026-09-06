using UnityEngine;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class PrototypeSession : MonoBehaviour
    {
        [Header("Provisional test resources - unlimited storage")]
        [SerializeField, Min(0)] private int startingFood = 100;
        [SerializeField, Min(0)] private int startingWater = 100;
        [SerializeField, Min(0)] private int startingRecyclableMaterial = 100;
        [SerializeField, Min(0)] private int startingWood = 100;
        [SerializeField, Min(0)] private int startingIron = 100;

        private ResourceState resources;
        public ResourceState Resources => resources ??= new ResourceState(startingWood, startingIron,
            startingFood, startingWater, startingRecyclableMaterial);

        private void Awake()
        {
            // A scene reload / fresh Play Mode run creates a new state, with no persistence.
            _ = Resources;
        }
    }
}
