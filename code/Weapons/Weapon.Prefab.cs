namespace Fortwars;

public partial class Weapon
{
	[Prefab] public Prefab ViewModelPrefab { get; set; }

	/// <summary>
	/// The icon for this weapon, if applicable.
	/// </summary>
	[Prefab, ResourceType( "png" )]
	public string Icon { get; set; }

	/// <summary>
	/// The multiplier this weapon has on the movement speed of its owner.
	/// </summary>
	[Prefab] public float MovementSpeedMultiplier { get; set; } = 1.0f;

	/// <summary>
	/// The inventory slot this weapon should belong to.
	/// </summary>
	[Prefab] public InventorySlot InventorySlot { get; set; } = InventorySlot.Primary;

	/// <summary>
	/// The HoldType of this weapon.
	/// </summary>
	[Prefab] public HoldType HoldType { get; set; } = HoldType.None;

	/// <summary>
	/// The handedness of this weapon.
	/// </summary>
	[Prefab] public Handedness Handedness { get; set; } = Handedness.Both;
}
