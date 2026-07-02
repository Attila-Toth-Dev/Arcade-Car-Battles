using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;

using Inspector;
using System;

namespace Car
{
    public class CarController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody carRb;
        [SerializeField] private Transform model;
        [SerializeField] private Transform parent;

        [Header("Properties")]
        [SerializeField] private float speed = 10.0f;
        [SerializeField] private float steer = 10.0f;
        [SerializeField] private float offset = 0.4f;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveActionRef;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private bool isAccelerating;
        [SerializeField, ReadOnly] private bool isSteering;
        [SerializeField, ReadOnly] private float currentSpeed;
        [SerializeField, ReadOnly] private float currentSteer;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner) 
                return;
            
            moveActionRef.action.Enable();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!base.IsOwner)
                return;

            moveActionRef.action.Disable();
        }

        private void Update()
        {
            if (!base.IsOwner)
                return;

            InputHandler();

            CalculateAcceleration();
            CalculateSteering();
        }

        private void FixedUpdate()
        {
            if (!base.IsOwner)
                return;
        }

        #region Update Functions
        
        private void InputHandler()
        {
            moveInput = moveActionRef.action.ReadValue<Vector2>();
            
            isAccelerating = moveActionRef.action.ReadValue<Vector2>().y != 0.0f;
            isSteering = moveActionRef.action.ReadValue<Vector2>().x != 0.0f;
        }

        private void CalculateAcceleration()
        {
        }

        private void CalculateSteering()
        {
        }

        #endregion
    }
}
