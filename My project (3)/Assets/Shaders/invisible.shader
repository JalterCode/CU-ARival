Shader "Custom/NewSurfaceShader" {
    Properties {
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "Queue" = "Geometry-1" "RenderType" = "Opaque" }
        LOD 200

        ZWrite On
        ColorMask 0 // Don't write color to the buffer

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return fixed4(0,0,0,0);
            }

            ENDCG
        }
    }
}