using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace _OlympicPulse.Scripts
{
    public class OP_Welcome_Script : MonoBehaviour
    {
        public TextMeshProUGUI welcomeText;
        //public TextMeshProUGUI eventText;
        public TextMeshProUGUI countdownText;
        public float fadeTime = 2f;
        private Vector2 touchStartPos, touchEndPos;
        private bool swipedUp = false;

        private DateTime _eventDateTime;

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

        public void ToMainButtonPress()
        {
            Debug.Log("Main Button pressed");
            LoadScene("Main");
        }

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