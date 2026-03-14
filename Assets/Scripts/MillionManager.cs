using UnityEngine;
using TMPro; // Thêm thư viện này để dùng TextMeshPro

public class MillionManager : MonoBehaviour {
    [Header("UI Reference")]
    public TextMeshProUGUI counterText; // Kéo cái EnemyCounter vào đây

    [Header("Settings")]
    public ComputeShader computeShader;
    public Mesh mesh;
    public Material material;
    public Transform portalTransform;
    public Transform targetTransform;
    
    public int maxCount = 10000000; 
    private int currentDisplayCount = 0; 
    public float spawnRate = 10000f; 

    private ComputeBuffer dataBuffer;
    private GraphicsBuffer argsBuffer;
    private RenderParams rp;
    private uint[] args = new uint[5];

    struct CubeData { public Vector3 pos; public Vector3 vel; public float life; }

    void Start() {
        // Giữ nguyên phần khởi tạo như cũ...
        dataBuffer = new ComputeBuffer(maxCount, 28);
        CubeData[] data = new CubeData[maxCount];
        Vector3 pPos = portalTransform.position;
        for (int i = 0; i < maxCount; i++) {
            data[i].pos = pPos;
            data[i].vel = Vector3.zero;
            data[i].life = 0;
        }
        dataBuffer.SetData(data);
        material.SetBuffer("_Buffer", dataBuffer);
        rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
        argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, 5 * sizeof(uint));
        args[0] = mesh.GetIndexCount(0);
        args[1] = 0;
        argsBuffer.SetData(args);
    }

    void Update() {
        // 1. Cập nhật số lượng quái tăng dần
        if (currentDisplayCount < maxCount) {
            currentDisplayCount += (int)(spawnRate * Time.deltaTime);
            currentDisplayCount = Mathf.Min(currentDisplayCount, maxCount);
            
            args[1] = (uint)currentDisplayCount;
            argsBuffer.SetData(args);
        }

        // 2. CẬP NHẬT UI (Mỗi khung hình con số sẽ nhảy lên)
        if (counterText != null) {
            counterText.text = "Enemies: " + currentDisplayCount.ToString("N0"); // N0 để có dấu phẩy ngăn cách hàng nghìn
        }

        // 3. Chạy Compute Shader (Xử lý đa luồng an toàn)
        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernel, "_Buffer", dataBuffer);
        computeShader.SetVector("_TargetPos", targetTransform.position);
        computeShader.SetVector("_PortalPos", portalTransform.position);
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        computeShader.SetFloat("_Time", Time.time);

        // Chia thread groups cho cả X và Y để tránh lỗi vượt quá 65535
        int totalGroups = Mathf.CeilToInt(currentDisplayCount / 64f);
        int xGroups = Mathf.Min(totalGroups, 65535);
        int yGroups = Mathf.Max(1, Mathf.CeilToInt((float)totalGroups / 65535f));

        computeShader.Dispatch(kernel, xGroups, yGroups, 1);

        Graphics.RenderMeshIndirect(rp, mesh, argsBuffer);
    }

    void OnDestroy() {
        dataBuffer?.Release();
        argsBuffer?.Release();
    }
}