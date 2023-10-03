using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts
{
    public class OP_QR_SCAN : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button scanButton; // Reference to the button in the UI
        public RawImage overlay; // RawImage to show when a QR code is detected

        private WebCamTexture _camTexture;

        // Ticket info
        public string personName;
        public string sport;
        public string date;
        public string time;

        private bool isScanning = false; // Flag to indicate if we should scan

        void Start()
        {
            // Lock the screen in portrait orientation
            Screen.orientation = ScreenOrientation.Portrait;

            Debug.Log("Starting QR Scanner...");

            // Check for camera permission
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                InitializeWebCam();
            }
            else
            {
                StartCoroutine(RequestCameraPermission());
            }

            // Set up button listener
            scanButton.onClick.AddListener(StartScanning);
        }

        void InitializeWebCam()
        {
            // Initialize webcam
            _camTexture = new WebCamTexture();
            _camTexture.requestedHeight = 1920;
            _camTexture.requestedWidth = 1080;

            if (_camTexture != null)
            {
                _camTexture.Play();
            }
        }

        IEnumerator RequestCameraPermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                InitializeWebCam();
            }
            else
            {
                Debug.LogWarning("User did not grant camera permission.");
            }
        }

        public void StartScanning()
        {
            isScanning = true;
        }

        void Update()
        {
            if (isScanning)
            {
                try
                {
                    IBarcodeReader barcodeReader = new BarcodeReader();
                    var result = barcodeReader.Decode(_camTexture.GetPixels32(), _camTexture.width, _camTexture.height);
                    if (result != null)
                    {
                        overlay.gameObject.SetActive(true);  // Show overlay

                        // Parsing code
                        string[] pairs = result.Text.Split(',');
                        foreach (string pair in pairs)
                        {
                            int index = pair.IndexOf(':');
                            if (index > 0)
                            {
                                string key = pair.Substring(0, index);
                                string value = pair.Substring(index + 1);

                                if (key.Equals("Name"))
                                {
                                    personName = value;
                                }
                                else if (key.Equals("Sport"))
                                {
                                    sport = value;
                                }
                                else if (key.Equals("Date"))
                                {
                                    date = value;
                                }
                                else if (key.Equals("Time"))
                                {
                                    time = value;
                                }
                            }
                        }

                        // Log the parsed information
                        Debug.Log($"Name: {personName}, Sport: {sport}, Date: {date}, Time: {time}");

                        // Stop the camera
                        if (_camTexture != null)
                        {
                            _camTexture.Stop();
                        }

                        // Store the information in PlayerPrefs
                        PlayerPrefs.SetString("PersonName", personName);
                        PlayerPrefs.SetString("Sport", sport);
                        PlayerPrefs.SetString("Date", date);
                        PlayerPrefs.SetString("Time", time);
                        PlayerPrefs.SetInt("HasScannedTicket", 1); // Set flag to indicate a ticket has been scanned
                        PlayerPrefs.Save();

                        // Load the next scene
                        StartCoroutine(LoadSceneAfterDelay("PersonalisedWelcome", 1));

                        isScanning = false;  // Stop scanning
                        overlay.gameObject.SetActive(false);  // Hide overlay
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
        }

        IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred while loading the scene: " + e.Message);
            }
        }
    }
}
