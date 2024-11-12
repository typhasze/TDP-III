using UnityEngine;

[RequireComponent(typeof(EnemyChaseLogic))]
public class EnemyShootLogic : MonoBehaviour
{
    public enum ShotType { Single, Shotgun, Homing, AOE }

    public GameObject BulletPrefab;
    public ShotType Type = ShotType.Single;
    public int Power = 1;
    public float ShootCooldown = 1f;
    public float BulletSpeed = 8f;

    //Shotgun only properties
    public int ShotgunBullets = 3;
    public float ShotgunAngle = 0.5f;

    // Add new properties for attack patterns
    public bool UseAttackPattern = false;
    private int CurrentPhase = 2;
    private int currentPatternIndex = 0;
    private ShotType[] phase2Pattern = new[] { 
        ShotType.Single, 
        ShotType.Single, 
        ShotType.Shotgun 
    };
    private ShotType[] phase3Pattern = new[] { 
        ShotType.Shotgun,
        ShotType.Homing,
        ShotType.AOE
    };

    // Add homing bullet properties
    public float HomingStrength = 4f;
    public float HomingDuration = 10f;

    private EnemyChaseLogic ChaseBehavior;
    private float Timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ChaseBehavior = GetComponent<EnemyChaseLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ChaseBehavior.Aggroed)
            return;

        Timer += Time.deltaTime;

        if (Timer >= ShootCooldown)
        {
            if (UseAttackPattern)
            {
                // Use the appropriate pattern based on current phase
                var currentPattern = CurrentPhase == 2 ? phase2Pattern : phase3Pattern;
                ExecuteShot(currentPattern[currentPatternIndex]);
                
                // Move to next attack in pattern
                currentPatternIndex = (currentPatternIndex + 1) % currentPattern.Length;
            }
            else
            {
                ExecuteShot(Type);
            }

            Timer = 0;
        }
    }

    // Extract shooting logic to a separate method
    private void ExecuteShot(ShotType shotType)
    {
        if (shotType == ShotType.Single)
        {
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.up = transform.up;
            bullet.GetComponent<Rigidbody2D>().linearVelocity = transform.up * BulletSpeed;
            bullet.GetComponent<BulletLogic>().Power = Power;
        }
        else if (shotType == ShotType.Shotgun)
        {
            for (int i = 0; i < ShotgunBullets; i++)
            {
                var fwd = RotateVector(transform.up, -ShotgunAngle * ((float)ShotgunBullets / 2f) + (ShotgunAngle * i));
                var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.up = fwd.normalized;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = fwd * BulletSpeed;
                bullet.GetComponent<BulletLogic>().Power = Power;
            }
        }
        else if (shotType == ShotType.Homing)
        {
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.up = transform.up;
            
            // Add homing behavior
            var bulletRb = bullet.GetComponent<Rigidbody2D>();
            var bulletLogic = bullet.GetComponent<BulletLogic>();
            
            bulletRb.linearVelocity = transform.up * BulletSpeed;
            bulletLogic.Power = Power;
            bulletLogic.IsHoming = true;
            bulletLogic.HomingStrength = HomingStrength;
            bulletLogic.HomingDuration = HomingDuration;
        }
        else if (shotType == ShotType.AOE)
        {
            // AOE is just a shotgun with 10 bullets spread in a full circle
            int aoeCount = 10;
            float angleStep = (2f * Mathf.PI) / aoeCount;  // Full circle divided by bullet count
            
            for (int i = 0; i < aoeCount; i++)
            {
                var fwd = RotateVector(transform.up, angleStep * i);
                var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
                bullet.transform.up = fwd.normalized;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = fwd * BulletSpeed;
                bullet.GetComponent<BulletLogic>().Power = Power;
            }
        }
    }

    Vector2 RotateVector(Vector2 vec, float Angle)
    {
        var newX = Mathf.Cos(Angle) * vec.x - Mathf.Sin(Angle) * vec.y;
        var newY = Mathf.Sin(Angle) * vec.x + Mathf.Cos(Angle) * vec.y;
        
        return new Vector2(newX, newY);
    }

    // Add these public methods
    public void SetPhase(int newPhase)
    {
        if (newPhase >= 2 && newPhase <= 3)  // Validate phase number
        {
            CurrentPhase = newPhase;
            currentPatternIndex = 0;  // Reset pattern index when changing phases
        }
    }

    public int GetCurrentPhase()
    {
        return CurrentPhase;
    }
}
