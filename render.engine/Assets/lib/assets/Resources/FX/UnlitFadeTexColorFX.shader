// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitFadeTex ColorFX"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		[MaterialToggle]
		_FlipY("Flip Y", Float) = 0
		[Header(Vignette FX)]
		_FadeFX("Fade FX", Float) = 1
		_Hue("Hue", Range(0, 360.0)) = 0
		_Saturation("Saturation", Range(0, 2.0)) = 1
		_Brightness("Brightness", Range(-1, 1.0)) = 0
		_Contrast("Contrast", Range(0, 2.0)) = 1
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

			float _Hue, _Saturation, _Brightness, _Contrast;

			inline float3 applyHue(float3 aColor, float aHue)
			{
				float angle = radians(aHue);
				float3 k = float3(0.57735, 0.57735, 0.57735);
				float cosAngle = cos(angle);
				//Rodrigues' rotation formula
				return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
			}


			inline float4 applyHSBEffect(float4 startColor)
			{
				//float hue = 360 * hsbc.r;
				//float saturation = hsbc.g * 2;
				//float brightness = hsbc.b * 2 - 1;
				//float contrast = hsbc.a * 2;
				float4 outputColor = startColor;
				outputColor.rgb = applyHue(outputColor.rgb, _Hue);
				outputColor.rgb = (outputColor.rgb - 0.5f) * _Contrast + 0.5f + _Brightness;
				outputColor.rgb = lerp(Luminance(outputColor.rgb), outputColor.rgb, _Saturation);

				return outputColor;
			}

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
				fixed4 fxCol = applyHSBEffect(col);

				col.rgb = lerp(col.rgb, fxCol.rgb, _FadeFX);
				col.a *= _Color.a;
				return col;
			}
			ENDCG
		}
	}
}
