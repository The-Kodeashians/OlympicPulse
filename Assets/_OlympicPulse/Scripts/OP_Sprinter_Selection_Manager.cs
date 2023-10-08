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

        private List<GameObject> allSprinters = new List<GameObject>();

        private void Start()
        {
            // Populate list programatically
            selectableSprinters.Add(new SprinterInfo { name = "Busain Yolt", details = "Fastest man alive", worldRecordTime100M = 9.58f, color = Color.yellow });
            selectableSprinters.Add(new SprinterInfo { name = "Speedy Gonzales", details = "Fast but not the fastest", worldRecordTime100M = 20f, color = Color.green });
            selectableSprinters.Add(new SprinterInfo { name = "Moderate", details = "Moderate speed", worldRecordTime100M = 30f, color = Color.blue });
            selectableSprinters.Add(new SprinterInfo { name = "Slowpoke", details = "Takes it easy", worldRecordTime100M = 40f, color = Color.red });

            // Spawn and display all characters
            for (int i = 0; i < selectableSprinters.Count; i++)
            {
                float angle = i * Mathf.PI * 2 / selectableSprinters.Count;
                Vector3 pos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;  // Use SpawnCenter's position here
                GameObject sprinter = Instantiate(sprinterPrefab, pos, Quaternion.identity);
                UpdateSprinterDetails(sprinter, i);
                allSprinters.Add(sprinter);
            }


            // Initialize camera and highlighted character
            HighlightSelectedSprinter();
        }

        private void Update()
        {
            // Detect swipe using touch
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        break;

                    case TouchPhase.Ended:
                        touchEndPos = touch.position;

                        if (touchEndPos.x > touchStartPos.x)
                        {
                            selectedIndex = (selectedIndex - 1 + selectableSprinters.Count) % selectableSprinters.Count;
                        }
                        else if (touchEndPos.x < touchStartPos.x)
                        {
                            selectedIndex = (selectedIndex + 1) % selectableSprinters.Count;
                        }
                        
                        HighlightSelectedSprinter();
                        RotateRing();
                        break;
                }
            }
            // Keyboard controls for testing
            if (Input.GetKeyDown(KeyCode.Z)) // Swipe left
            {
                selectedIndex = (selectedIndex - 1 + selectableSprinters.Count) % selectableSprinters.Count;
                HighlightSelectedSprinter();
                RotateRing();
            }
            if (Input.GetKeyDown(KeyCode.X)) // Swipe right
            {
                selectedIndex = (selectedIndex + 1) % selectableSprinters.Count;
                HighlightSelectedSprinter();
                RotateRing();
            }
        }
        
        private void RotateRing()
        {
            for (int i = 0; i < allSprinters.Count; i++)
            {
                float angle = (i - selectedIndex) * Mathf.PI * 2 / selectableSprinters.Count;
                Vector3 targetPos = spawnCentre.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                
                // Use Vector3.Lerp for smooth transition
                allSprinters[i].transform.position = Vector3.Lerp(allSprinters[i].transform.position, targetPos, Time.deltaTime * 5);
            }
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