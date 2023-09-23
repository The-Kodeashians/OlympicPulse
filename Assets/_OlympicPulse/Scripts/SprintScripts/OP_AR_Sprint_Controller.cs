using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts.SprintScripts
{
    public class OP_AR_Sprint_Controller : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject SprinterPrefab;
        public GameObject FinishLinePrefab;

        [Header("Spawn Points")]
        public Transform ARSprinterSpawnPoint;
        public Transform ARFinishLineSpawnPoint;

        [Header("Race Settings")]
        public float WorldRecordPace; // Pace of the WR (Distance over Time)
        public float RaceCountdownDuration = 3.0f; // Duration of the countdown before the race starts

        [Header("UI Elements")]
        public Button PlaceSprinterButton;
        public Button StartRaceButton;

        private ARPlaneManager arPlaneManager;
        private ARRaycastManager arRaycastManager;

        private GameObject spawnedSprinter;
        private GameObject spawnedFinishLine;

        private RaceState currentRaceState = RaceState.NotStarted;
        private float raceTimer = 0.0f;
        private float raceCountdown = 0.0f;

        private void Awake()
        {
            arPlaneManager = GetComponent<ARPlaneManager>();
            arRaycastManager = GetComponent<ARRaycastManager>();

            // Set up button listeners
            PlaceSprinterButton.onClick.AddListener(OnPlaceSprinterButtonPressed);
            StartRaceButton.onClick.AddListener(OnStartRaceButtonPressed);
        }

        private void Update()
        {
            if (currentRaceState == RaceState.NotStarted)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        List<ARRaycastHit> hits = new List<ARRaycastHit>();
                        if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                        {
                            Pose hitPose = hits[0].pose;
                            SpawnARObjects(hitPose);
                        }
                    }
                }
            }
            else if (currentRaceState == RaceState.Running)
            {
                if (raceCountdown > 0)
                {
                    raceCountdown -= Time.deltaTime;
                    if (raceCountdown <= 0)
                    {
                        // Start actual race here
                        if (spawnedSprinter)
                        {
                            spawnedSprinter.GetComponent<OP_Sprinter_Script>().StartRunning();
                        }
                    }
                }
                else
                {
                    raceTimer += Time.deltaTime;
                    UpdateRunnerPosition();
                    CheckRaceCompletion();
                }
            }
        }
        
        private void OnPlaceSprinterButtonPressed()
        {
            Debug.Log("OnPlaceSprinterButtonPressed called");

            if (arRaycastManager == null)
            {
                Debug.LogError("arRaycastManager is null");
                return;
            }

            // Raycast from the center of the camera view
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Check for null or uninitialized objects
                if (SprinterPrefab == null)
                {
                    Debug.LogError("SprinterPrefab is null");
                    return;
                }

                if (Camera.main == null)
                {
                    Debug.LogError("Main Camera is null");
                    return;
                }

                // If a sprinter already exists, destroy it
                if (spawnedSprinter != null)
                {
                    Destroy(spawnedSprinter);
                }

                // Offset to the right of the camera (assuming Y is up)
                Vector3 spawnPosition = hitPose.position + Camera.main.transform.right * 0.5f; // Offset by 0.5 meters to the right

                // Create a rotation that respects only the camera's forward direction
                Quaternion spawnRotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);

                // Instantiate the sprinter
                spawnedSprinter = Instantiate(SprinterPrefab, spawnPosition, spawnRotation);

                if (spawnedSprinter == null)
                {
                    Debug.LogError("Failed to spawn the sprinter");
                }
                else
                {
                    Debug.Log("Sprinter successfully spawned");
                }
            }
            else
            {
                Debug.LogError("Raycasting failed");
            }
        }
        
        private void OnStartRaceButtonPressed()
        {
            // Code to start the race countdown
            InitializeRace();
        }

        private void SpawnARObjects(Pose hitPose)
        {
            if (spawnedSprinter == null)
            {
                spawnedSprinter = Instantiate(SprinterPrefab, ARSprinterSpawnPoint.position, ARSprinterSpawnPoint.rotation);
            }
            else
            {
                spawnedSprinter.transform.position = ARSprinterSpawnPoint.position;
            }

            if (spawnedFinishLine == null)
            {
                spawnedFinishLine = Instantiate(FinishLinePrefab, ARFinishLineSpawnPoint.position, ARFinishLineSpawnPoint.rotation);
            }
            else
            {
                spawnedFinishLine.transform.position = ARFinishLineSpawnPoint.position;
            }
        }

        public void InitializeRace()
        {
            Debug.Log("InitializeRace called");
            if (spawnedSprinter && spawnedFinishLine)
            {
                raceCountdown = RaceCountdownDuration;
                currentRaceState = RaceState.Running;
            }
        }

        private void UpdateRunnerPosition()
        {
            float distanceToCover = Vector3.Distance(ARSprinterSpawnPoint.position, ARFinishLineSpawnPoint.position);
            float distanceCovered = WorldRecordPace * raceTimer;
            float fractionOfJourney = distanceCovered / distanceToCover;

            spawnedSprinter.transform.position = Vector3.Lerp(ARSprinterSpawnPoint.position, ARFinishLineSpawnPoint.position, fractionOfJourney);
        }

        private void CheckRaceCompletion()
        {
            if (Vector3.Distance(spawnedSprinter.transform.position, ARFinishLineSpawnPoint.position) < 0.1f) // Assuming 0.1 as a close enough threshold
            {
                EndRace();
            }
        }

        private void EndRace()
        {
            currentRaceState = RaceState.Finished;
            if (spawnedSprinter)
            {
                spawnedSprinter.GetComponent<OP_Sprinter_Script>().StopRunning();
            }
            // Other end race actions
        }
    }
}
