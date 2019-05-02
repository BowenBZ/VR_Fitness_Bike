// Upgrade NOTE: upgraded instancing buffer 'NatureManufactureShadersStandardMetalicCoverWet' to new syntax.

Shader "NatureManufacture Shaders/Standard Metalic Cover Wet"
{
	Properties
	{
		_WetColor("Wet Color", Color) = (0.6691177,0.6691177,0.6691177,1)
		_WetSmoothness("Wet Smoothness", Range( 0 , 0.99)) = 0.67
		_MainTex("MainTex ", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 5)) = 0
		_MetalicRAmbientOcclusionGSmoothnessA("Metalic (R) Ambient Occlusion (G) Smoothness (A)", 2D) = "white" {}
		_MetalicPower("Metalic Power", Range( 0 , 2)) = 1
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SmothnessPower("Smothness Power", Range( 0 , 2)) = 1
		_DetailMask("DetailMask", 2D) = "black" {}
		_DetailAlbedoPower("Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 0
		_Cover_Amount("Cover_Amount", Range( 0 , 2)) = 0.13
		_Cover_AmountGrowSpeed("Cover_Amount Grow Speed", Range( 1 , 3)) = 2
		_TriplanarCoverFalloff("Triplanar Cover Falloff", Range( 1 , 100)) = 8
		MossAlbedoRGB("Cover Albedo (RGB)", 2D) = "white" {}
		_CoverAlbedoColor("Cover Albedo Color", Color) = (1,1,1,1)
		_CoverTiling("Cover Tiling", Range( 0.0001 , 100)) = 15
		_CoverNormalRGB("Cover Normal (RGB)", 2D) = "white" {}
		_CoverNormalScale("Cover Normal Scale", Range( 0 , 5)) = 0
		_CoverMetalicRAmbientOcclusionGSmothnessA("Cover Metalic (R) Ambient Occlusion(G) Smothness (A)", 2D) = "white" {}
		_CoverMetalicPower("Cover Metalic Power", Range( 0 , 2)) = 1
		_CoverAmbientOcclusionPower("Cover Ambient Occlusion Power", Range( 0 , 1)) = 1
		_CoverSmoothnessPower("Cover Smoothness Power", Range( 0 , 2)) = 1
		_CoverMaxAngle("Cover Max Angle ", Range( 0.001 , 90)) = 90
		_CoverHardness("Cover Hardness", Range( 1 , 10)) = 5
		_CoverNormalHardness("Cover Normal Hardness", Range( 0 , 10)) = 0
		_Cover_Min_Height("Cover_Min_Height", Range( -1000 , 10000)) = -1000
		_Cover_Min_Height_Blending("Cover_Min_Height_Blending", Range( 0 , 500)) = 1
		_CoverHeightG("Cover Height (G)", 2D) = "white" {}
		_CoverHeightSharpness("Cover Height Sharpness", Range( 0 , 2)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			fixed2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
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
		uniform sampler2D _CoverNormalRGB;
		uniform fixed _CoverTiling;
		uniform fixed _TriplanarCoverFalloff;
		uniform fixed _CoverNormalScale;
		uniform fixed _CoverNormalHardness;
		uniform fixed _Cover_Amount;
		uniform fixed _Cover_AmountGrowSpeed;
		uniform fixed _CoverMaxAngle;
		uniform fixed _CoverHardness;
		uniform fixed _Cover_Min_Height;
		uniform fixed _Cover_Min_Height_Blending;
		uniform sampler2D _CoverHeightG;
		uniform fixed _CoverHeightSharpness;
		uniform fixed _DetailAlbedoPower;
		uniform sampler2D MossAlbedoRGB;
		uniform sampler2D _MetalicRAmbientOcclusionGSmoothnessA;
		uniform sampler2D _CoverMetalicRAmbientOcclusionGSmothnessA;
		uniform fixed _CoverSmoothnessPower;
		uniform fixed _WetSmoothness;

		UNITY_INSTANCING_BUFFER_START(NatureManufactureShadersStandardMetalicCoverWet)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _CoverAlbedoColor)
#define _CoverAlbedoColor_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _WetColor)
#define _WetColor_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _MetalicPower)
#define _MetalicPower_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _CoverMetalicPower)
#define _CoverMetalicPower_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _SmothnessPower)
#define _SmothnessPower_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _AmbientOcclusionPower)
#define _AmbientOcclusionPower_arr NatureManufactureShadersStandardMetalicCoverWet
			UNITY_DEFINE_INSTANCED_PROP(fixed, _CoverAmbientOcclusionPower)
#define _CoverAmbientOcclusionPower_arr NatureManufactureShadersStandardMetalicCoverWet
		UNITY_INSTANCING_BUFFER_END(NatureManufactureShadersStandardMetalicCoverWet)


		inline float3 TriplanarSamplingSNF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			fixed3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_BumpScale );
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			fixed3 tex2DNode485 = UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ) ,_DetailNormalMapScale );
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			fixed4 tex2DNode481 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult479 = lerp( tex2DNode4 , BlendNormals( tex2DNode4 , tex2DNode485 ) , tex2DNode481.a);
			float temp_output_265_0 = ( 1 / _CoverTiling );
			float3 ase_worldPos = i.worldPos;
			fixed3 ase_worldNormal = WorldNormalVector( i, fixed3( 0, 0, 1 ) );
			fixed3 ase_worldTangent = WorldNormalVector( i, fixed3( 1, 0, 0 ) );
			fixed3 ase_worldBitangent = WorldNormalVector( i, fixed3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar457 = TriplanarSamplingSNF( _CoverNormalRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 0 );
			float3 tanTriplanarNormal457 = mul( ase_worldToTangent, triplanar457 );
			float3 appendResult458 = (fixed3(_CoverNormalScale , _CoverNormalScale , 1));
			float3 temp_output_306_0 = ( tanTriplanarNormal457 * appendResult458 );
			float3 normalizeResult483 = normalize( BlendNormals( UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ) ,_CoverNormalHardness ) , tex2DNode485 ) );
			float temp_output_489_0 = ( 4 - _Cover_AmountGrowSpeed );
			float clampResult492 = clamp( pow( ( _Cover_Amount / temp_output_489_0 ) , temp_output_489_0 ) , 0 , 2 );
			float clampResult87 = clamp( ase_worldNormal.y , 0 , 0.999999 );
			float temp_output_85_0 = ( _CoverMaxAngle / 45 );
			float clampResult83 = clamp( ( clampResult87 - ( 1.0 - temp_output_85_0 ) ) , 0 , 2 );
			float temp_output_329_0 = ( ( 1.0 - _Cover_Min_Height ) + ase_worldPos.y );
			float clampResult336 = clamp( ( temp_output_329_0 + 1 ) , 0 , 1 );
			float clampResult335 = clamp( ( ( 1.0 - ( ( temp_output_329_0 + _Cover_Min_Height_Blending ) / temp_output_329_0 ) ) + -0.5 ) , 0 , 1 );
			float clampResult338 = clamp( ( clampResult336 + clampResult335 ) , 0 , 1 );
			float temp_output_349_0 = ( pow( ( clampResult83 * ( 1 / temp_output_85_0 ) ) , _CoverHardness ) * clampResult338 );
			float3 lerpResult15 = lerp( normalizeResult483 , tanTriplanarNormal457 , ( saturate( ( ase_worldNormal.y * clampResult492 ) ) * temp_output_349_0 ));
			float clampResult368 = clamp( ( ( WorldNormalVector( i , lerpResult15 ).y * clampResult492 ) * ( ( clampResult492 * _CoverHardness ) * temp_output_349_0 ) ) , 0 , 1 );
			float4 triplanar460 = TriplanarSamplingSF( _CoverHeightG, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 0 );
			float temp_output_18_0 = saturate( ( clampResult368 * pow( triplanar460.y , _CoverHeightSharpness ) ) );
			float3 lerpResult369 = lerp( lerpResult479 , temp_output_306_0 , temp_output_18_0);
			float3 lerpResult506 = lerp( lerpResult369 , temp_output_306_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).g);
			float3 lerpResult509 = lerp( lerpResult506 , lerpResult479 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).b);
			o.Normal = lerpResult509;
			fixed4 _Color_Instance = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			float4 temp_output_77_0 = ( tex2D( _MainTex, uv_MainTex ) * _Color_Instance );
			fixed4 blendOpSrc474 = temp_output_77_0;
			fixed4 blendOpDest474 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , ( _DetailAlbedoPower * tex2DNode481.a ));
			float4 triplanar455 = TriplanarSamplingSF( MossAlbedoRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 0 );
			fixed4 _CoverAlbedoColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverAlbedoColor_arr, _CoverAlbedoColor);
			float4 temp_output_78_0 = ( triplanar455 * _CoverAlbedoColor_Instance );
			float4 lerpResult10 = lerp( lerpResult480 , temp_output_78_0 , temp_output_18_0);
			float4 lerpResult502 = lerp( lerpResult10 , temp_output_78_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).g);
			float4 lerpResult507 = lerp( lerpResult502 , lerpResult480 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).b);
			fixed4 _WetColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_WetColor_arr, _WetColor);
			float4 lerpResult497 = lerp( lerpResult507 , ( lerpResult507 * _WetColor_Instance ) , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).r);
			o.Albedo = lerpResult497.rgb;
			fixed4 tex2DNode2 = tex2D( _MetalicRAmbientOcclusionGSmoothnessA, uv_MainTex );
			fixed _MetalicPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_MetalicPower_arr, _MetalicPower);
			float temp_output_64_0 = ( tex2DNode2.r * _MetalicPower_Instance );
			float4 triplanar459 = TriplanarSamplingSF( _CoverMetalicRAmbientOcclusionGSmothnessA, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 0 );
			fixed _CoverMetalicPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverMetalicPower_arr, _CoverMetalicPower);
			float temp_output_66_0 = ( triplanar459.x * _CoverMetalicPower_Instance );
			float lerpResult17 = lerp( temp_output_64_0 , temp_output_66_0 , temp_output_18_0);
			float lerpResult503 = lerp( lerpResult17 , temp_output_66_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).g);
			float lerpResult508 = lerp( lerpResult503 , temp_output_64_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).b);
			o.Metallic = lerpResult508;
			fixed _SmothnessPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_SmothnessPower_arr, _SmothnessPower);
			float temp_output_62_0 = ( tex2DNode2.a * _SmothnessPower_Instance );
			float temp_output_125_0 = ( triplanar459.w * _CoverSmoothnessPower );
			float lerpResult27 = lerp( temp_output_62_0 , temp_output_125_0 , temp_output_18_0);
			float lerpResult505 = lerp( lerpResult27 , temp_output_125_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).g);
			float lerpResult511 = lerp( lerpResult505 , temp_output_62_0 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).b);
			float lerpResult496 = lerp( lerpResult511 , _WetSmoothness , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).r);
			o.Smoothness = lerpResult496;
			fixed _AmbientOcclusionPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_AmbientOcclusionPower_arr, _AmbientOcclusionPower);
			float clampResult96 = clamp( tex2DNode2.g , ( 1 - _AmbientOcclusionPower_Instance ) , 1 );
			fixed _CoverAmbientOcclusionPower_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverAmbientOcclusionPower_arr, _CoverAmbientOcclusionPower);
			float clampResult94 = clamp( triplanar459.y , ( 1 - _CoverAmbientOcclusionPower_Instance ) , 1 );
			float lerpResult28 = lerp( clampResult96 , clampResult94 , temp_output_18_0);
			float lerpResult504 = lerp( lerpResult28 , clampResult94 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).g);
			float lerpResult510 = lerp( lerpResult504 , clampResult96 , ( 1.0 - ( i.vertexColor / float4( 1,1,1,1 ) ) ).b);
			o.Occlusion = lerpResult510;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				fixed4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}