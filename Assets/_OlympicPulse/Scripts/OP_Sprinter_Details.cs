using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _OlympicPulse.Scripts
{
    public class OP_Sprinter_Details : MonoBehaviour
    {
        [Header("Sprinter details:")] 
        public string sprinterName;
        public string sprinterDetails;
        public float worldRecordTime100M;
        public Color sprinterColor;

        // Method to apply color to the sprinter
        public void ApplyColor()
        {
            // Assuming the sprinter object has a Renderer component
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = sprinterColor;
            }
        }
    }
}