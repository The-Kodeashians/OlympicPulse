using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace _OlympicPulse.Scripts
{
    public class OP_AR_Sprint_Controller : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject sprinterPrefab;
        public GameObject finishLinePrefab;
        public GameObject startLinePrefab;
        
        [Header("Race Settings")]
        public float worldRecordTime100M;
        public float raceCountdownDuration = 3.0f;

        [Header("UI Elements")]
        public Button placeSprinterButton;
        public Button startRaceButton;
        public TextMeshProUGUI countdownText;
        
        [Header("Screen Recorder")]
        public OP_Screen_Recorder screenRecorder;
        
        [Header("Audio")]
        public AudioSource countdownAudioSource;
        
        private ARRaycastManager _arRaycastManager;

        private GameObject _spawnedSprinter;
        private GameObject _spawnedFinishLine;
        private GameObject _spawnedStartLine;
        private float _raceTimer = 0.0f;
        private float _raceCountdown = 0.0f;
        private float _totalDistanceCovered = 0.0f;
        private bool _raceEnded = false;

        private Vector3 _initialPosition;

        private void Awake()
        {
            // Initialize world record time from PlayerPrefs
            if (PlayerPrefs.HasKey("SprinterWorldRecordTime100M"))
            {
                worldRecordTime100M = PlayerPrefs.GetFloat("SprinterWorldRecordTime100M");
            }
            
            _arRaycastManager = GetComponent<ARRaycastManager>();
            placeSprinterButton.onClick.AddListener(OnPlaceSprinterButtonPressed);
            startRaceButton.onClick.AddListener(OnStartRaceButtonPressed);
            _raceEnded = false;
        }

        private void Update()
        {
            if (_raceCountdown > 0)
            {
                RunRaceCountdown();
            }
            else if (_raceTimer > 0 && !_raceEnded)
            {
                RunRace();
            }
        }

        private void RunRaceCountdown()
        {
            _raceCountdown -= Time.deltaTime;
            int ceilCountdown = Mathf.CeilToInt(_raceCountdown);
            countdownText.text = ceilCountdown > 0 ? ceilCountdown.ToString() : "GO";

            switch (ceilCountdown)
            {
                case 3:
                    countdownText.color = Color.red;
                    break;
                case 2:
                    countdownText.color = Color.yellow;
                    break;
                case 1:
                    countdownText.color = Color.blue;
                    break;
                default:
                    countdownText.color = Color.green;
                    StartCoroutine(FadeText());
                    break;
            }

            if (_raceCountdown <= 1.0f && !_spawnedSprinter.GetComponent<Actions>().IsAnimating("Run"))
            {
                if (_spawnedSprinter)
                {
                    _spawnedSprinter.GetComponent<Actions>().Run();
                }
            }

            if (_raceCountdown <= 0)
            {
                _raceTimer = 0.01f;
            }
        }

        private void RunRace()
        {
            _raceTimer += Time.deltaTime;
            UpdateRunnerPosition();
            CheckRaceCompletion();
        }

        private void OnPlaceSprinterButtonPressed()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (_arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                SpawnSprinter(hitPose.position);
                _initialPosition = _spawnedSprinter.transform.position;
            }
        }

        private void SpawnSprinter(Vector3 position)
        {
            if (_spawnedSprinter != null || _spawnedFinishLine != null || _spawnedStartLine != null)
            {
                Destroy(_spawnedSprinter);
                Destroy(_spawnedFinishLine);
                Destroy(_spawnedStartLine);
            }

            // Get the camera's right direction but nullify any tilt in the Y-axis.
            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();  // Normalize to ensure it's a unit vector.

            // Set the sprinter's orientation.
            Quaternion sprinterOrientation = Quaternion.LookRotation(cameraRight);

            _spawnedSprinter = Instantiate(sprinterPrefab, position, sprinterOrientation);
            
            // Change the color of the spawned sprinter based on saved color in PlayerPrefs
            if (PlayerPrefs.HasKey("SprinterColorR") && PlayerPrefs.HasKey("SprinterColorG") &&
                PlayerPrefs.HasKey("SprinterColorB") && PlayerPrefs.HasKey("SprinterColorA"))
            {
                Color savedColor = new Color(
                    PlayerPrefs.GetFloat("SprinterColorR"),
                    PlayerPrefs.GetFloat("SprinterColorG"),
                    PlayerPrefs.GetFloat("SprinterColorB"),
                    PlayerPrefs.GetFloat("SprinterColorA")
                );

                // Apply the color to the spawned sprinter
                OP_Sprinter_Details details = _spawnedSprinter.GetComponent<OP_Sprinter_Details>();
                if (details != null)
                {
                    details.sprinterColor = savedColor;
                    details.ApplyColor();
                }
            }

            // Spawn Finish Line 20 meters in front of the sprinter
            Vector3 finishLinePosition = position + (_spawnedSprinter.transform.forward * 20.0f);
            _spawnedFinishLine = Instantiate(finishLinePrefab, finishLinePosition, sprinterOrientation);

            // Spawn Start Line 1 metre in front of the sprinter
            Vector3 startLinePosition = position + (_spawnedSprinter.transform.forward * 1.0f);
            _spawnedStartLine = Instantiate(startLinePrefab, startLinePosition, sprinterOrientation);

            _spawnedStartLine.transform.Rotate(0, 180, 0);
        }

        private void OnStartRaceButtonPressed()
        {
            if (_spawnedSprinter == null)
            {
                Debug.LogError("Sprinter is null. Cannot start race.");
                return;
            }
            
            // Play the countdown audio clip
            if (countdownAudioSource != null)
            {
                Debug.Log("Attempting to play audio.");  // Debug statement
                countdownAudioSource.Play();
                if(countdownAudioSource.isPlaying)
                {
                    Debug.Log("Audio is playing.");  // Debug statement
                }
                else
                {
                    Debug.LogError("Failed to play audio.");  // Debug statement
                }
            }
            else
            {
                Debug.LogError("AudioSource is not set.");  // Debug statement
            }
            
            _raceCountdown = raceCountdownDuration;
            _totalDistanceCovered = 0.0f;
            _raceTimer = 0.0f;
            _raceEnded = false;
            
            // Start the screen recording
            if (screenRecorder != null)
            {
                screenRecorder.StartRecording();
            }
        }

        private void UpdateRunnerPosition()
        {
            // Calculate time and speed for 20 metres based on the world record time for 100 metres
            float timeFor20m = (20.0f / 100.0f) * worldRecordTime100M;
            float speedFor20m = 20.0f / timeFor20m;

            // Calculate the distance to move in this frame
            float distanceToMove = speedFor20m * Time.deltaTime;

            // Update the sprinter's position
            Vector3 newPosition = _spawnedSprinter.transform.position + _spawnedSprinter.transform.forward * distanceToMove;
            _spawnedSprinter.transform.position = newPosition;
            
            // Update total distance covered
            _totalDistanceCovered += distanceToMove;
        }

        private void CheckRaceCompletion()
        {
            if (_totalDistanceCovered >= 20.0f && !_raceEnded)  // Changed to 20 metres
            {
                EndRace();
            }
        }

        private void EndRace()
        {
            if (_spawnedSprinter == null)
            {
                Debug.LogError("Sprinter is null. Cannot end race.");
                return;
            }
            _spawnedSprinter.GetComponent<Actions>().Stay();
            _raceEnded = true;
            _raceTimer = 0; 
            
            // Stop the screen recording and show preview
            if (screenRecorder != null)
            {
                screenRecorder.StopRecordingShowPreview();
            }
        }
        
        private IEnumerator FadeText()
        {
            for (float i = 1.0f; i >= 0; i -= Time.deltaTime)
            {
                countdownText.color = new Color(0, 1, 0, i);
                yield return null;
            }
        }
        
        public void BackButton()
        {
            LoadScene("SprintingSelect");
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