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
        [SerializeField] private Transform carModel;
        [SerializeField] private Transform carParent;
        [SerializeField] private Transform carNormal;

        [Header("Car Properties")]
        [SerializeField] private AnimationCurve accelerationCurve;
        [SerializeField] private float topSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float steering;

        [Header("Physics Properties")]
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float colliderOffset = 0.5f;
        [SerializeField] private float rayDistance = 1.1f;
        [SerializeField] private LayerMask layerMask;
 
        [Header("Input References")]
        [SerializeField] private InputActionReference moveActionRef;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private bool isAccelerating;
        [SerializeField, ReadOnly] private bool isSteering;
        [SerializeField, ReadOnly] private bool isGrounded;
        [SerializeField, ReadOnly] private float currentSpeed;
        [SerializeField, ReadOnly] private float currentRotate;
        [SerializeField, ReadOnly] private float speed;
        [SerializeField, ReadOnly] private float rotate;

        private Quaternion cachedRotation;

        private RaycastHit hitOn;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner) 
                return;
            
            moveActionRef.action.Enable();

            cachedRotation = new Quaternion(carNormal.rotation.x, carNormal.rotation.y, carNormal.rotation.z, 0.0f);
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

            carParent.position = carRb.transform.position - new Vector3(0, colliderOffset, 0);

            InputHandler();

            CalculateAcceleration();
            CalculateSteering();   
        }

        private void FixedUpdate()
        {
            if (!base.IsOwner)
                return;

            GroundCheck();

            ApplyAcceleration();
            ApplySteering();
            ApplyGravity();

            RotateCarBody();
        }

        #region Update Functions

        private void InputHandler()
        {
            moveInput = moveActionRef.action.ReadValue<Vector2>();

            isAccelerating = moveInput.y != 0.0f;
            isSteering = moveInput.x != 0.0f;
        }

        private void CalculateAcceleration()
        {
            if (isAccelerating)
                speed = topSpeed * moveInput.y;
            else
                speed = 0;

            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * acceleration);
        }

        private void CalculateSteering()
        {
            if (isSteering && currentSpeed > Mathf.Abs(2.0f))
            {
                int dir = moveInput.x > 0 ? 1 : -1;
                float amount = Mathf.Abs(moveInput.x);

                rotate = (steering * dir) * amount;
            }
            else
                rotate = 0;

            currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * deceleration);
        }

        #endregion

        #region Fixed Update Functions

        private void GroundCheck()
        {
            isGrounded = Physics.Raycast(carParent.transform.position, Vector3.down, out hitOn, rayDistance, layerMask);
        }

        private void ApplyAcceleration()
        {
            carRb.AddForce(carModel.transform.forward * (isGrounded ? currentSpeed : currentSpeed * 0.2f), ForceMode.Acceleration);
        }

        private void ApplySteering()
        {
            carParent.transform.eulerAngles = Vector3.Lerp(carParent.transform.eulerAngles, new Vector3(0, carParent.transform.eulerAngles.y + currentRotate, 0), Time.fixedDeltaTime * 5.0f);
        }

        private void ApplyGravity()
        {
            carRb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        private void RotateCarBody()
        {
            if(isGrounded)
            {
                carNormal.up = Vector3.Lerp(carNormal.up, hitOn.normal, Time.fixedDeltaTime * 8.0f);
                carNormal.Rotate(0, carParent.transform.eulerAngles.y, 0);
            }
            else
                carNormal.Rotate(cachedRotation.x, cachedRotation.y, cachedRotation.z);
        }

        #endregion
    }
}
