Shader "UI/RoundedCorners"
{
    Properties
    {
        _Radius ("Corner Radius", Range(0, 1)) = 0.5
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Overlay"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Radius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Example: Apply radius-based corner mask
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                col.a *= smoothstep(_Radius, _Radius - 0.1, dist); // Adjust edge smoothness
                return col;
            }
            ENDCG
        }
    }
}