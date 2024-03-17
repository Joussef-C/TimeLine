Shader "Unlit/Gradient with Rounded Edges"
{
    Properties {
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.1
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _TopColor;
            fixed4 _BottomColor;
            float _Radius;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = lerp(_BottomColor, _TopColor, i.uv.y);

                // Calculate the distance from the edges of the UI element
                float2 edges = abs(i.uv - 0.5) * 2.0;
                float dist = length(max(edges, 0.0)) - 1.0 + _Radius;

                // Apply a smoothstep function to create rounded edges
                float alpha = smoothstep(0.0, _Radius, dist);

                // Multiply the color by the alpha value
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}