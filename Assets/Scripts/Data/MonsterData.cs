using UnityEngine;
namespace VampireSurvivors.Data
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Game/Data/Monster Data")]
    public class MonsterData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Slime";
        public GameObject prefab;

        [Header("Stats")]
        public int maxHP = 50;
        public int expReward = 10;

        [Header("Movement")]
        public float moveSpeed = 2.5f;

        [Header("Combat")]
        public int attack = 10;

        public float detectRange = 8f;

        public float attackRange = 2f;

        public float attackCooldown = 1.5f;

        [Range(0f, 1f)]
        public float attackHitTime = 0.5f;
    }
}