using UnityEngine;

public class BossFSM : MonoBehaviour
{
    public enum BossState
    {
        PhaseOne,    // 100% - 50% HP
        PhaseTwo,    // 50% - 25% HP
        PhaseThree   // 25% - 0% HP
    }

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private BossState currentState;

    private FloatingHealthBar healthBar;
    private EnemyShootLogic shootLogic;
    private void Start()
    {
        currentHealth = maxHealth;
        currentState = BossState.PhaseOne;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        shootLogic = GetComponent<EnemyShootLogic>();
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
        currentState = newState;
        
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
        }
    }

    private void HandlePhaseOne()
    {
        // Implement phase one behavior (100% - 50% HP)
        shootLogic.UseAttackPattern = false;
        Debug.Log("Entering Phase One");
    }

    private void HandlePhaseTwo()
    {
        // Implement phase two behavior (50% - 25% HP)
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(2);
        Debug.Log("Entering Phase Two");
    }

    private void HandlePhaseThree()
    {
        // Implement phase three behavior (25% - 0% HP)
        shootLogic.UseAttackPattern = true;
        shootLogic.SetPhase(3);
        Debug.Log("Entering Phase Three");
    }

    private void Update()
    {
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
        Debug.Log($"Boss hit by {other.gameObject.name}");        if (other.gameObject.CompareTag("PlayerDamage"))
        {
            TakeDamage(10f);
        }
    }
}
