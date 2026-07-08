using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;

using Physics;
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

        // -- STEERING CONSTANTS -- //
        private const float STEERING_SENSITIVITY = 5.0f;
        private const float VALID_STEERING_THRESHOLD = 0.5f;

        // -- BODY ROTATION CONSTANTS -- //
        private const float BODY_ROTATE_SPEED = 5.0f;

        [Header("References")]
        [SerializeField] private SphereCollider carCollider;
        [SerializeField] private Rigidbody carRb;
        [SerializeField] private Transform carModel;
        [SerializeField] private Transform carParent;
        [SerializeField] private Transform carNormal;
        [SerializeField] private CameraController cameraController;

        [Header("Car Properties")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float maxSteerAngle;
        [SerializeField] private AnimationCurve steeringSensCurve;

        [Header("Physics Properties")]
        [SerializeField] private float colliderOffset = 0.5f;
        [SerializeField] private float rayDistance = 1.1f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField, ReadOnly] private bool isGrounded;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveActionRef;

        [Header("Debugging - Input")]
        [SerializeField, ReadOnly] private Vector2 moveInput;

        [Header("Debugging - Acceleration")]
        [SerializeField, ReadOnly] private float currentSpeed;
        [SerializeField, ReadOnly] private float setSpeed;

        [Header("Debugging - Steering")]
        [SerializeField, ReadOnly] private float stickAngle;
        //[SerializeField, ReadOnly] private float currentRotate;
        //[SerializeField, ReadOnly] private float setRotate;
        //[SerializeField, ReadOnly] private float amount;

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

            MoveCar();
            RotateCar();

            RotateCarBody();
        }

        #region Update Functions

        private void InputHandler()
        {
            moveInput = moveActionRef.action.ReadValue<Vector2>();
        }

        private void CalculateAcceleration()
        {
            setSpeed = maxSpeed * Mathf.Abs(moveInput.sqrMagnitude);

            currentSpeed = Mathf.SmoothStep(currentSpeed, setSpeed, Time.deltaTime * acceleration);
        }

        private void CalculateSteering()
        {
            

            //float clampedSpeed = Mathf.Clamp01(currentSpeed / maxSpeed);
            //amount = steeringSensCurve.Evaluate(clampedSpeed);
            //
            //float dir = moveInput.x > 0 ? amount : -amount;
            //setRotate = maxSteerAngle * dir;
            //
            //currentRotate = Mathf.Lerp(currentRotate, setRotate, Time.deltaTime * deceleration);
        }

        #endregion

        #region Fixed Update Functions

        private void GroundCheck()
        {
            isGrounded = UnityEngine.Physics.Raycast(carParent.transform.position, Vector3.down, out hitOn, rayDistance, layerMask);
        }

        private void MoveCar()
        {
            Vector3 moveDir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
            carRb.AddForce(moveDir * currentSpeed, ForceMode.Acceleration);
        }

        private void RotateCar()
        {
            //carParent.transform.eulerAngles = Vector3.Lerp(carParent.transform.eulerAngles, new Vector3(0, carParent.transform.eulerAngles.y + currentRotate, 0), Time.fixedDeltaTime * STEERING_SENSITIVITY);
        }

        private void RotateCarBody()
        {
            if(isGrounded)
            {
                carNormal.up = Vector3.Lerp(carNormal.up, hitOn.normal, Time.fixedDeltaTime * BODY_ROTATE_SPEED);
                carNormal.Rotate(0, carParent.transform.eulerAngles.y, 0);
            }
            else
                carNormal.Rotate(cachedRotation.x, cachedRotation.y, cachedRotation.z);
        }

        #endregion
    }
}
