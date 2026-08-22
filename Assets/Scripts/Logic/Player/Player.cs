using UnityEngine;
using System.Collections;
using VampireSurvivors.Core;
using VampireSurvivors.Data;
using VampireSurvivors.Common;
namespace VampireSurvivors.Logic
{
    public class Player : Entity, IDamageable
    {
        private Joystick joystick;
        [SerializeField] private float moveSpeed;

        [SerializeField] private PlayerData playerData;
     

        private Animator animator;
        private Rigidbody rb;

        private Vector3 moveDirection;

        private int currentLevel = 1;
        private int currentAttack = 1;
        private int currentExp = 0;
        private int expToNextLevel = 100;

        public int CurrentLevel => currentLevel;
        public int CurrentAttack => currentAttack;
        public int CurrentExp => currentExp;
        public int ExpToNextLevel => expToNextLevel;

        private CapsuleCollider capsuleCollider;

        public void Initialize(Joystick joystick)
        {
            this.joystick = joystick;
            LocalPlayer.Register(this);
        }

        private void Awake()
        {
            base.Awake();

            capsuleCollider = GetComponentInChildren<CapsuleCollider>(true);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

            DisplayName = "Player"; 
            Debug.Log($"PlayerData = {playerData}");  

            // moveSpeed = playerData.moveSpeed;
        	// currentAttack = playerData.attack;

        	// MaxHP = playerData.maxHP;
        	currentHP = MaxHP;

            
        }

        private void Start()
        {

        }

        private void Update()
        {
            moveDirection = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

            if (moveDirection.sqrMagnitude > 1f)
            {
                moveDirection.Normalize();
            }


            animator.SetFloat("Speed", moveDirection.magnitude);

            UpdateRotation();
        }

        private void FixedUpdate()
        {

            Vector3 velocity = moveDirection * moveSpeed;


            velocity.y = rb.linearVelocity.y;

            rb.linearVelocity = velocity;
        }

        // xoay mặt
        private void UpdateRotation()
        {
            if (moveDirection.sqrMagnitude <= 0.01f)
                return;
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

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

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {

        }
    }


}