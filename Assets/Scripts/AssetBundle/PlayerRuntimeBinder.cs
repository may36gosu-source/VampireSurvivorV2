using System;
using UnityEngine;
using VampireSurvivors.Common;

namespace VampireSurvivors.Logic
{
    public class PlayerRuntimeBinder : RuntimeBinder {
        [SerializeField]
        private Player player;

        public override void  Initialize(RuntimeContext context)
        {
            Joystick result = context.Find<Joystick>();

            if(result == null)
            {
                Debug.LogError("Joystick not found");
                return;
            }
            player.Initialize(result);
        }
    } 
}

