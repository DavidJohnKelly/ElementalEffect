using UnityEngine;

/// <summary>
/// Controls the camera movement for the tower defense game, allowing free flight with movement, zoom, and rotation.
/// </summary>
public class TowerDefenceCamera : MonoBehaviour
{
    // Thanks to FreeFlyCamera at: https://assetstore.unity.com/packages/tools/camera/free-fly-camera-140739
    // Helped me learn more about camera movement,
    // and some nice layout options :)
    #region UI

    /// <summary>
    /// Settings related to camera movement.
    /// </summary>
    [Header("Movement")]
    /// <summary>
    /// The base speed of the camera movement.
    /// </summary>
    [Tooltip("The base speed of the camera movement.")]
    [SerializeField] private float movementBaseSpeed = 5f;
    /// <summary>
    /// The acceleration rate of the camera movement.
    /// </summary>
    [Tooltip("The acceleration rate of the camera movement.")]
    [SerializeField] private float movementAcceleration = 5f;
    /// <summary>
    /// The maximum speed the camera can reach during movement.
    /// </summary>
    [Tooltip("The maximum speed the camera can reach during movement.")]
    [SerializeField] private float movementMaxSpeed = 80f;
    /// <summary>
    /// The speed multiplier when the sprint key is held down.
    /// </summary>
    [Tooltip("The speed multiplier when the sprint key is held down.")]
    [SerializeField] private float movementSprintSpeed = 160f;
    /// <summary>
    /// A factor that influences movement speed based on the camera's height.
    /// </summary>
    [Tooltip("A factor that influences movement speed based on the camera's height.")]
    [SerializeField] private float movementHeightFactor = 3f;

    /// <summary>
    /// Limits for camera movement on the X and Z axes.
    /// </summary>
    [Header("Movement Limits")]
    /// <summary>
    /// The minimum allowed X position for the camera.
    /// </summary>
    [Tooltip("The minimum allowed X position for the camera.")]
    [SerializeField] private float xMinLimit;
    /// <summary>
    /// The maximum allowed X position for the camera.
    /// </summary>
    [Tooltip("The maximum allowed X position for the camera.")]
    [SerializeField] private float xMaxLimit;
    /// <summary>
    /// The minimum allowed Z position for the camera.
    /// </summary>
    [Tooltip("The minimum allowed Z position for the camera.")]
    [SerializeField] private float zMinLimit;
    /// <summary>
    /// The maximum allowed Z position for the camera.
    /// </summary>
    [Tooltip("The maximum allowed Z position for the camera.")]
    [SerializeField] private float zMaxLimit;

    /// <summary>
    /// Settings related to camera zooming.
    /// </summary>
    [Header("Zoom")]
    /// <summary>
    /// The base speed of the camera zooming.
    /// </summary>
    [Tooltip("The base speed of the camera zooming.")]
    [SerializeField] private float zoomBaseSpeed = 5f;
    /// <summary>
    /// The acceleration rate of the camera zooming.
    /// </summary>
    [Tooltip("The acceleration rate of the camera zooming.")]
    [SerializeField] private float zoomAcceleration = 10f;
    /// <summary>
    /// The maximum speed the camera can reach during zooming.
    /// </summary>
    [Tooltip("The maximum speed the camera can reach during zooming.")]
    [SerializeField] private float zoomMaxSpeed = 100f;
    /// <summary>
    /// A factor that influences zoom speed based on the camera's height.
    /// </summary>
    [Tooltip("A factor that influences zoom speed based on the camera's height.")]
    [SerializeField] private float zoomHeightFactor = 3f;

    /// <summary>
    /// Settings related to camera rotation.
    /// </summary>
    [Header("Rotation")]
    /// <summary>
    /// The speed of the camera rotation.
    /// </summary>
    [Tooltip("The speed of the camera rotation.")]
    [SerializeField] private float rotationSpeed = 2f;

