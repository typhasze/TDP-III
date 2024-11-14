using UnityEngine;

public class BossFSM : MonoBehaviour
{
    public enum BossState
    {
        PhaseOne,    // 100% - 50% HP
        PhaseTwo,    // 50% - 25% HP
        PhaseThree,  // 25% - 0% HP
        Angry,       // Special behavior
        FinalStand   // Final phase at 0 HP
    }

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageTaken = 5f;
    private float currentHealth;
    private BossState currentState;

    private FloatingHealthBar healthBar;
    private EnemyShootLogic shootLogic;
    private EnemyChaseLogic chaseLogic;

    private float angerTimer = 0f;
    [SerializeField] private float angerDuration = 5f;
    [SerializeField] private float angerInterval = 10f;
    private BossState previousState;

    [SerializeField] private int wallsRequired = 2; // Number of walls that need to be destroyed
    private int wallsDestroyed = 0;
    private bool isAngryFromWalls = false;

    private BossAngerWall[] angerWalls;

    [SerializeField] private SpriteRenderer bossRenderer;

    [SerializeField] private float shootCooldown = 1f;

    [SerializeField] private float maxShield = 3f;
    private float currentShield;
    private FloatingShieldBar shieldBar;

    [SerializeField] private float finalStandDuration = 20f;
    [SerializeField] private float finalStandShootCooldown = 0.1f;
    [SerializeField] private float finalStandProjectileSpeed = 20f;
    private float finalStandTimer = 0f;
    private Vector3 centerPosition;

    [SerializeField] private float finalStandAttackDuration = 10f;
    private bool isFinalAttackStarted = false;

    [SerializeField] private GameObject aoeEffectPrefab; // Assign a sprite or particle effect in inspector
    [SerializeField] private float maxAoeRadius = 20f;   // Maximum size of the AOE
    private GameObject currentAoe;
    private float startingAoeScale = 0.1f;  // Initial size of the AOE

    [SerializeField] private GameObject totemPrefab;
    [SerializeField] private Vector2 totem1Offset = new Vector2(0f, 2f);
    [SerializeField] private Vector2 totem2Offset = new Vector2(0f, -2f);
    private GameObject[] activeTotemObjects = new GameObject[2];
    private int totemsDestroyed = 0;

    [SerializeField] private GameObject shieldEffectPrefab; // Add this field
    private GameObject activeShieldEffect; // Add this field

    private void Start()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;  // Initialize shield
        currentState = BossState.PhaseOne;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        shieldBar = GetComponentInChildren<FloatingShieldBar>();  // Get shield bar reference
        shootLogic = GetComponent<EnemyShootLogic>();
        chaseLogic = GetComponent<EnemyChaseLogic>();
        angerWalls = FindObjectsByType<BossAngerWall>(FindObjectsSortMode.None);
        bossRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateBossColor();

        // Initialize shield bar
        if (shieldBar != null)
        {
            shieldBar.UpdateShieldBar(currentShield, maxShield);
        }

        centerPosition = Vector3.zero; // Set this to your arena's center position

