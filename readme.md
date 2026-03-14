# 🌌 Infinity Horde — GPU Massive Swarm System

> **GPU-driven swarm simulation capable of rendering tens of millions of entities in real-time.**

---

# 🎬 Demo

<p align="center">
  <video src="img&video/demo.mp4" controls width="600"></video>
</p>

---

# 📊 Performance

| Unity Profiler | System Monitor |
|---|---|
| <img src="media/profiler.png" width="520"> | <img src="media/taskmanager_on_pc.png" width="520"> |

---

# 🚀 Overview

| English | Tiếng Việt |
|---|---|
| **Infinity Horde** is a **GPU-driven swarm simulation system** designed to render and simulate millions of entities in real time. | **Infinity Horde** là **hệ thống mô phỏng bầy đàn chạy trên GPU** cho phép hiển thị và xử lý hàng triệu thực thể thời gian thực. |
| Instead of traditional **GameObject CPU loops**, the system moves simulation logic to **GPU Compute Shaders**. | Thay vì vòng lặp **GameObject trên CPU**, hệ thống chuyển toàn bộ logic mô phỏng sang **GPU Compute Shader**. |
| Entity data is stored inside **GPU buffers** and rendered using **indirect instancing**. | Dữ liệu thực thể được lưu trong **GPU buffer** và render bằng **indirect instancing**. |
| Result: **extreme scale with minimal CPU usage**. | Kết quả: **quy mô cực lớn với CPU gần như không tải**. |

---

# ⚡ Core Features

| Feature | Description |
|---|---|
| Massive Scale | Supports **1M → 100M+ entities** depending on GPU VRAM |
| GPU Simulation | Movement computed using **Compute Shaders** |
| Multi-Portal Spawn | Multiple spawn portals simultaneously |
| Infinite Persistence | Entities remain active once spawned |
| Ground Lock | Stable movement constrained to terrain plane |
| Single Draw Call | Entire swarm rendered using **one draw call** |

---

# 🧠 Architecture

Infinity Horde uses a **GPU-first simulation pipeline** to eliminate CPU bottlenecks.


CPU
↓
Spawn Parameters
↓
Compute Shader Simulation
↓
Structured GPU Buffers
↓
RenderMeshIndirect
↓
GPU Rendering


---

# 🧮 Movement Model

Velocity update:


Velocity = lerp(CurrentVel, TargetDir * Speed, Δt)


Position update:


Position = Position + Velocity * Δt


### Benefits

- No PhysX overhead  
- Fully parallel GPU compute  
- Cache-friendly memory access  

---

# 🧩 Data Structure

```hlsl
struct EnemyData
{
    float3 position;
    float3 velocity;
    float lifetime;
};

Stored in GPU memory using:

StructuredBuffer<EnemyData>
📈 Performance
Entity Count	FPS	CPU Load
100K	200+	~1%
1M	150+	~2%
10M	90+	~3%
50M+	GPU dependent	~3%
⚙️ Requirements
Component	Requirement
Unity	2021.3 LTS or newer
Render Pipeline	Universal Render Pipeline (URP)
GPU	Compute Shader support
Graphics API	DX11 / DX12 / Vulkan / Metal
Memory	16GB+ recommended
🧪 Usage

Assign Enemy Mesh

Assign Instanced Material

Configure Spawn Portals

Assign Target Object

Adjust simulation parameters

SpawnRate
MaxEntityCount
MovementSpeed

Run the scene → the swarm simulation begins.

🔬 Use Cases
English	Tiếng Việt
Massive zombie survival games	Game sinh tồn zombie quy mô lớn
RTS army simulations	Mô phỏng quân đội RTS
GPU rendering research	Nghiên cứu render GPU
Graphics pipeline stress testing	Stress test pipeline đồ họa
Procedural crowd systems	Hệ thống đám đông procedural
👨‍💻 Author

Quách Thành Long
Game Developer • System Architect • AI Engineer

🌐 Website
https://quachthanhlong.com

📧 Email
stephensouth1307@gmail.com

🏫 VTC Academy — Game Development Department

📜 License
MIT License

© 2026 Infinity Horde — Engineered by Quách Thành Long

When CPU reaches its limits, the GPU leads the swarm.