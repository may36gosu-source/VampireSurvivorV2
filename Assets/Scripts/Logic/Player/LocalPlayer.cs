using UnityEngine;
using System.Collections;
using System;

namespace VampireSurvivors.Logic
{
    public static class LocalPlayer
    {
        public static Player Instance { get; private set; }

        public static event Action<Player> OnRegistered;

        public static event Action<Player> OnUnregistered;

        public static void Register(Player player)
        {
            Instance = player;

            OnRegistered?.Invoke(player);
        }

        public static void Unregister(Player player)
        {
            if (Instance != player)
                return;

            Instance = null;

            OnUnregistered?.Invoke(player);
        }

        public static Transform Transform => Instance != null ? Instance.transform : null;
    }
}