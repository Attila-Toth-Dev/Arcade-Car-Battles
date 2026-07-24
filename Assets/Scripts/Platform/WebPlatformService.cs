using System;

using FishNet.Transporting.Bayou;

namespace Platform
{
    public class WebPlatformService : IPlatformService
    {
        #region Properties
        
        public PlatformService Service
        {
            get;
            set;
        } 

        #endregion

        public event Action<ulong> LobbyCreated;
        public event Action<ulong> LobbyEntered;
        public event Action<ulong> JoinRequest;

        private Bayou bayou;

        public WebPlatformService(Bayou _bayou)
        {
            bayou = _bayou;
        }

        public void CreateLobby(LobbyType _type, int _maxPlayers)
        {
        }

        public void DeleteLobby(ulong _id, string _key)
        {
        }

        public bool InitialisePlatform()
        {
            return true;
        }

        public bool IsPlatformRunning()
        {
            return true;
        }

        public void JoinLobby(ulong _id)
        {
        }

        public void LeaveLobby(ulong _id)
        {
        }

        public bool RequestLobby(ulong _id)
        {
            return true;
        }
    }
}
