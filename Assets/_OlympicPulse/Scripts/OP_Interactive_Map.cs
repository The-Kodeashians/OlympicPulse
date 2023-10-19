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
    /// <summary>
    /// Manages the interactive map features in AR.
    /// This script is attached to the InteractiveMap prefab in the Main scene.
    /// </summary>
    public class OP_Interactive_Map : MonoBehaviour
    {
        /// <summary>
        /// Manages AR planes for the scene.
        /// </summary>
        public ARPlaneManager arPlaneManager;
        
        /// <summary>
        /// The object to be placed on the AR plane.
        /// </summary>
        public GameObject objectToPlace;
        
        /// <summary>
        /// Button that toggles map placement.
        /// </summary>
        public Button placeMapButton;
        
        /// <summary>
        /// Text displayed on the placeMapButton.
        /// </summary>
        public TextMeshProUGUI buttonText; 

        /// <summary>
        /// Instance of the object that is currently placed.
        /// </summary>
        private GameObject _currentObjectInstance;

        /// <summary>
        /// Stores the difference between two touch points from the previous frame.
        /// </summary>
        private Vector2 _previousTouchDifference;

        /// <summary>
        /// Stores distance between two touch points from the previous frame.
        /// </summary>
        private float _previousTouchDistance;

        /// <summary>
        /// Initialization logic for setting up AR and UI elements.
        /// </summary>
        void Start()
        {
            // Initially, set the button as invisible
            placeMapButton.gameObject.SetActive(false);

            // Add a listener for the button click event
            placeMapButton.onClick.AddListener(ToggleMap);
        }

        /// <summary>
        /// Update logic for handling user interactions and object scaling.
        /// </summary>
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

        /// <summary>
        /// Handler for the back button, returns the user to the PersonalisedWelcome scene.
        /// </summary>
        public void OnBackButtonPress()
        {
            LoadScene("PersonalisedWelcome");
        }
        
        /// <summary>
        /// Toggles the map between placed and not placed.
        /// </summary>
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
        
        /// <summary>
        /// Event handler for when an AR plane is added.
        /// </summary>
        /// <param name="args">Event arguments, which include the list of added planes.</param>
        void OnPlaneAdded(ARPlanesChangedEventArgs args)
        {
            // If a plane is added, make the button visible
            if (args.added != null && args.added.Count > 0)
            {
                placeMapButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Event handler for when an AR plane is removed.
        /// </summary>
        /// <param name="args">Event arguments, which include the list of removed planes.</param>
        void OnPlaneRemoved(ARPlanesChangedEventArgs args)
        {
            // If all planes are removed, make the button invisible
            if (arPlaneManager.trackables.count == 0)
            {
                placeMapButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Subscribes to relevant ARPlaneManager events when enabled.
        /// </summary>
        void OnEnable()
        {
            // Subscribe to the planesChanged event
            arPlaneManager.planesChanged += OnPlaneAdded;
            arPlaneManager.planesChanged += OnPlaneRemoved;
        }

        /// <summary>
        /// Unsubscribes from ARPlaneManager events when disabled.
        /// </summary>
        void OnDisable()
        {
            // Unsubscribe from the planesChanged event
            arPlaneManager.planesChanged -= OnPlaneAdded;
            arPlaneManager.planesChanged -= OnPlaneRemoved;
        }
        
        /// <summary>
        /// Loads a specified Unity scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
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
