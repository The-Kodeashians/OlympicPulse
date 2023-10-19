using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts
{
    /// <summary>
    /// Manages the introductory splash screen user input for scene transition.
    /// This script is attached to the IntroScript prefab in the Intro scene.
    /// </summary>
    public class OP_Intro_Script : MonoBehaviour
    {
        /// <summary>
        /// Start position of a touch event for swipe-up detection.
        /// </summary>
        private Vector2 touchStartPos;

        /// <summary>
        /// End position of a touch event for swipe-up detection.
        /// </summary>
        private Vector2 touchEndPos;

        /// <summary>
        /// Flag to indicate if a swipe-up has been detected.
        /// </summary>
        private bool swipedUp = false;

        /// <summary>
        /// Initializes the script, setting up user input.
        /// </summary>
        void Start()
        {
            Debug.Log("Starting QR Scanner...");
            
            // Check if user has already scanned a ticket
            // Commented out for demo
            /**
            if (PlayerPrefs.HasKey("HasScannedTicket"))
            {
                // User has scanned before. Load the Main scene.
                LoadScene("Main");
            }*/
        }

        /// <summary>
        /// Updates the script each frame to handle user input for scene transition.
        /// </summary>
        void Update()
        {
            // Swipe-up detection
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        break;
                    case TouchPhase.Ended:
                        touchEndPos = touch.position;
                        if (touchEndPos.y - touchStartPos.y > 100) // Swipe sensitivity
                        {
                            swipedUp = true;
                        }

                        break;
                }
            }
            
            // Keyboard input for desktop or Mac
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                swipedUp = true;
            }

            if (swipedUp)
            {
                LoadScene("TicketScan");
            }
        }

        /// <summary>
        /// Loads a specified Unity scene.
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
}
