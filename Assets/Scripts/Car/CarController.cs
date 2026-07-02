using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;

using Inspector;

namespace Car
{
    public class CarController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody carRb;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveActionRef;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private bool isMoving;

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
        }

        private void FixedUpdate()
        {
            if (!base.IsOwner)
                return;

            MoveVehicle();
        }

        #region Update Functions
        
        private void InputHandler()
        {

        }

        #endregion

        #region Fixed Update Functions
        
        private void MoveVehicle()
        {

        } 

        #endregion
    }
}
