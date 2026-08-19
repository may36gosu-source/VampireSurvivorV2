using UnityEngine;
namespace VampireSurvivors.Data
{
    [CreateAssetMenu(fileName = "ExpData", menuName = "Game/Data/Exp Data")]
    public class ExpData : ScriptableObject
    {
        public GameObject prefab;

        [Header("Pool")]
        public int preloadCount = 30;

        [Header("Collect")]
        public float collectRadius = 2f;

        public float moveSpeed = 12f;
    }


}