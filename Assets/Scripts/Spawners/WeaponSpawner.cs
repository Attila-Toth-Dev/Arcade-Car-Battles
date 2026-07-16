using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;

using Weapons;

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
            if (!IsItemSpawned.Value)
            {
                Debug.Log($"Returning early as {this.GetType()} spawner has no child objects.");
                return;
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
