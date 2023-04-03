Shader "Unlit/DoG"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Matrix1[225]; // Define a matrix with maximum size 15x15
            float _Matrix2[225]; // Define a matrix with maximum size 15x15
            float _Kernel_Radius;

            float3 convolution(float2 uv) {
                float3 color1 = float3(0,0,0);
                float3 color2 = float3(0,0,0);
                for (int y = 0; y < _Kernel_Radius * 2 + 1; y++)
                {
                    for (int x = 0; x < _Kernel_Radius * 2 + 1; x++)
                    {
                        float pixelPosX = _MainTex_TexelSize.z * (uv.x);
                        float pixelPosY = _MainTex_TexelSize.w * (uv.y);
                        float u = float(pixelPosX + x - _Kernel_Radius) / float(_MainTex_TexelSize.z); // width
                        float v = float(pixelPosY + y - _Kernel_Radius) / float(_MainTex_TexelSize.w); // height

                        u = clamp(u, 0.0, 1.0);
                        v = clamp(v, 0.0, 1.0);
                        color1.x = color1.x + tex2D(_MainTex, float2(u, v)).x * _Matrix1[(y) * (_Kernel_Radius * 2 + 1) + x];
                        color1.y = color1.y + tex2D(_MainTex, float2(u, v)).y * _Matrix1[(y) * (_Kernel_Radius * 2 + 1) + x];
                        color1.z = color1.z + tex2D(_MainTex, float2(u, v)).z * _Matrix1[(y) * (_Kernel_Radius * 2 + 1) + x];

                        color2.x = color2.x + tex2D(_MainTex, float2(u, v)).x * _Matrix2[(y) * (_Kernel_Radius * 2 + 1) + x];
                        color2.y = color2.y + tex2D(_MainTex, float2(u, v)).y * _Matrix2[(y) * (_Kernel_Radius * 2 + 1) + x];
                        color2.z = color2.z + tex2D(_MainTex, float2(u, v)).z * _Matrix2[(y) * (_Kernel_Radius * 2 + 1) + x];
                    };
                };
                return color1 - color2;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 color = convolution(i.uv);

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
