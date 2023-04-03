Shader "Unlit/WeightedMedianFilter"
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

            // Gaussian kernel
            static const float3x3 kernel = {
                { 1, 2, 1 },
                { 2, 4, 2 },
                { 1, 2, 1 }
            };

            static float new_kernel_r[9] = {0,0,0,0,0,0,0,0,0};
            static float new_kernel_g[9] = {0,0,0,0,0,0,0,0,0};
            static float new_kernel_b[9] = {0,0,0,0,0,0,0,0,0};

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

            void sort(inout float arr[9])
            {
                for (int i = 1; i < 9; i++)
                {
                    for (int k = i; k > 0; k--)
                    {
                        if (arr[k-1] > arr[k])
                        {
                            arr[k-1] = arr[k];
                        }
                    }
                }
            }

            float3 convolution(float2 uv) 
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        float pixelPosX = _MainTex_TexelSize.z * (uv.x);
                        float pixelPosY = _MainTex_TexelSize.w * (uv.y);
                        float u = float(pixelPosX + x - 1) / float(_MainTex_TexelSize.z); // width
                        float v = float(pixelPosY + y - 1) / float(_MainTex_TexelSize.w); // height
                        u = clamp(u, 0.0, 1.0);
                        v = clamp(v, 0.0, 1.0);
                        new_kernel_r[y*3 + x] = tex2D(_MainTex, float2(u, v)).x * kernel[y][x];
                        new_kernel_g[y*3 + x] = tex2D(_MainTex, float2(u, v)).y * kernel[y][x];
                        new_kernel_b[y*3 + x] = tex2D(_MainTex, float2(u, v)).z * kernel[y][x];
                    };
                };
                
                sort(new_kernel_r);
                sort(new_kernel_g);
                sort(new_kernel_b);
                
                return float3(new_kernel_r[4], new_kernel_g[4], new_kernel_b[4]);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = convolution(i.uv);

                return float4(col,1);
            }

            ENDCG
        }
    }

}
