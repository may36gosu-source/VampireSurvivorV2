using UnityEngine;
using System.Collections;

namespace VampireSurvivors.Core
{
    public abstract class Entity : MonoBehaviour
    {
        private static ulong nextId = 1;

        [Header("Entity")]
        [SerializeField]
        protected Transform headPoint;

        protected int currentHP;
        protected bool isDead;

        public ulong Id { get; private set; }

        public Transform HeadPoint => headPoint;

        public Vector3 Position => transform.position;

        public Vector3 Forward => transform.forward;

        public int CurrentHP => currentHP;

        public int MaxHP { get; protected set; }

        public string DisplayName { get;protected set;}

        public bool IsDead => isDead;

        protected virtual void Awake()
        {
            Id = nextId++;

            if (headPoint == null)
            {
                headPoint = transform.Find("HeadPoint");
            }

            if (headPoint == null)
            {
                Debug.LogError($"{name} missing HeadPoint.");
            }
        }
    }
}