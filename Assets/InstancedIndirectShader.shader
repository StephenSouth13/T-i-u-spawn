Shader "Custom/InstancedIndirect"
{
    Properties { 
        _Color("Color", Color) = (1,1,1,1) 
        _Scale("Model Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Cấu trúc này PHẢI khớp 100% với file .cs và .compute
            struct CubeData { 
                float3 position; 
                float3 velocity; 
                float life; 
            };

            StructuredBuffer<CubeData> _Buffer;
            float4 _Color;
            float _Scale;

            struct Attributes { 
                float4 positionOS : POSITION; 
            };

            struct Varyings { 
                float4 positionCS : SV_POSITION; 
                float4 color : COLOR; 
            };

            Varyings vert(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings OUT;
                
                // 1. Lấy dữ liệu từ Buffer
                float3 worldPos = _Buffer[instanceID].position;
                float3 vel = _Buffer[instanceID].velocity;

                // 2. Tạo ma trận xoay (Để model nhìn về hướng velocity)
                // Nếu quái đứng im thì mặc định nhìn về phía trước (0,0,1)
                float3 forward = normalize(vel + float3(0, 0, 0.00001)); 
                float3 up = float3(0, 1, 0);
                float3 right = normalize(cross(up, forward));
                up = cross(forward, right);
                
                float3x3 rotationMatrix = float3x3(right, up, forward);

                // 3. Tính toán vị trí đỉnh: Xoay -> Scale -> Dịch chuyển
                float3 localPos = IN.positionOS.xyz * _Scale;
                float3 rotatedPos = mul(localPos, rotationMatrix); // Xoay model
                float3 finalWorldPos = rotatedPos + worldPos;

                OUT.positionCS = TransformWorldToHClip(finalWorldPos);
                OUT.color = _Color;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target 
            { 
                return IN.color; 
            }
            ENDHLSL
        }
    }
}