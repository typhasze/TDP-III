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
    private float currentHealth;
    private BossState currentState;

    private FloatingHealthBar healthBar;
    private EnemyShootLogic shootLogic;
    private EnemyChaseLogic chaseLogic;

    private float angerTimer = 0f;
    private float angerDuration = 5f;
    private float angerInterval = 10f;
    private BossState previousState;

    [SerializeField] private int wallsRequired = 2; // Number of walls that need to be destroyed
    private int wallsDestroyed = 0;
    private bool isAngryFromWalls = false;

    private BossAngerWall[] angerWalls;

    [SerializeField] private SpriteRenderer bossRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        currentState = BossState.PhaseOne;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        shootLogic = GetComponent<EnemyShootLogic>();
        chaseLogic = GetComponent<EnemyChaseLogic>();
        angerWalls = FindObjectsByType<BossAngerWall>(FindObjectsSortMode.None);
        bossRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateBossColor();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        UpdateState();
    }

    private void UpdateState()
    {
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
            Destroy(gameObject);
        }
    }

    private void TransitionToState(BossState newState)
    {
        if (currentState == BossState.Angry)
        {
            ResetWalls();
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
        shootLogic.UseAttackPattern = false;
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase One");
    }

    private void HandlePhaseTwo()
    {
        // Implement phase two behavior (50% - 25% HP)
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(2);
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase Two");
    }

    private void HandlePhaseThree()
    {
        // Implement phase three behavior (25% - 0% HP)
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(3);
        chaseLogic.SetState("Base");
        Debug.Log("Entering Phase Three");
    }

    private void HandleAngryState()
    {
        // Implement angry state behavior
        shootLogic.UseAttackPattern = false;
        chaseLogic.SetState("Charging");
        Debug.Log("Entering Angry State");
    }

    public void WallDestroyed()
    {
        wallsDestroyed++;
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
            if (isAngryFromWalls)
            {
                angerTimer += Time.deltaTime;
                if (angerTimer >= angerInterval && currentState != BossState.Angry)
                {
                    previousState = currentState;
                    TransitionToState(BossState.Angry);
                    angerTimer = 0f;
                }
            }
        }
        else
        {
            angerTimer += Time.deltaTime;
            if (angerTimer >= angerDuration)
            {
                isAngryFromWalls = false;
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
            TakeDamage(10f);
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
