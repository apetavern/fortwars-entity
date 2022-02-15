using Sandbox;
using Sandbox.UI;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class Hitmarker : Panel
	{
		public static Hitmarker Instance { get; private set; }
		private static HitmarkerInstance currentHitmarkerInstance;

		public Hitmarker()
		{
			Instance = this;
			StyleSheet.Load( "/ui/hud/Hitmarker.scss" );
		}

		public void OnHit( float amount, bool isKill, bool isBlock = false )
		{
			currentHitmarkerInstance?.Delete();
			currentHitmarkerInstance = new HitmarkerInstance( amount, this );
			currentHitmarkerInstance.SetClass( "kill", isKill );

			Log.Trace( isBlock );
			currentHitmarkerInstance.SetClass( "block", isBlock );
		}

		public class HitmarkerInstance : Panel
		{
			public HitmarkerInstance( float amount, Panel parent )
			{
				float scale = 0.25f.LerpTo( 1.0f, amount / 15f );
				Style.Width = scale * 96f;
				Style.Height = scale * 96f;
				Parent = parent;
				_ = KillAfterTime();
			}

			async Task KillAfterTime()
			{
				await Task.Delay( 75 );
				Delete();
			}
		}
	}
}
