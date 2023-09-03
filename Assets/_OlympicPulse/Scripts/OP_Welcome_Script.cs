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
        public TextMeshProUGUI countdownText;
        public float fadeTime = 2f;

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
                welcomeText.text = $"Welcome, {personName}!\nYou're going to watch {sport}.";

                // Parse DateTime
                _eventDateTime = DateTime.ParseExact($"{date} {time}", "dd-MM-yyyy HH:mm", null);

                // Debug line to confirm DateTime parsing
                Debug.Log($"Parsed DateTime: {_eventDateTime}");

                // Start countdown coroutine
                StartCoroutine(CountdownToEvent());

                // Start fade to Main scene after 4 seconds
                StartCoroutine(FadeToMainSceneAfterDelay(4));
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occurred: {e.Message}");
            }
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
                countdownText.text = $"{timeRemaining.Days} days {timeRemaining.Hours} hours {timeRemaining.Minutes} minutes left";

                yield return new WaitForSeconds(1);
            }
        }
        
        
        IEnumerator FadeToMainSceneAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Time is up, starting fade.");
            StartCoroutine(FadeToMainScene());
        }

        IEnumerator FadeToMainScene()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component missing.");
                yield break;
            }

            float rate = 1f / fadeTime;
            float progress = 0f;

            while (progress < 1f)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
                progress += rate * Time.deltaTime;
                yield return null;
            }

            // Load the Main scene
            Debug.Log("Loading Main scene.");
            SceneManager.LoadScene("Main");
        }
    }
}