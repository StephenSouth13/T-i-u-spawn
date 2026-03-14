using UnityEngine;

public class MillionManager : MonoBehaviour {
    public ComputeShader computeShader;
    public Mesh mesh;
    public Material material;
    public int count = 10000000; // 10 TRIỆU KHỐI

    private ComputeBuffer dataBuffer;
    private ComputeBuffer argsBuffer;
    private RenderParams rp;

    struct CubeData { public Vector3 pos; public Vector3 vel; }

    void Start() {
        // 1. Khởi tạo dữ liệu trên GPU
        dataBuffer = new ComputeBuffer(count, sizeof(float) * 6);
        CubeData[] data = new CubeData[count];
        for (int i = 0; i < count; i++) {
            data[i].pos = Random.insideUnitSphere * 50f;
            data[i].vel = Random.onUnitSphere * 2f;
        }
        dataBuffer.SetData(data);

        // 2. Thiết lập Material
        material.SetBuffer("_Buffer", dataBuffer);
        rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, Vector3.one * 200f);

        // 3. Buffer phụ để ra lệnh vẽ
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] args = new uint[5] { mesh.GetIndexCount(0), (uint)count, 0, 0, 0 };
        argsBuffer.SetData(args);
    }

    void Update() {
        // Chạy vật lý trên GPU
        int kernel = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernel, "_Buffer", dataBuffer);
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        computeShader.Dispatch(kernel, count / 64, 1, 1);

        // Vẽ 10 triệu khối trong 1 lệnh duy nhất!
        Graphics.RenderMeshIndirect(rp, mesh, argsBuffer);
    }

    void OnDestroy() {
        dataBuffer?.Release();
        argsBuffer?.Release();
    }
}