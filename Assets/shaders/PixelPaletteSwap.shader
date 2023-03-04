Shader "Hidden/PixelSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelScale ("Pixel Scale", Integer) = 1
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

            static const float4x4 bayer4 = {
                {0, 8, 2, 10},
                {12, 4, 14, 6},
                {3, 11, 1, 9},
                {15, 7, 13, 5}
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
            float4 _Colors[255];
            float _Colors_Amount;
            int _PixelScale;

            float4 apply_palette (float lum) 
            {
                lum = floor(lum * (_Colors_Amount));
                if (lum >= _Colors_Amount) lum = _Colors_Amount - 1;
                return _Colors[lum];
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float ditherSpread = 1.0/( _Colors_Amount);

                // https://luka712.github.io/2018/07/01/Pixelate-it-Shadertoy-Unity/
                float pixelSizeX = 1.0 / _MainTex_TexelSize.z;
                float pixelSizeY = 1.0 / _MainTex_TexelSize.w;
                float cellSizeX = pow(2.0, float(_PixelScale)) * pixelSizeX;
                float cellSizeY = pow(2.0, float(_PixelScale)) * pixelSizeY;

                // Convert pixel coordinates to cell coordinates
                float u = cellSizeX * floor(i.uv.x / cellSizeX);
                float v = cellSizeY * floor(i.uv.y / cellSizeY);
                

                // get pixel information from the cell coordinates
                fixed4 col = tex2D(_MainTex, float2(u, v));


                // https://en.wikipedia.org/wiki/Ordered_dithering
                int n = 4;
                int x = int(u / cellSizeX) % n;
                int y = int(v / cellSizeY) % n;
                float M = bayer4[x][y] * (1.0/(n*n)) - .5;
                col = col + (M * ditherSpread);
                col = floor(col * (_Colors_Amount-1) + .5)/(_Colors_Amount-1);

                // Calculate luminance
                float lum = (0.299*col.r + 0.587*col.g + 0.114*col.b);

                // Apply the new color palette
                col = apply_palette(lum);

                return col;
            }
            ENDCG
        }
    }
}
