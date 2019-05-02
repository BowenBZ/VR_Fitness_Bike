// Upgrade NOTE: upgraded instancing buffer 'NatureManufactureShadersStandardMetallicWet' to new syntax.

Shader "NatureManufacture Shaders/Standard Metallic Wet"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_WetColor("Wet Color", Color) = (0.6691177,0.6691177,0.6691177,1)
		_MainTex("Albedo_Sm", 2D) = "white" {}
		_SmothnessPower("Smothness Power", Range( 0 , 2)) = 1
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 5)) = 0
		_MetalicRAmbientOcclusionG("Metalic (R) Ambient Occlusion (G)", 2D) = "white" {}
		_MetalicPower("Metalic Power", Range( 0 , 2)) = 1
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_WetSmoothness("Wet Smoothness", Range( 0 , 0.99)) = 0.67
		_DetailMask("DetailMask (A)", 2D) = "white" {}
		_DetailAlbedoPower("Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
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
		uniform sampler2D _MetalicRAmbientOcclusionG;
		uniform fixed _WetSmoothness;

		UNITY_INSTANCING_BUFFER_START(NatureManufactureShadersStandardMetallicWet)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr NatureManufactureShadersStandardMetallicWet
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _WetColor)
#define _WetColor_arr NatureManufactureShadersStandardMetallicWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _MetalicPower)
#define _MetalicPower_arr NatureManufactureShadersStandardMetallicWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _SmothnessPower)
#define _SmothnessPower_arr NatureManufactureShadersStandardMetallicWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _AmbientOcclusionPower)
#define _AmbientOcclusionPower_arr NatureManufactureShadersStandardMetallicWet
		UNITY_INSTANCING_BUFFER_END(NatureManufactureShadersStandardMetallicWet)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			fixed3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_BumpScale );
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			fixed4 tex2DNode481 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult479 = lerp( tex2DNode4 , BlendNormals( tex2DNode4 , UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ) ,_DetailNormalMapScale ) ) , tex2DNode481.a);
			o.Normal = lerpResult479;
			fixed4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			fixed4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float4 temp_output_77_0 = ( tex2DNode1 * _Color_Instance );
			fixed4 blendOpSrc474 = temp_output_77_0;
			fixed4 blendOpDest474 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , ( _DetailAlbedoPower * tex2DNode481.a ));
			fixed4 _WetColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_WetColor_arr, _WetColor);
			float temp_output_522_0 = ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ).r );
			float4 lerpResult541 = lerp( lerpResult480 , ( lerpResult480 * _WetColor_Instance ) , temp_output_522_0);
			o.Albedo = lerpResult541.rgb;
			fixed4 tex2DNode2 = tex2D( _MetalicRAmbientOcclusionG, uv_MainTex );
			fixed _MetalicPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_MetalicPower_arr, _MetalicPower);
			o.Metallic = ( tex2DNode2.r * _MetalicPower_Instance );
			fixed _SmothnessPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_SmothnessPower_arr, _SmothnessPower);
			float lerpResult540 = lerp( ( tex2DNode1.a * _SmothnessPower_Instance ) , _WetSmoothness , temp_output_522_0);
			o.Smoothness = lerpResult540;
			fixed _AmbientOcclusionPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_AmbientOcclusionPower_arr, _AmbientOcclusionPower);
			float clampResult96 = clamp( tex2DNode2.g , ( 1 - _AmbientOcclusionPower_Instance ) , 1 );
			o.Occlusion = clampResult96;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}