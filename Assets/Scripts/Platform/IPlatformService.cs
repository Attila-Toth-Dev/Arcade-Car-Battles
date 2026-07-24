using System;

namespace Platform
{
    public enum LobbyType
    {
        Private = 0,
        FriendsOnly = 1,
        Public = 2,
        Invisible = 3,
        PrivateUnique = 4
    }

    public enum PlatformService
    {
        None = 0,
        Steam = 1,
        Xbox = 2,
        Switch = 3,
        PlayStation = 4
    }

    public interface IPlatformService
    {
        PlatformService Service { get; set; }

        event Action<ulong> LobbyCreated;
        event Action<ulong> LobbyEntered;
        event Action<ulong> JoinRequest;

        bool IsPlatformRunning();
        bool InitialisePlatform();


        // Lobby Functions
        void CreateLobby(LobbyType _type, int _maxPlayers);
        void LeaveLobby(ulong _id);
        void DeleteLobby(ulong _id, string _key);
        bool RequestLobby(ulong _id);
        void JoinLobby(ulong _id);
    }
}
