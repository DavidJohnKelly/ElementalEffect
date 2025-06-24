using UnityEngine;

/// <summary>
/// Makes the attached GameObject always face the main camera.
/// </summary>
public class Billboard : MonoBehaviour
{
    // The main camera in the scene.
    private Camera mainCamera;

    /// <summary>
    /// Finds the main camera in the scene on start.
    /// </summary>
    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    /// <summary>
    /// Updates the GameObject's rotation in LateUpdate to ensure it faces the camera after the camera has moved.
    /// </summary>
    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                mainCamera.transform.rotation * Vector3.up);
        }
    }
}