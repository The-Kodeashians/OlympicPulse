using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.SceneManagement;

namespace _OlympicPulse.Scripts
{
    /// <summary>
    /// Manages the QR code scanning logic and transitions to other scenes.
    /// This script is attached to the TicketScanner prefab in the TicketScan scene.
    /// </summary>
    public class OP_QR_SCAN : MonoBehaviour
    {
        /// <summary>
        /// Webcam texture for capturing the QR code.
        /// </summary>
        private WebCamTexture _camTexture;
        
        /// <summary>
        /// Rectangle representing the screen dimensions.
        /// </summary>
        private Rect _screenRect;

        /// <summary>
        // Scanned in information from the QR code.
        // </summary>
        public string personName;
        public string sport;
        public string date; // Must be "DD-MM-YYYY"
        public string time; // Must be 24hr time, e.g. "18:00"

        /// <summary>
        /// Initializes the script.
        /// Sets screen orientation and initializes the webcam if permission is granted.
        /// </summary>
        void Start()
        {
            // Lock the screen in portrait orientation
            Screen.orientation = ScreenOrientation.Portrait;

            _screenRect = new Rect(0, 0, Screen.width, Screen.height);

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
        }

        /// <summary>
        /// Initializes the webcam for QR scanning.
        /// Called in the Start() method.
        /// </summary>
        void InitializeWebCam()
        {
            // Initialise webcam
            _camTexture = new WebCamTexture();
            _camTexture.requestedHeight = 480;
            _camTexture.requestedWidth = 640;

            if (_camTexture != null)
            {
                _camTexture.Play();
            }
        }

        /// <summary>
        /// Coroutine to request camera permission from the user.
        /// Initializes the webcam if permission is granted.
        /// </summary>
        /// <returns>An IEnumerator for the coroutine.</returns>
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

        /// <summary>
        /// Handles the GUI for QR scanning.
        /// Draws the webcam texture, scanner frame, and captures the QR code.
        /// </summary>
        void OnGUI()
        {
            if (_camTexture == null)
            {
                Debug.LogError("Camera texture is null.");
                return;
            }

            if (!_camTexture.isPlaying)
            {
                Debug.LogError("Camera texture is not playing.");
                return;
            }

            // Calculate aspect ratio scaling
            float videoRatio = (float)_camTexture.width / (float)_camTexture.height;

            // Calculate the position to start drawing the texture so that it's centered
            float startX = (Screen.width - (_screenRect.height * videoRatio)) / 2;
            Rect scaledRect = new Rect(startX, 0, _screenRect.height * videoRatio, _screenRect.height);

            int rotate = 270;

            // Flip the x-axis to mirror the texture
            GUIUtility.ScaleAroundPivot(new Vector2(-1, 1),
                new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Rotate the texture
            GUIUtility.RotateAroundPivot(rotate, new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Draw the camera background for the scanner
            GUI.DrawTexture(scaledRect, _camTexture, ScaleMode.StretchToFill);

            // Revert the rotation to not affect other GUI elements
            GUIUtility.RotateAroundPivot(-rotate, new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Revert the x-axis scale to not affect other GUI elements
            GUIUtility.ScaleAroundPivot(new Vector2(-1, 1),
                new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));
            
            // Style for the text
            GUIStyle style = new GUIStyle();
            style.fontSize = 50;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            // Position and display the text
            float textWidth = 600;
            GUI.Label(new Rect((Screen.width - textWidth) / 2, Screen.height - 200, textWidth, 60), "Please scan your ticket's QR code", style);

            // Draw corner arrows
            int lineLength = 50; // Length of the lines
            int lineWidth = 5;  // Width of the lines
            int offset = Screen.width / 3; // Offset from the edge for x coordinate
            int offset_y = Screen.height / 3; // Offset from the edge for y coordinate

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            // Top-left corner
            GUI.DrawTexture(new Rect(offset, offset_y, lineLength, lineWidth), texture);
            GUI.DrawTexture(new Rect(offset, offset_y, lineWidth, lineLength), texture);

            // Top-right corner
            GUI.DrawTexture(new Rect(Screen.width - offset - lineLength, offset_y, lineLength, lineWidth), texture);
            GUI.DrawTexture(new Rect(Screen.width - offset - lineWidth, offset_y, lineWidth, lineLength), texture);

            // Bottom-left corner
            GUI.DrawTexture(new Rect(offset, Screen.height - offset_y - lineWidth, lineLength, lineWidth), texture);
            GUI.DrawTexture(new Rect(offset, Screen.height - offset_y - lineLength, lineWidth, lineLength), texture);

            // Bottom-right corner
            GUI.DrawTexture(new Rect(Screen.width - offset - lineLength, Screen.height - offset_y - lineWidth, lineLength, lineWidth), texture);
            GUI.DrawTexture(new Rect(Screen.width - offset - lineWidth, Screen.height - offset_y - lineLength, lineWidth, lineLength), texture);
            
            GUIStyle debugButtonStyle = new GUIStyle("button");
            debugButtonStyle.fontSize = 30;
    
            // Place button at top right corner
            if (GUI.Button(new Rect(Screen.width - 250, 20, 200, 80), "DEBUG SKIP", debugButtonStyle))
            {
                DebugSkip();
            }
            
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // Decode the current frame
                var result = barcodeReader.Decode(_camTexture.GetPixels32(), _camTexture.width, _camTexture.height);
                if (result != null)
                {
                    Debug.Log("DECODED TEXT FROM QR: " + result.Text);

                    // Parse the data from the QR code
                    string[] pairs = result.Text.Split(',');
                    foreach (string pair in pairs)
                    {
                        int index = pair.IndexOf(':');
                        if (index > 0)
                        {
                            string key = pair.Substring(0, index);
                            string value = pair.Substring(index + 1);

                            Debug.Log($"Key: '{key}', Value: '{value}'");

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

                    Debug.Log("Attempting to load PersnalisedWelcome scene.");

                    // Load the next scene with a delay
                    StartCoroutine(LoadSceneAfterDelay("PersonalisedWelcome", 1));
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
        
        /// <summary>
        /// Debug function to skip QR scanning and proceed to the next scene.
        /// Sets hardcoded values for event details.
        /// </summary>
        void DebugSkip()
        {
            // Hardcoded values for debug skip
            personName = "Melissa";
            sport = "Sprinting";
            date = "20-12-2023";
            time = "18:30";

            // Store the hardcoded information in PlayerPrefs
            PlayerPrefs.SetString("PersonName", personName);
            PlayerPrefs.SetString("Sport", sport);
            PlayerPrefs.SetString("Date", date);
            PlayerPrefs.SetString("Time", time);
            PlayerPrefs.SetInt("HasScannedTicket", 1);  // Set flag to indicate a ticket has been "scanned"

            // Log the hardcoded information
            Debug.Log($"Debug Skip - Name: {personName}, Sport: {sport}, Date: {date}, Time: {time}");

            // Load the PersonalisedWelcome scene
            Debug.Log("Debug Skip - Attempting to load PersonalisedWelcome scene.");
            StartCoroutine(LoadSceneAfterDelay("PersonalisedWelcome", 1));
        }

        /// <summary>
        /// Coroutine to load a Unity scene after a delay.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="delay">The time to wait before loading the scene, in seconds.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
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
