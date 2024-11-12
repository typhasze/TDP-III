/*******************************************************************************
File:      BulletLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component is added to the bullet and controls all of its behavior,
    including how to handle when different objects are hit.

*******************************************************************************/
using UnityEngine;

public enum Teams { Player, Enemy }

public class BulletLogic : MonoBehaviour
{
    public Teams Team = Teams.Player;
    public int Power = 1;
    public bool IsHoming { get; set; }
    public float HomingStrength { get; set; }
    public float HomingDuration { get; set; }

    private float homingTimer;
    private Rigidbody2D rb;
    private GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (IsHoming)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            homingTimer = HomingDuration;
        }
    }

    void FixedUpdate()
    {
        if (IsHoming && player != null && homingTimer > 0)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Vector2 currentVelocity = rb.linearVelocity;
            float speed = currentVelocity.magnitude;  // Store current speed
            
            // Lerp between current direction and direction to player
            Vector2 newDirection = Vector2.Lerp(currentVelocity.normalized, direction, HomingStrength * Time.fixedDeltaTime);
            
            // Apply the original speed to the new direction
            rb.linearVelocity = newDirection.normalized * speed;
            transform.up = newDirection.normalized;
            
            homingTimer -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger || col.tag == Team.ToString())
            return;

        Destroy(gameObject);
    }
}
