using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    public GameObject gameobjectToInstantiate;

    private GameObject parentObj;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPos;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();


    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool GetTouchPos(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            return true;
        }
        touchPos = default;
        return false;
    }


    void Update()
    {
        if (!GetTouchPos(out Vector2 touchPos))
        {
            return;
        }
        if (_arRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = hits[0].pose;

            if (parentObj == null)
            {
                parentObj=Instantiate(gameobjectToInstantiate, hitPos.position, hitPos.rotation);
                // I think it can also be done by Instantiate(gameobjectToInstantiate, hitPos.position, hitPos.rotation,parentObj.transform);
                //gameobjectToInstantiate.transform.parent = parentObj.transform;
            }
            else
                parentObj.transform.position = hitPos.position;
        }
    }
}
