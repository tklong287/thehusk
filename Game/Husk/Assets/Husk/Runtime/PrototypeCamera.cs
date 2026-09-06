using UnityEngine;
using UnityEngine.InputSystem;

namespace Husk
{
    [DisallowMultipleComponent, RequireComponent(typeof(Camera))]
    public sealed class PrototypeCamera : MonoBehaviour
    {
        [Header("Provisional observation framing")]
        [SerializeField] private Vector3 focus = new Vector3(0, 0.5f, 0);
        [SerializeField] private float yaw = 35f;
        [SerializeField, Range(15f, 80f)] private float pitch = 48f;
        [SerializeField, Min(1f)] private float viewSize = 15f;
        [SerializeField, Min(1f)] private float minViewSize = 8f;
        [SerializeField, Min(1f)] private float maxViewSize = 24f;
        [SerializeField, Min(0.01f)] private float orbitSensitivity = 0.2f;
        [SerializeField, Min(0.001f)] private float zoomSensitivity = 0.02f;
        [SerializeField, Min(0f)] private float moveSpeed = 12f;

        private Camera view;
        private FishingHarbor harbor;
        private WaterPlant waterPlant;
        private Recycler recycler;
        private ModuleNetworkPrototype modulePrototype;
        private float initialYaw;
        private float initialPitch;
        private float initialSize;
        private Vector3 initialFocus;

        private void Awake()
        {
            view = GetComponent<Camera>();
            harbor = FindAnyObjectByType<FishingHarbor>();
            waterPlant = FindAnyObjectByType<WaterPlant>();
            recycler = FindAnyObjectByType<Recycler>();
            modulePrototype = FindAnyObjectByType<ModuleNetworkPrototype>();
            initialYaw = yaw;
            initialPitch = pitch;
            initialSize = viewSize;
            initialFocus = focus;
            ApplyView();
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse != null && (modulePrototype == null || !modulePrototype.IsScreenPointOverPanel(mouse.position.ReadValue())))
            {
                if (mouse.rightButton.isPressed)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    yaw += delta.x * orbitSensitivity;
                    pitch = Mathf.Clamp(pitch - delta.y * orbitSensitivity, 15f, 80f);
                }
                if ((harbor == null || !harbor.IsScreenPointOverPanel(mouse.position.ReadValue()))
                    && (waterPlant == null || !waterPlant.IsScreenPointOverPanel(mouse.position.ReadValue()))
                    && (recycler == null || !recycler.IsScreenPointOverPanel(mouse.position.ReadValue())))
                    viewSize = Mathf.Clamp(viewSize - mouse.scroll.ReadValue().y * zoomSensitivity,
                        minViewSize, Mathf.Max(minViewSize, maxViewSize));
            }
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                Vector2 movement = new Vector2(
                    (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f),
                    (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f));
                movement = Vector2.ClampMagnitude(movement, 1f);
                // Move along the water plane relative to the current viewing direction.
                focus += Quaternion.Euler(0f, yaw, 0f) * new Vector3(movement.x, 0f, movement.y)
                    * (moveSpeed * Time.unscaledDeltaTime);

                if (keyboard.rKey.wasPressedThisFrame)
                {
                    focus = initialFocus;
                    yaw = initialYaw;
                    pitch = initialPitch;
                    viewSize = initialSize;
                }
            }
            ApplyView();
        }

        private void ApplyView()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.SetPositionAndRotation(focus - rotation * Vector3.forward * 50f, rotation);
            view.orthographic = true;
            view.orthographicSize = viewSize;
        }
    }
}
