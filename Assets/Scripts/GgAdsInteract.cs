using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Sample;
using System;

public class GoogleAdsInteract : MonoBehaviour
{
    public BannerViewController bannerController;
    public MillionManager millionManager;

#if UNITY_ANDROID
    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
    private string rewardedAdUnitId = "unused";
#endif

    private RewardedAd rewardedAd;

    void Awake()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads initialized.");
        });
    }

    void Start()
    {
        // Banner
        if (bannerController != null)
        {
            bannerController.CreateBannerView();
            bannerController.LoadAd();
        }

        // Rewarded
        LoadRewardedAd();
    }

    // ========================
    // Banner
    // ========================

    public void ShowBanner() => bannerController?.ShowAd();
    public void HideBanner() => bannerController?.HideAd();

    // ========================
    // Rewarded Ads Logic
    // ========================

    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad...");
        AdRequest adRequest = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }
            Debug.Log("Rewarded ad loaded.");
            rewardedAd = ad;
            RegisterRewardedEvents(ad);
        });
    }

    // CHỈ GIỮ LẠI MỘT HÀM SHOW DUY NHẤT CÓ ĐỔI MÀU
    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("User earned reward: " + reward.Amount);

                // --- PHẦN THƯỞNG ĐỔI MÀU QUÂN ĐOÀN ---
                if (millionManager != null)
                {
                    Color randomColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    millionManager.SetEnemyColor(randomColor);
                }
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready. Loading a new one...");
            LoadRewardedAd();
        }
    }

    private void RegisterRewardedEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed.");
            LoadRewardedAd(); // Load cái mới để lần sau xem tiếp
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open: " + error);
        };
    }
}