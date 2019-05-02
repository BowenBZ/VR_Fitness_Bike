// Upgrade NOTE: upgraded instancing buffer 'NatureManufactureShadersStandardSpecularWet' to new syntax.

Shader "NatureManufacture Shaders/Standard Specular Wet"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo_Sm", 2D) = "white" {}
		_WetColor("Wet Color", Color) = (0.6691177,0.6691177,0.6691177,1)
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 1
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 2)) = 1
		_SpecularRGB("Specular (RGB)", 2D) = "white" {}
		_SpecularPower("Specular Power", Range( 0 , 2)) = 1
		_WetSmoothness("Wet Smoothness", Range( 0 , 100)) = 0
		_AmbientOcclusionG("Ambient Occlusion (G)", 2D) = "white" {}
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_DetailMask("DetailMask", 2D) = "white" {}
		_DetailAlbedoPower("Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			fixed2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform fixed _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform fixed _DetailNormalMapScale;
		uniform sampler2D _DetailNormalMap;
		uniform sampler2D _DetailAlbedoMap;
		uniform float4 _DetailAlbedoMap_ST;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform fixed _DetailAlbedoPower;
		uniform sampler2D _SpecularRGB;
		uniform fixed _WetSmoothness;
		uniform sampler2D _AmbientOcclusionG;

		UNITY_INSTANCING_BUFFER_START(NatureManufactureShadersStandardSpecularWet)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr NatureManufactureShadersStandardSpecularWet
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _WetColor)
#define _WetColor_arr NatureManufactureShadersStandardSpecularWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _SpecularPower)
#define _SpecularPower_arr NatureManufactureShadersStandardSpecularWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _SmoothnessPower)
#define _SmoothnessPower_arr NatureManufactureShadersStandardSpecularWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _AmbientOcclusionPower)
#define _AmbientOcclusionPower_arr NatureManufactureShadersStandardSpecularWet
		UNITY_INSTANCING_BUFFER_END(NatureManufactureShadersStandardSpecularWet)

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			fixed3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_BumpScale );
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float3 normalizeResult202 = normalize( BlendNormals( tex2DNode4 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ) ,_DetailNormalMapScale ) ) );
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			fixed4 tex2DNode195 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult193 = lerp( tex2DNode4 , normalizeResult202 , tex2DNode195.a);
			o.Normal = lerpResult193;
			fixed4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			fixed4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float4 temp_output_44_0 = ( tex2DNode1 * _Color_Instance );
			fixed4 blendOpSrc189 = temp_output_44_0;
			fixed4 blendOpDest189 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult192 = lerp( temp_output_44_0 , (( blendOpDest189 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest189 - 0.5 ) ) * ( 1.0 - blendOpSrc189 ) ) : ( 2.0 * blendOpDest189 * blendOpSrc189 ) ) , ( _DetailAlbedoPower * tex2DNode195.a ));
			fixed4 _WetColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_WetColor_arr, _WetColor);
			float temp_output_261_0 = ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ).r );
			float4 lerpResult272 = lerp( lerpResult192 , ( lerpResult192 * _WetColor_Instance ) , temp_output_261_0);
			o.Albedo = lerpResult272.rgb;
			fixed _SpecularPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_SpecularPower_arr, _SpecularPower);
			o.Specular = ( tex2D( _SpecularRGB, uv_MainTex ) * _SpecularPower_Instance ).rgb;
			fixed _SmoothnessPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_SmoothnessPower_arr, _SmoothnessPower);
			float lerpResult269 = lerp( ( tex2DNode1.a * _SmoothnessPower_Instance ) , _WetSmoothness , temp_output_261_0);
			o.Smoothness = lerpResult269;
			fixed _AmbientOcclusionPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_AmbientOcclusionPower_arr, _AmbientOcclusionPower);
			float clampResult67 = clamp( tex2D( _AmbientOcclusionG, uv_MainTex ).g , ( 1 - _AmbientOcclusionPower_Instance ) , 1 );
			o.Occlusion = clampResult67;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}