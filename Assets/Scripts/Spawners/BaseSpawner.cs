using UnityEngine;

using FishNet.Object;

using Inspector;

namespace Spawners
{
    public abstract class BaseSpawner : NetworkBehaviour
    {
        private const float OBJECT_RATE = 25.0f;

        [Header("Properties")]
        [SerializeField] protected float SpawnRate = 1.0f;
        
        [Header("Spawner References")]
        [SerializeField] protected Transform SpawnTransform;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float CurrentSpawnTime;

        [ServerRpc(RequireOwnership = false, RunLocally = false)]
        public virtual void ServerRpc_SpawnObject() 
        { 
        
        }

        public virtual void RotateSpawnedObject() 
        {
            SpawnTransform.Rotate(0.0f, OBJECT_RATE * Time.deltaTime, 0.0f);        
        }
    }
}
