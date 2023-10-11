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
        public TextMeshProUGUI eventText;
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
                eventText.text = $"{sport} Attendee";

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
                LoadScene("Main");
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
                countdownText.text = $"{timeRemaining.Days} Days\n{timeRemaining.Hours} Hours\n{timeRemaining.Minutes} Minutes";

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
            LoadScene("Main");
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