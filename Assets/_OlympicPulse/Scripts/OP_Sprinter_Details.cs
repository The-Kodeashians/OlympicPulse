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
            // Find the child GameObject with the SkinnedMeshRenderer
            Transform childTransform = transform.Find("SportyGirl");
            if (childTransform != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = childTransform.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    Material material = skinnedMeshRenderer.material;
                    if (material != null && material.HasProperty("_Color"))
                    {
                        // Set the color in the shader
                        material.SetColor("_Color", sprinterColor);
                        Debug.Log("Color applied: " + sprinterColor);
                    }
                    else
                    {
                        Debug.LogWarning("Material does not have a '_Color' property.");
                    }
                }
                else
                {
                    Debug.LogWarning("No SkinnedMeshRenderer found on the child GameObject.");
                }
            }
            else
            {
                Debug.LogWarning("No child GameObject named 'SportyGirl' found.");
            }
        }
    }
}