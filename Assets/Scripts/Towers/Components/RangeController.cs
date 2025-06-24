using UnityEngine;

/// <summary>
/// Controls the range of a tower, including visual indication and applying modifiers.
/// </summary>
public class RangeController : MonoBehaviour
{
    /// <summary>
    /// The prefab to instantiate for visualizing the tower's range.
    /// </summary>
    [Tooltip("The prefab to instantiate for visualizing the tower's range.")]
    [SerializeField] private GameObject rangeIndicatorPrefab;
    /// <summary>
    /// Whether to draw the range radius as a gizmo in the editor.
    /// </summary>
    [Tooltip("Whether to draw the range radius as a gizmo in the editor.")]
    [SerializeField] private bool debugGizmos = true;

    /// <summary>
    /// The base range of the tower.
    /// </summary>
    [Tooltip("The base range of the tower.")]
    public float BaseRange;

    // The instantiated GameObject for the range indicator.
    private GameObject rangeIndicator;
    // Reference to the parent TowerController.
    private TowerController towerController;
    // The current calculated range radius of the tower.
    private float rangeRadius;
    // The total modifier applied to the base range.
    private float totalModifier = 0f;

    /// <summary>
    /// Retrieves the TowerController component from the parent.
    /// </summary>
    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    /// <summary>
    /// Instantiates the range indicator prefab (if assigned) and updates the initial radius.
    /// </summary>
    private void Start()
    {
        if (rangeIndicatorPrefab != null)
        {
            rangeIndicator = Instantiate(rangeIndicatorPrefab, transform);
            rangeIndicator.transform.localPosition = Vector3.zero;
            rangeIndicator.SetActive(false);
        }
        UpdateRadius();
    }

    /// <summary>
    /// Applies a modifier to the tower's range. The modifier is a relative value (e.g., 0.1 for a 10% increase).
    /// </summary>
    /// <param name="modifier">The modifier to apply to the range.</param>
    public void ApplyRadiusModifier(float modifier)
    {
        totalModifier += modifier;
        UpdateRadius();
    }

    // Calculates the final range radius based on the base range and total modifier, then updates the visual indicator and notifies the TowerController.
    private void UpdateRadius()
    {
        rangeRadius = Mathf.Max(0.1f, BaseRange * (1 + totalModifier));
        UpdateRangeIndicator();
        towerController.UpdateRangeDependencies();
    }

    // Updates the scale of the range indicator to match the current range radius.
    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (rangeRadius * 2);
        }
    }

    /// <summary>
    /// Shows the visual range indicator.
    /// </summary>
    public void ShowRangeRadius()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the visual range indicator.
    /// </summary>
    public void HideRangeRadius()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the current range radius of the tower.
    /// </summary>
    /// <returns>The current range radius.</returns>
    public float GetRangeRadius()
    {
        return rangeRadius;
    }

    /// <summary>
    /// Draws a wire sphere gizmo in the editor to visualize the tower's range.
    /// </summary>
    public void OnDrawGizmos()
    {
        if (!debugGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }

    /// <summary>
    /// Destroys the range indicator GameObject when this component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (rangeIndicator != null)
        {
            Destroy(rangeIndicator);
        }
    }
}