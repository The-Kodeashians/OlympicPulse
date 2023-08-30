using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class OP_Interactive_Map : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public GameObject objectToPlace;

    // This method is called whenever a new plane is detected
    void OnPlaneAdded(ARPlanesChangedEventArgs args)
    {
        foreach (var newPlane in args.added)
        {
            // Instantiate objectToPlace at the centre of the detected plane
            Instantiate(objectToPlace, newPlane.center, Quaternion.identity);
        }
    }

    void OnEnable()
    {
        // Subscribe to the planeAdded event
        arPlaneManager.planesChanged += OnPlaneAdded;
    }

    void OnDisable()
    {
        // Unsubscribe from the planeAdded event
        arPlaneManager.planesChanged -= OnPlaneAdded;
    }
}