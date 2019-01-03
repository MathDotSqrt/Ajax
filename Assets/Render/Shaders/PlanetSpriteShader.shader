Shader "ChrisShaders/PlanetSpriteShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _r ("Planet_Radius", Float) = 1000
        _sealevel ("Sea Level", Float) = 0
        //[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"
            #include "UnitySprites.cginc"

            struct vertin {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct fragin {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            
            //uniform float _r;
            //uniform float _sealevel;
            float RADIUS = 1000;
            float SEALEVEL = 0;

            float2 transform(float2 cartesian) {
                cartesian.y += SEALEVEL;

                float r = RADIUS;

                float h = RADIUS * exp(cartesian.y / RADIUS);
                float phi = cartesian.x / RADIUS;

                float2 polar;
                polar.x = h * sin(phi);
                polar.y = h * cos(phi) - RADIUS;
                return polar;
            }

            fragin vert(vertin IN) {
                fragin OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float4 world_vertex = mul(UNITY_MATRIX_M, IN.vertex);
                //world_vertex.xy = transform(world_vertex.xy); 

                float4 MATRIX_MV = mul(UNITY_MATRIX_V, world_vertex);
                
                MATRIX_MV.xy = transform(MATRIX_MV.xy);

                OUT.vertex = mul(UNITY_MATRIX_P, MATRIX_MV);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;

                return OUT;
            }

            fixed4 frag(fragin IN) : SV_Target {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }

            ENDCG
        }
    }
}
