Shader "NatureManufacture Shaders/Debug/Flowmap Direction"
{
	Properties
	{
		_Direction("Direction", 2D) = "white" {}
		_RotateUV("Rotate UV", Range( 0 , 1)) = 0
		_TextureSize("Texture Size", Vector) = (246,246,0,0)
		_TileSize("TileSize", Vector) = (20,30,0,0)
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv4_texcoord4;
			float2 uv_texcoord;
		};

		uniform float _RotateUV;
		uniform sampler2D _Direction;
		uniform float2 _TileSize;
		uniform float2 _TextureSize;
		uniform sampler2D sampler048;
		uniform float4 _Direction_TexelSize;


		inline float2 MyCustomExpression63( float2 A , float2 B )
		{
			return frac(A/B)*B;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv4_TexCoord1 = i.uv4_texcoord4 * float2( 1,1 ) + float2( 0,0 );
			float2 temp_output_25_0 = radians( ( uv4_TexCoord1 * float2( -90,-90 ) ) );
			float2 ifLocalVar98 = 0;
			if( _RotateUV <= 0.0 )
				ifLocalVar98 = temp_output_25_0;
			else
				ifLocalVar98 = ( temp_output_25_0 + float2( 1.578,0 ) );
			float2 uv_TexCoord60 = i.uv_texcoord * float2( 2,2 ) + float2( 0,0 );
			float2 A63 = uv_TexCoord60;
			float2 invTileSize61 = ( float2( 1,1 ) / _TileSize );
			float2 B63 = invTileSize61;
			float2 localMyCustomExpression63 = MyCustomExpression63( A63 , B63 );
			float2 appendResult50 = (float2(_Direction_TexelSize.x , _Direction_TexelSize.y));
			float2 ScaledMax56 = ( _TextureSize * appendResult50 );
			float2 ScaledMin57 = ( appendResult50 * float2( 10,10 ) );
			float2 Size62 = ( ScaledMax56 - ScaledMin57 );
			float2 TiledVar65 = ( localMyCustomExpression63 * Size62 );
			float2 finalUV69 = ( 0 + ( TiledVar65 * _TileSize ) );
			float cos3 = cos( ifLocalVar98.x );
			float sin3 = sin( ifLocalVar98.x );
			float2 rotator3 = mul( finalUV69 - float2( 0.5,0.5 ) , float2x2( cos3 , -sin3 , sin3 , cos3 )) + float2( 0.5,0.5 );
			float4 tex2DNode2 = tex2Dlod( _Direction, float4( rotator3, 0, 0.0) );
			float cos32 = cos( ( ifLocalVar98.x + 3.14 ) );
			float sin32 = sin( ( ifLocalVar98.x + 3.14 ) );
			float2 rotator32 = mul( finalUV69 - float2( 0.5,0.5 ) , float2x2( cos32 , -sin32 , sin32 , cos32 )) + float2( 0.5,0.5 );
			float ifLocalVar30 = 0;
			if( ifLocalVar98.y >= 0.0 )
				ifLocalVar30 = tex2DNode2.a;
			else
				ifLocalVar30 = tex2Dlod( _Direction, float4( rotator32, 0, 0.0) ).a;
			float ifLocalVar100 = 0;
			if( _RotateUV <= 0.0 )
				ifLocalVar100 = uv4_TexCoord1.y;
			else
				ifLocalVar100 = uv4_TexCoord1.x;
			float3 _Vector0 = float3(1,1,0);
			float clampResult78 = clamp( ( _Vector0.y - ( abs( uv4_TexCoord1.y ) * 2.3 ) ) , 0.0 , 1.0 );
			float3 appendResult72 = (float3(_Vector0.x , clampResult78 , _Vector0.z));
			float3 appendResult73 = (float3(_Vector0.z , clampResult78 , _Vector0.x));
			float3 ifLocalVar39 = 0;
			if( ifLocalVar100 >= 0.0 )
				ifLocalVar39 = appendResult72;
			else
				ifLocalVar39 = appendResult73;
			float3 clampResult36 = clamp( ( ifLocalVar39 * float3( 2,2,2 ) ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
			o.Albedo = ( ifLocalVar30 * clampResult36 );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}