Shader "Unlit/Sharpen"
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

            static const float3x3 kernel = {
                { 0, -1, 0},
                {-1,  4,-1},
                { 0, -1, 0}
            };

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
            float _Scalar;

            float3 convolution(float2 uv) {
                float3x3 scaled_kernel = kernel * _Scalar;
                scaled_kernel[1][1] += 1;
                float3 color = float3(0,0,0);
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
                        color.x = color.x + tex2D(_MainTex, float2(u, v)).x * scaled_kernel[y][x];
                        color.y = color.y + tex2D(_MainTex, float2(u, v)).y * scaled_kernel[y][x];
                        color.z = color.z + tex2D(_MainTex, float2(u, v)).z * scaled_kernel[y][x];
                    };
                };
                return color;
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
