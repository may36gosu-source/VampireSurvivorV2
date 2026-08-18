using UnityEngine;
using System.Collections;
using VampireSurvivors.Core;
namespace VampireSurvivors.Logic
{
    public class Player : Entity
    {
        [SerializeField] private FixedJoystick joystick;
        [SerializeField] private float moveSpeed;

        #region ScriptableObject

        #endregion

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



        private void Awake()
        {
            base.Awake();

            capsuleCollider = GetComponentInChildren<CapsuleCollider>(true);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

            DisplayName = "Player";

            LocalPlayer.Register(this);

            
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {

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