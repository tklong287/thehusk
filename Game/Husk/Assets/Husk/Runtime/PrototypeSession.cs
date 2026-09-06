using UnityEngine;

namespace Husk
{
    [DisallowMultipleComponent]
    public sealed class PrototypeSession : MonoBehaviour
    {
        [Header("Provisional starting materials - not design canon")]
        [SerializeField, Min(0)] private int startingWood = 0;
        [SerializeField, Min(0)] private int startingIron = 5;

        [Header("Provisional Water need representation")]
        [SerializeField, TextArea(2, 4)] private string waterNeedMessage =
            "Residents need a reliable supply of Water. The settlement has only a small starting reserve.";

        private ResourceState resources;
        public ResourceState Resources => resources ??= new ResourceState(startingWood, startingIron);
        public string WaterNeedMessage => waterNeedMessage;

        private void Awake()
        {
            // A scene reload / fresh Play Mode run creates a new state, with no persistence.
            _ = Resources;
        }
    }
}
