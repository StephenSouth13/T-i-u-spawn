using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Sample;
using System;

public class GoogleAds : MonoBehaviour
{
    public BannerViewController bannerController;

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
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Google Mobile Ads initialized");
        });
    }

    void Start()
    {
        // Banner setup
        if (bannerController != null)
        {
            bannerController.CreateBannerView();
            bannerController.LoadAd();
        }

        // Load rewarded ad
        LoadRewardedAd();
    }

    // ========================
    // Banner
    // ========================

    public void ShowBanner()
    {
        if (bannerController != null)
            bannerController.ShowAd();
    }

    public void HideBanner()
    {
        if (bannerController != null)
            bannerController.HideAd();
    }

    // ========================
    // Rewarded Ads
    // ========================

    public void LoadRewardedAd()
    {
        Debug.Log("Loading Rewarded Ad");

        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load: " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded");

                rewardedAd = ad;

                // Khi quảng cáo đóng → load quảng cáo mới
                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad closed");

                    rewardedAd.Destroy();
                    rewardedAd = null;

                    LoadRewardedAd();
                };

                rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
                {
                    Debug.LogError("Rewarded ad failed to show: " + adError);
                };
            });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("User earned reward: " + reward.Amount);

                // Ví dụ: cộng thưởng cho player
                // PlayerCoins += reward.Amount;
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready");
        }
    }
}