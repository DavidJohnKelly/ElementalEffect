using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles player shooting, stamina management, and staff mechanics in an FPS game.
/// </summary>
public class FPSPlayerController : MonoBehaviour
{
    [Header("Shooting Settings")]
    [Tooltip("Base speed of fired projectiles.")]
    [SerializeField] private float baseShootingSpeed = 250f;
    [Tooltip("Delay between consecutive shots (seconds).")]
    [SerializeField] float fireRate = 1.0f;
    [Tooltip("Maximum time a projectile can be charged (seconds).")]
    [SerializeField] private float maxChargeTime = 1.0f;

    [Header("Stamina Settings")]
    [Tooltip("Maximum stamina value.")]
    [SerializeField] private float maxStamina = 100;
    [Tooltip("Stamina regained per second when idle.")]
    [SerializeField] private float staminaRechargeRate = 10f;
    [Tooltip("Stamina drained per second while running.")]
    [SerializeField] private float staminaRunningDrainRate = 10;
    [Tooltip("Stamina drained per second while charging a shot.")]
    [SerializeField] private float staminaChargingDrainRate = 15f;
    [Tooltip("Delay before stamina starts recharging after use (seconds).")]
    [SerializeField] private float staminaRegenDelay = 1f;
    [Tooltip("Minimum stamina required to start running.")]
    [SerializeField] private float minStaminaToRun = 10f;
    [Tooltip("Minimum stamina required to shoot.")]
    [SerializeField] private float minStaminaToShoot = 15f;

    [Header("Projectile Spawn Settings")]
    [Tooltip("Maximum distance a projectile can travel.")]
    [SerializeField] private float maxProjectileDistance = 250f;
    [Tooltip("Distance from camera where projectiles spawn while charging.")]
    [SerializeField] private float projectileSpawnBuffer = 2f;
    [Tooltip("Maximum scale a charged projectile can reach.")]
    [SerializeField] private float maxProjectileScale = 0.4f;

    [Header("Projectile Damage Settings")]
    [Tooltip("Base elemental damage dealt by projectiles.")]
    [SerializeField] private float elementalDamage = 20.0f;
    [Tooltip("Proportion of physical damage for Air element (0-1).")]
    [SerializeField] private float airPhysicalDamageProportion = 0.3f;
    [Tooltip("Proportion of physical damage for Earth element (0-1).")]
    [SerializeField] private float earthPhysicalDamageProportion = 0.9f;
    [Tooltip("Proportion of physical damage for Electric element (0-1).")]
    [SerializeField] private float electricPhysicalDamageProportion = 0.4f;
    [Tooltip("Proportion of physical damage for Fire element (0-1).")]
    [SerializeField] private float firePhysicalDamageProportion = 0.4f;
    [Tooltip("Proportion of physical damage for Ice element (0-1).")]
    [SerializeField] private float icePhysicalDamageProportion = 0.5f;
    [Tooltip("Proportion of physical damage for Water element (0-1).")]
    [SerializeField] private float waterPhysicalDamageProportion = 0.4f;

    [Header("UI Data")]
    [Tooltip("UI warning shown when stamina is too low to shoot.")]
    [SerializeField] private GameObject staminaWarning;
    [Tooltip("Duration the stamina warning stays visible (seconds).")]
    [SerializeField] private float staminaWarningTimeout = 2f;
    [Tooltip("Text displaying current stamina.")]
    [SerializeField] private TMP_Text staminaDisplay;

    [Header("Staff Rotation Settings")]
    [Tooltip("Transform used to rotate the staff when aiming.")]
    [SerializeField] private Transform staffPivot;
    [Tooltip("Staff tilt angle when charging a shot (degrees).")]
    [SerializeField] private float aimRotationAmount = 15f;
    [Tooltip("Speed at which staff tilts when charging.")]
    [SerializeField] private float rotationSpeed = 10f;
    [Tooltip("Speed at which staff returns to default position.")]
    [SerializeField] private float returnSpeed = 20f;

    [Header("Staff Material Settings")]
    [Tooltip("Staff part whose material changes with selected element.")]
    [SerializeField] private GameObject staffMaterialTarget;

    private float nextFire;
    private Camera mainCamera;

    private bool isCharging;
    private float currentChargeTime;
    private GameObject chargingProjectile;

