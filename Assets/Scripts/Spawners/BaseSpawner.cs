using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;

using Weapons;
using Inspector;

namespace Spawners
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class BaseSpawner : NetworkBehaviour
    {
        private const float OBJECT_RATE = 25.0f;

        public readonly SyncTimer SpawnTimer = new SyncTimer();

        public readonly SyncVar<bool> CanSpawn = new SyncVar<bool>();

        [Header("Base Spawner Properties")]
        [SerializeField] protected Transform SpawnTransform;
        [SerializeField] protected float Cooldown = 1.0f;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float DebugSpawnTime;
        [SerializeField, ReadOnly] protected bool DebugCanSpawn;

        #region RPC Functions
        
        [ServerRpc(RequireOwnership = false, RunLocally = false)]
        public virtual void ServerRpc_SpawnWeapon(BaseWeapon _weapon)
        {
            if (CanSpawn.Value)
            {
                Debug.Log($"Spawning new weapon {_weapon.GetType()}");

                NetworkObject nob = Instantiate(_weapon);
                ServerManager.Spawn(nob);

                if (SpawnTransform.TryGetComponent(out NetworkBehaviour spawn))
                    nob.SetParent(spawn);

                nob.transform.localPosition = Vector3.zero;
                nob.transform.localRotation = Quaternion.identity;
            }
        }

        [ServerRpc(RequireOwnership = false, RunLocally = false)]
        public virtual void ServerRpc_DespawnWeapon(NetworkObject _weapon)
        {
            ServerManager.Despawn(_weapon);

            SpawnTimer.StartTimer(Cooldown);
        }

        #endregion

        #region Update Functions
        
        public virtual void RotateSpawnedObject()
        {
            SpawnTransform.Rotate(0.0f, OBJECT_RATE * Time.deltaTime, 0.0f);
        } 

        #endregion
    }
}
