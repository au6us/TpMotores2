using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Configuración")]
    [SerializeField] string _androidGameId = "TU_GAME_ID_AQUI"; // <--- OJO ACÁ: Pone tu ID del Dashboard
    [SerializeField] bool _testMode = true;

    [Header("IDs de Anuncios (Android)")]
    private string _bannerId = "Banner_Android";
    private string _interstitialId = "Interstitial_Android";
    private string _rewardedId = "Rewarded_Android";

    public static AdsManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAds()
    {
        if (string.IsNullOrEmpty(_androidGameId)) _androidGameId = "1234567";

        Advertisement.Initialize(_androidGameId, _testMode, this);
    }

    // --- 1. BANNER (Menú Principal) ---
    public void ShowBanner()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(_bannerId);
        Advertisement.Banner.Show(_bannerId);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    // --- 2. INTERSTITIAL (Al Morir) ---
    public void ShowInterstitial()
    {
        Debug.Log("Cargando Interstitial...");
        Time.timeScale = 0f;
        Advertisement.Load(_interstitialId, this);
    }

    // --- 3. REWARDED (Stamina/Vida) ---
    public void ShowRewarded()
    {
        Debug.Log("Cargando Rewarded...");
        Advertisement.Load(_rewardedId, this);
    }

    
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Inicializado.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Error Init Ads: {error} - {message}");
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log($"Anuncio cargado: {adUnitId}");

        if (adUnitId.Equals(_interstitialId))
        {
            Advertisement.Show(_interstitialId, this);
        }
        if (adUnitId.Equals(_rewardedId))
        {
            Advertisement.Show(_rewardedId, this);
        }
    }

   
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error cargando Ad {adUnitId}: {error} - {message}");

        if (adUnitId.Equals(_interstitialId))
        {
            Time.timeScale = 1f;
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        if (adUnitId.Equals(_interstitialId))
        {
            Time.timeScale = 1f; 
        }
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        
        if (adUnitId.Equals(_interstitialId))
        {
            Debug.Log("Interstitial cerrado, reanudando el juego...");
            Time.timeScale = 1f; 
        }

        if (adUnitId.Equals(_rewardedId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("¡Premio Otorgado!");
        }
    }
}