    /// <summary>
    /// Limits for the camera's height (Y position).
    /// </summary>
    [Header("Height Limits")]
    /// <summary>
    /// The minimum allowed height for the camera.
    /// </summary>
    [Tooltip("The minimum allowed height for the camera.")]
    [SerializeField] private float heightMin = 10f;
    /// <summary>
    /// The maximum allowed height for the camera.
    /// </summary>
    [Tooltip("The maximum allowed height for the camera.")]
    [SerializeField] private float heightMax = 100f;

    #endregion UI

    // The current target velocity of the camera.
    private Vector3 targetVelocity = Vector3.zero;
    // The current target height for the camera.
    private float targetHeight;
    // The current rotation angles of the camera (yaw and pitch).
    private Vector2 rotation;
    // A factor representing the camera's height within the height limits (0 to 1).
    private float heightFactor;

    /// <summary>
    /// Initializes the camera's starting position and rotation.
    /// </summary>
    private void Start()
    {
        targetHeight = transform.position.y;
        rotation.x = transform.eulerAngles.y;
        rotation.y = transform.eulerAngles.x;
        heightFactor = Mathf.InverseLerp(heightMin, heightMax, targetHeight);
    }

    /// <summary>
    /// Updates the camera's state every frame, handling movement, zoom, rotation, and reset.
    /// </summary>
    private void Update()
    {
        heightFactor = Mathf.InverseLerp(heightMin, heightMax, targetHeight);
        HandleMovement();
        HandleZoom();
        HandleRotation();
        HandleReset();
    }

    // Handles the camera movement based on input.
    private void HandleMovement()
    {
        Vector3 forward = new Vector3(transform.up.x, 0, transform.up.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 moveDirection = Vector3.zero;

        bool is_moving = false;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += forward;
            is_moving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= forward;
            is_moving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= right;
            is_moving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += right;
            is_moving = true;
        }

        if (is_moving)
        {
            moveDirection.Normalize();

            // Dynamically adjust movement speed based on height
            float dynamicMovementSpeed = Mathf.Lerp(movementBaseSpeed, movementMaxSpeed, heightFactor) * movementHeightFactor;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                dynamicMovementSpeed = Mathf.Lerp(dynamicMovementSpeed, movementSprintSpeed, heightFactor) * movementHeightFactor;
            }

            // Add some acceleration to the movement, feels better generally
            targetVelocity = Vector3.Lerp(targetVelocity, moveDirection * dynamicMovementSpeed, movementAcceleration * Time.deltaTime);
            transform.position += Time.deltaTime * targetVelocity;

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, xMinLimit, xMaxLimit),
                transform.position.y,
                Mathf.Clamp(transform.position.z, zMinLimit, zMaxLimit)
            );
        }
        else
        { // If not moving, stop instantaneously with no decleartion
            targetVelocity = Vector3.zero;
        }
    }

    // Handles the camera zooming based on input.
    private void HandleZoom()
    {
        float zoom = 0f;
        bool q_pressed = Input.GetKey(KeyCode.Q);
        bool e_pressed = Input.GetKey(KeyCode.E);

        // If both pressed then just rely on mouse zooming
        if (q_pressed && !e_pressed)
        {
            zoom = -1f; // Zoom down
        }
        else if (e_pressed && !q_pressed)
        {
            zoom = 1f; // Zoom up
        }

        if (zoom != 0)
        {
            float dynamicZoomSpeed = Mathf.Lerp(zoomBaseSpeed, zoomMaxSpeed, heightFactor) * zoomHeightFactor;
            targetHeight = Mathf.Clamp(targetHeight + zoom * dynamicZoomSpeed * Time.deltaTime, heightMin, heightMax);
            float newHeight = Mathf.Lerp(transform.position.y, targetHeight, zoomAcceleration * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
        else
        {
            targetHeight = transform.position.y;
        }
    }

    // Handles the camera rotation based on mouse input.
    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
            rotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;

            rotation.y = Mathf.Clamp(rotation.y, 0, 90);
        }

        transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0);
    }

    // Handles the camera reset based on input.
    private void HandleReset()
    {
        // Reset camera rotation if R pressed
        if (Input.GetKey(KeyCode.R))
        {
            rotation.y = 90f;
            transform.rotation = Quaternion.Euler(90f, rotation.x, 0f);
        }
    }
}