using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseLogic : MonoBehaviour
{
    public float AggroRange = 8f;
    public float DeaggroRange = 10f;
    public float MoveSpeed = 1f;
    public float StopDistanceBuffer = 1f; // Buffer distance to stop at

    public float ChargeSpeed = 3f; // Faster than normal move speed
    public float ChargePreparationTime = 1f; // Time to wait before charging
    public float ChargeDuration = 2f; // How long to charge for

    public bool Aggroed = false;

    private Transform Player;
    private Rigidbody2D RB;

    private enum EnemyState
    {
        Base,
        Charging
    }

    private EnemyState currentState = EnemyState.Base;
    private float stateTimer = 0f;

    void Start()
    {
        Player = GameObject.Find("Player").transform;
        RB = GetComponent<Rigidbody2D>();
    }

    public void SetState(string stateName)
    {
        if (stateName == "Base")
        {
            currentState = EnemyState.Base;
            if (Player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
                Aggroed = distanceToPlayer <= AggroRange;
            }
        }
        else if (stateName == "Charging")
        {
            currentState = EnemyState.Charging;
            stateTimer = ChargePreparationTime;
            RB.linearVelocity = Vector2.zero;
            Aggroed = true;
        }
    }

    void Update()
    {
        if (Player == null) return;

        var dir = Player.position - transform.position;
        float distanceToPlayer = dir.magnitude;

        switch (currentState)
        {
            case EnemyState.Base:
                HandleBaseChasing(dir, distanceToPlayer);
                break;

            case EnemyState.Charging:
                HandleCharging(dir);
                break;
        }
    }

    private void HandleBaseChasing(Vector2 dir, float distanceToPlayer)
    {
        if (distanceToPlayer >= DeaggroRange)
        {
            Aggroed = false;
            RB.linearVelocity = Vector2.zero;
        }
        else if (distanceToPlayer <= AggroRange)
        {
            Aggroed = true;
            if (distanceToPlayer > StopDistanceBuffer)
            {
                transform.up = dir.normalized;
                RB.linearVelocity = transform.up * MoveSpeed;
            }
        }
    }

    private void HandleCharging(Vector2 dir)
    {
        Aggroed = true;
        stateTimer -= Time.deltaTime;
        
        if (stateTimer <= 0)
        {
            if (RB.linearVelocity.magnitude < 0.1f) // We were stopping, now start charging
            {
                transform.up = dir.normalized;
                RB.linearVelocity = transform.up * ChargeSpeed;
                stateTimer = ChargeDuration;
            }
            else // We were charging, now stop
            {
                RB.linearVelocity = Vector2.zero;
                stateTimer = ChargePreparationTime;
            }
        }
    }
}
