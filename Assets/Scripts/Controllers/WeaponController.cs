using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;
using FishNet.Connection;

using Weapons;
using Inspector;
using System;

namespace Controllers
{
    public class WeaponController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private BaseWeapon currentWeapon;
        [SerializeField] private Transform weaponAttachPoint;
        [SerializeField] private new CameraController camera;

        [Header("Weapon Properties")]
        [SerializeField] private float maxRotation;
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

            RotateWeapon();
        }

        #region RPC Functions

        [ServerRpc(RequireOwnership = false)]
        private void ServerRpc_SpawnWeapon(NetworkConnection _conn)
        {
            NetworkObject nob = Instantiate(currentWeapon);
            ServerManager.Spawn(nob, _conn);

            if (weaponAttachPoint.TryGetComponent(out NetworkBehaviour attachNob))
                nob.SetParent(attachNob);

            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
        }

        #endregion

        #region Update Functions

        private void InputHandler()
        {
            weaponInput = weaponActionRef.action.ReadValue<Vector2>();

            magnitude = weaponInput.magnitude;
        }

        private void CalculateRotation()
        {
            setRotation = rotationSensCurve.Evaluate(weaponInput.magnitude) * maxRotation;
        }

        #endregion

        #region Fixed Update Functions

        private void RotateWeapon()
        {
            if (weaponInput.magnitude == 0)
                return;

            rotationDirection = (camera.Forward * weaponInput.y) + (camera.Right * weaponInput.x);
            stickAngle = Mathf.Atan2(rotationDirection.x, rotationDirection.z) * Mathf.Rad2Deg;

            targetAngle = Mathf.SmoothDampAngle(weaponAttachPoint.eulerAngles.y, stickAngle, ref turnVelocity, Time.fixedDeltaTime * setRotation);

            weaponAttachPoint.rotation = Quaternion.Euler(0.0f, -targetAngle, 0.0f);
        }

        #endregion
    }
}
