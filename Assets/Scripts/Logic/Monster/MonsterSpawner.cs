
using System.Collections;
using UnityEngine;
using VampireSurvivors.Core;
using VampireSurvivors.Data;
using VampireSurvivors.Manager;
using VampireSurvivors.Core;
using VampireSurvivors.Common;
namespace VampireSurvivors.Logic {

    public class MonsterSpawner : RuntimeBinder
    {
        [Header("Wave")]
        [SerializeField]
        private WaveMonsterData waveData;

        [Header("Spawn")]
        [SerializeField]
        private Transform spawnCenter;

        [SerializeField]
        private float spawnRadius = 5f;

        // [Header("Drop")]
        // [SerializeField]
        // private ExpData expData;

        private int currentAlive;

        private Coroutine respawnRoutine;

        private ObjectPool monsterPool;
        private ObjectPool expPool;

        private AssetBundleLoader bundleLoader;


        public override void Initialize(RuntimeContext context)
        {
            AssetBundleLoader bundle = context.Find<AssetBundleLoader>();

            if(bundle == null)
            {
                Debug.LogError("AssetBundleLoader not found");
                return;
            }
            bundleLoader = bundle;
        }

        private void Awake()
        {
            RuntimeContext ct = new RuntimeContext();
            Initialize(ct);
        }

        private void Start()
        {


            // if(!bundleLoader) 
            //     return; // bundle không tồn tại
            
            // // get prefab
            // GameObject prefab = bundleLoader.LoadPrefab(waveData.monsterData.bundleName, waveData.monsterData.prefabName);

            // if(prefab == null)
            //     return;

            // monsterPool = new ObjectPool(prefab, waveData.spawnCount, transform);

        
            // SpawnWave();


        }

        private void SpawnWave()
        {
            for (int i = 0; i < waveData.spawnCount; i++)
            {
                SpawnMonster();
            }
        }

        //==================================================
        // Monster
        //==================================================

        private void SpawnMonster()
        {
            GameObject monster = monsterPool.Get();

            Vector3 spawnPosition = GetSpawnPosition();

            // monster.transform.position = spawnPosition;

            // monster.transform.rotation = Quaternion.identity;

            monster.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

            Monster controller = monster.GetComponent<Monster>();

            if (controller == null)
            {
                controller = monster.AddComponent<Monster>();
            }

            // Initialize SAU KHI set position.
            controller.Initialize( waveData.monsterData, spawnPosition);

            GameEvents.EntitySpawn( controller);

            controller.OnDead += HandleMonsterDead;

            currentAlive++;
        }

        //==================================================
        // Spawn Position
        //==================================================

        private Vector3 GetSpawnPosition()
        {
            Vector2 random = Random.insideUnitCircle * spawnRadius;

            return spawnCenter.position + new Vector3(random.x, 0f, random.y);
        }


       
        //==================================================
        // Monster Death
        //==================================================

        private void HandleMonsterDead(Monster monster)
        {
            monster.OnDead -= HandleMonsterDead;

            currentAlive--;

            //==================================================
            // QUAN TRỌNG
            //
            // Không dùng:
            // monster.transform.position
            //
            // Vì Death Animation có thể thay đổi
            // vị trí visual/root.
            //==================================================

            // SpawnExp( monster.DeathPosition);

            monsterPool.Release( monster.gameObject );
        }


        private void OnEnable()
        {
        
        }

        private void OnDisable()
        {
            
        }

       
      



    //     private Vector3 GetExpGroundPosition(Vector3 position, SphereCollider sphereCollider)
    //     {
    //         if (GroundSystem.Instance == null)
    //             return position;

    //         if (GroundSystem.Instance.RaycastGround(position, out RaycastHit hit))
    //         {
    //             position.y = hit.point.y;

    //             if (sphereCollider != null)
    //                 position.y += sphereCollider.bounds.extents.y;
    //         }

    //         return position;
    //     }
    }
}