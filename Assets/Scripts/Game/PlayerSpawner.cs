using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using FishNet;
using FishNet.Object;
using FishNet.Managing;
using FishNet.Connection;

using Inspector;

namespace Game
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;

        [Header("Settings")]
        [SerializeField] private int requiredPlayerCount;

        [Header("Debugging")]
        [SerializeField, ReadOnly]private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = FindFirstObjectByType<NetworkManager>();

            if (networkManager == null)
                networkManager = InstanceFinder.NetworkManager;

            if(networkManager == null)
            {
                Debug.LogWarning($"Player Spaenrt cannot work as a NetworkManager couldn't be found.");
                return;
            }

            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
        }

        private void OnDestroy()
        {
            if(networkManager != null)
                networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
        }

        private void OnClientLoadedStartScenes(NetworkConnection _conn, bool _asServer)
        {
            if (!_asServer)
                return;

            List<NetworkConnection> authenticatedClients = networkManager.ServerManager.Clients.Values
                .Where(conn => conn.IsAuthenticated).ToList();

            if (authenticatedClients.Count < requiredPlayerCount)
                return;

            foreach(NetworkConnection client in authenticatedClients)
            {
                Transform spawnPosition = spawnPoints[client.ClientId % spawnPoints.Length];

                NetworkObject obj = Instantiate(playerPrefab, spawnPosition);
                networkManager.ServerManager.Spawn(obj, client);

                if (!client.Scenes.Contains(gameObject.scene))
                    networkManager.SceneManager.AddConnectionToScene(client, gameObject.scene);
            }
        }
    }
}
