using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts
{
    public class OP_AR_Sprint_Controller : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject SprinterPrefab;
        public GameObject FinishLinePrefab;
        public GameObject StartLinePrefab;

        [Header("Race Settings")]
        public float WorldRecordTime100m;
        public float RaceCountdownDuration = 3.0f;

        [Header("UI Elements")]
        public Button PlaceSprinterButton;
        public Button StartRaceButton;
        public TextMeshProUGUI CountdownText;
        
        private ARRaycastManager arRaycastManager;

        private GameObject spawnedSprinter;
        private GameObject spawnedFinishLine;
        private GameObject spawnedStartLine;
        private float raceTimer = 0.0f;
        private float raceCountdown = 0.0f;
        private float totalDistanceCovered = 0.0f;
        private bool raceEnded = false;

        private Vector3 initialPosition;

        private void Awake()
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            PlaceSprinterButton.onClick.AddListener(OnPlaceSprinterButtonPressed);
            StartRaceButton.onClick.AddListener(OnStartRaceButtonPressed);
            raceEnded = false;
        }

        private void Update()
        {
            if (raceCountdown > 0)
            {
                RunRaceCountdown();
            }
            else if (raceTimer > 0 && !raceEnded)
            {
                RunRace();
            }
        }

        private void RunRaceCountdown()
        {
            raceCountdown -= Time.deltaTime;
            if (CountdownText != null)
            {
                CountdownText.text = Mathf.Ceil(raceCountdown).ToString();
            }

            if (raceCountdown <= 1.0f && !spawnedSprinter.GetComponent<Actions>().IsAnimating("Run"))
            {
                if (spawnedSprinter)
                {
                    spawnedSprinter.GetComponent<Actions>().Run();
                }
            }

            if (raceCountdown <= 0)
            {
                if (CountdownText != null)
                {
                    CountdownText.text = "";
                }
                raceTimer = 0.01f;
            }
        }

        private void RunRace()
        {
            raceTimer += Time.deltaTime;
            UpdateRunnerPosition();
            CheckRaceCompletion();
        }

        private void OnPlaceSprinterButtonPressed()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SpawnSprinter(hitPose.position);
                initialPosition = spawnedSprinter.transform.position;
            }
        }

        private void SpawnSprinter(Vector3 position)
        {
            if (spawnedSprinter != null || spawnedFinishLine != null || spawnedStartLine != null)
            {
                Destroy(spawnedSprinter);
                Destroy(spawnedFinishLine);
                Destroy(spawnedStartLine);
            }

            // Get the camera's right direction but nullify any tilt in the Y-axis.
            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();  // Normalize to ensure it's a unit vector.

            // Set the sprinter's orientation.
            Quaternion sprinterOrientation = Quaternion.LookRotation(cameraRight);

            spawnedSprinter = Instantiate(SprinterPrefab, position, sprinterOrientation);

            // Spawn Finish Line 20 meters in front of the sprinter
            Vector3 finishLinePosition = position + (spawnedSprinter.transform.forward * 20.0f);
            spawnedFinishLine = Instantiate(FinishLinePrefab, finishLinePosition, sprinterOrientation);

            // Spawn Start Line 1 metre in front of the sprinter
            Vector3 startLinePosition = position + (spawnedSprinter.transform.forward * 1.0f);
            spawnedStartLine = Instantiate(StartLinePrefab, startLinePosition, sprinterOrientation);

            spawnedStartLine.transform.Rotate(0, 180, 0);
        }

        private void OnStartRaceButtonPressed()
        {
            if (spawnedSprinter == null)
            {
                Debug.LogError("Sprinter is null. Cannot start race.");
                return;
            }
            raceCountdown = RaceCountdownDuration;
            totalDistanceCovered = 0.0f;
            raceTimer = 0.0f;
            raceEnded = false;
        }

        private void UpdateRunnerPosition()
        {
            // Calculate time and speed for 20 metres based on the world record time for 100 metres
            float timeFor20m = (20.0f / 100.0f) * WorldRecordTime100m;
            float speedFor20m = 20.0f / timeFor20m;

            // Calculate the distance to move in this frame
            float distanceToMove = speedFor20m * Time.deltaTime;

            // Update the sprinter's position
            Vector3 newPosition = spawnedSprinter.transform.position + spawnedSprinter.transform.forward * distanceToMove;
            spawnedSprinter.transform.position = newPosition;
            
            // Update total distance covered
            totalDistanceCovered += distanceToMove;
        }

        private void CheckRaceCompletion()
        {
            if (totalDistanceCovered >= 20.0f && !raceEnded)  // Changed to 20 metres
            {
                EndRace();
            }
        }

        private void EndRace()
        {
            if (spawnedSprinter == null)
            {
                Debug.LogError("Sprinter is null. Cannot end race.");
                return;
            }
            spawnedSprinter.GetComponent<Actions>().Stay();
            raceEnded = true;
            raceTimer = 0;
        }
    }
}