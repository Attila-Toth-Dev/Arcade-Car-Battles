using UnityEngine;

using FishNet.Object;
using FishNet.Object.Synchronizing;

using Inspector;
using Weapons;

namespace Spawners
{
    public abstract class BaseSpawner : NetworkBehaviour
    {
        private const float OBJECT_RATE = 25.0f;

        public readonly SyncTimer SpawnTimer = new SyncTimer();

        [Header("Properties")]
        [SerializeField] protected float Cooldown = 1.0f;
        
        [Header("Spawner References")]
        [SerializeField] protected Transform SpawnTransform;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float CurrentSpawnTime;

        [ServerRpc(RequireOwnership = false, RunLocally = false)]
        public virtual void ServerRpc_SpawnWeapon(BaseWeapon _weapon) 
        {
            NetworkObject nob = Instantiate(_weapon);
            ServerManager.Spawn(nob);

            if (SpawnTransform.TryGetComponent(out NetworkBehaviour spawn))
                nob.SetParent(spawn);

            nob.transform.localPosition = Vector3.zero;
            nob.transform.localRotation = Quaternion.identity;
        }

        public virtual void RotateSpawnedObject() 
        {
            SpawnTransform.Rotate(0.0f, OBJECT_RATE * Time.deltaTime, 0.0f);        
        }
    }
}
