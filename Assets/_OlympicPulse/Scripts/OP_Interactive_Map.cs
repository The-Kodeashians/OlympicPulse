using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class OP_Interactive_Map : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public GameObject objectToPlace;

    private GameObject currentObjectInstance; // This will store our instantiated object

    void Update()
    {
        // Check for screen touches
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        // If the map is not instantiated, instantiate it
        if (currentObjectInstance == null)
        {
            List<ARPlane> allPlanes = new List<ARPlane>();
            foreach (var plane in arPlaneManager.trackables)
            {
                allPlanes.Add(plane);
            }

            if (allPlanes.Count > 0) 
            {
                currentObjectInstance = Instantiate(objectToPlace, allPlanes[0].center, Quaternion.identity);
            }
        }
        // If the map is already instantiated, destroy it
        else
        {
            Destroy(currentObjectInstance);
            currentObjectInstance = null;
        }
    }

    void OnPlaneAdded(ARPlanesChangedEventArgs args)
    {
        // We won't automatically instantiate the object here anymore
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