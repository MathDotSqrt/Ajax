Shader "ChrisShaders/WaterReflectionShader"
{
    Properties
    {
        _DistortionTexture ("Distortion Texture", 2D) = "white" {}
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
                float2 reflection_uv : TEXCOORD0;
                float2 distortion_uv : TEXCOORD1;
            };

            uniform sampler2D _ReflectionTexture;
            uniform sampler2D _DistortionTexture;
            uniform float4 _DistortionTexture_ST;


            uniform float quadY;

            fragin vert (vertin v)
            {
                fragin o;

                //*Transforms quads vertex to clipspace*//
                o.vertex = UnityObjectToClipPos(v.vertex);

                //* Transforms top left vertex of quad to clip space *// 
                float4 quadClipPos = UnityWorldToClipPos(float4(0, quadY, 0, 1));
                quadClipPos.xy /= quadClipPos.w;

                //* Sets reflection uv to clipspace of the vertex *// 
                o.reflection_uv = o.vertex.xy / o.vertex.w;

                //* Transforms coordinates from screen space (-1, 1) to texture space (0, 1) *//
                o.reflection_uv = (o.reflection_uv + 1) / 2;
                //o.reflection_uv.y = 1 - o.reflection_uv.y;

                //* Offsets the uv y with the top left quad in clip space *//
                o.reflection_uv.y -= quadClipPos.y + .01;

                //* Gets uv coordinates of the distortion texture *//
                o.distortion_uv = v.uv;//TRANSFORM_TEX(v.uv, _DistortionTexture);


                return o;
            }

            float2 stair(float2 uv, float scale){
                uv *= scale;
                uv = floor(uv);
                uv /= scale;

                return uv;
            }

            fixed4 frag (fragin i) : SV_Target
            {


                //* Affine Transform offset *//
                half2 offset = half2(.04 * (.5 - i.reflection_uv.x) * (1 - i.distortion_uv.y), 0.0f);
                
                //* UV sample coords with the affine transfer offset *//
                half2 distortion_sample_uv1 = (i.distortion_uv + offset);
                half2 distortion_sample_uv2 = (i.distortion_uv + offset) * 3;
                distortion_sample_uv1.x *= 10;
                distortion_sample_uv2.x *= 10;


                //* Transforms UV.x with time to create water flow effect
                distortion_sample_uv1.x -= _Time.x;
                distortion_sample_uv2.x -= _Time.x * 2;

                fixed4 distortion_color1 = tex2D(_DistortionTexture, distortion_sample_uv1);
                fixed4 distortion_color2 = tex2D(_DistortionTexture, distortion_sample_uv2);
                fixed4 distortion_color = (distortion_color1 + distortion_color2) / 2;
                fixed4 distortion_offset = distortion_color;

                distortion_offset.b = 0;
                distortion_offset -= .5;
                distortion_offset /= 25;
                distortion_offset.y /= 5;

                float2 sample_reflection = i.reflection_uv;      
                sample_reflection.xy += distortion_offset.rg;

                fixed4 reflection_col = tex2D(_ReflectionTexture, sample_reflection);
                return reflection_col * .6 + distortion_color * .4;
                //return reflection_col;
                //return distortion_offset;
            }
            ENDCG
        }
    }
}
