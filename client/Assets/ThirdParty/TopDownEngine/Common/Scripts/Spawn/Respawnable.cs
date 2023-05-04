namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// Interface for player respawn
	/// </summary>
	public interface Respawnable
	{
		void OnPlayerRespawn(CheckPoint checkpoint, Character player);
	}
}