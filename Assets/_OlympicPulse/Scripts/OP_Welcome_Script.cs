using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace _OlympicPulse.Scripts
{
    /// <summary>
    /// Manages the welcome screen, including displaying personalized messages and countdown to the event.
    /// This script is attached to the WelcomeManager prefab in the PersonalisedWelcome scene.
    /// </summary>
    public class OP_Welcome_Script : MonoBehaviour
    {
        /// <summary>
        /// TextMeshProUGUI element for displaying the welcome message.
        /// </summary>
        public TextMeshProUGUI welcomeText;
        
        /// <summary>
        /// TextMeshProUGUI element for displaying the countdown to the event.
        /// </summary>
        public TextMeshProUGUI countdownText;
        
        /// <summary>
        /// Start and end position of a touch event for swipe-up detection.
        /// </summary>
        private Vector2 touchStartPos, touchEndPos;
        
        /// <summary>
        /// Flag to indicate if a swipe-up has been detected.
        /// </summary>
        private bool swipedUp = false;

        /// <summary>
        /// DateTime object to store the parsed event date and time.
        /// </summary>
        private DateTime _eventDateTime;

        /// <summary>
        /// Initializes the script, setting up the welcome screen and user information.
        /// </summary>
        void Start()
        {
            try
            {
                // Retrieve user information from PlayerPrefs
                string personName = PlayerPrefs.GetString("PersonName");
                string sport = PlayerPrefs.GetString("Sport");
                string date = PlayerPrefs.GetString("Date");
                string time = PlayerPrefs.GetString("Time");

                // Debug lines to check the retrieved date and time
                Debug.Log($"Retrieved from PlayerPrefs: Date: {date}, Time: {time}");

                // Set welcome message
                welcomeText.text = $"Welcome, {personName}!";

                // Set event text
                //eventText.text = $"{sport} Attendee";

                // Parse DateTime
                _eventDateTime = DateTime.ParseExact($"{date} {time}", "dd-MM-yyyy HH:mm", null);

                // Debug line to confirm DateTime parsing
                Debug.Log($"Parsed DateTime: {_eventDateTime}");

                // Start countdown coroutine
                StartCoroutine(CountdownToEvent());
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occurred: {e.Message}");
            }
        }

        /// <summary>
        /// Handler for the main button press that transitions to the Main scene.
        /// </summary>
        public void ToMainButtonPress()
        {
            Debug.Log("Main Button pressed");
            LoadScene("Main");
        }

        /// <summary>
        /// Coroutine that manages the countdown to the event.
        /// </summary>
        /// <returns>An IEnumerator for the coroutine.</returns>
        IEnumerator CountdownToEvent()
        {
            while (true)
            {
                // Calculate time remaining until the event
                TimeSpan timeRemaining = _eventDateTime - DateTime.Now;

                // Debug line to check countdown
                Debug.Log($"Time remaining: {timeRemaining}");

                // Display the countdown
                countdownText.text = $"{timeRemaining.Days} Days\n{timeRemaining.Hours} Hours\n{timeRemaining.Minutes} Minutes\n{timeRemaining.Seconds} Seconds";

                yield return new WaitForSeconds(1);
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
