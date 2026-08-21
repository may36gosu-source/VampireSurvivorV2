using System;
using System.Collections;
using UnityEngine;
using VampireSurvivors.Data;
using VampireSurvivors.Core;
using VampireSurvivors.Manager;
using VampireSurvivors.Manager;
using VampireSurvivors.Core;
using VampireSurvivors.Helper;
namespace VampireSurvivors.Logic {

    public class Monster : Entity, IPoolable, IDamageable
    {
        private Animator animator;
        private Transform player;

        private MonsterData monsterData;

        public event Action<Monster> OnDead;

        private CapsuleCollider capsuleCollider;

        #region Ground
        [Header("Ground")]
        [SerializeField]
        private float groundRayHeight = 2f;

        [SerializeField]
        private float groundRayDistance = 5f;

        private Collider groundCollider;
        #endregion

        #region Movement Animation
        [Header("Movement Animation")]
        [SerializeField]
        private float runSpeedThreshold = 3f;
        #endregion
        
        #region Death
        private Vector3 deathPosition;

        public Vector3 DeathPosition => deathPosition;
        #endregion

        private Collider monsterCollider;
        private Collider playerCollider;

        private bool isAttacking;

        private float attackTimer;

        private Coroutine attackRoutine;

        private Vector3 spawnPosition;

        private AIState currentState; // state hiện tại

        private const float ReachThreshold = 0.1f;

        private enum AIState
        {
            Wander,
            Chase,
            Return
        }

        private Vector3 wanderTarget; // điểm tới


        public void Initialize(MonsterData data, Vector3 spawnPosition)
        {
            monsterData = data;

            MaxHP = data.maxHP;
            currentHP = MaxHP;

            DisplayName = data.displayName;

            // Quan trọng:
            // Initialize được gọi SAU khi Spawner đã set position.
            // Vì vậy SnapToGround ở đây mới đúng.

            IgnorePlayerCollision(); // bỏ qua va chạm với player

            this.spawnPosition = spawnPosition;

            PickWanderTarget();

            // SnapToGround();
            currentState = AIState.Wander;
        }

        private void Awake()
        {
            base.Awake();

            PrepareComponents();
        }

        private void OnEnable()
        {
     
            LocalPlayer.OnRegistered += HandlePlayerRegistered;
            ResolvePlayer(); 
        }

        private void OnDisable()
        {
        
            LocalPlayer.OnRegistered -= HandlePlayerRegistered;
        }

        private void PickWanderTarget() {

            Vector2 random = UnityEngine.Random.insideUnitCircle *  monsterData.wanderRadius;

            Vector3 offset = new Vector3(random.x, 0f, random.y);

            wanderTarget = spawnPosition + offset;

        }

        private void UpdateWander() {

            if (CheckPlayerDetection())
            {
                currentState = AIState.Chase;
                return;
            }

            Vector3 direction = wanderTarget - transform.position; // lấy hướng

            direction.y = 0f;

            if(direction.sqrMagnitude <= ReachThreshold * ReachThreshold) {
                // đã tới
                PickWanderTarget();
                animator.SetFloat(GameConst.ANIM_SPEED, 0f);
                return;
            }
            direction.Normalize();

            transform.position += direction * monsterData.moveSpeed *Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetFloat(GameConst.ANIM_SPEED, 1f);

        }

        private bool CheckPlayerDetection() {
            if (player == null)
            {
                Debug.Log("[Monster] Player NULL");
                return false;
            }

            Vector3 offset = player.position - transform.position;
            offset.y = 0f;

            float distanceSqr = offset.sqrMagnitude;

            if(distanceSqr <= monsterData.detectRange*monsterData.detectRange) {
                
                // Debug.Log("[Monster] Wander -> Chase");
                return true;
                
            }
            return false;
        }

