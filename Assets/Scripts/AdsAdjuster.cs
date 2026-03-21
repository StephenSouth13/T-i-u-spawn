using UnityEngine;
using GoogleMobileAds.Api;

public class AdsAdjuster : MonoBehaviour 
{
    // Hàm chuyển Banner lên trên (Top)
    public void SetTop() {
        RefreshBanner(AdSize.Banner, AdPosition.Top);
    }

    // Hàm chuyển Banner xuống dưới (Bottom)
    public void SetBottom() {
        RefreshBanner(AdSize.Banner, AdPosition.Bottom);
    }

    // Hàm phóng to quảng cáo (Dạng khối hình chữ nhật)
    public void SetBigSize() {
        RefreshBanner(AdSize.MediumRectangle, AdPosition.Center);
    }
    public void CloseBanner() {
        if (AdsManager.Instance != null) {
            Debug.Log("Đang tắt quảng cáo...");
            AdsManager.Instance.DestroyBanner(); 
        }
    }

    private void RefreshBanner(AdSize size, AdPosition pos) {
        if (AdsManager.Instance != null) {
            AdsManager.Instance.DestroyBanner(); 
            AdsManager.Instance.CreateNewCustomBanner(size, pos);
        }
    }
}