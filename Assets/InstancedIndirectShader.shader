Shader "Custom/InstancedIndirect"
{
    Properties { 
        _Color("Horde Color", Color) = (1,1,1,1) 
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

            // Cấu trúc dữ liệu khớp với Compute Shader
            struct CubeData { 
                float3 position; 
                float3 velocity; 
                float life; 
            };

            // Buffer chứa 10 triệu thực thể
            StructuredBuffer<CubeData> _Buffer;
            
            // Các biến điều khiển từ C# (GoogleAds sẽ thay đổi _Color)
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
                
                // 1. Lấy dữ liệu từ GPU Buffer
                float3 worldPos = _Buffer[instanceID].position;
                float3 vel = _Buffer[instanceID].velocity;

                // 2. Tạo ma trận xoay nhìn về hướng di chuyển (Look Rotation)
                float3 forward = normalize(vel + float3(0, 0, 0.00001)); 
                float3 up = float3(0, 1, 0);
                float3 right = normalize(cross(up, forward));
                up = cross(forward, right);
                
                float3x3 rotationMatrix = float3x3(right, up, forward);

                // 3. Transform: Scale -> Rotate -> Translate
                float3 localPos = IN.positionOS.xyz * _Scale;
                float3 rotatedPos = mul(localPos, rotationMatrix); 
                float3 finalWorldPos = rotatedPos + worldPos;

                OUT.positionCS = TransformWorldToHClip(finalWorldPos);
                
                // Gán màu đã được cập nhật từ GoogleAdsInteract
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