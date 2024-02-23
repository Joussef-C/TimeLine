Shader "Custom/SectioningShader"
{
    Properties
    {
        _SectioningPlane("Sectioning Plane", Vector) = (0,1,0,0)  // Define the sectioning plane (normal)
        _MainTex("Main Texture", 2D) = "white" {} // Assuming the object has a texture, adjust this accordingly if not
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _SectioningPlane;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the object's original color
                half4 originalColor = tex2D(_MainTex, i.uv);

                // Clip fragments based on the sectioning plane
                float distance = dot(i.vertex, _SectioningPlane);
                clip(distance);

                return originalColor;
            }
            ENDCG
        }
    }
}
