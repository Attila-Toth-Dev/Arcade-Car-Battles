using UnityEngine;

using FishNet.Object;

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
            base.OnStartServer();

            ServerRpc_SpawnObject();
        }

        public override void ServerRpc_SpawnObject()
        {
            NetworkObject nob = Instantiate(weaponToSpawn);
            ServerManager.Spawn(nob);

            if(SpawnTransform.TryGetComponent(out NetworkBehaviour spawn))
                nob.SetParent(spawn);

            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            RotateSpawnedObject();
        }

        private void OnTriggerEnter(Collider _other)
        {
            if(_other.GetComponentInParent<WeaponController>())
            {
                WeaponController controller = _other.GetComponentInParent<WeaponController>();

                //if (controller.CurrentWeapon != null || controller.CurrentWeapon == weaponToSpawn.GetCurrentWeaponType<BaseWeapon>(weaponToSpawn))
                //    return;
                //else
                    controller.ServerRpc_SpawnWeapon(controller.LocalConnection, weaponToSpawn);
            }
        }
    }
}
