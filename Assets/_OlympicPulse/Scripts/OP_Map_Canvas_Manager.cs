using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Responsible for setting World Space Event Camera
 * and facing the canvas towards the camera at all times
 */
public class OP_Map_Canvas_Manager : MonoBehaviour
{
    private Camera mainCamera;
    private Canvas canvas;
    
    [Header("Game Buttons")]
    public Button sprintingGameButton; // Assign this in the inspector

    void Start()
    {
        mainCamera = Camera.main;

        // Get the Canvas component
        canvas = GetComponentInParent<Canvas>();

        // If a canvas is found, set its worldCamera to mainCamera
        if (canvas != null)
        {
            canvas.worldCamera = mainCamera;
        }
        else
        {
            Debug.LogWarning("No Canvas component found in parent objects.");
        }

        // Assign the button click event
        if (sprintingGameButton != null)
        {
            sprintingGameButton.onClick.AddListener(OnSprintingGameButtonClick);
        }
        else
        {
            Debug.LogWarning("Sprinting Game Button is not assigned.");
        }
    }
    
    void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    // This function will be called when the button is clicked
    public void OnSprintingGameButtonClick()
    {
        // Load the "Sprinting" scene
        LoadScene("SprintingSelect");
    }
    
    void LoadScene(string sceneName)
    {
        try
        {
            Debug.Log($"Attempting to load {sceneName} scene.");
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred while loading the scene: " + e.Message);
        }
    }
}