using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;

using Weapons;
using Inspector;
using Controllers;

namespace Spawners
{
    [RequireComponent(typeof(SphereCollider))]
    public abstract class BaseSpawner : NetworkBehaviour
    {
        private const float OBJECT_RATE = 25.0f;

        public readonly SyncTimer SpawnTimer = new SyncTimer();

        public readonly SyncVar<bool> IsItemSpawned = new SyncVar<bool>();

        [Header("Base Spawner Properties")]
        [SerializeField] protected Transform SpawnTransform;
        [SerializeField] protected float Cooldown = 1.0f;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float DebugSpawnTime;
        [SerializeField, ReadOnly] protected bool DebugIsItemSpawned;

        #region RPC Functions
        
        [ServerRpc(RequireOwnership = false)]
        public virtual void ServerRpc_SpawnWeapon(BaseWeapon _weapon)
        {
            NetworkObject nob = NetworkManager.GetPooledInstantiated(_weapon, SpawnTransform.position, SpawnTransform.rotation, asServer: true);
            ServerManager.Spawn(nob);

            if (SpawnTransform.TryGetComponent(out NetworkBehaviour spawn))
            {
                nob.SetParent(spawn);

                nob.transform.localPosition = Vector3.zero;
                nob.transform.localRotation = Quaternion.identity;
                nob.transform.localScale = Vector3.one * 2.5f;
            }

            IsItemSpawned.Value = true;
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void ServerRpc_DespawnCurrentPlayerWeapon(WeaponController _controller)
        {
            ServerManager.Despawn(_controller.CurrentWeapon);
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void ServerRpc_DespawnWeapon(NetworkObject _weapon)
        {
            ServerManager.Despawn(_weapon);
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void ServerRpc_StartTimer(float _cooldown)
        {
            SpawnTimer.StartTimer(_cooldown);
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
