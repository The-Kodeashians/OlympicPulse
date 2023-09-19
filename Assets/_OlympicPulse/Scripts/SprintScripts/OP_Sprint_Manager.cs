using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _OlympicPulse.Scripts.SprintScripts
{
    public class OP_Sprint_Manager : MonoBehaviour
    {
        public GameObject Sprinter; 
        public float WorldRecordPace; 
        public Transform StartPoint;
        public Transform FinishPoint;
        public Text countdownText; 

        private RaceState currentRaceState = RaceState.NotStarted;
        private float raceTimer = 0.0f;

        void Start()
        {
            if (countdownText)
            {
                countdownText.gameObject.SetActive(false);
            }
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
            StartCoroutine(CountdownAndStartRace());
        }

        private IEnumerator CountdownAndStartRace()
        {
            int countdown = 3;
            countdownText.gameObject.SetActive(true);

            while (countdown > 0)
            {
                countdownText.text = countdown.ToString();
                yield return new WaitForSeconds(1);
                countdown--;
            }

            countdownText.text = "GO!";
            yield return new WaitForSeconds(1);
            countdownText.gameObject.SetActive(false);

            StartRace();
        }

        private void StartRace()
        {
            currentRaceState = RaceState.Running;
            Sprinter.GetComponent<OP_Sprinter_Script>().StartRunning();
        }

        private void UpdateRunnerPosition()
        {
            float distanceToCover = Vector3.Distance(StartPoint.position, FinishPoint.position);
            float currentSpeed = WorldRecordPace; // may need work here

            float fractionOfJourney = raceTimer * currentSpeed / distanceToCover;
            Sprinter.transform.position = Vector3.Lerp(StartPoint.position, FinishPoint.position, fractionOfJourney);
        }

        private void CheckRaceCompletion()
        {
            if(Vector3.Distance(Sprinter.transform.position, FinishPoint.position) < 0.1f)
            {
                EndRace();
            }
        }

        private void EndRace()
        {
            currentRaceState = RaceState.Finished;
            Sprinter.GetComponent<OP_Sprinter_Script>().StopRunning();
            // Any other end race actions
        }
    }
}
