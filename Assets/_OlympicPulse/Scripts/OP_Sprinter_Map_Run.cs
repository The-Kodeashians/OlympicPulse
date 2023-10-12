using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OP_Sprinter_Map_Run : MonoBehaviour
{
    public float distanceToRun = 10.0f; // Distance the sprinter will run in meters
    public float runningSpeed = 5.0f;  // Speed at which the sprinter runs
    public float stayDuration = 2.0f;  // Time sprinter will stay in seconds

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startingPosition = transform.position; // The position where the sprinter starts
        targetPosition = transform.position + transform.forward * distanceToRun; // Target position based on the distanceToRun
        StartCoroutine(RunAndStay());
    }

    private IEnumerator RunAndStay()
    {
        while (true)
        {
            // Running to the target
            GetComponent<Actions>().Run();
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, runningSpeed * Time.deltaTime);
                yield return null;
            }

            // Staying at the target
            GetComponent<Actions>().Stay();
            yield return new WaitForSeconds(stayDuration);

            // Turn around
            transform.rotation = Quaternion.Euler(0, 180, 0);

            // Running back to the start
            GetComponent<Actions>().Run();
            while (Vector3.Distance(transform.position, startingPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startingPosition, runningSpeed * Time.deltaTime);
                yield return null;
            }

            // Staying at the start
            GetComponent<Actions>().Stay();
            yield return new WaitForSeconds(stayDuration);

            // Turn around to original position
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
