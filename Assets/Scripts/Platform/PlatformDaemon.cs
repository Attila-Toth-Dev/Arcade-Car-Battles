using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using FishNet.Managing.Transporting;
using FishNet.Transporting.Multipass;

using Inspector;

namespace Platform
{
    public class PlatformDaemon : MonoBehaviour
    {
        #region Properties
    
        public static PlatformDaemon Instance
        {
            get;
            private set;
        }
    
        public IPlatformService Platform
        {
            get;
            private set;
        }
    
        #endregion
    
        [Header("Startup Event")]
        [SerializeField] private UnityEvent startupEvent;
    
        [Header("References")]
        [SerializeField] private TransportManager transportManager;
    
        [Header("Debugging")]
        [SerializeField, ReadOnly] private string currentPlatform;
        [SerializeField, ReadOnly] private string currentSystem;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
    
                DontDestroyOnLoad(this);
            }
    
            currentSystem = SystemInfo.operatingSystemFamily.ToString();
    
            Multipass mp = transportManager.GetTransport<Multipass>();

#if (WINDOWS_STEAM_RELEASE || MACOS_STEAM_RELEASE || LINUX_STEAM_RELEASE) && APP4346940
                mp.SetClientTransport<FishySteamworks.FishySteamworks>();
    
                Platform = new SteamPlatformService(mp.GetTransport<FishySteamworks.FishySteamworks>());
    
                Platform.Service = PlatformService.Steam;
                currentPlatform = Platform.Service.ToString();

#elif IOS_RELEASE || ANDROID_RELEASE
    
#elif BASE_RELEASE
                mp.SetClientTransport<FishNet.Transporting.Tugboat>();
    
                Platform = new BasePlatformService(mp.GetTransport<FishNet.Transporting.Tugboat>());
    
                Platform.Service = PlatformService.None;
                currentPlatform = Platform.Service.ToString();
    
#endif
            if (Platform.GetType() == null)
                return;
    
            if (Platform.InitialisePlatform())
            {
                Debug.Log($"Platform has initialised, continuing startup process.");
                startupEvent?.Invoke();
    
                SceneManager.LoadScene("Main_Menu_Scene");
            }
        }
    }
}