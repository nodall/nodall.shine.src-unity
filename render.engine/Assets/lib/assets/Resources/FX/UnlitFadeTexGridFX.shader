// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitFadeTex GridFX"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		[MaterialToggle]
		_FlipY("Flip Y", Float) = 0
		_FadeFX("Fade FX", Float) = 1
		_GridStep("Grid size", Float) = 3
		_GridWidth("Grid width", Float) = 1.9
		_GridOffset("Grid offset", Float) = 0.5
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

			float _GridStep = 10;
			float _GridWidth = 1;
			float _GridOffset;
			float _FadeFX;
			float _FlipY;

			float2 addUV;
			float2 mulUV;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);


				addUV = float2(0, _FlipY);
				mulUV = float2(1, 1 - 2 * _FlipY);
				o.uv = o.uv * mulUV + addUV;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv).rgba * _Color;

				float4 white = float4(1, 1, 1, 1);
				float2 pos = (i.uv * _GridStep) + (_GridOffset, _GridOffset);
				float2 f = abs(frac(pos) - .5);
				float2 df = fwidth(pos) * _GridWidth;
				float2 g = smoothstep(-df, df, f);
				float grid = 1.0 - saturate(g.x * g.y);
				col = lerp(col, white, grid * _FadeFX);

				pos = (i.uv * _GridStep * 4) + (_GridOffset, _GridOffset);
				f = abs(frac(pos) - .5);
				df = fwidth(pos) * _GridWidth * 1.1;
				g = smoothstep(-df, df, f);
				grid = 1.0 - (g.x * g.y);
				col = lerp(col, white, 0.4*grid * _FadeFX);


				pos = i.uv  + (0.5, 0.5);
				f = abs(frac(pos) - .5);
				df = fwidth(pos) * 250;
				g = smoothstep(-df, df, f);
				grid =  1- saturate(0.6 * sqrt(g.x * g.x + g.y * g.y));
				//col.rgb = lerp(col.rgb, float3(1, 1, 1), 0.2 * grid);

				pos = i.uv + (0.5, 0.5);
				f = abs(frac(pos) - .5);
				df = fwidth(pos) * 25;
				g = smoothstep(-df, df, f);
				grid = 1.0 - saturate(g.x * g.y);
				col = lerp(col, white, 0.6 * grid * _FadeFX);

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);				

				//col.a = _Color.a;
				return col;
			}
			ENDCG
		}
	}
}
