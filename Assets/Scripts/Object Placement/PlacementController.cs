using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// Controls the placement and selection of objects in the game world.
/// </summary>
public class PlacementController : MonoBehaviour
{
    /// <summary>
    /// The LayerMask for objects that can be selected.
    /// </summary>
    [Tooltip("The LayerMask for objects that can be selected.")]
    [SerializeField] private LayerMask objectLayer;
    /// <summary>
    /// The audio clip to play when an object is successfully placed.
    /// </summary>
    [Tooltip("The audio clip to play when an object is successfully placed.")]
    public AudioClip PlacementSound;

    /// <summary>
    /// Singleton instance of the PlacementController.
    /// </summary>
    public static PlacementController Instance { get; private set; }

    // The ObjectPlacer component responsible for handling object placement.
    private ObjectPlacer objectPlacer;
    // The ObjectSelector component responsible for handling object selection.
    private ObjectSelector objectSelector;
    // The AudioSource component used to play placement sounds.
    private AudioSource audioSource;
    // The main camera used for raycasting.
    private Camera placementCamera;
    // An array to store raycast hits, used for non-alloc raycasting.
    private readonly RaycastHit[] raycastHits = new RaycastHit[10];

    /// <summary>
    /// Initializes the singleton instance and retrieves necessary components.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            objectPlacer = GetComponentInChildren<ObjectPlacer>();
            objectSelector = GetComponentInChildren<ObjectSelector>();
            placementCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Handles input for object placement and selection each frame.
    /// </summary>
    private void Update()
    {
        Ray ray = placementCamera.ScreenPointToRay(Input.mousePosition);

        bool overUI = IsOverUI();
        bool overTower = IsOverSelectable(ray);
        bool overTerrain = IsOverTerrain(ray, out var hitPoint);

        objectPlacer.UpdatePreview(hitPoint);

        objectPlacer.CanPlace = (!overUI) && (!overTower) && overTerrain;
        objectSelector.CanSelect = !overUI;

        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            if (objectPlacer.CanPlace)
            {
                objectPlacer.HandlePlacement();
                audioSource.clip = PlacementSound;
                audioSource.loop = false;
                audioSource.Play();
            }
            if (objectSelector.CanSelect && !objectPlacer.CanPlace)
            {
                bool validSelection = objectSelector.TrySelectObject(ray, objectLayer);
                if (validSelection)
                {
                    objectPlacer.CancelMovement();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            PlaceableObject selectedObject = objectSelector.SelectedObject;
            objectPlacer.StartMovingObject(selectedObject);

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            objectPlacer.CancelMovement();
            objectPlacer.ObjectToPlace = null;
            objectSelector.SelectedObject = null;
        }
    }

    // Checks if the mouse pointer is currently over a UI element.
    private bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Checks if the raycast hits the terrain on the placement layer.
    private bool IsOverTerrain(Ray ray, out RaycastHit hitPoint)
    {
        hitPoint = default;

        if (objectPlacer.ObjectToPlace != null)
        {
            return Physics.Raycast(ray, out hitPoint, Mathf.Infinity, objectPlacer.ObjectToPlace.PlacementLayer);
        }

        return false;
    }

    // Checks if the raycast hits a selectable object (excluding the preview object).
    private bool IsOverSelectable(Ray ray)
    {
        int hits = Physics.RaycastNonAlloc(ray, raycastHits, Mathf.Infinity, objectLayer);
        for (int i = 0; i < hits; i++)
        {
            GameObject objectHit = raycastHits[i].collider.gameObject;

            if (objectHit.transform.parent == null) continue;

            GameObject objectParent = objectHit.transform.parent.gameObject;

            if (objectParent.CompareTag("Selectable"))
            {
                if (objectParent != objectPlacer.PreviewObject)
                {
                    return true;
                }
            }
        }

        return false;
    }
}