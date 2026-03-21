using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float updateInterval = 0.5f; // Cập nhật mỗi 0.5 giây
    
    private float accum = 0; 
    private int frames = 0; 
    private float timeleft; 

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            float fps = accum / frames;
            string format = string.Format("{0:F0} FPS", fps);
            fpsText.text = format;

            // Đổi màu theo hiệu năng
            if (fps < 30) fpsText.color = Color.red;
            else if (fps < 60) fpsText.color = Color.yellow;
            else fpsText.color = Color.green;

            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}