using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;

using Weapons;
using Controllers;

namespace Spawners
{
    public class WeaponSpawner : BaseSpawner
    {
        [Header("Weapon References")]
        [SerializeField] private BaseWeapon weaponToSpawn;

        public override void OnStartClient()
        {
            base.OnStartClient();

            SpawnTimer.OnChange += OnTimerChange;

            if (IsServerStarted || IsHostStarted)
                ServerRpc_SpawnWeapon(weaponToSpawn);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            SpawnTimer.OnChange -= OnTimerChange;
        }

        private void Update()
        {
            SpawnTimer.Update();

            DebugSpawnTime = SpawnTimer.Remaining;
            DebugIsItemSpawned = IsItemSpawned.Value;

            RotateSpawnedObject();
        }

        private void OnTriggerEnter(Collider _other)
        {
            WeaponController controller = _other.GetComponentInParent<WeaponController>();
            
            if (!IsItemSpawned.Value)
            {
                Debug.Log($"Returning early as {this.GetType()} spawner has no child objects.");
                return;
            }
            else if(controller.CurrentWeapon.GetType().IsInstanceOfType(weaponToSpawn) && controller.CurrentWeapon != null)
            {
                Debug.Log($"Returning early as car's current weapon is the same as weapon in spawner.");
                return;
            }

            if (controller != null)
            {
                NetworkObject nob = controller.GetComponent<NetworkObject>();
                controller.ServerRpc_SpawnWeapon(nob.LocalConnection, weaponToSpawn);
            }

            NetworkObject weapon = SpawnTransform.GetComponentInChildren<NetworkObject>();
            ServerRpc_DespawnWeapon(weapon);

            ServerRpc_StartTimer(Cooldown);
        }

        #region Timer Functions

        public virtual void OnTimerChange(SyncTimerOperation _operation, float _prev, float _next, bool _asServer)
        {
            if (!_asServer)
                return;

            if (_operation == SyncTimerOperation.Start)
                IsItemSpawned.Value = false;

            if (_operation == SyncTimerOperation.Finished)
                ServerRpc_SpawnWeapon(weaponToSpawn);
        } 

        #endregion
    }
}
