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
        public TextMeshProUGUI CountdownText;

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
            if (currentRaceState == RaceState.Running)
            {
                if (raceCountdown > 0)
                {
                    raceCountdown -= Time.deltaTime;
                    if (CountdownText != null)
                    {
                        CountdownText.text = Mathf.Ceil(raceCountdown).ToString();
                    }
                    if (raceCountdown <= 0)
                    {
                        if (CountdownText != null) 
                        {
                            CountdownText.text = "";
                        }
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
                SpawnSprinter(hitPose.position);
                SpawnFinishLine(hitPose.position);
            }
            else
            {
                Debug.LogError("Raycasting failed");
            }
        }
        
        private void SpawnSprinter(Vector3 position)
        {
            if (spawnedSprinter != null)
            {
                Destroy(spawnedSprinter);
            }
            spawnedSprinter = Instantiate(SprinterPrefab, position, Quaternion.identity);
        }

        private void SpawnFinishLine(Vector3 position)
        {
            // Assuming the finish line is 100 metres ahead of the sprinter
            Vector3 finishLinePosition = position + Camera.main.transform.forward * 100;
            if (spawnedFinishLine != null)
            {
                Destroy(spawnedFinishLine);
            }
            spawnedFinishLine = Instantiate(FinishLinePrefab, finishLinePosition, Quaternion.identity);
        }

        private void OnStartRaceButtonPressed()
        {
            InitializeRace();
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
