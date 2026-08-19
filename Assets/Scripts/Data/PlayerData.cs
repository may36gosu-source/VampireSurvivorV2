using UnityEngine;
namespace VampireSurvivors.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Data/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public int maxHP = 100;
        public float moveSpeed = 5f;
        public int attack = 20;
        public float attackRange = 3f;
        public float attackCooldown = 1f;
    }
}