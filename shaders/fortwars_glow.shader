//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Fort Wars GLOW BAYBEEE!";
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
    ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
    ToolsWireframe( "vr_tools_wireframe.vfx" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "vr_tools_shading_complexity.vfx" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
	#define USE_CUSTOM_SHADING 1
	#include "common/shared.hlsl"
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

//
// Used for outline
//
GS
{
	#include "common/vertex.hlsl"

	float g_flOutlineSize < UiType( Slider ); Range( 0.025f, 10.0f ); Default( 1.0f ); UiGroup( "Outline Settings,10/20" ); >;
	bool g_bResolutionDependentOutline < UiType( CheckBox ); Default( 0 ); UiGroup( "Outline Settings,10/20" ); >;
	bool g_bScaleByDistance < UiType( CheckBox ); Default( 0 ); UiGroup( "Outline Settings,10/20" ); >;
	float g_flOffset < UiType( Slider ); Range( 0.0f, 10.0f ); Default( 0.0f ); UiGroup( "Outline Settings,10/20" ); >;


	void PositionOffset (inout PixelInput input, float2 vOffsetDir, float flOutlineSize)
	{
		float2 vAspectRatio = normalize(g_vInvViewportSize);
		input.vPositionPs.xy += (vOffsetDir * 2.0) * vAspectRatio * flOutlineSize;
	}
	
	//
	// Use this one if you want absolute pixel size
	//
	void PositionOffsetResolutionDependent(inout PixelInput input, float2 vOffsetDir, float flOutlineSize)
	{
		input.vPositionPs.xy += (vOffsetDir * 2.0) * g_vInvViewportSize * flOutlineSize;
	}
	
    //
    // Main
    //
    [maxvertexcount(3*10)]
    void MainGs(triangle in PixelInput vertices[3], inout TriangleStream<PixelInput> triStream)
    {
        const float fTwoPi = 6.28318f;
		const uint nNumIterations = 6; 

        PixelInput v[3];

        [unroll]
        for( uint i = 0; i <= nNumIterations; i += 1 )
		{
			float fCycle = (float)i / (float)nNumIterations;

			float2 vOffset = float2( 
				( sin( fCycle * fTwoPi ) ),
				( cos( fCycle * fTwoPi ) )
			);

			for ( int i = 0; i < 3; i++ )
			{
				v[i] = vertices[i];

				float flOutlineSize = g_flOutlineSize;

				if ( g_bScaleByDistance )
					flOutlineSize *= v[i].vPositionPs.w * 0.01f;

				if ( g_bResolutionDependentOutline )
					PositionOffsetResolutionDependent( v[i], vOffset, flOutlineSize );
				else
					PositionOffset( v[i], vOffset, flOutlineSize );

				// Todo: I will make this use a stencil mask instead of
				// positioning it backwards when moving glow logic to C# - Sam
				
				v[i].vPositionPs.z += D_ENABLE_USER_CLIP_PLANE ? 0.1f : g_flOffset;
			}

			triStream.Append(v[2]);
			triStream.Append(v[0]);
			triStream.Append(v[1]);
		}
		
		// emit the vertices
		triStream.RestartStrip();
    }
}


//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"
	//
	// Main
	//
	PixelOutput MainPs( PixelInput i )
	{
		PixelOutput o;

		o.vColor = float4( i.vVertexColor.rgb, 1.0 );

		return o;
	}
}