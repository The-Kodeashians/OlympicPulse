using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace _OlympicPulse.Scripts.SprintScripts
{
    public class OP_Sprint_Manager : MonoBehaviour
    {
        [FormerlySerializedAs("ARSprinter")] public GameObject ARSprinter; // Reference to runners' model
        public float WorldRecordPace; // Pace of the WR?? TODO: figure out how to keep up on WR pace
        public Transform StartPoint;
        public Transform FinishPoint;

        private enum RaceState
        {
            NotStarted,
            Running,
            Finished
        }

        private RaceState currentRaceState = RaceState.NotStarted;
        private float raceTimer = 0.0f;

        void Start()
        {
            // Initialization code here if needed
        }

        void Update()
        {
            if (currentRaceState == RaceState.Running)
            {
                raceTimer += Time.deltaTime;
                UpdateRunnerPosition();
                CheckRaceCompletion();
            }
        }

        public void InitializeRace()
        {
            // Setu p code for the race
        }

        public void StartRace()
        {
            currentRaceState = RaceState.Running;
            // Code to start the AR sprinters movement
            ARSprinter.GetComponent<OP_AR_Sprinter_Script>().StartRunning();
        }

        void UpdateRunnerPosition()
        {
            // Code to update the position of the AR sprinter based on the world record pace?? not sure
        }

        void CheckRaceCompletion()
        {
            // Check if the AR sprinter has reached the finish line
        }

        void EndRace()
        {
            currentRaceState = RaceState.Finished;
            ARSprinter.GetComponent<OP_AR_Sprinter_Script>().StopRunning();
            // Other end race actions
        }
    }
}