using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

namespace _OlympicPulse.Scripts
{ 
    public class OP_Interactive_Map : MonoBehaviour
    {
        public ARPlaneManager arPlaneManager;
        public GameObject objectToPlace;
        public Button placeMapButton; // Reference to the button
        public TextMeshProUGUI buttonText; // Reference to the button's text component

        private GameObject _currentObjectInstance; // This will store our instantiated object

        private Vector2 _previousTouchDifference; // Stores the difference between two touch points from the previous frame

        private float _previousTouchDistance; // Stores distance between two touch points from the previous frame

        void Start()
        {
            // Initially, set the button as invisible
            placeMapButton.gameObject.SetActive(false);

            // Add a listener for the button click event
            placeMapButton.onClick.AddListener(ToggleMap);
        }

        void Update()
        {
            // Pinch to scale
            if (Input.touchCount == 2)
            {
                // Get current touch positions
                Touch touchZero = Input.touches[0];
                Touch touchOne = Input.touches[1];

                // Calculate touch difference for current and previous frame
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

                if (_currentObjectInstance != null)
                {
                    // Calculate the scale factor based on the pinch gesture
                    float pinchAmount = deltaMagnitudeDiff * 0.0001f;  // Scale factor

                    // Calculate the new scale
                    Vector3 newScale = _currentObjectInstance.transform.localScale +
                                       new Vector3(pinchAmount, pinchAmount, pinchAmount);

                    // Ensure the object doesn't become too small or too large
                    newScale = new Vector3(Mathf.Clamp(newScale.x, 0.01f, 1f), Mathf.Clamp(newScale.y, 0.01f, 1f),
                        Mathf.Clamp(newScale.z, 0.01f, 1f));

                    _currentObjectInstance.transform.localScale = newScale;
                }
            }
        }

        public void OnBackButtonPress()
        {
            LoadScene("PersonalisedWelcome");
        }
        
        public void ToggleMap()
        {
            Debug.Log("ToggleMap was called!");

            if (_currentObjectInstance == null)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                // Raycast from the centre of the screen
                Debug.Log("About to access arPlaneManager");
                if (arPlaneManager.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    Vector3 instantiatePosition = hitPose.position + new Vector3(0, objectToPlace.transform.localScale.y / 2, 0);
                    _currentObjectInstance = Instantiate(objectToPlace, instantiatePosition, Quaternion.identity);

                    Debug.Log($"Object instantiated at {_currentObjectInstance.transform.position} with scale {_currentObjectInstance.transform.localScale}");
                }
                else
                {
                    Debug.Log("No AR planes detected under the screen centre.");
                }
            }
            else
            {
                Debug.Log($"Destroying object at {_currentObjectInstance.transform.position}");
                Destroy(_currentObjectInstance);
                _currentObjectInstance = null;
            }

            buttonText.text = _currentObjectInstance == null ? "Place Map" : "Remove Map";
        }
        
        void OnPlaneAdded(ARPlanesChangedEventArgs args)
        {
            // If a plane is added, make the button visible
            if (args.added != null && args.added.Count > 0)
            {
                placeMapButton.gameObject.SetActive(true);
            }
        }

        void OnPlaneRemoved(ARPlanesChangedEventArgs args)
        {
            // If all planes are removed, make the button invisible
            if (arPlaneManager.trackables.count == 0)
            {
                placeMapButton.gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            // Subscribe to the planesChanged event
            arPlaneManager.planesChanged += OnPlaneAdded;
            arPlaneManager.planesChanged += OnPlaneRemoved;
        }

        void OnDisable()
        {
            // Unsubscribe from the planesChanged event
            arPlaneManager.planesChanged -= OnPlaneAdded;
            arPlaneManager.planesChanged -= OnPlaneRemoved;
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