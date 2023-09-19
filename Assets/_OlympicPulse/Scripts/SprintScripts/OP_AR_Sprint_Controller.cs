using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace _OlympicPulse.Scripts.SprintScripts
{
    public class OP_AR_Sprint_Controller : MonoBehaviour
    {
        public GameObject SprinterPrefab; 
        public GameObject FinishLinePrefab;
        public Transform ARSprinterSpawnPoint; 
        public Transform ARFinishLineSpawnPoint;
        
        private ARPlaneManager arPlaneManager;
        private ARRaycastManager arRaycastManager;
        
        private GameObject spawnedSprinter;
        private GameObject spawnedFinishLine;

        private void Awake()
        {
            arPlaneManager = GetComponent<ARPlaneManager>();
            arRaycastManager = GetComponent<ARRaycastManager>();
        }

        private void Update()
        {
            if (Input.touchCount == 0)
                return;

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
    }
}