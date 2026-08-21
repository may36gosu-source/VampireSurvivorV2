using System;
using UnityEngine;
using VampireSurvivors.Logic;
namespace VampireSurvivors.Core
{
    public static class GameEvents
    {

        // ------------------------
        // Player
        // ------------------------
        public static event Action<int> OnExpCollected;

        public static event Action<int> OnLevelUp;

        public static void ExpCollected(int exp)
        {
            OnExpCollected?.Invoke(exp);
        }

        public static void LevelUp(int level)
        {
            OnLevelUp?.Invoke(level);
        }

        // ------------------------
        // Monster
        // ------------------------

        public static event Action<Monster> OnMonsterDead;
        
        public static event Action<Monster> OnMonsterDamaged;

        public static event Action<Entity> OnEntityDamaged;
        public static event Action<Entity> OnEntityDead;



        public static void MonsterDead(Monster monster)
        {
            OnMonsterDead?.Invoke(monster);
        }

        public static void MonsterDamaged(Monster monster)
        {
            OnMonsterDamaged?.Invoke(monster);
        }

        // ------------------------
        // Entity
        // ------------------------

        public static void EntityDamaged(Entity entity)
        {
            OnEntityDamaged?.Invoke(entity);
        }

        public static void EntityDead(Entity entity)
        {
            OnEntityDead?.Invoke(entity);
        }

        public static event Action<Entity> OnEntitySpawn;

        public static void EntitySpawn(Entity entity)
        {
            OnEntitySpawn?.Invoke(entity);
        }

        // ------------------------
        // Damage
        // ------------------------

        public static event Action<Vector3, Vector3, int> OnPopupDamage;

        public static void PopupDamage(Vector3 position, Vector3 direction, int damage)
        {
            OnPopupDamage?.Invoke(position, direction, damage);
        }


        // ------------------------
        // GameStarted
        // ------------------------
        public static event Action OnGameStarted;

        public static void GameStarted()
        {
            OnGameStarted?.Invoke();
        }


        // ------------------------
        // GameOver
        // ------------------------
        public static event Action OnGameOver;

        public static void GameOver()
        {
            OnGameOver?.Invoke();
        }

        // ------------------------
        // Xoay player mượt khi cast skill
        // ------------------------
        public static event Action<Transform> OnSkillFaceTarget;

        public static void SkillFaceTarget(Transform target)
        {
            OnSkillFaceTarget?.Invoke(target);
        }

        
        // ------------------------
        // Skill Message
        // ------------------------

        public static event Action<string> OnSkillMessage;

        public static void SkillMessage(string message)
        {
            OnSkillMessage?.Invoke(message);
        }

        
    }
}