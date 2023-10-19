using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the world-space canvas for the interactive map.
/// This script is attached to the Stadium final prefab inside the canvas and is intended for the
/// Main scene after the user places the map.
/// </summary>
public class OP_Map_Canvas_Manager : MonoBehaviour
{
    /// <summary>
    /// Reference to the main camera in the scene.
    /// </summary>
    private Camera mainCamera;
    
    /// <summary>
    /// Reference to the canvas component.
    /// </summary>
    private Canvas canvas;
    
    /// <summary>
    /// Buttons for in-game actions, like starting the sprinting game.
    /// </summary>
    [Header("Game Buttons")]
    public Button sprintingGameButton; // Assign this in the inspector

    /// <summary>
    /// Initialization logic to find and assign the main camera and canvas.
    /// </summary>
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
    
    /// <summary>
    /// Updates the canvas orientation to always face the camera.
    /// </summary>
    void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    /// <summary>
    /// Handles the click event for the sprinting game button.
    /// </summary>
    public void OnSprintingGameButtonClick()
    {
        // Load the "Sprinting" scene
        LoadScene("SprintingSelect");
    }
    
    /// <summary>
    /// Loads the specified Unity scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
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
