// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class HealthBar : Panel
{
	private Label owner;
	private Label health;
	private Panel inner;

	private FortwarsBlock block;

	public HealthBar( FortwarsBlock block )
	{
		Parent = FortwarsHUD.Root;
		this.block = block;

		StyleSheet.Load( "/ui/world/HealthBar.scss" );
		health = Add.Label( "0", "health" );
		owner = Add.Label( "owner", "owner" );
		inner = Add.Panel( "inner" );
	}

	public override void Tick()
	{
		base.Tick();

		health.Text = $"{block.Health.CeilToInt()} / {block.MaxHealth.CeilToInt()}";
		inner.Style.Width = Length.Fraction( block.Health / block.MaxHealth );
		owner.Text = $"Owned by {block.Client?.Name ?? "(disconnected)"}";
	}
}
