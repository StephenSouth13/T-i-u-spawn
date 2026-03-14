using UnityEngine;

public class MegaSpawner : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int count = 10000;
    
    private Vector3[] positions;
    private Vector3[] velocities;
    private Matrix4x4[] matrices;

    void Start() {
        positions = new Vector3[count];
        velocities = new Vector3[count];
        matrices = new Matrix4x4[1023]; // Vẽ theo cụm

        for (int i = 0; i < count; i++) {
            positions[i] = new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
            velocities[i] = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * 2f;
        }
    }

    void Update() {
        RenderParams rp = new RenderParams(material);

        for (int i = 0; i < count; i++) {
            // 1. Chạy: Di chuyển vị trí
            positions[i] += velocities[i] * Time.deltaTime;

            // 2. Va chạm tường (Giữ chúng trong vùng 40x40)
            if (Mathf.Abs(positions[i].x) > 20) velocities[i].x *= -1;
            if (Mathf.Abs(positions[i].z) > 20) velocities[i].z *= -1;

            // 3. Fake va chạm giữa các khối (Chỉ tính với vài khối lân cận để tránh lag)
            // Nếu bạn muốn 1 triệu khối va chạm thật, phải dùng Compute Shader
        }

        // 4. Vẽ ra màn hình
        int drawn = 0;
        while (drawn < count) {
            int batch = Mathf.Min(count - drawn, 1023);
            for (int j = 0; j < batch; j++) {
                matrices[j] = Matrix4x4.TRS(positions[drawn + j], Quaternion.identity, Vector3.one * 0.5f);
            }
            Graphics.RenderMeshInstanced(rp, mesh, 0, matrices, batch);
            drawn += batch;
        }
    }
}