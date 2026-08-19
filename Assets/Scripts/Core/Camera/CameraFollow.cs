using UnityEngine;
using VampireSurvivors.Logic;


namespace VampireSurvivors.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Focus")]
        [SerializeField] private Vector3 focusOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Camera")]
        [SerializeField] private float distance = 8f;
        [SerializeField] private float pitch = 35f;
        [SerializeField] private float yaw = 0f;

        [Header("Follow")]
        [SerializeField] private float smoothSpeed = 8f;

        public void Initialize(Transform target)
        {
            this.target = target;
        }

        private void OnEnable()
        {
            LocalPlayer.OnRegistered += OnPlayerRegistered;
            LocalPlayer.OnUnregistered += OnPlayerUnregistered;

            if (LocalPlayer.Instance != null)
            {
                OnPlayerRegistered(LocalPlayer.Instance); // trường hợp camera xuất hiện sau  LocalPlayer.Register, không nghe được event
            }
        }

        private void OnDisable() {
            LocalPlayer.OnRegistered -= OnPlayerRegistered;
            LocalPlayer.OnUnregistered -= OnPlayerUnregistered;
        }

        private void OnPlayerRegistered(Player player)
        {
            Initialize(player.transform);
        }

        private void OnPlayerUnregistered(Player player)
        {
            if (target == player.transform)
            {
                target = null;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 focusPoint = target.position + focusOffset;

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

            transform.position = desiredPosition;
            transform.rotation = rotation;
        }

    }
}
