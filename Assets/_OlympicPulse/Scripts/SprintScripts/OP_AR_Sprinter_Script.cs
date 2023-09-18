using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _OlympicPulse.Scripts.SprintScripts
{
    public class OP_AR_Sprinter_Script : MonoBehaviour
    {
        private Actions actions;

        private void Awake()
        {
            // Get the Actions component from the GameObject
            actions = GetComponent<Actions>();

            // Check if the Actions component is available
            if (actions == null)
            {
                Debug.LogError("Actions component not f ound on the GameObject");
            }
        }

        public void StartRunning()
        {
            if (actions != null)
            {
                actions.Run(); // triggers the animation
            }
        }
        
        public void StopRunning()
        {
            if (actions != null)
            {
                actions.Death(); // triggers the animation
            }
        }
    }
}