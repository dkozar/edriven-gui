Shader "Planet" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (0,0,0,1)
		_AtmospehereColor ("Atmospehere Color", Color) = (0,0,0,1)
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_CloudsTex ("Cluds texture", 2D) = "white" { }
		_LightsTex ("Lights texture", 2D) = "white" { }
		_Rim ("Rim", Range(8,0.01)) = .005
		_Atmospehere ("Atmospehere width", Range(0.01,10)) = 1
		_AtmospehereFalloff("Atmospehere Falloff", Range(10,0.01)) = 1
		_AtmospehereTransparency("Atmospehere Transparency", Range(0.01,10)) = 1
		
	}

	SubShader 
	{
		Tags {"Queue" = "Transparent" }
		Fog { Mode Off }

		// Atmosphere
		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			  
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			uniform float _Atmospehere;
			uniform float _AtmospehereFalloff;
			uniform float _AtmospehereTransparency;
			uniform float4 _AtmospehereColor;

			struct v2f
			{
				 float4 pos : SV_POSITION;
				 float3 normal : TEXCOORD0;
				 float3 viewDir : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				v.vertex.xyz += v.normal * _Atmospehere;
				o.normal = v.normal;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				   
				return o;
			}
			  
			float4 frag(v2f i) : COLOR
			{
				i.normal = normalize(i.normal) * -1;

				float4 color = _AtmospehereColor;
				float4 falloff = saturate(dot(i.viewDir,i.normal)) * _AtmospehereTransparency;
				color.a = pow(falloff, _AtmospehereFalloff);
				return color;
			}

			ENDCG
		}

		// Surface
		CGPROGRAM
		  #pragma surface surf SimpleLambert
		  
		  struct EditorSurfaceOutput {
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half3 Gloss;
            half Specular;
            half Alpha;
            half4 Lights;
          };
		  
		  struct Input 
		  {
			  float2 uv_MainTex;
			  float2 uv_CloudsTex;
			  float3 viewDir;
		  };

		  sampler2D _MainTex;
		  sampler2D _CloudsTex;
		  sampler2D _LightsTex;
		  float4 _RimColor;
		  float4 _Color;
		  float _Rim;

		  half4 LightingSimpleLambert (EditorSurfaceOutput s, half3 lightDir, half atten) 
		  {
			  half NdotL = dot (s.Normal, lightDir);
			  half4 c;
			  float daySide = (NdotL * atten );
			  c.rgb = s.Albedo  * _LightColor0.rgb * daySide;
			  c.rgb += s.Lights.rgb * s.Lights.rgb * (1-daySide);
			  c.a = s.Alpha;
			  return c;
		  }

		  void surf (Input IN, inout EditorSurfaceOutput o) 
		  {
		  	  half4 lights = tex2D (_LightsTex, IN.uv_MainTex);
		  	  half3 cluds = tex2D (_CloudsTex, IN.uv_CloudsTex).rgb;
			  o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
			  o.Albedo += lerp(0, 1, cluds.r);
			  //o.Normal = UnpackNormal ();
			  half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			  o.Emission = _RimColor.rgb * pow (rim, _Rim);
			  //o.Albedo = lerp(o.Albedo, 1, o.Emission.r);
			  o.Lights = lights;
			  //o.Albedo += lerp(0, 1, lights.r);
		  }
		ENDCG
	}

	fallback "Diffuse"
}