        private void UpdateChase() {
            if (player == null)
            {
               
                currentState = AIState.Return;
                return;
            }
            
            if (CheckHomeDistance())
            {
                currentState = AIState.Return;

                animator.SetFloat(GameConst.ANIM_SPEED, 0f);
                return;
            }


            // đuổi theo player
            Vector3 direction = player.position - transform.position; // lấy hướng

            direction.y = 0f;

            if(direction.sqrMagnitude <= ReachThreshold * ReachThreshold) {
                // đã tới
               
                animator.SetFloat(GameConst.ANIM_SPEED, 0f);
                // Debug.Log("[Monster] Chase -> Finish"); // đuổi tới giả sử player đứng đó thì monster có thể đánh
                return;
            }
          
            direction.Normalize();
            

            transform.position += direction * monsterData.moveSpeed *Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetFloat(GameConst.ANIM_SPEED, 1f);
            

           
        }

        private bool CheckHomeDistance() {

            Vector3 offset = transform.position - spawnPosition;

            offset.y = 0f;

            float distanceSqr = offset.sqrMagnitude;

            if(distanceSqr >= monsterData.returnRange*monsterData.returnRange) {
                
                Debug.Log("[Monster] Chase -> Home");
                return true;
                
            }
            return false;
            
        }

        private void UpdateReturn() {

    

            Vector3 direction = spawnPosition - transform.position; // lấy hướng

            direction.y = 0f;

            if(direction.sqrMagnitude <= ReachThreshold * ReachThreshold) {
                // đã tới
               
                animator.SetFloat(GameConst.ANIM_SPEED, 0f);
                currentState = AIState.Wander; // set lại state
                PickWanderTarget();
                return;
            }
           
            direction.Normalize();
            

            transform.position += direction * monsterData.moveSpeed *Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(direction);

            animator.SetFloat(GameConst.ANIM_SPEED, 1f);

            
        }

        private void Update()
        {
            switch (currentState)
            {
                case AIState.Wander:
                    UpdateWander();
                    break;

                case AIState.Chase:
                    UpdateChase();
                    break;

                case AIState.Return:
                    UpdateReturn();
                    break;
            }
        }


        //==================================================
        // Components
        //==================================================

        private void PrepareComponents()
        {
            animator = GetComponentInChildren<Animator>(true);
            monsterCollider = GetComponentInChildren<CapsuleCollider>(true);
        }

        private void HandlePlayerRegistered(Player player) {

            this.player = player.transform;
            IgnorePlayerCollision();
        }

        private void ResolvePlayer()
        {
            if (LocalPlayer.Instance != null)
            {
                player = LocalPlayer.Transform;
            }
        }


        private void IgnorePlayerCollision()
        {
            monsterCollider = GetComponentInChildren<CapsuleCollider>(true);

            if (monsterCollider == null)
                return;


            if (player == null)
                return;

            Collider playerCollider = player.GetComponent<Collider>();

            if (playerCollider == null)
                return;

            Physics.IgnoreCollision( monsterCollider, playerCollider, true );
        }

        //==================================================
        // Pool
        //==================================================


        public void OnSpawn()
        {
            isDead = false;

            enabled = true;

            isAttacking = false;
            attackTimer = 0f;
            attackRoutine = null;

            if (monsterCollider != null)
                monsterCollider.enabled = true;

            animator.Rebind();
            animator.Update(0f);

            animator.SetFloat(GameConst.ANIM_SPEED, 0f);

            if (monsterData != null)
            {
                currentHP = monsterData.maxHP;
            }
        }

       

        public void OnDespawn()
        {
            enabled = false;

            if (monsterCollider != null)
                monsterCollider.enabled = false;

            isAttacking = false;
            attackTimer = 0f;
            attackRoutine = null;
        }

        //==================================================
        // Damage
        //==================================================

        public void TakeDamage(int damage)
        {
            if (isDead)
                return;

            Vector3 hitPosition = HeadPoint.position;

            Vector3 hitDirection = Forward;

            currentHP = Mathf.Max(0, currentHP - damage);

            GameEvents.EntityDamaged(this);

            GameEvents.PopupDamage(hitPosition, hitDirection, damage);


            if (currentHP == 0)
            {
                Dead();
            }
        }

        private void Dead() {

        }


    
    }
}