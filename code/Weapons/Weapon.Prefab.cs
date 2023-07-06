namespace Fortwars;

public partial class Weapon
{
	[Prefab, Net] public Model ViewModel { get; set; }

	/// <summary>
	/// The icon for this weapon, if applicable.
	/// </summary>
	[Prefab, ResourceType( "png" )]
	public string Icon { get; set; }

	/// <summary>
	/// The multiplier this weapon has on the movement speed of its owner.
	/// </summary>
	[Prefab, Net] public float MovementSpeedMultiplier { get; set; } = 1.0f;

	/// <summary>
	/// The inventory slot this weapon should belong to.
	/// </summary>
	[Prefab, Net] public InventorySlot InventorySlot { get; set; } = InventorySlot.Primary;

	/// <summary>
	/// The HoldType of this weapon.
	/// </summary>
	[Prefab, Net] public HoldType HoldType { get; set; } = HoldType.None;

	/// <summary>
	/// The handedness of this weapon.
	/// </summary>
	[Prefab, Net] public Handedness Handedness { get; set; } = Handedness.Both;

	[Prefab, Net] public Vector3 PositionOffset { get; set; } = Vector3.Zero;

	/// <summary>
	/// Spawns and returns a Weapon from the Prefab Library.
	/// </summary>
	/// <param name="prefabName">The asset path to the prefab.</param>
	/// <returns>The weapon if spawned successfully, otherwise null.</returns>
	public static Weapon FromPrefab( string prefabName )
	{
		if ( PrefabLibrary.TrySpawn<Weapon>( prefabName, out var weapon ) )
		{
			return weapon;
		}

		return null;
	}

	/// <summary>
	/// Returns all prefabs from the Prefab Library that are of Weapon type.
	/// </summary>
	/// <returns>A collection of Prefabs with the Weapon type.</returns>
	public static IEnumerable<Prefab> GetAllWeaponPrefabs()
	{
		foreach ( var prefab in ResourceLibrary.GetAll<Prefab>() )
		{
			var prefabType = TypeLibrary.GetType( prefab.Root.Class );
			if ( prefabType is not null && prefabType.TargetType == typeof( Weapon ) )
			{
				yield return prefab;
			}
		}
	}
}