    private bool isRunning;
    private float currentStamina;
    private float timeSinceLastStaminaUse;
    private float warningTimer;
    private FirstPersonController firstPersonController;

    private GameObject[] projectiles;
    private int currentProjectile;

    private Quaternion originalStaffRotation;
    private Vector3 originalStaffPosition;
    private Material staffMaterial;

    private void Awake()
    {
        firstPersonController = GetComponentInChildren<FirstPersonController>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (staffPivot == null)
        {
            Debug.LogError("Error in FPSPlayerController! Staff pivot is not assigned in inspector!");
            return;
        }
        originalStaffRotation = staffPivot.localRotation;
        originalStaffPosition = staffPivot.localPosition;

        if (staffMaterialTarget == null)
        {
            Debug.LogError("Error in FPS Player Controller! staffMaterialTarget is not assigned in inspector!");
            return;
        }
        staffMaterial = staffMaterialTarget.GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Handles all input and stamina updates each frame.
    /// </summary>
    private void Update()
    {
        HandleMouseInput();
        HandleScrollInput();
        HandleNumInput();
        HandleStamina();
    }

    // Handles left mouse button input for charging/firing projectiles
    private void HandleMouseInput()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire && !isCharging)
        {
            if (currentStamina >= minStaminaToShoot)
            {
                StartCoroutine(ChargeAndFire());
            }
            else
            {
                ShowStaminaWarning();
            }
        }
    }

    // Displays the low stamina warning UI
    private void ShowStaminaWarning()
    {
        staminaWarning.SetActive(true);
        warningTimer = 0f;
    }

    // Handles mouse scroll input for projectile selection
    private void HandleScrollInput()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta > 0f)
        {
            SetCurrentProjectile(currentProjectile + 1);
        }
        else if (scrollDelta < 0f)
        {
            SetCurrentProjectile(currentProjectile - 1);
        }
    }

    // Handles number key input for projectile selection
    private void HandleNumInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetCurrentProjectile(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetCurrentProjectile(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetCurrentProjectile(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SetCurrentProjectile(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            SetCurrentProjectile(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            SetCurrentProjectile(5);
        }
    }

    /// <summary>
    /// Manages stamina drain/regen and updates UI.
    /// </summary>
    private void HandleStamina()
    {
        CheckIsRunning();

        bool isUsingStamina = isRunning || isCharging;

        if (isUsingStamina)
        {
            float drainRate = isCharging ? staminaChargingDrainRate : staminaRunningDrainRate;
            currentStamina -= drainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            timeSinceLastStaminaUse = 0f;
        }
        timeSinceLastStaminaUse += Time.deltaTime;

        if (timeSinceLastStaminaUse >= staminaRegenDelay && currentStamina < maxStamina)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        staminaDisplay.text = "Stamina: " + Mathf.RoundToInt(currentStamina) + "/ " + maxStamina;

        if (staminaWarning.activeSelf)
        {
            warningTimer += Time.deltaTime;

            if (warningTimer >= staminaWarningTimeout)
            {
                staminaWarning.SetActive(false);
            }
        }
    }

    // Updates running state based on shift key and stamina
    private void CheckIsRunning()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (isRunning && currentStamina <= minStaminaToRun)
        {
            isRunning = false;
        }

        firstPersonController.CanSprint = isRunning;
    }

    /// <summary>
    /// Changes the currently selected projectile type.
    /// </summary>
    /// <param name="newIndex">Index of the new projectile type.</param>
    private void SetCurrentProjectile(int newIndex)
    {
        if (isCharging) return;

        int maxIndex = projectiles.Length - 1;

        if (newIndex > maxIndex)
        {
            newIndex = 0;
        }
        else if (newIndex < 0)
        {
            newIndex = maxIndex;
        }

        currentProjectile = newIndex;

        Color staffColour = ColourFactory.Instance.GetColour(newIndex);
        staffMaterial.color = staffColour;
        staffMaterial.SetColor("_BaseColor", staffColour);
        staffMaterial.SetColor("_Color", staffColour);
    }

    /// <summary>
    /// Handles the projectile charging sequence.
    /// </summary>
    private IEnumerator ChargeAndFire()
    {
        GameObject projectilePrefab = projectiles[currentProjectile];
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned!");
            yield break;
        }

        isCharging = true;
        currentChargeTime = 0f;

        chargingProjectile = Instantiate(projectilePrefab);
        chargingProjectile.SetActive(true);

        if (chargingProjectile.TryGetComponent<ProjectileController>(out var projectileController))
        {
            projectileController.EnableCollision(false);
            projectileController.enabled = false;
        }

        yield return null;

        while (!Input.GetButtonUp("Fire1") && currentStamina > 0)
        {
            currentChargeTime += Time.deltaTime;
            float chargePercent = Mathf.Clamp01(currentChargeTime / maxChargeTime);
            float currentScale = Mathf.Lerp(0f, maxProjectileScale, chargePercent);
            chargingProjectile.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            UpdateBulletIndicator(chargingProjectile, currentScale);
            yield return null;
        }

        FireProjectile(chargingProjectile, Mathf.Clamp01(currentChargeTime / maxChargeTime));
        chargingProjectile = null;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        projectiles = new GameObject[6]
        {
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Air, TowerLevel.Two),
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Earth, TowerLevel.Two),
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Electric, TowerLevel.Two),
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Fire, TowerLevel.Two),
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Ice, TowerLevel.Two),
            ProjectileFactory.Instance.GetProjectile(TowerType.Aiming, TowerElement.Water, TowerLevel.Two)
        };

        SetCurrentProjectile(0);

        staminaWarning.SetActive(false);
        warningTimer = 0f;
        nextFire = 0f;
        currentChargeTime = 0f;
        currentStamina = maxStamina;
        timeSinceLastStaminaUse = 0f;
        isCharging = false;
        isRunning = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isCharging = false;
        isRunning = false;
        currentChargeTime = 0f;
        Destroy(chargingProjectile);
        staffPivot.localRotation = originalStaffRotation;
        staffPivot.localPosition = originalStaffPosition;
    }

    /// <summary>
    /// Smoothly rotates the staff when charging or returning to rest.
    /// </summary>
    private void LateUpdate()
    {
        if (isCharging)
        {
            Quaternion targetRotation = originalStaffRotation * Quaternion.Euler(aimRotationAmount, 0, 0);
            staffPivot.transform.localRotation = Quaternion.Slerp(
                staffPivot.transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            staffPivot.transform.localRotation = Quaternion.Slerp(
                staffPivot.transform.localRotation,
                originalStaffRotation,
                returnSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Fires a charged projectile with appropriate force and damage.
    /// </summary>
    private void FireProjectile(GameObject projectile, float chargePercent)
    {
        isCharging = false;

        if (projectile == null) return;

        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        if (projectile.TryGetComponent<ProjectileController>(out var projectileController))
        {
            projectileController.Direction = ray.direction;
            projectileController.Speed = baseShootingSpeed;
            projectileController.PhysicalDamagePercent = GetPhysicalDamageProportion(projectileController.Element);
            projectileController.ElementalDamage = elementalDamage * chargePercent;
            projectileController.MaxDistance = maxProjectileDistance;

            projectileController.EnableCollision(true);
            projectileController.enabled = true;
        }

        nextFire = Time.time + fireRate;
    }

    // Returns the physical damage proportion for the given element
    private float GetPhysicalDamageProportion(TowerElement element)
    {
        return element switch
        {
            TowerElement.Air => airPhysicalDamageProportion,
            TowerElement.Earth => earthPhysicalDamageProportion,
            TowerElement.Electric => electricPhysicalDamageProportion,
            TowerElement.Fire => firePhysicalDamageProportion,
            TowerElement.Ice => icePhysicalDamageProportion,
            TowerElement.Water => waterPhysicalDamageProportion,
            _ => airPhysicalDamageProportion,
        };
    }

    // Updates the position of the charging projectile indicator
    private void UpdateBulletIndicator(GameObject projectile, float currentScale)
    {
        if (projectile == null) return;

        float scaleOffset = currentScale / 2f;

        Vector3 spawnPosition = new(Screen.width / 2, Screen.height / 2, projectileSpawnBuffer + scaleOffset);
        projectile.transform.position = mainCamera.ScreenToWorldPoint(spawnPosition);
        projectile.transform.LookAt(mainCamera.transform);
    }
}