        // Initialize shield effect if we start with shields
        UpdateShieldEffect();
    }

    public void TakeDamage(float damage)
    {
        // If we have shield, remove 1 shield point regardless of damage amount
        if (currentShield > 0)
        {
            currentShield -= 1f;  // Always subtract 1 shield point
            if (currentShield < 0) currentShield = 0;
            if (shieldBar != null)
            {
                shieldBar.UpdateShieldBar(currentShield, maxShield);
            }
            
            // Update shield effect when shield changes
            UpdateShieldEffect();
            return; // Don't damage health while shield is up
        }

        // Original health damage code
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        UpdateState();
    }

    private void UpdateState()
    {
        // Only skip normal state changes if angry (but allow Final Stand transition)
        if (currentState == BossState.Angry)
        {
            // Check for Final Stand transition even during angry state
            if (currentHealth <= 0f)
            {
                TransitionToState(BossState.FinalStand);
                return;
            }
            return;
        }

        // Skip if already in final stand
        if (currentState == BossState.FinalStand)
            return;

        float healthPercentage = (currentHealth / maxHealth) * 100f;

        if (currentHealth <= 0f)
        {
            TransitionToState(BossState.FinalStand);
        }
        else if (healthPercentage <= 25f && currentState != BossState.PhaseThree)
        {
            TransitionToState(BossState.PhaseThree);
        }
        else if (healthPercentage <= 50f && healthPercentage > 25f && currentState != BossState.PhaseTwo)
        {
            TransitionToState(BossState.PhaseTwo);
        }
        else if (healthPercentage > 50f && currentState != BossState.PhaseOne)
        {
            TransitionToState(BossState.PhaseOne);
        }
    }

    private void TransitionToState(BossState newState)
    {
        if (currentState == BossState.Angry)
        {
            ResetWalls();
            isAngryFromWalls = false;
        }

        currentState = newState;
        UpdateBossColor();
        
        switch (currentState)
        {
            case BossState.PhaseOne:
                HandlePhaseOne();
                break;
            case BossState.PhaseTwo:
                HandlePhaseTwo();
                break;
            case BossState.PhaseThree:
                HandlePhaseThree();
                break;
            case BossState.Angry:
                HandleAngryState();
                break;
            case BossState.FinalStand:
                HandleFinalStand();
                break;
        }
    }

    private void HandlePhaseOne()
    {
        // Implement phase one behavior (100% - 50% HP)
        shootLogic.ShootCooldown = shootCooldown;
        shootLogic.UseAttackPattern = false;
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase One");
    }

    private void HandlePhaseTwo()
    {
        // Implement phase two behavior (50% - 25% HP)
        shootLogic.ShootCooldown = shootCooldown;
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(2);
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase Two");
    }

    private void HandlePhaseThree()
    {
        // Implement phase three behavior (25% - 0% HP)
        shootLogic.ShootCooldown = shootCooldown;
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(3);
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase Three");
    }

    private void HandleAngryState()
    {
        // Implement angry state behavior
        shootLogic.ShootCooldown = 100f;
        shootLogic.UseAttackPattern = false;
        chaseLogic.SetState("Charging");
        Debug.Log("Entering Angry State");
    }

    private void HandleFinalStand()
    {
        transform.position = centerPosition;
        shootLogic.ShootCooldown = 100f;
        shootLogic.UseAttackPattern = false;
        chaseLogic.enabled = true;
        isFinalAttackStarted = false;
        totemsDestroyed = 0;

        // Spawn totems
        SpawnTotems();

        // Create the initial AOE effect
        if (aoeEffectPrefab != null)
        {
            currentAoe = Instantiate(aoeEffectPrefab, centerPosition, Quaternion.identity);
            currentAoe.transform.localScale = Vector3.one * startingAoeScale;
        }
        
        Debug.Log("Entering Final Stand");
    }

    private void SpawnTotems()
    {
        if (totemPrefab != null)
        {
            // Spawn first totem
            Vector3 totem1Pos = centerPosition + new Vector3(totem1Offset.x, totem1Offset.y, 0);
            activeTotemObjects[0] = Instantiate(totemPrefab, totem1Pos, Quaternion.identity);

            // Spawn second totem
            Vector3 totem2Pos = centerPosition + new Vector3(totem2Offset.x, totem2Offset.y, 0);
            activeTotemObjects[1] = Instantiate(totemPrefab, totem2Pos, Quaternion.identity);
        }
    }

    public void WallDestroyed()
    {
        Debug.Log($"Wall destroyed. Count: {wallsDestroyed + 1}/{wallsRequired}");
        // Don't count wall destruction during Final Stand
        if (currentState == BossState.FinalStand)
            return;
        
        // Add validation to ensure we don't over-count
        if (wallsDestroyed < wallsRequired)
        {
            wallsDestroyed++;
            
            // Add shield when wall is destroyed (up to max shield)
            currentShield = Mathf.Min(currentShield + 1f, maxShield);
            if (shieldBar != null)
            {
                shieldBar.UpdateShieldBar(currentShield, maxShield);
            }
            
            UpdateShieldEffect();

            // Only trigger anger if we've destroyed exactly the required number
            if (wallsDestroyed == wallsRequired && !isAngryFromWalls && currentState != BossState.Angry)
            {
                isAngryFromWalls = true;
                previousState = currentState;
                TransitionToState(BossState.Angry);
                angerTimer = 0f;
            }
        }
    }

    private void ResetWalls()
    {
        wallsDestroyed = 0;
        foreach (var wall in angerWalls)
        {
            wall.ResetWall();
        }
    }

    private void Update()
    {
        if (currentState == BossState.FinalStand)
        {
            // Force position to stay at center
            transform.position = centerPosition;
            
            finalStandTimer += Time.deltaTime;

            // Update AOE size based on timer
            if (currentAoe != null && !isFinalAttackStarted)
            {
                float progress = finalStandTimer / finalStandDuration;
                float currentRadius = Mathf.Lerp(startingAoeScale, maxAoeRadius, progress);
                currentAoe.transform.localScale = Vector3.one * currentRadius;
            }
            
            // After 20 seconds, start the deadly attack
            if (finalStandTimer >= finalStandDuration && !isFinalAttackStarted)
            {
                // Destroy the AOE effect
                if (currentAoe != null)
                {
                    Destroy(currentAoe);
                }

                // Configure the deadly attack
                shootLogic.ShootCooldown = finalStandShootCooldown;
                shootLogic.BulletSpeed = finalStandProjectileSpeed;
                shootLogic.UseAttackPattern = false;
                shootLogic.Type = EnemyShootLogic.ShotType.AOE;
                isFinalAttackStarted = true;
            }
            
            // After attack duration, end the game
            if (isFinalAttackStarted && finalStandTimer >= (finalStandDuration + finalStandAttackDuration))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }
            return;
        }

        // Only check for wall-based anger if NOT in final stand
        if (currentState != BossState.Angry)
        {
            if (wallsDestroyed >= wallsRequired)
            {
                isAngryFromWalls = true;
                angerTimer += Time.deltaTime;
                if (angerTimer >= angerInterval)
                {
                    previousState = currentState;
                    TransitionToState(BossState.Angry);
                    angerTimer = 0f;
                }
            }
            else
            {
                isAngryFromWalls = false;
                angerTimer = 0f;
            }
        }
        else if (currentState == BossState.Angry)
        {
            angerTimer += Time.deltaTime;
            if (angerTimer >= angerDuration)
            {
                TransitionToState(previousState);
                angerTimer = 0f;
            }
        }

        #region Debugging
        // Set health to 75% (Phase One)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentHealth = maxHealth * 0.75f;
            UpdateState();
            Debug.Log($"Health set to: {currentHealth}/{maxHealth} ({(currentHealth/maxHealth)*100}%)");
        }
        // Set health to 40% (Phase Two)
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentHealth = maxHealth * 0.40f;
            UpdateState();
            Debug.Log($"Health set to: {currentHealth}/{maxHealth} ({(currentHealth/maxHealth)*100}%)");
        }
        // Set health to 20% (Phase Three)
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentHealth = maxHealth * 0.20f;
            UpdateState();
            Debug.Log($"Health set to: {currentHealth}/{maxHealth} ({(currentHealth/maxHealth)*100}%)");
        }
        // Full heal to 100%
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentHealth = maxHealth;
            UpdateState();
            Debug.Log($"Health set to: {currentHealth}/{maxHealth} ({(currentHealth/maxHealth)*100}%)");
        }
        #endregion

        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.CompareTag("PlayerDamage"))
        {
            TakeDamage(damageTaken);
        }
    }

    private void UpdateBossColor()
    {
        switch (currentState)
        {
            case BossState.PhaseOne:
                bossRenderer.material.color = Color.white; // Normal color
                break;
            case BossState.PhaseTwo:
                bossRenderer.material.color = new Color(1f, 0.7f, 0f); // Orange-ish
                break;
            case BossState.PhaseThree:
                bossRenderer.material.color = new Color(0.7f, 0f, 1f); // Purple-ish
                break;
            case BossState.Angry:
                bossRenderer.material.color = Color.red; // Crimson/red
                break;
            case BossState.FinalStand:
                bossRenderer.material.color = Color.red;
                break;
        }
    }

    // Add cleanup in case the boss is destroyed before finishing
    private void OnDestroy()
    {
        if (currentAoe != null)
        {
            Destroy(currentAoe);
        }

        foreach (GameObject totem in activeTotemObjects)
        {
            if (totem != null)
            {
                Destroy(totem);
            }
        }

        if (activeShieldEffect != null)
        {
            Destroy(activeShieldEffect);
        }
    }

    public void TotemDestroyed()
    {
        totemsDestroyed++;
        if (totemsDestroyed >= 2)
        {
            // Grant immortality to player
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                player.GrantImmortality();
            }
        }
    }

    private void UpdateShieldEffect()
    {
        if (currentShield > 0)
        {
            // Create shield effect if it doesn't exist
            if (activeShieldEffect == null && shieldEffectPrefab != null)
            {
                activeShieldEffect = Instantiate(shieldEffectPrefab, transform);
            }
        }
        else
        {
            // Destroy shield effect if it exists
            if (activeShieldEffect != null)
            {
                Destroy(activeShieldEffect);
                activeShieldEffect = null;
            }
        }
    }
}
