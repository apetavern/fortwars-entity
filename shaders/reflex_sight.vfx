//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
    CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
    Description = "Reflex Sight Shader";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
COMMON
{
    #include "common/shared.hlsl"
    #define S_TRANSLUCENT 1
    #define BLEND_MODE_ALREADY_SET
    #define COLOR_WRITE_ALREADY_SET
}

//=========================================================================================================================

struct VertexInput
{
    #include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
    #include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
    #include "common/vertex.hlsl"
    //
    // Main
    // 	
    PixelInput MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
    {
        PixelInput o = ProcessVertex( i );
        return FinalizeVertex( o );
    }
}

//=========================================================================================================================

PS
{
    #define CUSTOM_TEXTURE_FILTERING
    SamplerState TextureFiltering < Filter( LINEAR ); AddressU( CLAMP ); AddressV( CLAMP ); AddressW( CLAMP ); >;

    RenderState( BlendEnable, true );
    RenderState( SrcBlend, SRC_ALPHA ); 
    RenderState( DstBlend, INV_SRC_ALPHA );
    RenderState( AlphaToCoverageEnable, true );
    RenderState( ColorWriteEnable0, RGBA );

    #include "common/pixel.hlsl"

    //=====================================================================================================================
    // Properties
    //=====================================================================================================================
    float g_flDepth < UiType( Slider ); Default( 1.0 ); Range(0.0f, 50.0f); UiGroup( "Reflex Sight,0/Depth,1/1" ); >;
    bool g_bUseInfiniteDepth < UiType( CheckBox ); UiGroup( "Reflex Sight,0/Depth,1/2" ); >;

    float g_flSightTexScale < UiType( Slider ); Default( 1.0 ); Range(0.0f, 5.0f); UiGroup( "Reflex Sight,0/Appearance,2/1" ); >;
    float3 g_vSightColor < UiType( Color ); Default3( 1.0, 0.0, 0.0 ); UiGroup( "Reflex Sight,0/Appearance,2/2" ); >;
    
    CreateInputTexture2D( TextureReticleMask, Srgb, 8, "", "_color", "Reflex Sight,0/Appearance,2/3", Default3( 1.0, 1.0, 1.0 ) );
    CreateTexture2DWithoutSampler( g_tReticleMask ) < Channel( RGB, Box( TextureReticleMask ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >;
    //=====================================================================================================================

    float2 CalculateTextureCoords( float2 vTextureCoords, float3 vTangentViewVector ) 
    {
        float2 vCalcTextureCoords;

        if ( g_bUseInfiniteDepth ) 
        {
            vCalcTextureCoords = vTangentViewVector.xy / g_flSightTexScale;
        }
        else
        {
            vCalcTextureCoords = (
                vTextureCoords - float2(0.5, 0.5) + (vTangentViewVector.xy * g_flDepth)
            ) / g_flSightTexScale;
        }

        return vCalcTextureCoords + float2( 0.5, 0.5 );
    }

    PixelOutput MainPs( PixelInput i )
    {
        PixelOutput o;
        Material m = GatherMaterial( i );
        
        float3 vPositionWs = i.vPositionWithOffsetWs.xyz + g_vHighPrecisionLightingOffsetWs.xyz;
        float3 vCameraToPositionDirWs = CalculateCameraToPositionDirWs( vPositionWs.xyz );

        float3 vNormalWs = normalize( i.vNormalWs.xyz );
        float3 vTangentUWs = i.vTangentUWs.xyz;
        float3 vTangentVWs = i.vTangentVWs.xyz;
        float3 vTangentViewVector = Vec3WsToTs( vCameraToPositionDirWs.xyz, vNormalWs.xyz, vTangentUWs.xyz, vTangentVWs.xyz );

        float2 vCalcTextureCoords = CalculateTextureCoords( i.vTextureCoords, vTangentViewVector );
        i.vTextureCoords = vCalcTextureCoords;

        o.vColor.rgba = float4( g_vSightColor, 1.0 ) * 32.0;
        
        float4 vTextureSample = Tex2DS( g_tReticleMask, TextureFiltering, vCalcTextureCoords );
        o.vColor.a = vTextureSample.r;
        
        return o;
    }
}