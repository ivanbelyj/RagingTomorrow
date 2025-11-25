Shader "Aurora FPS Engine/Sight/Holographic Sight (Unlit)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D)="white"{}

		[Space]
		_RedDotColor ("Red Dot Color(RGB) Brightness(A)", Color) = (1,1,1,1)
		_RedDotTex ("Red Dot Texture (A)", 2D) = "white" {}
		_RedDotSize ("Red Dot size", Range(0,10)) = 0.0
		[Toggle(FIXED_SIZE)] _FixedSize ("Use Fixed Size", Float) = 0
		_RedDotDist ("Red Dot offset distance", Range(0,50)) = 2.0
		_OffsetX ("Side Offset", Float) = 0.0
		_OffsetY ("Height Offset", Float) = 0.0
	}
 
	SubShader {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
       
       
        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                       
                #include "UnityCG.cginc"
 
                struct appdata_t {
                     float4 vertex : POSITION;
					 float2 uv : TEXCOORD0;
                };
 
                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
					float2 RedDotUV : TEXCOORD1;
                };
 
				fixed4 _Color;
                sampler2D _MainTex;
				fixed4 _MainTex_ST;
				sampler2D _RedDotTex;
				fixed4 _RedDotColor;
				fixed _RedDotSize;
				fixed _RedDotDist;
				fixed _OffsetX;
				fixed _OffsetY;
                       
                v2f vert (appdata_t v) {
                    v2f o;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.vertex = UnityObjectToClipPos(v.vertex);
					fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					fixed3 viewDir = _WorldSpaceCameraPos - worldPos;
			
					#if defined(FIXED_SIZE)
						fixed3 objectCenter = mul(unity_ObjectToWorld, fixed4(0,0,0,1));
						fixed dist = length(objectCenter - _WorldSpaceCameraPos);
						_RedDotSize *= dist;
					#endif

					o.RedDotUV = v.vertex.xy - fixed2(_OffsetX, _OffsetY);
					o.RedDotUV -= mul(unity_WorldToObject, viewDir).xy * _RedDotDist;
					o.RedDotUV /= _RedDotSize;
					o.RedDotUV += fixed2(0.5, 0.5);
                    return o;
                }
                       
                fixed4 frag (v2f i): COLOR {
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					fixed redDot = tex2D (_RedDotTex, i.RedDotUV).a;
					col.rgb += redDot * _RedDotColor.rgb * _RedDotColor.a;
					col.a += redDot * _RedDotColor.a;
                    return col;
                }
            ENDCG
		}
	}
 }