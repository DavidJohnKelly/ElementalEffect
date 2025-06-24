using UnityEngine;

/// <summary>
/// Triggers end-of-path damage to the player when an enemy reaches the goal.
/// </summary>
public class EndDamageController : MonoBehaviour
{
    [Tooltip("Amount of damage to apply when this enemy reaches the end")]
    [SerializeField] private float endDamage = 5f;

    /// <summary>
    /// Applies damage to the player via the GameController.
    /// </summary>
    public void TriggerEndDamage()
    {
        GameController.Instance.HandleEndDamage(endDamage);
    }
}
