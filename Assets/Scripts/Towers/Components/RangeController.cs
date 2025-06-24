using UnityEngine;

public class RangeController : MonoBehaviour
{
    [SerializeField] private float rangeRadius = 10f;
    [SerializeField] private GameObject rangeIndicatorPrefab;
    [SerializeField] private bool debugGizmos = true;

    private GameObject rangeIndicator;

    private TowerController towerController;

    private void Awake()
    {
        towerController = GetComponentInParent<TowerController>();
    }

    private void Start()
    {
        if (rangeIndicatorPrefab != null)
        {
            rangeIndicator = Instantiate(rangeIndicatorPrefab, transform);
            rangeIndicator.transform.localPosition = Vector3.zero;
            UpdateRangeIndicator();
            rangeIndicator.SetActive(false); // Hide initially
        }
        SetRangeRadius(rangeRadius);
    }

    public void SetRangeRadius(float radius)
    {
        rangeRadius = radius;
        UpdateRangeIndicator();
        towerController.HandleRadiusUpdate(radius);
    }

    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (rangeRadius * 2);
        }
    }

    public void ShowRangeRadius()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }
    }

    public void HideRangeRadius()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    public float GetRangeRadius()
    {
        return rangeRadius;
    }

    public void AddRangeRadius(float rad)
    {
        SetRangeRadius(rangeRadius + rad);
    }

    public void OnDrawGizmos()
    {
        if (!debugGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }

}
