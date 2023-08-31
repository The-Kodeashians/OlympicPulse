using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using UnityEngine.SceneManagement;

namespace _OlympicPulse.Scripts
{
    public class OP_QR_SCAN : MonoBehaviour
    {
        private WebCamTexture _camTexture;
        private Rect _screenRect;

        // Ticket info
        public string personName;
        public string sport;
        public string date;
        public string time;

        void Start()
        {
            // Lock the screen in portrait orientation
            Screen.orientation = ScreenOrientation.Portrait;
            
            _screenRect = new Rect(0, 0, Screen.width, Screen.height);

            Debug.Log("Starting QR Scanner...");

            // Check if user has already scanned a ticket
            if (PlayerPrefs.HasKey("HasScannedTicket"))
            {
                // User has scanned before. Load the Main scene.
                try 
                {
                    Debug.Log("Attempting to load Main scene due to existing PlayerPrefs.");
                    SceneManager.LoadScene("Main");
                } 
                catch (Exception e) 
                {
                    Debug.LogError("An error occurred while loading the scene: " + e.Message);
                }
                
                return;
            }

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

        void InitializeWebCam()
        {
            // Initialise webcam
            _camTexture = new WebCamTexture();
            _camTexture.requestedHeight = 480; // lower value
            _camTexture.requestedWidth = 640;  // lower value

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
            GUIUtility.ScaleAroundPivot(new Vector2(-1, 1), new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Rotate the texture
            GUIUtility.RotateAroundPivot(rotate, new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Draw the camera background for the scanner
            GUI.DrawTexture(scaledRect, _camTexture, ScaleMode.ScaleAndCrop);

            // Revert the rotation to not affect other GUI elements
            GUIUtility.RotateAroundPivot(-rotate, new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

            // Revert the x-axis scale to not affect other GUI elements
            GUIUtility.ScaleAroundPivot(new Vector2(-1, 1), new Vector2(_screenRect.width * 0.5f, _screenRect.height * 0.5f));

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
                        string[] keyValue = pair.Split(':');
                        if (keyValue.Length == 2)
                        {
                            if (keyValue[0].Equals("Name"))
                            {
                                personName = keyValue[1];
                            }
                            else if (keyValue[0].Equals("Sport"))
                            {
                                sport = keyValue[1];
                            }
                            else if (keyValue[0].Equals("Date"))
                            {
                                date = keyValue[1];
                            }
                            else if (keyValue[0].Equals("Time"))
                            {
                                time = keyValue[1];
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

                    Debug.Log("Attempting to load Main scene.");

                    // Load the next scene with a delay
                    StartCoroutine(LoadSceneAfterDelay("Main", 1));
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
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
