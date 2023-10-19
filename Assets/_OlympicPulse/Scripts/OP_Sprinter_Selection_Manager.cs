using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

namespace _OlympicPulse.Scripts
{
    /// <summary>
    /// Manages the selection of sprinters in the SprinterSelect scene.
    /// This script is intended to be attached to the SprinterSelectionManager prefab.
    /// </summary>
    public class OP_Sprinter_Selection_Manager : MonoBehaviour
    {
        /// <summary>
        /// Class to hold information about each selectable sprinter.
        /// </summary>
        [System.Serializable]
        public class SprinterInfo
        {
            public string name;
            public string details;
            public float worldRecordTime100M;
            public Color color;
        }

        public List<SprinterInfo> selectableSprinters = new List<SprinterInfo>();
        public GameObject sprinterPrefab;
        public GameObject spawnCentre;
        public float radius = 1.0f;
        public int selectedIndex = 0;
        public TextMeshProUGUI selectedSprinterDetailsText;
        public TextMeshProUGUI errorMessage;
        private Vector2 _touchStartPos;
        private Vector2 _touchEndPos;
        private bool _isRotating = false;
        

        private List<GameObject> _allSprinters = new List<GameObject>();

        /// <summary>
        /// Initializes the sprinter selection scene by populating and spawning the sprinters.
        /// </summary>
        private void Start()
        {
            PopulateSprinters();
            SpawnSprinters();
            HighlightSelectedSprinter();
        }

        /// <summary>
        /// Populates the list of selectable sprinters.
        /// </summary>
        private void PopulateSprinters()
        {
            selectableSprinters.Add(new SprinterInfo { name = "Busain Yolt", details = "Fastest person alive", worldRecordTime100M = 9.58f, color = Color.white });
            selectableSprinters.Add(new SprinterInfo { name = "Flo-Joked", details = "Unlock for 400 tokens.", worldRecordTime100M = 10.49f, color = Color.gray });
            selectableSprinters.Add(new SprinterInfo { name = "CatchMe Freedman", details = "Unlock for 350 tokens. Australian sprinting legend.", worldRecordTime100M = 11.11f, color = Color.gray });
            selectableSprinters.Add(new SprinterInfo { name = "Speedy Gonzales", details = "Fast but not the fastest", worldRecordTime100M = 20f, color = Color.magenta });
            selectableSprinters.Add(new SprinterInfo { name = "Hol' Up Jones", details = "Moderate speed", worldRecordTime100M = 30f, color = Color.cyan });
            selectableSprinters.Add(new SprinterInfo { name = "Slowpoke Sally", details = "Takes it easy", worldRecordTime100M = 40f, color = Color.yellow });
        }

        /// <summary>
        /// Spawns the sprinters around the center point.
        /// </summary>
        private void SpawnSprinters()
        {
            for (int i = 0; i < selectableSprinters.Count; i++)
            {
                float angle = (i * Mathf.PI * 2 / selectableSprinters.Count) + Mathf.PI / 2 - (selectedIndex * Mathf.PI * 2 / selectableSprinters.Count);
                Vector3 pos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                GameObject sprinter = Instantiate(sprinterPrefab, pos, Quaternion.identity);
                UpdateSprinterDetails(sprinter, i);
                _allSprinters.Add(sprinter);
            }
        }
        
        /// <summary>
        /// Handles the button press event for selecting a sprinter.
        /// </summary>
        public void SelectSprinterButton()
        {
            Debug.Log("SelectSprinterButton was called.");
            // Get the selected sprinter's information
            SprinterInfo selectedSprinter = selectableSprinters[selectedIndex];
            
            // Add a check here to see if the selected sprinter is one of the locked characters
            if (selectedSprinter.name == "CatchMe Freedman" || selectedSprinter.name == "Flo-Joked")
            {
                ShowError("You haven't unlocked this sprinter yet.");
                return;
            }

            // Save the selected sprinter's details to PlayerPrefs
            PlayerPrefs.SetString("SprinterName", selectedSprinter.name);
            PlayerPrefs.SetString("SprinterDetails", selectedSprinter.details);
            PlayerPrefs.SetFloat("SprinterWorldRecordTime100M", selectedSprinter.worldRecordTime100M);
            PlayerPrefs.SetFloat("SprinterColorR", selectedSprinter.color.r);
            PlayerPrefs.SetFloat("SprinterColorG", selectedSprinter.color.g);
            PlayerPrefs.SetFloat("SprinterColorB", selectedSprinter.color.b);
            PlayerPrefs.SetFloat("SprinterColorA", selectedSprinter.color.a);
            
            PlayerPrefs.Save();
            
            LoadScene("Sprinting");
        }
        
        /// <summary>
        /// Shows an error message and fades it out after a duration.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        private void ShowError(string message)
        {
            errorMessage.text = message;
            errorMessage.color = new Color(errorMessage.color.r, errorMessage.color.g, errorMessage.color.b, 1); // Set alpha to 1
            StartCoroutine(FadeOutError(4.0f));
        }

