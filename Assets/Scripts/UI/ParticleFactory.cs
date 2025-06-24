using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A factory class responsible for creating and managing particle systems for different tower elements.
/// This class follows the Singleton pattern for easy access.
/// </summary>
public class ParticleFactory : MonoBehaviour
{
    /// <summary>
    /// The prefab used as the base for the particle system. Should contain a MeshFilter and Renderer.
    /// </summary>
    [Header("Particle System Settings")]
    [SerializeField] private GameObject particlePrefab;
    /// <summary>
    /// The number of particles to spawn per cubic meter of the spherical radius.
    /// </summary>
    [SerializeField] private float particlesPerCubedM = 0.01f;

    /// <summary>
    /// The initial speed of the spawned particles.
    /// </summary>
    [Header("Particle Appearance Settings")]
    [SerializeField] private float startSpeed = 0.1f;
    /// <summary>
    /// The minimum size of the spawned particles.
    /// </summary>
    [SerializeField] private float minParticleSize = 0.1f;
    /// <summary>
    /// The maximum size of the spawned particles.
    /// </summary>
    [SerializeField] private float maxParticleSize = 0.3f;
    /// <summary>
    /// The duration in seconds for the particles to fade in.
    /// </summary>
    [SerializeField] private float particleFadeInDuration = 0.2f;
    /// <summary>
    /// The duration in seconds for the particles to fade out.
    /// </summary>
    [SerializeField] private float particleFadeOutDuration = 0.3f;
    /// <summary>
    /// The total lifetime in seconds of the spawned particles.
    /// </summary>
    [SerializeField] private float particleLifetime = 1f;

    /// <summary>
    /// The singleton instance of the ParticleFactory.
    /// </summary>
    public static ParticleFactory Instance;

    // A cache to store instantiated particle prefabs for each tower element.
    private readonly Dictionary<TowerElement, GameObject> particleCache = new();

    /// <summary>
    /// Sets up the Singleton instance of the ParticleFactory and disables the particle prefab.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error in ParticleFactory! Instance is not null!");
            Destroy(this);
            return;
        }

        Instance = this;
        particlePrefab.SetActive(false);
    }

    /// <summary>
    /// Gets a cached particle prefab for the specified tower element. If not cached, it instantiates a new one and caches it.
    /// </summary>
    /// <param name="element">The TowerElement to get the particle for.</param>
    /// <returns>The GameObject representing the particle for the given element.</returns>
    public GameObject GetParticle(TowerElement element)
    {
        if (particleCache.ContainsKey(element))
        {
            return particleCache[element];
        }

        if (particlePrefab == null)
        {
            Debug.LogError("Error in ParticleFactory! circleParticlePrefab is not assigned.");
            return null;
        }

        GameObject particleObject = Instantiate(particlePrefab);
        if (particleObject.TryGetComponent<Renderer>(out var renderer))
        {
            Color particleColour = ColourFactory.Instance.GetColour(element);
            Material particleMaterial = renderer.material;
            particleMaterial.color = particleColour;
            particleMaterial.SetColor("_BaseColor", particleColour);
            particleMaterial.SetColor("_Color", particleColour);
        }

        particleCache.Add(element, particleObject);

        return particleObject;
    }

    /// <summary>
    /// Spawns a particle system using the provided particle prefab, attached to the given transform, with the specified spherical radius.
    /// </summary>
    /// <param name="particle">The particle prefab to use for the system.</param>
    /// <param name="parentTransform">The transform to attach the particle system to.</param>
    /// <param name="sphericalRadius">The radius of the spherical emission shape for the particle system.</param>
    public void SpawnParticleSystem(GameObject particle, Transform parentTransform, float sphericalRadius)
    {
        GameObject particleSystemObject = new("Particle System Object");
        particleSystemObject.transform.position = parentTransform.transform.position;
        particleSystemObject.transform.parent = parentTransform;

        ParticleSystem particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
        ParticleSystemRenderer particleSystemRenderer = particleSystemObject.GetComponent<ParticleSystemRenderer>();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        MeshFilter particleMeshFilter = particle.GetComponent<MeshFilter>();
        Renderer particleRenderer = particle.GetComponent<Renderer>();

        if (particleMeshFilter == null || particleRenderer == null)
        {
            Debug.LogError("Particle prefab is missing required components");
            Destroy(particleSystemObject);
            return;
        }

        Material particleMaterial = particleRenderer.material;
        particleSystemRenderer.mesh = particleMeshFilter.mesh;
        particleSystemRenderer.material = particleRenderer.material;

        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.duration = particleLifetime;
        mainModule.startLifetime = particleLifetime;
        mainModule.startSpeed = startSpeed;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        mainModule.loop = false;

        float volume = 4f / 3f * Mathf.PI * Mathf.Pow(sphericalRadius, 3);
        int particleCount = Mathf.CeilToInt(volume * particlesPerCubedM);

        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = true;
        ParticleSystem.Burst burst = new(0f, (short)particleCount);
        emissionModule.SetBursts(new ParticleSystem.Burst[] { burst });

        ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
        shapeModule.enabled = true;
        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
        shapeModule.radius = sphericalRadius;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        float fadeInEnd = particleFadeInDuration / particleLifetime;
        float fadeOutStart = (particleLifetime - particleFadeOutDuration) / particleLifetime;
        Gradient gradient = new();
        gradient.SetKeys(
            new GradientColorKey[] { new(particleMaterial.color, 0f), new(particleMaterial.color, 1f) },
            new GradientAlphaKey[] {
                new(0f, 0f),
                new(1f, fadeInEnd),
                new(1f, fadeOutStart),
                new(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        particleSystem.Play();

        Destroy(particleSystemObject, particleLifetime + 0.1f);

        return;
    }

    /// <summary>
    /// Spawns a particle system for the specified tower element, attached to the given transform, with the specified spherical radius.
    /// </summary>
    /// <param name="element">The TowerElement to spawn the particle system for.</param>
    /// <param name="parentTransform">The transform to attach the particle system to.</param>
    /// <param name="sphericalRadius">The radius of the spherical emission shape for the particle system.</param>
    public void SpawnParticleSystem(TowerElement element, Transform parentTransform, float sphericalRadius)
    {
        GameObject particle = GetParticle(element);
        SpawnParticleSystem(particle, parentTransform, sphericalRadius);
    }
}