using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class EditMode : Mode
{
    public void BackToPlaneMode()
    {
        UIManager.instance.SetMode(ModeState.PlaneMode);
    }

    [Header("Ramp Properties")]
    public GameObject currentRamp;
    public LayerMask rampLayerMask;
    public LayerMask planeLayerMask;
    public TMP_Dropdown rampDropdown;

    [Header("Buttons")]
    public Button planeButton;
    public Button acceptButton;
    public GameObject rampPlaceButtons;
    public GameObject rampEditButtons;

    public bool isPlacingRamp;

    public AnimationCurve animationCurve;

    private void Start()
    {
        rampDropdown.ClearOptions();
        rampDropdown.AddOptions(RampManager.instance.GetRampNames());
        SetEditButtons(false);
        SetPlaceButtons(false);
    }

    public void OnStartPlacingRamp(int index)
    {
        currentRamp = RampManager.instance.GetAndActiveRamp(index);
        OnEditRamp();
    }

    public void OnEditRamp()
    {
        isPlacingRamp = true;
        SetEditButtons(false);
        SetPlaceButtons(true);
    }

    public void OnPlaceRamp()
    {
        isPlacingRamp = false;
        StartCoroutine(ScaleAndRemoveRamp(currentRamp, 0.75f));
        SetEditButtons(false);
        SetPlaceButtons(false);
        currentRamp = null;

    }

    public void OnRemoveRamp()
    {
        isPlacingRamp = false;
        SetEditButtons(false);
        SetPlaceButtons(false);
        currentRamp = null;
    }

    public void SetEditButtons(bool value)
    {
        rampEditButtons.SetActive(value);
        planeButton.interactable = !value;
        acceptButton.interactable = !value;
    }

    public void SetPlaceButtons(bool value)
    {
        rampPlaceButtons.SetActive(value);
        planeButton.interactable = !value;
        acceptButton.interactable = !value;
    }

    public void Update()
    {
        if(Input.touchCount<=0)
            return;
        
        Touch touch = Input.GetTouch(0);

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if(!isPlacingRamp && Physics.Raycast(ray, out hit, float.MaxValue, rampLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp = hit.transform.gameObject;
            SetEditButtons(true);
        }
        else if(currentRamp != null && !isPlacingRamp && Physics.Raycast(ray, out hit, float.MaxValue, ~rampLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp = null;
            SetEditButtons(false);
        }

        if(!isPlacingRamp)
        {
            return ;
        }

        DetectTouchMovement.Calculate();

        if(Physics.Raycast(ray, out hit, float.MaxValue, planeLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp.transform.position = hit.point;

            if(Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0)
            {
                Vector3 rotationDeg = Vector3.zero;
                rotationDeg.y = DetectTouchMovement.turnAngleDelta * 2;
                currentRamp.transform.rotation *= Quaternion.Euler(rotationDeg);
            }

        }
    }

    public IEnumerator ScaleAndRemoveRamp(GameObject ramp,float time)
    {
        float elaspedTime = 0;
        Vector3 startScale = ramp.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while(elaspedTime < time)
        {
            ramp.transform.localScale = Vector3.Lerp(startScale,targetScale,animationCurve.Evaluate(elaspedTime/time));
            elaspedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        RampManager.instance.DeactiveRamp(ramp);
    }
}
