using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;
using FishNet.Connection;

using Weapons;
using Inspector;

namespace Controllers
{
    public class WeaponController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private BaseWeapon currentWeapon;
        [SerializeField] private Transform weaponAttachPoint;
        [SerializeField] private CameraController playerCamera;

        [Header("Weapon Properties")]
        [SerializeField] private float rotationSensitivity;
        [SerializeField] private AnimationCurve rotationSensCurve;
 
        [Header("Input References")]
        [SerializeField] private InputActionReference weaponActionRef;

        [Header("Debugging - Input")]
        [SerializeField, ReadOnly] private Vector2 weaponInput;
        [SerializeField, ReadOnly] private float magnitude;

        [Header("Debugging - Rotation")]
        [SerializeField, ReadOnly] private Vector3 rotationDirection;
        [SerializeField, ReadOnly] private float stickAngle;
        [SerializeField, ReadOnly] private float targetAngle;
        [SerializeField, ReadOnly] private float turnVelocity;
        [SerializeField, ReadOnly] private float setRotation;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!base.IsOwner)
                return;

            weaponActionRef.action.Enable();

            ServerRpc_SpawnWeapon(LocalConnection);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!base.IsOwner)
                return;

            weaponActionRef.action.Disable();
        }

        private void Update()
        {
            if (!base.IsOwner)
                return;

            InputHandler();

            CalculateRotation();
        }

        private void FixedUpdate()
        {
            if (!base.IsOwner)
                return;

            RotateWeapon(weaponInput);

            if (currentWeapon != null)
                currentWeapon.Fire();
        }

        #region RPC Functions

        [ServerRpc(RequireOwnership = false)]
        private void ServerRpc_SpawnWeapon(NetworkConnection _conn)
        {
            NetworkObject nob = Instantiate(currentWeapon);
            ServerManager.Spawn(nob, _conn);

            if (weaponAttachPoint.TryGetComponent(out NetworkBehaviour attachNob))
                nob.SetParent(attachNob);

            StartCoroutine(SetParentDelay(nob, _conn.ClientId));

            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
        }

        private IEnumerator SetParentDelay(NetworkObject _nob, int _clientId)
        {
            yield return null;

            Debug.Log($"{_nob.transform.parent}, Client ID: {_clientId}");
        }

        #endregion

        #region Update Functions

        private void InputHandler()
        {
            weaponInput = weaponActionRef.action.ReadValue<Vector2>();

            InputAction action = weaponActionRef.action;
            InputControl control = action.activeControl;

            if(control != null)
            {
                InputDevice device = control.device;
                Debug.Log(device.displayName);
            }

            magnitude = weaponInput.magnitude;
        }

        private void CalculateRotation()
        {
            if (weaponInput.sqrMagnitude == 0)
                return;

            setRotation = rotationSensCurve.Evaluate(weaponInput.sqrMagnitude) * rotationSensitivity;
        }

        #endregion

        #region Fixed Update Functions

        private void RotateWeapon(Vector2 _weaponInput)
        {
            if (weaponInput.sqrMagnitude == 0)
            {
                turnVelocity = 0.0f;
                return;
            }

            rotationDirection = (playerCamera.Forward * weaponInput.y) + (playerCamera.Right * weaponInput.x);
            
            stickAngle = Mathf.Atan2(rotationDirection.x, rotationDirection.z) * Mathf.Rad2Deg;
            targetAngle = Mathf.SmoothDampAngle(weaponAttachPoint.eulerAngles.y, stickAngle, ref turnVelocity, Time.fixedDeltaTime * setRotation);

            weaponAttachPoint.rotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
        }

        #endregion
    }
}
