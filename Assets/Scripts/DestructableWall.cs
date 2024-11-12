// using UnityEngine;

// public class DestructableWall : MonoBehaviour
// {
//     private int hitCount = 0;
//     private const int HITS_TO_DESTROY = 5;
//     private BossFSM boss;

//     private void Start()
//     {
//         boss = FindFirstObjectByType<BossFSM>();
//     }

//     public void TakeHit()
//     {
//         hitCount++;
//         Debug.Log($"Wall {gameObject.name} hit count: {hitCount}");

//         if (hitCount >= HITS_TO_DESTROY)
//         {
//             boss.HandleWallReady(gameObject);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D col)
//     {
//         Debug.Log($"Wall hit by: {col.gameObject.name}");
        
//         BulletLogic bullet = col.GetComponent<BulletLogic>();
//         if (bullet != null)
//         {
//             Debug.Log($"Bullet team: {bullet.Team}");
            
//             if (bullet.Team == Teams.Enemy)
//             {
//                 TakeHit();
//                 Destroy(bullet.gameObject);
//             }
//         }
//     }
// } 