using UnityEngine;

public class BossFSM : MonoBehaviour
{
    public enum BossState
    {
        PhaseOne,    // 100% - 50% HP
        PhaseTwo,    // 50% - 25% HP
        PhaseThree,  // 25% - 0% HP
        Angry        // Special behavior
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
            return; // Don't damage health while shield is up
        }

        // Original health damage code
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        UpdateState();
    }

    private void UpdateState()
    {
        // Skip state changes if currently angry
        if (currentState == BossState.Angry)
            return;

        float healthPercentage = (currentHealth / maxHealth) * 100f;

        if (healthPercentage <= 25f && currentState != BossState.PhaseThree)
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
        else if (currentHealth <= 0f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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

    public void WallDestroyed()
    {
        wallsDestroyed++;
        
        // Add shield when wall is destroyed (up to max shield)
        currentShield = Mathf.Min(currentShield + 1f, maxShield);
        if (shieldBar != null)
        {
            shieldBar.UpdateShieldBar(currentShield, maxShield);
        }

        if (wallsDestroyed >= wallsRequired && !isAngryFromWalls && currentState != BossState.Angry)
        {
            isAngryFromWalls = true;
            previousState = currentState;
            TransitionToState(BossState.Angry);
            angerTimer = 0f;
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
        else
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
        }
    }
}
