using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaneMode : Mode
{
    public GameObject sceneObject;

    private Anchor currentSceneAnchor;
    private Vector3 _previousAnchorPosition;

    [Header("References")]
    public Button acceptButton;

    public bool canRaycast = false;
    private bool _firstTime = true;

    public override void onModeEnabled()
    {
        base.onModeEnabled();
        PlaneManager.instance.SetDetectedPlaneVisualizerActive(true);
        canRaycast = true;
    }

    public override void onModeDisabled()
    {
        base.onModeDisabled();
        PlaneManager.instance.SetDetectedPlaneVisualizerActive(false);
        canRaycast = false;
    }


    // Start is called before the first frame update
    public void Start()
    {
        acceptButton.interactable = false;
        sceneObject.SetActive(false);
    }

    public void PlaneIsSelected()
    {
        acceptButton.interactable = true;
    }

    public void onAccept()
    {
        canRaycast = false;
        UIManager.instance.SetMode(ModeState.EditMode);
    }

    // Update is called once per frame
    public void Update()
    {
        if(!canRaycast || Input.touchCount <= 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        
        TrackableHit hit;
        TrackableHitFlags filter = TrackableHitFlags.PlaneWithinPolygon;

        if(Frame.Raycast(touch.position.x, touch.position.y, filter, out hit) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            if((hit.Trackable is DetectedPlane) && Vector3.Dot(Camera.main.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
                return;

            if((hit.Trackable is DetectedPlane) && ((DetectedPlane)hit.Trackable).PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
            {
                sceneObject.transform.position = hit.Pose.position;
                if(sceneObject.activeSelf == false)
                {
                    sceneObject.SetActive(true);
                }
                if(touch.phase == TouchPhase.Ended)
                {
                    PlaneIsSelected();
                    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    currentSceneAnchor = anchor;
                    sceneObject.transform.SetParent(anchor.transform);
                }
            }
        }
    }
}
