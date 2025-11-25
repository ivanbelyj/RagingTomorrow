
Shader "Aurora FPS Engine/Sight/Holographic Sight (Standard)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

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
		Tags { "RenderType"="Opaque" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows vertex:vert alpha:blend

		#pragma target 3.0

		#pragma shader_feature FIXED_SIZE

		sampler2D _MainTex;
		sampler2D _RedDotTex;

		struct Input {
			float2 uv_MainTex;
			float2 RedDotUV;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _RedDotColor;
		fixed _RedDotSize;
		fixed _RedDotDist;
		fixed _OffsetX;
		fixed _OffsetY;

		void vert( inout appdata_full v, out Input o ) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
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
		}


		UNITY_INSTANCING_BUFFER_START(Props)

		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed redDot = tex2D (_RedDotTex, IN.RedDotUV).a;
			o.Emission = redDot * _RedDotColor.rgb * _RedDotColor.a;
			o.Albedo = c.rgb;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a + redDot * _RedDotColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
