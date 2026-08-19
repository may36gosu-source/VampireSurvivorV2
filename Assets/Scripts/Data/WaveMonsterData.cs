using UnityEngine;
namespace VampireSurvivors.Data {
    [CreateAssetMenu(fileName = "WaveMonsterData", menuName = "Game/Data/Wave Monster Data")]
    public class WaveMonsterData : ScriptableObject
    {
        public MonsterData monsterData;
        public int spawnCount = 10;
        public float spawnInterval = 2f;
    }
}