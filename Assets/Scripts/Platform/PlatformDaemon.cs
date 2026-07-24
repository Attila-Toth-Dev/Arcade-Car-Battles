using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using FishNet.Transporting.Tugboat;
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

#if WINDOWS_RELEASE || LINUX_RELEASE
                mp.SetClientTransport<Tugboat>();
    
                Platform = new BasePlatformService(mp.GetTransport<Tugboat>());
    
                Platform.Service = PlatformService.None;
                currentPlatform = Platform.Service.ToString();

#elif IOS_RELEASE || ANDROID_RELEASE

#elif WEB_RELEASE

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