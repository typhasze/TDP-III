using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseLogic : MonoBehaviour
{
    public float AggroRange = 8f;
    public float DeaggroRange = 10f;
    public float MoveSpeed = 1f;
    public float StopDistanceBuffer = 1f; // Buffer distance to stop at

    public bool Aggroed = false;

    private Transform Player;
    private Rigidbody2D RB;

    void Start()
    {
        Player = GameObject.Find("Player").transform;
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // No reference to player, nothing to chase
        if (Player == null || !Player.gameObject)
        {
            RB.linearVelocity = Vector2.zero;
            Aggroed = false;
            return;
        }

        // Calculate direction and distance to the player
        var dir = Player.position - transform.position;
        float distanceToPlayer = dir.magnitude;

        // If player is within aggro range and beyond the buffer distance, chase
        if (distanceToPlayer <= AggroRange && distanceToPlayer > StopDistanceBuffer)
        {
            // Rotate to face the player
            transform.up = dir.normalized;

            Aggroed = true;

            // Move towards the player
            RB.linearVelocity = transform.up * MoveSpeed;
        }
        else if (distanceToPlayer >= DeaggroRange || distanceToPlayer <= StopDistanceBuffer)
        {
            // Stop chasing if beyond deaggro range or within stop buffer
            Aggroed = false;
            RB.linearVelocity = Vector2.zero;
        }
    }
}
