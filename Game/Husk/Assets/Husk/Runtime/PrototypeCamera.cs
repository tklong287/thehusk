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

        private Camera view;
        private float initialYaw;
        private float initialPitch;
        private float initialSize;

        private void Awake()
        {
            view = GetComponent<Camera>();
            initialYaw = yaw;
            initialPitch = pitch;
            initialSize = viewSize;
            ApplyView();
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse != null)
            {
                if (mouse.rightButton.isPressed)
                {
                    Vector2 delta = mouse.delta.ReadValue();
                    yaw += delta.x * orbitSensitivity;
                    pitch = Mathf.Clamp(pitch - delta.y * orbitSensitivity, 15f, 80f);
                }
                viewSize = Mathf.Clamp(viewSize - mouse.scroll.ReadValue().y * zoomSensitivity,
                    minViewSize, Mathf.Max(minViewSize, maxViewSize));
            }
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                yaw = initialYaw;
                pitch = initialPitch;
                viewSize = initialSize;
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
