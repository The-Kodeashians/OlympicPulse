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

        [Header("Race Settings")]
        public float WorldRecordPace;
        public float RaceCountdownDuration = 3.0f;

        [Header("UI Elements")]
        public Button PlaceSprinterButton;
        public Button StartRaceButton;
        public TextMeshProUGUI CountdownText;

        private ARPlaneManager arPlaneManager;
        private ARRaycastManager arRaycastManager;

        private GameObject spawnedSprinter;
        private float raceTimer = 0.0f;
        private float raceCountdown = 0.0f;
        private float totalDistanceCovered = 0.0f;
        private bool raceEnded = false;

        private Vector3 initialPosition;  // To keep track of the initial position of the sprinter

        private void Awake()
        {
            arPlaneManager = GetComponent<ARPlaneManager>();
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
            // Start the animation at 1 second remaining
            if (raceCountdown <= 1.0f && !spawnedSprinter.GetComponent<Actions>().IsAnimating("Run"))
            {
                if (spawnedSprinter)
                {
                    spawnedSprinter.GetComponent<Actions>().Run();
                }
            }
            // End the countdown and start the race
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
            Debug.Log($"Sprinter Position at RunRace(): {spawnedSprinter.transform.position} and at raceTimer: {raceTimer}");
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
                Debug.Log($"Sprinter Position after being placed: {initialPosition}");
            }
        }

        private void SpawnSprinter(Vector3 position)
        {
            if (spawnedSprinter != null)
            {
                Destroy(spawnedSprinter);
            }
            // Project the camera's forward vector onto the XZ-plane
            Vector3 projectedForward = Camera.main.transform.forward;
            projectedForward.y = 0.0f;
            projectedForward.Normalize();
    
            // Use the projected forward vector for the sprinter's orientation
            Quaternion sprinterOrientation = Quaternion.LookRotation(projectedForward);
            spawnedSprinter = Instantiate(SprinterPrefab, position, sprinterOrientation);
        }


        private void OnStartRaceButtonPressed()
        {
            if (spawnedSprinter == null)
            {
                Debug.LogError("Sprinter is null. Cannot start race.");
                return;
            }
            Debug.Log("Starting race countdown.");
            raceCountdown = RaceCountdownDuration;
            totalDistanceCovered = 0.0f;
            raceTimer = 0.0f;
            raceEnded = false;
            Debug.Log($"Sprinter Position at OnStartRaceButtonPressed(): {initialPosition}");
        }

        private void UpdateRunnerPosition()
        {
            float distanceToCover = 100.0f;
            float distanceCovered = WorldRecordPace * raceTimer;
            float fractionOfJourney = distanceCovered / distanceToCover;

            Vector3 endPosition = initialPosition + spawnedSprinter.transform.forward * 100.0f;
            Vector3 newPosition = Vector3.Lerp(initialPosition, endPosition, fractionOfJourney);

            spawnedSprinter.transform.position = newPosition;
            totalDistanceCovered = Vector3.Distance(initialPosition, newPosition);

            Debug.Log("New Position: " + newPosition);
        }

        private void CheckRaceCompletion()
        {
            Debug.Log("Total Distance Covered: " + totalDistanceCovered);

            if (totalDistanceCovered >= 100.0f && !raceEnded)
            {
                Debug.Log("Ending Race.");
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
            Debug.Log("Race ended. Sprinter should stop.");
            Debug.Log($"Is Sprinter Active: {spawnedSprinter.activeSelf}");
            Debug.Log($"Sprinter Position at Race End: {spawnedSprinter.transform.position}");

            spawnedSprinter.GetComponent<Actions>().Stay();
            raceEnded = true;
            raceTimer = 0;
        }
    }
}
