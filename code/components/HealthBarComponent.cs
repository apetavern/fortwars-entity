using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace Fortwars
{
	/// <summary>
	/// For things that need a health bar overlay
	/// </summary>
	public class HealthBarComponent : EntityComponent<FortwarsBlock>
	{
		private HealthBar healthBar;

		protected override void OnActivate()
		{
			healthBar = new HealthBar( Entity );
		}

		protected override void OnDeactivate()
		{
			healthBar?.Delete();
			healthBar = null;
		}

		[Event.Frame]
		public void FrameUpdate()
		{
			var transform = Entity.Transform;
			var localCenter = Entity.PhysicsBody?.LocalMassCenter ?? Vector3.Zero;
			transform.Position = transform.Position + (localCenter * transform.Rotation);
			transform.Rotation = Rotation.LookAt( -CurrentView.Rotation.Forward );

			healthBar.Transform = transform;
		}

		[Event.Frame]
		public static void SystemUpdate()
		{
			float maxDistance = 256f;

			foreach ( var entity in Sandbox.Entity.All.OfType<FortwarsBlock>() )
			{
				void Remove()
				{
					var existingHealthBar = entity.Components.Get<HealthBarComponent>();
					existingHealthBar?.Remove();
				}

				if ( entity.Position.Distance( CurrentView.Position ) > maxDistance )
				{
					Remove();
					continue;
				}

				var tr = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * maxDistance ).Ignore( Local.Pawn ).Run();
				if ( tr.Entity != entity )
				{
					Remove();
					continue;
				}

				entity.Components.GetOrCreate<HealthBarComponent>();
			}
		}
	}
}
