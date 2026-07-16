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
            base.OnStartServer();

            SpawnTimer.OnChange += OnTimerChange;

            if (IsServerStarted)
                SpawnTimer.StartTimer(Cooldown);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            SpawnTimer.OnChange -= OnTimerChange;
        }

        private void Update()
        {
            SpawnTimer.Update();

            if (SpawnTimer.Remaining > 0)
                CanSpawn.Value = false;
            else
                CanSpawn.Value = true;

            DebugSpawnTime = SpawnTimer.Remaining;
            DebugCanSpawn = CanSpawn.Value;

            RotateSpawnedObject();
        }

        private void OnTriggerEnter(Collider _other)
        {
            NetworkObject weapon = SpawnTransform.GetComponentInChildren<NetworkObject>();
            ServerRpc_DespawnWeapon(weapon);
        }

        #region Timer Functions
        
        public virtual void OnTimerChange(SyncTimerOperation _operation, float _prev, float _next, bool _asServer)
        {
            if (_operation == SyncTimerOperation.Finished)
                ServerRpc_SpawnWeapon(weaponToSpawn);
        } 

        #endregion
    }
}
