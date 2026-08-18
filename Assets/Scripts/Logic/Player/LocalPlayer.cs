using UnityEngine;
using System.Collections;

namespace VampireSurvivors.Logic
{
    public static class LocalPlayer
    {
        public static Player Instance { get; private set; }

        public static void Register(Player player)
        {
            Instance = player;
        }

        public static void Unregister(Player player)
        {
            if (Instance == player)
            {
                Instance = null;
            }
        }

        public static Transform Transform => Instance != null ? Instance.transform : null;
    }
}