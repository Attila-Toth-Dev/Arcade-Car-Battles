using UnityEngine;

using FishNet.Object;
using FishNet.Connection;
using Inspector;

namespace Spawners
{
    public abstract class BaseSpawner : NetworkBehaviour
    {
        private const float ROTATION_SPEED = 1.0f;

        [Header("Properties")]
        [SerializeField] protected float spawnRate = 1.0f;
        
        [Header("Spawner References")]
        [SerializeField] protected Transform spawnTransform;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float currentSpawnTime;

        [ServerRpc(RequireOwnership = false, RunLocally = false)]
        public virtual void ServerRpc_SpawnObject(NetworkConnection _conn) { }

        public virtual void RotateSpawnedObject() { }
    }
}
