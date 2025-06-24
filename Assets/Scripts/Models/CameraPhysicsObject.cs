using UnityEngine;

/// <summary>
/// Makes a Rigidbody-based object follow a specified pivot point using spring physics.
/// </summary>
public class CameraPhysicsObject : MonoBehaviour
{
    /// <summary>
    /// The Transform that this object will follow.
    /// </summary>
    [Tooltip("The Transform that this object will follow.")]
    public Transform pivotPoint;
    /// <summary>
    /// The maximum distance this object can be from the pivot point before the spring force is applied.
    /// </summary>
    [Tooltip("The maximum distance this object can be from the pivot point.")]
    public float maxDistance = 0.1f;
    /// <summary>
    /// The strength of the spring force pulling this object towards the pivot point.
    /// </summary>
    [Tooltip("The strength of the spring force pulling this object towards the pivot point.")]
    public float springForce = 50f;
    /// <summary>
    /// The amount of damping applied to the object's velocity to prevent excessive oscillation.
    /// </summary>
    [Tooltip("The amount of damping applied to the object's velocity to prevent excessive oscillation.")]
    public float damping = 25f;

    // The Rigidbody component attached to this object.
    private Rigidbody rb;
    // The initial offset vector between this object and the pivot point.
    private Vector3 offset;

    /// <summary>
    /// Gets the Rigidbody component and calculates the initial offset from the pivot point.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        offset = transform.position - pivotPoint.position;
    }

    /// <summary>
    /// Applies a spring force to move this object towards the pivot point if it exceeds the maximum distance.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 targetPosition = pivotPoint.position + pivotPoint.TransformDirection(offset);

        Vector3 displacement = rb.position - targetPosition;
        float distance = displacement.magnitude;

        if (distance > maxDistance)
        {
            Vector3 springDirection = displacement.normalized;
            Vector3 springForceVector = (distance - maxDistance) * springForce * -springDirection;
            rb.AddForce(springForceVector);
            rb.AddForce(-rb.linearVelocity * damping);
        }
    }
}