using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts
{
    public class OP_Intro_Script : MonoBehaviour
    {
        public RawImage splashImage;
        public float splashTime = 2f;
        private Vector2 touchStartPos, touchEndPos;
        private bool swipedUp = false;

        void Start()
        {
            Debug.Log("Starting QR Scanner...");

            splashImage.color = new Color(1f, 1f, 1f, 1f); // Fully Visible

            // Check if user has already scanned a ticket
            if (PlayerPrefs.HasKey("HasScannedTicket"))
            {
                // User has scanned before. Load the Main scene.
                LoadScene("Main");
            }
            else
            {
                // Start splash screen routine
                StartCoroutine(FadeOutSplash(splashTime));
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

            if (swipedUp)
            {
                LoadScene("TicketScan");
            }
        }

        IEnumerator FadeOutSplash(float time)
        {
            Color startColor = splashImage.color;

            float rate = 1f / time;
            float progress = 0f;

            while (progress < 1f)
            {
                splashImage.color = Color.Lerp(startColor, new Color(1f, 1f, 1f, 0f), progress);
                progress += rate * Time.deltaTime;
                yield return null;
            }

            splashImage.color = new Color(1f, 1f, 1f, 0f); // Fully transparent
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