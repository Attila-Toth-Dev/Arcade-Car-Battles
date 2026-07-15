using FishNet.Connection;
using UnityEngine;

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

            ServerRpc_SpawnObject(LocalConnection);
        }

        public override void ServerRpc_SpawnObject(NetworkConnection _conn)
        {
            Debug.Log($"Connection {_conn.ClientId} is spawning an object.");
        }

        private void Update()
        {
            RotateSpawnedObject();
        }
    }
}
