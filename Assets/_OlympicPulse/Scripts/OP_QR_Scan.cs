using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;

namespace _OlympicPulse.Scripts
{
    public class OP_QR_SCAN : MonoBehaviour
    {
        private WebCamTexture camTexture;
        private Rect screenRect;

        // Ticket infoz
        public string personName;
        public string sport;
        public string date;
        public string time;

        void Start()
        {
            // Initialise webcam
            screenRect = new Rect(0, 0, Screen.width, Screen.height);
            camTexture = new WebCamTexture();
            camTexture.requestedHeight = Screen.height;
            camTexture.requestedWidth = Screen.width;
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
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }
}