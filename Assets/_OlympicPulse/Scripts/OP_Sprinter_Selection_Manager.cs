using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _OlympicPulse.Scripts
{
    public class OP_Sprinter_Selection_Manager : MonoBehaviour
    {
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
        private Vector2 touchStartPos;
        private Vector2 touchEndPos;
        private bool isRotating = false;

        private List<GameObject> allSprinters = new List<GameObject>();

        private void Start()
        {
            PopulateSprinters();
            SpawnSprinters();
            HighlightSelectedSprinter();
        }

        private void PopulateSprinters()
        {
            selectableSprinters.Add(new SprinterInfo { name = "Busain Yolt", details = "Fastest man alive", worldRecordTime100M = 9.58f, color = Color.white });
            selectableSprinters.Add(new SprinterInfo { name = "Speedy Gonzales", details = "Fast but not the fastest", worldRecordTime100M = 20f, color = Color.magenta });
            selectableSprinters.Add(new SprinterInfo { name = "Hol' Up Jones", details = "Moderate speed", worldRecordTime100M = 30f, color = Color.cyan });
            selectableSprinters.Add(new SprinterInfo { name = "Slowpoke", details = "Takes it easy", worldRecordTime100M = 40f, color = Color.yellow });
        }

        private void SpawnSprinters()
        {
            for (int i = 0; i < selectableSprinters.Count; i++)
            {
                float angle = (i * Mathf.PI * 2 / selectableSprinters.Count) + Mathf.PI / 2 - (selectedIndex * Mathf.PI * 2 / selectableSprinters.Count);
                Vector3 pos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                GameObject sprinter = Instantiate(sprinterPrefab, pos, Quaternion.identity);
                UpdateSprinterDetails(sprinter, i);
                allSprinters.Add(sprinter);
            }
        }

        private void Update()
        {
            if (!isRotating)
            {
                // Detect swipe using touch
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchStartPos = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        touchEndPos = touch.position;

                        int direction = (touchEndPos.x > touchStartPos.x) ? 1 : -1;
                        StartCoroutine(RotateRingCoroutine(direction));
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

        private IEnumerator RotateRingCoroutine(int direction)
        {
            if (isRotating)
                yield break;

            isRotating = true;

            float duration = 1.0f;
            float elapsed = 0.0f;

            List<Vector3> initialPositions = new List<Vector3>();
            foreach (var sprinter in allSprinters)
            {
                initialPositions.Add(sprinter.transform.position);
            }

            selectedIndex = (selectedIndex - direction + selectableSprinters.Count) % selectableSprinters.Count;

            List<Vector3> targetPositions = new List<Vector3>();
            for (int i = 0; i < allSprinters.Count; i++)
            {
                float angle = (i * Mathf.PI * 2 / selectableSprinters.Count) + Mathf.PI / 2 - (selectedIndex * Mathf.PI * 2 / selectableSprinters.Count);
                Vector3 targetPos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                targetPositions.Add(targetPos);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                for (int i = 0; i < allSprinters.Count; i++)
                {
                    allSprinters[i].transform.position = Vector3.Lerp(initialPositions[i], targetPositions[i], t);
                }

                yield return null;
            }

            for (int i = 0; i < allSprinters.Count; i++)
            {
                allSprinters[i].transform.position = targetPositions[i];
            }

            HighlightSelectedSprinter();
            isRotating = false;
        }

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

        private void HighlightSelectedSprinter()
        {
            for (int i = 0; i < allSprinters.Count; i++)
            {
                if (i == selectedIndex)
                {
                    allSprinters[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                }
                else
                {
                    allSprinters[i].transform.localScale = Vector3.one;
                }
            }
        }
    }
}