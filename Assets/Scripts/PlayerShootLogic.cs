using UnityEngine;

public class PlayerShootLogic : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float BulletSpeed = 5.0f;
    public float ShotCooldown = 1.0f;
    public float BulletSpawnOffset = 0.5f; // Distance to spawn bullet in front of player

    private float Timer = 1.0f;

    void Start()
    {
        
    }

    void Update()
    {
        // Rotate player towards mouse position
        var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 10f);
        transform.up = (worldMousePos - transform.position).normalized;

        Timer += Time.deltaTime;

        if (Timer >= ShotCooldown && Input.GetMouseButton(0))
        {
            // Calculate bullet spawn position offset from player
            Vector3 bulletSpawnPosition = transform.position + transform.up * BulletSpawnOffset;

            // Spawn Bullet slightly in front of player
            var obj = Instantiate(BulletPrefab, bulletSpawnPosition, Quaternion.identity);

            // Rotate bullet to match player direction
            obj.transform.up = transform.up;

            // Add bullet velocity
            obj.GetComponent<Rigidbody2D>().linearVelocity = transform.up * BulletSpeed;

            // Reset shoot timer
            Timer = 0f;
        }
    }
}
