using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MillionManager : MonoBehaviour {
    [Header("Giao diện & Thống kê")]
    public TextMeshProUGUI counterText;
    
    [Header("Cài đặt Quân đoàn")]
    [Range(1000, 100000000)] public int maxCount = 1000000;
    public float spawnRate = 5000f; // Số quái xuất hiện mỗi giây
    public float moveSpeed = 8f;

    [Header("Tài nguyên (Assets)")]
    public ComputeShader computeShader;
    public Mesh enemyMesh;
    public Material material;

    [Header("Vị trí chiến trường")]
    public Transform[] portals;    // Danh sách các cổng không gian
    public Transform targetTransform; // Mục tiêu di động

    private int currentDisplayCount = 0;
    private ComputeBuffer dataBuffer;
    private GraphicsBuffer argsBuffer;
    private RenderParams rp;
    private uint[] args = new uint[5];
    private float spawnAccumulator = 0;

    // Cấu trúc dữ liệu khớp 100% với Shader
    struct EnemyData {
        public Vector3 pos;
        public Vector3 vel;
        public float life;
    }

    void Start() {
        if (portals == null || portals.Length == 0 || targetTransform == null) {
            Debug.LogError("Thiếu Portal hoặc Target!");
            return;
        }

        // 1. Khởi tạo Buffer trên GPU (28 bytes mỗi quái)
        dataBuffer = new ComputeBuffer(maxCount, 28);
        EnemyData[] initialData = new EnemyData[maxCount];

        // Khởi tạo toàn bộ quái nằm ẩn tại cổng đầu tiên
        Vector3 startPos = portals[0].position;
        for (int i = 0; i < maxCount; i++) {
            initialData[i].pos = new Vector3(startPos.x, -100f, startPos.z); // Để dưới lòng đất cho đến khi được spawn
            initialData[i].vel = Vector3.zero;
            initialData[i].life = 0;
        }
        dataBuffer.SetData(initialData);

        // 2. Thiết lập Render Params (Vẽ gián tiếp)
        material.SetBuffer("_Buffer", dataBuffer);
        rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, Vector3.one * 20000f); // Tầm nhìn cực rộng

        // 3. Chuẩn bị Args Buffer
        argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, 5 * sizeof(uint));
        args[0] = enemyMesh.GetIndexCount(0);
        args[1] = 0; // Bắt đầu từ 0
        argsBuffer.SetData(args);
    }

    void Update() {
        if (dataBuffer == null || portals.Length == 0) return;

        // --- LOGIC SINH QUÁI TỪ NHIỀU CỔNG ---
        if (currentDisplayCount < maxCount) {
            spawnAccumulator += spawnRate * Time.deltaTime;
            int countToSpawn = Mathf.FloorToInt(spawnAccumulator);
            
            if (countToSpawn > 0) {
                // Lấy dữ liệu cũ để cập nhật vị trí cho quái mới xuất hiện
                // (Để tối ưu tuyệt đối, ta chỉ cần Reset vị trí trong Compute Shader dựa trên 'life')
                spawnAccumulator -= countToSpawn;
                currentDisplayCount = Mathf.Min(currentDisplayCount + countToSpawn, maxCount);
                
                args[1] = (uint)currentDisplayCount;
                argsBuffer.SetData(args);
            }
        }

        // --- CẬP NHẬT UI ---
        if (counterText != null) {
            counterText.text = $"Horde: {currentDisplayCount:N0}";
        }

        // --- CHẠY GPU COMPUTING ---
        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernel, "_Buffer", dataBuffer);
        computeShader.SetVector("_TargetPos", targetTransform.position);
        
        // Gửi vị trí cổng ngẫu nhiên vào GPU (để quái mới sinh ra đúng chỗ)
        Vector3 randomPortalPos = portals[Random.Range(0, portals.Length)].position;
        computeShader.SetVector("_PortalPos", randomPortalPos);
        
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        computeShader.SetFloat("_MoveSpeed", moveSpeed);
        computeShader.SetInt("_MaxCount", maxCount);
        computeShader.SetInt("_CurrentCount", currentDisplayCount);

        // Chia luồng thông minh vượt giới hạn 65535
        int totalGroups = Mathf.CeilToInt(currentDisplayCount / 64f);
        int xGroups = Mathf.Min(totalGroups, 65535);
        int yGroups = Mathf.Max(1, Mathf.CeilToInt((float)totalGroups / 65535f));
        computeShader.Dispatch(kernel, xGroups, yGroups, 1);

        // --- RENDER ---
        Graphics.RenderMeshIndirect(rp, enemyMesh, argsBuffer);
    }

    void OnDestroy() {
        dataBuffer?.Release();
        argsBuffer?.Release();
    }
}