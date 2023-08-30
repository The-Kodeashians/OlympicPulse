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
        private WebCamTexture camTexture;
        private Rect screenRect;

        // Ticket info
        public string personName;
        public string sport;
        public string date;
        public string time;

        void Start()
        {
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

            // Initialise webcam
            screenRect = new Rect(0, 0, Screen.width, Screen.height);
            camTexture = new WebCamTexture();

            // Specify a lower resolution if needed
            camTexture.requestedHeight = 480; // lower value
            camTexture.requestedWidth = 640;  // lower value

            if (camTexture != null)
            {
                camTexture.Play();
            }
        }

        void OnGUI()
        {
            // Draw the camera background for the scanner
            GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);

            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                // Decode the current frame
                var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
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
                    if (camTexture != null)
                    {
                        camTexture.Stop();
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
            catch (System.Exception ex)
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

        void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
        }
    }
}
