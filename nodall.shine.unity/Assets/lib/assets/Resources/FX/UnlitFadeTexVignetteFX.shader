// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitFadeTex VignetteFX"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		[MaterialToggle]
		_FlipY("Flip Y", Float) = 0
		[Header(Vignette FX)]
		_FadeFX("Fade FX", Float) = 1
		[MaterialToggle]
		_InvertVignette("Invert Vignette", Float) = 0
		_Intensity("Intensity", Float) = 3
		[PowerSlider(2.0)]
		_Size("Size", Range(0, 50)) = 1.9
		_VignetteColor("Vignette Color", Color) = (0, 0, 0, 1)
	}
	SubShader
	{
		//Tags { "RenderType"="Transparent" }
			Tags{ Queue = Transparent }

		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			float _FadeFX;
			float _FlipY;

			fixed4 _VignetteColor;
			float _Intensity = 10;
			float _Size = 1;
			float _InvertVignette = 1;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				float2 addUV = float2(0, _FlipY);
				float2 mulUV = float2(1, 1 - 2 * _FlipY);
				o.uv = o.uv * mulUV + addUV;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv).rgba * _Color;
				fixed4 vigCol = _Color;
				float dist = _Size*distance(i.uv.xy, float2(0.5, 0.5));
				//col.rgb = lerp(col.rgb, _VignetteColor.rgb, _InvertVignette - (1-2*_InvertVignette) * sqrt(pow(dist, _Intensity)));
				vigCol.rgb = lerp(col.rgb, _VignetteColor.rgb,  _InvertVignette + (1-2*_InvertVignette) * sqrt(pow(dist, _Intensity)));
				//col.rgb = lerp(col.rgb, _VignetteColor.rgb, 1 + -1 * sqrt(pow(dist, _Intensity)));
				col.rgb = lerp(col.rgb, vigCol.rgb, _FadeFX);
				col.a *= _Color.a;
				return col;
			}
			ENDCG
		}
	}
}
