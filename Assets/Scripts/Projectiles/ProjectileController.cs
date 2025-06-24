using UnityEngine;

/// <summary>
/// Controls the behavior of a projectile, including its movement, damage, and sound effects.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    /// <summary>
    /// Settings related to the elemental properties of the projectile.
    /// </summary>
    [Header("Element Settings")]
    /// <summary>
    /// The elemental type of the projectile.
    /// </summary>
    [Tooltip("The elemental type of the projectile.")]
    public TowerElement Element;
    /// <summary>
    /// The base amount of elemental damage the projectile deals.
    /// </summary>
    [Tooltip("The base amount of elemental damage the projectile deals.")]
    public float ElementalDamage;
    /// <summary>
    /// A modifier to scale the elemental damage.
    /// </summary>
    [Tooltip("A modifier to scale the elemental damage.")]
    public float ElementalDamageModfier = 1f;
    /// <summary>
    /// The percentage of elemental damage that is converted to physical damage.
    /// </summary>
    [Tooltip("The percentage of elemental damage that is converted to physical damage.")]
    public float PhysicalDamagePercent;

    /// <summary>
    /// Settings related to the projectile's movement and travel.
    /// </summary>
    [Header("Travel Settings")]
    /// <summary>
    /// The direction vector in which the projectile will travel.
    /// </summary>
    [Tooltip("The direction vector in which the projectile will travel.")]
    public Vector3 Direction;
    /// <summary>
    /// The speed at which the projectile moves.
    /// </summary>
    [Tooltip("The speed at which the projectile moves.")]
    public float Speed;
    /// <summary>
    /// The maximum distance the projectile can travel before being destroyed.
    /// </summary>
    [Tooltip("The maximum distance the projectile can travel before being destroyed.")]
    public float MaxDistance;

    /// <summary>
    /// Settings related to the projectile's sound effects.
    /// </summary>
    [Header("Sound Settings")]
    /// <summary>
    /// The audio clip to play when the projectile is launched.
    /// </summary>
    [Tooltip("The audio clip to play when the projectile is launched.")]
    public AudioClip ShotSound;
    /// <summary>
    /// The audio clip to play when the projectile impacts an enemy.
    /// </summary>
    [Tooltip("The audio clip to play when the projectile impacts an enemy.")]
    public AudioClip ImpactSound;

    /// <summary>
    /// Settings indicating if the projectile was fired by the player.
    /// </summary>
    [Header("Player Settings")]
    /// <summary>
    /// Indicates whether this projectile was fired by the player.
    /// </summary>
    [Tooltip("Indicates whether this projectile was fired by the player.")]
    public bool IsFromPlayer = false;

    // The starting position of the projectile when it was enabled.
    private Vector3 startPosition;
    // The CapsuleCollider component attached to the projectile.
    private CapsuleCollider capsuleCollider;
    // The Rigidbody component attached to the projectile for physics-based movement.
    private Rigidbody rb;
    // The AudioSource component for playing the shot sound.
    private AudioSource shotAudioSource;
    // The AudioSource component for playing the impact sound.
    private AudioSource impactAudioSource;

    /// <summary>
    /// Initializes the projectile's starting position, gets necessary components, and applies initial velocity.
    /// </summary>
    private void OnEnable()
    {
        startPosition = transform.position;
        capsuleCollider = GetComponent<CapsuleCollider>();
        shotAudioSource = GetComponent<AudioSource>();
        impactAudioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Direction * Speed;
        }
    }

    /// <summary>
    /// Checks if the projectile has exceeded its maximum travel distance and destroys it if so.
    /// </summary>
    private void FixedUpdate()
    {
        if (MaxDistance <= 0)
        {
            Debug.LogWarning("Distance is not set for the projectile!");
            return;
        }
        if (Vector3.Distance(startPosition, transform.position) > MaxDistance)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the projectile's collider enters another collider. Handles damage application to enemies.
    /// </summary>
    /// <param name="other">The other Collider involved in the collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out EnemyController enemyController))
            {
                DamageTextFactory.Instance.CreateDamageText(transform.position, Element, ElementalDamage, PhysicalDamagePercent);
                enemyController.HandleElementalDamage(Element, ElementalDamage * ElementalDamageModfier);
                enemyController.HandlePhysicalDamage(ElementalDamage * ElementalDamageModfier * PhysicalDamagePercent, IsFromPlayer);

                if (ImpactSound != null && impactAudioSource != null && impactAudioSource.isActiveAndEnabled)
                {
                    impactAudioSource.PlayOneShot(ImpactSound);
                }
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Enables or disables the projectile's collision detection and plays the shot sound if enabled.
    /// </summary>
    /// <param name="enabled">True to enable collision, false to disable.</param>
    public void EnableCollision(bool enabled)
    {
        capsuleCollider.enabled = enabled;

        if (enabled)
        {
            if (ShotSound != null && shotAudioSource != null && shotAudioSource.isActiveAndEnabled)
            {
                shotAudioSource.PlayOneShot(ShotSound);
            }
        }
    }
}