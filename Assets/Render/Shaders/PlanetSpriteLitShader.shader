// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "ChrisShaders/PlanetSpriteLitShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _r ("Planet_Radius", Float) = 1000
        _sealevel ("Sea Level", Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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

        CGPROGRAM
        #pragma vertex vert nofog nolightmap nodynlightmap keepalpha noinstancing

        #pragma surface surf Lambert 
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };

        //uniform float _r;
        //uniform float _sealevel;
        uniform float RADIUS = 1000;
        uniform float SEALEVEL = 0;

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

        void vert (inout appdata_full v, out Input o) {
            v.vertex = mul(UNITY_MATRIX_V, v.vertex.xyzw);
            v.vertex.xy = transform(v.vertex.xy);
            v.vertex = mul(UNITY_MATRIX_I_V, v.vertex);
            //v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color * _RendererColor;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = SampleSpriteTexture (IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}
