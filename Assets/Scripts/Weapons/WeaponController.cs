using UnityEngine;
using UnityEngine.InputSystem;

using FishNet.Object;

using Inspector;
using System;
using FishNet.Connection;

namespace Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private BaseWeapon currentWeapon;
        [SerializeField] private NetworkObject weaponAttachPoint;

        [Header("Input References")]
        [SerializeField] private InputActionReference weaponActionRef;

        [Header("Debugging - Input")]
        [SerializeField, ReadOnly] private Vector2 weaponInput;
        [SerializeField, ReadOnly] private bool isFiring;
        [SerializeField, ReadOnly] private bool isAiming;

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

            if(isAiming)
            {
                currentWeapon.Fire();
            }
        }

        private void InputHandler()
        {
            weaponInput = weaponActionRef.action.ReadValue<Vector2>();

            isFiring = weaponActionRef.action.triggered;
            isAiming = weaponInput.y > 0.0f || weaponInput.x > 0.0f;
        }

        #region RPC Functions

        [ServerRpc(RequireOwnership = false)]
        private void ServerRpc_SpawnWeapon(NetworkConnection _conn)
        {
            Debug.Log($"Client requesting to spawn weapon: {_conn.ClientId}");

            NetworkObject nob = Instantiate(currentWeapon);
            Spawn(nob, _conn, gameObject.scene);

            if(weaponAttachPoint.TryGetComponent(out NetworkBehaviour behaviour))
                nob.SetParent(behaviour);

            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
        } 

        #endregion
    }
}
