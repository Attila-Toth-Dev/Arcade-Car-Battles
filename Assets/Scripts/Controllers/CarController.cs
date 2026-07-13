using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;

using Inspector;

namespace Controllers
{
    public class CarController : NetworkBehaviour
    {
        #region Getters & Setters
        
        public float ColliderRadius
        {
            get => carCollider.radius;
        } 

        #endregion

        // -- BODY ROTATION CONSTANTS -- //
        private const float BODY_ROTATE_SPEED = 5.0f;

        [Header("References")]
        [SerializeField] private Transform model;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform normal;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private SphereCollider carCollider;
        [SerializeField] private new CameraController camera;

        [Header("Car Properties")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float steeringSensitivity;
        [SerializeField] private AnimationCurve steeringSensCurve;

        [Header("Physics Properties")]
        [SerializeField] private float colliderOffset = 0.5f;
        [SerializeField] private float rayDistance = 1.1f;
        [SerializeField] private LayerMask layerMask;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveActionRef;

        [Header("Debugging - Input")]
        [SerializeField, ReadOnly] private Vector2 moveInput;

        [Header("Debugging - Acceleration")]
        [SerializeField, ReadOnly] private float currentSpeed;
        [SerializeField, ReadOnly] private float clampedSpeed;
        [SerializeField, ReadOnly] private float setSpeed;

        [Header("Debugging - Steering")]
        [SerializeField, ReadOnly] private Vector3 rotationDirection;
        [SerializeField, ReadOnly] private float stickAngle;
        [SerializeField, ReadOnly] private float targetAngle;
        [SerializeField, ReadOnly] private float turnVelocity;
        [SerializeField, ReadOnly] private float setSteer;

        [Header("Debugging - Physics")]
        [SerializeField, ReadOnly] private bool isGrounded;

        private Vector3 cachedRotation;

        private RaycastHit hitOn;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!base.IsOwner) 
                return;
            
            moveActionRef.action.Enable();

            cachedRotation = new Vector3(normal.rotation.x, normal.rotation.y, normal.rotation.z);
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
            parent.position = rigidBody.transform.position - new Vector3(0, colliderOffset, 0);

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

            GroundCheck();

            MoveCar(moveInput);
            RotateCar(moveInput);
            
            RotateCarBody();
        }

        #region Update Functions

        private void InputHandler()
        {
            moveInput = moveActionRef.action.ReadValue<Vector2>();
        }

        private void CalculateAcceleration()
        {
            setSpeed = maxSpeed * moveInput.sqrMagnitude;
            clampedSpeed = Mathf.Clamp01(currentSpeed / maxSpeed);

            currentSpeed = Mathf.SmoothStep(currentSpeed, setSpeed, Time.deltaTime * acceleration);
        }

        private void CalculateSteering()
        {
            if (moveInput.magnitude == 0)
                return;

            setSteer = steeringSensCurve.Evaluate(clampedSpeed) * steeringSensitivity;
        }

        #endregion

        #region Fixed Update Functions

        private void GroundCheck()
        {
            isGrounded = UnityEngine.Physics.Raycast(parent.transform.position, Vector3.down, out hitOn, rayDistance, layerMask);
        }

        private void MoveCar(Vector2 _moveInput)
        {
            Vector3 moveDir = (camera.Forward * _moveInput.y) + (camera.Right * _moveInput.x);
            rigidBody.AddForce(moveDir * currentSpeed, ForceMode.Acceleration);
        }

        private void RotateCar(Vector2 _moveInput)
        {
            if (moveInput.sqrMagnitude == 0)
            {
                turnVelocity = 0.0f;
                return;
            }

            rotationDirection = (camera.Forward * _moveInput.y) + (camera.Right * _moveInput.x);

            stickAngle = Mathf.Atan2(rotationDirection.x, rotationDirection.z) * Mathf.Rad2Deg;
            targetAngle = Mathf.SmoothDampAngle(parent.eulerAngles.y, stickAngle, ref turnVelocity, Time.fixedDeltaTime * setSteer);

            parent.rotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
        }

        private void RotateCarBody()
        {
            if(isGrounded)
            {
                normal.up = Vector3.Lerp(normal.up, hitOn.normal, Time.fixedDeltaTime * BODY_ROTATE_SPEED);
                normal.Rotate(0, parent.transform.eulerAngles.y, 0);
            }
            else
                normal.eulerAngles = cachedRotation;
        }

        #endregion
    }
}
