namespace Fortwars;

public static class Commands
{
	[ConVar.Client( "fw_viewmodel_fov", Help = "ViewModel field of view", Min = 50f, Max = 90f )]
	public static float ViewModelFov { get; set; } = 90f;
}
