Shader "Custom/InstancedIndirect"
{
    Properties { _Color("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct CubeData { 
                float3 position; 
                float3 velocity; 
            };

            // Đây là cái bồn chứa dữ liệu từ Compute Shader
            StructuredBuffer<CubeData> _Buffer;

            struct Attributes { 
                float4 positionOS : POSITION; 
            };

            struct Varyings { 
                float4 positionCS : SV_POSITION; 
                float4 color : COLOR; 
            };

            float4 _Color;

            Varyings vert(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings OUT;
                // Lấy vị trí của khối thứ "instanceID" trong 10 triệu khối
                float3 worldPos = _Buffer[instanceID].position;
                
                // Di chuyển đỉnh của khối lập phương theo vị trí đó
                float3 finalPos = IN.positionOS.xyz + worldPos;
                
                OUT.positionCS = TransformWorldToHClip(finalPos);
                OUT.color = _Color;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target { 
                return IN.color; 
            }
            ENDHLSL
        }
    }
}