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

            if(IsServerStarted)
                ServerRpc_SpawnWeapon(weaponToSpawn);
        }

        private void Update()
        {
            SpawnTimer.Update();

            CurrentSpawnTime = SpawnTimer.Remaining;

            RotateSpawnedObject();
        }

        private void OnTriggerEnter(Collider _other)
        {
            if (CurrentSpawnTime > 0)
                return;

            NetworkObject weapon = SpawnTransform.GetComponentInChildren<NetworkObject>();            
            ServerManager.Despawn(weapon, DespawnType.Destroy);

            SpawnTimer.StartTimer(Cooldown);
            SpawnTimer.OnChange += OnTimerChange;
        }

        private void OnTimerChange(SyncTimerOperation _operation, float _prev, float _next, bool _asServer)
        {
            if (_operation == SyncTimerOperation.Finished || _operation == SyncTimerOperation.Complete)
                ServerRpc_SpawnWeapon(weaponToSpawn);
        }
    }
}
