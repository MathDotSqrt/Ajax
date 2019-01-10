Shader "ChrisShaders/WaterReflectionShader"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct vertin
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD0;
            };

            struct fragin
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform sampler2D ReflectionTexture;
            uniform float4 ReflectionTexture_ST;

            uniform float quadY;

            fragin vert (vertin v)
            {
                fragin o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 quadClip = UnityWorldToClipPos(float4(0, quadY, 0, 1));
                quadClip.xy /= quadClip.w;

                o.uv = o.vertex.xy / o.vertex.w;
               
                o.uv = (o.uv + 1) / 2;
                o.uv.y -= quadClip.y + .01;
                o.uv.y = 1 - o.uv.y;

                return o;
            }

            fixed4 frag (fragin i) : SV_Target
            {
                //half2 offset = half2(2 * (0.5 - i.uv.x) * (i.uv.y ), 0.0f);

                // sample the texture
                fixed4 col = tex2D(ReflectionTexture, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
