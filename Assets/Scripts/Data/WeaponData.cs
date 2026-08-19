using UnityEngine;
namespace VampireSurvivors.Data {
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public int damage = 20;
        public float cooldown = 1f;
        public float range = 3f;
        public float projectileSpeed = 10f;
    }
}