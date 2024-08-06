using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARSubsystems;

public class InputManager : MonoBehaviour
{
    public GameObject furniturePrefab;
    public Camera arCamera;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    // Start is called before the first frame update
    void Start()
    {
        EnhancedTouchSupport.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];
            
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.screenPosition);

                if (raycastManager.Raycast(ray, hits, 
                    UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose pose = hits[0].pose;
                    Instantiate(furniturePrefab, pose.position, pose.rotation);
                }
            }
        }

    }
}
