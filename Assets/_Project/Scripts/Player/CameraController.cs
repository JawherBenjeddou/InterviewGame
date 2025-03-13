using Cinemachine;
using UnityEngine;

namespace InterviewGame.player
{
    public class CameraController : MonoBehaviour
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera followCamera;
        public GameObject followTarget;

        [Header("Speed FOV Settings")]
        [Tooltip("The base FOV when the player is stationary.")]
        public float baseFOV = 70f;

        [Tooltip("The maximum FOV to reach when at max speed.")]
        public float maxFOV = 75f;

        [Header("Camera Settings")]
        [Tooltip("Sensitivity for looking around.")]
        public Vector2 lookSensitivity = new Vector2(2.0f, 2.0f);

        [Tooltip("Min and max pitch angles.")]
        public float minPitch = -80.0f;
        public float maxPitch = 80.0f;

        [Tooltip("The min and max follow distances.")]
        public float followMinDistance = 1.0f;
        public float followMaxDistance = 10.0f;

        [Tooltip("Default follow distance.")]
        public float followDistance = 5.0f;

        [Header("External Components")]
        private Player m_player;

        private Cinemachine3rdPersonFollow m_cmThirdPersonFollow;
        private float m_cameraTargetPitch;
        private float m_followDistanceSmoothVelocity;

        private void Awake()
        {
            m_player = GetComponent<Player>();

            // Fetch the Cinemachine 3rd person follow component
            m_cmThirdPersonFollow = followCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Use old input system for looking around
            float lookX = Input.GetAxis("Mouse X") * lookSensitivity.x;
            float lookY = Input.GetAxis("Mouse Y") * lookSensitivity.y;

            // Invert the lookY for up and down
            lookY = -lookY;

            // Apply the pitch clamping
            AddControlPitchInput(lookY, minPitch, maxPitch);

            // Update the camera rotation based on mouse X for yaw (left/right)
            followTarget.transform.Rotate(0, lookX, 0);
        }


        private void LateUpdate()
        {
            UpdateCamera();
        }

        public void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            if (value == 0.0f)
                return;

            // Only clamp the pitch (up and down)
            m_cameraTargetPitch = Mathf.Clamp(m_cameraTargetPitch + value, minValue, maxValue);
        }

        public void AddControlZoomInput(float value)
        {
            followDistance = Mathf.Clamp(followDistance - value, followMinDistance, followMaxDistance);
        }

        private void UpdateCamera()
        {
            // Apply the pitch angle to the follow target's rotation
            followTarget.transform.rotation = Quaternion.Euler(m_cameraTargetPitch, followTarget.transform.eulerAngles.y, 0.0f);

            // Smooth camera zoom transitions
            m_cmThirdPersonFollow.CameraDistance = Mathf.SmoothDamp(m_cmThirdPersonFollow.CameraDistance, followDistance, ref m_followDistanceSmoothVelocity, 0.1f);
        }

        public float GetCameraRotationOffset()
        {
            // Ensure followTarget and followCamera exist
            if (!followTarget || !followCamera)
                return 0f;

            // Get the player's forward direction, ignoring the y-axis
            Vector3 playerForward = Vector3.ProjectOnPlane(followTarget.transform.forward, Vector3.up).normalized;

            // Get the camera's forward direction, ignoring the y-axis
            Vector3 cameraForward = Vector3.ProjectOnPlane(followCamera.transform.forward, Vector3.up).normalized;

            // Calculate the signed angle between the player's forward and the camera's forward
            float angleOffset = Vector3.SignedAngle(playerForward, cameraForward, Vector3.up);

            return angleOffset; // Offset in degrees
        }

        public void RotateCharacterTowards(Transform target, Vector3 direction, float rotationRate)
        {
            if (direction.sqrMagnitude < Mathf.Epsilon)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            target.rotation = Quaternion.Lerp(target.rotation, targetRotation, Time.deltaTime * rotationRate);
        }
    }
}
