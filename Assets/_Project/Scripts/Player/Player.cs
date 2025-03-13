using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterviewGame.player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [Header("Resource Managers")]
        [SerializeField] private HealthManager m_healthManager;

        [Header("Movement Settings")]
        [SerializeField] private float m_moveSpeed = 5f;
        [SerializeField] private float m_jumpForce = 7f;
        [SerializeField] private float m_friction = 5f; // Higher value = stops faster

        [SerializeField] private LayerMask m_groundLayer;

        private Rigidbody m_rigidbody;

        private void Awake()
        {
            m_healthManager = GetComponent<HealthManager>();
            m_rigidbody = GetComponent<Rigidbody>();

            // Ensure Rigidbody settings are correct
            m_rigidbody.freezeRotation = true;
            m_rigidbody.useGravity = true;
            m_rigidbody.drag = m_friction; // Apply friction to reduce sliding
        }

        void Update()
        {
            HandleMovement();
            HandleJump();
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // Get camera forward and right vectors
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Ignore the y component to keep movement on the ground
            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            // Compute movement direction relative to camera
            Vector3 moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                Vector3 velocity = moveDirection * m_moveSpeed;
                m_rigidbody.velocity = new Vector3(velocity.x, m_rigidbody.velocity.y, velocity.z);
            }
            else
            {
                // Apply manual friction when no input is detected
                m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, 0);
            }
        }


        private void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, m_jumpForce, m_rigidbody.velocity.z);
            }
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.1f, m_groundLayer);
        }
    }
}
