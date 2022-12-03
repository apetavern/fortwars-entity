//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Fortwars Shield Shader";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
	VrForward();													// Indicates this shader will be used for main rendering
	Depth( "vr_depth_only.vfx" ); 									// Shader that will be used for shadowing and depth prepass
	ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
	ToolsWireframe( "vr_tools_wireframe.vfx" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "vr_tools_shading_complexity.vfx" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
    #define S_TRANSLUCENT 1
    #define BLEND_MODE_ALREADY_SET
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
		// Add your vertex manipulation functions here
		return FinalizeVertex( o );
	}
}

//=========================================================================================================================

PS
{
  	#include "common/pixel.hlsl"
	  
    RenderState( BlendEnable, true );
    RenderState( SrcBlend, SRC_ALPHA );
    RenderState( DstBlend, INV_SRC_ALPHA );
    RenderState(AlphaToCoverageEnable, true);

    float3 g_vColor< Default3( 1.0f, 0.0f, 1.0f ); UiGroup( "Shield/Color" ); UiType( Color ); >;
	
    float g_flFresnelReflectance< UiGroup( "Shield/Fresnel" ); UiType( Slider ); Default( 0.01f ); Range( 0.0f, 1.0f ); >;
    FloatAttribute( g_flFresnelReflectance, true );

    float g_flFresnelExponent< UiGroup( "Shield/Fresnel" ); UiType( Slider ); Default( 5.0f ); Range( 0.0f, 10.0f ); >;
    FloatAttribute( g_flFresnelExponent, true );

	BoolAttribute( bWantsFBCopyTexture, true );
  	CreateTexture2D( g_tFrameBufferCopyTexture ) < AsFramebuffer; SrgbRead( false ); Filter( MIN_MAG_MIP_LINEAR ); >;

	//
	// Noise functions yoinked from https://www.shadertoy.com/view/Msf3WH
	//
	float2 hash( float2 p )
	{
		p = float2( dot(p,float2(127.1,311.7)), dot(p,float2(269.5,183.3)) );
		return -1.0 + 2.0*frac(sin(p)*43758.5453123);
	}

	float noise( in float2 p )
	{
		const float K1 = 0.366025404; // (sqrt(3)-1)/2;
		const float K2 = 0.211324865; // (3-sqrt(3))/6;

		float2 i = floor( p + (p.x+p.y)*K1 );
		float2 a = p - i + (i.x+i.y)*K2;
		float m = step(a.y,a.x); 
		float2 o = float2(m,1.0-m);
		float2 b = a - o + K2;
		float2 c = a - 1.0 + 2.0*K2;
		float3 h = max( 0.5-float3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );
		float3 n = h*h*h*h*float3( dot(a,hash(i+0.0)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));
		return dot( n, float3(70.0, 70.0, 70.0) );
	}

	float4 Lighten(float4 base, float4 blend)
	{
		float4 res;
		res.rgb = max(base.rgb, blend.rgb);
		res.a = 1.0;
		return res;
	}

	//
	// Gauss blur yoinked from sbox_glass.vfx
	//
    float3 GaussianBlur(float3 color, float2 uv, float2 size)
    {
        float Pi = 6.28318530718; // Pi*2
        float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
        float Quality = 4.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
        float taps = 1;

        // Blur calculations
        [unroll]
        for( float d=0.0; d<Pi; d+=Pi/Directions)
        {
            [unroll]
            for(float j=1.0/Quality; j<=1.0; j+=1.0/Quality)
            {
                taps += 1;
                color += Tex2D( g_tFrameBufferCopyTexture, uv + float2( cos(d), sin(d) ) * size * j ).rgb;    
            }
        }
        
        // Output to screen
        color /= taps;
        return color;
    }

	//
	// Main
	//
	float4 MainPs( PixelInput i ) : SV_Target0
	{
		float4 o = float4( 0, 0, 0, 1 );

        float3 vPositionWs = i.vPositionWithOffsetWs.xyz + g_vHighPrecisionLightingOffsetWs.xyz;
        float2 vPositionUv = (i.vPositionSs.xy - g_vViewportOffset) / g_vRenderTargetSize;

		float2 timeOffset = float2( g_flTime, g_flTime );
		float2 noisePos = ( vPositionWs.xy + vPositionWs.xz + vPositionWs.yz ) / 3.0f;

		//
		// noise for that 'bumpy' offset warping effect
		//
		float2 bumpNoise = noise( (noisePos * 0.5)+ timeOffset ) + noise( (noisePos * 0.05)+ timeOffset );
		bumpNoise *= 0.01f;
		o.xyz = GaussianBlur( o.xyz, vPositionUv + bumpNoise, 0.01f );

		//
		// off-angle fresnel color warping
		//
        float fresnel = CalculateNormalizedFresnel( g_flFresnelReflectance, g_flFresnelExponent, vPositionWs, i.vNormalWs );
		fresnel += 1.0f;
		fresnel = pow( fresnel, 2 );
		o.xyz *= fresnel * g_vColor;

		//
		// noise for color distortion effect
		//
		float2 colNoise = noise( (noisePos * 1.0)+ timeOffset ) + noise( (noisePos * 10.0)+ timeOffset  ) + noise( (noisePos * 0.1)+ timeOffset	);
		colNoise /= 3.0f;
		colNoise = (colNoise + 1.0f) / 2.0f;
		o.xyz = Lighten( o, o * ( colNoise.x + colNoise.y ) ).xyz;

		float distance = length( vPositionWs - g_vCameraPositionWs );
		distance -= 8;
		distance /= 8;
		distance = clamp( distance, 0, 1 );
		o.a *= distance;

		return o;
	}
}