        /// <summary>
        /// Fades out the error message over a given duration.
        /// </summary>
        /// <param name="duration">The duration for the fade out.</param>
        /// <returns>An IEnumerator to manage the coroutine.</returns>
        private IEnumerator FadeOutError(float duration)
        {
            float startTime = Time.time;
            while (Time.time - startTime <= duration)
            {
                float t = (Time.time - startTime) / duration;
                errorMessage.color = new Color(errorMessage.color.r, errorMessage.color.g, errorMessage.color.b, Mathf.Lerp(1, 0, t));
                yield return null;
            }
            errorMessage.color = new Color(errorMessage.color.r, errorMessage.color.g, errorMessage.color.b, 0); // Ensure alpha is 0
            errorMessage.text = "";
        }
        
        /// <summary>
        /// Loads the Main scene when the Back button is pressed.
        /// </summary>
        public void BackButton()
        {
            LoadScene("Main");
        }

        /// <summary>
        /// Updates the UI and logic for sprinter selection.
        /// </summary>
        private void Update()
        {
            if (!_isRotating)
            {
                // Detect swipe using touch
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        _touchStartPos = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        _touchEndPos = touch.position;
                        float swipeDistance = Vector2.Distance(_touchStartPos, _touchEndPos);

                        // Only rotate if the swipe distance is greater than a threshold
                        if (swipeDistance > 50)  // Can try different distances here
                        {
                            int direction = (_touchEndPos.x > _touchStartPos.x) ? 1 : -1;
                            StartCoroutine(RotateRingCoroutine(direction));
                        }
                    }
                }
                // Keyboard controls for testing
                else if (Input.GetKeyDown(KeyCode.Z)) // Swipe left
                {
                    StartCoroutine(RotateRingCoroutine(1));
                }
                else if (Input.GetKeyDown(KeyCode.X)) // Swipe right
                {
                    StartCoroutine(RotateRingCoroutine(-1));
                }
            }
        }

        /// <summary>
        /// Coroutine to rotate the ring of sprinters.
        /// </summary>
        /// <param name="direction">The direction of rotation. +1 for clockwise, -1 for counter-clockwise.</param>
        /// <returns>An IEnumerator to manage the coroutine.</returns>
        private IEnumerator RotateRingCoroutine(int direction)
        {
            if (_isRotating)
                yield break;

            _isRotating = true;

            float duration = 1.0f;
            float elapsed = 0.0f;

            List<Vector3> initialPositions = new List<Vector3>();
            foreach (var sprinter in _allSprinters)
            {
                initialPositions.Add(sprinter.transform.position);
            }

            selectedIndex = (selectedIndex - direction + selectableSprinters.Count) % selectableSprinters.Count;

            List<Vector3> targetPositions = new List<Vector3>();
            for (int i = 0; i < _allSprinters.Count; i++)
            {
                float angle = (i * Mathf.PI * 2 / selectableSprinters.Count) + Mathf.PI / 2 - (selectedIndex * Mathf.PI * 2 / selectableSprinters.Count);
                Vector3 targetPos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                targetPositions.Add(targetPos);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                for (int i = 0; i < _allSprinters.Count; i++)
                {
                    _allSprinters[i].transform.position = Vector3.Lerp(initialPositions[i], targetPositions[i], t);
                }

                yield return null;
            }

            for (int i = 0; i < _allSprinters.Count; i++)
            {
                _allSprinters[i].transform.position = targetPositions[i];
            }

            HighlightSelectedSprinter();
            _isRotating = false;
        }

        /// <summary>
        /// Updates the details of the spawned sprinter GameObject.
        /// </summary>
        /// <param name="sprinter">The GameObject representing the sprinter.</param>
        /// <param name="index">The index of the sprinter in the selectableSprinters list.</param>
        private void UpdateSprinterDetails(GameObject sprinter, int index)
        {
            OP_Sprinter_Details details = sprinter.GetComponent<OP_Sprinter_Details>();
            SprinterInfo info = selectableSprinters[index];
            details.sprinterName = info.name;
            details.sprinterDetails = info.details;
            details.worldRecordTime100M = info.worldRecordTime100M;
            details.sprinterColor = info.color;
            details.ApplyColor();
        }

        /// <summary>
        /// Highlights the currently selected sprinter and updates the details text.
        /// </summary>
        private void HighlightSelectedSprinter()
        {
            for (int i = 0; i < _allSprinters.Count; i++)
            {
                if (i == selectedIndex)
                {
                    _allSprinters[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    SprinterInfo selectedSprinter = selectableSprinters[selectedIndex];
                    selectedSprinterDetailsText.text = $"Name: {selectedSprinter.name}\n" +
                                               $"Record for 100m: {selectedSprinter.worldRecordTime100M}s\n" +
                                               $"Bio: {selectedSprinter.details}";
                }
                else
                {
                    _allSprinters[i].transform.localScale = Vector3.one;
                }
            }
        }
        
        /// <summary>
        /// Load a given scene by its name.
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
