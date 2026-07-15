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
            NetworkObject weapon = SpawnTransform.GetComponentInChildren<NetworkObject>();
            Debug.Log($"{weapon.name}, {weapon.transform.parent}");
            //ServerManager.Despawn(, DespawnType.Destroy);

            //if(_other.GetComponentInParent<WeaponController>())
            //{
            //    WeaponController controller = _other.GetComponentInParent<WeaponController>();
            //    controller.ServerRpc_SpawnWeapon(controller.LocalConnection, weaponToSpawn);
            //}

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
