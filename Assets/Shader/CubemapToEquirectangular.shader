Shader "Hidden/CubemapToEquirectangular"
{
    Properties
    {
        _Cubemap ("Cubemap", CUBE) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Cubemap;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv :  TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 EquirectangularToCubemap(float2 uv)
            {
                // UV座標をパノラマの角度に変換
                float theta = uv.y * UNITY_PI; // 0 to PI (上下)
                float phi = uv.x * UNITY_PI * 2.0; // 0 to 2PI (左右)
                
                // 球面座標からカルテシアン座標へ
                float3 dir;
                dir.x = sin(theta) * cos(phi);
                dir.y = cos(theta);
                dir.z = sin(theta) * sin(phi);
                
                return normalize(dir);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = EquirectangularToCubemap(i.uv);
                return texCUBE(_Cubemap, dir);
            }
            ENDCG
        }
    }
}