using System;

namespace Platform
{
    public class AndroidPlatformService : IPlatformService
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

        public AndroidPlatformService()
        {

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
