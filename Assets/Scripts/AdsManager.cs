using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

#if UNITY_ANDROID
    string bannerId = "ca-app-pub-3940256099942544/6300978111";
    string interstitialId = "ca-app-pub-3940256099942544/1033173712";
    string rewardedId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    string bannerId = "ca-app-pub-3940256099942544/2934735716";
    string interstitialId = "ca-app-pub-3940256099942544/4411468910";
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";
#endif

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        MobileAds.Initialize((InitializationStatus status) =>
        {
            Debug.Log("AdMob Initialized");

            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });
    }

    // =========================
    // Banner
    // =========================

    void LoadBanner()
    {
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);

        AdRequest request = new AdRequest();

        bannerView.LoadAd(request);
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    // =========================
    // Interstitial
    // =========================

    public void LoadInterstitial()
    {
        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialId, request,
        (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.Log("Interstitial load failed");
                return;
            }

            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed");

                LoadInterstitial();
            };
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready");
        }
    }

    // =========================
    // Rewarded
    // =========================

    public void LoadRewarded()
    {
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedId, request,
        (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.Log("Rewarded load failed");
                return;
            }

            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded closed");

                LoadRewarded();
            };
        });
    }

    public void ShowRewarded(Action rewardAction)
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Reward earned");

                rewardAction?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded not ready");
        }
    }
}