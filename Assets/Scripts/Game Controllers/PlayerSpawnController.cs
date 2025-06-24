using UnityEngine;

/// <summary>
/// Controls the spawning location of a player object.
/// </summary>
public class PlayerSpawnController : MonoBehaviour
{
    /// <summary>
    /// The vertical offset applied to the player's spawn position.
    /// </summary>
    [Tooltip("The vertical offset applied to the player's spawn position.")]
    [SerializeField] private float playerHeightOffset = 1;

    private Vector3 offset;

    private void Start()
    {
        offset = new Vector3(0, playerHeightOffset, 0);
    }

    /// <summary>
    /// Sets the provided player's position and rotation to this object's location.
    /// </summary>
    /// <param name="player">The GameObject of the player to spawn.</param>
    public void SetSpawnLocation(GameObject player)
    {
        player.transform.SetParent(gameObject.transform);
        player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        player.transform.localScale = Vector3.one;
        player.transform.SetParent(null);
    }
}